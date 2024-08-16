using Microsoft.AspNetCore.Mvc;

namespace EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.HttpProblemDetails;

internal class AggregateNotFoundProblemDetails : ProblemDetails
{
    public AggregateNotFoundProblemDetails(string detail)
    {
        Title = "Aggregate Not Found";
        Detail = "Aggregate Not Found";
        Status = StatusCodes.Status404NotFound;
        Type = "https://example.com/probs/internal";
    }
}
