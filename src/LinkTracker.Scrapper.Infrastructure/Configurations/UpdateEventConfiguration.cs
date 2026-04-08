using LinkTracker.Scrapper.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkTracker.Scrapper.Infrastructure.Configurations;

public class UpdateEventConfiguration : IEntityTypeConfiguration<UpdateEvent>
{
    public void Configure(EntityTypeBuilder<UpdateEvent> builder)
    {
        builder.ToTable("scrapper_update_event");
        
        builder.HasKey(x => x.Id);
        
         builder.Property(x => x.LinkId)
             .HasColumnName("link_id")
             .IsRequired();
        
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .HasColumnName("id");

        builder.Property(x => x.EventType)
            .HasColumnName("event_type")
            .IsRequired();
        
        builder.Property(x => x.Source)
            .HasColumnName("source")
            .IsRequired();
        
        builder.Property(x => x.Title)
            .HasColumnName("title")
            .IsRequired();
        
        builder.Property(x => x.Author)
            .HasColumnName("author")
            .IsRequired();
        
        builder.Property(x => x.Preview)
            .HasColumnName("preview")
            .IsRequired();
        
        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.DetectedAt)
            .HasColumnName("detected_at")
            .IsRequired();
        
        builder.HasOne(x => x.Link)
            .WithMany()
            .HasForeignKey(x => x.LinkId);
    }
}