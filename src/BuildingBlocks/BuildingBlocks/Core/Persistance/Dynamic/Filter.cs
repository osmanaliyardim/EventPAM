namespace EventPAM.BuildingBlocks.Core.Persistence.Dynamic;

public class Filter
{
    public string Field { get; set; } = default!;

    public string Operator { get; set; } = default!;

    public string? Value { get; set; }

    public string? Logic { get; set; }

    public IEnumerable<Filter>? Filters { get; set; }

    public Filter() { }

    public Filter
        (string field, string @operator, string? value, string? logic, IEnumerable<Filter>? filters)
        : this()
    {
        Field = field;
        Operator = @operator;
        Value = value;
        Logic = logic;
        Filters = filters;
    }
}
