using Microsoft.EntityFrameworkCore;
using newproj.Data;
using newproj.DTO.User;
using newproj.Helpers;
using newproj.Models.Users;

namespace newproj.Services.Users;

public class UserService(AppDbContext db) : IUserService
{
    private readonly AppDbContext _db = db;

    public async Task<ICollection<UserResponseDTO>> FindAll()
    {
        ICollection<User> users = await _db.Users
            .Where(u => u.ClientId == null)
            .ToArrayAsync();

        return [.. users.Select(u => u.ToResponse())];
    }

    public async Task<UserResponseDTO?> FindOne(int id)
    {
        User? user = await _db.Users.FindAsync(id);
        if (user is null) return null;
        return user.ToResponse();
    }

    public async Task<ICollection<UserResponseDTO>> FindAssignableUsers()
    {
        ICollection<User> users = await _db.Users
            .Where(u => u.ClientId == null)
            .Where(u => u.Role != UserRole.READONLY)
            .OrderByDescending(u => u.Role)
            .ToArrayAsync();

        return [.. users.Select(u => u.ToResponse())];
    }

    public async Task<ICollection<UserResponseDTO>> FindClientUsers(int clientId)
    {
        ICollection<User> users = await _db.Users
            .Where(u => u.ClientId == clientId)
            .OrderByDescending(u => u.Role)
            .ToArrayAsync();

        return [.. users.Select(u => u.ToResponse())];
    }

    public async Task<UserResponseDTO?> Create(UserCreateDTO dto)
    {
        User? existingEmailUser = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (existingEmailUser is not null) return null;

        User user = User.Create(dto);
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return user.ToResponse();
    }

    public async Task<UserUpdateResult> Update(UserUpdateDTO dto, int actorId, UserRole actorRole, int id)
    {
        User? existingUser = await _db.Users.FindAsync(id);
        if (existingUser is null) return UserUpdateResult.UserNotFound;

        if (dto.Email is not null)
        {
            User? duplicateEmailUser = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email && u.Id != id);
            if (duplicateEmailUser is not null) return UserUpdateResult.DuplicateEmail;
        }

        if (!RoleAuthorization.CanManageUser(actorRole, existingUser.Role) && actorId != id) return UserUpdateResult.CannotManageRole;

        if (dto.Role is UserRole role)
        {
            bool canAssign = RoleAuthorization.CanAssignRole(actorRole, role);
            if (!canAssign || actorId == id) return UserUpdateResult.CannotAssignRole;
        }

        existingUser.Update(dto);
        await _db.SaveChangesAsync();

        return UserUpdateResult.Success;
    }

    public async Task<bool> Delete(int id)
    {
        User? existingUser = await _db.Users.FindAsync(id);
        if (existingUser is null) return false;

        _db.Users.Remove(existingUser);
        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<ResetPasswordResult> ResetPassword(UserPasswordResetDTO dto, int id)
    {
        User? existingUser = await _db.Users.FindAsync(id);
        if (existingUser is null) return ResetPasswordResult.UserNotFound;

        if (!existingUser.ResetPassword(dto))
        {
            return ResetPasswordResult.WrongPassword;
        }

        await _db.SaveChangesAsync();
        
        return ResetPasswordResult.Success;
    }
}