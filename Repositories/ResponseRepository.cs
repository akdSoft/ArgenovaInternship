using Microsoft.EntityFrameworkCore;
using RaporAsistani.Data;
using RaporAsistani.Models;

namespace RaporAsistani.Repositories;

public class ResponseRepository
{
    private readonly AppDbContext _context;

    public ResponseRepository(AppDbContext context) => _context = context;

    public async Task<Response> SaveResponseAsync(Response response)
    {
        _context.Responses.Add(response);
        await _context.SaveChangesAsync();
        return response;
    }

    public async Task<List<Response>> GetResponseHistoryAsync()
    {
        return await _context.Responses.ToListAsync();
    }
}
