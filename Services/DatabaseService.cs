using Microsoft.EntityFrameworkCore;
using MusiCloud.Data;
using MusiCloud.Interface;

namespace MusiCloud.Services;

public class DatabaseService(MusiCloudDbContext context, ILogger<DatabaseService> logger,IFileWatchService fileWatchService) : IDatabaseService
{
    private readonly MusiCloudDbContext _context = context;

    private readonly ILogger<DatabaseService> _logger = logger;

    private readonly IFileWatchService _fileWatchService = fileWatchService;

    private async Task<bool> MarkAllDeleted()
    {
        try
        {
            var currentTime = DateTime.Now;

            var allMusics = await _context.Musics!.ToListAsync();
            var allArtists = await _context.Artists!.ToListAsync();
            var allAlbums = await _context.Albums!.ToListAsync();
            var allMetadatas = await _context.Metadatas!.ToListAsync();
            var allMusicArtists = await _context.MusicArtists!.ToListAsync();
            var allAlbumArtists = await _context.AlbumArtists!.ToListAsync();

            foreach (var entity in allMusics)
            {
                entity.IsDeleted = true;
                entity.DeleteTime = currentTime;
            }

            foreach (var entity in allArtists)
            {
                entity.IsDeleted = true;
                entity.DeleteTime = currentTime;
            }

            foreach (var entity in allAlbums)
            {
                entity.IsDeleted = true;
                entity.DeleteTime = currentTime;
            }

            foreach (var entity in allMetadatas)
            {
                entity.IsDeleted = true;
                entity.DeleteTime = currentTime;
            }

            foreach (var entity in allMusicArtists)
            {
                entity.IsDeleted = true;
                entity.DeleteTime = currentTime;
            }

            foreach (var entity in allAlbumArtists)
            {
                entity.IsDeleted = true;
                entity.DeleteTime = currentTime;
            }

            await SaveAsync();

            _logger.LogInformation("所有记录都已标记删除.");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除记录失败");
            return false;
        }
    }

    public async Task<bool> RebuildDatabaseAsync()
    {
        _logger.LogInformation("开始重建数据库...");

        try
        {
            await _fileWatchService.StopAsync(CancellationToken.None);
            if(await MarkAllDeleted()){
                await _fileWatchService.StartAsync(CancellationToken.None);
                return true;
            }
            else throw new Exception("标记删除失败");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "重建数据库失败");
            return false;
        }
    }

    private async Task<bool> SaveAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}