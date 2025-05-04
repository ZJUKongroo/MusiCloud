using Microsoft.EntityFrameworkCore;
using MusiCloud.Data;
using MusiCloud.Models;
using MusiCloud.Interface;

namespace MusiCloud.Services;

public class AlbumService(MusiCloudDbContext context) : IAlbumService
{
    private readonly MusiCloudDbContext _context = context;

    // 获取专辑详情
    public async Task<Album?> GetAlbumAsync(Guid albumId)
    {
        if (albumId == Guid.Empty)
            throw new ArgumentNullException(nameof(albumId));

        return await _context.Albums!
            .Where(a => a.Id == albumId && !a.IsDeleted)
            .Include(a => a.AlbumArtists.Where(aa => !aa.IsDeleted))
                .ThenInclude(aa => aa.Artist)
            .Include(a => a.Musics.Where(m => !m.IsDeleted))
                .ThenInclude(m => m.Metadata)
            .Include(a => a.Musics.Where(m => !m.IsDeleted))
                .ThenInclude(m => m.MusicArtists.Where(ma => !ma.IsDeleted))
                .ThenInclude(ma => ma.Artist)
            .AsSplitQuery()
            .FirstOrDefaultAsync();
    }

    // 获取推荐专辑
    public async Task<IEnumerable<Album>> GetRecommendedAlbumsAsync(int count = 10)
    {
        return await _context.Albums!
            .Where(a => !a.IsDeleted)
            .OrderBy(_ => EF.Functions.Random()) // 随机排序
            .Take(count)
            .ToListAsync();
    }
}
