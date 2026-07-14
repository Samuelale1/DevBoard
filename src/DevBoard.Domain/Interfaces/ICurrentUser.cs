
using DevBoard.Domain.Enums;
namespace DevBoard.Domain.Interfaces;

public interface ICurrentUser
{
    Guid UserId { get; }

    string Email { get; }

    UserRole Role { get; }

    bool IsAdmin { get; }
}