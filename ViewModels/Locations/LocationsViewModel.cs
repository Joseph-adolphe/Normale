using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScaffTrack.Shared.Models;
using ScaffTrack1.Services;
using ScaffTrack1.ViewModels.Base;
using ScaffTrack1.Views.Mouvements;

namespace ScaffTrack1.ViewModels.Locations;

public partial class LocationsViewModel : BaseViewModel
{
    private readonly IApiService _apiService;
    private List<MouvementStock> _locations = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(LocationsFiltrees))]
    private ObservableCollection<MouvementStock> locations = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(LocationsFiltrees))]
    private string searchText = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(LocationsFiltrees))]
    private string filtre = "En cours";

    public IEnumerable<MouvementStock> LocationsFiltrees => _locations
        .Where(mouvement => mouvement.Type == TypeMouvement.SortieChantier)
        .Where(mouvement => string.IsNullOrWhiteSpace(SearchText)
            || mouvement.NumeroBon.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
            || mouvement.Chantier?.Nom.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true
            || mouvement.Chantier?.Client?.NomEntreprise.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true)
        .Where(mouvement => Filtre == "Toutes"
            || (Filtre == "Retard" && mouvement.EstEnRetard)
            || (Filtre == "Clôturées" && mouvement.Chantier?.Statut == StatutChantier.Termine)
            || (Filtre == "En cours" && mouvement.Chantier?.Statut != StatutChantier.Termine));

    public int EnCoursCount => _locations.Count(m => m.Type == TypeMouvement.SortieChantier && m.Chantier?.Statut != StatutChantier.Termine);
    public int EnRetardCount => _locations.Count(m => m.Type == TypeMouvement.SortieChantier && m.EstEnRetard);
    public int ClotureCount => _locations.Count(m => m.Type == TypeMouvement.SortieChantier && m.Chantier?.Statut == StatutChantier.Termine);
    public bool HasRetards => EnRetardCount > 0;

    public LocationsViewModel(IApiService apiService)
    {
        _apiService = apiService;
        Title = "Locations";
    }

    partial void OnSearchTextChanged(string value) => OnPropertyChanged(nameof(LocationsFiltrees));

    partial void OnFiltreChanged(string value) => OnPropertyChanged(nameof(LocationsFiltrees));

    [RelayCommand]
    public async Task LoadLocationsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            _locations = await _apiService.GetMouvementsAsync();
            OnPropertyChanged(nameof(LocationsFiltrees));
            OnPropertyChanged(nameof(EnCoursCount));
            OnPropertyChanged(nameof(EnRetardCount));
            OnPropertyChanged(nameof(ClotureCount));
            OnPropertyChanged(nameof(HasRetards));
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void SelectFilter(string filter) => Filtre = filter;

    [RelayCommand]
    private async Task NewLocationAsync()
    {
        await Shell.Current.GoToAsync($"{nameof(SortieStockPage)}?mode=sortie");
    }

    [RelayCommand]
    private async Task NewReturnAsync()
    {
        await Shell.Current.GoToAsync($"{nameof(SortieStockPage)}?mode=retour");
    }

    [RelayCommand]
    private async Task OpenLocationAsync(MouvementStock movement)
    {
        await Shell.Current.GoToAsync(nameof(MouvementDetailPage), new Dictionary<string, object>
        {
            ["mouvement"] = movement
        });
    }
}
