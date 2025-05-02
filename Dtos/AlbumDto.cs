namespace MusiCloud.Dtos;

public class AlbumDto
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string[] Genre { get; set; } = [];
    public string? CoverPath { get; set; }
}
