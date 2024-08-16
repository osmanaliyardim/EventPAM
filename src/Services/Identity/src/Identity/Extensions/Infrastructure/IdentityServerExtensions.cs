using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using EventPAM.Identity.Configs;
using EventPAM.BuildingBlocks;
using TokenOptions = EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.JWT.TokenOptions;
using Microsoft.Extensions.Configuration;


namespace EventPAM.Identity.Extensions.Infrastructure;

public static class IdentityServerExtensions
{
    public static WebApplicationBuilder AddCustomIdentityServer(this WebApplicationBuilder builder)
    {
        TokenOptions? tokenOptions = builder.Configuration.GetSection(EventPAMBase.Configs.TOKEN_OPTIONS).Get<TokenOptions>();

        var identityServerBuilder = builder.Services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                options.IssuerUri = tokenOptions!.Issuer;
            })
            .AddInMemoryApiResources(Config.ApiResources)
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddInMemoryClients(Config.Clients);

        identityServerBuilder.AddDeveloperSigningCredential();

        return builder;
    }
}
