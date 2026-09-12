using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using newproj.Models.Users;

namespace newproj.Helpers.Auth;

public static class AuthHelpers
{
    public static string CreateToken(IConfiguration config, User user)
    {
        Claim[] claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

        SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(config["Auth:Key"]!));

        SigningCredentials credentials = new(
            securityKey,
            SecurityAlgorithms.HmacSha256
        );

        JwtSecurityToken token = new(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static RefreshTokenResult CreateRefreshToken()
    {
        byte[] tokenBytes = RandomNumberGenerator.GetBytes(32);
        string token = Convert.ToBase64String(tokenBytes);
        string tokenHash = Convert.ToBase64String(SHA256.HashData(tokenBytes));

        return new RefreshTokenResult(Token: token, TokenHash: tokenHash);
    }
}