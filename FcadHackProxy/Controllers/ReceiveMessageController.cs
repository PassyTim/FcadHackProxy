using FcadHackProxy.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace FcadHackProxy.Controllers;

[ApiController]
[Route("proxy/send/")]
public class ReceiveMessageController(MessageFilterService filterService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<JObject>> ReceiveMessage([FromBody] JObject message)
    {
        var jsonObject = await filterService.ExecuteAsync(message);
        return Ok(jsonObject);
    }
}