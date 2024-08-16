using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Constants;

namespace EventPAM.Identity.Identity.Constants;

public static class Constants
{
    public static class Role
    {
        public const string Admin = GeneralOperationClaims.Admin;
        public const string Customer = GeneralOperationClaims.Customer;
        public const string EventManager = GeneralOperationClaims.EventManager;
    }

    public static class StandardScopes
    {
        public const string Roles = "roles";
        public const string CustomerApi = "customer-api";
        public const string EventApi = "event-api";
        public const string EventManagerApi = "event-manager-api";
        public const string IdentityApi = "identity-api";
        public const string OfferApi = "offer-api";
        public const string TicketingApi = "ticketing-api";
        public const string VenueApi = "venue-api";
    }
}
