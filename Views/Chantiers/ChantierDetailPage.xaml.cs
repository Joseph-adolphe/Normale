using ScaffTrack.Shared.Models;
using ScaffTrack1.Services;
using ScaffTrack1.Views.Clients;
using ScaffTrack1.Views.Mouvements;

namespace ScaffTrack1.Views.Chantiers;

[QueryProperty(nameof(Chantier), "chantier")]
public partial class ChantierDetailPage : ContentPage
{
    private static readonly Dictionary<StatutChantier, (string Libelle, string Couleur)> Statuts = new()
    {
        [StatutChantier.EnAttente] = ("En attente", "#9A6B0A"),
        [StatutChantier.EnCours] = ("En cours", "#087A50"),
        [StatutChantier.Suspendu] = ("Suspendu", "#B4232C"),
        [StatutChantier.Termine] = ("Terminé", "#586B84")
    };

    private readonly IApiService _apiService;

    public ChantierDetailPage(IApiService apiService)
    {
        InitializeComponent();
        _apiService = apiService;
    }

    public Chantier? Chantier
    {
        get => BindingContext as Chantier;
        set => AfficherChantier(value);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Recharge la fiche pour refléter les sorties / restitutions enregistrées depuis
        if (BindingContext is Chantier chantier
            && await _apiService.GetChantierByIdAsync(chantier.Id) is { } aJour)
        {
            AfficherChantier(aJour);
        }
    }

    private void AfficherChantier(Chantier? chantier)
    {
        BindingContext = null;
        BindingContext = chantier;
        if (chantier is null)
            return;

        var (libelle, couleur) = Statuts[chantier.Statut];
        StatutLabel.Text = libelle;
        StatutLabel.TextColor = StatutDot.TextColor = Color.FromArgb(couleur);
        ActionsMouvementGrid.IsVisible = chantier.Statut != StatutChantier.Termine;
    }

    private async void OnChangeStatusClicked(object? sender, EventArgs e)
    {
        if (BindingContext is not Chantier chantier)
            return;

        var choix = Statuts.Where(s => s.Key != chantier.Statut).Select(s => s.Value.Libelle).ToArray();
        var reponse = await DisplayActionSheet("Nouveau statut", "Annuler", null, choix);
        if (reponse is null || !choix.Contains(reponse))
            return;

        var statut = Statuts.First(s => s.Value.Libelle == reponse).Key;

        var (succes, message) = await _apiService.ChangerStatutChantierAsync(chantier.Id, statut);
        if (!succes)
        {
            await DisplayAlert("Statut", message, "OK");
            return;
        }

        chantier.Statut = statut;
        chantier.DateFinReelle = statut == StatutChantier.Termine ? DateTime.Today : null;
        AfficherChantier(chantier);
    }

    private async void OnNewSortieClicked(object? sender, EventArgs e)
    {
        if (BindingContext is Chantier chantier)
            await Shell.Current.GoToAsync($"{nameof(SortieStockPage)}?mode=sortie&chantierId={chantier.Id}");
    }

    private async void OnNewRetourClicked(object? sender, EventArgs e)
    {
        if (BindingContext is Chantier chantier)
            await Shell.Current.GoToAsync($"{nameof(SortieStockPage)}?mode=retour&chantierId={chantier.Id}");
    }

    private async void OnCallClientClicked(object? sender, EventArgs e)
    {
        if (BindingContext is Chantier { Client.Telephone.Length: > 0 } chantier && PhoneDialer.Default.IsSupported)
            PhoneDialer.Default.Open(chantier.Client.Telephone);
        else
            await DisplayAlert("Téléphone", "Aucun numéro de téléphone valide n’est disponible.", "OK");
    }

    private async void OnClientClicked(object? sender, EventArgs e)
    {
        if (BindingContext is Chantier { Client: not null } chantier)
        {
            await Shell.Current.GoToAsync(nameof(ClientDetailPage), new Dictionary<string, object>
            {
                ["client"] = chantier.Client
            });
        }
    }

    private async void OnMapClicked(object? sender, EventArgs e)
    {
        if (BindingContext is not Chantier chantier || string.IsNullOrWhiteSpace(chantier.Adresse))
        {
            await DisplayAlert("Localisation", "Aucune adresse n’est disponible pour ce chantier.", "OK");
            return;
        }

        var query = Uri.EscapeDataString($"{chantier.Adresse}, {chantier.Ville}");
        await Launcher.Default.OpenAsync(new Uri($"https://www.google.com/maps/search/?api=1&query={query}"));
    }

    private async void OnMaterialsClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//main/parc/MaterielListPage");
    }

    private async void OnMouvementSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not MouvementStock mouvement)
            return;

        ((CollectionView)sender!).SelectedItem = null;
        mouvement.Chantier ??= BindingContext as Chantier;
        await Shell.Current.GoToAsync(nameof(MouvementDetailPage), new Dictionary<string, object>
        {
            ["mouvement"] = mouvement
        });
    }
}
