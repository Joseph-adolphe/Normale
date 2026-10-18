using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScaffTrack.Shared.DTOs;
using ScaffTrack.Shared.Models;
using ScaffTrack1.Services;
using ScaffTrack1.ViewModels.Base;

namespace ScaffTrack1.ViewModels.Auth;

public partial class RegisterViewModel : BaseViewModel
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private string nom = string.Empty;

    [ObservableProperty]
    private string prenom = string.Empty;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string motDePasse = string.Empty;

    [ObservableProperty]
    private string telephone = string.Empty;

    [ObservableProperty]
    private UserRole role = UserRole.ChefChantier;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasErrorMessage))]
    private string messageErreur = string.Empty;

    public bool HasErrorMessage => !string.IsNullOrWhiteSpace(MessageErreur);

    public List<UserRole> RolesDisponibles { get; } = new()
    {
        UserRole.Admin,
        UserRole.Magasinier,
        UserRole.ChefChantier
    };

    public RegisterViewModel(IApiService apiService)
    {
        _apiService = apiService;
        Title = "Inscription";
    }

    [RelayCommand]
    public async Task RegisterAsync()
    {
        if (IsBusy) return;

        if (string.IsNullOrWhiteSpace(Nom) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(MotDePasse))
        {
            MessageErreur = "Veuillez renseigner au moins votre nom, email et mot de passe.";
            return;
        }

        try
        {
            IsBusy = true;
            MessageErreur = string.Empty;

            var request = new RegisterRequestDto
            {
                Nom = Nom,
                Prenom = Prenom,
                Email = Email,
                MotDePasse = MotDePasse,
                Telephone = Telephone,
                Role = Role
            };

            var result = await _apiService.RegisterAsync(request);

            if (result.Succes)
            {
                await Shell.Current.DisplayAlert("Bienvenue !", $"Compte créé avec succès pour {result.Nom}.", "Continuer");
                await Shell.Current.GoToAsync("//DashboardPage");
            }
            else
            {
                MessageErreur = result.Message ?? "Erreur lors de l'inscription.";
            }
        }
        catch (Exception ex)
        {
            MessageErreur = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task GoToLoginAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
