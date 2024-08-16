namespace EventPAM.BuildingBlocks.EventStoreDB.Events;

public interface IProjection
{
    void When(object @event);
}
