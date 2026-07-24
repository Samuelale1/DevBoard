// src/DevBoard.Application/Options/SmtpOptions.cs
namespace DevBoard.Application.Options;
public sealed class SmtpOptions
{
    public required string Host { get; init; }
    public required int Port { get; init; }
    public required string From { get; init; }
    public string? Username { get; init; }
    public string? Password { get; init; }
}