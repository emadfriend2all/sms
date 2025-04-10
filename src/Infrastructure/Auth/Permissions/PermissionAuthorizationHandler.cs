using System.Security.Claims;
using Showmatics.Application.Identity.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Showmatics.Shared.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;

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
        var token = httpContext?.Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

        if (!string.IsNullOrEmpty(token))
        {
            if (IsTokenExpired(token))
            {
                var user = await _userService.GetUserByApiToken(token, CancellationToken.None);
                if (user == null)
                {
                    return;
                }

                context.Succeed(requirement);
                return;
            }
        }

        var userId = context.User?.GetUserId();
        if (userId != null)
        {
            var hasPermission = await _userService.HasPermissionAsync(userId, requirement.Permission);
            if (hasPermission)
            {
                context.Succeed(requirement);
            }
        }
    }

    private bool IsTokenExpired(string token)
    {
        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();

        if (!handler.CanReadToken(token))
            return true; // not a valid JWT, could be API key

        var jwtToken = handler.ReadJwtToken(token);
        var exp = jwtToken.ValidTo;
        var expUnix = 1744281040;
        var expiryDate = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;
        // Tokens are in UTC
        return jwtToken.ValidTo < DateTime.UtcNow;
    }
}