using Nest;

namespace EventPAM.BuildingBlocks.ElasticSearch.Models;

public class ElasticSearchModel
{
    public Id ElasticId { get; set; } = default!;

    public string IndexName { get; set; } = default!;
}
