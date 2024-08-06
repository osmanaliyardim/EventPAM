namespace EventPAM.BuildingBlocks.ElasticSearch.Models;

public class SearchByFieldParameters : SearchParameters
{
    public string FieldName { get; set; } = default!;

    public string Value { get; set; } = default!;
}
