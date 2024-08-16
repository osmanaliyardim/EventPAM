﻿using Ardalis.GuardClauses;
using EventPAM.BuildingBlocks.Core.Event;
using EventPAM.BuildingBlocks.Core.Model;
using EventPAM.BuildingBlocks.EFCore;
using EventPAM.BuildingBlocks.Mongo;
using EventPAM.BuildingBlocks.PersistMessageProcessor;
using EventPAM.BuildingBlocks.Web;
using EasyNetQ.Management.Client;
using Grpc.Net.Client;
using MassTransit;
using MassTransit.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using NSubstitute;
using Respawn;
using Serilog;
using Xunit;
using Xunit.Abstractions;
using ILogger = Serilog.ILogger;
using System.Security.Claims;
using WebMotions.Fake.Authentication.JwtBearer;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Testcontainers.MongoDb;
using Testcontainers.EventStoreDb;
using System.Net;
using System.Globalization;
using Npgsql;

namespace EventPAM.BuildingBlocks.TestBase;

public class TestFixture<TEntryPoint> : IAsyncLifetime
    where TEntryPoint : class
{
    private readonly WebApplicationFactory<TEntryPoint> _factory;

    private static int Timeout => 120; // Second

    private ITestHarness TestHarness => ServiceProvider?.GetTestHarness()!;

    private Action<IServiceCollection> TestRegistrationServices { get; set; } = default!;

    private PostgreSqlContainer PostgresTestcontainer = default!;
    private PostgreSqlContainer PostgresPersistTestContainer = default!;
    public RabbitMqContainer RabbitMqTestContainer = default!;
    public MongoDbContainer MongoDbTestContainer = default!;
    public EventStoreDbContainer EventStoreDbTestContainer = default!;
    public CancellationTokenSource CancellationTokenSource = default!;

    public PersistMessageBackgroundService PersistMessageBackgroundService =>
        ServiceProvider.GetRequiredService<PersistMessageBackgroundService>();

    public HttpClient HttpClient
    {
        get
        {
            var claims =
                new Dictionary<string, object>
                {
                    { ClaimTypes.Name, "test@sample.com" },
                    { ClaimTypes.Role, "admin" },
                    {"scope", "event-api"}
                };

            var httpClient = _factory?.CreateClient();
            httpClient.SetFakeBearerToken(claims);

            return httpClient!;
        }
    }

    public GrpcChannel Channel =>
        GrpcChannel.ForAddress(HttpClient.BaseAddress!, new GrpcChannelOptions { HttpClient = HttpClient });

    public IServiceProvider ServiceProvider => _factory?.Services!;

    public IConfiguration Configuration => _factory?.Services.GetRequiredService<IConfiguration>()!;

    public ILogger Logger { get; set; } = default!;

    protected TestFixture()
    {
        _factory = new WebApplicationFactory<TEntryPoint>()
            .WithWebHostBuilder(builder =>
            {
                //builder.ConfigureAppConfiguration(AddCustomAppSettings);

                builder.UseEnvironment("tests");
                builder.ConfigureServices(services =>
                {
                    TestRegistrationServices?.Invoke(services);
                    services.ReplaceSingleton(AddHttpContextAccessorMock);

                    services.AddSingleton<PersistMessageBackgroundService>();

                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = FakeJwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = FakeJwtBearerDefaults.AuthenticationScheme;
                    }).AddFakeJwtBearer();
                });
            });
    }

    public async Task InitializeAsync()
    {
        CancellationTokenSource = new CancellationTokenSource();
        await StartTestContainerAsync();
    }

    public async Task DisposeAsync()
    {
        await StopTestContainerAsync();
        await _factory.DisposeAsync();
        CancellationTokenSource.Cancel();
    }

    public virtual void RegisterServices(Action<IServiceCollection> services)
    {
        TestRegistrationServices += services;
    }

    public ILogger CreateLogger(ITestOutputHelper output)
    {
        if (output != null)
        {
            return new LoggerConfiguration()
                .WriteTo.TestOutput(output)
                .CreateLogger();
        }

        return null!;
    }

    protected async Task ExecuteScopeAsync(Func<IServiceProvider, Task> action)
    {
        using var scope = ServiceProvider.CreateScope();
        await action(scope.ServiceProvider);
    }

    protected async Task<T> ExecuteScopeAsync<T>(Func<IServiceProvider, Task<T>> action)
    {
        using var scope = ServiceProvider.CreateScope();

        var result = await action(scope.ServiceProvider);

        return result;
    }

    public Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {
        return ExecuteScopeAsync(sp =>
        {
            var mediator = sp.GetRequiredService<IMediator>();

            return mediator.Send(request);
        });
    }

    public Task SendAsync(IRequest request)
    {
        return ExecuteScopeAsync(sp =>
        {
            var mediator = sp.GetRequiredService<IMediator>();
            return mediator.Send(request);
        });
    }

    public async Task Publish<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage : class, IEvent
    {
        await TestHarness.Bus.Publish(message, cancellationToken);
    }

    public async Task<bool> WaitForPublishing<TMessage>(CancellationToken cancellationToken = default)
        where TMessage : class, IEvent
    {
        var result = await WaitUntilConditionMet(async () =>
        {
            var published = await TestHarness.Published.Any<TMessage>(cancellationToken);
            var faulty = await TestHarness.Published.Any<Fault<TMessage>>(cancellationToken);
            
            return published && faulty == false;
        });
        
        return result;
    }

    public async Task<bool> WaitForConsuming<TMessage>(CancellationToken cancellationToken = default)
        where TMessage : class, IEvent
    {
        var result = await WaitUntilConditionMet(async () =>
        {
            var consumed = await TestHarness.Consumed.Any<TMessage>(cancellationToken);
            var faulty = await TestHarness.Consumed.Any<Fault<TMessage>>(cancellationToken);

            return consumed && faulty == false;
        });

        return result;
    }

    public async Task<bool> ShouldProcessedPersistInternalCommand<TInternalCommand>(
        CancellationToken cancellationToken = default)
        where TInternalCommand : class, IInternalCommand
    {
        var result = await WaitUntilConditionMet(async () =>
        {
            return await ExecuteScopeAsync(async sp =>
            {
                var persistMessageProcessor = sp.GetService<IPersistMessageProcessor>();
                Guard.Against.Null(persistMessageProcessor, nameof(persistMessageProcessor));

                var filter = await persistMessageProcessor.GetByFilterAsync(x =>
                    x.DeliveryType == MessageDeliveryType.Internal &&
                    typeof(TInternalCommand).ToString() == x.DataType);

                var res = filter.Any(x => x.MessageStatus == MessageStatus.Processed);

                return res;
            });
        });

        return result;
    }

    private static async Task<bool> WaitUntilConditionMet(Func<Task<bool>> conditionToMet, int? timeoutSecond = null)
    {
        var time = timeoutSecond ?? Timeout;

        var startTime = DateTime.Now;
        var timeoutExpired = false;
        var meet = await conditionToMet.Invoke();
        while (!meet)
        {
            if (timeoutExpired)
            {
                return false;
            }

            await Task.Delay(100);
            meet = await conditionToMet.Invoke();
            timeoutExpired = DateTime.Now - startTime > TimeSpan.FromSeconds(time);
        }

        return true;
    }

    private async Task StartTestContainerAsync()
    {
        PostgresTestcontainer = TestContainers.PostgresTestContainer();
        PostgresPersistTestContainer = TestContainers.PostgresPersistTestContainer();
        RabbitMqTestContainer = TestContainers.RabbitMqTestContainer();
        MongoDbTestContainer = TestContainers.MongoTestContainer();
        EventStoreDbTestContainer = TestContainers.EventStoreTestContainer();

        await MongoDbTestContainer.StartAsync();
        await PostgresTestcontainer.StartAsync();
        await PostgresPersistTestContainer.StartAsync();
        await RabbitMqTestContainer.StartAsync();
        await EventStoreDbTestContainer.StartAsync();
    }

    private async Task StopTestContainerAsync()
    {
        await PostgresTestcontainer.StopAsync();
        await PostgresPersistTestContainer.StopAsync();
        await RabbitMqTestContainer.StopAsync();
        await MongoDbTestContainer.StopAsync();
        await EventStoreDbTestContainer.StopAsync();
    }

    private void AddCustomAppSettings(IConfigurationBuilder configuration)
    {
        configuration.AddInMemoryCollection(new KeyValuePair<string, string>[]
        {
            new(Configs.POSTGRES_CONFIG, PostgresTestcontainer.GetConnectionString()),
            new(Configs.PMBS_CONFIG, PostgresPersistTestContainer.GetConnectionString()),
            new(Configs.RABBITMQ_HOST, RabbitMqTestContainer.Hostname),
            new(Configs.RABBITMQ_USERNAME, TestContainers.RabbitMqContainerConfiguration.UserName),
            new(Configs.RABBITMQ_PASS, TestContainers.RabbitMqContainerConfiguration.Password), new(
                Configs.RABBITMQ_PORT,
                RabbitMqTestContainer.GetMappedPublicPort(TestContainers.RabbitMqContainerConfiguration.Port)
                    .ToString(NumberFormatInfo.InvariantInfo)),
            new(Configs.MONGO_CONFIG, MongoDbTestContainer.GetConnectionString()),
            new(Configs.MONGO_NAME, TestContainers.MongoContainerConfiguration.Name),
            new(Configs.EVENTSTORE_CONFIG, EventStoreDbTestContainer.GetConnectionString())
        }!);
    }

    private IHttpContextAccessor AddHttpContextAccessorMock(IServiceProvider serviceProvider)
    {
        var httpContextAccessorMock = Substitute.For<IHttpContextAccessor>();
        using var scope = serviceProvider.CreateScope();
        httpContextAccessorMock.HttpContext = new DefaultHttpContext { RequestServices = scope.ServiceProvider };

        httpContextAccessorMock.HttpContext.Request.Host = new HostString("localhost", 6012);
        httpContextAccessorMock.HttpContext.Request.Scheme = "http";

        return httpContextAccessorMock;
    }
}

public class TestWriteFixture<TEntryPoint, TWContext> : TestFixture<TEntryPoint>
    where TEntryPoint : class
    where TWContext : DbContext
{
    public Task ExecuteDbContextAsync(Func<TWContext, Task> action)
    {
        return ExecuteScopeAsync(sp => action(sp.GetService<TWContext>()!));
    }

    public Task ExecuteDbContextAsync(Func<TWContext, ValueTask> action)
    {
        return ExecuteScopeAsync(sp => action(sp.GetService<TWContext>()!).AsTask());
    }

    public Task ExecuteDbContextAsync(Func<TWContext, IMediator, Task> action)
    {
        return ExecuteScopeAsync(sp => action(sp.GetService<TWContext>()!, sp.GetService<IMediator>()!));
    }

    public Task<T> ExecuteDbContextAsync<T>(Func<TWContext, Task<T>> action)
    {
        return ExecuteScopeAsync(sp => action(sp.GetService<TWContext>()!));
    }

    public Task<T> ExecuteDbContextAsync<T>(Func<TWContext, ValueTask<T>> action)
    {
        return ExecuteScopeAsync(sp => action(sp.GetService<TWContext>()!).AsTask());
    }

    public Task<T> ExecuteDbContextAsync<T>(Func<TWContext, IMediator, Task<T>> action)
    {
        return ExecuteScopeAsync(sp => action(sp.GetService<TWContext>()!, sp.GetService<IMediator>()!));
    }

    public Task InsertAsync<T>(params T[] entities) where T : class
    {
        return ExecuteDbContextAsync(db =>
        {
            foreach (var entity in entities)
            {
                db.Set<T>().Add(entity);
            }

            return db.SaveChangesAsync();
        });
    }

    public async Task InsertAsync<TEntity>(TEntity entity) where TEntity : class
    {
        await ExecuteDbContextAsync(db =>
        {
            db.Set<TEntity>().Add(entity);

            return db.SaveChangesAsync();
        });
    }

    public Task InsertAsync<TEntity, TEntity2>(TEntity entity, TEntity2 entity2)
        where TEntity : class
        where TEntity2 : class
    {
        return ExecuteDbContextAsync(db =>
        {
            db.Set<TEntity>().Add(entity);
            db.Set<TEntity2>().Add(entity2);

            return db.SaveChangesAsync();
        });
    }

    public Task InsertAsync<TEntity, TEntity2, TEntity3>(TEntity entity, TEntity2 entity2, TEntity3 entity3)
        where TEntity : class
        where TEntity2 : class
        where TEntity3 : class
    {
        return ExecuteDbContextAsync(db =>
        {
            db.Set<TEntity>().Add(entity);
            db.Set<TEntity2>().Add(entity2);
            db.Set<TEntity3>().Add(entity3);

            return db.SaveChangesAsync();
        });
    }

    public Task InsertAsync<TEntity, TEntity2, TEntity3, TEntity4>(TEntity entity, TEntity2 entity2, TEntity3 entity3,
        TEntity4 entity4)
        where TEntity : class
        where TEntity2 : class
        where TEntity3 : class
        where TEntity4 : class
    {
        return ExecuteDbContextAsync(db =>
        {
            db.Set<TEntity>().Add(entity);
            db.Set<TEntity2>().Add(entity2);
            db.Set<TEntity3>().Add(entity3);
            db.Set<TEntity4>().Add(entity4);

            return db.SaveChangesAsync();
        });
    }

    public Task<T> FindAsync<T, TKey>(TKey id)
        where T : class, IEntity
    {
        return ExecuteDbContextAsync(db => db.Set<T>().FindAsync(id).AsTask())!;
    }

    public Task<T> FirstOrDefaultAsync<T>()
        where T : class, IEntity
    {
        return ExecuteDbContextAsync(db => db.Set<T>().FirstOrDefaultAsync())!;
    }
}

public class TestReadFixture<TEntryPoint, TRContext> : TestFixture<TEntryPoint>
    where TEntryPoint : class
    where TRContext : MongoDbContext
{
    public Task ExecuteReadContextAsync(Func<TRContext, Task> action)
    {
        return ExecuteScopeAsync(sp => action(sp.GetRequiredService<TRContext>()));
    }

    public Task<T> ExecuteReadContextAsync<T>(Func<TRContext, Task<T>> action)
    {
        return ExecuteScopeAsync(sp => action(sp.GetRequiredService<TRContext>()));
    }

    public async Task InsertMongoDbContextAsync<T>(string collectionName, params T[] entities) where T : class
    {
        await ExecuteReadContextAsync(async db =>
        {
            await db.GetCollection<T>(collectionName).InsertManyAsync(entities.ToList());
        });
    }
}

public class TestFixture<TEntryPoint, TWContext, TRContext> : TestWriteFixture<TEntryPoint, TWContext>
    where TEntryPoint : class
    where TWContext : DbContext
    where TRContext : MongoDbContext
{
    public Task ExecuteReadContextAsync(Func<TRContext, Task> action)
    {
        return ExecuteScopeAsync(sp => action(sp.GetRequiredService<TRContext>()));
    }

    public Task<T> ExecuteReadContextAsync<T>(Func<TRContext, Task<T>> action)
    {
        return ExecuteScopeAsync(sp => action(sp.GetRequiredService<TRContext>()));
    }

    public async Task InsertMongoDbContextAsync<T>(string collectionName, params T[] entities) where T : class
    {
        await ExecuteReadContextAsync(async db =>
        {
            await db.GetCollection<T>(collectionName).InsertManyAsync(entities.ToList());
        });
    }
}

public class TestFixtureCore<TEntryPoint> : IAsyncLifetime
    where TEntryPoint : class
{
    private Respawner _reSpawnerDefaultDb = default!;
    private Respawner _reSpawnerPersistDb = default!;

    private NpgsqlConnection DefaultDbConnection { get; set; } = default!;

    private NpgsqlConnection PersistDbConnection { get; set; } = default!;


    public TestFixtureCore(TestFixture<TEntryPoint> integrationTestFixture, ITestOutputHelper outputHelper)
    {
        Fixture = integrationTestFixture;
        integrationTestFixture.RegisterServices(RegisterTestsServices);
        integrationTestFixture.Logger = integrationTestFixture.CreateLogger(outputHelper);
    }

    public TestFixture<TEntryPoint> Fixture { get; }


    public async Task InitializeAsync()
    {
        await InitPostgresAsync();
    }

    [Obsolete]
    public async Task DisposeAsync()
    {
        await ResetPostgresAsync();
        await ResetMongoAsync();
        await ResetRabbitMqAsync();
    }

    private async Task InitPostgresAsync()
    {
        var postgresOptions = Fixture.ServiceProvider.GetService<PostgresOptions>();
        var persistOptions = Fixture.ServiceProvider.GetService<PersistMessageOptions>();

        if (!string.IsNullOrEmpty(persistOptions?.ConnectionString))
        {
            await Fixture.PersistMessageBackgroundService.StartAsync(Fixture.CancellationTokenSource.Token);

            PersistDbConnection = new NpgsqlConnection(persistOptions.ConnectionString);
            await PersistDbConnection.OpenAsync();

            _reSpawnerPersistDb = await Respawner.CreateAsync(PersistDbConnection,
                new RespawnerOptions { DbAdapter = DbAdapter.Postgres });
        }

        if (!string.IsNullOrEmpty(postgresOptions?.ConnectionString))
        {
            DefaultDbConnection = new NpgsqlConnection(postgresOptions.ConnectionString);
            await DefaultDbConnection.OpenAsync();

            _reSpawnerDefaultDb = await Respawner.CreateAsync(DefaultDbConnection,
                new RespawnerOptions { DbAdapter = DbAdapter.Postgres });

            await SeedDataAsync();
        }
    }

    private async Task ResetPostgresAsync()
    {
        if (PersistDbConnection is not null)
        {
            await _reSpawnerPersistDb.ResetAsync(PersistDbConnection);

            await Fixture.PersistMessageBackgroundService.StopAsync(Fixture.CancellationTokenSource.Token);
        }

        if (DefaultDbConnection is not null)
        {
            await _reSpawnerDefaultDb.ResetAsync(DefaultDbConnection);
        }
    }

    private async Task ResetMongoAsync(CancellationToken cancellationToken = default)
    {
        var dbClient = new MongoClient(Fixture.MongoDbTestContainer?.GetConnectionString());
        var collections = await dbClient.GetDatabase(TestContainers.MongoContainerConfiguration.Name)
            .ListCollectionsAsync(cancellationToken: cancellationToken);

        foreach (var collection in collections.ToList())
        {
            await dbClient.GetDatabase(TestContainers.MongoContainerConfiguration.Name)
                .DropCollectionAsync(collection["name"].AsString, cancellationToken);
        }
    }

    [Obsolete]
    private async Task ResetRabbitMqAsync(CancellationToken cancellationToken = default)
    {
        var port = Fixture.RabbitMqTestContainer?.GetMappedPublicPort(TestContainers.RabbitMqContainerConfiguration
                       .ApiPort)
                   ?? TestContainers.RabbitMqContainerConfiguration.ApiPort;

        var managementClient = new ManagementClient(Fixture.RabbitMqTestContainer?.Hostname!,
            TestContainers.RabbitMqContainerConfiguration?.UserName!,
            TestContainers.RabbitMqContainerConfiguration?.Password!, port);

        var bd = await managementClient.GetBindingsAsync(cancellationToken);
        var bindings = bd.Where(x => !string.IsNullOrEmpty(x.Source) && !string.IsNullOrEmpty(x.Destination));

        foreach (var binding in bindings)
        {
            await managementClient.DeleteBindingAsync(binding, cancellationToken);
        }

        var queues = await managementClient.GetQueuesAsync(cancellationToken: cancellationToken);

        foreach (var queue in queues)
        {
            await managementClient.DeleteQueueAsync(queue, cancellationToken: cancellationToken);
        }
    }

    protected virtual void RegisterTestsServices(IServiceCollection services)
    {

    }

    private async Task SeedDataAsync()
    {
        using var scope = Fixture.ServiceProvider.CreateScope();

        var seeders = scope.ServiceProvider.GetServices<IDataSeeder>();
        foreach (var seeder in seeders)
        {
            await seeder.SeedAllAsync();
        }
    }
}

public abstract class TestReadBase<TEntryPoint, TRContext> : TestFixtureCore<TEntryPoint>
    //,IClassFixture<IntegrationTestFactory<TEntryPoint, TWContext>>
    where TEntryPoint : class
    where TRContext : MongoDbContext
{
    protected TestReadBase(
        TestReadFixture<TEntryPoint, TRContext> integrationTestFixture, ITestOutputHelper outputHelper = null!) : base(
        integrationTestFixture, outputHelper)
    {
        Fixture = integrationTestFixture;
    }

    public new TestReadFixture<TEntryPoint, TRContext> Fixture { get; }
}

public abstract class TestWriteBase<TEntryPoint, TWContext> : TestFixtureCore<TEntryPoint>
    //,IClassFixture<IntegrationTestFactory<TEntryPoint, TWContext>>
    where TEntryPoint : class
    where TWContext : DbContext
{
    protected TestWriteBase(
        TestWriteFixture<TEntryPoint, TWContext> integrationTestFixture, ITestOutputHelper outputHelper = null) : base(
        integrationTestFixture, outputHelper)
    {
        Fixture = integrationTestFixture;
    }

    public new TestWriteFixture<TEntryPoint, TWContext> Fixture { get; }
}

public abstract class TestBase<TEntryPoint, TWContext, TRContext> : TestFixtureCore<TEntryPoint>
    //,IClassFixture<IntegrationTestFactory<TEntryPoint, TWContext, TRContext>>
    where TEntryPoint : class
    where TWContext : DbContext
    where TRContext : MongoDbContext
{
    protected TestBase(
        TestFixture<TEntryPoint, TWContext, TRContext> integrationTestFixture, ITestOutputHelper outputHelper = null!) :
        base(integrationTestFixture, outputHelper)
    {
        Fixture = integrationTestFixture;
    }

    public new TestFixture<TEntryPoint, TWContext, TRContext> Fixture { get; }
}
