using FcadHackProxy.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace FcadHackProxy.Controllers;

[ApiController]
[Route("api/messages")]
public class SensitiveMessagesController(IMessageRepository repository) : ControllerBase
{
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSensitiveMessage(string id)
    {
        await repository.DeleteAsync(id);
        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<JObject>>> GetAllMessages([FromQuery] int pageSize, int pageNumber)
    {
        var messages = await repository.GetAllAsync(pageSize, pageNumber);
        return Ok(messages);
    }
}