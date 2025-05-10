using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MusiCloud.Dtos;
using MusiCloud.Interface;

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
            var music = await _musicService.GetMusicAsync(id) ?? throw new ArgumentNullException(nameof(id), "Music not found");

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
