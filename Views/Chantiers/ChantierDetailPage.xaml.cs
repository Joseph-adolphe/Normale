using ScaffTrack.Shared.Models;
using ScaffTrack1.Views.Clients;

namespace ScaffTrack1.Views.Chantiers;

[QueryProperty(nameof(Chantier), "chantier")]
public partial class ChantierDetailPage : ContentPage
{
    public ChantierDetailPage()
    {
        InitializeComponent();
    }

    public Chantier? Chantier
    {
        get => BindingContext as Chantier;
        set => BindingContext = value;
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
}
