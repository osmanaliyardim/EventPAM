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
        public const string ES_INDEX_ALREADY_EXISTS = "Elastic Index already exists";
        public const string ES_INDEX_NULL = "Elastic Index name cannot be null or empty";
        public const string INVALID_CREDENTIALS = "Invalid Credentials";
        public const string USER_NOT_FOUND = "User could not be found!";
        public const string VENUE_NOT_FOUND = "Venue could not be found!";
        public const string AGE_INVALID = "Age cannot be null or negative";
        public const string NAME_INVALID = "Name cannot be empty or whitespace.";
        public const string USER_ALREADY_EXISTS = "User already exists!";
        public const string INVALID_DURATION = "Duration cannot be negative.";
        public const string INVALID_PRICE = "Price cannot be negative.";
        public const string ALL_SEATS_FULL = "All seats are full!";
        public const string INVALID_SEAT_NUMBER = "SeatNumber cannot be null or negative";
        public const string SEAT_ALREADY_EXISTS = "Seat already exist!";
        public const string INCORRECT_SEAT_NUMBER = "Seat number is incorrect!";
        public const string VENUE_ALREADY_EXISTS = "Venue already exists!";
        public const string INVALID_VENUE_NAME = "Venue Name cannot be empty or whitespace.";
        public const string EVENT_ALREADY_EXISTS = "Event already exist!";
        public const string EVENT_NOT_FOUND = "Event not found!";
        public const string INVALID_CAPACITY = "Venue Capacity cannot be less than zero.";
        public const string TICKET_ALREADY_EXIST = "Ticketing already exists!";
        public const string ALREADY_VERIFIED_OTP = "Verified Otp Authenticator already exists!";
        public const string REFRESH_TOKEN_NOT_FOUND = "Refresh does not exist!";
        public const string INVALID_REFRESH_TOKEN = "Invalid refresh token!";
        public const string INVALID_PASSWORD = "Invalid password!";
        public const string EMAIL_AUTH_NOT_FOUND = "Email Authenticator does not exist!";
        public const string EMAIL_AUTHKEY_NOT_FOUND = "Email Activation Key does not exist!";
        public const string OTP_AUTH_NOT_FOUND = "Otp Authenticator does not exist!";

        // Warning Messages
        public const string PERFORMANCE_WARNING = "Performance -> ";
        public const string DB_CONSISTENCY_WARNING = "The record no longer exists in the database, the record has been deleted by another user.";

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
        public const string ELASTIC_CONFIG = "ElasticSearchConfig";
        public const string RABBITMQ_HOST = "RabbitMQSettings:Host";
        public const string RABBITMQ_USERNAME = "RabbitMQSettings:UserName";
        public const string RABBITMQ_PASS = "RabbitMQSettings:Password";
        public const string RABBITMQ_PORT = "RabbitMQSettings:Port";
        public const string AZURE_SB_CONFIG = "AzureServiceBusSettings:ConnectionString";
        public const string APP_CONFIG = "AppOptions";
        public const string IDENTITYDB_CONFIG = "Server=localhost;Port=5432;Database=IdentityDB;User Id=postgres;Password=postgres;Include Error Detail=true";
        public const string CUSTOMERDB_CONFIG = "Server=localhost;Port=5432;Database=CustomerDB;User Id=postgres;Password=postgres;Include Error Detail=true";
        public const string EVENTDB_CONFIG = "Server=localhost;Port=5432;Database=EventDB;User Id=postgres;Password=postgres;Include Error Detail=true";
        public const string ELASTIC_URL = "http://localhost:9200";
        public const string POSTGRES_CONFIG = "PostgresOptions:ConnectionString";
        public const string PMBS_CONFIG = "PersistMessageOptions:ConnectionString";
        public const string MONGO_CONFIG = "MongoOptions:ConnectionString";
        public const string MONGO_NAME = "MongoOptions:DatabaseName";
        public const string EVENTSTORE_CONFIG = "MongoOptions:DatabaseName";
        public const string REDIS_CONFIG = "localhost:6379";
        public const string TOKEN_OPTIONS = "TokenOptions";
        public const string API_CONFIG = "WebAPIConfiguration";
        public const string MAIL_SETTINGS = "MailSettings";
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
