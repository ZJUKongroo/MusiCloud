using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MusiCloud.Dtos;
using MusiCloud.Services;

namespace MusiCloud.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MusicController(IMusicService musicService, IMapper mapper, ILogger<MusicController> logger) : ControllerBase
{
    private readonly IMusicService _musicService = musicService;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<MusicController> _logger = logger;

    [HttpGet("{id}")]
    public async Task<ActionResult<MusicWithAlbumArtistDto>> GetMusic(Guid id)
    {
        try
        {
            var music = await _musicService.GetMusicAsync(id);
            if (music == null)
                return NotFound();

            var musicDto = _mapper.Map<MusicWithAlbumArtistDto>(music);
            return Ok(musicDto);
        }
        catch (ArgumentNullException)
        {
            return BadRequest("无效的音乐ID");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取音乐详情时出错 ID: {Id}", id);
            return StatusCode(500, "获取音乐详情时发生内部错误");
        }
    }

    [HttpGet("album/{id}")]
    public async Task<ActionResult<AlbumWithMusicsArtistsDto>> GetAlbum(Guid id)
    {
        try
        {
            var album = await _musicService.GetAlbumAsync(id);
            if (album == null)
                return NotFound();

            var albumDto = _mapper.Map<AlbumWithMusicsArtistsDto>(album);
            return Ok(albumDto);
        }
        catch (ArgumentNullException)
        {
            return BadRequest("无效的专辑ID");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取专辑详情时出错 ID: {Id}", id);
            return StatusCode(500, "获取专辑详情时发生内部错误");
        }
    }

    [HttpGet("artist/{id}")]
    public async Task<ActionResult<ArtistWithAlbumDto>> GetArtist(Guid id)
    {
        try
        {
            var artist = await _musicService.GetArtistAsync(id);
            if (artist == null)
                return NotFound();

            var artistDto = _mapper.Map<ArtistWithAlbumDto>(artist);
            return Ok(artistDto);
        }
        catch (ArgumentNullException)
        {
            return BadRequest("无效的艺术家ID");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取艺术家详情时出错 ID: {Id}", id);
            return StatusCode(500, "获取艺术家详情时发生内部错误");
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<SearchResultDto>> SearchAll([FromQuery] string query)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("搜索关键词不能为空");

            var musics = await _musicService.SearchMusicAsync(query);
            var albums = await _musicService.SearchAlbumAsync(query);
            var artists = await _musicService.SearchArtistAsync(query);
            var result = _mapper.Map<SearchResultDto>(new SearchResult {
                Musics = musics,
                Albums = albums,
                Artists = artists
            });
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "搜索音乐时出错 Query: {Query}", query);
            return StatusCode(500, "搜索音乐时发生内部错误");
        }
    }

    [HttpGet("search/music")]
    public async Task<ActionResult<IEnumerable<MusicWithAlbumArtistDto>>> SearchMusic([FromQuery] string query)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("搜索关键词不能为空");

            var musics = await _musicService.SearchMusicAsync(query);
            var musicDtos = _mapper.Map<IEnumerable<MusicWithAlbumArtistDto>>(musics);
            return Ok(musicDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "搜索音乐时出错 Query: {Query}", query);
            return StatusCode(500, "搜索音乐时发生内部错误");
        }
    }

    [HttpGet("search/album")]
    public async Task<ActionResult<IEnumerable<AlbumWithMusicsArtistsDto>>> SearchAlbum([FromQuery] string query)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("搜索关键词不能为空");

            var albums = await _musicService.SearchAlbumAsync(query);
            var albumDtos = _mapper.Map<IEnumerable<AlbumWithMusicsArtistsDto>>(albums);
            return Ok(albumDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "搜索专辑时出错 Query: {Query}", query);
            return StatusCode(500, "搜索专辑时发生内部错误");
        }
    }

    [HttpGet("search/artist")]
    public async Task<ActionResult<IEnumerable<ArtistWithAlbumDto>>> SearchArtist([FromQuery] string query)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("搜索关键词不能为空");

            var artists = await _musicService.SearchArtistAsync(query);
            var artistDtos = _mapper.Map<IEnumerable<ArtistWithAlbumDto>>(artists);
            return Ok(artistDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "搜索艺术家时出错 Query: {Query}", query);
            return StatusCode(500, "搜索艺术家时发生内部错误");
        }
    }

    [HttpGet("recommended")]
    public async Task<ActionResult<IEnumerable<MusicWithAlbumArtistDto>>> GetRecommendedMusic([FromQuery] int count = 10)
    {
        try
        {
            var musics = await _musicService.GetRecommendedMusicAsync(count);
            var musicDtos = _mapper.Map<IEnumerable<MusicWithAlbumArtistDto>>(musics);
            return Ok(musicDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取推荐音乐时出错");
            return StatusCode(500, "获取推荐音乐时发生内部错误");
        }
    }

    [HttpGet("recommended/albums")]
    public async Task<ActionResult<IEnumerable<AlbumWithMusicsArtistsDto>>> GetRecommendedAlbums([FromQuery] int count = 10)
    {
        try
        {
            var albums = await _musicService.GetRecommendedAlbumsAsync(count);
            var albumDtos = _mapper.Map<IEnumerable<AlbumWithMusicsArtistsDto>>(albums);
            return Ok(albumDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取推荐专辑时出错");
            return StatusCode(500, "获取推荐专辑时发生内部错误");
        }
    }

    [HttpGet("latest")]
    public async Task<ActionResult<IEnumerable<MusicWithAlbumArtistDto>>> GetLatestMusic([FromQuery] int count = 10)
    {
        try
        {
            var musics = await _musicService.GetLatestMusicAsync(count);
            var musicDtos = _mapper.Map<IEnumerable<MusicWithAlbumArtistDto>>(musics);
            return Ok(musicDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取最新音乐时出错");
            return StatusCode(500, "获取最新音乐时发生内部错误");
        }
    }
}
