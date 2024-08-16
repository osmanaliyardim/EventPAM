using EventPAM.Customer.Extensions.Infrastructure;
using EventPAM.BuildingBlocks.Web;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security;
using EventPAM.Customer.API;
using static EventPAM.BuildingBlocks.EventPAMBase;
using EventPAM.Customer;

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

builder.AddMinimalEndpoints(assemblies: typeof(CustomerRoot).Assembly);

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

namespace EventPAM.Customer.API
{
    public partial class Program { }
}
