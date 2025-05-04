using Microsoft.EntityFrameworkCore;
using MusiCloud.Data;
using MusiCloud.Models;
using MusiCloud.Interface;

namespace MusiCloud.Services;

public class MusicService(MusiCloudDbContext context) : IMusicService
{
    private readonly MusiCloudDbContext _context = context;

    // 已实现的方法
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

    // 获取推荐音乐
    public async Task<IEnumerable<Music>> GetRecommendedMusicAsync(int count = 10)
    {
        return await _context.Musics!
            .Where(m => !m.IsDeleted)
            .Include(m => m.Metadata)
            .Include(m => m.Album)
            .Include(m => m.MusicArtists.Where(ma => !ma.IsDeleted))
            .ThenInclude(ma => ma.Artist)
            .AsSplitQuery()
            .OrderBy(_ => EF.Functions.Random()) 
            .Take(count)
            .ToListAsync();
    }

    // 获取最新音乐
    public async Task<IEnumerable<Music>> GetLatestMusicAsync(int count = 10)
    {
        return await _context.Musics!
            .Where(m => !m.IsDeleted)
            .Include(m => m.Metadata)
            .Include(m => m.Album)
            .Include(m => m.MusicArtists.Where(ma => !ma.IsDeleted))
                .ThenInclude(ma => ma.Artist)
            .AsSplitQuery()
            .OrderByDescending(m => m.UpdateTime) // 按更新时间降序排序
            .Take(count)
            .ToListAsync();
    }
}
