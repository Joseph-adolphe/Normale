using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScaffTrack.Shared.Models;
using ScaffTrack1.Services;
using ScaffTrack1.ViewModels.Base;
using ScaffTrack1.Views.Mouvements;

namespace ScaffTrack1.ViewModels.Mouvements;

public partial class RestitutionsViewModel : BaseViewModel
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private ObservableCollection<MouvementStock> restitutions = new();

    public RestitutionsViewModel(IApiService apiService)
    {
        _apiService = apiService;
        Title = "Restitutions";
    }

    [RelayCommand]
    public async Task LoadRestitutionsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            var mouvements = await _apiService.GetMouvementsAsync();
            Restitutions = new ObservableCollection<MouvementStock>(
                mouvements.Where(mouvement => mouvement.Type == TypeMouvement.RetourStock)
                    .OrderByDescending(mouvement => mouvement.DateMouvement));
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task NewReturnAsync()
    {
        await Shell.Current.GoToAsync($"{nameof(SortieStockPage)}?mode=retour");
    }

    [RelayCommand]
    private async Task OpenReturnAsync(MouvementStock movement)
    {
        await Shell.Current.GoToAsync(nameof(MouvementDetailPage), new Dictionary<string, object>
        {
            ["mouvement"] = movement
        });
    }
}
