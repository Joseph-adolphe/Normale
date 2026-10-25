using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScaffTrack.Api.Data;

namespace ScaffTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlertesController : ControllerBase
{
    private readonly ScaffTrackDbContext _context;

    public AlertesController(ScaffTrackDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAlertes([FromQuery] bool inclureResolues = false)
    {
        var query = _context.Alertes
            .Include(alerte => alerte.Chantier)
            .Include(alerte => alerte.Materiel)
            .AsQueryable();

        if (!inclureResolues)
            query = query.Where(alerte => !alerte.EstResolue);

        return Ok(await query.OrderByDescending(alerte => alerte.DateCreation).ToListAsync());
    }

    [HttpPut("{id:guid}/resolve")]
    public async Task<IActionResult> ResolveAlerte(Guid id)
    {
        var alerte = await _context.Alertes.FindAsync(id);
        if (alerte is null)
            return NotFound();

        alerte.EstResolue = true;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
