using ImageApp.Application.DTOs.Admin;
using ImageApp.Application.Interfaces;
using ImageApp.Application.Interfaces.Repositories;
using ImageApp.Application.Interfaces.Services;
using ImageApp.Domain.Enums;

namespace ImageApp.Application.Services;

public class AdminService(IUserRepository usersRepos, ICurrentUser current) : IAdminService
{
    public async Task ChangeRole(ChangeRoleRequest request, CancellationToken ct = default)
    {
        var user = await usersRepos.GetByIdAsync(request.UserId, ct)
                   ?? throw new InvalidOperationException("User not found.");

        if (user.Id == request.UserId)
            throw new InvalidOperationException("Cannot change roles because user is already an admin.");

        if (user.Role == request.UserRole) return;

        user.Role = request.UserRole;
        await usersRepos.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<UserSearchResult>> SearchUsersAsync(string query, CancellationToken ct)
    {
        EnsureAdmin();
        var users = await usersRepos.SearchAsync(query, 25, ct);
        return users.Select(u => new UserSearchResult(u.Id, u.Email, u.DisplayName, u.Role, u.CreatedAt)).ToList();
    }

    private void EnsureAdmin()
    {
        if (!current.IsAuthenticated || current.Role != nameof(UserRole.Admin))
            throw new InvalidOperationException("Admin access required.");
    }
}