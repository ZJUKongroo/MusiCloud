using Microsoft.EntityFrameworkCore;
using MusiCloud.Data;
using MusiCloud.Interface;
using MusiCloud.Models;

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
            .Where(a => a.Id == artistId)
            .Include(a => a.AlbumArtists)
                .ThenInclude(aa => aa.Album)
            .AsSplitQuery()
            .FirstOrDefaultAsync();
    }
}
