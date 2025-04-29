namespace MusiCloud.Models;

public class MusicBlob : ModelBase
{
    public Guid Id { get; set; }
    public Guid MusicId { get; set; }
    public Music? Music { get; set; } = null!;
    public string? FileName { get; set; }
    public string? FilePath { get; set; }
    public long FileSize { get; set; }
    public string? HashValue { get; set; }
}
