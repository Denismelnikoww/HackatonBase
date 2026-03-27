using Domain.Models;
using Infrastructure.DbContexts.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DbContexts;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Session> UserSessions { get; set; }
    public DbSet<Token> UserTokens { get; set; }
    public DbSet<ApiKey> ApiKeys { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("hackaton");

        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new SessionConfiguration());
        modelBuilder.ApplyConfiguration(new TokenConfiguration());
        modelBuilder.ApplyConfiguration(new ApiKeyConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}
