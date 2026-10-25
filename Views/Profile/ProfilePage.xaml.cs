using ScaffTrack1.Services;

namespace ScaffTrack1.Views.Profile;

public partial class ProfilePage : ContentPage
{
    public const string PrefAlerteRetard = "pref_alerte_retard";
    public const string PrefRappelEcheance = "pref_rappel_echeance";

    private readonly AuthStateService _authState;
    private bool _chargement;

    public ProfilePage(AuthStateService authState)
    {
        InitializeComponent();
        _authState = authState;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Rafraîchit le nom après une modification du profil
        BindingContext = null;
        BindingContext = _authState;

        _chargement = true;
        AlerteRetardSwitch.IsToggled = Preferences.Default.Get(PrefAlerteRetard, true);
        RappelEcheanceSwitch.IsToggled = Preferences.Default.Get(PrefRappelEcheance, true);
        _chargement = false;
    }

    private void OnPreferenceToggled(object? sender, ToggledEventArgs e)
    {
        if (_chargement)
            return;

        if (sender == AlerteRetardSwitch)
            Preferences.Default.Set(PrefAlerteRetard, e.Value);
        else if (sender == RappelEcheanceSwitch)
            Preferences.Default.Set(PrefRappelEcheance, e.Value);
    }

    private async void OnEditClicked(object? sender, EventArgs e) =>
        await Shell.Current.GoToAsync(nameof(EditProfilePage));
}
