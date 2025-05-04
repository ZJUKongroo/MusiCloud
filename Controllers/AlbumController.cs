using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MusiCloud.Dtos;
using MusiCloud.Interface;

namespace MusiCloud.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlbumController(IAlbumService albumService, IMapper mapper, ILogger<AlbumController> logger) : ControllerBase
{
    private readonly IAlbumService _albumService = albumService;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<AlbumController> _logger = logger;

    [HttpGet]
    public async Task<ActionResult<AlbumWithMusicsArtistsDto>> GetAlbum([FromQuery] Guid id)
    {
        try
        {
            var album = await _albumService.GetAlbumAsync(id);
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

    [HttpGet("recommended")]
    public async Task<ActionResult<IEnumerable<AlbumDto>>> GetRecommendedAlbums([FromQuery] int count = 10)
    {
        try
        {
            var albums = await _albumService.GetRecommendedAlbumsAsync(count);
            var albumDtos = _mapper.Map<IEnumerable<AlbumDto>>(albums);
            return Ok(albumDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取推荐专辑时出错");
            return StatusCode(500, "获取推荐专辑时发生内部错误");
        }
    }
}
