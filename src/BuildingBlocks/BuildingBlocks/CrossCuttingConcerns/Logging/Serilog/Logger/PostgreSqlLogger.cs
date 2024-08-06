using EventPAM.BuildingBlocks.CrossCuttingConcerns.Logging.Serilog.ConfigurationModels;
using NpgsqlTypes;
using Serilog;
using Serilog.Sinks.PostgreSQL;

namespace EventPAM.BuildingBlocks.CrossCuttingConcerns.Logging.Serilog.Logger;

public class PostgreSqlLogger : LoggerServiceBase
{
    public PostgreSqlLogger(IConfiguration configuration)
    {
        PostgreSqlConfiguration postgreConfiguration =
            configuration.GetSection(Configs.SERILOG_POSTGRESQL).Get<PostgreSqlConfiguration>()
            ?? throw new Exception(Messages.SERILOG_NULLOPTIONS);

        IDictionary<string, ColumnWriterBase> columnWriters = new Dictionary<string, ColumnWriterBase>
        {
            { "message", new RenderedMessageColumnWriter() },
            { "message_template", new MessageTemplateColumnWriter() },
            { "level", new LevelColumnWriter(renderAsText: true, NpgsqlDbType.Varchar) },
            { "raise_date", new TimestampColumnWriter() },
            { "exception", new ExceptionColumnWriter() },
            { "properties", new LogEventSerializedColumnWriter() },
            { "props_test", new PropertiesColumnWriter() },
            { "machine_name", new SinglePropertyColumnWriter(propertyName: "MachineName", format: "l") }
        };

        global::Serilog.Core.Logger loggerConfiguration = new LoggerConfiguration().WriteTo
            .PostgreSQL(
                postgreConfiguration.ConnectionString,
                postgreConfiguration.TableName,
                columnWriters,
                needAutoCreateTable: postgreConfiguration.NeedAutoCreateTable
            )
            .CreateLogger();
        Logger = loggerConfiguration;
    }
}
