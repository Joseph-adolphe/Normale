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

    public int TotalPiecesSurChantier => Mouvements?
        .Where(m => m.Type == TypeMouvement.SortieChantier && m.Statut == StatutMouvement.Valide)
        .SelectMany(m => m.Lignes)
        .Sum(l => l.Quantite) ?? 0;
}
