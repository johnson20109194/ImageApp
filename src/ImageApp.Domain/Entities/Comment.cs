namespace ImageApp.Domain.Entities
{
    public class Comment
    {
        public Guid Id { get; set; }
        public Guid PhotoId { get; set; }
        public Photo Photo { get; set; } = default!;
        public Guid UserId { get; set; }
        public User User { get; set; } = default!;
        public string Text { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}