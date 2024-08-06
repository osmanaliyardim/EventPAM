namespace EventPAM.BuildingBlocks.CrossCuttingConcerns.Logging.Serilog.ConfigurationModels;

public class PostgreSqlConfiguration
{
    public string ConnectionString { get; set; } = default!;

    public string TableName { get; set; } = default!;

    public bool NeedAutoCreateTable { get; set; }
}
