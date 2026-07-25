// src/DevBoard.Application/Services/Implementations/AuthService.cs
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DevBoard.Application.Options;
using DevBoard.Application.Services.Interfaces;
using DevBoard.Domain.Entities;
using DevBoard.Domain.Exceptions;
using DevBoard.Domain.Interfaces;
using DevBoard.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DevBoard.Application.Services.Implementations;

public sealed class AuthService : IAuthService
{
    private readonly IRepository<User> _users;
    private readonly IRepository<RefreshToken> _refreshTokens;
    private readonly JwtOptions _options;

    public AuthService(IRepository<User> users, IRepository<RefreshToken> refreshTokens, IOptions<JwtOptions> options)
    {
        _users = users;
        _refreshTokens = refreshTokens;
        _options = options.Value;
    }

    public async Task<AuthResult> RegisterAsync(string email, string password, string displayName, Guid workspaceId, CancellationToken ct = default)
    {
        var normalizedEmail = Email.From(email);
        var exists = await _users.Query().AnyAsync(u => u.Email.Value == normalizedEmail.Value, ct);
        if (exists) throw new ConflictException("A user with that email already exists.");

        var hash = BCrypt.Net.BCrypt.HashPassword(password);
        var user = User.Create(normalizedEmail, displayName, hash, Domain.Enums.UserRole.Member, workspaceId);

        await _users.AddAsync(user, ct);
        await _users.SaveChangesAsync(ct);

        return await IssueTokensAsync(user, ct);
    }

    public async Task<AuthResult> LoginAsync(string email, string password, CancellationToken ct = default)
    {
        var normalized = Email.From(email).Value;
        var user = await _users.Query().FirstOrDefaultAsync(u => u.Email.Value == normalized, ct)
            ?? throw new UnauthorizedException("Invalid email or password.");

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            throw new UnauthorizedException("Invalid email or password.");

        return await IssueTokensAsync(user, ct);
    }

    public async Task<AuthResult> RefreshAsync(string refreshToken, CancellationToken ct = default)
    {
        var stored = await _refreshTokens.Query().FirstOrDefaultAsync(r => r.Token == refreshToken, ct)
            ?? throw new UnauthorizedException("Invalid refresh token.");

        if (!stored.IsActive) throw new UnauthorizedException("Refresh token is expired or revoked.");

        stored.Revoke();
        _refreshTokens.Update(stored);

        var user = await _users.GetByIdAsync(stored.UserId, ct)
            ?? throw new NotFoundException("User not found.");

        var result = await IssueTokensAsync(user, ct);
        return result;
    }

    public async Task RevokeAsync(string refreshToken, CancellationToken ct = default)
    {
        var stored = await _refreshTokens.Query().FirstOrDefaultAsync(r => r.Token == refreshToken, ct);
        if (stored is null) return;
        stored.Revoke();
        _refreshTokens.Update(stored);
        await _refreshTokens.SaveChangesAsync(ct);
    }

    private async Task<AuthResult> IssueTokensAsync(User user, CancellationToken ct)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_options.AccessTokenMinutes);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email.Value),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(_options.Issuer, _options.Audience, claims, expires: expiresAt, signingCredentials: creds);
        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        var refreshTokenValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var refreshToken = RefreshToken.Create(refreshTokenValue, user.Id, TimeSpan.FromDays(_options.RefreshTokenDays));

        await _refreshTokens.AddAsync(refreshToken, ct);
        await _refreshTokens.SaveChangesAsync(ct);

        return new AuthResult(accessToken, refreshTokenValue, expiresAt);
    }
}