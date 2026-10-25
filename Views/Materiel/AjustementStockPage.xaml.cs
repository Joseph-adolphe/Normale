using ScaffTrack1.ViewModels.Materiel;

namespace ScaffTrack1.Views.Materiel;

public partial class AjustementStockPage : ContentPage
{
    private readonly AjustementStockViewModel _viewModel;

    public AjustementStockPage(AjustementStockViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadMaterielsCommand.ExecuteAsync(null);
    }

    private void OnTypeSelected(object? sender, EventArgs e)
    {
        if (sender is Button { Text: string type })
            _viewModel.SelectedType = type;
    }
}
