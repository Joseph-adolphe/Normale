using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScaffTrack.Shared.Models;
using ScaffTrack1.Services;
using ScaffTrack1.ViewModels.Base;

namespace ScaffTrack1.ViewModels.Materiel;

public partial class MaterielListViewModel : BaseViewModel
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private ObservableCollection<ScaffTrack.Shared.Models.Materiel> materiels = new();

    [ObservableProperty]
    private string searchText = string.Empty;

    public MaterielListViewModel(IApiService apiService)
    {
        _apiService = apiService;
        Title = "Parc Matériel";
    }

    [RelayCommand]
    public async Task LoadMaterielAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            var list = await _apiService.GetMaterielsAsync(SearchText);
            Materiels.Clear();
            foreach (var m in list)
            {
                Materiels.Add(m);
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
        await LoadMaterielAsync();
    }
}
