using ScaffTrack.Shared.Models;
using ScaffTrack1.Services;

namespace ScaffTrack1.Views.Mouvements;

[QueryProperty(nameof(Mouvement), "mouvement")]
public partial class MouvementDetailPage : ContentPage
{
    private readonly IApiService _apiService;

    public MouvementDetailPage(IApiService apiService)
    {
        InitializeComponent();
        _apiService = apiService;
    }

    public MouvementStock? Mouvement
    {
        get => BindingContext as MouvementStock;
        set
        {
            BindingContext = value;
            ReturnDatePicker.Date = value?.DateRetourPrevue?.AddDays(1) ?? DateTime.Today.AddDays(7);
            ReturnDatePicker.MinimumDate = value?.DateMouvement.Date ?? DateTime.Today;
        }
    }

    private async void OnReturnClicked(object? sender, EventArgs e)
    {
        if (BindingContext is MouvementStock movement)
            await Shell.Current.GoToAsync($"{nameof(SortieStockPage)}?mode=retour&chantierId={movement.ChantierId}");
    }

    private async void OnExtendClicked(object? sender, EventArgs e)
    {
        if (BindingContext is not MouvementStock movement)
            return;

        if (ReturnDatePicker.Date <= movement.DateMouvement.Date)
        {
            await DisplayAlert("Date invalide", "La date de retour doit être postérieure à la date de sortie.", "OK");
            return;
        }

        if (!await _apiService.ExtendLocationAsync(movement.Id, ReturnDatePicker.Date))
        {
            await DisplayAlert("Prolongation impossible", "La date n’a pas pu être enregistrée. Vérifiez votre connexion et réessayez.", "OK");
            return;
        }

        const string marker = "Retour prévu : ";
        var observations = movement.Observations ?? string.Empty;
        var markerIndex = observations.LastIndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (markerIndex >= 0)
            observations = observations[..markerIndex].TrimEnd(' ', '•');
        movement.Observations = $"{observations} • Retour prévu : {ReturnDatePicker.Date:dd/MM/yyyy}".Trim(' ', '•');
        BindingContext = null;
        BindingContext = movement;
        await DisplayAlert("Location prolongée", "La nouvelle échéance de retour a été enregistrée.", "OK");
    }
}
