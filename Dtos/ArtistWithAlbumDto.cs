namespace MusiCloud.Dtos
{
    public class ArtistWithAlbumDto : ArtistDto
    {
        public List<AlbumDto> Albums { get; set; } = [];
    }
}
