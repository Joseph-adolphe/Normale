using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScaffTrack.Api.Data;
using ScaffTrack.Shared.Models;

namespace ScaffTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MaterielsController : ControllerBase
{
    private readonly ScaffTrackDbContext _context;

    public MaterielsController(ScaffTrackDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Materiel>>> GetMateriels([FromQuery] string? search = null, [FromQuery] Guid? categorieId = null)
    {
        var query = _context.Materiels
            .Include(m => m.Categorie)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(m => m.Nom.Contains(search) || m.Reference.Contains(search));
        }

        if (categorieId.HasValue)
        {
            query = query.Where(m => m.CategorieId == categorieId.Value);
        }

        var list = await query.OrderBy(m => m.Nom).ToListAsync();
        return Ok(list);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Materiel>> GetMateriel(Guid id)
    {
        var materiel = await _context.Materiels
            .Include(m => m.Categorie)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (materiel == null)
            return NotFound(new { message = "Matériel introuvable." });

        return Ok(materiel);
    }

    [HttpGet("categories")]
    public async Task<ActionResult<IEnumerable<Categorie>>> GetCategories()
    {
        var categories = await _context.Categories.ToListAsync();
        return Ok(categories);
    }

    [HttpPost]
    public async Task<ActionResult<Materiel>> CreateMateriel(Materiel materiel)
    {
        materiel.QuantiteDisponible = materiel.QuantiteTotale;
        materiel.QuantiteEnChantier = 0;
        materiel.QuantiteEnMaintenance = 0;

        _context.Materiels.Add(materiel);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMateriel), new { id = materiel.Id }, materiel);
    }
}
