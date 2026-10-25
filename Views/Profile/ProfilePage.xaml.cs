using ScaffTrack1.Services;

namespace ScaffTrack1.Views.Profile;

public partial class ProfilePage : ContentPage
{
    public ProfilePage(AuthStateService authState)
    {
        InitializeComponent();
        BindingContext = authState;
    }

    private async void OnEditClicked(object? sender, EventArgs e) =>
        await DisplayAlert("Profil", "La modification du profil sera disponible après la configuration de l’API de compte.", "OK");
}
