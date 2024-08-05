namespace EventPAM.BuildingBlocks;

public class EventPAMBase
{
    public static class Messages
    {
        // Error Messages
        public const string SERILOG_NULLOPTIONS = "You have sent a blank value! Something went wrong. Please try again.";
        public const string VALIDATION = "Validation failed: ";

        // Warning Messages


        // Info Messages
        public const string CACHE_FETCHED = "Fetched from Cache -> ";
        public const string CACHE_ADDED = "Added to Cache -> ";
        public const string CACHE_REMOVED = "Removed Cache -> ";
    }

    public static class Configs
    {
        // Connection Strings - Keys - Secrets
        public const string YARP_GATEWAY_NAME = "ReverseProxy";
        public const string CACHE = "CacheSettings";
        public const string SERILOG_RABBITMQ = "SeriLogConfigurations:RabbitMQConfiguration";
        public const string SERILOG_POSTGRESQL = "SeriLogConfigurations:PostgreConfiguration";
        public const string SERILOG_SQLSERVER = "SeriLogConfigurations:MsSqlConfiguration";
        public const string SERILOG_MONGO = "SeriLogConfigurations:MongoDbConfiguration";
        public const string SERILOG_GRAYLOG = "SeriLogConfigurations:GraylogConfiguration";
        public const string SERILOG_FILE = "SeriLogConfigurations:FileLogConfiguration";
        public const string SERILOG_ELASTICSEARCH = "SeriLogConfigurations:ElasticSearchConfiguration";
    }

    public static class Endpoints
    {
        // Endpoints - Urls

    }

    public static class Policies
    {
        // Policies
        public const string RATE_LIMITING = "fixed";
    }
}
