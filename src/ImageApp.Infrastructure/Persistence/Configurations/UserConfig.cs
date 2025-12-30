using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ImageApp.Domain.Entities;
using ImageApp.Domain.Enums;

namespace ImageApp.Infrastructure.Persistence.Configurations;

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .ValueGeneratedNever();

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.DisplayName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(300);

        // Enum stored as int by default
        builder.Property(u => u.Role)
            .IsRequired();

        builder.Property(u => u.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        // ---- Seed Admin + Creator ----
        // Use FIXED GUIDs so the seed is stable across environments.
        // Use PRECOMPUTED bcrypt hashes (see section 2 below).
        var adminId = new Guid("11111111-1111-1111-1111-111111111111");
        var creatorId = new Guid("22222222-2222-2222-2222-222222222222");

        string adminHash = BCrypt.Net.BCrypt.HashPassword("Admin@123");
        string creatorHash = BCrypt.Net.BCrypt.HashPassword("Creator@123");

        var seedCreatedAt = new DateTime(2025, 12, 26, 0, 0, 0, DateTimeKind.Utc);

        builder.HasData(
            new User
            {
                Id = adminId,
                Email = "admin@imageapp.local",
                DisplayName = "System Admin",
                PasswordHash = adminHash,
                Role = UserRole.Admin,
                CreatedAt = seedCreatedAt
            },
            new User
            {
                Id = creatorId,
                Email = "creator@imageapp.local",
                DisplayName = "Default Creator",
                PasswordHash = creatorHash,
                Role = UserRole.Creator,
                CreatedAt = seedCreatedAt
            }
        );
    }
}