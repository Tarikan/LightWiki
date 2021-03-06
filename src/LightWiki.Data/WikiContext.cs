using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LightWiki.Data.Configurations;
using LightWiki.Domain.Interfaces;
using LightWiki.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace LightWiki.Data;

public class WikiContext : DbContext
{
    public WikiContext(DbContextOptions opts) : base(opts)
    {
    }

    public DbSet<Article> Articles { get; set; }

    public DbSet<ArticleVersion> ArticleVersions { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<Group> Groups { get; set; }

    public DbSet<GroupPersonalAccessRule> GroupPersonalAccessRules { get; set; }

    public DbSet<Workspace> Workspaces { get; set; }

    public DbSet<Image> Images { get; set; }

    public DbSet<Party> Parties { get; set; }

    public DbSet<WorkspaceAccess> WorkspaceAccesses { get; set; }

    public DbSet<ArticleAccess> ArticleAccesses { get; set; }

    public DbSet<UserInfo> UserInfos { get; set; }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        var addedEntities = ChangeTracker.Entries()
            .Where(entry => entry.State == EntityState.Added)
            .ToList();

        addedEntities.ForEach(entry =>
        {
            if (entry.Entity is ITrackable)
            {
                entry.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
                entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
            }
        });

        var editedEntities = ChangeTracker.Entries()
            .Where(entry => entry.State == EntityState.Modified)
            .ToList();

        editedEntities.ForEach(entry =>
        {
            if (entry.Entity is ITrackable)
            {
                entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
            }
        });

        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSnakeCaseNamingConvention();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ArticleConfiguration());
        modelBuilder.ApplyConfiguration(new ArticleGroupAccessRuleConfiguration());
        modelBuilder.ApplyConfiguration(new ArticlePersonalAccessRuleConfiguration());
        modelBuilder.ApplyConfiguration(new GroupConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new GroupPersonalAccessRuleConfiguration());
        modelBuilder.ApplyConfiguration(new WorkspaceConfiguration());
        modelBuilder.ApplyConfiguration(new WorkspaceGroupAccessRuleConfiguration());
        modelBuilder.ApplyConfiguration(new WorkspacePersonalAccessRuleConfiguration());
        modelBuilder.ApplyConfiguration(new ImageConfiguration());
        modelBuilder.ApplyConfiguration(new WorkspaceAccessConfiguration());
        modelBuilder.ApplyConfiguration(new ArticleAccessConfiguration());
        modelBuilder.ApplyConfiguration(new PartyConfiguration());
    }
}