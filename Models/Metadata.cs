namespace MusiCloud.Models;

public class Metadata : ModelBase
{
    public Guid Id { get; set; }
    public byte[]? Thumbnail { get; set; }
    public TimeOnly Duration { get; set; }
    public DateOnly ReleaseDate { get; set; }
}
