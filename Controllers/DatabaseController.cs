using Microsoft.AspNetCore.Mvc;
using MusiCloud.Interface;

namespace MusiCloud.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DatabaseController(IDatabaseService databaseService) : ControllerBase
{
    private readonly IDatabaseService _databaseService = databaseService;

    [HttpPost("reset")]
    public async Task<IActionResult> ResetDatabase()
    {
        try
        {
            if(await _databaseService.RebuildDatabaseAsync()){
                return Ok();
            }
            else throw new Exception("数据库重建失败");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}