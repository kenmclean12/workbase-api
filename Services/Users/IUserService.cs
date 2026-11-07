using newproj.DTO.User;

namespace newproj.Services.Users;

public interface IUserService
{
    Task<ICollection<UserResponseDTO>> FindAll();

    Task<UserResponseDTO?> FindOne(int id);

    Task<ICollection<UserResponseDTO>> FindAssignableUsers();

    Task<ICollection<UserResponseDTO>> FindClientUsers(int clientId);

    Task<UserResponseDTO?> Create(UserCreateDTO dto);

    Task<UserUpdateResult> Update(UserUpdateDTO dto, int id);

    Task<bool> Delete(int id);

    Task<ResetPasswordResult> ResetPassword(UserPasswordResetDTO dto, int id);
}