using FileChunker.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileChunker.DataAccess.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<FileMetadata> Files { get; set; }
        public DbSet<ChunkMetadata> Chunks { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileMetadata>()
                .HasMany(f => f.Chunks)
                .WithOne(c => c.File)
                .HasForeignKey(c => c.FileId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
