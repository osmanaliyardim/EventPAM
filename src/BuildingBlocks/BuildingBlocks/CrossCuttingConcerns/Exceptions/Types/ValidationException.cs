namespace EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.Types;

public class ValidationException : Exception
{
    public IEnumerable<ValidationExceptionModel> Errors { get; }

    public ValidationException(IEnumerable<ValidationExceptionModel> errors)
        : base(BuildErrorMessage(errors))
    {
        Errors = errors;
    }

    private static string BuildErrorMessage(IEnumerable<ValidationExceptionModel> errors)
    {
        IEnumerable<string> arr = errors.Select(
            errModel => $"{Environment.NewLine} -- {errModel.Property}: {string.Join(Environment.NewLine, errModel.Errors!)}"
        );
        return $"{Messages.VALIDATION}{string.Join(string.Empty, arr)}";
    }
}

public class ValidationExceptionModel
{
    public string? Property { get; set; }

    public IEnumerable<string>? Errors { get; set; }
}
