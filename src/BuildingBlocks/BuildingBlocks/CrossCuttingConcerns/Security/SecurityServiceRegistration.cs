using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.EmailAuthenticator;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Encryption;
using TokenOptions = EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.JWT.TokenOptions;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.JWT;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.OtpAuthenticator;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.OtpAuthenticator.OtpNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace EventPAM.BuildingBlocks.CrossCuttingConcerns.Security;

public static class SecurityServiceRegistration
{
    public static WebApplicationBuilder AddSecurityServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ITokenHelper, JwtHelper>();
        builder.Services.AddScoped<IEmailAuthenticatorHelper, EmailAuthenticatorHelper>();
        builder.Services.AddScoped<IOtpAuthenticatorHelper, OtpNetOtpAuthenticatorHelper>();

        TokenOptions? tokenOptions = builder.Configuration.GetSection(Configs.TOKEN_OPTIONS).Get<TokenOptions>();
        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = tokenOptions!.Issuer,
                    ValidAudience = tokenOptions.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = SecurityKeyHelper.CreateSecurityKey(tokenOptions.SecurityKey)
                };
            });
        builder.Services.AddAuthorization();

        return builder;
    }
}
