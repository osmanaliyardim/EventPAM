using Microsoft.AspNetCore.Mvc;

namespace EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.HttpProblemDetails;

internal class BadRequestProblemDetails : ProblemDetails
{
    public BadRequestProblemDetails(string detail)
    {
        Title = "Bad request";
        Detail = "Bad request";
        Status = StatusCodes.Status400BadRequest;
        Type = "https://example.com/probs/internal";
    }
}
