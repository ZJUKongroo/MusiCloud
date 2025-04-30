namespace MusiCloud.Services {
    public interface IFileProcessService{
        
        Task<bool> SaveAsync();
        Task handleFileDeleted(string filePath);
        Task handleFileCreated(string filePath);
        Task handleFile(string filePath);
        Task handleFileRenamed(string filePath, string oldPath);
        Task handleFileModified(string filePath);
    }
}