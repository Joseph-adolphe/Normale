namespace ScaffTrack.Shared.Models;

public enum UserRole
{
    Admin,
    Magasinier,
    ChefChantier
}

public enum StatutChantier
{
    EnAttente,
    EnCours,
    Termine,
    Suspendu
}

public enum EtatMateriel
{
    Neuf,
    Bon,
    Use,
    Endommage,
    EnReparation,
    Perdu
}

public enum TypeMouvement
{
    SortieChantier,
    RetourStock,
    Transfert,
    Maintenance,
    Perte
}

public enum StatutMouvement
{
    Brouillon,
    Valide,
    EnLivraison,
    Cloture,
    Annule
}

public enum StatutFacture
{
    Brouillon,
    Emise,
    PartiellementPayee,
    Payee,
    EnRetard
}

public enum TypeAlerte
{
    RetardRetour,
    StockCritique,
    InspectionSecurite,
    Anomalie
}
