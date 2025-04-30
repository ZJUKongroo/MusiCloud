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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MusicDto>>> GetMusics()
    {
        try
        {
            var musics = await _musicService.GetMusicsAsync();
            var musicDtos = _mapper.Map<IEnumerable<MusicDto>>(musics);
            return Ok(musicDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取音乐列表时出错");
            return StatusCode(500, "获取音乐列表时发生内部错误");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MusicDto>> GetMusic(Guid id)
    {
        try
        {
            var music = await _musicService.GetMusicAsync(id);
            if (music == null)
                return NotFound();

            var musicDto = _mapper.Map<MusicDto>(music);
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
}
