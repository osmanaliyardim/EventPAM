namespace EventPAM.BuildingBlocks.Polly;

public class PolicyOptions
{
    public RetryOptions Retry { get; set; } = default!;

    public CircuitBreakerOptions CircuitBreaker { get; set; } = default!;
}
