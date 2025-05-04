using MusiCloud.Interface;

namespace MusiCloud.Services
{
    public class FileWatchService(IServiceProvider serviceProvider,
                               ILogger<FileWatchService> logger,
                               IConfiguration configuration) : IFileWatchService
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly ILogger<FileWatchService> _logger = logger;
        private FileSystemWatcher? _watcher;
        private readonly string _musicFolder = configuration["MusicFolder"] ?? Path.Combine(AppContext.BaseDirectory, "MusicFiles");

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Directory.CreateDirectory(_musicFolder);

            await ScanMusicFilesAsync();

            _watcher = new FileSystemWatcher(_musicFolder)
            {
                IncludeSubdirectories = true,
                EnableRaisingEvents = true,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName |
                              NotifyFilters.LastWrite | NotifyFilters.CreationTime
            };

            _watcher.Created += OnFileChanged;
            _watcher.Deleted += OnFileChanged;
            _watcher.Renamed += OnFileRenamed;
            _watcher.Changed += OnFileChanged;

            _logger.LogInformation("已开始监控音乐文件夹: {Folder}", _musicFolder);
        }

        private async void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            await ProcessFileChangeAsync(e.FullPath);
        }

        private async void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            await ProcessFileRenameAsync(e.OldFullPath, e.FullPath);
        }

        private async Task ProcessFileChangeAsync(string filePath)
        {
            if (!IsMusicFile(filePath)) return;

            var fileInfo = new FileInfo(filePath);
            using var scope = _serviceProvider.CreateScope();
            var _IFileProcessService = scope.ServiceProvider.GetRequiredService<IFileProcessService>();

            if (!fileInfo.Exists)
            {
                // 文件被删除
                await _IFileProcessService.HandleFileDeletedAsync(filePath);
                return;
            }

            // Wait for file to be fully copied
            if (!await WaitForFileCopyToComplete(filePath)) return;

            await _IFileProcessService.HandleFileCreatedAsync(filePath);
            _logger.LogInformation("已更新数据库中的文件: {FilePath}", filePath);
        }

        private async Task<bool> WaitForFileCopyToComplete(string filePath)
        {
            const int maxAttempts = 10;
            const int delayMs = 500;
            long lastSize = 0;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                try
                {
                    var fileInfo = new FileInfo(filePath);
                    if (!fileInfo.Exists) return false;

                    // Try to open the file exclusively
                    using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        // If we can open it exclusively and the size hasn't changed, it's likely not being copied
                        if (fileInfo.Length == lastSize || attempt > 0)
                        {
                            return true;
                        }
                    }
                }
                catch (IOException)
                {
                    // File is locked - likely still being copied
                    _logger.LogDebug("文件正在复制中: {FilePath}", filePath);
                }

                try
                {
                    lastSize = new FileInfo(filePath).Length;
                }
                catch (Exception)
                {
                    // Ignore errors getting file size
                }

                await Task.Delay(delayMs);
            }

            _logger.LogWarning("文件可能正在被复制或被锁定: {FilePath}", filePath);
            return false;
        }

        private async Task ProcessFileRenameAsync(string oldPath, string newPath)
        {
            using var scope = _serviceProvider.CreateScope();
            var _IFileProcessService = scope.ServiceProvider.GetRequiredService<IFileProcessService>();

            if (!IsMusicFile(newPath)) await _IFileProcessService.HandleFileDeletedAsync(oldPath);

            await _IFileProcessService!.HandleFileRenamedAsync(oldPath, newPath);
        }
        private async Task ScanMusicFilesAsync()
        {
            _logger.LogInformation("开始扫描音乐文件...");

            var files = Directory.GetFiles(_musicFolder, "*.*", SearchOption.AllDirectories)
                .Where(IsMusicFile);

            int count = 0;
            using var scope = _serviceProvider.CreateScope();
            var _IFileProcessService = scope.ServiceProvider.GetRequiredService<IFileProcessService>();

            await _IFileProcessService.InitializeAsync();

            foreach (var filePath in files)
            {
                await _IFileProcessService.HandleFileCreatedAsync(filePath);
                count++;
            }

            await _IFileProcessService.CleanupAsync();

            _logger.LogInformation("扫描完成，处理了 {count} 个文件", count);
        }

        private bool IsMusicFile(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return extension is ".mp3" or ".flac" or ".wav" or ".ogg" or ".aac" or ".m4a";
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _watcher?.Dispose();
            _logger.LogInformation("已停止音乐文件监控");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _watcher?.Dispose();
        }
    }
}