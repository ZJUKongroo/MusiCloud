using MusiCloud.Models;

public class AlbumArtist : ModelBase
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AlbumId { get; set; }
    public Album? Album { get; set; }
    public Guid ArtistId { get; set; }
    public Artist? Artist { get; set; }
}