namespace ScaffTrack.Shared.DTOs;

public class DashboardStatsDto
{
    public int TotalUnites { get; set; }
    public int UnitesSurChantiers { get; set; }
    public int UnitesDisponibles { get; set; }
    public int UnitesEnMaintenance { get; set; }
    public int NombreCategories { get; set; }

    public int ChantiersActifs { get; set; }
    public int ChantiersEnAttente { get; set; }
    public int RetoursEnAttente { get; set; }
    public int AlertesNonResolues { get; set; }

    public decimal ChiffreAffairesMoisFCFA { get; set; }
    public decimal EncoursClientsFCFA { get; set; }

    public double TauxOccupation => Pourcentage(UnitesSurChantiers);
    public double TauxDisponibilite => Pourcentage(UnitesDisponibles);
    public double TauxMaintenance => Pourcentage(UnitesEnMaintenance);

    private double Pourcentage(int unites) => TotalUnites > 0
        ? Math.Round((double)unites / TotalUnites * 100, 1)
        : 0;
}
