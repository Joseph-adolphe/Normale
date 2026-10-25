using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScaffTrack.Shared.Models;
using ScaffTrack1.Services;
using ScaffTrack1.ViewModels.Base;
using ScaffTrack1.Views.Chantiers;

namespace ScaffTrack1.ViewModels.Chantiers;

public partial class ChantierListViewModel : BaseViewModel
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private ObservableCollection<Chantier> chantiers = new();

    [ObservableProperty]
    private string searchText = string.Empty;

    public ChantierListViewModel(IApiService apiService)
    {
        _apiService = apiService;
        Title = "Chantiers";
    }

    [RelayCommand]
    public async Task LoadChantiersAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            var list = await _apiService.GetChantiersAsync(SearchText);
            Chantiers.Clear();
            foreach (var c in list)
            {
                Chantiers.Add(c);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task SearchAsync()
    {
        await LoadChantiersAsync();
    }

    [RelayCommand]
    public async Task NouveauChantierAsync()
    {
        await Shell.Current.GoToAsync("CreateChantierPage");
    }

    [RelayCommand]
    private async Task OpenChantierAsync(Chantier chantier)
    {
        await Shell.Current.GoToAsync(nameof(ChantierDetailPage), new Dictionary<string, object>
        {
            ["chantier"] = chantier
        });
    }
}
