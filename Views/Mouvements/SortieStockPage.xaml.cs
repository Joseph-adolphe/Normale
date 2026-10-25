using ScaffTrack1.ViewModels.Mouvements;

namespace ScaffTrack1.Views.Mouvements;

public partial class SortieStockPage : ContentPage, IQueryAttributable
{
    private readonly SortieStockViewModel _viewModel;

    public SortieStockPage(SortieStockViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        var mode = query.TryGetValue("mode", out var modeValue) ? modeValue?.ToString() : "sortie";
        var chantierId = query.TryGetValue("chantierId", out var idValue) ? idValue?.ToString() : null;
        _viewModel.SetMode(mode ?? "sortie", chantierId);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadDataCommand.ExecuteAsync(null);
    }
}
