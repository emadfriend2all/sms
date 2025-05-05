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
using Finbuckle.MultiTenant;
using Showmatics.Infrastructure.Multitenancy;
using Showmatics.Shared.Authorization;
using Showmatics.Shared.Multitenancy;
using System.Text.Json;
using Mapster;

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
    public AppAuthJWTAuthenticationHandler(
        IOptionsMonitor<JwtBearerOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IUserService userService)
        : base(options, logger, encoder, clock)
    {
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
                var jwt = jwtHandler.ReadJwtToken(accessToken);

                string? tenantId = jwt.Claims.FirstOrDefault(c => c.Type == FSHClaims.Tenant)?.Value;
                string? userId = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                bool isApiToken = jwt.Claims.FirstOrDefault(c => c.Type == FSHClaims.IsApiToken)?.Value == "true";

                if (!isApiToken)
                {
                    return await base.HandleAuthenticateAsync();
                }

                if (tenantId == null && userId == null)
                {
                    return AuthenticateResult.Fail("Invalid API Token");
                }

                var scopeFactory = Request.HttpContext.RequestServices.GetRequiredService<IServiceScopeFactory>();
                var scope = scopeFactory.CreateScope();

                var tenantStore = scope.ServiceProvider.GetRequiredService<IMultiTenantStore<FSHTenantInfo>>();
                var tenantInfo = await tenantStore.TryGetAsync(tenantId);
                if (tenantInfo == null)
                {
                    return AuthenticateResult.Fail("Tenant not found");
                }

                var tenantAccessor = scope.ServiceProvider.GetRequiredService<IMultiTenantContextAccessor>();
                tenantAccessor.MultiTenantContext = new MultiTenantContext<FSHTenantInfo>
                {
                    TenantInfo = tenantInfo
                };

                var currentUserInitializer = scope.ServiceProvider.GetRequiredService<ICurrentUserInitializer>();
                currentUserInitializer.SetCurrentUserId(userId!);

                Request.HttpContext.RequestServices = scope.ServiceProvider;
                var userService = Request.HttpContext.RequestServices.GetRequiredService<IUserService>();
                var user = await userService.GetUserByApiToken(userId!, accessToken, CancellationToken.None);
                if (user != null)
                {
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new(FSHClaims.Tenant, tenantId!),
                    };

                    var identity = new ClaimsIdentity(claims, Scheme.Name);
                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal, Scheme.Name);

                    return AuthenticateResult.Success(ticket);
                }

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