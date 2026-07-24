using DevBoard.Application.Services.Interfaces;
using DevBoard.Domain.Entities;
using DevBoard.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace DevBoard.Application.Services.Implementations;

public sealed class ProjectService : IProjectService
{
    private readonly IRepository<Project> _repository;
    private readonly ILogger<ProjectService> _logger;

    public ProjectService(IRepository<Project> repository, ILogger<ProjectService> logger)
    {
        _repository = repository;
        _logger = logger;
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
        
        _logger.LogInformation("Created project {ProjectName}",project.Name);

        return project;
    }
}