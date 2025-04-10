using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Showmatics.Application.Identity.Users;

namespace Showmatics.Infrastructure.Auth.Jwt;

internal static class Startup
{
    internal static IServiceCollection AddJwtAuth(this IServiceCollection services)
    {
        services.AddOptions<JwtSettings>()
            .BindConfiguration($"SecuritySettings:{nameof(JwtSettings)}")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<ApiJwtSettings>()
            .BindConfiguration($"SecuritySettings:{nameof(ApiJwtSettings)}")
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>();

        return services
            .AddAuthentication(authentication =>
            {
                authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, null!)
            .AddScheme<JwtBearerOptions, AppAuthJWTAuthenticationHandler>("ApiToken", options => { })
            .Services;
    }
}

public class AppAuthJWTAuthenticationHandler : JwtBearerHandler
{
    private readonly IUserService _userService;

    public AppAuthJWTAuthenticationHandler(
        IOptionsMonitor<JwtBearerOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IUserService userService
        )
        : base(options, logger, encoder, clock)
    {
        _userService = userService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string accessToken = ReadTokenFromHeader();

        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return AuthenticateResult.NoResult();
        }

        var jwtHandler = new JwtSecurityTokenHandler();

        if (jwtHandler.CanReadToken(accessToken))
        {
            try
            {
                return await base.HandleAuthenticateAsync();
                //var token = jwtHandler.ReadJwtToken(accessToken);
                //if (token.Issuer == Options.TokenValidationParameters.ValidIssuer)
                //{
                //    return await base.HandleAuthenticateAsync();
                //}
            }
            catch
            {
                return AuthenticateResult.Fail("Invalid API Token");
            }
        }

        return AuthenticateResult.Fail("Invalid API Token");
    }

    private string ReadTokenFromHeader()
    {
        string? authorization = Request?.Headers["Authorization"];

        if (string.IsNullOrEmpty(authorization))
        {
            return string.Empty;
        }

        if (authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return authorization["Bearer ".Length..].Trim();
        }

        return string.Empty;

    }
}