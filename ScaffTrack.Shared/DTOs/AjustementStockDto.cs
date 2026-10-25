namespace ScaffTrack.Shared.DTOs;

public class AjustementStockDto
{
    public string Type { get; set; } = string.Empty;
    public int Quantite { get; set; }
    public string Motif { get; set; } = string.Empty;
    public DateTime DateMouvement { get; set; } = DateTime.Today;
}
