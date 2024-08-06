using Asp.Versioning.Builder;

namespace EventPAM.BuildingBlocks.Web;

public class EndpointConfig
{
    public const string BaseApiPath = Configs.BASE_API_PATH;

    public static ApiVersionSet VersionSet { get; private set; } = default!;
}
