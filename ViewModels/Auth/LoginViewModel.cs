using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScaffTrack1.Services;
using ScaffTrack1.ViewModels.Base;
using ScaffTrack1.Views.Auth;

namespace ScaffTrack1.ViewModels.Auth;

public partial class LoginViewModel : BaseViewModel
{
    private readonly IApiService _apiService;
    private readonly AuthStateService _authState;

    [ObservableProperty]
    private string email = "admin@scafftrack.com";

    [ObservableProperty]
    private string motDePasse = "admin123";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasErrorMessage))]
    private string messageErreur = string.Empty;

    public bool HasErrorMessage => !string.IsNullOrWhiteSpace(MessageErreur);

    public LoginViewModel(IApiService apiService, AuthStateService authState)
    {
        _apiService = apiService;
        _authState = authState;
        Title = "Connexion";
    }

    [RelayCommand]
    public async Task LoginAsync()
    {
        if (IsBusy) return;

        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(MotDePasse))
        {
            MessageErreur = "Veuillez renseigner votre email et mot de passe.";
            return;
        }

        try
        {
            IsBusy = true;
            MessageErreur = string.Empty;

            var result = await _apiService.LoginAsync(Email, MotDePasse);

            if (result.Succes)
            {
                // Enregistrer la session
                _authState.SetSession(
                    nom: result.Nom ?? Email,
                    role: result.Role ?? "Utilisateur",
                    email: Email,
                    id: result.UtilisateurId == Guid.Empty ? null : result.UtilisateurId
                );

                // Redirection vers le tableau de bord
                await Shell.Current.GoToAsync("//main");
            }
            else
            {
                MessageErreur = result.Message ?? "Identifiants invalides.";
            }
        }
        catch (Exception ex)
        {
            MessageErreur = "Erreur de connexion : " + ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task MotDePasseOublieAsync()
    {
        await Shell.Current.GoToAsync(nameof(ForgotPasswordPage));
    }

    [RelayCommand]
    public async Task GoToRegisterAsync()
    {
        await Shell.Current.GoToAsync("RegisterPage");
    }
}
