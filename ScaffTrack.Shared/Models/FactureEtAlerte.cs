namespace ScaffTrack.Shared.Models;

public class Facture
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string NumeroFacture { get; set; } = string.Empty; // Ex: FAC-2026-042

    public Guid ChantierId { get; set; }
    public Chantier? Chantier { get; set; }

    public Guid ClientId { get; set; }
    public Client? Client { get; set; }

    public decimal MontantLocationFCFA { get; set; }
    public decimal MontantPenalitesFCFA { get; set; }
    public decimal MontantTotalFCFA => MontantLocationFCFA + MontantPenalitesFCFA;

    public decimal MontantPayeFCFA { get; set; }
    public decimal ResteAPayerFCFA => MontantTotalFCFA - MontantPayeFCFA;

    public StatutFacture Statut { get; set; } = StatutFacture.Emise;
    public DateTime DateEmission { get; set; } = DateTime.UtcNow;
    public DateTime DateEcheance { get; set; } = DateTime.UtcNow.AddDays(15);
}

public class Alerte
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public TypeAlerte Type { get; set; } = TypeAlerte.RetardRetour;
    public string Titre { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool EstResolue { get; set; } = false;
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;

    public Guid? ChantierId { get; set; }
    public Chantier? Chantier { get; set; }

    public Guid? MaterielId { get; set; }
    public Materiel? Materiel { get; set; }
}
