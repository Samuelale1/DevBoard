// src/DevBoard.Application/Services/Interfaces/IDailyDigestService.cs
namespace DevBoard.Application.Services.Interfaces;

public interface IDailyDigestService
{
    Task SendAllAsync(CancellationToken ct = default);
}