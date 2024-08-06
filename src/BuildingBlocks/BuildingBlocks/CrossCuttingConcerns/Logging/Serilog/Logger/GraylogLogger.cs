using EventPAM.BuildingBlocks.CrossCuttingConcerns.Logging.Serilog.ConfigurationModels;
using Serilog;
using Serilog.Sinks.Graylog;
using Serilog.Sinks.Graylog.Core.Transport;

namespace EventPAM.BuildingBlocks.CrossCuttingConcerns.Logging.Serilog.Logger;

public class GraylogLogger : LoggerServiceBase
{
    public GraylogLogger(IConfiguration configuration)
    {
        GraylogConfiguration logConfiguration = configuration
            .GetSection(Configs.SERILOG_GRAYLOG)
            .Get<GraylogConfiguration>()!;

        Logger = new LoggerConfiguration().WriteTo
            .Graylog(
                new GraylogSinkOptions
                {
                    HostnameOrAddress = logConfiguration!.HostnameOrAddress,
                    Port = logConfiguration.Port,
                    TransportType = TransportType.Udp
                }
            )
            .CreateLogger();
    }
}
