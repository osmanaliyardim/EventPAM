namespace EventPAM.BuildingBlocks.Core.Model;

// To handle optimistic concurrency
public interface IVersion
{
    long Version { get; set; }
}
