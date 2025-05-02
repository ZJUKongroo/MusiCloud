using Microsoft.EntityFrameworkCore;
using MusiCloud.Data;
using MusiCloud.Models;

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
            .AsSplitQuery()
            .FirstOrDefaultAsync();
    }

    // 获取艺术家详情
    public async Task<Artist?> GetArtistAsync(Guid artistId)
    {
        if (artistId == Guid.Empty)
            throw new ArgumentNullException(nameof(artistId));

        return await _context.Artists!
            .Where(a => a.Id == artistId && !a.IsDeleted)
            .Include(a => a.AlbumArtists.Where(aa => !aa.IsDeleted))
                .ThenInclude(aa => aa.Album)
            .Include(a => a.MusicArtists.Where(ma => !ma.IsDeleted))
                .ThenInclude(ma => ma.Music)
                    .ThenInclude(m => m!.Metadata)
            .AsSplitQuery()
            .FirstOrDefaultAsync();
    }

    // 搜索音乐
    public async Task<IEnumerable<Music>> SearchMusicAsync(string queryString)
    {
        if (string.IsNullOrWhiteSpace(queryString))
            return Array.Empty<Music>();

        queryString = queryString.Trim().ToLower();

        return await _context.Musics!
            .Where(m => !m.IsDeleted && m.Title != null && 
               EF.Functions.Like(m.Title.ToLower(), "%" + queryString + "%"))
            .Include(m => m.Metadata)
            .Include(m => m.Album)
            .Include(m => m.MusicArtists.Where(ma => !ma.IsDeleted))
                .ThenInclude(ma => ma.Artist)
            .AsSplitQuery()
            .OrderBy(m => m.Title)
            .Take(10) 
            .ToListAsync();
    }

    // 搜索专辑
    public async Task<IEnumerable<Album>> SearchAlbumAsync(string queryString)
    {
        if (string.IsNullOrWhiteSpace(queryString))
            return Array.Empty<Album>();

        queryString = queryString.Trim().ToLower();

        return await _context.Albums!
            .Where(a => !a.IsDeleted && a.Title != null && 
               EF.Functions.Like(a.Title.ToLower(), "%" + queryString + "%"))
            .Include(a => a.AlbumArtists.Where(aa => !aa.IsDeleted))
                .ThenInclude(aa => aa.Artist)
            .Include(a => a.Musics.Where(m => !m.IsDeleted))
                .ThenInclude(m => m.Metadata)
            .AsSplitQuery()
            .OrderBy(a => a.Title)
            .Take(10) 
            .ToListAsync();
    }

    // 搜索艺术家
    public async Task<IEnumerable<Artist>> SearchArtistAsync(string queryString)
    {
        if (string.IsNullOrWhiteSpace(queryString))
            return Array.Empty<Artist>();

        queryString = queryString.Trim().ToLower();

        return await _context.Artists!
            .Where(a => !a.IsDeleted && a.Name != null && 
               EF.Functions.Like(a.Name.ToLower(), "%" + queryString + "%"))
            .Include(a => a.AlbumArtists.Where(aa => !aa.IsDeleted))
            .ThenInclude(aa => aa.Album)
            .AsSplitQuery()
            .Take(10) 
            .ToListAsync();
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

    // 获取推荐专辑
    public async Task<IEnumerable<Album>> GetRecommendedAlbumsAsync(int count = 10)
    {
        return await _context.Albums!
            .Where(a => !a.IsDeleted)
            .Include(a => a.AlbumArtists.Where(aa => !aa.IsDeleted))
                .ThenInclude(aa => aa.Artist)
            .AsSplitQuery()
            .OrderByDescending(a => a.UpdateTime) // 按更新时间降序排序
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
