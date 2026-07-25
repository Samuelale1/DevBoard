
namespace DevBoard.Domain.Entities;

public sealed class RefreshToken : BaseEntity
{
    public string Token { get; private set; } = string.Empty;
    public Guid UserId { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public bool IsRevoked => RevokedAt is not null;
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => !IsRevoked && !IsExpired;

    private RefreshToken() { }
    private RefreshToken(string token, Guid userId, DateTime expiresAt)
    {
        Token = token; UserId = userId; ExpiresAt = expiresAt;
    }

    public static RefreshToken Create(string token, Guid userId, TimeSpan lifetime)
        => new(token, userId, DateTime.UtcNow.Add(lifetime));

    public void Revoke() => RevokedAt = DateTime.UtcNow;
}