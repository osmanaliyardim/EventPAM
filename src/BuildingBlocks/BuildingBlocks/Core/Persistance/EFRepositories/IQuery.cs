namespace EventPAM.BuildingBlocks.Core.Persistence.EFRepositories;

public interface IQuery<T>
{
    IQueryable<T> Query();
}
