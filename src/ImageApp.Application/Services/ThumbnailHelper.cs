using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;

namespace ImageApp.Application.Services;

internal static class ThumbnailHelper
{
    public static async Task<Stream> CreateThumbnailAsync(Stream input, string contentType, int maxWidth, CancellationToken ct)
    {
        // ImageSharp supports jpeg/png/webp depending on installed packages.
        input.Position = 0;

        using var image = await Image.LoadAsync(input, ct);
        var ratio = (double)maxWidth / image.Width;
        var newW = maxWidth;
        var newH = (int)Math.Round(image.Height * ratio);

        image.Mutate(x => x.Resize(newW, newH));

        var output = new MemoryStream();

        IImageEncoder encoder = contentType switch
        {
            "image/png" => new SixLabors.ImageSharp.Formats.Png.PngEncoder(),
            "image/webp" => new SixLabors.ImageSharp.Formats.Webp.WebpEncoder(),
            _ => new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder { Quality = 80 }
        };

        await image.SaveAsync(output, encoder, ct);
        output.Position = 0;
        return output;
    }
}