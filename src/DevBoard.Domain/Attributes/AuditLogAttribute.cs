namespace DevBoard.Domain.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public sealed class AuditLogAttribute
    : Attribute
{
    public string Action { get; }

    public AuditLogAttribute(string action)
    {
        Action = action;
    }
}