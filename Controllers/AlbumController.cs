using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MusiCloud.Dtos;
using MusiCloud.Models;
using MusiCloud.Services;

namespace MusiCloud.Controller;

[ApiController]
[Route("api/[controller]")]
public class AlbumController(IAlbumService albumService, IMapper mapper, ILogger<AlbumController> logger) : ControllerBase
{
    private readonly IAlbumService _albumService = albumService;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<AlbumController> _logger = logger;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AlbumDto>>> GetAlbums()
    {
        var albums = await _albumService.GetAlbumsAsync();

        var albumDtos = _mapper.Map<IEnumerable<Album>>(albums);
        return Ok(albumDtos);
    }
}
