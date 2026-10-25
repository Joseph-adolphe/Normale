using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScaffTrack.Shared.Models;
using ScaffTrack1.Services;
using ScaffTrack1.ViewModels.Base;

namespace ScaffTrack1.ViewModels.Alertes;

public partial class AlertesViewModel : BaseViewModel
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private ObservableCollection<Alerte> alertes = new();

    public AlertesViewModel(IApiService apiService)
    {
        _apiService = apiService;
        Title = "Notifications";
    }

    [RelayCommand]
    public async Task LoadAlertesAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            Alertes = new ObservableCollection<Alerte>(await _apiService.GetAlertesAsync());
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ResolveAsync(Alerte alerte)
    {
        if (!await _apiService.ResolveAlerteAsync(alerte.Id))
        {
            await Shell.Current.DisplayAlert("Erreur", "L’alerte n’a pas pu être clôturée. Vérifiez la connexion à l’API.", "OK");
            return;
        }

        Alertes.Remove(alerte);
    }
}
