using MusiCloud.Data;
using MusiCloud.Models;


namespace MusiCloud.Services
{
    public class FileProcessService(MusiCloudDbContext context,IConfiguration configuration) : IFileProcessService
    {
        private readonly MusiCloudDbContext _context = context;
         private readonly string _coverCacheFolder = configuration["CoverCache"] ?? Path.Combine(AppContext.BaseDirectory, "CoverCache");

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        private void ProcessMetadata(string filePath, FileInfo fileInfo, TagLib.File tagFile, Music music)
        {
            var metadata = _context.Metadatas!.FirstOrDefault(a => a.FilePath == filePath);
            if (metadata == null)
            {
                metadata = new Models.Metadata
                {
                    Music = music,
                    Duration = TimeOnly.FromTimeSpan(tagFile.Properties.Duration),
                    FileName = fileInfo.Name,
                    FilePath = filePath,
                    FileSize = fileInfo.Length,
                };
                _context.Metadatas!.Add(metadata);
            }
            else
            {
                metadata.Duration = TimeOnly.FromTimeSpan(tagFile.Properties.Duration);
                metadata.FileName = fileInfo.Name;
                metadata.FilePath = filePath;
                _context.Metadatas!.Update(metadata);
            }
        }

        private void ProcessArtist(string artistName)
        {
            var artist = _context.Artists?.FirstOrDefault(a => a.Name == artistName);
            if (artist == null)
            {
                artist = new Models.Artist { Name = artistName };
                _context.Artists!.Add(artist);
            }
            else if(artist.IsDeleted)
            {
                artist.IsDeleted = false;
                _context.Artists!.Update(artist);
            }
        }

        private void ProcessAlbum(string albumTitle, Artist artist)
        {
            var album = _context.Albums?.FirstOrDefault(a => a.Title == albumTitle && a.Artists.Any(ar => ar.Id == artist.Id));
            if (album == null)
            {
                album = new Models.Album 
                { 
                    Title = albumTitle,
                };
                album.Artists.Add(artist);
                _context.Albums!.Add(album);
            }
            else if(album.IsDeleted)
            {
                album.IsDeleted = false;
                _context.Albums!.Update(album);
            }
        }

        public async Task handleFileDeleted(string filePath)
        {
            var metadata = _context.Metadatas!.FirstOrDefault(m => m.FilePath == filePath);
            if (metadata != null)
            {
                var music = metadata.Music;
                if (music != null)
                {
                    music.IsDeleted = true;
                    metadata.IsDeleted = true;
                    
                    metadata.DeleteTime = DateTime.UtcNow;
                    music.DeleteTime = DateTime.UtcNow;
                    
                    _context.Metadatas!.Update(metadata);
                    _context.Musics!.Update(music);
                    
                    await SaveAsync();
                }
            }
        }

        public async Task handleFileCreated(string filePath)
        {
            try
            {
                var fileInfo = new FileInfo(filePath);
                    
                using var tagFile = TagLib.File.Create(filePath);
                
                // Process Artist
                string artistName = tagFile.Tag.Performers.FirstOrDefault() ?? "Unknown Artist";
                ProcessArtist(artistName);
                var artist = _context.Artists!.FirstOrDefault(a => a.Name == artistName);
                
                // Process Album
                string albumTitle = tagFile.Tag.Album ?? "Unknown Album";
                ProcessAlbum(albumTitle, artist!);
                var album = _context.Albums!.FirstOrDefault(a => a.Title == albumTitle && a.Artists.Any(ar => ar.Id == artist!.Id));
                
                // Create Music
                var music = new Music
                {
                    // Set properties based on the file and tags
                };
                _context.Musics!.Add(music);
                
                // Process Metadata
                ProcessMetadata(filePath, fileInfo, tagFile, music);
                
                await SaveAsync();
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error processing file: {ex.Message}");
            }
        }

        public async Task handleFileModified(string filePath)
        {
            var metadata = _context.Metadatas!.FirstOrDefault(m => m.FilePath == filePath);
            if (metadata != null && metadata.Music != null)
            {
                var fileInfo = new FileInfo(filePath);
                using var tagFile = TagLib.File.Create(filePath);
                
                // Update metadata
                ProcessMetadata(filePath, fileInfo, tagFile, metadata.Music);
                
                // Update artist info if needed
                string artistName = tagFile.Tag.Performers.FirstOrDefault() ?? "Unknown Artist";
                ProcessArtist(artistName);
                var artist = _context.Artists!.FirstOrDefault(a => a.Name == artistName);
                
                // Update album info if needed
                string albumTitle = tagFile.Tag.Album ?? "Unknown Album";
                ProcessAlbum(albumTitle, artist!);
                
                await SaveAsync();
            }
            else
            {
                // If the file is not in the database, treat it as a new file
                await handleFileCreated(filePath);
            }
        }

        public async Task handleFileRenamed(string oldPath, string newPath)
        {
            var metadata = _context.Metadatas!.FirstOrDefault(m => m.FilePath == oldPath);
            if (metadata != null)
            {
                // Update the file path
                metadata.FilePath = newPath;
                metadata.FileName = Path.GetFileName(newPath);
                metadata.UpdateTime = DateTime.UtcNow;
                
                _context.Metadatas!.Update(metadata);
                await SaveAsync();
            }
            else
            {
                // If old path not found, treat as a new file
                await handleFileCreated(newPath);
            }
        }
        public async Task handleFile(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            var metadata = _context.Metadatas!.FirstOrDefault(f=>f.FilePath == filePath);
            if(metadata == null){
                await handleFileCreated(filePath);
            }
            else await handleFileModified(filePath);
        }
    }
}