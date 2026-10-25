using ScaffTrack1.ViewModels.Mouvements;

namespace ScaffTrack1.Views.Mouvements;

public partial class RestitutionsPage : ContentPage
{
    private readonly RestitutionsViewModel _viewModel;

    public RestitutionsPage(RestitutionsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadRestitutionsCommand.ExecuteAsync(null);
    }
}
