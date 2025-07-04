using System.Security.Claims;
using Showmatics.Application.Identity.Tokens;

namespace Showmatics.Host.Controllers.Identity;

public sealed class TokensController : VersionNeutralApiController
{
    private readonly ITokenService _tokenService;

    public TokensController(ITokenService tokenService) => _tokenService = tokenService;

    [HttpPost]
    [AllowAnonymous]
    [TenantIdHeader]
    [OpenApiOperation("Request an access token using credentials.", "")]
    public Task<TokenResponse> GetTokenAsync(TokenRequest request, CancellationToken cancellationToken)
    {
        return _tokenService.GetTokenAsync(request, GetIpAddress()!, cancellationToken);
    }

    [HttpPost("GenerateApiToken")]
    [Authorize]
    [TenantIdHeader]
    [OpenApiOperation("Request an api token.", "")]
    public Task<ApiTokenGenerationResponse> GetApiTokenAsync(CancellationToken cancellationToken)
    {
        string? userId = User.GetUserId();
        var request = new ApiTokenGenerationCommand(userId!);
        return _tokenService.GenerateApiTokenAsync(request, cancellationToken);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    [TenantIdHeader]
    [OpenApiOperation("Request an access token using a refresh token.", "")]
    [ApiConventionMethod(typeof(FSHApiConventions), nameof(FSHApiConventions.Search))]
    public Task<TokenResponse> RefreshAsync(RefreshTokenRequest request)
    {
        return _tokenService.RefreshTokenAsync(request, GetIpAddress()!);
    }

    private string? GetIpAddress() =>
        Request.Headers.ContainsKey("X-Forwarded-For")
            ? Request.Headers["X-Forwarded-For"]
            : HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "N/A";
}