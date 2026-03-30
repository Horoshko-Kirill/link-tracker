using LinkTracker.Bot.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkTracker.Bot.Infrastructure.Configurations;

public class ActionItemConfiguration : IEntityTypeConfiguration<ActionItem>
{
    public void Configure(EntityTypeBuilder<ActionItem> builder)
    {
        builder.ToTable("bot_action_items");

        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .HasColumnName("id");
        
        builder.Property(x => x.ProcessId)
            .HasColumnName("process_id")
            .IsRequired();
        
        builder.Property(x => x.ActionType)
            .HasColumnName("action_type")
            .IsRequired();
        
        builder.Property(x => x.PayloadJson)
            .HasColumnName("payload_json")
            .IsRequired();
        
        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        
        builder.HasOne<Process>()
            .WithMany()
            .HasForeignKey(x => x.ProcessId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}