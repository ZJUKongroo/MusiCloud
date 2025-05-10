using MusiCloud.Models;

namespace MusiCloud.Interface;

public interface IArtistService
{
    Task<Artist?> GetArtistAsync(Guid artistId);
}
