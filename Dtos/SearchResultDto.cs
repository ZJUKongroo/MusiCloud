namespace MusiCloud.Dtos
{
    public class SearchResultDto
    {
        public List<MusicWithAlbumArtistDto> Musics { get; set; } = [];
        public List<AlbumWithMusicsArtistsDto> Albums { get; set; } = [];
        public List<ArtistWithAlbumDto> Artists { get; set; } = [];
    }
}