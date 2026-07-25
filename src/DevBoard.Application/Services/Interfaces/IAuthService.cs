// src/DevBoard.Application/Services/Interfaces/IAuthService.cs
namespace DevBoard.Application.Services.Interfaces;

public sealed record AuthResult(string AccessToken, string RefreshToken, DateTime ExpiresAt);

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(string email, string password, string displayName, Guid workspaceId, CancellationToken ct = default);
    Task<AuthResult> LoginAsync(string email, string password, CancellationToken ct = default);
    Task<AuthResult> RefreshAsync(string refreshToken, CancellationToken ct = default);
    Task RevokeAsync(string refreshToken, CancellationToken ct = default);
}