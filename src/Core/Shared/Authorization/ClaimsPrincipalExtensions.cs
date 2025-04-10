using Showmatics.Shared.Authorization;
using Showmatics.Shared.Multitenancy;

namespace System.Security.Claims;

public static class ClaimsPrincipalExtensions
{
    public static string? GetEmail(this ClaimsPrincipal principal)
        => principal.FindFirstValue(ClaimTypes.Email);

    public static string? GetTenant(this ClaimsPrincipal principal)
        => principal.FindFirstValue(FSHClaims.Tenant);

    public static string? GetFullName(this ClaimsPrincipal principal)
        => principal?.FindFirst(FSHClaims.Fullname)?.Value;

    public static string? GetFirstName(this ClaimsPrincipal principal)
        => principal?.FindFirst(ClaimTypes.Name)?.Value;

    public static string? GetSurname(this ClaimsPrincipal principal)
        => principal?.FindFirst(ClaimTypes.Surname)?.Value;

    public static string? GetPhoneNumber(this ClaimsPrincipal principal)
        => principal.FindFirstValue(ClaimTypes.MobilePhone);

    public static string? GetUserId(this ClaimsPrincipal principal)
       => principal.FindFirstValue(ClaimTypes.NameIdentifier);

    public static string? GetUserName(this ClaimsPrincipal principal)
       => principal.FindFirstValue(ClaimTypes.GivenName);
    public static string? GetApiToken(this ClaimsPrincipal principal)
       => principal.FindFirstValue(FSHClaims.ApiToken);

    public static string? GetImageUrl(this ClaimsPrincipal principal)
       => principal.FindFirstValue(FSHClaims.ImageUrl);

    public static DateTimeOffset GetExpiration(this ClaimsPrincipal principal) =>
        DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(
            principal.FindFirstValue(FSHClaims.Expiration)));

    private static string? FindFirstValue(this ClaimsPrincipal principal, string claimType) =>
        principal is null
            ? throw new ArgumentNullException(nameof(principal))
            : principal.FindFirst(claimType)?.Value;

    public static string? GetMemberId(this ClaimsPrincipal principal) =>
        principal?.FindFirst(FSHClaims.MemberId)?.Value;

    public static TenantBasicInfoDto? GetCurrentUserTenantInfo(this ClaimsPrincipal principal)
    {
        string? json = principal?.FindFirst(FSHClaims.TenantInfo)?.Value;
        return string.IsNullOrEmpty(json)
            ? null
            : Text.Json.JsonSerializer.Deserialize<TenantBasicInfoDto>(json);
    }
}