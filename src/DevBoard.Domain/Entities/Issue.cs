using DevBoard.Domain.Enums;
using DevBoard.Domain.ValueObjects;
using DevBoard.Domain.Services;
using DevBoard.Domain.Exceptions;

namespace DevBoard.Domain.Entities
{
    public sealed class Issue: BaseEntity
    {
       public  string Title { get; private set; }

        public string? Description { get; set; }

         public IssueStatus Status { get; private set; }

         public IssueType Type { get; private set; }

         public IssuePriority Priority { get; private set; }

         public  string IssueKey { get; init; }= string.Empty;

        public Guid ProjectId { get; init; }

        public Guid? AssigneeId { get; set; }
        public List<Comment> Comments { get; } = [];
        public List<Label> Labels { get; } = [];
        // TO DO: public List<IssueHistory> History { get; } = [];

        private Issue()
        {
            
        }
         private Issue
         (
            string title, string? description, IssueType type, IssuePriority priority, string issueKey,Guid projectId
        )
    {
        Title = title;
        Description = description;
        Type = type;
        Priority = priority;
        IssueKey = issueKey;
        ProjectId = projectId;
        Status = IssueStatus.Backlog;

        
    }
/* 
* A factory method to create a new Issue instance with validation.
* This method ensures that the title, issue key, and project ID are provided and valid before creating a new Issue object.
 */
    public static Issue Create(
    string title,
    string? description,
    IssueType type,
    IssuePriority priority,
    string issueKey,
    Guid projectId)
{
    // validation
    if (string.IsNullOrWhiteSpace(title))
    {
        throw new ValidationException("Issue title is required.");
    }

    if (string.IsNullOrWhiteSpace(issueKey))
    {
        throw new ValidationException("Issue key is required.");
    }

    if (projectId == Guid.Empty)
    {
        throw new ValidationException("Project Id is required.");
    }
    

    return new Issue(
        title,
        description,
        type,
        priority,
        issueKey,
        projectId);

        
}

     public void TransitionTo(IssueStatus newStatus)
    {
        if (!IssueStateMachine.IsValidTransition(Status, newStatus))
        {
            throw new InvalidIssueTransitionException(Status, newStatus);
        }

        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }

    }
}