// src/DevBoard.Application/Options/JwtOptions.cs
namespace DevBoard.Application.Options;
public sealed class JwtOptions
{
    public required string Secret { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public int AccessTokenMinutes { get; init; } = 15;
    public int RefreshTokenDays { get; init; } = 7;
}