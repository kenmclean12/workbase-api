using Microsoft.EntityFrameworkCore;
using newproj.Data;
using newproj.DTO.RequestNote;
using newproj.Models.Requests;
using newproj.Models.Users;

namespace newproj.Services.RequestNotes;

public class RequestNoteService(AppDbContext db) : IRequestNoteService
{
    private readonly AppDbContext _db = db;

    public async Task<ICollection<RequestNoteResponseDTO>> FindByRequestId(int requestId)
    {
        ICollection<RequestNote> notes = await _db.RequestNotes
                .Where(n => n.RequestId == requestId)
                .Include(n => n.CreatedByUser)
                .OrderByDescending(n => n.CreatedAt)
                .ThenByDescending(n => n.UpdatedAt)
                .ToArrayAsync();

        return [.. notes.Select(n => n.ToResponse())];
    }

    public async Task<RequestNoteCreateResult> Create(RequestNoteCreateDTO dto, int createdByUserId)
    {
        Request? request = await _db.Requests.FindAsync(dto.RequestId);
        if (request is null)
        {
            return RequestNoteCreateResult.RequestNotFound;
        }

        User? user = await _db.Users.FindAsync(createdByUserId);
        if (user is null)
        {
            return RequestNoteCreateResult.UserNotFound;
        }

        var note = RequestNote.Create(dto, createdByUserId);
        _db.RequestNotes.Add(note);
        await _db.SaveChangesAsync();

        return RequestNoteCreateResult.Success;
    }

    public async Task<RequestNoteMutationResult> Update(RequestNoteUpdateDTO dto, int id, int userId)
    {
        RequestNote? note = await _db.RequestNotes.FindAsync(id);
        if (note is null) return RequestNoteMutationResult.NoteNotFound;

        if (userId != note.CreatedByUserId)
        {
            return RequestNoteMutationResult.UnauthorizedUser;
        }

        note.Update(dto);
        await _db.SaveChangesAsync();

        return RequestNoteMutationResult.Success;
    }

    public async Task<RequestNoteMutationResult> Delete(int id, int userId)
    {
        var note = await _db.RequestNotes.FindAsync(id);
        if (note is null) return RequestNoteMutationResult.NoteNotFound;

        if (userId != note.CreatedByUserId)
        {
            return RequestNoteMutationResult.UnauthorizedUser;
        }

        _db.RequestNotes.Remove(note);
        await _db.SaveChangesAsync();

        return RequestNoteMutationResult.Success;
    }
}