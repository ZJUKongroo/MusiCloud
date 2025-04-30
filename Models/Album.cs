namespace MusiCloud.Models;

public class Album : ModelBase
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string[] Genre { get; set; } = [];
    public string? CoverPath { get; set; }
    public ICollection<Music> Musics { get; } = [];
    public List<AlbumArtist> AlbumArtists { get;  } = [];
}
