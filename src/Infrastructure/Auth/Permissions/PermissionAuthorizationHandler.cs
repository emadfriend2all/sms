using System.Security.Claims;
using Showmatics.Application.Identity.Users;
using Microsoft.AspNetCore.Authorization;
using Showmatics.Shared.Authorization;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;

namespace Showmatics.Infrastructure.Auth.Permissions;

internal class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IUserService _userService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PermissionAuthorizationHandler(IUserService userService, IHttpContextAccessor httpContextAccessor)
    {
        _userService = userService;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        string? userId = context.User?.GetUserId();
        if (userId != null && httpContext != null)
        {
            string accessToken = ReadTokenFromHeader();
            var jwtHandler = new JwtSecurityTokenHandler();

            if (!string.IsNullOrWhiteSpace(accessToken) && jwtHandler.CanReadToken(accessToken))
            {
                var jwt = jwtHandler.ReadJwtToken(accessToken);

                bool isApiToken = jwt.Claims.FirstOrDefault(c => c.Type == FSHClaims.IsApiToken)?.Value == "true";

                var endpoint = httpContext.GetEndpoint();
                string? routeName = endpoint?.Metadata.GetMetadata<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>()?.ActionName;

                if (isApiToken && string.Equals(routeName, "SendSms", StringComparison.OrdinalIgnoreCase))
                {
                    context.Succeed(requirement);
                    return;
                }
            }

            bool hasPermission = await _userService.HasPermissionAsync(userId, requirement.Permission);
            if (hasPermission)
            {
                context.Succeed(requirement);
            }
        }
    }

    private string ReadTokenFromHeader()
    {
        string? authorization = _httpContextAccessor.HttpContext.Request?.Headers["Authorization"];

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