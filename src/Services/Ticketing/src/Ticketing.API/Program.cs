using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security;
using EventPAM.BuildingBlocks.Web;
using EventPAM.Ticketing;
using EventPAM.Ticketing.API;
using EventPAM.Ticketing.Extensions.Infrastructure;
using static EventPAM.BuildingBlocks.EventPAMBase;

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

builder.AddMinimalEndpoints(assemblies: typeof(TicketingRoot).Assembly);

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

namespace EventPAM.Ticketing.API
{
    public partial class Program { }
}
