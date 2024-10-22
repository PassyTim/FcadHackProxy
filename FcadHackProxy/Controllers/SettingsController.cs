using FcadHackProxy.FilteringSettings;
using Microsoft.AspNetCore.Mvc;

namespace FcadHackProxy.Controllers;

[ApiController]
[Route("api/settings/")]
public class SettingsController : ControllerBase
{
    private readonly FilterSettingsService _settingsService;

    public SettingsController(FilterSettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    [HttpPost("update")]
    public IActionResult UpdateSettings([FromBody] FilterSettings newSettings)
    {
        if (newSettings == null)
        {
            return BadRequest("Invalid settings.");
        }
        
        _settingsService.UpdateSettings(newSettings);

        return Ok("Settings updated successfully.");
    }

    [HttpGet("current")]
    public IActionResult GetCurrentSettings()
    {
        var settings = _settingsService.CurrentSettings;
        return Ok(settings);
    }
}