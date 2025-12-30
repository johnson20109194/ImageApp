using System.Text.Json;
using Microsoft.AspNetCore.Http;
using ImageApp.Application.DTOs.Constants;
using ImageApp.Application.DTOs.Photos;
using ImageApp.Application.Interfaces;
using ImageApp.Application.Interfaces.Repositories;
using ImageApp.Application.Interfaces.Services;
using ImageApp.Domain.Entities;
using ImageApp.Domain.Enums;

namespace ImageApp.Application.Services;

public sealed class CreatorService(
    ICurrentUser currentUser,
    IMediaStorage media,
    IPhotoRepository photoWriteRepo,
    IRedisCacheService redisCacheService)
    : ICreatorService
{
    // you add below

    public async Task<Guid> UploadPhotoAsync(CreatePhotoRequest req, IFormFile file, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated)
            throw new InvalidOperationException("Login required.");

        if (currentUser.Role is not (nameof(UserRole.Creator) or nameof(UserRole.Admin)))
            throw new InvalidOperationException("Creator access required.");

        var title = (req.Title ?? "").Trim();
        if (title.Length < 2) throw new InvalidOperationException("Title is required.");
        if (title.Length > 200) throw new InvalidOperationException("Title too long.");

        if (file is null || file.Length == 0)
            throw new InvalidOperationException("A photo file is required.");

        if (file.Length > 10 * 1024 * 1024)
            throw new InvalidOperationException("Max upload size is 10MB for this MVP.");

        var contentType = file.ContentType?.ToLowerInvariant() ?? "";
        var allowed = new[] { "image/jpeg", "image/png", "image/webp" };
        if (!allowed.Contains(contentType))
            throw new InvalidOperationException("Only JPEG, PNG, or WEBP files are allowed.");

        var ext = contentType switch
        {
            "image/jpeg" => ".jpg",
            "image/png" => ".png",
            "image/webp" => ".webp",
            _ => ".img"
        };

        await media.EnsureContainersExistAsync(ct);

        // Upload original
        await using var originalStream = file.OpenReadStream();
        var originalKey = await media.UploadOriginalAsync(originalStream, contentType, ext, ct);

        // Generate thumbnail (simple MVP)
        // Note: we need to re-open stream because we consumed it.
        await using var originalStream2 = file.OpenReadStream();
        await using var thumbStream = await ThumbnailHelper.CreateThumbnailAsync(originalStream2, contentType, 480, ct);

        var thumbKey = await media.UploadThumbnailAsync(thumbStream, contentType, ext, ct);

        var photo = new Photo
        {
            Id = Guid.NewGuid(),
            Title = title,
            Caption = string.IsNullOrWhiteSpace(req.Caption) ? null : req.Caption.Trim(),
            Location = string.IsNullOrWhiteSpace(req.Location) ? null : req.Location.Trim(),
            PeoplePresentJson = JsonSerializer.Serialize(req.PeoplePresent ?? []),
            TagsJson = JsonSerializer.Serialize(req.Tags ?? []),
            BlobOriginalKey = originalKey,
            BlobThumbnailKey = thumbKey,
            CreatedAt = DateTime.UtcNow,
            CreatorId = currentUser.UserId
        };

        await photoWriteRepo.AddAsync(photo, ct);
        await photoWriteRepo.SaveChangesAsync(ct);

        var photoKey = $"photo-{photo.Id.ToString()}";
        await redisCacheService.SetDataAsync<Photo>(photoKey, photo, CacheTime.TwentyFourHoursInSeconds, ct);


        return photo.Id;
    }
}
