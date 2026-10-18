using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScaffTrack.Shared.DTOs;
using ScaffTrack.Shared.Models;
using ScaffTrack1.Services;
using ScaffTrack1.ViewModels.Base;

namespace ScaffTrack1.ViewModels.Dashboard;

public partial class DashboardViewModel : BaseViewModel
{
    private readonly IApiService _apiService;
    private readonly AuthStateService _authState;

    [ObservableProperty]
    private DashboardStatsDto stats = new();

    [ObservableProperty]
    private ObservableCollection<Chantier> chantiersRecents = new();

    /// <summary>Nom affiché dans le header du dashboard.</summary>
    public string NomUtilisateur => _authState.NomUtilisateur ?? "Utilisateur";
    public string RoleUtilisateur => _authState.Role ?? "";

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
        await Shell.Current.GoToAsync("SortieStockPage");
    }

    [RelayCommand]
    public async Task RetourMaterielAsync()
    {
        await Shell.Current.DisplayAlert("Retour Matériel", "Formulaire de contrôle de retour et réintégration en stock.", "OK");
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
}
