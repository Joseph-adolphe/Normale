using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScaffTrack.Shared.Models;
using ScaffTrack1.Services;
using ScaffTrack1.ViewModels.Base;

namespace ScaffTrack1.ViewModels.Chantiers;

public partial class CreateChantierViewModel : BaseViewModel
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private string nom = string.Empty;

    [ObservableProperty]
    private string adresse = string.Empty;

    [ObservableProperty]
    private string ville = "Douala";

    [ObservableProperty]
    private DateTime dateDebut = DateTime.Today;

    [ObservableProperty]
    private DateTime dateFinPrevue = DateTime.Today.AddDays(30);

    [ObservableProperty]
    private decimal montantLocationFCFA = 1200000;

    [ObservableProperty]
    private decimal montantCautionFCFA = 300000;

    [ObservableProperty]
    private string remarques = string.Empty;

    [ObservableProperty]
    private ObservableCollection<Client> clients = new();

    [ObservableProperty]
    private Client? selectedClient;

    public CreateChantierViewModel(IApiService apiService)
    {
        _apiService = apiService;
        Title = "Nouveau Chantier";
    }

    [RelayCommand]
    public async Task LoadClientsAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            var list = await _apiService.GetClientsAsync();
            Clients.Clear();
            foreach (var client in list)
            {
                Clients.Add(client);
            }

            if (Clients.Count > 0 && SelectedClient == null)
            {
                SelectedClient = Clients[0];
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task SaveChantierAsync()
    {
        if (string.IsNullOrWhiteSpace(Nom))
        {
            await Shell.Current.DisplayAlert("Validation", "Veuillez entrer le nom du chantier.", "OK");
            return;
        }

        if (SelectedClient == null)
        {
            await Shell.Current.DisplayAlert("Validation", "Veuillez sélectionner un client.", "OK");
            return;
        }

        try
        {
            IsBusy = true;

            var newChantier = new Chantier
            {
                Id = Guid.NewGuid(),
                Nom = Nom,
                Adresse = Adresse,
                Ville = Ville,
                ClientId = SelectedClient.Id,
                DateDebut = DateDebut,
                DateFinPrevue = DateFinPrevue,
                Statut = StatutChantier.EnAttente,
                MontantLocationPrevueFCFA = MontantLocationFCFA,
                MontantCautionFCFA = MontantCautionFCFA,
                Remarques = Remarques
            };

            var success = await _apiService.CreateChantierAsync(newChantier);

            if (success)
            {
                await Shell.Current.DisplayAlert("Succès", "Le chantier a été créé avec succès !", "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Shell.Current.DisplayAlert("Erreur", "Impossible de créer le chantier. Vérifiez la connexion à l'API.", "OK");
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
