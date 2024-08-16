using static EventPAM.BuildingBlocks.EventPAMBase;
using Microsoft.AspNetCore.RateLimiting;
using EventPAM.BuildingBlocks.Web;
using Figgle;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Logging.Serilog;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
var env = builder.Environment;
var appOptions = builder.Services.GetOptions<AppOptions>(Configs.APP_CONFIG);
Console.WriteLine(FiggleFonts.Standard.Render(appOptions.Name));

// Configs
var rateLimitingWindowTime = TimeSpan.FromSeconds(10);
var rateLimitingReqLimit = 3;

// Add services to the container.
builder.AddCustomSerilog(env);

builder.Services.AddHttpContextAccessor();

builder.Services.AddReverseProxy()
    .LoadFromConfig(config.GetSection(Configs.YARP_GATEWAY_NAME));

// Rate-limiting configurations
builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.AddFixedWindowLimiter(Policies.RATE_LIMITING, options =>
    {
        options.Window = rateLimitingWindowTime;
        options.PermitLimit = rateLimitingReqLimit;
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseRateLimiter();

app.UseSerilogRequestLogging();

app.UseCorrelationId();

app.UseRouting();

app.UseHttpsRedirection();

//app.UseAuthentication();

//app.UseAuthorization();

app.MapReverseProxy();

app.Run();
