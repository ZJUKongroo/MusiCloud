using Microsoft.EntityFrameworkCore;
using MusiCloud.Models;

namespace MusiCloud.Data;

public class MusiCloudDbContext(DbContextOptions<MusiCloudDbContext> options) : DbContext(options)
{
    public DbSet<Album>? Albums { get; set; }
    public DbSet<Artist>? Artists { get; set; }
    public DbSet<Metadata>? Metadatas { get; set; }
    public DbSet<Music>? Musics { get; set; }
    public DbSet<AlbumArtist>? AlbumArtists { get; set; }
    public DbSet<MusicArtist>? MusicArtists { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Album>()
            .HasQueryFilter(p => !p.IsDeleted);
        modelBuilder.Entity<Artist>()
            .HasQueryFilter(p => !p.IsDeleted);
        modelBuilder.Entity<Metadata>()
            .HasQueryFilter(p => !p.IsDeleted);
        modelBuilder.Entity<Music>()
            .HasQueryFilter(p => !p.IsDeleted);
        modelBuilder.Entity<AlbumArtist>()
            .HasQueryFilter(p => !p.IsDeleted);
        modelBuilder.Entity<MusicArtist>()
            .HasQueryFilter(p => !p.IsDeleted);
    }
}
