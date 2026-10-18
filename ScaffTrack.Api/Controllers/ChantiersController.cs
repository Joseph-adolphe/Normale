using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScaffTrack.Api.Data;
using ScaffTrack.Shared.Models;

namespace ScaffTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChantiersController : ControllerBase
{
    private readonly ScaffTrackDbContext _context;

    public ChantiersController(ScaffTrackDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Chantier>>> GetChantiers([FromQuery] string? search = null, [FromQuery] StatutChantier? statut = null)
    {
        var query = _context.Chantiers
            .Include(c => c.Client)
            .Include(c => c.Responsable)
            .Include(c => c.Mouvements)
                .ThenInclude(m => m.Lignes)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(c => c.Nom.Contains(search) || c.Code.Contains(search) || c.Adresse.Contains(search));
        }

        if (statut.HasValue)
        {
            query = query.Where(c => c.Statut == statut.Value);
        }

        var list = await query.OrderByDescending(c => c.DateDebut).ToListAsync();
        return Ok(list);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Chantier>> GetChantier(Guid id)
    {
        var chantier = await _context.Chantiers
            .Include(c => c.Client)
            .Include(c => c.Responsable)
            .Include(c => c.Mouvements)
                .ThenInclude(m => m.Lignes)
                    .ThenInclude(l => l.Materiel)
            .Include(c => c.Alertes)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (chantier == null)
            return NotFound(new { message = "Chantier introuvable." });

        return Ok(chantier);
    }

    [HttpPost]
    public async Task<ActionResult<Chantier>> CreateChantier(Chantier chantier)
    {
        if (string.IsNullOrWhiteSpace(chantier.Code))
        {
            var count = await _context.Chantiers.CountAsync() + 1;
            chantier.Code = $"CH-{DateTime.Now.Year}-{count:D3}";
        }

        _context.Chantiers.Add(chantier);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetChantier), new { id = chantier.Id }, chantier);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateChantier(Guid id, Chantier chantier)
    {
        if (id != chantier.Id)
            return BadRequest(new { message = "Identifiant non correspondant." });

        _context.Entry(chantier).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Chantiers.AnyAsync(c => c.Id == id))
                return NotFound();
            throw;
        }

        return NoContent();
    }
}
