using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScaffTrack.Shared.Models;
using ScaffTrack1.Services;
using ScaffTrack1.ViewModels.Base;
using ScaffTrack1.Views.Clients;

namespace ScaffTrack1.ViewModels.Clients;

public partial class ClientsViewModel : BaseViewModel
{
    private readonly IApiService _apiService;
    private List<Client> _allClients = new();

    [ObservableProperty]
    private ObservableCollection<Client> clients = new();

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private string nomEntreprise = string.Empty;

    [ObservableProperty]
    private string contactNom = string.Empty;

    [ObservableProperty]
    private string telephone = string.Empty;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string adresse = string.Empty;

    [ObservableProperty]
    private string ville = "Douala";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string messageErreur = string.Empty;

    public bool HasError => !string.IsNullOrWhiteSpace(MessageErreur);

    public ClientsViewModel(IApiService apiService)
    {
        _apiService = apiService;
        Title = "Clients";
    }

    [RelayCommand]
    public async Task LoadClientsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            _allClients = await _apiService.GetClientsAsync();
            ApplyFilter();
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnSearchTextChanged(string value) => ApplyFilter();

    private void ApplyFilter()
    {
        var filtered = string.IsNullOrWhiteSpace(SearchText)
            ? _allClients
            : _allClients.Where(client =>
                client.NomEntreprise.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                || client.ContactNom.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                || client.Ville.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();

        Clients = new ObservableCollection<Client>(filtered);
    }

    [RelayCommand]
    private async Task OpenClientAsync(Client client)
    {
        await Shell.Current.GoToAsync(nameof(ClientDetailPage), new Dictionary<string, object>
        {
            ["client"] = client
        });
    }

    [RelayCommand]
    private async Task NewClientAsync()
    {
        await Shell.Current.GoToAsync(nameof(ClientFormPage));
    }

    [RelayCommand]
    public async Task SaveClientAsync()
    {
        if (string.IsNullOrWhiteSpace(NomEntreprise)
            || string.IsNullOrWhiteSpace(ContactNom)
            || string.IsNullOrWhiteSpace(Telephone)
            || string.IsNullOrWhiteSpace(Email))
        {
            MessageErreur = "Renseignez l’entreprise, le contact, le téléphone et l’adresse e-mail.";
            return;
        }

        if (!Email.Contains('@') || !Email.Contains('.'))
        {
            MessageErreur = "Saisissez une adresse e-mail valide.";
            return;
        }

        try
        {
            IsBusy = true;
            MessageErreur = string.Empty;

            var client = new Client
            {
                NomEntreprise = NomEntreprise.Trim(),
                ContactNom = ContactNom.Trim(),
                Telephone = Telephone.Trim(),
                Email = Email.Trim(),
                Adresse = Adresse.Trim(),
                Ville = Ville.Trim()
            };

            if (!await _apiService.CreateClientAsync(client))
            {
                MessageErreur = "Impossible d’enregistrer le client. Vérifiez la connexion à l’API.";
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
