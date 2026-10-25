namespace ScaffTrack.Shared.Models;

public class MouvementStock
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string NumeroBon { get; set; } = string.Empty; // Ex: BL-2026-001 ou BR-2026-001
    public TypeMouvement Type { get; set; } = TypeMouvement.SortieChantier;
    public StatutMouvement Statut { get; set; } = StatutMouvement.Brouillon;

    public Guid ChantierId { get; set; }
    public Chantier? Chantier { get; set; }

    public Guid CreeParId { get; set; }
    public Utilisateur? CreePar { get; set; }

    public DateTime DateMouvement { get; set; } = DateTime.UtcNow;
    public string NomRecepteur { get; set; } = string.Empty;
    public string TelephoneRecepteur { get; set; } = string.Empty;
    public string? SignatureBase64 { get; set; }

    public string Observations { get; set; } = string.Empty;

    public List<LigneMouvement> Lignes { get; set; } = new();

    public int TotalArticles => Lignes.Sum(l => l.Quantite);
    public bool EstLocation => Type == TypeMouvement.SortieChantier;
    public DateTime? DateRetourPrevue
    {
        get
        {
            const string marker = "Retour prévu : ";
            var observations = Observations ?? string.Empty;
            var start = observations.LastIndexOf(marker, StringComparison.OrdinalIgnoreCase);
            if (start < 0)
                return Chantier?.DateFinPrevue;

            var dateText = observations[(start + marker.Length)..].Trim();
            return DateTime.TryParseExact(dateText, "dd/MM/yyyy", System.Globalization.CultureInfo.GetCultureInfo("fr-FR"), System.Globalization.DateTimeStyles.None, out var date)
                ? date
                : Chantier?.DateFinPrevue;
        }
    }

    public bool EstEnRetard => DateRetourPrevue is { } dateRetour && dateRetour.Date < DateTime.Today;
}

public class LigneMouvement
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid MouvementStockId { get; set; }
    public MouvementStock? MouvementStock { get; set; }

    public Guid MaterielId { get; set; }
    public Materiel? Materiel { get; set; }

    public int Quantite { get; set; }
    public EtatMateriel EtatAuMouvement { get; set; } = EtatMateriel.Bon;

    public int QuantiteEndommagee { get; set; } = 0;
    public int QuantitePerdue { get; set; } = 0;
    public decimal PenaliteFCFA { get; set; } = 0;
}
