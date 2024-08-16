using EventPAM.BuildingBlocks.Web;
using EventPAM.Event.Extensions.Infrastructure;
using EventPAM.Event;
using static EventPAM.BuildingBlocks.EventPAMBase;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security;
using EventPAM.Event.API;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.UseDefaultServiceProvider((context, options) =>
{
    // Service provider validation
    options.ValidateScopes =
           context.HostingEnvironment.IsDevelopment()
        || context.HostingEnvironment.IsStaging()
        || context.HostingEnvironment.IsEnvironment("tests");

    options.ValidateOnBuild = true;
});

//builder.Services.AddDistributedMemoryCache(); // In memory caching
builder.Services.AddStackExchangeRedisCache(opt => opt.Configuration = Configs.REDIS_CONFIG);

builder.AddMinimalEndpoints(assemblies: typeof(EventRoot).Assembly);

builder.AddInfrastructure();

builder.AddSecurityServices();

builder.Services.AddCors(
    opt =>
        opt.AddDefaultPolicy(p =>
        {
            p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        })
);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapMinimalEndpoints();

app.UseAuthentication();

app.UseAuthorization();

app.UseInfrastructure();

app.UseCors(
    opt =>
        opt.WithOrigins(app.Configuration.GetSection(Configs.API_CONFIG).Get<WebAPIConfiguration>()!.AllowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
);

app.Run();

namespace EventPAM.Event.API
{
    public partial class Program { }
}
