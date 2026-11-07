using newproj.DTO.UserInvite;

namespace newproj.Services.UserInvites;

public interface IUserInviteService
{
    Task<ICollection<UserInviteResponseDTO>> FindPending();

    Task<ICollection<UserInviteResponseDTO>> FindPendingByClientId(int clientId);

    Task<bool> Create(UserInviteBulkCreateDTO dto, int invitedByUserId);

    Task<bool> Delete(int id);
}