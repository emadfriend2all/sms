using System.Security.Claims;
using Showmatics.Application.Common.Interfaces;
using Showmatics.Shared.Multitenancy;

namespace Showmatics.Infrastructure.Auth;

public class CurrentUser : ICurrentUser, ICurrentUserInitializer
{
    private ClaimsPrincipal? _user;

    public string? Name => _user?.Identity?.Name;

    private Guid _userId = Guid.Empty;

    public Guid GetUserId() =>
        IsAuthenticated()
            ? Guid.Parse(_user?.GetUserId() ?? Guid.Empty.ToString())
            : _userId;

    public string? GetUserEmail() =>
        IsAuthenticated()
            ? _user!.GetEmail()
            : string.Empty;

    public int? GetMemberId()
    {
        if (IsAuthenticated())
        {
            string? memberId = _user!.GetMemberId();
            if (!string.IsNullOrEmpty(memberId))
            {
                return int.Parse(memberId);
            }
        }

        return null;
    }

    public TenantBasicInfoDto? GetCurrentUserTenantInfo()
    {
        if (IsAuthenticated())
        {
            return _user!.GetCurrentUserTenantInfo();
        }

        return null;
    }

    public bool IsAuthenticated() =>
        _user?.Identity?.IsAuthenticated is true;

    public bool IsInRole(string role) =>
        _user?.IsInRole(role) is true;

    public IEnumerable<Claim>? GetUserClaims() =>
        _user?.Claims;

    public string? GetTenant() =>
        IsAuthenticated() ? _user?.GetTenant() : string.Empty;

    public void SetCurrentUser(ClaimsPrincipal user)
    {
        if (_user != null)
        {
            throw new Exception("Method reserved for in-scope initialization");
        }

        _user = user;
    }

    public void SetCurrentUserId(string userId)
    {
        if (_userId != Guid.Empty)
        {
            throw new Exception("Method reserved for in-scope initialization");
        }

        if (!string.IsNullOrEmpty(userId))
        {
            _userId = Guid.Parse(userId);
        }
    }

    public string? GetUserName() =>
        IsAuthenticated()
            ? _user!.GetUserName()
            : string.Empty;

    public string? GetApiToken() =>
        IsAuthenticated()
            ? _user!.GetApiToken()
            : string.Empty;
}