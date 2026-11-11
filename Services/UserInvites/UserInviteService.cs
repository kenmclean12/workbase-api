using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using newproj.Data;
using newproj.DTO.UserInvite;
using newproj.Models.UserInvites;
using newproj.Services.Emails;

namespace newproj.Services.UserInvites;

public class UserInviteService(AppDbContext db, IEmailService emailService, IWebHostEnvironment environment, IConfiguration configuration) : IUserInviteService
{
    private readonly AppDbContext _db = db;
    private readonly IEmailService _emailService = emailService;
    private readonly IWebHostEnvironment _environment = environment;
    private readonly IConfiguration _configuration = configuration;

    public async Task<ICollection<UserInviteResponseDTO>> FindPending()
    {
        ICollection<UserInvite> pendingInvites = await _db.UserInvites
                .Where(ui => ui.ClientId == null && ui.AcceptedAt == null)
                .Include(ui => ui.InvitedByUser)
                .ToArrayAsync();

        return [.. pendingInvites.Select(ui => ui.ToResponse())];
    }

    public async Task<ICollection<UserInviteResponseDTO>> FindPendingByClientId(int clientId)
    {
        ICollection<UserInvite> pendingInvites = await _db.UserInvites
                .Where(ui => ui.ClientId == clientId && ui.AcceptedAt == null)
                .Include(ui => ui.Client)
                .Include(ui => ui.InvitedByUser)
                .ToArrayAsync();

        return [.. pendingInvites.Select(ui => ui.ToResponse()).ToArray()];
    }

    public async Task<bool> Create(UserInviteBulkCreateDTO dto, int invitedByUserId)
    {
        string templatePath = Path.Combine(_environment.ContentRootPath, "Templates", "Emails", "UserInvite.html");
        string htmlTemplate = await File.ReadAllTextAsync(templatePath);
        string frontendUrl = _configuration["Frontend:URL"] ?? throw new InvalidOperationException("Frontend:URL is not configured");

        List<(string email, string body)> pendingEmails = new();
        for (int i = 0; i < dto.Emails.Length; i++)
        {
            byte[] tokenBytes = RandomNumberGenerator.GetBytes(32);
            string token = Convert.ToBase64String(tokenBytes);
            string tokenHash = Convert.ToBase64String(SHA256.HashData(tokenBytes));

            var createDTO = new UserInviteCreateDTO
            {
                Email = dto.Emails[i],
                Role = dto.Role,
                ClientId = dto.ClientId,
                TokenHash = tokenHash,
                InvitedByUserId = invitedByUserId,
            };

            var invite = UserInvite.Create(createDTO);
            _db.UserInvites.Add(invite);

            string inviteUrl = $"{frontendUrl}/register?token={Uri.EscapeDataString(token)}";
            string htmlBody = htmlTemplate.Replace("{{InviteUrl}}", inviteUrl);
            pendingEmails.Add((dto.Emails[i], htmlBody));
        }

        await _db.SaveChangesAsync();

        foreach (var (email, body) in pendingEmails)
        {
            await _emailService.SendAsync(email, "Workbase Invite", body);
        }

        return true;
    }

    public async Task<bool> Delete(int id)
    {
        var invite = await _db.UserInvites.FindAsync(id);
        if (invite is null) return false;

        _db.UserInvites.Remove(invite);
        await _db.SaveChangesAsync();

        return true;
    }
}