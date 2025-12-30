namespace ImageApp.Infrastructure.Storage;

public sealed class StorageOptions
{
    public string ConnectionString { get; set; } = null!;
    public string OriginalContainer { get; set; } = "photos-original";
    public string ThumbnailContainer { get; set; } = "photos-thumbs";

    // CDN host, e.g. https://cdn.imageapp.com
    // If set, blob URLs returned will be rewritten to use this host.
    public string? CdnHost { get; set; }

    public string PublicBaseUrl { get; init; } = default!;

}