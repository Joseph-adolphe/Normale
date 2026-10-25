using ScaffTrack.Shared.DTOs;
using ScaffTrack.Shared.Models;

namespace ScaffTrack1.Services;

public interface IApiService
{
    Task<LoginResponseDto> LoginAsync(string email, string password);
    Task<LoginResponseDto> RegisterAsync(RegisterRequestDto request);
    Task<DashboardStatsDto> GetDashboardStatsAsync();
    Task<List<Chantier>> GetChantiersAsync(string? search = null);
    Task<Chantier?> GetChantierByIdAsync(Guid id);
    Task<List<Materiel>> GetMaterielsAsync(string? search = null);
    Task<List<Categorie>> GetCategoriesAsync();
    Task<List<Client>> GetClientsAsync();
    Task<bool> CreateClientAsync(Client client);
    Task<List<MouvementStock>> GetMouvementsAsync();
    Task<List<Alerte>> GetAlertesAsync();
    Task<bool> ResolveAlerteAsync(Guid id);
    Task<bool> CreateChantierAsync(Chantier chantier);
    Task<bool> CreateMaterielAsync(Materiel materiel);
    Task<bool> AdjustMaterielStockAsync(Guid materielId, AjustementStockDto adjustment);
    Task<bool> CreateMouvementAsync(MouvementStock mouvement);
    Task<bool> ExtendLocationAsync(Guid mouvementId, DateTime dateRetourPrevue);
}
