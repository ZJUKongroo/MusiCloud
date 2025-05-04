using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MusiCloud.Dtos;
using MusiCloud.Interface;

namespace MusiCloud.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArtistController(IArtistService artistService, IMapper mapper, ILogger<ArtistController> logger) : ControllerBase
{
    private readonly IArtistService _artistService = artistService;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<ArtistController> _logger = logger;

    [HttpGet]
    public async Task<ActionResult<ArtistWithAlbumDto>> GetArtist([FromQuery] Guid id)
    {
        try
        {
            var artist = await _artistService.GetArtistAsync(id);
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
}
