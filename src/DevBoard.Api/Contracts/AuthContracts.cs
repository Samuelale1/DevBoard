// src/DevBoard.Api/Contracts/AuthContracts.cs
namespace DevBoard.Api.Contracts;

public sealed record RegisterRequest(string Email, string Password, string DisplayName, Guid WorkspaceId);
public sealed record LoginRequest(string Email, string Password);
public sealed record RefreshRequest(string RefreshToken);