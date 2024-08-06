using EventPAM.BuildingBlocks.CrossCuttingConcerns.Logging.Serilog.ConfigurationModels;
using MongoDB.Driver;
using Serilog;

namespace EventPAM.BuildingBlocks.CrossCuttingConcerns.Logging.Serilog.Logger;

public class MongoDbLogger : LoggerServiceBase
{
    public MongoDbLogger(IConfiguration configuration)
    {
        MongoDbConfiguration? logConfiguration = configuration
            .GetSection(Configs.SERILOG_MONGO)
                .Get<MongoDbConfiguration>();

        Logger = new LoggerConfiguration().WriteTo
            .MongoDBBson(cfg =>
            {
                MongoClient client = new(logConfiguration!.ConnectionString);
                IMongoDatabase? mongoDbInstance = client.GetDatabase(logConfiguration.Collection);
                cfg.SetMongoDatabase(mongoDbInstance);
            })
            .CreateLogger();
    }
}
