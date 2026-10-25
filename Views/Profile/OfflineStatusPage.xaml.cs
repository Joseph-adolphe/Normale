namespace ScaffTrack1.Views.Profile;

public partial class OfflineStatusPage : ContentPage
{
    public OfflineStatusPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        UpdateStatus();
    }

    private void UpdateStatus()
    {
        var isConnected = Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
        StatusLabel.Text = isConnected
            ? "Connexion Internet disponible. Les données se synchronisent avec le serveur."
            : "Vous êtes hors connexion. Les données locales restent consultables.";
        StatusLabel.TextColor = isConnected ? Color.FromArgb("#087A50") : Color.FromArgb("#B4232C");
    }

    private void OnRetryClicked(object? sender, EventArgs e) => UpdateStatus();

    private async void OnLocalDataClicked(object? sender, EventArgs e) =>
        await Shell.Current.GoToAsync("//main/home/DashboardPage");
}
