namespace MusiCloud.Dtos;

public class MusicWithAlbumArtistDto : MusicDto
{
    public AlbumDto? Album { get; set; } = null;
    public List<ArtistDto> Artists { get; set; } = [];
}
