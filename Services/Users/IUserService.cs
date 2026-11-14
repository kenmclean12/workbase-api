using newproj.DTO.User;
using newproj.Models.Users;

namespace newproj.Services.Users;

public interface IUserService
{
    Task<ICollection<UserResponseDTO>> FindAll();

    Task<UserResponseDTO?> FindOne(int id);

    Task<ICollection<UserResponseDTO>> FindAssignableUsers();

    Task<ICollection<UserResponseDTO>> FindClientUsers(int clientId);

    Task<UserResponseDTO?> Create(UserCreateDTO dto);

    Task<UserUpdateResult> Update(UserUpdateDTO dto, int actorId, UserRole actorRole, int id);

    Task<bool> Delete(int id);

    Task<ResetPasswordResult> ResetPassword(UserPasswordResetDTO dto, int id);
}