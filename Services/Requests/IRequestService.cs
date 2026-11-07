using newproj.DTO.Request;

namespace newproj.Services.Requests;

public interface IRequestService
{
    Task<ICollection<RequestResponseDTO>> FindAll();

    Task<RequestResponseDTO?> FindOne(int id);

    Task<ICollection<RequestResponseDTO>> FindByClientId(int clientId);

    Task<ICollection<RequestResponseDTO>> FindByUserId(int userId);

    Task<RequestCreateResult> Create(RequestCreateDTO dto, int createdByUserId);

    Task<RequestUpdateResult> Update(RequestUpdateDTO dto, int id, int actionUserId);
}