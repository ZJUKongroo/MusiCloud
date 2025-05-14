namespace MusiCloud.Dtos;

public class MusicWithArtistDto : MusicDto
{
    public List<ArtistDto> Artists { get; set; } = [];
}
