using Microsoft.AspNetCore.Http;
using ImageApp.Application.DTOs.Photos;

namespace ImageApp.Application.Interfaces.Services;

public interface ICreatorService
{
    Task<Guid> UploadPhotoAsync(CreatePhotoRequest req, IFormFile file, CancellationToken ct);
}