namespace DevBoard.Domain.Entities;

using DevBoard.Domain.Enums;
using DevBoard.Domain.Exceptions;
using DevBoard.Domain.ValueObjects;

public sealed class User : BaseEntity
{
    public Email Email { get; private set; }

    public string DisplayName { get; private set; }

    public string PasswordHash { get; private set; }

    public UserRole Role { get; private set; }

    public Guid WorkspaceId { get; private set; }

    private User(
        Email email,
        string displayName,
        string passwordHash,
        UserRole role,
        Guid workspaceId)
    {
        Email = email;
        DisplayName = displayName;
        PasswordHash = passwordHash;
        Role = role;
        WorkspaceId = workspaceId;
    }

    public static User Create(
        Email email,
        string displayName,
        string passwordHash,
        UserRole role,
        Guid workspaceId)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ValidationException("Display name is required.");

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ValidationException("Password hash is required.");

        if (workspaceId == Guid.Empty)
            throw new ValidationException("Workspace is required.");

        return new User(
            email,
            displayName,
            passwordHash,
            role,
            workspaceId);
    }
}