namespace MusiCloud.Interface;

public interface IFileWatchService : IHostedService, IDisposable{
    Task ScanMusicFilesAsync();
}
