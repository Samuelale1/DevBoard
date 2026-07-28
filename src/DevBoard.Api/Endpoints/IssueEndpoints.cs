// src/DevBoard.Api/Endpoints/IssueEndpoints.cs
using DevBoard.Api.Contracts;
using DevBoard.Application.Services.Interfaces;
using DevBoard.Domain.ValueObjects;
using FluentValidation;
using DevBoard.Api.Validators;
using System.Text.Json;

namespace DevBoard.Api.Endpoints;

public static class IssueEndpoints
{
    public static RouteGroupBuilder MapIssues(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetForProject).WithName("GetIssues").WithSummary("List issues for a project").Produces<IEnumerable<ProjectResponse>>(200);
        group.MapGet("/{id:guid}", GetById).WithName("GetIssueById");

        group.MapGet("/key/{issueKey}", GetByKey).WithName("GetIssueByKey");

        group.MapPost("/", Create).WithName("CreateIssue")
            .Produces<ProjectResponse>(201)
            .ProducesValidationProblem(422).AddEndpointFilter<ValidationFilter<CreateIssueRequest>>();

        group.MapPatch("/{id:guid}/status", ChangeStatus).WithName("ChangeIssueStatus");
        group.MapGet("/{id:guid}/audit-log", StreamAuditLog).WithName("GetIssueAuditLog");
        // IssueEndpoints.cs — add to MapIssues()
        group.MapPost("/import", ImportCsv).WithName("ImportIssuesCsv").DisableAntiforgery();
        return group;
    }

    
    private static async Task<IResult> GetForProject(
        Guid projectId, int page, int pageSize, IIssueService service, CancellationToken ct)
    {
        var result = await service.GetProjectIssuesAsync(projectId, page <= 0 ? 1 : page, pageSize <= 0 ? 20 : pageSize, ct);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetById(Guid id, IIssueService service, CancellationToken ct)
    {
        var issue = await service.GetByIdAsync(id, ct);
        return issue is null ? Results.NotFound() : Results.Ok(IssueResponse.FromDomain(issue));
    }

    private static async Task<IResult> GetByKey(string issueKey, IIssueService service, CancellationToken ct)
    {
        var issue = await service.GetByKeyAsync(issueKey, ct);
        return issue is null ? Results.NotFound() : Results.Ok(IssueResponse.FromDomain(issue));
    }

    private static async Task<IResult> Create(CreateIssueRequest req, IIssueService service, CancellationToken ct)
    {
          
        var priority = IssuePriority.From(req.Priority);
        var issue = await service.CreateAsync(req.ProjectId, req.Title, req.Description, req.Type, priority, ct);
        return Results.Created($"/api/issues/{issue.Id}", IssueResponse.FromDomain(issue));
    }

    private static async Task<IResult> ChangeStatus(Guid id, IssueStatusRequest req, IIssueService service, CancellationToken ct)
    {
        await service.ChangeStatusAsync(id, req.NewStatus, ct);
        return Results.NoContent();// a patch return 204 no content so better.
    }
    private static IResult StreamAuditLog(Guid id, IIssueService service, CancellationToken ct)
    {
        return Results.Stream(async stream =>
        {
            var entries = service.StreamAuditLogAsync(id, ct);
            await JsonSerializer.SerializeAsync(stream, entries, cancellationToken: ct);
        }, contentType: "application/json");
    }

    private static async Task<IResult> ImportCsv(
    Guid projectId, IFormFile file, IIssueService service, CancellationToken ct)
    {
        await using var stream = file.OpenReadStream();
        var count = await service.ImportCsvAsync(projectId, stream, ct);
        return Results.Ok(new { imported = count });
    }
}