using LinkTracker.Scrapper.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkTracker.Scrapper.Infrastructure.Configurations;

public class LinkConfiguration : IEntityTypeConfiguration<Link>
{
    public void Configure(EntityTypeBuilder<Link> builder)
    {
        builder.ToTable("scrapper_links");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .HasColumnName("id");

        builder.Property(x => x.Url)
            .HasColumnName("url")
            .IsRequired();

        builder.Property(x => x.LastChecked)
            .HasColumnName("last_checked")
            .IsRequired();

    }
}
