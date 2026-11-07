using newproj.DTO.RequestHistory;

namespace newproj.Services.RequestHistoryRecords;

public interface IRequestHistoryService
{
    Task<ICollection<RequestHistoryResponseDTO>> GetByRequestId(int requestId);

    Task<ICollection<RequestHistoryResponseDTO>> GetByUserId(int userId);
}