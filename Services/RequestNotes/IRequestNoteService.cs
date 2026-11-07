using newproj.DTO.RequestNote;

namespace newproj.Services.RequestNotes;

public interface IRequestNoteService
{
    Task<ICollection<RequestNoteResponseDTO>> FindByRequestId(int requestId);

    Task<RequestNoteCreateResult> Create(RequestNoteCreateDTO dto, int createdByUserId);

    Task<RequestNoteMutationResult> Update(RequestNoteUpdateDTO dto, int id, int userId);

    Task<RequestNoteMutationResult> Delete(int id, int userId);
}