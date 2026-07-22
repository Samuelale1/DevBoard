namespace DevBoard.Domain.Entities;
using DevBoard.Domain.Enums;

public sealed class Webhook : BaseEntity
{
    public string TargetUrl { get; private set; }

    public List<WebhookEvent> Events { get; } = [];

    public Guid WorkspaceId { get; private set; }


    private Webhook()
    {
    }
    private Webhook(
        string targetUrl,
        Guid workspaceId)
    {
        TargetUrl = targetUrl;
        WorkspaceId = workspaceId;
    }

    public static Webhook Create(
        string targetUrl,
        Guid workspaceId)
    {
        return new Webhook(targetUrl, workspaceId);
    }
}