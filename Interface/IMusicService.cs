using MusiCloud.Models;

namespace MusiCloud.Interface;

public interface IMusicService
{
    Task<Music?> GetMusicAsync(Guid musicId);
    Task<IEnumerable<Music>> GetRecommendedMusicAsync(int count = 10);
    Task<IEnumerable<Music>> GetLatestMusicAsync(int count = 10);
}
