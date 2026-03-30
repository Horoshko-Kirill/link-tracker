using LinkTracker.Bot.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace LinkTracker.Bot.Infrastructure.Database;

public class BotDbContext : DbContext
{
    public DbSet<Process> Processes { get; set; }
    public DbSet<ActionItem> ActionItems { get; set; }
    
    public BotDbContext(DbContextOptions<BotDbContext> options) : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BotDbContext).Assembly);
    }
}