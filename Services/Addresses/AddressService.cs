using Microsoft.EntityFrameworkCore;
using newproj.Data;
using newproj.DTO.Address;
using newproj.Models.Addresses;

namespace newproj.Services.Addresses;

public static class AddressService
{
    public static void MapAddressEndpoints(this WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup("/address");

        group.MapPost("/", async (AppDbContext db, AddressCreateDTO dto) =>
        {
            var client = await db.Clients.FirstOrDefaultAsync(c => c.Id == dto.ClientId);
            if (client is null)
            {
                return Results.NotFound($"No client was found with the provided client id: {dto.ClientId}");
            }

            Address address = Address.Create(dto);
            db.Addresses.Add(address);
            await db.SaveChangesAsync();
            return Results.Created($"/address/{address.Id}", address.ToResponse());
        });

        group.MapPut("/{clientId:int}", async (AppDbContext db, AddressUpdateDTO dto, int clientId) =>
        {
            var address = await db.Addresses.FirstOrDefaultAsync(a => a.ClientId == clientId);
            if (address is null)
            {
                return Results.NotFound($"No address was found with the provided client id: {clientId}");
            }

            address.Update(dto);
            await db.SaveChangesAsync();
            return Results.Ok(address.ToResponse());
        });

        group.MapDelete("/{clientId:int}", async (AppDbContext db, int clientId) =>
        {
            var address = await db.Addresses.FirstOrDefaultAsync(a => a.ClientId == clientId);
            if (address is null)
            {
                return Results.NotFound($"No address was found with the provided client id: {clientId}");
            }

            db.Addresses.Remove(address);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });
    }
}