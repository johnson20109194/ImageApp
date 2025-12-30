using ImageApp.Application.DTOs.Photos;

namespace ImageApp.Application.Interfaces.Services
{
    public interface IPhotoService
    {
        Task<bool> CreatePhoto(CreatePhotoRequest request, Guid userId);

        Task<PhotoSearchResponse>
            SearchAsync(string? search, string? location, int page, int pageSize,
                CancellationToken cancellationToken = default);

        Task<PhotoResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task UpdateCacheCacheAsync(Guid photoId, CancellationToken cancellationToken);
    }
}