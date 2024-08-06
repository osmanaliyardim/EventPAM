namespace EventPAM.BuildingBlocks;

public class EventPAMBase
{
    public static class Messages
    {
        // Error Messages
        public const string SERILOG_NULLOPTIONS = "You have sent a blank value! Something went wrong. Please try again.";
        public const string VALIDATION = "Validation failed: ";
        public const string NOT_AUTHENTICATED = "You are not authenticated.";
        public const string NOT_AUTHORIZED = "You are not authorized.";
        public const string CANNOT_CREATE_DB = "Cannot create table if there are pending migrations.";
        public const string IVERSION_ERROR = "Try to find IVersion";
        public const string IAGGREGATE_ERROR = "Try to find IAggregate";
        public const string POLLY_SERVICE_DOWN = "Service shutdown during {BreakDuration} after {RetryCount} failed retries";
        public const string POLLY_FAILED = "Request failed with {StatusCode}. Waiting {TimeSpan} before next retry. Retry attempt {RetryCount}.";

        // Warning Messages
        public const string PERFORMANCE_WARNING = "Performance -> ";
        public const string DB_CONSISTENCY_WARNING = "The record no longer exists in the database, The record has been deleted by another user.";

        // Info Messages
        public const string CACHE_FETCHED = "Fetched from Cache -> ";
        public const string CACHE_ADDED = "Added to Cache -> ";
        public const string CACHE_REMOVED = "Removed Cache -> ";
        public const string SERVICE_RESTARTED = "Service restarted";
        public const string PMBS_STARTED = "PersistMessage Background Service Start";
        public const string PMBS_STOPPED = "PersistMessage Background Service Stop";
        public const string PMBS_MESSAGE_SAVED = "Message with id: {MessageID} and delivery type: {DeliveryType} saved in persistence message store.";
        public const string PMBS_MESSAGE_PROCESSED = "InternalCommand with id: {EventID} and delivery type: {DeliveryType} processed from the persistence message store.";
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
        public const string POLLY_GRPC_CB_LOGGER = "PollyGrpcCircuitBreakerPoliciesLogger";
        public const string POLLY_HTTP_CB_LOGGER = "PollyHttpClientCircuitBreakerPoliciesLogger";
        public const string ENVIRONMENT_VARIABLE = "ASPNETCORE_ENVIRONMENT";
        public const string DEFAULT_CONN_STR = "DefaultConnection";
        public const string BASE_API_PATH = "api/v{version:apiVersion}";
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
