// src/DevBoard.Api/Hubs/BoardHub.cs
using Microsoft.AspNetCore.SignalR;

namespace DevBoard.Infrastructure.Hubs;

public sealed class BoardHub : Hub
{
    public async Task JoinProject(string projectId)
        => await Groups.AddToGroupAsync(Context.ConnectionId, $"project:{projectId}");

    public async Task LeaveProject(string projectId)
        => await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"project:{projectId}");
}