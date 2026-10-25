using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScaffTrack.Api.Data;
using ScaffTrack.Shared.DTOs;
using ScaffTrack.Shared.Models;

namespace ScaffTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly ScaffTrackDbContext _context;

    public DashboardController(ScaffTrackDbContext context)
    {
        _context = context;
    }

    [HttpGet("stats")]
    public async Task<ActionResult<DashboardStatsDto>> GetStats()
    {
        var debutMois = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

        // La date de retour prévue est calculée côté modèle : on filtre les retards en mémoire
        var locationsActives = await _context.MouvementsStock
            .Include(m => m.Chantier)
            .Where(m => m.Type == TypeMouvement.SortieChantier && m.Chantier!.Statut != StatutChantier.Termine)
            .ToListAsync();

        var stats = new DashboardStatsDto
        {
            TotalUnites = await _context.Materiels.SumAsync(m => m.QuantiteTotale),
            UnitesSurChantiers = await _context.Materiels.SumAsync(m => m.QuantiteEnChantier),
            UnitesDisponibles = await _context.Materiels.SumAsync(m => m.QuantiteDisponible),
            UnitesEnMaintenance = await _context.Materiels.SumAsync(m => m.QuantiteEnMaintenance),
            NombreCategories = await _context.Categories.CountAsync(),
            ChantiersActifs = await _context.Chantiers.CountAsync(c => c.Statut == StatutChantier.EnCours),
            ChantiersEnAttente = await _context.Chantiers.CountAsync(c => c.Statut == StatutChantier.EnAttente),
            RetoursEnAttente = locationsActives.Count(m => m.EstEnRetard),
            AlertesNonResolues = await _context.Alertes.CountAsync(a => !a.EstResolue),
            EncoursClientsFCFA = await _context.Clients.SumAsync(c => c.SoldeEncoursFCFA),
            ChiffreAffairesMoisFCFA = await _context.Factures
                .Where(f => f.DateEmission >= debutMois)
                .SumAsync(f => f.MontantLocationFCFA + f.MontantPenalitesFCFA)
        };

        return Ok(stats);
    }
}
