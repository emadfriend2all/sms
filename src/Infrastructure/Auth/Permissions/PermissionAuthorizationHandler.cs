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
        string? userId = context.User?.GetUserId();
        if (userId != null)
        {
            bool hasPermission = await _userService.HasPermissionAsync(userId, requirement.Permission);
            if (hasPermission)
            {
                context.Succeed(requirement);
            }
        }
    }
}