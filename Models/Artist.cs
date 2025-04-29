namespace MusiCloud.Models;

public class Artist : ModelBase
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Guid AlbumId { get; set; }
    public Album Album { get; set; } = null!;
}
