using Microsoft.EntityFrameworkCore;
using MusiCloud.Data;
using MusiCloud.Models;

namespace MusiCloud.Services;

public class AlbumService(MusiCloudDbContext context) : IAlbumService
{
    private readonly MusiCloudDbContext _context = context;

    public async Task<IEnumerable<Album>> GetAlbumsAsync()
    {
        return await _context.Albums!
            .OrderBy(x => x.UpdateTime)
            .ToListAsync();
    }

    public async Task<Album> GetAlbumAsync(Guid albumId)
    {
        if (albumId == Guid.Empty)
            throw new ArgumentNullException(nameof(albumId));

        return await _context.Albums!
            .Where(x => x.Id == albumId)
            .OrderBy(x => x.UpdateTime)
            .FirstOrDefaultAsync() ?? throw new NullReferenceException(nameof(albumId));
    }

    public async Task<bool> SaveAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
