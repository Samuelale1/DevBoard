namespace DevBoard.Domain.Entities;

using DevBoard.Domain.Exceptions;

public sealed class Label : BaseEntity
{
    public string Name { get; private set; }

    public string Color { get; private set; }

    public Guid WorkspaceId { get; private set; }

    private Label(
        string name,
        string color,
        Guid workspaceId)
    {
        Name = name;
        Color = color;
        WorkspaceId = workspaceId;
    }

    public static Label Create(
        string name,
        string color,
        Guid workspaceId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ValidationException("Label name is required.");

        return new Label(name, color, workspaceId);
    }
}