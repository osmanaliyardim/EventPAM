namespace EventPAM.BuildingBlocks.CrossCuttingConcerns.Logging.Serilog.ConfigurationModels;

public class MsSqlConfiguration
{
    public string ConnectionString { get; set; } = default!;

    public string TableName { get; set; } = default!;

    public bool AutoCreateSqlTable { get; set; }
}
