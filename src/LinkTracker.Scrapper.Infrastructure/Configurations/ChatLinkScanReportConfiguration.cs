using LinkTracker.Scrapper.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkTracker.Scrapper.Infrastructure.Configurations;

public class ChatLinkScanReportConfiguration : IEntityTypeConfiguration<ChatLinkScanReport>
{
    public void Configure(EntityTypeBuilder<ChatLinkScanReport> builder)
    {
        builder.ToTable("scrapper_link_scan_report");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .HasColumnName("id");
        
        builder.Property(x => x.ChatId)
            .HasColumnName("chat_id")
            .IsRequired();
        
        builder.Property(x => x.ScanStartedAt)
            .HasColumnName("scan_started_at")
            .IsRequired();
        
        builder.Property(x => x.ScanFinishedAt)
            .HasColumnName("scan_finished_at")
            .IsRequired();
        
        builder.Property(x => x.FailedCount)
            .HasColumnName("failed_count")
            .IsRequired();
        
        builder.Property(x => x.Message)
            .HasColumnName("message")
            .IsRequired();

        builder.Property(x => x.SentAt)
            .HasColumnName("sent_at");
        
        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired();

        builder.HasOne(x => x.Chat)
            .WithMany()
            .HasForeignKey(x => x.ChatId);

    }
}