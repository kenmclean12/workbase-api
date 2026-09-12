using Microsoft.EntityFrameworkCore;
using newproj.Data;
using newproj.DTO.Address;

namespace newproj.Services.Addresses;

public static class AddressService
{
    public static void MapAddressEndpoints(this WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup("/address").RequireAuthorization();

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
        })
            .RequireAuthorization("AdminOrManager");

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
        })
            .RequireAuthorization("AdminOrManager");
    }
}