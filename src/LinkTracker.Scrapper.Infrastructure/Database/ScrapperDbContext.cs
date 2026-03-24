using LinkTracker.Scrapper.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace LinkTracker.Scrapper.Infrastructure.Database;

public class ScrapperDbContext : DbContext
{
    public DbSet<Chat> Chats { get; set; }
    public DbSet<Link> Links { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Tag> Tags { get; set; }


    public ScrapperDbContext(DbContextOptions<ScrapperDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ScrapperDbContext).Assembly);
    }
}
