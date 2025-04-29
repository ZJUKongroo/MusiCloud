using Microsoft.AspNetCore.Mvc;
using MusiCloud.Services;

namespace MusiCloud.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MusicController(IMusicService musicService, ILogger<MusicController> logger) : ControllerBase
{
    private readonly IMusicService _musicService = musicService;
    private readonly ILogger<MusicController> _logger = logger;
}
