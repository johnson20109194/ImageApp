using ImageApp.Domain.Enums;

namespace ImageApp.Application.Interfaces;

public interface ICurrentUser
{
    Guid UserId { get; }
    string? Email { get; }
    string? DisplayName { get; }
    string? Role { get; }
    bool IsAuthenticated { get; }
}