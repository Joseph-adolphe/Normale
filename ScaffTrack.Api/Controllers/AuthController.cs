using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScaffTrack.Api.Data;
using ScaffTrack.Shared.DTOs;
using ScaffTrack.Shared.Models;

namespace ScaffTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ScaffTrackDbContext _context;

    public AuthController(ScaffTrackDbContext context)
    {
        _context = context;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto request)
    {
        var user = await _context.Utilisateurs
            .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower() && u.MotDePasseHash == request.MotDePasse);

        if (user == null || !user.EstActif)
        {
            return Unauthorized(new LoginResponseDto
            {
                Succes = false,
                Message = "Email ou mot de passe incorrect."
            });
        }

        return Ok(new LoginResponseDto
        {
            Succes = true,
            Message = "Connexion réussie.",
            Nom = user.NomComplet,
            Role = user.Role.ToString(),
            UtilisateurId = user.Id,
            Token = "demo-jwt-token-scafftrack-" + Guid.NewGuid()
        });
    }

    [HttpPost("register")]
    public async Task<ActionResult<LoginResponseDto>> Register(RegisterRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.MotDePasse))
        {
            return BadRequest(new LoginResponseDto { Succes = false, Message = "Email et mot de passe obligatoires." });
        }

        var existe = await _context.Utilisateurs.AnyAsync(u => u.Email.ToLower() == request.Email.ToLower());
        if (existe)
        {
            return Conflict(new LoginResponseDto { Succes = false, Message = "Cet email est déjà utilisé par un autre compte." });
        }

        var nouvUtilisateur = new Utilisateur
        {
            Id = Guid.NewGuid(),
            Nom = request.Nom,
            Prenom = request.Prenom,
            Email = request.Email,
            MotDePasseHash = request.MotDePasse,
            Telephone = request.Telephone,
            Role = request.Role,
            EstActif = true,
            DateCreation = DateTime.UtcNow
        };

        _context.Utilisateurs.Add(nouvUtilisateur);
        await _context.SaveChangesAsync();

        return Ok(new LoginResponseDto
        {
            Succes = true,
            Message = "Compte créé avec succès.",
            Nom = nouvUtilisateur.NomComplet,
            Role = nouvUtilisateur.Role.ToString(),
            UtilisateurId = nouvUtilisateur.Id,
            Token = "demo-jwt-token-scafftrack-" + Guid.NewGuid()
        });
    }
}
