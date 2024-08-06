namespace EventPAM.BuildingBlocks.ElasticSearch.Models;

public class SearchByQueryParameters : SearchParameters
{
    public string QueryName { get; set; } = default!;

    public string Query { get; set; } = default!;

    public string[] Fields { get; set; } = default!;
}
