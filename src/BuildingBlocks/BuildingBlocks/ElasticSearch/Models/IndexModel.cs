namespace EventPAM.BuildingBlocks.ElasticSearch.Models;

public class IndexModel
{
    public string IndexName { get; set; } = default!;

    public string AliasName { get; set; } = default!;

    public int NumberOfReplicas { get; set; } = 3;

    public int NumberOfShards { get; set; } = 3;
}
