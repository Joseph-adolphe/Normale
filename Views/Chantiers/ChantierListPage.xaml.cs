using ScaffTrack1.ViewModels.Chantiers;

namespace ScaffTrack1.Views.Chantiers;

public partial class ChantierListPage : ContentPage
{
    private readonly ChantierListViewModel _viewModel;

    public ChantierListPage(ChantierListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadChantiersCommand.ExecuteAsync(null);
    }
}
