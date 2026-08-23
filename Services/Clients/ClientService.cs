using Microsoft.EntityFrameworkCore;
using newproj.Data;
using newproj.DTO.Client;
using newproj.Models.Clients;

namespace newProj.Services.Clients;

public static class ClientService
{
  public static void MapClientEndpoints(this WebApplication app)
  {
    RouteGroupBuilder group = app.MapGroup("/client");

    group.MapGet("/", async (AppDbContext db) =>
    {
      var clients = await db.Clients
        .Include(c => c.Address)
        .ToArrayAsync();

      return Results.Ok(clients.Select(c => c.ToResponse()).ToArray());
    });

    group.MapGet("/{id:int}", async (AppDbContext db, int id) =>
    {
      var client = await db.Clients
            .Include(c => c.Address)
            .FirstOrDefaultAsync(c => c.Id == id);

      if (client is null)
      {
        return Results.NotFound($"No client was found with the provided id: {id}");
      }

      return Results.Ok(client.ToResponse());
    });

    group.MapPost("/", async (AppDbContext db, ClientCreateDTO dto) =>
    {
       var client = Client.Create(dto);
       db.Clients.Add(client);
       await db.SaveChangesAsync();
       return Results.Created($"/client/{client.Id}", client.ToResponse());
    });

    group.MapPut("/{id:int}", async (AppDbContext db, ClientUpdateDTO dto, int id) =>
    {
      var client = await db.Clients.FirstOrDefaultAsync(c => c.Id == id);
      if (client is null)
      {
        return Results.NotFound($"No client was found with the provided id: {id}");
      }

      client.Update(dto);
      await db.SaveChangesAsync();

      return Results.Ok(client.ToResponse());
    });

    group.MapDelete("/{id:int}", async (AppDbContext db, int id) =>
    {
      var client = await db.Clients.FirstOrDefaultAsync(c => c.Id == id);
      if (client is null)
      {
        return Results.NotFound($"No client was found with the provided id: {id}");
      }

      db.Clients.Remove(client);
      await db.SaveChangesAsync();

      return Results.NoContent();
    });
  }
}