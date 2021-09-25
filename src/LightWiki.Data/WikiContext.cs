using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LightWiki.Domain.Interfaces;
using LightWiki.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace LightWiki.Data
{
    public class WikiContext : DbContext
    {
        public DbSet<Article> Articles { get; set; }

        public DbSet<ArticleVersion> ArticleVersions { get; set; }

        public DbSet<User> Users { get; set; }

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
    }
}