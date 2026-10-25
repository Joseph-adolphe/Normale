using ScaffTrack1.Services;
using ScaffTrack1.Views.Alertes;
using ScaffTrack1.Views.Clients;
using ScaffTrack1.Views.Historique;
using ScaffTrack1.Views.Mouvements;
using ScaffTrack1.Views.Profile;

namespace ScaffTrack1.Views.Plus;

public partial class PlusPage : ContentPage
{
    private readonly AuthStateService _authState;

    public PlusPage(AuthStateService authState)
    {
        InitializeComponent();
        _authState = authState;
    }

    private async void OnProfileTapped(object? sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync(nameof(ProfilePage));

    private async void OnAlertsTapped(object? sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync(nameof(AlertesPage));

    private async void OnClientsTapped(object? sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync(nameof(ClientsPage));

    private async void OnHistoryTapped(object? sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync(nameof(HistoriquePage));

    private async void OnReturnsTapped(object? sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync(nameof(RestitutionsPage));

    private async void OnConnectionTapped(object? sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync(nameof(OfflineStatusPage));

    private async void OnSignOutClicked(object? sender, EventArgs e)
    {
        if (await DisplayAlert("Déconnexion", "Voulez-vous vraiment vous déconnecter ?", "Oui", "Non"))
        {
            _authState.ClearSession();
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
