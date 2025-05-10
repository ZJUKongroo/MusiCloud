namespace MusiCloud.Interface;

public interface IDatabaseService
{
    Task<bool> RebuildDatabaseAsync();
}