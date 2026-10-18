using Microsoft.EntityFrameworkCore;
using ScaffTrack.Shared.Models;

namespace ScaffTrack.Api.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(ScaffTrackDbContext context)
    {
        // Applique les migrations ou crée la base si elle n'existe pas
        await context.Database.EnsureCreatedAsync();

        if (await context.Utilisateurs.AnyAsync())
        {
            return; // Déjà initialisé
        }

        // 1. Utilisateurs
        var admin = new Utilisateur
        {
            Id = Guid.NewGuid(),
            Nom = "Bayihe",
            Prenom = "Joseph",
            Email = "admin@scafftrack.com",
            MotDePasseHash = "admin123", // Dans une vraie appli de prod, hacher avec BCrypt/Argon2
            Telephone = "+237 659 19 40 47",
            Role = UserRole.Admin,
            EstActif = true
        };

        var magasinier = new Utilisateur
        {
            Id = Guid.NewGuid(),
            Nom = "Kouassi",
            Prenom = "Marc",
            Email = "magasinier@scafftrack.com",
            MotDePasseHash = "magasin123",
            Telephone = "+237 670 11 22 33",
            Role = UserRole.Magasinier,
            EstActif = true
        };

        var chefChantier = new Utilisateur
        {
            Id = Guid.NewGuid(),
            Nom = "Diallo",
            Prenom = "Moussa",
            Email = "diallo@scafftrack.com",
            MotDePasseHash = "chantier123",
            Telephone = "+237 699 44 55 66",
            Role = UserRole.ChefChantier,
            EstActif = true
        };

        await context.Utilisateurs.AddRangeAsync(admin, magasinier, chefChantier);

        // 2. Catégories
        var catCadres = new Categorie { Id = Guid.NewGuid(), Nom = "Cadres & Échelles", Description = "Cadres de base, échelles de passage, portiques" };
        var catPlateaux = new Categorie { Id = Guid.NewGuid(), Nom = "Plateaux & Planchers", Description = "Planchers acier galvanisé et alu/bois antidérapants" };
        var catSecurite = new Categorie { Id = Guid.NewGuid(), Nom = "Garde-corps & Diagonales", Description = "Lisses de sécurité, diagonales de contreventement" };
        var catEtaiement = new Categorie { Id = Guid.NewGuid(), Nom = "Étaiement & Vérins", Description = "Vérins de pied à vis, platines, étals télescopiques" };

        await context.Categories.AddRangeAsync(catCadres, catPlateaux, catSecurite, catEtaiement);

        // 3. Matériels (Total: 1240 unités réparties : 780 en chantier, 420 disponibles, 40 en maintenance)
        var matCadreH = new Materiel
        {
            Id = Guid.NewGuid(),
            Reference = "CAD-H-2-100",
            Nom = "Cadre H 2.0m x 1.0m",
            Description = "Cadre métallique robuste pour structure verticale standard",
            CategorieId = catCadres.Id,
            QuantiteTotale = 400,
            QuantiteEnChantier = 250,
            QuantiteDisponible = 135,
            QuantiteEnMaintenance = 15,
            PrixLocationJourFCFA = 150,
            PrixRemplacementFCFA = 18500,
            Dimensions = "200cm x 100cm",
            PoidsKg = 12.5,
            Etat = EtatMateriel.Bon,
            PhotoUrl = "cadre_h.png"
        };

        var matPlateauAcier = new Materiel
        {
            Id = Guid.NewGuid(),
            Reference = "PLT-AC-300",
            Nom = "Plateau acier galva 3.0m x 0.3m",
            Description = "Plancher ajouré antidérapant avec ergots de verrouillage",
            CategorieId = catPlateaux.Id,
            QuantiteTotale = 350,
            QuantiteEnChantier = 220,
            QuantiteDisponible = 120,
            QuantiteEnMaintenance = 10,
            PrixLocationJourFCFA = 200,
            PrixRemplacementFCFA = 22000,
            Dimensions = "300cm x 30cm",
            PoidsKg = 14.0,
            Etat = EtatMateriel.Bon,
            PhotoUrl = "plateau_acier.png"
        };

        var matDiagonale = new Materiel
        {
            Id = Guid.NewGuid(),
            Reference = "DIAG-CV-200",
            Nom = "Diagonale de contreventement 2.5m",
            Description = "Tube acier galva avec mâchoires de serrage rapide",
            CategorieId = catSecurite.Id,
            QuantiteTotale = 250,
            QuantiteEnChantier = 160,
            QuantiteDisponible = 85,
            QuantiteEnMaintenance = 5,
            PrixLocationJourFCFA = 100,
            PrixRemplacementFCFA = 9500,
            Dimensions = "250cm",
            PoidsKg = 6.2,
            Etat = EtatMateriel.Bon,
            PhotoUrl = "diagonale.png"
        };

        var matVerin = new Materiel
        {
            Id = Guid.NewGuid(),
            Reference = "VER-VIS-60",
            Nom = "Vérin de pied à vis réglable 60cm",
            Description = "Platine de base filetée avec écrou de réglage haute résistance",
            CategorieId = catEtaiement.Id,
            QuantiteTotale = 240,
            QuantiteEnChantier = 150,
            QuantiteDisponible = 80,
            QuantiteEnMaintenance = 10,
            PrixLocationJourFCFA = 80,
            PrixRemplacementFCFA = 6500,
            Dimensions = "60cm x Ø38mm",
            PoidsKg = 4.5,
            Etat = EtatMateriel.Bon,
            PhotoUrl = "verin_pied.png"
        };

        await context.Materiels.AddRangeAsync(matCadreH, matPlateauAcier, matDiagonale, matVerin);

        // 4. Clients
        var client1 = new Client
        {
            Id = Guid.NewGuid(),
            NomEntreprise = "BTP Construction Plus",
            ContactNom = "Ing. Jean-Luc Ndombe",
            Telephone = "+237 699 10 20 30",
            Email = "contact@btpplus.cm",
            Adresse = "Quartier Bonapriso",
            Ville = "Douala",
            SoldeEncoursFCFA = 450000
        };

        var client2 = new Client
        {
            Id = Guid.NewGuid(),
            NomEntreprise = "Société Générale de Bâtiment (SGB)",
            ContactNom = "Mme Sophie Mballa",
            Telephone = "+237 677 88 99 00",
            Email = "s.mballa@sgb-cm.com",
            Adresse = "Bastos, Rue des Ambassades",
            Ville = "Yaoundé",
            SoldeEncoursFCFA = 800000
        };

        var client3 = new Client
        {
            Id = Guid.NewGuid(),
            NomEntreprise = "Artisans Façades Modernes",
            ContactNom = "M. Ibrahim Touré",
            Telephone = "+237 691 33 44 55",
            Email = "toure.facades@gmail.com",
            Adresse = "Zone Industrielle Bassa",
            Ville = "Douala",
            SoldeEncoursFCFA = 0
        };

        await context.Clients.AddRangeAsync(client1, client2, client3);

        // 5. Chantiers
        var chantier1 = new Chantier
        {
            Id = Guid.NewGuid(),
            Code = "CH-2026-001",
            Nom = "Résidence Les Palmiers",
            Adresse = "Boulevard de la Liberté, Akwa",
            Ville = "Douala",
            ClientId = client1.Id,
            ResponsableId = chefChantier.Id,
            DateDebut = DateTime.Today.AddDays(-15),
            DateFinPrevue = DateTime.Today.AddDays(45),
            Statut = StatutChantier.EnCours,
            MontantLocationPrevueFCFA = 1250000,
            MontantCautionFCFA = 300000,
            Remarques = "Ravalement de façade R+6. Échafaudage de 360m² requis."
        };

        var chantier2 = new Chantier
        {
            Id = Guid.NewGuid(),
            Code = "CH-2026-002",
            Nom = "Tour Horizon Plateau",
            Adresse = "Plateau administratif",
            Ville = "Yaoundé",
            ClientId = client2.Id,
            ResponsableId = chefChantier.Id,
            DateDebut = DateTime.Today.AddDays(-5),
            DateFinPrevue = DateTime.Today.AddDays(60),
            Statut = StatutChantier.EnCours,
            MontantLocationPrevueFCFA = 3600000,
            MontantCautionFCFA = 700000,
            Remarques = "Travaux de gros œuvre et crépissage."
        };

        var chantier3 = new Chantier
        {
            Id = Guid.NewGuid(),
            Code = "CH-2026-003",
            Nom = "Centre Commercial Grand Baie",
            Adresse = "Carrefour Ndokoti",
            Ville = "Douala",
            ClientId = client3.Id,
            ResponsableId = chefChantier.Id,
            DateDebut = DateTime.Today.AddDays(7),
            DateFinPrevue = DateTime.Today.AddDays(30),
            Statut = StatutChantier.EnAttente,
            MontantLocationPrevueFCFA = 850000,
            MontantCautionFCFA = 200000,
            Remarques = "Pose d'enseignes et vitrage extérieur."
        };

        await context.Chantiers.AddRangeAsync(chantier1, chantier2, chantier3);

        // 6. Mouvement de stock (Bon de livraison pour Chantier 1)
        var bl1 = new MouvementStock
        {
            Id = Guid.NewGuid(),
            NumeroBon = "BL-2026-001",
            Type = TypeMouvement.SortieChantier,
            Statut = StatutMouvement.Valide,
            ChantierId = chantier1.Id,
            CreeParId = magasinier.Id,
            DateMouvement = DateTime.Today.AddDays(-14),
            NomRecepteur = "Ing. Jean-Luc Ndombe",
            TelephoneRecepteur = "+237 699 10 20 30",
            Observations = "Livraison camion remorque N° LT-842-AB. Matériel réceptionné conforme.",
            Lignes = new List<LigneMouvement>
            {
                new() { Id = Guid.NewGuid(), MaterielId = matCadreH.Id, Quantite = 150, EtatAuMouvement = EtatMateriel.Bon },
                new() { Id = Guid.NewGuid(), MaterielId = matPlateauAcier.Id, Quantite = 120, EtatAuMouvement = EtatMateriel.Bon },
                new() { Id = Guid.NewGuid(), MaterielId = matDiagonale.Id, Quantite = 100, EtatAuMouvement = EtatMateriel.Bon },
                new() { Id = Guid.NewGuid(), MaterielId = matVerin.Id, Quantite = 80, EtatAuMouvement = EtatMateriel.Bon }
            }
        };

        await context.MouvementsStock.AddAsync(bl1);

        // 7. Alertes
        var alerte1 = new Alerte
        {
            Id = Guid.NewGuid(),
            Type = TypeAlerte.RetardRetour,
            Titre = "Retard restitution chantier",
            Message = "Le chantier Résidence Les Palmiers a 10 plateaux prévus en retour depuis hier.",
            EstResolue = false,
            DateCreation = DateTime.UtcNow.AddHours(-12),
            ChantierId = chantier1.Id
        };

        var alerte2 = new Alerte
        {
            Id = Guid.NewGuid(),
            Type = TypeAlerte.StockCritique,
            Titre = "Stock faible Vérins de pied",
            Message = "Il ne reste que 80 vérins disponibles en dépôt.",
            EstResolue = false,
            DateCreation = DateTime.UtcNow.AddDays(-1),
            MaterielId = matVerin.Id
        };

        await context.Alertes.AddRangeAsync(alerte1, alerte2);

        await context.SaveChangesAsync();
    }
}
