using EventPAM.BuildingBlocks.CrossCuttingConcerns.Logging.Serilog.ConfigurationModels;
using Serilog;
using Serilog.Sinks.MSSqlServer;

namespace EventPAM.BuildingBlocks.CrossCuttingConcerns.Logging.Serilog.Logger;

public class MsSqlLogger : LoggerServiceBase
{
    public MsSqlLogger(IConfiguration configuration)
    {
        MsSqlConfiguration logConfiguration =
            configuration.GetSection(Configs.SERILOG_SQLSERVER).Get<MsSqlConfiguration>()
            ?? throw new Exception(Messages.SERILOG_NULLOPTIONS);

        MSSqlServerSinkOptions sinkOptions =
            new() { TableName = logConfiguration.TableName, AutoCreateSqlTable = logConfiguration.AutoCreateSqlTable };

        ColumnOptions columnOptions = new();

        global::Serilog.Core.Logger serilogConfig = new LoggerConfiguration().WriteTo
            .MSSqlServer(logConfiguration.ConnectionString, sinkOptions, columnOptions: columnOptions)
            .CreateLogger();

        Logger = serilogConfig;
    }
}
