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
    private readonly AuthStateService _authState;
    private string? _requestedChantierId;

    [ObservableProperty]
    private bool isRetour;

    [ObservableProperty]
    private DateTime dateSortie = DateTime.Today;

    [ObservableProperty]
    private DateTime dateRetourPrevu = DateTime.Today.AddDays(7);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string messageErreur = string.Empty;

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

    public string TitreAction => IsRetour ? "Nouvelle restitution" : "Nouvelle location";
    public string NomEtape => IsRetour ? "Restitution" : "Chantier";
    public string DateMouvementLabel => IsRetour ? "Date de restitution" : "Date de sortie";
    public bool ShowDateReturn => !IsRetour;
    public string TitreSelectionMateriel => IsRetour ? "Matériel à restituer" : "Matériel à expédier";
    public string TexteValidation => IsRetour ? "Enregistrer la restitution" : "Confirmer la sortie";
    public bool HasError => !string.IsNullOrWhiteSpace(MessageErreur);

    public SortieStockViewModel(IApiService apiService, AuthStateService authState)
    {
        _apiService = apiService;
        _authState = authState;
        Title = "Nouvelle location";
        PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(IsRetour))
            {
                OnPropertyChanged(nameof(TitreAction));
                OnPropertyChanged(nameof(NomEtape));
                OnPropertyChanged(nameof(DateMouvementLabel));
                OnPropertyChanged(nameof(ShowDateReturn));
                OnPropertyChanged(nameof(TitreSelectionMateriel));
                OnPropertyChanged(nameof(TexteValidation));
            }
        };
    }

    public void SetMode(string mode, string? chantierId = null)
    {
        IsRetour = string.Equals(mode, "retour", StringComparison.OrdinalIgnoreCase);
        _requestedChantierId = chantierId;
        ApplyRequestedChantier();
        Title = TitreAction;
    }

    private void ApplyRequestedChantier()
    {
        if (Guid.TryParse(_requestedChantierId, out var parsedId))
            SelectedChantier = Chantiers.FirstOrDefault(ch => ch.Id == parsedId);
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

            ApplyRequestedChantier();
            if (Chantiers.Count > 0 && SelectedChantier == null)
                SelectedChantier = Chantiers[0];

            var materielList = await _apiService.GetMaterielsAsync();
            MaterielItems.Clear();
            foreach (var mat in materielList.Where(m => IsRetour ? m.QuantiteEnChantier > 0 : m.QuantiteDisponible > 0))
            {
                MaterielItems.Add(new MaterielLigneItem
                {
                    Materiel = mat,
                    Quantite = 0,
                    Maximum = IsRetour ? mat.QuantiteEnChantier : mat.QuantiteDisponible
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
        MessageErreur = string.Empty;
        if (SelectedChantier == null)
        {
            MessageErreur = "Veuillez sélectionner un chantier.";
            return;
        }

        if (!IsRetour && DateRetourPrevu <= DateSortie)
        {
            MessageErreur = "La date de retour doit être postérieure à la date de sortie.";
            return;
        }

        var lignesChoisie = MaterielItems.Where(i => i.Quantite > 0).ToList();
        if (lignesChoisie.Count == 0)
        {
            MessageErreur = "Sélectionnez au moins un article avec une quantité supérieure à 0.";
            return;
        }

        // Vérification de stock disponible
        foreach (var item in lignesChoisie)
        {
            if (item.QuantiteEndommagee < 0 || item.QuantitePerdue < 0)
            {
                MessageErreur = "Les quantités endommagées et perdues ne peuvent pas être négatives.";
                return;
            }

            var maximum = IsRetour ? item.Materiel.QuantiteEnChantier : item.Materiel.QuantiteDisponible;
            if (item.Quantite > maximum)
            {
                MessageErreur = IsRetour
                    ? $"Quantité reçue pour {item.Materiel.Nom} supérieure à celle suivie sur chantier ({maximum})."
                    : $"Quantité demandée pour {item.Materiel.Nom} supérieure au stock disponible ({maximum}).";
                return;
            }

            if (item.QuantiteEndommagee + item.QuantitePerdue > item.Quantite)
            {
                MessageErreur = $"Les quantités endommagées et perdues dépassent la restitution de {item.Materiel.Nom}.";
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
                CreeParId = _authState.UtilisateurId ?? Guid.Empty,
                Type = IsRetour ? TypeMouvement.RetourStock : TypeMouvement.SortieChantier,
                NomRecepteur = NomRecepteur,
                TelephoneRecepteur = TelephoneRecepteur,
                Observations = IsRetour || DateRetourPrevu <= DateSortie
                    ? Observations
                    : $"{Observations} • Retour prévu : {DateRetourPrevu:dd/MM/yyyy}".Trim(' ', '•'),
                DateMouvement = DateSortie,
                Lignes = lignesChoisie.Select(i => new LigneMouvement
                {
                    Id = Guid.NewGuid(),
                    MaterielId = i.Materiel.Id,
                    Quantite = i.Quantite,
                    EtatAuMouvement = i.QuantiteEndommagee > 0 ? EtatMateriel.Endommage : EtatMateriel.Bon,
                    QuantiteEndommagee = i.QuantiteEndommagee,
                    QuantitePerdue = i.QuantitePerdue
                }).ToList()
            };

            var success = await _apiService.CreateMouvementAsync(mouvement);

            if (success)
            {
                await Shell.Current.DisplayAlert(
                    IsRetour ? "Restitution enregistrée" : "Bon validé",
                    IsRetour ? "Les quantités restituées ont été prises en compte." : "Le bon de sortie a été validé et le stock mis à jour.",
                    "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                MessageErreur = "Une erreur est survenue lors de l’enregistrement. Vérifiez le stock et réessayez.";
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
    public int Maximum { get; set; }

    [ObservableProperty]
    private int quantite;

    [ObservableProperty]
    private int quantiteEndommagee;

    [ObservableProperty]
    private int quantitePerdue;

    [RelayCommand]
    private void Increment()
    {
        if (Quantite < Maximum)
            Quantite++;
    }

    [RelayCommand]
    private void Decrement()
    {
        if (Quantite > 0)
            Quantite--;
    }
}
