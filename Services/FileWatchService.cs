namespace MusiCloud.Services
{
    public class FileWatchService(IServiceProvider serviceProvider,
                               ILogger<FileWatchService> logger,
                               IConfiguration configuration,FileProcessService fileProcessService) : IFileWatchService
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly ILogger<FileWatchService> _logger = logger;
        private FileSystemWatcher? _watcher;
        private readonly string _musicFolder = configuration["MusicFolder"] ?? Path.Combine(AppContext.BaseDirectory, "MusicFiles");
    
        private readonly FileProcessService _fileProcessService = fileProcessService;
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // 确保目录存在
            Directory.CreateDirectory(_musicFolder);

            // 初始扫描文件
            await ScanMusicFilesAsync();

            // 设置文件监控
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
            if (!fileInfo.Exists)
            {
                // 文件被删除
                await _fileProcessService.handleFileDeleted(filePath);
                return;
            }

            await _fileProcessService.handleFileCreated(filePath);
            _logger.LogInformation("已更新数据库中的文件: {FilePath}", filePath);
        }

        private async Task ProcessFileRenameAsync(string oldPath, string newPath)
        {
            if (!IsMusicFile(newPath)) await _fileProcessService.handleFileDeleted(oldPath);

            await _fileProcessService!.handleFileRenamed(oldPath, newPath);
        }
        private async Task ScanMusicFilesAsync()
        {
            _logger.LogInformation("开始扫描音乐文件...");

            var files = Directory.GetFiles(_musicFolder, "*.*", SearchOption.AllDirectories)
                .Where(IsMusicFile);

            int count = 0;
            using var scope = _serviceProvider.CreateScope();
            var _fileProcessService = scope.ServiceProvider.GetRequiredService<FileProcessService>();
            
            foreach (var filePath in files)
            {
                if(IsMusicFile(filePath))
                {
                    await _fileProcessService.handleFile(filePath);
                }
                count++;  
            }

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