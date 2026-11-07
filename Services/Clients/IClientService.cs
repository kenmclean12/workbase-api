using newproj.DTO.Client;

namespace newproj.Services.Clients;

public interface IClientService
{
    Task<ICollection<ClientResponseDTO>> FindAll();

    Task<ClientResponseDTO?> FindOne(int id);

    Task<ClientResponseDTO?> Create(ClientCreateDTO dto);

    Task<ClientResponseDTO?> Update(ClientUpdateDTO dto, int id);
    
    Task<bool> Delete(int id);
}