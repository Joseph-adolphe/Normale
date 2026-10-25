using ScaffTrack1.ViewModels.Locations;

namespace ScaffTrack1.Views.Locations;

public partial class LocationsPage : ContentPage
{
    private readonly LocationsViewModel _viewModel;

    public LocationsPage(LocationsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadLocationsCommand.ExecuteAsync(null);
    }
}
