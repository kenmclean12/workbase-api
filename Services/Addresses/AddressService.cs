using Microsoft.EntityFrameworkCore;
using newproj.Data;
using newproj.DTO.Address;
using newproj.Models.Addresses;

namespace newproj.Services.Addresses;

public class AddressService(AppDbContext db) : IAddressService
{
    private readonly AppDbContext _db = db;
    public async Task<AddressResponseDTO?> Update(AddressUpdateDTO dto, int clientId)
    {
        Address? address = await _db.Addresses.FirstOrDefaultAsync(a => a.ClientId == clientId);
        if (address is null) return null;

        address.Update(dto);
        await _db.SaveChangesAsync();

        return address.ToResponse();
    }

    public async Task<bool> Delete(int clientId)
    {
        Address? address = await _db.Addresses.FirstOrDefaultAsync(a => a.ClientId == clientId);
        if (address is null) return false;

        _db.Addresses.Remove(address);
        await _db.SaveChangesAsync();

        return true;
    }
}