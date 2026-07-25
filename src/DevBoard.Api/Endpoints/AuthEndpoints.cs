// src/DevBoard.Api/Endpoints/AuthEndpoints.cs
using DevBoard.Api.Contracts;
using DevBoard.Application.Services.Interfaces;

namespace DevBoard.Api.Endpoints;

public static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuth(this RouteGroupBuilder group)
    {
        group.MapPost("/register", Register);
        group.MapPost("/login", Login);
        group.MapPost("/refresh", Refresh);
        group.MapPost("/revoke", Revoke);
        return group;
    }

    private static async Task<IResult> Register(RegisterRequest req, IAuthService auth, CancellationToken ct)
        => Results.Ok(await auth.RegisterAsync(req.Email, req.Password, req.DisplayName, req.WorkspaceId, ct));

    private static async Task<IResult> Login(LoginRequest req, IAuthService auth, CancellationToken ct)
        => Results.Ok(await auth.LoginAsync(req.Email, req.Password, ct));

    private static async Task<IResult> Refresh(RefreshRequest req, IAuthService auth, CancellationToken ct)
        => Results.Ok(await auth.RefreshAsync(req.RefreshToken, ct));

    private static async Task<IResult> Revoke(RefreshRequest req, IAuthService auth, CancellationToken ct)
    {
        await auth.RevokeAsync(req.RefreshToken, ct);
        return Results.NoContent();
    }
}