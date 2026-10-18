using ScaffTrack1.ViewModels.Chantiers;

namespace ScaffTrack1.Views.Chantiers;

public partial class CreateChantierPage : ContentPage
{
    private readonly CreateChantierViewModel _viewModel;

    public CreateChantierPage(CreateChantierViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadClientsCommand.ExecuteAsync(null);
    }
}
