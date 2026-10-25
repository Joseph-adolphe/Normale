using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScaffTrack.Shared.Models;
using ScaffTrack1.Services;
using ScaffTrack1.ViewModels.Base;
using ScaffTrack1.Views.Mouvements;

namespace ScaffTrack1.ViewModels.Historique;

public partial class HistoriqueViewModel : BaseViewModel
{
    private readonly IApiService _apiService;
    private List<MouvementStock> _mouvements = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MouvementsFiltres))]
    private string recherche = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MouvementsFiltres))]
    private string filtre = "Tout";

    public ObservableCollection<MouvementStock> Mouvements { get; } = new();

    public IEnumerable<MouvementStock> MouvementsFiltres => _mouvements
        .Where(mouvement => Filtre == "Tout" || mouvement.Type.ToString().Contains(Filtre, StringComparison.OrdinalIgnoreCase))
        .Where(mouvement => string.IsNullOrWhiteSpace(Recherche)
            || mouvement.NumeroBon.Contains(Recherche, StringComparison.OrdinalIgnoreCase)
            || mouvement.Chantier?.Nom.Contains(Recherche, StringComparison.OrdinalIgnoreCase) == true);

    public HistoriqueViewModel(IApiService apiService)
    {
        _apiService = apiService;
        Title = "Historique";
    }

    partial void OnRechercheChanged(string value) => OnPropertyChanged(nameof(MouvementsFiltres));
    partial void OnFiltreChanged(string value) => OnPropertyChanged(nameof(MouvementsFiltres));

    [RelayCommand]
    public async Task LoadHistoriqueAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            _mouvements = await _apiService.GetMouvementsAsync();
            OnPropertyChanged(nameof(MouvementsFiltres));
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void SelectFilter(string filter) => Filtre = filter;

    [RelayCommand]
    private async Task OpenMouvementAsync(MouvementStock mouvement)
    {
        await Shell.Current.GoToAsync(nameof(MouvementDetailPage), new Dictionary<string, object>
        {
            ["mouvement"] = mouvement
        });
    }
}
