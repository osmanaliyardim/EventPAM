namespace EventPAM.BuildingBlocks.Core.CQRS;

public interface IQuery<out TResponse> : IRequest<TResponse>
    where TResponse : notnull
{

}
