namespace EventPAM.BuildingBlocks.CrossCuttingConcerns.Logging;

public class LogDetailWithException : LogDetail
{
    public string ExceptionMessage { get; set; } = default!;
}
