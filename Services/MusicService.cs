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
            .Where(m => !m.IsDeleted)
            .Include(m => m.Metadata)
            .Include(m => m.Album)
                .ThenInclude(a => a.AlbumArtists)
                    .ThenInclude(aa => aa.Artist)
            .Include(m => m.MusicArtists)
                .ThenInclude(ma => ma.Artist)
            .AsSplitQuery()  // 优化性能，分割复杂查询
            .OrderBy(m => m.Title)
            .ToListAsync();
    }

    public async Task<Music?> GetMusicAsync(Guid musicId)
    {
        if (musicId == Guid.Empty)
            throw new ArgumentNullException(nameof(musicId));

        return await _context.Musics!
            .Where(m => m.Id == musicId && !m.IsDeleted)
            .Include(m => m.Metadata)
            .Include(m => m.Album)
                .ThenInclude(a => a.AlbumArtists)
                    .ThenInclude(aa => aa.Artist)
            .Include(m => m.MusicArtists)
                .ThenInclude(ma => ma.Artist)
            .AsSplitQuery()
            .FirstOrDefaultAsync();
    }

    public async Task<bool> SaveAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
