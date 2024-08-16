﻿using Testcontainers.EventStoreDb;
using Testcontainers.MongoDb;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using EventPAM.BuildingBlocks.Web;

namespace EventPAM.BuildingBlocks.TestBase;

public static class TestContainers
{
    public static RabbitMqContainerOptions RabbitMqContainerConfiguration { get; }

    public static PostgresContainerOptions PostgresContainerConfiguration { get; }

    public static PostgresPersistContainerOptions PostgresPersistContainerConfiguration { get; }

    public static MongoContainerOptions MongoContainerConfiguration { get; }

    public static EventStoreContainerOptions EventStoreContainerConfiguration { get; }

    static TestContainers()
    {
        var configuration = ConfigurationHelper.GetConfiguration();

        RabbitMqContainerConfiguration =
            configuration.GetOptions<RabbitMqContainerOptions>(nameof(RabbitMqContainerOptions));

        PostgresContainerConfiguration =
            configuration.GetOptions<PostgresContainerOptions>(nameof(PostgresContainerOptions));

        PostgresPersistContainerConfiguration =
            configuration.GetOptions<PostgresPersistContainerOptions>(nameof(PostgresPersistContainerOptions));

        MongoContainerConfiguration = configuration.GetOptions<MongoContainerOptions>(nameof(MongoContainerOptions));

        EventStoreContainerConfiguration =
            configuration.GetOptions<EventStoreContainerOptions>(nameof(EventStoreContainerOptions));
    }

    public static PostgreSqlContainer PostgresTestContainer()
    {
        var baseBuilder = new PostgreSqlBuilder()
            .WithUsername(PostgresContainerConfiguration.UserName)
            .WithPassword(PostgresContainerConfiguration.Password)
            .WithLabel("Key", "Value");

        var builder = baseBuilder
            .WithImage(PostgresContainerConfiguration.ImageName)
            .WithName(PostgresContainerConfiguration.Name)
            .WithCommand(new string[2] { "-c", "max_prepared_transactions=10" })
            .WithPortBinding(PostgresContainerConfiguration.Port, true)
            .Build();

        return builder;
    }

    public static PostgreSqlContainer PostgresPersistTestContainer()
    {
        var baseBuilder = new PostgreSqlBuilder()
            .WithUsername(PostgresPersistContainerConfiguration.UserName)
            .WithPassword(PostgresPersistContainerConfiguration.Password)
            .WithLabel("Key", "Value");

        var builder = baseBuilder
            .WithImage(PostgresPersistContainerConfiguration.ImageName)
            .WithName(PostgresPersistContainerConfiguration.Name)
            .WithCommand(new string[2] { "-c", "max_prepared_transactions=10" })
            .WithPortBinding(PostgresPersistContainerConfiguration.Port, true)
            .Build();

        return builder;
    }

    public static MongoDbContainer MongoTestContainer()
    {
        var baseBuilder = new MongoDbBuilder()
            .WithUsername(MongoContainerConfiguration.UserName)
            .WithPassword(MongoContainerConfiguration.Password)
            .WithLabel("Key", "Value");

        var builder = baseBuilder
            .WithImage(MongoContainerConfiguration.ImageName)
            .WithName(MongoContainerConfiguration.Name)
            .WithPortBinding(MongoContainerConfiguration.Port, true)
            .Build();

        return builder;
    }

    public static RabbitMqContainer RabbitMqTestContainer()
    {
        var baseBuilder = new RabbitMqBuilder()
            .WithUsername(RabbitMqContainerConfiguration.UserName)
            .WithPassword(RabbitMqContainerConfiguration.Password)
            .WithLabel("Key", "Value");

        var builder = baseBuilder
            .WithImage(RabbitMqContainerConfiguration.ImageName)
            .WithName(RabbitMqContainerConfiguration.Name)
            .WithPortBinding(RabbitMqContainerConfiguration.ApiPort, true)
            .WithPortBinding(RabbitMqContainerConfiguration.Port, true)
            .Build();

        return builder;
    }

    public static EventStoreDbContainer EventStoreTestContainer()
    {
        var baseBuilder = new EventStoreDbBuilder()
            .WithLabel("Key", "Value");

        var builder = baseBuilder
            .WithImage(EventStoreContainerConfiguration.ImageName)
            .WithName(EventStoreContainerConfiguration.Name)
            .Build();

        return builder;
    }

    public sealed class RabbitMqContainerOptions
    {
        public string Name { get; set; } = "rabbitmq_" + Guid.NewGuid();

        public int Port { get; set; } = 5672;

        public int ApiPort { get; set; } = 15672;

        public string ImageName { get; set; } = "rabbitmq:management";

        public string UserName { get; set; } = "guest";

        public string Password { get; set; } = "guest";
    }

    public sealed class PostgresContainerOptions
    {
        public string Name { get; set; } = "postgreSql_" + Guid.NewGuid().ToString("D");

        public int Port { get; set; } = 5432;

        public string ImageName { get; set; } = "postgres:latest";

        public string UserName { get; set; } = Guid.NewGuid().ToString("D");

        public string Password { get; set; } = Guid.NewGuid().ToString("D");
    }

    public sealed class PostgresPersistContainerOptions
    {
        public string Name { get; set; } = "postgreSql_" + Guid.NewGuid().ToString("D");

        public int Port { get; set; } = 5432;

        public string ImageName { get; set; } = "postgres:latest";

        public string UserName { get; set; } = Guid.NewGuid().ToString("D");

        public string Password { get; set; } = Guid.NewGuid().ToString("D");
    }

    public sealed class MongoContainerOptions
    {
        public string Name { get; set; } = "mongo_" + Guid.NewGuid().ToString("D");

        public int Port { get; set; } = 27017;

        public string ImageName { get; set; } = "mongo:latest";

        public string UserName { get; set; } = Guid.NewGuid().ToString("D");

        public string Password { get; set; } = Guid.NewGuid().ToString("D");
    }

    public sealed class EventStoreContainerOptions
    {
        public string Name { get; set; } = "event_store_" + Guid.NewGuid().ToString("D");

        public int Port { get; set; } = 2113;

        public string ImageName { get; set; } = "eventstore/eventstore:latest";
    }
}
