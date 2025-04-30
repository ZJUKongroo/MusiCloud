using Microsoft.EntityFrameworkCore;
using MusiCloud.Data;
using MusiCloud.Models;
using TagLib;


namespace MusiCloud.Services
{
    public class FileProcessService(MusiCloudDbContext context, IConfiguration configuration) : IFileProcessService
    {
        private readonly MusiCloudDbContext _context = context;
        private readonly string _coverCacheFolder = configuration["CoverCache"] ?? Path.Combine(AppContext.BaseDirectory, "CoverCache");

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        private async Task<Metadata> ProcessMetadata(string filePath, FileInfo fileInfo, TagLib.File tagFile)
        {
            var metadata = await _context.Metadatas!.FirstOrDefaultAsync(a => a.FilePath == filePath);
            if (metadata == null)
            {
                metadata = new Metadata
                {
                    Duration = TimeOnly.FromTimeSpan(tagFile.Properties.Duration),
                    FileName = fileInfo.Name,
                    FilePath = filePath,
                    FileSize = fileInfo.Length,
                };
                _context.Metadatas!.Add(metadata);
            }
            else if (metadata.FileSize != fileInfo.Length)
            {
                metadata.Duration = TimeOnly.FromTimeSpan(tagFile.Properties.Duration);
                metadata.FileName = fileInfo.Name;
                metadata.FilePath = filePath;
                metadata.UpdateTime = DateTime.UtcNow;
                _context.Metadatas!.Update(metadata);
            }
            else if (metadata.IsDeleted)
            {
                metadata.IsDeleted = false;
                metadata.UpdateTime = DateTime.UtcNow;
                _context.Metadatas!.Update(metadata);
            }
            else
            {
                metadata.UpdateTime = DateTime.UtcNow;
            }
            return metadata;
        }

        private async Task<List<Artist>> ProcessArtist(Tag tag)
        {
            var performerNames = tag.Performers.Length > 0
                ? tag.Performers
                : ["Unknown Artist"];
            var existingArtists = await _context.Artists!
                .Where(a => performerNames.Contains(a.Name))
                .ToArrayAsync();
            var artists = new List<Artist>();
            foreach (var artistName in performerNames)
            {
                var artist = existingArtists.FirstOrDefault(a => a.Name == artistName);
                if (artist is not null)
                {
                    if (artist.IsDeleted)
                    {
                        artist.IsDeleted = false;
                        _context.Artists!.Update(artist);
                    }
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

        private async Task<Album> ProcessAlbum(Tag tag, List<Artist> artists)
        {
            string albumTitle = tag.Album ?? "Unknown Album";

            // First find album by title only
            var albums = await _context.Albums!
                .Include(a => a.AlbumArtists)
                .ThenInclude(aa => aa.Artist)
                .Where(a => a.Title == albumTitle)
                .ToArrayAsync();

            if (albums.Length > 0)
            {
                foreach (var candidateAlbum in albums)
                {
                    var albumArtistIds = candidateAlbum.AlbumArtists.Select(aa => aa.ArtistId).ToArray().Order();
                    var inputArtistIds = artists.Select(a => a.Id).ToArray().Order();

                    // Check if the sets of artists match exactly
                    if (albumArtistIds.SequenceEqual(inputArtistIds))
                    {
                        if (candidateAlbum.IsDeleted)
                        {
                            candidateAlbum.IsDeleted = false;
                            candidateAlbum.UpdateTime = DateTime.UtcNow;
                            _context.Albums!.Update(candidateAlbum);
                        }
                        return candidateAlbum;
                    }
                }
            }
            // Create new album
            var album = new Album
            {
                Title = albumTitle,
                Genre = tag.Genres.Length > 0 ? tag.Genres : ["Unknown Genre"],
            };
            foreach (var artist in artists)
            {
                var albumArtist = new AlbumArtist
                {
                    AlbumId = album.Id,
                    ArtistId = artist.Id,
                    Album = album,
                    Artist = artist
                };

                _context.AlbumArtists!.Add(albumArtist);
                album.AlbumArtists.Add(albumArtist);
                artist.AlbumArtists.Add(albumArtist);
            }
            _context.Albums!.Add(album);
            await _context.SaveChangesAsync();
            return album;
        }

        private async Task ProcessMusic(TagLib.File tagFile, FileInfo fileInfo,Metadata metadata, Album album, List<Artist> artists)
        {
            var musics = await _context.Musics!
                .Include(m => m.Album)
                .Include(m => m.MusicArtists)
                .ThenInclude(ma => ma.Artist)
                .Where(m => m.Title == tagFile.Tag.Title && m.AlbumId == album.Id )
                .ToArrayAsync();
            if (musics.Length > 0)
            {
                foreach (var candidateMusic in musics)
                {
                    var candidateMusicIds = candidateMusic.MusicArtists.Select(a => a.ArtistId).ToArray().Order();
                    var inputArtistIds = artists.Select(a => a.Id).ToArray().Order();

                    if (candidateMusicIds.SequenceEqual(inputArtistIds))
                    {
                        if (candidateMusic.IsDeleted)
                        {
                            candidateMusic.IsDeleted = false;
                            candidateMusic.UpdateTime = DateTime.UtcNow;
                            _context.Musics!.Update(candidateMusic);
                        }
                    }
                }
            }
            var music = new Music
            {
                Title = tagFile.Tag.Title,
                AlbumId = album.Id,
                Metadata = metadata,
            };
            metadata.Music = music;
            metadata.MusicId = music.Id;
            foreach (var artist in artists)
            {
                var musicArtist = new MusicArtist
                {
                    MusicId = music.Id,
                    ArtistId = artist.Id,
                    Music = music,
                    Artist = artist
                };
                _context.MusicArtists!.Add(musicArtist);
                music.MusicArtists.Add(musicArtist);
                artist.MusicArtists.Add(musicArtist);
            }
            _context.Musics!.Add(music);
        }

        public async Task handleFileDeleted(string filePath)
        {
            var metadata = _context.Metadatas!.FirstOrDefault(m => m.FilePath == filePath);
            if (metadata != null)
            {
                metadata.IsDeleted = true;
                _context.Metadatas!.Update(metadata);
                metadata.DeleteTime = DateTime.UtcNow;

                var music = metadata.Music;
                if (music != null)
                {
                    var musicArtists = music.MusicArtists.ToArray();
                    foreach (var musicArtist in musicArtists)
                    {
                        musicArtist.IsDeleted = true;
                        _context.MusicArtists!.Update(musicArtist);
                    }
                    music.IsDeleted = true;    
                    music.DeleteTime = DateTime.UtcNow;
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

                var artists = await ProcessArtist(tagFile.Tag);

                var album = await ProcessAlbum(tagFile.Tag, artists);

                var metadata = await ProcessMetadata(filePath, fileInfo, tagFile);

                await ProcessMusic(tagFile, fileInfo, metadata, album, artists);

                await SaveAsync();
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error processing file: {ex.Message}");
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
                metadata.IsDeleted = false;

                _context.Metadatas!.Update(metadata);
                await SaveAsync();
            }
            else
            {
                // If old path not found, treat as a new file
                await handleFileCreated(newPath);
            }
        }

        public async Task handleFileModified(string filePath)
        {
            await handleFileDeleted(filePath);
            await handleFileCreated(filePath);
            return;
        }

        public async Task handleFile(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            var metadata = _context.Metadatas!.FirstOrDefault(f => f.FilePath == filePath);
            if (metadata == null)
            {
                await handleFileCreated(filePath);
            }
            else await handleFileModified(filePath);
        }
    }
}