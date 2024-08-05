var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

// Add services to the container.
builder.Services.AddReverseProxy()
    .LoadFromConfig(config.GetSection(EventPAMBase.Configs.YARP_GATEWAY_NAME));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapReverseProxy();

app.Run();
