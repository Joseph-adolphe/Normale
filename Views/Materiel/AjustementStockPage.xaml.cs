using ScaffTrack1.ViewModels.Materiel;

namespace ScaffTrack1.Views.Materiel;

public partial class AjustementStockPage : ContentPage, IQueryAttributable
{
    private readonly AjustementStockViewModel _viewModel;

    public AjustementStockPage(AjustementStockViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("materielId", out var value) && Guid.TryParse(value?.ToString(), out var id))
            _viewModel.MaterielDemandeId = id;
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
