using ImageApp.Application.DTOs.Admin;

namespace ImageApp.Application.Interfaces.Services
{
    public interface IAdminService
    {
        Task<IReadOnlyList<UserSearchResult>> SearchUsersAsync(string query, CancellationToken ct);
        Task ChangeRole(ChangeRoleRequest request, CancellationToken cancellationToken = default);
    }
}