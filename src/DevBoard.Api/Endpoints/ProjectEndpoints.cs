// src/DevBoard.Api/Endpoints/ProjectEndpoints.cs
using DevBoard.Api.Contracts;
using DevBoard.Application.Services.Interfaces;
using DevBoard.Domain.Entities;
using DevBoard.Domain.ValueObjects;
using DevBoard.Api.Validators;

namespace DevBoard.Api.Endpoints;

public static class ProjectEndpoints
{
    public static RouteGroupBuilder MapProjects(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetAll).WithName("GetProjects");
        group.MapPost("/", Create).WithName("CreateProject").AddEndpointFilter<ValidationFilter<CreateProjectRequest>>();
        return group;
    }

    private static async Task<IResult> GetAll(IProjectService service, CancellationToken ct)
    {
        var projects = await service.GetAllAsync(ct);
        return Results.Ok(projects.Select(ProjectResponse.FromDomain));
    }

    private static async Task<IResult> Create(CreateProjectRequest req, IProjectService service, CancellationToken ct)
    {
        var project = Project.Create(req.Name, Slug.From(req.Slug), req.WorkspaceId);
        var created = await service.CreateAsync(project, ct);
        return Results.Created($"/api/projects/{created.Id}", ProjectResponse.FromDomain(created));
    }
}