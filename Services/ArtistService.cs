using Microsoft.EntityFrameworkCore;
using MusiCloud.Data;
using MusiCloud.Models;
using MusiCloud.Interface;

namespace MusiCloud.Services;

public class ArtistService(MusiCloudDbContext context) : IArtistService
{
    private readonly MusiCloudDbContext _context = context;

    // 获取艺术家详情
    public async Task<Artist?> GetArtistAsync(Guid artistId)
    {
        if (artistId == Guid.Empty)
            throw new ArgumentNullException(nameof(artistId));

        return await _context.Artists!
            .Where(a => a.Id == artistId && !a.IsDeleted)
            .Include(a => a.AlbumArtists.Where(aa => !aa.IsDeleted))
                .ThenInclude(aa => aa.Album)
            // .Include(a => a.MusicArtists.Where(ma => !ma.IsDeleted))
            //     .ThenInclude(ma => ma.Music)
            //         .ThenInclude(m => m!.Metadata)
            .AsSplitQuery()
            .FirstOrDefaultAsync();
    }
}
