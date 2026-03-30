using LinkTracker.Scrapper.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkTracker.Scrapper.Infrastructure.Configurations;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("scrapper_subscriptions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .HasColumnName("id");

        builder.Property(x => x.ChatId)
            .HasColumnName("chat_id")
            .IsRequired();

        builder.Property(x => x.LinkId)
            .HasColumnName("link_id")
            .IsRequired();

        builder.HasIndex(x => x.ChatId);
        builder.HasIndex(x => x.LinkId);
        
        builder.HasIndex(x => new { x.ChatId, x.LinkId })
            .IsUnique();

        builder.HasOne(x => x.Chat)
            .WithMany()
            .HasForeignKey(x => x.ChatId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Link)
            .WithMany(x => x.Subscriptions)
            .HasForeignKey(x => x.LinkId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
