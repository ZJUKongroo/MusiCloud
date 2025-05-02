using Microsoft.EntityFrameworkCore;
using MusiCloud.Data;
using MusiCloud.Models;
using TagLib;
using System.Security.Cryptography;

namespace MusiCloud.Services
{
    public class FileProcessService(MusiCloudDbContext context, IConfiguration configuration,
     ILogger<FileProcessService> logger) : IFileProcessService
    {
        private readonly MusiCloudDbContext _context = context;
        private readonly string _coverCacheFolder = configuration["CoverCache"] ?? Path.Combine(AppContext.BaseDirectory, "CoverCache");
        private readonly ILogger<FileProcessService> _logger = logger;

        private async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        /*
            计算文件的 SHA1 哈希值
        */
        private static int ComputeFileHash(string filePath)
        {
            using var sha1 = SHA1.Create();
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var hashBytes = sha1.ComputeHash(fileStream);
            return BitConverter.ToInt32(hashBytes, 0);
        }

        /*
            根据 MIME 类型获取文件扩展名
        */
        private static string GetExtByMineType(string mimeType)
        {
            return mimeType.ToLowerInvariant() switch
            {
            "image/jpeg" => ".jpg",
            "image/png" => ".png",
            "image/gif" => ".gif",
            "image/bmp" => ".bmp",
            "image/tiff" => ".tiff",
            "image/webp" => ".webp",
            "image/svg+xml" => ".svg",
            _ => ".jpg"  // Default to jpg for unknown image types
            };
        }

        /*
            保存封面图片到缓存目录 (可配置)
            封面图片的文件名为 GUID + 扩展名
            若封面图片不存在，则返回空字符串
        */
        private async Task<string> SaveCoverImage(Tag tag)
        {
            var cover = tag.Pictures.FirstOrDefault();
            if (cover == null)
            {
                return string.Empty;
            }
            var coverFileName = Guid.NewGuid().ToString() + GetExtByMineType(cover.MimeType);
            var coverFilePath = Path.Combine(_coverCacheFolder, coverFileName);
            if (!Directory.Exists(_coverCacheFolder))
            {
                Directory.CreateDirectory(_coverCacheFolder);
            }
            using var fileStream = new FileStream(coverFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
            _logger.LogInformation("保存封面图片到 {coverFilePath}", coverFilePath);
            await fileStream.WriteAsync(cover.Data.Data.AsMemory(0, cover.Data.Data.Length));
            return coverFileName;
        }

        /*
            删除封面图片
        */
        private Task DeleteCoverImage(string coverName)
        {
            if (Path.Exists(Path.Combine(_coverCacheFolder, coverName)))
            {
                _logger.LogInformation("删除旧封面图片 {oldName}", coverName);
                System.IO.File.Delete(Path.Combine(_coverCacheFolder, coverName));
            }
            return Task.CompletedTask;
        }

        /*
            修改封面图片
            = 先删除旧封面图片 + 保存新封面图片
        */
        private async Task<string> ChangeCoverImage(string oldName, Tag tag)
        {
            await DeleteCoverImage(oldName);
            return await SaveCoverImage(tag);
        }

        /*
            处理元数据
            -  如果文件不存在，则创建新的元数据
            -  如果文件存在，则更新元数据
        */
        private async Task<Metadata?> ProcessMetadata(string filePath, FileInfo fileInfo, TagLib.File tagFile)
        {
            var metadata = await _context.Metadatas!.FirstOrDefaultAsync(a => a.FilePath == filePath && !a.IsDeleted);
            var fileHash = ComputeFileHash(filePath);
            if (metadata == null)
            {
                metadata = new Metadata
                {
                    Duration = TimeOnly.FromTimeSpan(tagFile.Properties.Duration),
                    FileName = fileInfo.Name,
                    FilePath = filePath,
                    FileSize = fileInfo.Length,
                    FileHash = fileHash,
                    UpdateTime = DateTime.UtcNow,
                };
                _context.Metadatas!.Add(metadata);
                return metadata;
            }
            // 文件被修改
            else if (metadata.FileHash != fileHash)
            {
                metadata.Duration = TimeOnly.FromTimeSpan(tagFile.Properties.Duration);
                metadata.FileName = fileInfo.Name;
                metadata.FilePath = filePath;
                metadata.FileSize = fileInfo.Length;
                metadata.FileHash = fileHash;
                metadata.UpdateTime = DateTime.UtcNow;
                _context.Metadatas!.Update(metadata);
                return metadata;
            }
            metadata.IsExisted = true;
            return null;
        }

        /*
            处理歌手
            -  如果歌手已存在，则返回已存在的艺术家
            -  如果歌手不存在，则创建新的艺术家
        */
        private async Task<List<Artist>> ProcessArtist(Tag tag)
        {
            // 避免首尾的空格导致重复创建
            var performerNames = tag.Performers.Length > 0
                ? tag.Performers.Select(p => p.Trim()).ToArray()
                : ["Unknown Artist"];

            var existingArtists = await _context.Artists!
                .Where(a => performerNames.Contains(a.Name) && !a.IsDeleted)
                .ToArrayAsync();

            var artists = new List<Artist>();
            foreach (var artistName in performerNames)
            {
                var artist = existingArtists.FirstOrDefault(a => a.Name == artistName);
                if (artist is not null)
                {
                    artists.Add(artist);
                }
                else
                {
                    var newArtist = new Artist { Name = artistName };
                    _context.Artists!.Add(newArtist);
                    artists.Add(newArtist);
                }
            }
            return artists;
        }

        /*
            处理专辑
            -  如果专辑已存在，则返回已存在的专辑
            -  如果专辑不存在，则创建新的专辑
        */
        private async Task<Album> ProcessAlbum(Tag tag, List<Artist> artists)
        {
            string albumTitle = tag.Album ?? "Unknown Album";

            var albums = await _context.Albums!
                .Include(a => a.AlbumArtists.Where(aa => !aa.IsDeleted))
                .ThenInclude(aa => aa.Artist)
                .Where(a => a.Title == albumTitle && !a.IsDeleted)
                .ToArrayAsync();

            if (albums.Length > 0)
            {
                foreach (var candidateAlbum in albums)
                {
                    var albumArtistIds = candidateAlbum.AlbumArtists
                        .Select(aa => aa.ArtistId)
                        .ToArray();

                    var inputArtistIds = artists.Select(a => a.Id).ToArray();

                    // 只要有一个相同的艺术家就可以
                    if (albumArtistIds.Intersect(inputArtistIds).Any())
                    {
                        candidateAlbum.CoverPath = string.IsNullOrEmpty(candidateAlbum.CoverPath) ? await SaveCoverImage(tag) : await ChangeCoverImage(candidateAlbum.CoverPath, tag);
                        _context.Albums!.Update(candidateAlbum);
                        return candidateAlbum;
                    }
                }
            }

            var album = new Album
            {
                Title = albumTitle,
                Genre = tag.Genres.Length > 0 ? tag.Genres : ["Unknown Genre"],
                CoverPath = await SaveCoverImage(tag),
            };

            foreach (var artist in artists)
            {
                var albumArtist = new AlbumArtist
                {
                    Album = album,
                    Artist = artist
                };

                _context.AlbumArtists!.Add(albumArtist);
                album.AlbumArtists.Add(albumArtist);
                artist.AlbumArtists.Add(albumArtist);
            }

            _context.Albums!.Add(album);
            return album;
        }

        /*
            处理音乐
            -  如果音乐已存在，则更新音乐的元数据
            -  如果音乐不存在，则创建新的音乐
        */
        private async Task ProcessMusic(TagLib.File tagFile, FileInfo fileInfo, Metadata metadata, Album album, List<Artist> artists)
        {
            var musics = await _context.Musics!
                .Include(m => m.Album)
                .Include(m => m.MusicArtists.Where(ma => !ma.IsDeleted))
                .ThenInclude(ma => ma.Artist)
                .Where(m => m.Title == tagFile.Tag.Title && m.AlbumId == album.Id && !m.IsDeleted)
                .ToArrayAsync();

            if (musics.Length > 0)
            {
                // 匹配歌手
                foreach (var candidateMusic in musics)
                {
                    var candidateMusicIds = candidateMusic.MusicArtists
                        .Select(a => a.ArtistId)
                        .ToArray()
                        .Order();

                    var inputArtistIds = artists.Select(a => a.Id).ToArray().Order();

                    // 歌手必须一一对应
                    if (candidateMusicIds.SequenceEqual(inputArtistIds))
                    {
                        if (candidateMusic.Metadata != null && candidateMusic.Metadata.Id != metadata.Id)
                        {
                            candidateMusic.Metadata.IsDeleted = true;
                            candidateMusic.Metadata.DeleteTime = DateTime.UtcNow;
                        }

                        candidateMusic.Metadata = metadata;
                        metadata.Music = candidateMusic;
                        metadata.MusicId = candidateMusic.Id;

                        return;
                    }
                }
            }

            var music = new Music
            {
                Title = tagFile.Tag.Title,
                AlbumId = album.Id,
                Metadata = metadata,
                Track = tagFile.Tag.Track
            };

            metadata.Music = music;
            metadata.MusicId = music.Id;

            foreach (var artist in artists)
            {
                var musicArtist = new MusicArtist
                {
                    Music = music,
                    Artist = artist
                };

                _context.MusicArtists!.Add(musicArtist);
                music.MusicArtists.Add(musicArtist);
                artist.MusicArtists.Add(musicArtist);
            }
        }

        /*
            清理孤立的艺术家
            如果艺术家没有任何音乐，则标记为删除
        */
        private async Task CleanupOrphanedArtists(Guid[] artistIds)
        {
            foreach (var artistId in artistIds)
            {
                var hasActiveMusic = await _context.MusicArtists!
                    .AnyAsync(ma => ma.ArtistId == artistId && !ma.IsDeleted);

                if (!hasActiveMusic)
                {
                    var artist = await _context.Artists!
                        .Include(a => a.AlbumArtists)
                        .FirstOrDefaultAsync(a => a.Id == artistId && !a.IsDeleted);

                    if (artist != null)
                    {
                        _logger.LogInformation("艺术家 {artistName} 已无关联歌曲，标记为删除", artist.Name);
                        artist.IsDeleted = true;
                        artist.DeleteTime = DateTime.UtcNow;

                        foreach (var albumArtist in artist.AlbumArtists.Where(aa => !aa.IsDeleted))
                        {
                            albumArtist.IsDeleted = true;
                            albumArtist.DeleteTime = DateTime.UtcNow;
                        }
                    }
                }
            }
        }

        /*
            清理孤立的专辑
            如果专辑没有任何音乐，则标记为删除
        */
        private async Task CleanupOrphanedAlbum(Guid albumId)
        {
            var hasActiveMusic = await _context.Musics!
                .AnyAsync(m => m.AlbumId == albumId && !m.IsDeleted);

            if (!hasActiveMusic)
            {
                var album = await _context.Albums!
                    .Include(a => a.AlbumArtists)
                    .FirstOrDefaultAsync(a => a.Id == albumId && !a.IsDeleted);

                if (album != null)
                {
                    _logger.LogInformation("专辑 {albumTitle} 已无关联歌曲，标记为删除", album.Title);
                    album.IsDeleted = true;
                    album.DeleteTime = DateTime.UtcNow;

                    if (!string.IsNullOrEmpty(album.CoverPath))
                    {
                        await DeleteCoverImage(album.CoverPath);
                    }

                    // 标记相关的 AlbumArtist 记录为删除
                    foreach (var albumArtist in album.AlbumArtists.Where(aa => !aa.IsDeleted))
                    {
                        albumArtist.IsDeleted = true;
                        albumArtist.DeleteTime = DateTime.UtcNow;
                    }
                }
            }
        }

        /*
            传入要删除的元数据的引用
            元数据标记为删除
            之后删除相关的音乐、艺术家和专辑
        */
        private async Task<bool> CleanupOrphanedData(Metadata? metadata)
        {
            if (metadata == null) return false;

            metadata.IsDeleted = true;
            metadata.DeleteTime = DateTime.UtcNow;

            _context.Metadatas!.Update(metadata);

            var music = metadata.Music;

            if (music == null)
            {
                return false;
            }

            music.IsDeleted = true;
            music.DeleteTime = DateTime.UtcNow;
            _context.Musics!.Update(music);

            foreach (var musicArtist in music.MusicArtists.Where(ma => !ma.IsDeleted))
            {
                musicArtist.IsDeleted = true;
                musicArtist.DeleteTime = DateTime.UtcNow;
                _context.MusicArtists!.Update(musicArtist);
            }

            await SaveAsync();

            await CleanupOrphanedArtists([.. music.MusicArtists.Select(ma => ma.ArtistId)]);

            await CleanupOrphanedAlbum(music.AlbumId);

            return true;
        }

        /*
            处理文件删除事件
        */
        public async Task HandleFileDeletedAsync(string filePath)
        {
            var metadata = await _context.Metadatas!
                .Include(m => m.Music)
                    .ThenInclude(m => m!.MusicArtists)
                        .ThenInclude(ma => ma.Artist)
                .Include(m => m.Music)
                    .ThenInclude(m => m!.Album)
                        .ThenInclude(a => a.AlbumArtists)
                .FirstOrDefaultAsync(m => m.FilePath == filePath && !m.IsDeleted);
            bool res = await CleanupOrphanedData(metadata);
            await SaveAsync();
            if(res){
                _logger.LogInformation("文件 {filePath} 已删除，相关数据已清理", filePath);
            }
            else{
                _logger.LogInformation("文件 {filePath} 已删除，但未找到相关数据", filePath);
            }
        }

        /*
            处理文件创建事件
        */
        public async Task HandleFileCreatedAsync(string filePath)
        {
            try
            {
                var fileInfo = new FileInfo(filePath);
                using var tagFile = TagLib.File.Create(filePath);
                var metadata = await ProcessMetadata(filePath, fileInfo, tagFile);

                if (metadata == null)
                {
                    _logger.LogInformation("文件 {filePath} 的元数据未更新", filePath);
                    await SaveAsync();
                    return;
                }

                var artists = await ProcessArtist(tagFile.Tag);
                var album = await ProcessAlbum(tagFile.Tag, artists);
                await ProcessMusic(tagFile, fileInfo, metadata, album, artists);
                await SaveAsync();

                _logger.LogInformation("文件 {filePath} 处理完成", filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理文件 {filePath} 时发生错误", filePath);
            }
        }

        /*
            处理文件重命名事件
        */
        public async Task HandleFileRenamedAsync(string oldPath, string newPath)
        {
            var metadata = _context.Metadatas!.FirstOrDefault(m => m.FilePath == oldPath && !m.IsDeleted);
            if (metadata != null)
            {
                metadata.FilePath = newPath;
                metadata.FileName = Path.GetFileName(newPath);
                metadata.UpdateTime = DateTime.UtcNow;

                _context.Metadatas!.Update(metadata);
                _logger.LogInformation("文件 {oldPath} 重命名为 {newPath}", oldPath, newPath);
                await SaveAsync();
            }
            else
            {
                await HandleFileCreatedAsync(newPath);
            }
        }

        /*
            先标记所有元数据为不存在
            在扫盘的过程中，如果发现文件存在，则将 IsExisted 设置为 true
        */
        public async Task InitializeAsync()
        {
            var metadatas = await _context.Metadatas!
                .Where(m=>!m.IsDeleted)
                .ToListAsync();

            foreach (var metadata in metadatas)
            {
                metadata.IsExisted = false;
            }
            await SaveAsync();
        }

        /*
            清理扫盘后 IsExisted 仍为 false 的元数据
        */
        public async Task CleanupAsync()
        {
            var metadatas = await _context.Metadatas!
                .Include(m => m.Music)
                    .ThenInclude(m => m!.MusicArtists)
                        .ThenInclude(ma => ma.Artist)
                .Include(m => m.Music)
                    .ThenInclude(m => m!.Album)
                        .ThenInclude(a => a.AlbumArtists)
                .Where(m => !m.IsExisted && !m.IsDeleted)
                .ToListAsync();

            _logger.LogInformation("开始清理无效的元数据...");
            int count = 0;
            foreach (var metadata in metadatas){
                bool res = await CleanupOrphanedData(metadata);
                count++;
            }
            
            await SaveAsync();
            _logger.LogInformation("清理完成，处理了 {count} 个文件", count);
        }
    }
}
