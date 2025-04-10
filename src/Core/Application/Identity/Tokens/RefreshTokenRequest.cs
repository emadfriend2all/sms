namespace Showmatics.Application.Identity.Tokens;

public record RefreshTokenRequest(string Token, string RefreshToken);

public record ApiTokenGenerationCommand(string UserId);
public record ApiTokenGenerationResponse(string Token);