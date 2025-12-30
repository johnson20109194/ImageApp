namespace ImageApp.Application.Interfaces;

public interface IMediaStorage
{
    Task EnsureContainersExistAsync(CancellationToken ct = default);

    Task<string> GetReadUrlAsync(string blobKey, TimeSpan ttl, CancellationToken ct = default);

    Task<string> UploadOriginalAsync(Stream content, string contentType, string fileExt, CancellationToken ct = default);

    Task<string> UploadThumbnailAsync(Stream content, string contentType, string fileExt, CancellationToken ct = default);

    Task<string> UploadImageAsync(Guid photoId, string base64Image);
}