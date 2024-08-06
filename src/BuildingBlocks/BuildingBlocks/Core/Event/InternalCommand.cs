using EventPAM.BuildingBlocks.Core.CQRS;

namespace EventPAM.BuildingBlocks.Core.Event;

public record InternalCommand : IInternalCommand, ICommand;
