namespace ImageApp.Domain.Entities
{
    public class Photo
    {
        public Guid Id { get; init; }
        public Guid CreatorId { get; init; }
        public User Creator { get; init; } = default!;

        public string Title { get; init; } = default!;
        public string? Caption { get; init; }
        public string? Location { get; init; }
        public string? BlobOriginalUrl { get; set; }
        public string PeoplePresentJson { get; init; } = "[]";
        public string TagsJson { get; set; } = "[]";

        public string BlobOriginalKey { get; set; } = default!;
        public string BlobThumbnailKey { get; set; } = default!;
        public double AverageRating { get; set; }
        public int TotalRatings { get; set; }

        public bool IsPublic { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    }
}