using Microsoft.EntityFrameworkCore;
using ScaffTrack.Shared.Models;

namespace ScaffTrack.Api.Data;

public class ScaffTrackDbContext : DbContext
{
    public ScaffTrackDbContext(DbContextOptions<ScaffTrackDbContext> options) : base(options)
    {
    }

    public DbSet<Utilisateur> Utilisateurs => Set<Utilisateur>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Categorie> Categories => Set<Categorie>();
    public DbSet<Materiel> Materiels => Set<Materiel>();
    public DbSet<Chantier> Chantiers => Set<Chantier>();
    public DbSet<MouvementStock> MouvementsStock => Set<MouvementStock>();
    public DbSet<LigneMouvement> LignesMouvement => Set<LigneMouvement>();
    public DbSet<Facture> Factures => Set<Facture>();
    public DbSet<Alerte> Alertes => Set<Alerte>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuration des décimaux (FCFA)
        modelBuilder.Entity<Client>()
            .Property(c => c.SoldeEncoursFCFA)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Materiel>()
            .Property(m => m.PrixLocationJourFCFA)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Materiel>()
            .Property(m => m.PrixRemplacementFCFA)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Chantier>()
            .Property(c => c.MontantLocationPrevueFCFA)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Chantier>()
            .Property(c => c.MontantCautionFCFA)
            .HasPrecision(18, 2);

        modelBuilder.Entity<LigneMouvement>()
            .Property(l => l.PenaliteFCFA)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Facture>()
            .Property(f => f.MontantLocationFCFA)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Facture>()
            .Property(f => f.MontantPenalitesFCFA)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Facture>()
            .Property(f => f.MontantPayeFCFA)
            .HasPrecision(18, 2);

        // Relations & Clés étrangères
        modelBuilder.Entity<Chantier>()
            .HasOne(c => c.Client)
            .WithMany(cl => cl.Chantiers)
            .HasForeignKey(c => c.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Chantier>()
            .HasOne(c => c.Responsable)
            .WithMany()
            .HasForeignKey(c => c.ResponsableId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<MouvementStock>()
            .HasOne(m => m.Chantier)
            .WithMany(c => c.Mouvements)
            .HasForeignKey(m => m.ChantierId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<LigneMouvement>()
            .HasOne(l => l.MouvementStock)
            .WithMany(m => m.Lignes)
            .HasForeignKey(l => l.MouvementStockId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<LigneMouvement>()
            .HasOne(l => l.Materiel)
            .WithMany()
            .HasForeignKey(l => l.MaterielId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Facture>()
            .HasOne(f => f.Client)
            .WithMany(c => c.Factures)
            .HasForeignKey(f => f.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Facture>()
            .HasOne(f => f.Chantier)
            .WithMany(c => c.Factures)
            .HasForeignKey(f => f.ChantierId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
