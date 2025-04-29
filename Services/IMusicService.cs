using MusiCloud.Models;

namespace MusiCloud.Services;

public interface IMusicService
{
    Task<IEnumerable<Music>> GetMusicsAsync();
    Task<Music> GetMusicAsync(Guid musicId);
    Task<bool> SaveAsync();
}
