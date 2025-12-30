namespace ImageApp.Application.DTOs.Comments;

public record CommentResponse(
    Guid id,
    Guid photoId,
    Guid userId,
    string userDisplayName,
    string text,
    DateTime createdAt)
{
    public Guid Id { get; } = id;
    public Guid PhotoId { get; } = photoId;
    public Guid UserId { get; } = userId;
    public string UserDisplayName { get; } = userDisplayName;
    public string Text { get; } = text;
    public DateTime CreatedAt { get; } = createdAt;
    public Guid photoId { get; init; } = photoId;
}