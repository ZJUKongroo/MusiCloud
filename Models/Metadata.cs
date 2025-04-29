namespace MusiCloud.Models;

public class Metadata : ModelBase
{
    public Guid Id { get; set; }
    public Guid MusicId { get; set; }
    public Music? Music { get; set; }
    public TimeOnly Duration { get; set; }
    public DateOnly ReleaseDate { get; set; }
    public string? FileName { get; set; }
    public string? FilePath { get; set; }
    public long FileSize { get; set; }
    public string? HashValue { get; set; }
}
