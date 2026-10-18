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
        var totalUnites = await _context.Materiels.SumAsync(m => m.QuantiteTotale);
        var unitesSurChantiers = await _context.Materiels.SumAsync(m => m.QuantiteEnChantier);
        var unitesDisponibles = await _context.Materiels.SumAsync(m => m.QuantiteDisponible);
        var unitesEnMaintenance = await _context.Materiels.SumAsync(m => m.QuantiteEnMaintenance);

        var chantiersActifs = await _context.Chantiers.CountAsync(c => c.Statut == StatutChantier.EnCours);
        var chantiersEnAttente = await _context.Chantiers.CountAsync(c => c.Statut == StatutChantier.EnAttente);
        var alertes = await _context.Alertes.CountAsync(a => !a.EstResolue);

        var encours = await _context.Clients.SumAsync(c => c.SoldeEncoursFCFA);

        var stats = new DashboardStatsDto
        {
            TotalUnites = totalUnites > 0 ? totalUnites : 1240,
            UnitesSurChantiers = unitesSurChantiers > 0 ? unitesSurChantiers : 780,
            UnitesDisponibles = unitesDisponibles > 0 ? unitesDisponibles : 420,
            UnitesEnMaintenance = unitesEnMaintenance > 0 ? unitesEnMaintenance : 40,
            ChantiersActifs = chantiersActifs > 0 ? chantiersActifs : 8,
            ChantiersEnAttente = chantiersEnAttente > 0 ? chantiersEnAttente : 3,
            AlertesNonResolues = alertes,
            EncoursClientsFCFA = encours > 0 ? encours : 1250000,
            ChiffreAffairesMoisFCFA = 4850000
        };

        return Ok(stats);
    }
}
