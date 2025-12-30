namespace ImageApp.Domain.Entities
{
    public class Rating
    {
        public Guid Id { get; set; }
        public Guid PhotoId { get; set; }
        public Photo Photo { get; set; } = default!;
        public Guid UserId { get; set; }
        public User User { get; set; } = default!;
        public int Score { get; set; } // 1..5
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}