using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using newproj.DTO.Client;
using newproj.Services.Clients;

namespace newproj.Controllers;

[ApiController]
[Authorize]
[Route("client")]
public class ClientController(IClientService clientService) : ControllerBase
{
    private readonly IClientService _clientService = clientService;

    [HttpGet]
    public async Task<IActionResult> FindAll()
    {
        return Ok(await _clientService.FindAll());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> FindOne(int id)
    {
        ClientResponseDTO? client = await _clientService.FindOne(id);
        if (client is null) return NotFound($"No client was found with the provided id: {id}");
        return Ok(client);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOrManager")]
    public async Task<IActionResult> Create(ClientCreateDTO dto)
    {
        ClientResponseDTO? client = await _clientService.Create(dto);
        if (client is null) return BadRequest("An error occured while creating the client with the provided data");
        return Created($"client/{client.Id}", client);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "AdminOrManager")]
    public async Task<IActionResult> Update(ClientUpdateDTO dto, int id)
    {
        ClientResponseDTO? client = await _clientService.Update(dto, id);
        if (client is null) return NotFound($"No client was found with the provided id: {id}");
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminOrManager")]
    public async Task<IActionResult> Delete(HttpContext context, int id)
    {
        if (int.TryParse(context.User.FindFirstValue("client_id"), out int clientId) && id == clientId)
        {
            return Forbid();
        }

        bool deleted = await _clientService.Delete(id);
        if (!deleted) return NotFound($"No client was found with the provided id: {id}");

        return NoContent();
    }
}