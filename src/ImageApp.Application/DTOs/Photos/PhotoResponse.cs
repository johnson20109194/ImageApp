using ImageApp.Application.DTOs.Comments;
using ImageApp.Domain.Entities;

namespace ImageApp.Application.DTOs.Photos;

public record PhotoResponse(
    Guid id,
    string title,
    string? caption,
    string? location,
    IReadOnlyList<string> peoplePresent,
    string originalUrl,
    string thumbnailUrl,
    double averageRating,
    int totalRatings,
    DateTime createdAt
    )
{
    public Guid Id { get; } = id;
    public string Title { get; } = title;
    public string? Caption { get; } = caption;
    public string? Location { get; } = location;
    public IReadOnlyList<string> PeoplePresent { get; } = peoplePresent;
    public string OriginalUrl { get; } = originalUrl;
    public double AverageRating { get; } = averageRating;
    public int TotalRatings { get; } = totalRatings;
    public DateTime CreatedAt { get; } = createdAt;
    public string? ThumbnailUrl { get; } = thumbnailUrl;

    public ICollection<CommentResponse> Comments { get; set; }

}