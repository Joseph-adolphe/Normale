using ScaffTrack1.ViewModels.Historique;

namespace ScaffTrack1.Views.Historique;

public partial class HistoriquePage : ContentPage
{
    private readonly HistoriqueViewModel _viewModel;

    public HistoriquePage(HistoriqueViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadHistoriqueCommand.ExecuteAsync(null);
    }
}
