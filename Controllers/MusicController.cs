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
        var musics = await _musicService.GetMusicsAsync();

        var musicDtos = _mapper.Map<IEnumerable<MusicDto>>(musics);
        return Ok(musicDtos);
    }
}
