using FcadHackProxy.Data;
using FcadHackProxy.Dto;
using Microsoft.AspNetCore.Mvc;

namespace FcadHackProxy.Controllers;

[ApiController]
[Route("api/servers")]
public class ServerController : ControllerBase
{
    private readonly ServerRepository _serverRepository;

    public ServerController(ServerRepository serverRepository)
    {
        _serverRepository = serverRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllServers()
    {
        var servers = await _serverRepository.GetAllServersAsync();
        return Ok(servers);
    }

    [HttpPost]
    public async Task<IActionResult> AddServer([FromBody] Server server)
    {
        if (string.IsNullOrEmpty(server.Name) || string.IsNullOrEmpty(server.Url))
        {
            return BadRequest("Server name and URL are required.");
        }

        await _serverRepository.AddServerAsync(server);
        return Ok("Server added successfully.");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteServer(string id)
    {
        await _serverRepository.DeleteServerAsync(id);
        return Ok("Server deleted successfully.");
    }
}