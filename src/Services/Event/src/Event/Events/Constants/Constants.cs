using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Constants;

namespace EventPAM.Event.Events.Constants;

public static class Constants
{
    public static class Role
    {
        public const string Admin = GeneralOperationClaims.Admin;
        public const string Customer = GeneralOperationClaims.Customer;
        public const string EventManager = GeneralOperationClaims.EventManager;
    }
}
