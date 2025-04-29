using MusiCloud.Models;

namespace MusiCloud.Services;

public interface IAlbumService
{
    Task<IEnumerable<Album>> GetAlbumsAsync();
    Task<Album> GetAlbumAsync(Guid albumId);
    Task<bool> SaveAsync();
}
