namespace EventPAM.BuildingBlocks.CrossCuttingConcerns.Logging;

public class LogDetail
{
    public string FullName { get; set; } = default!;

    public string MethodName { get; set; } = default!;

    public string User { get; set; } = default!;

    public List<LogParameter> Parameters { get; set; } = [];
}
