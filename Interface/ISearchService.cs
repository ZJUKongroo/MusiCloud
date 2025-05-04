using MusiCloud.Models;

namespace MusiCloud.Interface;

public interface ISearchService
{
    Task<IEnumerable<Music>> SearchMusicAsync(string queryString);
    Task<IEnumerable<Album>> SearchAlbumAsync(string queryString);
    Task<IEnumerable<Artist>> SearchArtistAsync(string queryString);
}
