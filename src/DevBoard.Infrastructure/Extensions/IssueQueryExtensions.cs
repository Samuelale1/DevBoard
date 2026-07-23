using DevBoard.Domain.Entities;
using DevBoard.Domain.Enums;
using DevBoard.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;



namespace DevBoard.Infrastructure.Extensions;
/* This static class provides extension methods for querying issues in a more readable and expressive way.
* The methods in this class extend the IQueryable<Issue> interface, allowing for filtering, sorting, and pagination of issues based on various criteria.
* Each method returns an IQueryable<Issue> that can be further composed with other LINQ methods, enabling developers to build complex queries in a fluent manner.
* The methods include filtering by project, status, assignee, label, and text search, as well as ordering by priority and paginating the results.
* This class is designed to enhance the usability of the Issue entity in the context of a repository pattern, making it easier to work with issues in a consistent and efficient manner.
 */

public static class IssueQueryExtensions
{
    public static IQueryable<Issue> ForProject(
    this IQueryable<Issue> query,
    Guid projectId)
        {
            return query.Where(i => i.ProjectId == projectId);
        }

    public static IQueryable<Issue> WithStatus(
    this IQueryable<Issue> query,
    IssueStatus status)
        {
            return query.Where(i => i.Status == status);
        }

    public static IQueryable<Issue> AssignedTo(
    this IQueryable<Issue> query,
    Guid userId)
        {
            return query.Where(i => i.AssigneeId == userId);
        }

    public static IQueryable<Issue> OrderedByPriority(
    this IQueryable<Issue> query)
        {
            return query.OrderByDescending(i => i.Priority.Level);
        }

    public static IQueryable<Issue> WithLabel(
    this IQueryable<Issue> query,
    string labelName)
        {
            return query.Where(i =>
                i.Labels.Any(l => l.Name == labelName));
        }

    public static IQueryable<Issue> ContainingText(
    this IQueryable<Issue> query,
    string searchTerm)
        {
            return query.Where(i =>
                EF.Functions.ILike(i.Title, $"%{searchTerm}%") ||

                (i.Description != null &&
                EF.Functions.ILike(i.Description, $"%{searchTerm}%")));
        } 

    public static Task<PagedList<Issue>> PagedAsync(
    this IQueryable<Issue> query,
    int page,
    int size,
    CancellationToken ct = default)
        {
            return PagedList<Issue>.CreateAsync(
                query,
                page,
                size,
                ct);
        }   



}