namespace EventPAM.BuildingBlocks.Behaviors.Authorization;

public interface ISecuredRequest
{
    public string[] Roles { get; }
}
