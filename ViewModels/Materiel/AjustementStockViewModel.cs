using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScaffTrack.Shared.DTOs;
using ScaffTrack.Shared.Models;
using ScaffTrack1.Services;
using ScaffTrack1.ViewModels.Base;

namespace ScaffTrack1.ViewModels.Materiel;

public partial class AjustementStockViewModel : BaseViewModel
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private ObservableCollection<ScaffTrack.Shared.Models.Materiel> materiels = new();

    [ObservableProperty]
    private ScaffTrack.Shared.Models.Materiel? selectedMateriel;

    [ObservableProperty]
    private string selectedType = "Entrée";

    [ObservableProperty]
    private int quantite = 1;

    [ObservableProperty]
    private string motif = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string messageErreur = string.Empty;

    public bool HasError => !string.IsNullOrWhiteSpace(MessageErreur);
    public List<string> TypesMouvement { get; } = new() { "Entrée", "Sortie", "Maintenance", "Perte" };
    public DateTime DateMouvement { get; set; } = DateTime.Today;

    public AjustementStockViewModel(IApiService apiService)
    {
        _apiService = apiService;
        Title = "Ajuster le stock";
    }

    [RelayCommand]
    public async Task LoadMaterielsAsync()
    {
        Materiels = new ObservableCollection<ScaffTrack.Shared.Models.Materiel>(await _apiService.GetMaterielsAsync());
        SelectedMateriel ??= Materiels.FirstOrDefault();
    }

    [RelayCommand]
    public async Task SaveAdjustmentAsync()
    {
        if (SelectedMateriel is null || Quantite <= 0 || string.IsNullOrWhiteSpace(Motif))
        {
            MessageErreur = "Choisissez un matériel, une quantité positive et indiquez un motif.";
            return;
        }

        try
        {
            IsBusy = true;
            MessageErreur = string.Empty;
            var adjustment = new AjustementStockDto
            {
                Type = SelectedType,
                Quantite = Quantite,
                Motif = Motif.Trim(),
                DateMouvement = DateMouvement
            };

            if (!await _apiService.AdjustMaterielStockAsync(SelectedMateriel.Id, adjustment))
            {
                MessageErreur = "L’ajustement a échoué. Vérifiez le stock disponible et réessayez.";
                return;
            }

            await Shell.Current.GoToAsync("..");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
