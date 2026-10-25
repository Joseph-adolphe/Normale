using ScaffTrack1.ViewModels.Materiel;

namespace ScaffTrack1.Views.Materiel;

public partial class CreateMaterielPage : ContentPage
{
    private readonly CreateMaterielViewModel _viewModel;

    public CreateMaterielPage(CreateMaterielViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadCategoriesCommand.ExecuteAsync(null);
    }
}
