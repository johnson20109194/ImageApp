using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;
using ImageApp.Application.Interfaces;

namespace ImageApp.Infrastructure.Storage;

public sealed class AzureBlobMediaStorage : IMediaStorage
{
    private readonly BlobServiceClient _svc;
    private readonly StorageOptions _opt;

    public AzureBlobMediaStorage(IOptions<StorageOptions> opt)
    {
        _opt = opt.Value;
        _svc = new BlobServiceClient(_opt.ConnectionString);
    }

    public async Task EnsureContainersExistAsync(CancellationToken ct = default)
    {
        await _svc.GetBlobContainerClient(_opt.OriginalContainer).CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: ct);
        await _svc.GetBlobContainerClient(_opt.ThumbnailContainer).CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: ct);
    }

    public async Task<string> UploadOriginalAsync(Stream content, string contentType, string fileExt, CancellationToken ct = default)
    {
        var name = $"{DateTime.UtcNow:yyyy/MM/dd}/{Guid.NewGuid():N}{fileExt}";
        var container = _svc.GetBlobContainerClient(_opt.OriginalContainer);
        var blob = container.GetBlobClient(name);

        content.Position = 0;
        await blob.UploadAsync(content, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: ct);

        // store as "<container>/<blobName>" to keep it portable
        return $"{_opt.OriginalContainer}/{name}";
    }

    public async Task<string> UploadThumbnailAsync(Stream content, string contentType, string fileExt, CancellationToken ct = default)
    {
        var name = $"{DateTime.UtcNow:yyyy/MM/dd}/{Guid.NewGuid():N}{fileExt}";
        var container = _svc.GetBlobContainerClient(_opt.ThumbnailContainer);
        var blob = container.GetBlobClient(name);

        content.Position = 0;
        await blob.UploadAsync(content, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: ct);

        return $"{_opt.ThumbnailContainer}/{name}";
    }

    public Task<string> GetReadUrlAsync(string blobKey, TimeSpan ttl, CancellationToken ct = default)
    {
        var (containerName, blobName) = Split(blobKey);

        var container = _svc.GetBlobContainerClient(containerName);
        var blob = container.GetBlobClient(blobName);

        // 1) If CDN host is set, return a CDN URL (no SAS) for public CDN approach
        //    This assumes your CDN origin can access blob (public container or AFD with private origin + managed identity).
        if (!string.IsNullOrWhiteSpace(_opt.CdnHost))
        {
            // Common pattern: CDN host + "/<container>/<blobName>"
            var cdn = _opt.CdnHost!.TrimEnd('/');
            return Task.FromResult($"{cdn}/{containerName}/{blobName}");
        }

        // 2) Otherwise return SAS URL (private container)
        if (!blob.CanGenerateSasUri)
            return Task.FromResult(blob.Uri.ToString());

        var sas = new BlobSasBuilder
        {
            BlobContainerName = containerName,
            BlobName = blobName,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.Add(ttl)
        };
        sas.SetPermissions(BlobSasPermissions.Read);

        var uri = blob.GenerateSasUri(sas);
        return Task.FromResult(uri.ToString());
    }

    public async Task<string> UploadImageAsync(Guid photoId, string base64Image)
    {
        var data = Convert.FromBase64String(
            base64Image[(base64Image.IndexOf(",", StringComparison.Ordinal) + 1)..]); // strip prefix if data URI

        var container = _svc.GetBlobContainerClient(_opt.OriginalContainer);

        var blobClient = container.GetBlobClient($"{photoId}.jpg");
        using var stream = new MemoryStream(data);

        await blobClient.UploadAsync(stream, overwrite: true);

        return blobClient.Uri.ToString(); // or return blob name and generate SAS later
    }

    private static (string Container, string Blob) Split(string blobKey)
    {
        var idx = blobKey.IndexOf('/');
        if (idx <= 0 || idx == blobKey.Length - 1)
            throw new InvalidOperationException("Invalid blob key format. Expected '<container>/<blobName>'.");

        return (blobKey[..idx], blobKey[(idx + 1)..]);
    }
}
