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
            .OrderBy(x => x.UpdateTime)
            .ToListAsync();
    }

    public async Task<Music> GetMusicAsync(Guid musicId)
    {
        if (musicId == Guid.Empty)
            throw new ArgumentNullException(nameof(musicId));

        return await _context.Musics!
            .Where(x => x.Id == musicId)
            .OrderBy(x => x.UpdateTime)
            .FirstOrDefaultAsync() ?? throw new NullReferenceException(nameof(musicId));
    }

    public async Task<bool> SaveAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
