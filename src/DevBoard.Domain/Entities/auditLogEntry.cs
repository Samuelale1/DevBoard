using DevBoard.Domain.Exceptions;
namespace DevBoard.Domain.Entities;



public sealed class AuditLogEntry : BaseEntity
{
    public Guid IssueId { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    private AuditLogEntry() { }

    private AuditLogEntry(Guid issueId, string action, string description)
    {
        IssueId = issueId;
        Action = action;
        Description = description;
    }

    // Factory Method
    public static AuditLogEntry Create(Guid issueId, string action, string description)
    {
        if (string.IsNullOrWhiteSpace(action))
            throw new ValidationException("Audit log action is required.");

        return new AuditLogEntry(issueId, action, description);
    }
}