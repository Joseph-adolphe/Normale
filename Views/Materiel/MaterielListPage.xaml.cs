using ScaffTrack1.ViewModels.Materiel;

namespace ScaffTrack1.Views.Materiel;

public partial class MaterielListPage : ContentPage
{
    private readonly MaterielListViewModel _viewModel;

    public MaterielListPage(MaterielListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadMaterielCommand.ExecuteAsync(null);
    }
}
