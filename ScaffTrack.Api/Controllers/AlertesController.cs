using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScaffTrack.Api.Data;
using ScaffTrack.Shared.Models;

namespace ScaffTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlertesController : ControllerBase
{
    // Seuil de stock critique : moins de 10 % du parc disponible en dépôt
    private const double SeuilStockCritique = 0.10;

    private readonly ScaffTrackDbContext _context;

    public AlertesController(ScaffTrackDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAlertes([FromQuery] bool inclureResolues = false)
    {
        await GenererAlertesAsync();

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

    /// <summary>
    /// Crée les alertes de retard de restitution et de stock critique qui n'existent pas encore.
    /// Une alerte n'est recréée que si la précédente du même type pour le même objet a été résolue.
    /// </summary>
    private async Task GenererAlertesAsync()
    {
        var alertesOuvertes = await _context.Alertes
            .Where(a => !a.EstResolue)
            .Select(a => new { a.Type, a.ChantierId, a.MaterielId })
            .ToListAsync();

        var locationsEnRetard = (await _context.MouvementsStock
                .Include(m => m.Chantier)
                .Where(m => m.Type == TypeMouvement.SortieChantier && m.Chantier!.Statut != StatutChantier.Termine)
                .ToListAsync())
            .Where(m => m.EstEnRetard)
            .GroupBy(m => m.ChantierId);

        foreach (var groupe in locationsEnRetard)
        {
            if (alertesOuvertes.Any(a => a.Type == TypeAlerte.RetardRetour && a.ChantierId == groupe.Key))
                continue;

            var chantier = groupe.First().Chantier!;
            var echeance = groupe.Min(m => m.DateRetourPrevue)!.Value;
            _context.Alertes.Add(new Alerte
            {
                Type = TypeAlerte.RetardRetour,
                Titre = "Retard restitution chantier",
                Message = $"Le chantier {chantier.Nom} devait restituer le matériel le {echeance:dd/MM/yyyy} ({string.Join(", ", groupe.Select(m => m.NumeroBon))}).",
                ChantierId = chantier.Id
            });
        }

        var materielsCritiques = await _context.Materiels
            .Where(m => m.QuantiteTotale > 0 && m.QuantiteDisponible < m.QuantiteTotale * SeuilStockCritique)
            .ToListAsync();

        foreach (var materiel in materielsCritiques)
        {
            if (alertesOuvertes.Any(a => a.Type == TypeAlerte.StockCritique && a.MaterielId == materiel.Id))
                continue;

            _context.Alertes.Add(new Alerte
            {
                Type = TypeAlerte.StockCritique,
                Titre = $"Stock faible {materiel.Nom}",
                Message = $"Il ne reste que {materiel.QuantiteDisponible} unités disponibles en dépôt sur {materiel.QuantiteTotale}.",
                MaterielId = materiel.Id
            });
        }

        await _context.SaveChangesAsync();
    }
}
