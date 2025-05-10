using MusiCloud.Models;

namespace MusiCloud.Interface;

public interface IAlbumService
{
    Task<Album?> GetAlbumAsync(Guid albumId);
    Task<IEnumerable<Album>> GetRecommendedAlbumsAsync(int count = 10);
}
