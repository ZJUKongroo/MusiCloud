namespace MusiCloud.Dtos;

public class MusicDto
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public MetadataDto? Metadata { get; set; }
    public uint Track { get; set; }
}
