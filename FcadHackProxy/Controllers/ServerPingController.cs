using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FcadHackProxy.Controllers;

[ApiController]
[Route("api/ping")]
public class ServerPingController : ControllerBase
{
    [HttpGet]
    public ActionResult<string> PingServer()
    {
        return Ok("pong");
    }
}