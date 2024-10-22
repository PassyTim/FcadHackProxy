using FcadHackProxy.Dto;
using FcadHackProxy.FilteringSettings;
using Microsoft.AspNetCore.Mvc;

namespace FcadHackProxy.Controllers;

[ApiController]
[Route("api/regex")]
public class RegexController : ControllerBase
{
    private readonly FilterSettingsService _settingsService;

    public RegexController(FilterSettingsService settingsService)
    {
        _settingsService = settingsService;
    }
    [HttpPost("add")]
    public IActionResult AddRegexPattern([FromBody] RegexPatternDto dto)
    {
        if (string.IsNullOrEmpty(dto.Name) || string.IsNullOrEmpty(dto.Pattern))
        {
            return BadRequest("Invalid pattern or name.");
        }

        _settingsService.AddRegexPattern(dto.Name, dto.Pattern);
        return Ok("Regex pattern added successfully.");
    }

    [HttpDelete("remove")]
    public IActionResult RemoveRegexPattern([FromQuery] string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return BadRequest("Invalid name.");
        }

        _settingsService.RemoveRegexPattern(name);
        return Ok("Regex pattern removed successfully.");
    }

    [HttpGet("all")]
    public IActionResult GetAllRegexPatterns()
    {
        var patterns = _settingsService.GetAllPatterns();
        return Ok(patterns);
    }
}