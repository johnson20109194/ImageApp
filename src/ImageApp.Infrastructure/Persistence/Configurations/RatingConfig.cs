using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ImageApp.Domain.Entities;

namespace ImageApp.Infrastructure.Persistence.Configurations;

public class RatingConfig : IEntityTypeConfiguration<Rating>
{
    public void Configure(EntityTypeBuilder<Rating> builder)
    {
        builder.ToTable("Ratings");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .ValueGeneratedNever();

        builder.Property(r => r.PhotoId)
            .IsRequired();

        builder.Property(r => r.UserId)
            .IsRequired();

        builder.Property(r => r.Score)
            .IsRequired();

        builder.Property(r => r.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        // Relationships
        builder.HasOne(r => r.Photo)
            .WithMany(p => p.Ratings)
            .HasForeignKey(r => r.PhotoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Enforce one rating per user per photo
        builder.HasIndex(r => new { r.PhotoId, r.UserId })
            .IsUnique();

        // Useful for aggregates
        builder.HasIndex(r => r.PhotoId);
        builder.HasIndex(r => r.UserId);

        // Optional: add a check constraint for Score range in SQL Server
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_Ratings_Score_Range", "`Score` >= 1 AND `Score` <= 5");
        });
    }
}