using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScaffTrack.Shared.DTOs;
using ScaffTrack.Shared.Models;
using ScaffTrack1.Services;
using ScaffTrack1.ViewModels.Base;
using ScaffTrack1.Views.Alertes;
using ScaffTrack1.Views.Historique;
using ScaffTrack1.Views.Mouvements;

namespace ScaffTrack1.ViewModels.Dashboard;

public partial class DashboardViewModel : BaseViewModel
{
    private readonly IApiService _apiService;
    private readonly AuthStateService _authState;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasAlerts))]
    private DashboardStatsDto stats = new();

    [ObservableProperty]
    private ObservableCollection<Chantier> chantiersRecents = new();

    /// <summary>Nom affiché dans le header du dashboard.</summary>
    public string NomUtilisateur => _authState.NomUtilisateur ?? "Utilisateur";
    public string RoleUtilisateur => _authState.Role ?? "";
    public bool HasAlerts => Stats.AlertesNonResolues > 0;

    public DashboardViewModel(IApiService apiService, AuthStateService authState)
    {
        _apiService = apiService;
        _authState = authState;
        Title = "Tableau de bord";
    }

    [RelayCommand]
    public async Task LoadDashboardAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            Stats = await _apiService.GetDashboardStatsAsync();
            var chantiers = await _apiService.GetChantiersAsync();

            ChantiersRecents.Clear();
            foreach (var ch in chantiers.Take(3))
            {
                ChantiersRecents.Add(ch);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task NouveauBonSortieAsync()
    {
        await Shell.Current.GoToAsync($"{nameof(SortieStockPage)}?mode=sortie");
    }

    [RelayCommand]
    public async Task RetourMaterielAsync()
    {
        await Shell.Current.GoToAsync($"{nameof(SortieStockPage)}?mode=retour");
    }

    [RelayCommand]
    public async Task NouveauChantierAsync()
    {
        await Shell.Current.GoToAsync("CreateChantierPage");
    }

    [RelayCommand]
    public async Task DeconnexionAsync()
    {
        bool confirm = await Shell.Current.DisplayAlert("Déconnexion", "Voulez-vous vraiment vous déconnecter ?", "Oui", "Non");
        if (confirm)
        {
            // Effacer la session
            _authState.ClearSession();
            // Retour à la page de connexion
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }

    [RelayCommand]
    public async Task VoirAlertesAsync()
    {
        await Shell.Current.GoToAsync(nameof(AlertesPage));
    }

    [RelayCommand]
    public async Task VoirHistoriqueAsync()
    {
        await Shell.Current.GoToAsync(nameof(HistoriquePage));
    }
}
