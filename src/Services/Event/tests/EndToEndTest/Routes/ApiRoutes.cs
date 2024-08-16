namespace EndToEnd.Test.Routes;

public static class ApiRoutes
{
    private const string BaseApiPath = "api/v1.0";

    public static class Event
    {
        public const string Id = "{id}";
        public const string GetEventById = $"{BaseApiPath}/event/{Id}";
        public const string CreateEvent = $"{BaseApiPath}/event";
    }
}
