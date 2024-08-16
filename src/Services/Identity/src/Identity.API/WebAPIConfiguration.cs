namespace EventPAM.Identity.API;

public class WebAPIConfiguration
{
    public string APIDomain { get; set; } = default!;

    public string[] AllowedOrigins { get; set; } = default!;
}
