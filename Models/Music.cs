namespace MusiCloud.Models;

public class Music : ModelBase
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AlbumId { get; set; }
    public Album Album { get; set; } = null!;
    public string? Title { get; set; }
    public Metadata? Metadata { get; set; }
    public ICollection<MusicArtist> MusicArtists { get; } = [];
}
