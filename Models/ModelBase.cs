namespace MusiCloud.Models;

public class ModelBase
{
    public DateTime CreateTime { get; set; } = DateTime.UtcNow;
    public DateTime UpdateTime { get; set; } = DateTime.UtcNow;
    public DateTime DeleteTime { get; set; }
    public bool IsDeleted { get; set; } = false;
}
