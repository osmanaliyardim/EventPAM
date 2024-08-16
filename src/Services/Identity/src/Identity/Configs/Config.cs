using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace EventPAM.Identity.Configs;

public static class Config
{
    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        {
            new(Constants.StandardScopes.EventApi),
            new(Constants.StandardScopes.CustomerApi),
            new(Constants.StandardScopes.EventManagerApi),
            new(Constants.StandardScopes.IdentityApi),
            new(Constants.StandardScopes.TicketingApi),
            new(Constants.StandardScopes.VenueApi),
            new(Constants.StandardScopes.OfferApi)
        };

    public static IList<ApiResource> ApiResources =>
        new List<ApiResource>
        {
            new(Constants.StandardScopes.EventApi),
            new(Constants.StandardScopes.CustomerApi),
            new(Constants.StandardScopes.EventManagerApi),
            new(Constants.StandardScopes.IdentityApi),
            new(Constants.StandardScopes.TicketingApi),
            new(Constants.StandardScopes.VenueApi),
            new(Constants.StandardScopes.OfferApi)
        };

    public static IEnumerable<Client> Clients =>
        new List<Client>
        {
            new()
            {
                ClientId = "client",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    Constants.StandardScopes.EventApi,
                    Constants.StandardScopes.CustomerApi,
                    Constants.StandardScopes.EventManagerApi,
                    Constants.StandardScopes.IdentityApi,
                    Constants.StandardScopes.TicketingApi,
                    Constants.StandardScopes.VenueApi,
                    Constants.StandardScopes.OfferApi,
                },
                AccessTokenLifetime = 3600,  // authorize the client to access protected resources
                IdentityTokenLifetime = 3600 // authenticate the user
            }
        };
}
