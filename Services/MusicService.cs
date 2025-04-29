using Microsoft.EntityFrameworkCore;
using MusiCloud.Data;
using MusiCloud.Models;

namespace MusiCloud.Services;

public class MusicService(MusiCloudDbContext context) : IMusicService
{
    private readonly MusiCloudDbContext _context = context;

    public async Task<IEnumerable<Music>> GetMusicsAsync()
    {
        return await _context.Musics!
            .ToListAsync();
    }

    public async Task<Music> GetMusicAsync(Guid musicId)
    {
        return await _context.Musics!
            .Where(x => x.Id == musicId)
            .FirstOrDefaultAsync() ?? throw new NullReferenceException(nameof(musicId));
    }
}
