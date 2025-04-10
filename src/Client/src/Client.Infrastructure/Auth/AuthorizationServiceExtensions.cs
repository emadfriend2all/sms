using Microsoft.AspNetCore.Authorization;
using Showmatics.Shared.Authorization;

namespace Showmatics.Blazor.Client.Infrastructure.Auth;

public static class AuthorizationServiceExtensions
{
    public static async Task<bool> HasPermissionAsync(this IAuthorizationService service, ClaimsPrincipal user, string action, string resource)
    {
        var tenant = user.GetCurrentUserTenantInfo();
        bool hasModule = tenant?.Modules?.Any(x => x == resource) ?? false;
        if(!hasModule)
        {
            return false;
        }

        return (await service.AuthorizeAsync(user, null, FSHPermission.NameFor(action, resource))).Succeeded;
    }
}