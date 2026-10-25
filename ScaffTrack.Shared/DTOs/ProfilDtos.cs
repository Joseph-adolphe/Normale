using ScaffTrack.Shared.Models;

namespace ScaffTrack.Shared.DTOs;

public class ProfilUtilisateurDto
{
    public Guid Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telephone { get; set; } = string.Empty;
    public UserRole Role { get; set; }
}

public class UpdateProfilDto
{
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Telephone { get; set; } = string.Empty;

    // Renseignés uniquement pour changer le mot de passe
    public string? MotDePasseActuel { get; set; }
    public string? NouveauMotDePasse { get; set; }
}
