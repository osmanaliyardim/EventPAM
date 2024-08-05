namespace EventPAM.BuildingBlocks.Core.Persistence.Dynamic;

public class Sort
{
    public string Field { get; set; } = default!;
    public string Dir { get; set; } = default!;

    public Sort() { }

    public Sort(string field, string dir)
    {
        Field = field;
        Dir = dir;
    }
}
