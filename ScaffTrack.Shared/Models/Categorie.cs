namespace ScaffTrack.Shared.Models;

public class Categorie
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nom { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icone { get; set; } = "scaffold_icon.png";
    public List<Materiel> Materiels { get; set; } = new();
}
