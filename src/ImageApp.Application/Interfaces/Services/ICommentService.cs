using ImageApp.Application.DTOs.Comments;

namespace ImageApp.Application.Interfaces.Services;

public interface ICommentService
{
    Task<IReadOnlyList<CommentResponse>> GetByPhotoIdAsync(Guid photoId, CancellationToken ct = default);
    Task<CommentResponse> AddAsync(Guid photoId, CreateCommentRequest request, CancellationToken ct = default);
}