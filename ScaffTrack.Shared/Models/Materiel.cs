namespace ScaffTrack.Shared.Models;

public class Materiel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Reference { get; set; } = string.Empty; // Ex: CAD-H-2-100
    public string Nom { get; set; } = string.Empty;       // Ex: Cadre H 2.0m x 1.0m
    public string Description { get; set; } = string.Empty;
    public Guid CategorieId { get; set; }
    public Categorie? Categorie { get; set; }

    public int QuantiteTotale { get; set; }
    public int QuantiteDisponible { get; set; }
    public int QuantiteEnChantier { get; set; }
    public int QuantiteEnMaintenance { get; set; }

    public decimal PrixLocationJourFCFA { get; set; }
    public decimal PrixRemplacementFCFA { get; set; }

    public string Dimensions { get; set; } = string.Empty; // Ex: 200cm x 100cm
    public double PoidsKg { get; set; }
    public string PhotoUrl { get; set; } = string.Empty;
    public string EmplacementDepot { get; set; } = "Dépôt Principal";
    public EtatMateriel Etat { get; set; } = EtatMateriel.Bon;

    public DateTime DateAjout { get; set; } = DateTime.UtcNow;
}
