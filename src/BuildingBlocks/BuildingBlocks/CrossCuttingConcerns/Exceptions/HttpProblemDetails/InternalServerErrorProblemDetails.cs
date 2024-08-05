using Microsoft.AspNetCore.Mvc;

namespace EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.HttpProblemDetails;

internal class InternalServerErrorProblemDetails : ProblemDetails
{
    public InternalServerErrorProblemDetails(string detail)
    {
        Title = "Internal server error";
        Detail = "Internal server error";
        Status = StatusCodes.Status500InternalServerError;
        Type = "https://example.com/probs/internal";
    }
}
