using newproj.DTO.RequestHistory;

namespace newproj.Services.RequestHistoryRecords;

public interface IRequestHistoryService
{
    Task<ICollection<RequestHistoryResponseDTO>> FindByRequestId(int requestId);

    Task<ICollection<RequestHistoryResponseDTO>> FindByUserId(int userId);
}