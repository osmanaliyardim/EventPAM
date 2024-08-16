namespace EventPAM.BuildingBlocks.ElasticSearch.Models;

public class ElasticSearchConfig
{
    public string ConnectionString { get; set; } = default!;

    public string UserName { get; set; } = default!;

    public string Password { get; set; } = default!;

    public string Uri => $"http://{UserName}:{Password}@localhost:9200";
}
