namespace EventPAM.BuildingBlocks.ElasticSearch.Models;

public class SearchParameters
{
    public string IndexName { get; set; } = default!;

    public int From { get; set; } = 0;

    public int Size { get; set; } = 10;
}
