namespace MusiCloud.Models;

public class Album : ModelBase
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public ICollection<Music> Musics { get; } = [];
    public ICollection<Artist> Artists { get; } = [];
    public ICollection<Genre>? Genres { get; } = [];
}
