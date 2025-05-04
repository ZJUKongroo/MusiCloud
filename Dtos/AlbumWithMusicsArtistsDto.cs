namespace MusiCloud.Dtos;

public class AlbumWithMusicsArtistsDto : AlbumDto
{
    public List<ArtistDto> Artists { get; set; } = [];
    public List<MusicWithArtistDto> Musics { get; set; } = [];

}
