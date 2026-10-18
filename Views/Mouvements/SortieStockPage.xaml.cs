using ScaffTrack1.ViewModels.Mouvements;

namespace ScaffTrack1.Views.Mouvements;

public partial class SortieStockPage : ContentPage
{
    private readonly SortieStockViewModel _viewModel;

    public SortieStockPage(SortieStockViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadDataCommand.ExecuteAsync(null);
    }
}
