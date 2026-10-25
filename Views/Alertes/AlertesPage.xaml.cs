using ScaffTrack1.ViewModels.Alertes;

namespace ScaffTrack1.Views.Alertes;

public partial class AlertesPage : ContentPage
{
    private readonly AlertesViewModel _viewModel;

    public AlertesPage(AlertesViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAlertesCommand.ExecuteAsync(null);
    }
}
