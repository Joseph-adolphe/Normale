using ScaffTrack1.Services;

namespace ScaffTrack1.Views.Materiel;

[QueryProperty(nameof(Materiel), "materiel")]
public partial class MaterielDetailPage : ContentPage
{
    private readonly IApiService _apiService;

    public MaterielDetailPage(IApiService apiService)
    {
        InitializeComponent();
        _apiService = apiService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Recharge les quantités après un ajustement de stock
        if (Materiel is not { } materiel)
            return;

        var aJour = (await _apiService.GetMaterielsAsync(materiel.Reference)).FirstOrDefault(m => m.Id == materiel.Id);
        if (aJour is not null)
        {
            BindingContext = null;
            Materiel = aJour;
        }
    }

    public ScaffTrack.Shared.Models.Materiel? Materiel
    {
        get => BindingContext as ScaffTrack.Shared.Models.Materiel;
        set
        {
            BindingContext = value;
            AfficherDisponibilite(value);
        }
    }

    private void AfficherDisponibilite(ScaffTrack.Shared.Models.Materiel? materiel)
    {
        if (materiel is null)
            return;

        var (libelle, texte, fond) = materiel.QuantiteDisponible switch
        {
            0 => ("Épuisé", "#B4232C", "#FDECEE"),
            _ when materiel.QuantiteDisponible < materiel.QuantiteTotale * 0.10 => ("Stock faible", "#9A6B0A", "#FFF4DD"),
            _ => ("Disponible", "#087A50", "#E7F7EF")
        };

        DisponibiliteLabel.Text = libelle;
        DisponibiliteLabel.TextColor = Color.FromArgb(texte);
        DisponibiliteBadge.BackgroundColor = Color.FromArgb(fond);
    }

    private async void OnAdjustClicked(object? sender, EventArgs e)
    {
        if (Materiel is { } materiel)
            await Shell.Current.GoToAsync($"{nameof(AjustementStockPage)}?materielId={materiel.Id}");
    }
}
