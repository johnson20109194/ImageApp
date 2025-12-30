using Microsoft.Extensions.Logging;

namespace ImageApp.Infrastructure.Storage;

public class ProcessPhotoFunction
{
    // [FunctionName("ProcessPhoto")]
    // public async Task Run(
    //     [BlobTrigger("photos-original/{name}", Connection = "StorageConnection")] Stream image,
    //     string name,
    //     ILogger log)
    // {
    //     // 1. Generate thumbnail
    //     var thumbStream = await _imageResizer.ResizeAsync(image, 400, 400);
    //     await _thumbnailContainer.UploadBlobAsync(name, thumbStream);
    //
    //     // 2. Cognitive tags
    //     var tags = await _visionClient.GetTagsAsync(image);
    //
    //     // 3. Update DB photo record with tags & thumbnail url
    //     var photoId = Guid.Parse(Path.GetFileNameWithoutExtension(name));
    //     await _photoRepository.UpdateAsync(photoId, tags, thumbnailUrl);
    // }
}
