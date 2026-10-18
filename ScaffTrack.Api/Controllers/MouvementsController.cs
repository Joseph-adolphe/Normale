using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScaffTrack.Api.Data;
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
                if (materiel == null) continue;

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
                    materiel.QuantiteEnChantier = Math.Max(0, materiel.QuantiteEnChantier - ligne.Quantite);
                    
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
}
