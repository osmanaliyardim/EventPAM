namespace EventPAM.BuildingBlocks.Web;

public static class CookiesManagement
{
    public static void PassRefreshTokenToCookies(string token, IHttpContextAccessor context)
    {
        context.HttpContext!.Response.Cookies.Append(key: "refreshToken", token, SetCookieOptions());
    }

    private static CookieOptions SetCookieOptions()
    {
        var cookieOptions = new CookieOptions()
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddDays(7)
        };

        return cookieOptions;
    }
}
