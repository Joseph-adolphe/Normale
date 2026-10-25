namespace ScaffTrack1.Services;

/// <summary>
/// Service Singleton qui conserve la session de l'utilisateur connecté.
/// Toutes les pages protégées vérifient IsAuthenticated avant d'autoriser l'accès.
/// </summary>
public class AuthStateService
{
    // ─── Session en mémoire ───────────────────────────────────────────
    public bool IsAuthenticated { get; private set; }
    public string? NomUtilisateur { get; private set; }
    public string? Role { get; private set; }
    public string? Email { get; private set; }
    public Guid? UtilisateurId { get; private set; }

    /// <summary>Initiales affichées dans l'avatar (ex. « JB »).</summary>
    public string Initiales => string.Concat((NomUtilisateur ?? string.Empty)
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Where(mot => char.IsLetter(mot[0]))
            .Take(2)
            .Select(mot => char.ToUpperInvariant(mot[0])));

    // ─── Événement pour notifier les pages d'un changement de session ─
    public event Action? SessionChanged;

    // ─────────────────────────────────────────────────────────────────
    /// <summary>Appeler après un login réussi.</summary>
    public void SetSession(string nom, string role, string email, Guid? id = null)
    {
        NomUtilisateur = nom;
        Role = role;
        Email = email;
        UtilisateurId = id;
        IsAuthenticated = true;
        SessionChanged?.Invoke();
    }

    /// <summary>Appeler après la modification du profil.</summary>
    public void UpdateNom(string nom)
    {
        NomUtilisateur = nom;
        SessionChanged?.Invoke();
    }

    /// <summary>Appeler lors de la déconnexion.</summary>
    public void ClearSession()
    {
        NomUtilisateur = null;
        Role = null;
        Email = null;
        UtilisateurId = null;
        IsAuthenticated = false;
        SessionChanged?.Invoke();
    }
}
