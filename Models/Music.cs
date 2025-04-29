namespace MusiCloud.Models;

public class Music : ModelBase
{
    public Guid Id { get; set; }
    public Guid AlbumId { get; set; }
    public Album Album { get; set; } = null!;
    public string? Name { get; set; }
    public Metadata? Metadata { get; set; }
    public MusicBlob? MusicBlob { get; set; }
}
