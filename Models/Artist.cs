namespace MusiCloud.Models;

public class Artist : ModelBase
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Name { get; set; }
    public string? Description { get; set; }
    public List<AlbumArtist> AlbumArtists { get;  } = [];
    public List<MusicArtist> MusicArtists { get;  } = [];
}
