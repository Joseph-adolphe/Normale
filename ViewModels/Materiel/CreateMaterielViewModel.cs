using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScaffTrack.Shared.Models;
using ScaffTrack1.Services;
using ScaffTrack1.ViewModels.Base;

namespace ScaffTrack1.ViewModels.Materiel;

public partial class CreateMaterielViewModel : BaseViewModel
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private string nom = string.Empty;

    [ObservableProperty]
    private string reference = string.Empty;

    [ObservableProperty]
    private string dimensions = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private string emplacementDepot = string.Empty;

    [ObservableProperty]
    private int quantiteTotale;

    [ObservableProperty]
    private decimal prixLocationJourFCFA;

    [ObservableProperty]
    private decimal prixRemplacementFCFA;

    [ObservableProperty]
    private ObservableCollection<Categorie> categories = new();

    [ObservableProperty]
    private Categorie? selectedCategorie;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string messageErreur = string.Empty;

    public bool HasError => !string.IsNullOrWhiteSpace(MessageErreur);

    public CreateMaterielViewModel(IApiService apiService)
    {
        _apiService = apiService;
        Title = "Ajouter du matériel";
    }

    [RelayCommand]
    public async Task LoadCategoriesAsync()
    {
        var categories = await _apiService.GetCategoriesAsync();
        Categories = new ObservableCollection<Categorie>(categories);
        SelectedCategorie ??= Categories.FirstOrDefault();
    }

    [RelayCommand]
    public async Task SaveMaterielAsync()
    {
        if (string.IsNullOrWhiteSpace(Nom) || string.IsNullOrWhiteSpace(Reference) || SelectedCategorie is null || QuantiteTotale <= 0)
        {
            MessageErreur = "Renseignez le nom, la référence, la catégorie et une quantité positive.";
            return;
        }

        try
        {
            IsBusy = true;
            MessageErreur = string.Empty;

            var materiel = new ScaffTrack.Shared.Models.Materiel
            {
                Nom = Nom.Trim(),
                Reference = Reference.Trim(),
                Dimensions = Dimensions.Trim(),
                Description = Description.Trim(),
                EmplacementDepot = EmplacementDepot.Trim(),
                CategorieId = SelectedCategorie.Id,
                QuantiteTotale = QuantiteTotale,
                QuantiteDisponible = QuantiteTotale,
                PrixLocationJourFCFA = PrixLocationJourFCFA,
                PrixRemplacementFCFA = PrixRemplacementFCFA
            };

            if (!await _apiService.CreateMaterielAsync(materiel))
            {
                MessageErreur = "Impossible d’enregistrer le matériel. Vérifiez la connexion à l’API.";
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
