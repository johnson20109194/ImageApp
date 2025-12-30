using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ImageApp.Domain.Entities;

namespace ImageApp.Infrastructure.Persistence.Configurations;

public class PhotoConfig : IEntityTypeConfiguration<Photo>
{
    public void Configure(EntityTypeBuilder<Photo> builder)
    {
        builder.ToTable("Photos");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.Property(p => p.CreatorId)
            .IsRequired();

        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Caption)
            .HasMaxLength(2000);

        builder.Property(p => p.Location)
            .HasMaxLength(200);

        builder.Property(p => p.PeoplePresentJson)
            .IsRequired()
            .HasColumnType("longtext");

        builder.Property(p => p.TagsJson)
            .IsRequired()
            .HasColumnType("longtext");

        builder.Property(p => p.BlobOriginalKey)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(p => p.BlobThumbnailKey)
            .HasMaxLength(512);

        builder.Property(p => p.IsPublic)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(p => p.AverageRating)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(p => p.TotalRatings)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(p => p.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        // Relationships
        builder.HasOne(p => p.Creator)
            .WithMany()
            .HasForeignKey(p => p.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Comments)
            .WithOne(c => c.Photo)
            .HasForeignKey(c => c.PhotoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Ratings)
            .WithOne(r => r.Photo)
            .HasForeignKey(r => r.PhotoId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(p => p.CreatorId);
        builder.HasIndex(p => p.CreatedAt);
        builder.HasIndex(p => p.IsPublic);

        // Useful composite indexes for feed/search patterns
        builder.HasIndex(p => new { p.IsPublic, p.CreatedAt });
        builder.HasIndex(p => new { p.IsPublic, p.Location });

        // Note: "Contains" searches on Title/Caption won't use normal indexes well.
        // If you move to Azure AI Search later, keep DB indexes simple.
        builder.HasIndex(p => p.Title);
    }
}