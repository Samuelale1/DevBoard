namespace DevBoard.Domain.Enums;

public enum WebhookEvent
{
    IssueCreated,
    IssueUpdated,
    IssueDeleted,

    IssueAssigned,
    IssueUnassigned,

    IssueStatusChanged,

    CommentCreated,
    CommentDeleted,

    LabelCreated,
    LabelDeleted,

    ProjectCreated,
    ProjectUpdated,
    ProjectDeleted
}