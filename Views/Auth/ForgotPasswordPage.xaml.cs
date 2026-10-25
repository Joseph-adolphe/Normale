namespace ScaffTrack1.Views.Auth;

public partial class ForgotPasswordPage : ContentPage
{
    private int _cooldownSeconds;

    public ForgotPasswordPage()
    {
        InitializeComponent();
    }

    private async void OnBackClicked(object? sender, EventArgs e) =>
        await Shell.Current.GoToAsync("..");

    private async void OnSendClicked(object? sender, EventArgs e)
    {
        var email = EmailEntry.Text?.Trim();
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@') || !email.Contains('.'))
        {
            await DisplayAlert("Adresse invalide", "Saisissez une adresse e-mail professionnelle valide.", "OK");
            return;
        }

        RequestStatus.IsVisible = true;
        _cooldownSeconds = 42;
        SendButton.IsEnabled = false;
        ResendButton.IsEnabled = false;

        Dispatcher.StartTimer(TimeSpan.FromSeconds(1), () =>
        {
            _cooldownSeconds--;
            CountdownLabel.Text = $"00:{Math.Max(0, _cooldownSeconds):D2}";
            if (_cooldownSeconds > 0)
                return true;

            SendButton.IsEnabled = true;
            ResendButton.IsEnabled = true;
            return false;
        });
    }
}
