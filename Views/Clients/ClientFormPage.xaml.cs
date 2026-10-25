using ScaffTrack1.ViewModels.Clients;

namespace ScaffTrack1.Views.Clients;

public partial class ClientFormPage : ContentPage
{
    public ClientFormPage(ClientsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
