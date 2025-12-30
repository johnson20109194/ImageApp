using ImageApp.Domain.Enums;

namespace ImageApp.Application.DTOs.Admin;

public class ChangeRoleRequest
{
    public required UserRole UserRole { get; set; }
    public required Guid UserId { get; set; }
}