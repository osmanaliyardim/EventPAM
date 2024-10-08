﻿using EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.Types;
using Microsoft.AspNetCore.Mvc;

namespace EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.HttpProblemDetails;

internal class ValidationProblemDetails : ProblemDetails
{
    public IEnumerable<ValidationExceptionModel> Errors { get; init; }

    public ValidationProblemDetails(IEnumerable<ValidationExceptionModel> errors)
    {
        Title = "Validation error(s)";
        Detail = "One or more validation errors occurred.";
        Errors = errors;
        Status = StatusCodes.Status400BadRequest;
        Type = "https://example.com/probs/validation";
    }
}
