namespace MusiCloud.Dtos;

public class AlbumWithMusicsArtistsDto : AlbumDto
{
    public List<ArtistDto> Artists { get; set; } = [];
    public List<MusicDto> Musics { get; set; } = [];

}
