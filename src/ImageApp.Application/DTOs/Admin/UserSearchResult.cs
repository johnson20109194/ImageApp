using ImageApp.Domain.Enums;

namespace ImageApp.Application.DTOs.Admin;

public record UserSearchResult(Guid Id, string Email, string DisplayName, UserRole Role, DateTime CreatedAt);