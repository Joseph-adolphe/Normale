namespace ScaffTrack.Shared.Models;

public class Client
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string NomEntreprise { get; set; } = string.Empty;
    public string ContactNom { get; set; } = string.Empty;
    public string Telephone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Adresse { get; set; } = string.Empty;
    public string Ville { get; set; } = string.Empty;
    public decimal SoldeEncoursFCFA { get; set; } = 0;
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;

    public List<Chantier> Chantiers { get; set; } = new();
    public List<Facture> Factures { get; set; } = new();
}
