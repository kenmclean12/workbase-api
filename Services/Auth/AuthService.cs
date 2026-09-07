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

public static class AuthService
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/auth");

        group.MapPost("/login", async (AppDbContext db, LoginDTO dto) =>
        {
            User? user = await db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user is null)
            {
                return Results.Unauthorized();
            }

            if (!user.VerifyPassword(dto.Password))
            {
                return Results.Unauthorized();
            }

            RefreshTokenResult refreshTokenResult = AuthHelpers.CreateRefreshToken();
            RefreshTokenCreateDTO refreshTokenCreateDTO = new()
            {
                UserId = user.Id,
                TokenHash = refreshTokenResult.TokenHash,
                ExpiresAt = DateTime.UtcNow.AddMonths(1)
            };

            RefreshToken refreshToken = RefreshToken.Create(refreshTokenCreateDTO);
            db.RefreshTokens.Add(refreshToken);
            await db.SaveChangesAsync();

            string accessToken = AuthHelpers.CreateToken(app.Configuration, user);

            TokenResponseDTO responseDTO = new() { Token = accessToken, RefreshToken = refreshTokenResult.Token };
            return Results.Ok(responseDTO);
        });

        group.MapPost("/register", async (AppDbContext db, RegisterDTO dto) =>
        {
            byte[] tokenBytes;
            try
            {
                tokenBytes = Convert.FromBase64String(dto.InviteToken);
            }
            catch (FormatException)
            {
                return Results.Unauthorized();
            }

            string tokenHash = Convert.ToBase64String(SHA256.HashData(tokenBytes));

            UserInvite? invite = await db.UserInvites.FirstOrDefaultAsync(ui =>
                    ui.TokenHash == tokenHash &&
                    ui.Email == dto.Email &&
                    ui.ExpiresAt > DateTime.UtcNow &&
                    ui.AcceptedAt == null
                );

            if (invite is null)
            {
                return Results.Unauthorized();
            }

            await using var transaction = await db.Database.BeginTransactionAsync();

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
                db.Users.Add(newUser);

                await db.SaveChangesAsync();
                await transaction.CommitAsync();

                return Results.Ok(newUser.ToResponse());
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });

        group.MapPost("/refresh-token", async (AppDbContext db, RefreshTokenDTO dto) =>
        {
            byte[] tokenBytes;
            try
            {
                tokenBytes = Convert.FromBase64String(dto.RefreshToken);
            }
            catch (FormatException)
            {
                return Results.Unauthorized();
            }

            string tokenHash = Convert.ToBase64String(SHA256.HashData(tokenBytes));

            RefreshToken? refreshToken = await db.RefreshTokens.FirstOrDefaultAsync(rt =>
              rt.ExpiresAt > DateTime.UtcNow &&
              rt.RevokedAt == null &&
              rt.TokenHash == tokenHash
            );

            if (refreshToken is null)
            {
                return Results.Unauthorized();
            }

            User? user = await db.Users.FindAsync(refreshToken.UserId);
            if (user is null)
            {
                return Results.Unauthorized();
            }

            await using var transaction = await db.Database.BeginTransactionAsync();
            try
            {
                string accessToken = AuthHelpers.CreateToken(app.Configuration, user);
                refreshToken.RevokedAt = DateTime.UtcNow;

                RefreshTokenResult newRefreshTokenResult = AuthHelpers.CreateRefreshToken();
                RefreshTokenCreateDTO newRefreshTokenCreateDTO = new()
                {
                    UserId = user.Id,
                    TokenHash = newRefreshTokenResult.TokenHash,
                    ExpiresAt = DateTime.UtcNow.AddMonths(1)
                };

                RefreshToken newRefreshToken = RefreshToken.Create(newRefreshTokenCreateDTO);
                db.RefreshTokens.Add(newRefreshToken);

                await db.SaveChangesAsync();
                await transaction.CommitAsync();

                TokenResponseDTO responseDTO = new()
                {
                    Token = accessToken,
                    RefreshToken = newRefreshTokenResult.Token,
                };

                return Results.Ok(responseDTO);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });

        group.MapPost("/logout", async (AppDbContext db, RefreshTokenDTO dto) =>
        {
            byte[] tokenBytes;
            try
            {
                tokenBytes = Convert.FromBase64String(dto.RefreshToken);
            }
            catch (FormatException)
            {
                return Results.Unauthorized();
            }

            string tokenHash = Convert.ToBase64String(SHA256.HashData(tokenBytes));

            RefreshToken? refreshToken = await db.RefreshTokens.FirstOrDefaultAsync(rt =>
                rt.TokenHash == tokenHash &&
                rt.RevokedAt == null
            );

            if (refreshToken is null)
            {
                return Results.NoContent();
            }

            refreshToken.RevokedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();

            return Results.NoContent();
        });
    }
}