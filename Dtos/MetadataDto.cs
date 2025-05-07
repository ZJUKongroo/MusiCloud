namespace MusiCloud.Dtos;

public class MetadataDto
{
    public Guid Id { get; set; }
    public TimeOnly Duration { get; set; }
    public string? FileName { get; set; }
    public long FileSize { get; set; }
}
