using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScaffTrack.Shared.DTOs;
using ScaffTrack1.Services;
using ScaffTrack1.ViewModels.Base;

namespace ScaffTrack1.ViewModels.Profile;

public partial class EditProfileViewModel : BaseViewModel
{
    private const string MessageHorsLigne = "Session hors ligne : connectez-vous au serveur pour modifier votre profil.";

    private readonly IApiService _apiService;
    private readonly AuthStateService _authState;

    [ObservableProperty]
    private string prenom = string.Empty;

    [ObservableProperty]
    private string nom = string.Empty;

    [ObservableProperty]
    private string telephone = string.Empty;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string motDePasseActuel = string.Empty;

    [ObservableProperty]
    private string nouveauMotDePasse = string.Empty;

    [ObservableProperty]
    private string confirmationMotDePasse = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string messageErreur = string.Empty;

    public bool HasError => !string.IsNullOrWhiteSpace(MessageErreur);

    public EditProfileViewModel(IApiService apiService, AuthStateService authState)
    {
        _apiService = apiService;
        _authState = authState;
        Title = "Modifier mon profil";
    }

    [RelayCommand]
    public async Task LoadProfilAsync()
    {
        if (IsBusy)
            return;

        MessageErreur = string.Empty;
        Email = _authState.Email ?? string.Empty;

        if (_authState.UtilisateurId is not { } id)
        {
            MessageErreur = MessageHorsLigne;
            return;
        }

        try
        {
            IsBusy = true;
            var profil = await _apiService.GetProfilAsync(id);
            if (profil is null)
            {
                MessageErreur = "Impossible de charger le profil. Vérifiez la connexion à l’API.";
                return;
            }

            Prenom = profil.Prenom;
            Nom = profil.Nom;
            Telephone = profil.Telephone;
            Email = profil.Email;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task SaveProfilAsync()
    {
        if (_authState.UtilisateurId is not { } id)
        {
            MessageErreur = MessageHorsLigne;
            return;
        }

        if (string.IsNullOrWhiteSpace(Nom))
        {
            MessageErreur = "Le nom est obligatoire.";
            return;
        }

        var changeMotDePasse = !string.IsNullOrEmpty(NouveauMotDePasse);
        if (changeMotDePasse)
        {
            if (string.IsNullOrEmpty(MotDePasseActuel))
            {
                MessageErreur = "Saisissez votre mot de passe actuel pour le modifier.";
                return;
            }

            if (NouveauMotDePasse.Length < 6)
            {
                MessageErreur = "Le nouveau mot de passe doit contenir au moins 6 caractères.";
                return;
            }

            if (NouveauMotDePasse != ConfirmationMotDePasse)
            {
                MessageErreur = "La confirmation ne correspond pas au nouveau mot de passe.";
                return;
            }
        }

        try
        {
            IsBusy = true;
            MessageErreur = string.Empty;

            var (profil, message) = await _apiService.UpdateProfilAsync(id, new UpdateProfilDto
            {
                Prenom = Prenom.Trim(),
                Nom = Nom.Trim(),
                Telephone = Telephone.Trim(),
                MotDePasseActuel = changeMotDePasse ? MotDePasseActuel : null,
                NouveauMotDePasse = changeMotDePasse ? NouveauMotDePasse : null
            });

            if (profil is null)
            {
                MessageErreur = message;
                return;
            }

            _authState.UpdateNom($"{profil.Prenom} {profil.Nom}".Trim());
            MotDePasseActuel = NouveauMotDePasse = ConfirmationMotDePasse = string.Empty;
            await Shell.Current.DisplayAlert("Profil", "Vos informations ont été enregistrées.", "OK");
            await Shell.Current.GoToAsync("..");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
