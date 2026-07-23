using DevBoard.Domain.Entities;

namespace DevBoard.Application.Services.Interfaces;

public interface IProjectService
{
    Task<IEnumerable<Project>> GetAllAsync(
        CancellationToken ct = default);

    Task<Project> CreateAsync(
        Project project,
        CancellationToken ct = default);
}