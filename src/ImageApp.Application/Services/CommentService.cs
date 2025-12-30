using ImageApp.Application.DTOs.Comments;
using ImageApp.Application.Interfaces;
using ImageApp.Application.Interfaces.Repositories;
using ImageApp.Application.Interfaces.Services;
using ImageApp.Domain.Entities;

namespace ImageApp.Application.Services;

public class CommentService(
    ICommentRepository comments,
    IPhotoRepository photos,
    IUserRepository users,
    ICurrentUser currentUser,
    IPhotoService photoService)
    : ICommentService
{
    public async Task<IReadOnlyList<CommentResponse>> GetByPhotoIdAsync(Guid photoId, CancellationToken ct = default)
    {
        // Optional: validate the photo exists (nice API behavior)
        if (!await photos.ExistsAsync(photoId, ct))
            throw new InvalidOperationException("Photo not found.");

        var list = await comments.ListByPhotoIdAsync(photoId, ct);

        // NOTE: We want the display name; easiest is to return comments with User included (via Infra repo).
        // If your repo does NOT include User, then fetch users in batch here.
        return list.Select(c => new CommentResponse(
            c.Id,
            c.PhotoId,
            c.UserId,
            c.User.DisplayName,
            c.Text,
            c.CreatedAt
        )).ToList();
    }

    public async Task<CommentResponse> AddAsync(Guid photoId, CreateCommentRequest request, CancellationToken ct = default)
    {
        if (!currentUser.IsAuthenticated)
            throw new InvalidOperationException("Not authenticated.");

        if (string.IsNullOrWhiteSpace(request.Text))
            throw new InvalidOperationException("Comment text is required.");

        var text = request.Text.Trim();
        if (text.Length > 1000)
            throw new InvalidOperationException("Comment is too long (max 1000 characters).");

        if (!await photos.ExistsAsync(photoId, ct))
            throw new InvalidOperationException("Photo not found.");

        var user = await users.GetByIdAsync(currentUser.UserId, ct)
                   ?? throw new InvalidOperationException("User not found.");

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            PhotoId = photoId,
            UserId = user.Id,
            Text = text,
            CreatedAt = DateTime.UtcNow
        };

        await comments.AddAsync(comment, ct);
        await comments.SaveChangesAsync(ct);

        await photoService.UpdateCacheCacheAsync(photoId, ct);

        return new CommentResponse(
            comment.Id,
            comment.PhotoId,
            comment.UserId,
            user.DisplayName,
            comment.Text,
            comment.CreatedAt
        );
    }
}
