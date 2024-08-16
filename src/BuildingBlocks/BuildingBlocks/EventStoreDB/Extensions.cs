using EventPAM.BuildingBlocks.Web;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace EventPAM.BuildingBlocks.EventStoreDB;

public static class Extensions
{
    public static IServiceCollection AddEventStore(
        this IServiceCollection services,
        IConfiguration configuration,
        params Assembly[] assemblies
    )
    {
        services.AddValidateOptions<EventStoreOptions>();

        var assembliesToScan = assemblies.Length > 0 ? assemblies : [Assembly.GetEntryAssembly()!];

        return services
            .AddEventStoreDB(configuration)
            .AddProjections(assembliesToScan);
    }
}
