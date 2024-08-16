using EventPAM.BuildingBlocks.Core.Persistence.Paging;

namespace EventPAM.BuildingBlocks.Core.Responses;

public class GetListResponse<T> : BasePageableModel
{
    public IList<T> Items
    {
        get => _items ??= [];
        set => _items = value;
    }

    private IList<T>? _items;
}
