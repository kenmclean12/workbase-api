using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using newproj.DTO.Address;
using newproj.Services.Addresses;

namespace newproj.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class AddressController(IAddressService addressService) : ControllerBase
{
    private readonly IAddressService _addressService = addressService;

    [HttpPut("{clientId}")]
    [Authorize(Policy = "AdminOrManager")]
    public async Task<IActionResult> Update(AddressUpdateDTO dto, int clientId)
    {
        AddressResponseDTO? response = await _addressService.Update(dto, clientId);
        if (response is null) return NotFound($"Address not found with the provided client id: {clientId}");
        return Ok(response);
    }

    [HttpDelete("{clientId}")]
    [Authorize(Policy = "AdminOrManager")]
    public async Task<IActionResult> Delete(int clientId)
    {
        bool deleted = await _addressService.Delete(clientId);
        if (!deleted) return NotFound($"No address was found with the provided client id: {clientId}");
        return NoContent();
    }
}