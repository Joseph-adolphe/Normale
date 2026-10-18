using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScaffTrack.Shared.Models;
using ScaffTrack1.Services;
using ScaffTrack1.ViewModels.Base;

namespace ScaffTrack1.ViewModels.Mouvements;

public partial class SortieStockViewModel : BaseViewModel
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private ObservableCollection<Chantier> chantiers = new();

    [ObservableProperty]
    private Chantier? selectedChantier;

    [ObservableProperty]
    private ObservableCollection<MaterielLigneItem> materielItems = new();

    [ObservableProperty]
    private string nomRecepteur = string.Empty;

    [ObservableProperty]
    private string telephoneRecepteur = string.Empty;

    [ObservableProperty]
    private string observations = string.Empty;

    public SortieStockViewModel(IApiService apiService)
    {
        _apiService = apiService;
        Title = "Bon de Sortie Matériel";
    }

    [RelayCommand]
    public async Task LoadDataAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            var chantiersList = await _apiService.GetChantiersAsync();
            Chantiers.Clear();
            foreach (var ch in chantiersList)
                Chantiers.Add(ch);

            if (Chantiers.Count > 0 && SelectedChantier == null)
                SelectedChantier = Chantiers[0];

            var materielList = await _apiService.GetMaterielsAsync();
            MaterielItems.Clear();
            foreach (var mat in materielList)
            {
                MaterielItems.Add(new MaterielLigneItem
                {
                    Materiel = mat,
                    Quantite = 0
                });
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task ValiderBonSortieAsync()
    {
        if (SelectedChantier == null)
        {
            await Shell.Current.DisplayAlert("Validation", "Veuillez sélectionner un chantier destinataire.", "OK");
            return;
        }

        var lignesChoisie = MaterielItems.Where(i => i.Quantite > 0).ToList();
        if (lignesChoisie.Count == 0)
        {
            await Shell.Current.DisplayAlert("Validation", "Veuillez sélectionner au moins un article avec une quantité supérieure à 0.", "OK");
            return;
        }

        // Vérification de stock disponible
        foreach (var item in lignesChoisie)
        {
            if (item.Quantite > item.Materiel.QuantiteDisponible)
            {
                await Shell.Current.DisplayAlert("Stock insuffisant", 
                    $"Quantité demandée pour {item.Materiel.Nom} ({item.Quantite}) supérieure au disponible ({item.Materiel.QuantiteDisponible}).", "OK");
                return;
            }
        }

        try
        {
            IsBusy = true;

            var mouvement = new MouvementStock
            {
                Id = Guid.NewGuid(),
                ChantierId = SelectedChantier.Id,
                Type = TypeMouvement.SortieChantier,
                NomRecepteur = NomRecepteur,
                TelephoneRecepteur = TelephoneRecepteur,
                Observations = Observations,
                DateMouvement = DateTime.UtcNow,
                Lignes = lignesChoisie.Select(i => new LigneMouvement
                {
                    Id = Guid.NewGuid(),
                    MaterielId = i.Materiel.Id,
                    Quantite = i.Quantite,
                    EtatAuMouvement = EtatMateriel.Bon
                }).ToList()
            };

            var success = await _apiService.CreateMouvementAsync(mouvement);

            if (success)
            {
                await Shell.Current.DisplayAlert("Bon Validé", "Le bon de livraison a été validé et les stocks ont été mis à jour !", "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Shell.Current.DisplayAlert("Erreur", "Une erreur est survenue lors de la validation du bon.", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erreur", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}

public partial class MaterielLigneItem : ObservableObject
{
    public ScaffTrack.Shared.Models.Materiel Materiel { get; set; } = new();

    [ObservableProperty]
    private int quantite;

    [RelayCommand]
    private void Increment()
    {
        if (Quantite < Materiel.QuantiteDisponible)
            Quantite++;
    }

    [RelayCommand]
    private void Decrement()
    {
        if (Quantite > 0)
            Quantite--;
    }
}
