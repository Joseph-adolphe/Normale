namespace ScaffTrack.Shared.Models;

public class Chantier
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; } = string.Empty; // Ex: CH-2026-004
    public string Nom { get; set; } = string.Empty;  // Ex: Résidence Les Palmiers
    public string Adresse { get; set; } = string.Empty;
    public string Ville { get; set; } = string.Empty;

    public Guid ClientId { get; set; }
    public Client? Client { get; set; }

    public Guid? ResponsableId { get; set; }
    public Utilisateur? Responsable { get; set; }

    public DateTime DateDebut { get; set; } = DateTime.Today;
    public DateTime DateFinPrevue { get; set; } = DateTime.Today.AddDays(30);
    public DateTime? DateFinReelle { get; set; }

    public StatutChantier Statut { get; set; } = StatutChantier.EnAttente;

    public string PhotoUrl { get; set; } = string.Empty;
    public string Remarques { get; set; } = string.Empty;

    public decimal MontantLocationPrevueFCFA { get; set; }
    public decimal MontantCautionFCFA { get; set; }

    public List<MouvementStock> Mouvements { get; set; } = new();
    public List<Facture> Factures { get; set; } = new();
    public List<Alerte> Alertes { get; set; } = new();

    // Unités sorties vers le chantier moins celles déjà restituées
    public int TotalPiecesSurChantier => Mouvements?
        .Where(m => m.Statut == StatutMouvement.Valide)
        .Sum(m => m.Type switch
        {
            TypeMouvement.SortieChantier => m.TotalArticles,
            TypeMouvement.RetourStock => -m.TotalArticles,
            _ => 0
        }) ?? 0;
}
