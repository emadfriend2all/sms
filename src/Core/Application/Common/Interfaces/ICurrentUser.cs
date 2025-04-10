using Showmatics.Shared.Multitenancy;
using System.Security.Claims;

namespace Showmatics.Application.Common.Interfaces;

public interface ICurrentUser
{
    string? Name { get; }

    Guid GetUserId();
    string? GetUserName();

    string? GetUserEmail();

    string? GetTenant();

    int? GetMemberId();

    bool IsAuthenticated();

    bool IsInRole(string role);

    IEnumerable<Claim>? GetUserClaims();
    TenantBasicInfoDto? GetCurrentUserTenantInfo();
    string? GetApiToken();
}