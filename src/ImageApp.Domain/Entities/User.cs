using ImageApp.Domain.Enums;

namespace ImageApp.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public UserRole Role { get; set; } = UserRole.Consumer;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}