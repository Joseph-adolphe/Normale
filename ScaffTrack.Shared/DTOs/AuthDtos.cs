using ScaffTrack.Shared.Models;

namespace ScaffTrack.Shared.DTOs;

public class LoginRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string MotDePasse { get; set; } = string.Empty;
}

public class RegisterRequestDto
{
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string MotDePasse { get; set; } = string.Empty;
    public string Telephone { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.ChefChantier;
}

public class LoginResponseDto
{
    public bool Succes { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public Guid UtilisateurId { get; set; }
}
