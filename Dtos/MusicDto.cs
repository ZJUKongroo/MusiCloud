namespace MusiCloud.Dtos;

public class MusicDto
{
    public Guid Id { get; set; }
    public AlbumDto Album { get; set; } = null!;
    public string? Title { get; set; }
    public MetadataDto? Metadata { get; set; }
    public List<ArtistDto> Artists { get; set; } = [];
}
