using DevBoard.Domain.Enums;
using DevBoard.Domain.ValueObjects;

namespace DevBoard.Domain.Entities
{
    public class Issue: BaseEntity
    {
       public required string Title { get; set; }

        public string? Description { get; set; }

         public IssueStatus Status { get; private set; }
        = IssueStatus.Backlog;

         public IssueType Type { get; init; }

         public IssuePriority Priority { get; set; }
        = IssuePriority.None;

         public required string IssueKey { get; init; }

        public Guid ProjectId { get; init; }

        public Guid? AssigneeId { get; set; }
    }
}