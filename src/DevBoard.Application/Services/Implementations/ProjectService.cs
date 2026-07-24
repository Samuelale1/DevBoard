using DevBoard.Application.Services.Interfaces;
using DevBoard.Domain.Entities;
using DevBoard.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevBoard.Application.Services.Implementations;

public sealed class ProjectService : IProjectService
{
    private readonly IRepository<Project> _repository;

    public ProjectService(IRepository<Project> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Project>> GetAllAsync(
        CancellationToken ct = default)
    {
        return await _repository
            .Query()
            .ToListAsync(ct);
    }

    public async Task<Project> CreateAsync(
        Project project,
        CancellationToken ct = default)
    {
        await _repository.AddAsync(project, ct);

        await _repository.SaveChangesAsync(ct);

        return project;
    }
}