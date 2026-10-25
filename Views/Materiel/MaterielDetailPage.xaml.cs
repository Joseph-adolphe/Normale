namespace ScaffTrack1.Views.Materiel;

[QueryProperty(nameof(Materiel), "materiel")]
public partial class MaterielDetailPage : ContentPage
{
    public MaterielDetailPage()
    {
        InitializeComponent();
    }

    public ScaffTrack.Shared.Models.Materiel? Materiel
    {
        get => BindingContext as ScaffTrack.Shared.Models.Materiel;
        set => BindingContext = value;
    }
}
