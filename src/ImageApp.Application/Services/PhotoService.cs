using System.Text.Json;
using ImageApp.Application.DTOs.Constants;
using ImageApp.Application.DTOs.Photos;
using ImageApp.Application.Interfaces;
using ImageApp.Application.Interfaces.Repositories;
using ImageApp.Application.Interfaces.Services;
using ImageApp.Domain.Entities;

namespace ImageApp.Application.Services
{
    public class PhotoService(
        IPhotoRepository photoRepository,
        IMediaStorage mediaStorage,
        IRedisCacheService redisCacheService) : IPhotoService
    {
        public async Task<bool> CreatePhoto(CreatePhotoRequest request, Guid userId)
        {
            // Save metadata to DB
            var photo = new Photo
            {
                Id = Guid.NewGuid(),
                CreatorId = userId,
                Title = request.Title,
                Caption = request.Caption,
                Location = request.Location,
                PeoplePresentJson = JsonSerializer.Serialize(request.PeoplePresent),
                CreatedAt = DateTime.UtcNow
            };

            await photoRepository.AddAsync(photo);
            await photoRepository.SaveChangesAsync();

            // Upload image to Blob Storage
            var blobUrl = await mediaStorage.UploadImageAsync(photo.Id, request.Base64Image);
            photo.BlobOriginalUrl = blobUrl;

            await photoRepository.SaveChangesAsync();

            return true;
        }

        public async Task<PhotoSearchResponse>
            SearchAsync(string? search, string? location, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var (items, total) = await photoRepository.SearchAsync(search, location, page, pageSize);

            var mapped = new List<PhotoResponse>();
            foreach (var p in items)
            {
                var photoKey = $"photo-{p.Id.ToString()}";

                await redisCacheService.SetDataAsync<Photo>(photoKey, p, CacheTime.TwentyFourHoursInSeconds, cancellationToken);
                mapped.Add(await MapToResponse(p));
            }

            return new PhotoSearchResponse(mapped, total, page, pageSize);
        }

        public async Task<PhotoResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var photoKey = $"photo-{id.ToString()}";

            var photo = await redisCacheService.GetDataAsync<Photo>(photoKey, cancellationToken);
            if (photo is not null) return await MapToResponse(photo);

            photo = await photoRepository.GetByIdAsync(id);
            await redisCacheService.SetDataAsync<Photo>(photoKey, photo, CacheTime.TwentyFourHoursInSeconds, cancellationToken);

            return await MapToResponse(photo);
        }

        public async Task UpdateCacheCacheAsync(Guid photoId, CancellationToken cancellationToken)
        {
            var photoKey = $"photo-{photoId.ToString()}";

            var photo = await photoRepository.GetByIdAsync(photoId);

            await redisCacheService.SetDataAsync<Photo>(photoKey, photo, CacheTime.TwentyFourHoursInSeconds,
                cancellationToken);
        }

        private async Task<PhotoResponse> MapToResponse(Photo p)
        {
            var originalUrl = await mediaStorage.GetReadUrlAsync(p.BlobOriginalKey, TimeSpan.FromMinutes(15));
            var thumbnailUrl = await mediaStorage.GetReadUrlAsync(p.BlobThumbnailKey, TimeSpan.FromMinutes(15));

            return new PhotoResponse(
                p.Id, p.Title, p.Caption, p.Location,
                JsonSerializer.Deserialize<List<string>>(p.PeoplePresentJson) ?? [],
                originalUrl,
                thumbnailUrl,
                p.AverageRating,
                p.TotalRatings,
                p.CreatedAt
            );
        }
    }
}