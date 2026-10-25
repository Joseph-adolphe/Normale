using ScaffTrack.Shared.Models;

namespace ScaffTrack1.Views.Clients;

[QueryProperty(nameof(Client), "client")]
public partial class ClientDetailPage : ContentPage
{
    public ClientDetailPage()
    {
        InitializeComponent();
    }

    public Client? Client
    {
        get => BindingContext as Client;
        set => BindingContext = value;
    }

    private async void OnCallClicked(object? sender, EventArgs e)
    {
        if (BindingContext is Client { Telephone.Length: > 0 } client && PhoneDialer.Default.IsSupported)
            PhoneDialer.Default.Open(client.Telephone);
        else
            await DisplayAlert("Téléphone", "Aucun numéro de téléphone valide n’est disponible.", "OK");
    }

    private async void OnEmailClicked(object? sender, EventArgs e)
    {
        if (BindingContext is Client { Email.Length: > 0 } client && Email.Default.IsComposeSupported)
            await Email.Default.ComposeAsync(new EmailMessage { To = new List<string> { client.Email } });
        else
            await DisplayAlert("E-mail", "Aucune adresse e-mail valide n’est disponible.", "OK");
    }
}
