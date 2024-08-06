namespace EventPAM.BuildingBlocks.ElasticSearch.Models;

public class ElasticSearchGetModel<T>
{
    public string ElasticId { get; set; } = default!;

    public T Item { get; set; } = default!;
}
