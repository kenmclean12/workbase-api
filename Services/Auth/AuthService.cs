using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using newproj.Data;
using newproj.DTO.Auth;
using newproj.DTO.User;
using newproj.Helpers.Auth;
using newproj.Models.Auth;
using newproj.Models.UserInvites;
using newproj.Models.Users;

namespace newproj.Services.Auth;
public class AuthService(AppDbContext db, IConfiguration configuration) : IAuthService
{
    private readonly AppDbContext _db = db;
    private readonly IConfiguration _config = configuration;

    public async Task<LoginResult> Login(LoginDTO dto)
    {
        User? user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user is null) return new LoginResult(LoginFailure.UserNotFound, null);
        if (!user.VerifyPassword(dto.Password)) return new LoginResult(LoginFailure.WrongPassword, null);

        RefreshTokenResult refreshTokenResult = AuthHelpers.CreateRefreshToken();
        RefreshTokenCreateDTO refreshTokenCreateDTO = new()
        {
            UserId = user.Id,
            TokenHash = refreshTokenResult.TokenHash,
            ExpiresAt = DateTime.UtcNow.AddMonths(1)
        };

        RefreshToken refreshToken = RefreshToken.Create(refreshTokenCreateDTO);
        _db.RefreshTokens.Add(refreshToken);
        await _db.SaveChangesAsync();

        string accessToken = AuthHelpers.CreateToken(_config, user);

        TokenResponseDTO responseDTO = new(accessToken, refreshTokenResult.Token);
        return new LoginResult(null, responseDTO);
    }

    public async Task<RegisterResult> Register(RegisterDTO dto)
    {
        byte[] tokenBytes;
        try
        {
            tokenBytes = Convert.FromBase64String(dto.InviteToken);
        }
        catch (FormatException)
        {
            return new RegisterResult(RegisterFailure.InvalidToken, null);
        }

        string tokenHash = Convert.ToBase64String(SHA256.HashData(tokenBytes));

        UserInvite? invite = await _db.UserInvites.FirstOrDefaultAsync(ui =>
                ui.TokenHash == tokenHash &&
                ui.Email == dto.Email &&
                ui.ExpiresAt > DateTime.UtcNow &&
                ui.AcceptedAt == null
            );

        if (invite is null)
        {
            return new RegisterResult(RegisterFailure.InviteNotFound, null);
        }

        await using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            invite.AcceptedAt = DateTime.UtcNow;

            UserCreateDTO userCreateDTO = new()
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = invite.Email,
                Role = invite.Role,
                ClientId = invite.ClientId,
                Password = dto.Password,
            };

            User newUser = User.Create(userCreateDTO);
            _db.Users.Add(newUser);

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            return new RegisterResult(null, newUser.ToResponse());
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<RefreshAccessTokenResult> RefreshAccessToken(RefreshTokenDTO dto)
    {
        byte[] tokenBytes;
        try
        {
            tokenBytes = Convert.FromBase64String(dto.RefreshToken);
        }
        catch (FormatException)
        {
            return new RefreshAccessTokenResult(RefreshAccessTokenFailure.InvalidToken, null);
        }

        string tokenHash = Convert.ToBase64String(SHA256.HashData(tokenBytes));

        RefreshToken? refreshToken = await _db.RefreshTokens.FirstOrDefaultAsync(rt =>
          rt.ExpiresAt > DateTime.UtcNow &&
          rt.RevokedAt == null &&
          rt.TokenHash == tokenHash
        );

        if (refreshToken is null)
        {
            return new RefreshAccessTokenResult(RefreshAccessTokenFailure.RefreshTokenNotFound, null);
        }

        User? user = await _db.Users.FindAsync(refreshToken.UserId);
        if (user is null)
        {
            return new RefreshAccessTokenResult(RefreshAccessTokenFailure.UserNotFound, null);
        }

        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            string accessToken = AuthHelpers.CreateToken(_config, user);
            refreshToken.RevokedAt = DateTime.UtcNow;

            RefreshTokenResult newRefreshTokenResult = AuthHelpers.CreateRefreshToken();
            RefreshTokenCreateDTO newRefreshTokenCreateDTO = new()
            {
                UserId = user.Id,
                TokenHash = newRefreshTokenResult.TokenHash,
                ExpiresAt = DateTime.UtcNow.AddMonths(1)
            };

            RefreshToken newRefreshToken = RefreshToken.Create(newRefreshTokenCreateDTO);
            _db.RefreshTokens.Add(newRefreshToken);

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            TokenResponseDTO responseDTO = new(accessToken, newRefreshTokenResult.Token);
            return new RefreshAccessTokenResult(null, responseDTO);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<LogoutResult> Logout(RefreshTokenDTO dto)
    {
        byte[] tokenBytes;
        try
        {
            tokenBytes = Convert.FromBase64String(dto.RefreshToken);
        }
        catch (FormatException)
        {
            return LogoutResult.InvalidToken;
        }

        string tokenHash = Convert.ToBase64String(SHA256.HashData(tokenBytes));

        RefreshToken? refreshToken = await _db.RefreshTokens.FirstOrDefaultAsync(rt =>
            rt.TokenHash == tokenHash &&
            rt.RevokedAt == null
        );

        if (refreshToken is null)
        {
            return LogoutResult.RefreshTokenNotFound;
        }

        refreshToken.RevokedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return LogoutResult.Success;
    }
}