namespace EventPAM.BuildingBlocks.EFCore;

public interface IDataSeeder
{
    Task SeedAllAsync();
}