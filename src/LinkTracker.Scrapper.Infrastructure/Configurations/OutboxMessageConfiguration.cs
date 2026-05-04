using LinkTracker.Scrapper.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkTracker.Scrapper.Infrastructure.Configurations;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("scrapper_outbox_messages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .HasColumnName("id");
        
        builder.Property(x => x.Paylod)
            .HasColumnName("paylod")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .IsRequired();
       
        builder.Property(x => x.Attempts)
            .HasColumnName("attempts")
            .IsRequired();

        builder.Property(x => x.Error)
            .HasColumnName("error");

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at");
        
        builder.Property(x => x.SentAt)
            .HasColumnName("sent_at");
    }
}