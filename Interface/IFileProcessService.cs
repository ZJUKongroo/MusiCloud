namespace MusiCloud.Services {
    public interface IFileProcessService{
        
        // Task<bool> SaveAsync();
        Task HandleFileDeletedAsync(string filePath);
        Task HandleFileCreatedAsync(string filePath);
        Task HandleFileRenamedAsync(string filePath, string oldPath);
        Task InitializeAsync();
        Task CleanupAsync();
    }
}