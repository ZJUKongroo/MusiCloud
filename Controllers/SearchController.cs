using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MusiCloud.Dtos;
using MusiCloud.Interface;

namespace MusiCloud.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController(ISearchService searchService, IMapper mapper, ILogger<SearchController> logger) : ControllerBase
{
    private readonly ISearchService _searchService = searchService;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<SearchController> _logger = logger;

    [HttpGet]
    public async Task<ActionResult<SearchResultDto>> SearchAll([FromQuery] string query)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("搜索关键词不能为空");

            var musics = await _searchService.SearchMusicAsync(query);
            var albums = await _searchService.SearchAlbumAsync(query);
            var artists = await _searchService.SearchArtistAsync(query);
            var result = _mapper.Map<SearchResultDto>(new SearchResult
            {
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

    [HttpGet("music")]
    public async Task<ActionResult<IEnumerable<MusicWithAlbumArtistDto>>> SearchMusic([FromQuery] string query)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("搜索关键词不能为空");

            var musics = await _searchService.SearchMusicAsync(query);
            var musicDtos = _mapper.Map<IEnumerable<MusicWithAlbumArtistDto>>(musics);
            return Ok(musicDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "搜索音乐时出错 Query: {Query}", query);
            return StatusCode(500, "搜索音乐时发生内部错误");
        }
    }

    [HttpGet("album")]
    public async Task<ActionResult<IEnumerable<AlbumDto>>> SearchAlbum([FromQuery] string query)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("搜索关键词不能为空");

            var albums = await _searchService.SearchAlbumAsync(query);
            var albumDtos = _mapper.Map<IEnumerable<AlbumDto>>(albums);
            return Ok(albumDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "搜索专辑时出错 Query: {Query}", query);
            return StatusCode(500, "搜索专辑时发生内部错误");
        }
    }

    [HttpGet("artist")]
    public async Task<ActionResult<IEnumerable<ArtistDto>>> SearchArtist([FromQuery] string query)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("搜索关键词不能为空");

            var artists = await _searchService.SearchArtistAsync(query);
            var artistDtos = _mapper.Map<IEnumerable<ArtistDto>>(artists);
            return Ok(artistDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "搜索艺术家时出错 Query: {Query}", query);
            return StatusCode(500, "搜索艺术家时发生内部错误");
        }
    }
}
