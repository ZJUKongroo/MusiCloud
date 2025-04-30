using MusiCloud.Models;

public class MusicArtist : ModelBase
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid MusicId { get; set; }
    public Music? Music { get; set; }
    public Guid ArtistId { get; set; }
    public Artist? Artist { get; set; }
}