using Microsoft.EntityFrameworkCore;
using newproj.Data;
using newproj.DTO.Client;
using newproj.DTO.User;
using newproj.Models.Addresses;
using newproj.Models.Clients;
using newproj.Models.Users;
using newproj.Services.Clients;

namespace newProj.Services.Clients;

public class ClientService(AppDbContext db) : IClientService
{
  private readonly AppDbContext _db = db;

  public async Task<ICollection<ClientResponseDTO>> FindAll()
  {
    ICollection<Client> clients = await _db.Clients
        .Include(c => c.Address)
        .ToArrayAsync();

    return [.. clients.Select(c => c.ToResponse())];
  }

  public async Task<ClientResponseDTO?> FindOne(int id)
  {
    Client? client = await _db.Clients
            .Include(c => c.Address)
            .FirstOrDefaultAsync(c => c.Id == id);

    if (client is null) return null;
    return client.ToResponse();
  }

  public async Task<ClientResponseDTO> Create(ClientCreateDTO dto)
  {
    await using var transaction = await _db.Database.BeginTransactionAsync();
    try
    {
      var client = Client.Create(dto);
      _db.Clients.Add(client);
      await _db.SaveChangesAsync();

      if (dto.AddressDTO is not null)
      {
        var address = Address.Create(dto.AddressDTO, client.Id);
        _db.Addresses.Add(address);
      }

      var superUserDto = new UserCreateDTO
      {
        FirstName = "SUPERUSER",
        LastName = "SUPERUSER",
        Email = dto.InitUserEmail,
        Password = "SUPERUSER",
        ClientId = client.Id,
        Role = UserRole.SUPERADMIN
      };

      var superUser = User.Create(superUserDto);
      _db.Users.Add(superUser);

      await _db.SaveChangesAsync();
      await transaction.CommitAsync();

      return client.ToResponse();
    }
    catch
    {
      await transaction.RollbackAsync();
      throw;
    }
  }

  public async Task<ClientResponseDTO?> Update(ClientUpdateDTO dto, int id)
  {
    var client = await _db.Clients.FirstOrDefaultAsync(c => c.Id == id);
    if (client is null) return null;

    client.Update(dto);
    await _db.SaveChangesAsync();

    return client.ToResponse();
  }

  public async Task<bool> Delete(int id)
  {
    var client = await _db.Clients.FindAsync(id);
    if (client is null) return false;

    _db.Clients.Remove(client);
    await _db.SaveChangesAsync();

    return true;
  }
}