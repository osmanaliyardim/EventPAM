using static EventPAM.BuildingBlocks.EventPAMBase;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

// Configs
var rateLimitingWindowTime = TimeSpan.FromSeconds(10);
var rateLimitingReqLimit = 3;

// Add services to the container.
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

app.MapReverseProxy();

app.Run();
