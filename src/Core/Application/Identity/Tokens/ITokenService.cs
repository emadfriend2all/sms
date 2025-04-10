namespace Showmatics.Application.Identity.Tokens;

public interface ITokenService : ITransientService
{
    Task<ApiTokenGenerationResponse> GenerateApiTokenAsync(ApiTokenGenerationCommand request, CancellationToken cancellationToken);
    Task<TokenResponse> GetTokenAsync(TokenRequest request, string ipAddress, CancellationToken cancellationToken);

    Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request, string ipAddress);
}