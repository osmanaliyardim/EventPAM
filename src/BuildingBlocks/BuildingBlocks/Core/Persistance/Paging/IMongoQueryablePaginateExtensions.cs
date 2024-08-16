using EventPAM.BuildingBlocks.Core.Persistence.Paging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace EventPAM.BuildingBlocks.Core.Persistance.Paging;

public static class IMongoQueryablePaginateExtensions
{
    public static async Task<IPaginate<T>> ToReadPaginateAsync<T>(
        this IMongoQueryable<T> source,
        int index,
        int size,
        int from = 0,
        CancellationToken cancellationToken = default
    )
    {
        if (from > index)
            throw new ArgumentException($"From: {from} > Index: {index}, must from <= Index");

        int count = await source.CountAsync(cancellationToken).ConfigureAwait(false);

        IList<T> items = await source.Skip((index - from) * size).Take(size).ToListAsync(cancellationToken).ConfigureAwait(false);

        Paginate<T> list =
            new()
            {
                Index = index,
                Size = size,
                From = from,
                Count = count,
                Items = items,
                Pages = (int)Math.Ceiling(count / (double)size)
            };

        return list;
    }

    public static IPaginate<T> ToReadPaginate<T>(this IMongoQueryable<T> source, int index, int size, int from = 0)
    {
        if (from > index)
            throw new ArgumentException($"From: {from} > Index: {index}, must from <= Index");

        int count = source.Count();
        List<T> items = source.Skip((index - from) * size).Take(size).ToList();

        Paginate<T> list =
            new()
            {
                Index = index,
                Size = size,
                From = from,
                Count = count,
                Items = items,
                Pages = (int)Math.Ceiling(count / (double)size)
            };

        return list;
    }
}
