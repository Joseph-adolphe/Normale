namespace ScaffTrack.Shared.DTOs;

public class DashboardStatsDto
{
    public int TotalUnites { get; set; } = 1240;
    public int UnitesSurChantiers { get; set; } = 780;
    public int UnitesDisponibles { get; set; } = 420;
    public int UnitesEnMaintenance { get; set; } = 40;

    public int ChantiersActifs { get; set; } = 8;
    public int ChantiersEnAttente { get; set; } = 3;
    public int RetoursEnAttente { get; set; } = 4;
    public int AlertesNonResolues { get; set; } = 2;

    public decimal ChiffreAffairesMoisFCFA { get; set; } = 4850000;
    public decimal EncoursClientsFCFA { get; set; } = 1250000;

    public double TauxOccupation => TotalUnites > 0 
        ? Math.Round((double)UnitesSurChantiers / TotalUnites * 100, 1) 
        : 0;
}
