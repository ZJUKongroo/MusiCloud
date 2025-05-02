using MusiCloud.Models;

public class SearchResult
{
    public IEnumerable<Music> Musics { get; set; } = [];
    public IEnumerable<Album> Albums { get; set; } = [];
    public IEnumerable<Artist> Artists { get; set; } = [];
}