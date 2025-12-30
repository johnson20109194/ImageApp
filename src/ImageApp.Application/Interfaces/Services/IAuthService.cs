using ImageApp.Application.DTOs;
using ImageApp.Application.DTOs.Auth;

namespace ImageApp.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<GeneralResponse<AuthResponse>> LoginAsync(LoginRequest req, CancellationToken cancellationToken);

        Task<GeneralResponse<AuthResponse>> RegisterAsync(RegisterRequest req, CancellationToken cancellationToken);

        Task<GeneralResponse<AuthResponse>> GetMyInfoAsync(CancellationToken cancellationToken);
        Task UpdateUserCacheAsynce(Guid userId, CancellationToken cancellationToken);
    }
}