using Microsoft.EntityFrameworkCore;
using MusiCloud.Data;
using MusiCloud.Interface;
using MusiCloud.Models;

namespace MusiCloud.Services;

public class SearchService(MusiCloudDbContext context) : ISearchService
{
    private readonly MusiCloudDbContext _context = context;

    // 搜索音乐
    public async Task<IEnumerable<Music>> SearchMusicAsync(string queryString)
    {
        if (string.IsNullOrWhiteSpace(queryString))
            return Array.Empty<Music>();

        queryString = queryString.Trim().ToLower();

        return await _context.Musics!
            .Where(m => m.Title != null &&
               EF.Functions.Like(m.Title.ToLower(), "%" + queryString + "%"))
            .Include(m => m.Metadata)
            .Include(m => m.Album)
            .Include(m => m.MusicArtists)
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
            .Where(a => a.Title != null &&
               EF.Functions.Like(a.Title.ToLower(), "%" + queryString + "%"))
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
            .Where(a => a.Name != null &&
               EF.Functions.Like(a.Name.ToLower(), "%" + queryString + "%"))
            .Take(10)
            .ToListAsync();
    }
}
