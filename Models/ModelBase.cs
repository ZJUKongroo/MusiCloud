namespace MusiCloud.Models;

public class ModelBase
{
    public DateTime CreateTime { get; set; } = DateTime.Now;
    public DateTime UpdateTime { get; set; } = DateTime.Now;
    public DateTime DeleteTime { get; set; }
    public bool IsDeleted { get; set; } = false;
}
