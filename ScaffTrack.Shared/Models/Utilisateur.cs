namespace ScaffTrack.Shared.Models;

public class Utilisateur
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string MotDePasseHash { get; set; } = string.Empty;
    public string Telephone { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Magasinier;
    public bool EstActif { get; set; } = true;
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;

    public string NomComplet => $"{Prenom} {Nom}".Trim();
}
