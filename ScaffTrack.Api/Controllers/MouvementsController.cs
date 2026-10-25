using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScaffTrack.Api.Data;
using ScaffTrack.Shared.DTOs;
using ScaffTrack.Shared.Models;

namespace ScaffTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MouvementsController : ControllerBase
{
    private readonly ScaffTrackDbContext _context;

    public MouvementsController(ScaffTrackDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MouvementStock>>> GetMouvements([FromQuery] Guid? chantierId = null, [FromQuery] TypeMouvement? type = null)
    {
        var query = _context.MouvementsStock
            .Include(m => m.Chantier)
            .Include(m => m.CreePar)
            .Include(m => m.Lignes)
                .ThenInclude(l => l.Materiel)
            .AsQueryable();

        if (chantierId.HasValue)
            query = query.Where(m => m.ChantierId == chantierId.Value);

        if (type.HasValue)
            query = query.Where(m => m.Type == type.Value);

        var list = await query.OrderByDescending(m => m.DateMouvement).ToListAsync();
        return Ok(list);
    }

    [HttpPost]
    public async Task<ActionResult<MouvementStock>> CreateMouvement(MouvementStock mouvement)
    {
        if (mouvement.Type is not TypeMouvement.SortieChantier and not TypeMouvement.RetourStock)
            return BadRequest(new { message = "Type de mouvement non pris en charge." });

        if (mouvement.Lignes is null || mouvement.Lignes.Count == 0)
            return BadRequest(new { message = "Le mouvement doit contenir au moins un article." });

        if (mouvement.Lignes.Any(ligne => ligne.Quantite <= 0))
            return BadRequest(new { message = "Chaque quantité doit être supérieure à zéro." });

        if (mouvement.Type == TypeMouvement.RetourStock
            && mouvement.Lignes.Any(ligne => ligne.QuantiteEndommagee < 0
                || ligne.QuantitePerdue < 0
                || ligne.QuantiteEndommagee + ligne.QuantitePerdue > ligne.Quantite))
            return BadRequest(new { message = "Les quantités endommagées et perdues doivent être comprises dans la quantité restituée." });

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var prefix = mouvement.Type == TypeMouvement.SortieChantier ? "BL" : "BR";
            var count = await _context.MouvementsStock.CountAsync(m => m.Type == mouvement.Type) + 1;
            mouvement.NumeroBon = $"{prefix}-{DateTime.Now.Year}-{count:D3}";
            mouvement.Statut = StatutMouvement.Valide;

            // Mise à jour des stocks
            foreach (var ligne in mouvement.Lignes)
            {
                var materiel = await _context.Materiels.FindAsync(ligne.MaterielId);
                if (materiel == null)
                    return BadRequest(new { message = "Un matériel sélectionné est introuvable." });

                if (mouvement.Type == TypeMouvement.SortieChantier)
                {
                    if (materiel.QuantiteDisponible < ligne.Quantite)
                    {
                        return BadRequest(new { message = $"Stock insuffisant pour {materiel.Nom}. Disponible : {materiel.QuantiteDisponible}" });
                    }

                    materiel.QuantiteDisponible -= ligne.Quantite;
                    materiel.QuantiteEnChantier += ligne.Quantite;
                }
                else if (mouvement.Type == TypeMouvement.RetourStock)
                {
                    if (materiel.QuantiteEnChantier < ligne.Quantite)
                        return BadRequest(new { message = $"Quantité restituée supérieure au stock suivi sur chantier pour {materiel.Nom}." });

                    materiel.QuantiteEnChantier -= ligne.Quantite;
                    
                    var endommage = ligne.QuantiteEndommagee;
                    var perdu = ligne.QuantitePerdue;
                    var bon = Math.Max(0, ligne.Quantite - endommage - perdu);

                    materiel.QuantiteDisponible += bon;
                    materiel.QuantiteEnMaintenance += endommage;
                    materiel.QuantiteTotale -= perdu;
                }
            }

            _context.MouvementsStock.Add(mouvement);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(mouvement);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, new { message = "Erreur lors de l'enregistrement du mouvement : " + ex.Message });
        }
    }

    [HttpPut("{id:guid}/prolonger")]
    public async Task<IActionResult> ProlongerLocation(Guid id, ProlongationLocationDto prolongation)
    {
        var mouvement = await _context.MouvementsStock
            .Include(item => item.Chantier)
            .FirstOrDefaultAsync(item => item.Id == id);
        if (mouvement is null || mouvement.Type != TypeMouvement.SortieChantier)
            return NotFound(new { message = "Location introuvable." });

        if (prolongation.DateRetourPrevue.Date <= mouvement.DateMouvement.Date)
            return BadRequest(new { message = "La nouvelle échéance doit suivre la date de sortie." });

        if (mouvement.DateRetourPrevue is { } dateRetourActuelle
            && prolongation.DateRetourPrevue.Date <= dateRetourActuelle.Date)
            return BadRequest(new { message = "La nouvelle échéance doit être postérieure à l’échéance actuelle." });

        const string marker = "Retour prévu : ";
        var observations = mouvement.Observations ?? string.Empty;
        var markerIndex = observations.LastIndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (markerIndex >= 0)
            observations = observations[..markerIndex].TrimEnd(' ', '•');

        mouvement.Observations = $"{observations} • Retour prévu : {prolongation.DateRetourPrevue:dd/MM/yyyy}".Trim(' ', '•');
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
