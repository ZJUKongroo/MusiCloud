using MusiCloud.Models;

namespace MusiCloud.Services;

public interface IMusicService
{
    Task<Music?> GetMusicAsync(Guid musicId);
    Task<Album?> GetAlbumAsync(Guid albumId);
    Task<Artist?> GetArtistAsync(Guid artistId);
    Task<IEnumerable<Music>> SearchMusicAsync(string queryString);
    Task<IEnumerable<Album>> SearchAlbumAsync(string queryString);
    Task<IEnumerable<Artist>> SearchArtistAsync(string queryString);
    Task<IEnumerable<Music>> GetRecommendedMusicAsync(int count = 10);
    Task<IEnumerable<Album>> GetRecommendedAlbumsAsync(int count = 10);
    Task<IEnumerable<Music>> GetLatestMusicAsync(int count = 10);
}
