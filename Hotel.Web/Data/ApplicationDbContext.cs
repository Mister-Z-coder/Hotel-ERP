using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Models;
using Hotel.Services;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbConnection GetConnection()
        {
            return Database.GetDbConnection();
        }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Normal> Normals { get; set; }
        public DbSet<Resident> Residents { get; set; }
        public DbSet<Approvisionnement> Approvisionnements { get; set; }
        public DbSet<Ajout> Ajouts { get; set; }
        public DbSet<Agent> Agents { get; set; }
        public DbSet<Chambre> Chambres { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Devise> Devises { get; set; }
        public DbSet<Taux> Tauxes { get; set; }
        public DbSet<TypeReglement> TypeReglements { get; set; }
        public DbSet<ModeReglement> ModeReglements { get; set; }
        public DbSet<HistoriqueReglement> HistoriqueReglements { get; set; }
        public DbSet<PosteTravail> PosteTravails { get; set; }
        public DbSet<Caisse> Caisses { get; set; }
        public DbSet<Reglement> Reglements { get; set; }
        public DbSet<ReglementHistorique> ReglementHistoriques { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Brasserie> Brasseries { get; set; }
        public DbSet<CategorieBoisson> CategorieBoissons { get; set; }
        public DbSet<Boisson> Boissons { get; set; }
        public DbSet<HPrixUnitSBBoisson> HPrixUnitSBBoissons { get; set; }
        public DbSet<HPrixUnitLBBoisson> HPrixUnitLBBoissons { get; set; }
        public DbSet<HPrixUnitAliment> hPrixUnitAliments { get; set; }
        public DbSet<Aliment> Aliments { get; set; }
        public DbSet<Fournisseur> Fournisseurs { get; set; }
        public DbSet<TypeMouvement> TypeMouvements { get; set; }
        public DbSet<Mouvement> Mouvements { get; set; }
        public DbSet<Fournir> Fournirs { get; set; }
        public DbSet<Procurer> Procurers { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Demande> Demandes { get; set; }
        public DbSet<MaintenanceChambre> MaintenanceChambres { get; set; }
        public DbSet<FinMaintenanceChambre> FinMaintenanceChambres { get; set; }
        public DbSet<ShiftTravail> ShiftTravails { get; set; }
        public DbSet<FinShiftTravail> FinShiftTravails { get; set; }
        public DbSet<TableClient> TableClients { get; set; }
        public DbSet<Commande> Commandes { get; set; }
        public DbSet<CommandeTableClient> CommandeTableClients { get; set; }
        public DbSet<Concerner> Concerners { get; set; }
        public DbSet<Contenir> Contenirs { get; set; }
        public DbSet<CommandesService> CommandesServices { get; set; }
        public DbSet<ReglementCommande> ReglementCommandes { get; set; }
        public DbSet<ReglementCommandeHistorique> ReglementCommandeHistoriques { get; set; }
        public DbSet<ReglementReservation> ReglementReservations { get; set; }
        public DbSet<DetailsCommande> DetailsCommandes { get; set; }
        public DbSet<DetailsCommandeHistorique> DetailsCommandeHistoriques { get; set; }
        public DbSet<DetailsLivraison> DetailsLivraisons { get; set; }
        public DbSet<DetailsMouvement> DetailsMouvements { get; set; }
        public DbSet<DetailsSortie> DetailsSorties { get; set; }
        public DbSet<DetailsDemande> DetailsDemandes { get; set; }
        public DbSet<DetailsFacture> DetailsFactures { get; set; }
        public DbSet<DetailsFactureCaution> DetailsFactureCautions { get; set; }
        public DbSet<DetailsFactureReservation> DetailsFactureReservations { get; set; }
        public DbSet<DetailsReglement> DetailsReglements { get; set; }
        public DbSet<Configuration> Configurations { get; set; }
        public DbSet<Inclure> Inclures { get; set; }
        public DbSet<Comprendre> Comprendres { get; set; }
        public DbSet<Comporter> Comporters { get; set; }
        public DbSet<Mouvoir> Mouvoirs { get; set; }
        public DbSet<Bouger> Bougers { get; set; }
        public DbSet<SortiePourAjout> SortiePourAjouts { get; set; }
        public DbSet<Declassement> Declassements { get; set; }
        public DbSet<DetailsDeclassement> DetailsDeclassements { get; set; }
        public DbSet<Livraison> Livraisons { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Agent>().HasData(
                new Agent
                {
                    AgentId = 1,
                    NomAgent = "Bakatuasa",
                    PrenomAgent = "Jeremie",
                    SexeAgent = "M",
                    Lieu_nais_Agent = "KINSHASA",
                    Date_nais_Agent = new DateTime(2001, 11, 23),
                    Nationalite_Agent = "CONGOLAISE",
                    Fonction_Agent = "ADMIN",
                    PhotoPath = "Aucunephotopour l'agent"

                    //PhotoAgent = "C:/Users/Jeremie Bakatuasa/source/repos/Hotel/Hotel.Web/wwwroot/images/th.jpeg"
                });
            modelBuilder.Entity<Agent>().HasData(
                new Agent
                {
                    AgentId = 2,
                    NomAgent = "Bankoledi",
                    PrenomAgent = "Jean",
                    SexeAgent = "M",
                    Lieu_nais_Agent = "KINSHASA",
                    Date_nais_Agent = new DateTime(2001, 11, 23),
                    Nationalite_Agent = "CONGOLAISE",
                    Fonction_Agent = "CAISSIER",
                    PhotoPath = "Aucunephotopour l'agent"
                    //PhotoAgent = "C:/Users/Jeremie Bakatuasa/source/repos/Hotel/Hotel.Web/wwwroot/images/th.jpeg"
                });

            modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Username = "admin", Password = PasswordHasher.HashPassword("admin123"), Role = "ADMIN", AgentId = 1 },
            new User { Id = 2, Username = "user", Password = PasswordHasher.HashPassword("123"), Role = "CAISSIER", AgentId = 2 }
            );
            modelBuilder.Entity<DetailsFacture>()
                .Property(r => r.PU)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<ReglementCommande>()
                .Property(r => r.MontantAffecte)
                .HasColumnType("decimal(18,2)");


            modelBuilder.Entity<ReglementCommandeHistorique>()
                .Property(r => r.MontantAffecte)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Commande>()
                .Property(r => r.MontantTotal)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Livraison>()
                .Property(l => l.MontantLivraison)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<DetailsLivraison>()
                .Property(l => l.MontantLivraison)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<DetailsFacture>()
                .Property(r => r.Montanttotal)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<DetailsReglement>()
                .Property(r => r.MontantReglement)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<DetailsFactureReservation>()
                .Property(r => r.Montant)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<DetailsFactureCaution>()
                .Property(r => r.Montant)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Reglement>()
                .Property(r => r.MontantReglement)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<ReglementHistorique>()
                .Property(r => r.MontantReglement)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<HistoriqueReglement>()
                .HasNoKey();

            modelBuilder.Entity<HistoriqueReglement>()
                .Property(r => r.Montant)
                .HasColumnType("decimal(18,2)");
            //modelBuilder.Entity<TypeReglement>().HasData(
            //    new TypeReglement{TitreTypeReglement = "EN ESPECE"},
            //    new TypeReglement{TitreTypeReglement = "PAR VIREMENT"},
            //    new TypeReglement { TitreTypeReglement = "PAR CHEQUE"},
            //    new TypeReglement { TitreTypeReglement = "PAR E-MONEY"});

            //modelBuilder.Entity<ModeReglement>().HasData(
            //    new ModeReglement { TitreModeReglement = "EN UNE SEULE FOIS" },
            //    new ModeReglement { TitreModeReglement = "ACOMPTE" });


            //modelBuilder.Entity<Client>().HasData(
            //        new Client
            //        {
            //            ClientId = 1,
            //            NomClient = "Jeremie",
            //            SexeClient = "M",
            //            TypeClient = "Normal"
            //        },
            //        new Client
            //        {
            //            ClientId = 2,
            //            NomClient = "Jean",
            //            SexeClient = "M",
            //            TypeClient = "Resident"
            //        },
            //        new Client
            //        {
            //            ClientId = 3,
            //            NomClient = "John",
            //            SexeClient = "M",
            //            TypeClient = "Normal"
            //        },
            //        new Client
            //        {
            //            ClientId = 4,
            //            NomClient = "Juliette",
            //            SexeClient = "F",
            //            TypeClient = "Normal"
            //        },
            //        new Client
            //        {
            //            ClientId = 5,
            //            NomClient = "Priscille",
            //            SexeClient = "F",
            //            TypeClient = "Normal"
            //        }
            //    );

            modelBuilder.Entity<Client>().ToTable("Clients");
            modelBuilder.Entity<Normal>().ToTable("Normals");
            modelBuilder.Entity<Resident>().ToTable("Residents");
            modelBuilder.Entity<Approvisionnement>().ToTable("Approvisionnements");
            modelBuilder.Entity<Ajout>().ToTable("Ajouts");
            modelBuilder.Entity<PosteTravail>().ToTable("PosteTravails");
            modelBuilder.Entity<Caisse>().ToTable("Caisses");
            modelBuilder.Entity<MaintenanceChambre>().ToTable("MaintenanceChambres");
            modelBuilder.Entity<FinMaintenanceChambre>().ToTable("FinMaintenanceChambres");
            modelBuilder.Entity<ShiftTravail>().ToTable("ShiftTravails");
            modelBuilder.Entity<FinShiftTravail>().ToTable("FinShiftTravails");
            modelBuilder.Entity<Commande>().ToTable("Commandes");
            modelBuilder.Entity<CommandeTableClient>().ToTable("CommandeTableClients");
            //modelBuilder.Entity<ReglementCommande>().ToTable("ReglementCommandes");
            modelBuilder.Entity<ReglementReservation>().ToTable("ReglementReservations");
            modelBuilder.Entity<CommandesService>().ToTable("CommandesServices");
            modelBuilder.Entity<SortiePourAjout>().ToTable("SortiePourAjouts");
            modelBuilder.Entity<Declassement>().ToTable("Declassements");
            modelBuilder.Entity<Livraison>().ToTable("Livraisons");
            modelBuilder.Entity<HistoriqueReglement>().ToTable("HistoriqueReglements");

            modelBuilder.Entity<ReglementCommande>()
        .HasKey(rc => new { rc.ReglementId, rc.CommandeId });

            modelBuilder.Entity<ReglementCommandeHistorique>()
        .HasKey(rc => new { rc.ReglementId, rc.CommandeId });

            modelBuilder.Entity<ReglementCommande>()
                .HasOne(rc => rc.Reglement)
                .WithMany(r => r.Commandes)
                .HasForeignKey(rc => rc.ReglementId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReglementCommande>()
                .HasOne(rc => rc.Commande)
                .WithMany(c => c.Reglements)
                .HasForeignKey(rc => rc.CommandeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Concerner>()
            .HasKey(c => new { c.AlimentId, c.CommandeId }); // Configurer la clé composite

            modelBuilder.Entity<Concerner>()
                .HasOne(c => c.Aliment)
                .WithMany() // Si un Aliment peut être lié à plusieurs Concerner
                .HasForeignKey(c => c.AlimentId);

            modelBuilder.Entity<Concerner>()
                .HasOne(c => c.Commande)
                .WithMany() // Si une Commande peut être liée à plusieurs Concerner
                .HasForeignKey(c => c.CommandeId);
            ///
            modelBuilder.Entity<Contenir>()
            .HasKey(c => new { c.BoissonId, c.CommandeId }); // Configurer la clé composite

            modelBuilder.Entity<Contenir>()
                .HasOne(c => c.Boisson)
                .WithMany() // Si un Aliment peut être lié à plusieurs Contenir
                .HasForeignKey(c => c.BoissonId);

            modelBuilder.Entity<Contenir>()
                .HasOne(c => c.Commande)
                .WithMany() // Si une Commande peut être liée à plusieurs Contenir
                .HasForeignKey(c => c.CommandeId);

            // Configuration de la clé composite pour HPrixUnitLBBoisson
            modelBuilder.Entity<HPrixUnitLBBoisson>()
                .HasKey(p => new { p.BoissonId, p.DateHisto }); // Clé composite

            // Configuration de la relation avec Boisson pour HPrixUnitLBBoisson
            modelBuilder.Entity<HPrixUnitLBBoisson>()
                .HasOne(p => p.Boisson)
                .WithMany(b => b.PrixLBs)
                .HasForeignKey(p => p.BoissonId);

            // Configuration de la clé composite pour HPrixUnitSBBoisson
            modelBuilder.Entity<HPrixUnitSBBoisson>()
                .HasKey(p => new { p.BoissonId, p.DateHisto }); // Clé composite

            // Configuration de la relation avec Boisson pour HPrixUnitSBBoisson
            modelBuilder.Entity<HPrixUnitSBBoisson>()
                .HasOne(p => p.Boisson)
                .WithMany(b => b.PrixSBs)
                .HasForeignKey(p => p.BoissonId);

            modelBuilder.Entity<HPrixUnitAliment>()
                .HasKey(p => new { p.AlimentId, p.DateHisto }); // Clé composite

            // Configuration de la relation avec Boisson pour HPrixUnitSBBoisson
            modelBuilder.Entity<HPrixUnitAliment>()
                .HasOne(p => p.Aliment)
                .WithMany(b => b.PrixAliments)
                .HasForeignKey(p => p.AlimentId);

            modelBuilder.Entity<Fournir>()
                .HasKey(f => new { f.AlimentId, f.FournisseurId }); // Clé composite

            // Configuration de la relation avec Boisson pour HPrixUnitSBBoisson
            modelBuilder.Entity<Fournir>()
                .HasOne(f => f.Aliment)
                .WithMany(a => a.Fournirs)
                .HasForeignKey(p => p.AlimentId);

            modelBuilder.Entity<Fournir>()
                .HasOne(f => f.Fournisseur)
                .WithMany(r => r.Fournirs)
                .HasForeignKey(p => p.FournisseurId);

            //
            modelBuilder.Entity<Procurer>()
                .HasKey(p => new { p.BoissonId, p.FournisseurId }); // Clé composite

            // Configuration de la relation avec Boisson pour HPrixUnitSBBoisson
            modelBuilder.Entity<Procurer>()
                .HasOne(f => f.Boisson)
                .WithMany(p => p.Procurers)
                .HasForeignKey(p => p.BoissonId);

            modelBuilder.Entity<Procurer>()
                .HasOne(f => f.Fournisseur)
                .WithMany(p => p.Procurers)
                .HasForeignKey(p => p.FournisseurId);

            //
            //
            modelBuilder.Entity<Inclure>()
                .HasKey(i => new { i.ServiceId, i.ReservationId }); // Clé composite
                                                                    // Configuration de la relation avec Boisson pour HPrixUnitSBBoisson
            modelBuilder.Entity<Comprendre>()
               .HasKey(c => new { c.DemandeId, c.BoissonId }); // Clé composite
            modelBuilder.Entity<Comporter>()
               .HasKey(c => new { c.DemandeId, c.AlimentId }); // Clé composite
            modelBuilder.Entity<Mouvoir>()
              .HasKey(m => new { m.MouvementId, m.AlimentId }); // Clé composite
            modelBuilder.Entity<Bouger>()
               .HasKey(b => new { b.MouvementId, b.BoissonId }); // Clé composite
            //modelBuilder.Entity<ReglementCommande>()
            //   .HasKey(c => new { c.ReglementId, c.CommandeId }); // Clé composite

            modelBuilder.Entity<Inclure>()
                .HasOne(s => s.Service)
                .WithMany(i => i.Inclures)
                .HasForeignKey(i => i.ServiceId);

            modelBuilder.Entity<Inclure>()
                .HasOne(r => r.Reservation)
                .WithMany(i => i.Inclures)
                .HasForeignKey(i => i.ReservationId);

            modelBuilder.Entity<Comprendre>()
               .HasOne(b => b.Boisson)
               .WithMany(c => c.Comprendres)
               .HasForeignKey(c => c.BoissonId);
            modelBuilder.Entity<Comprendre>()
               .HasOne(d => d.Demande)
               .WithMany(c => c.Comprendres)
               .HasForeignKey(c => c.DemandeId);

            modelBuilder.Entity<Bouger>()
              .HasOne(b => b.Boisson)
              .WithMany(b => b.Bougers)
              .HasForeignKey(b => b.BoissonId);
            modelBuilder.Entity<Bouger>()
               .HasOne(m => m.Mouvement)
               .WithMany(b => b.Bougers)
               .HasForeignKey(b => b.MouvementId);

            modelBuilder.Entity<Mouvoir>()
              .HasOne(a => a.Aliment)
              .WithMany(m => m.Mouvoirs)
              .HasForeignKey(a => a.AlimentId);
            modelBuilder.Entity<Mouvoir>()
               .HasOne(a => a.Mouvement)
               .WithMany(m => m.Mouvoirs)
               .HasForeignKey(a => a.MouvementId);

            modelBuilder.Entity<Comporter>()
              .HasOne(a => a.Aliment)
              .WithMany(c => c.Comporters)
              .HasForeignKey(c => c.AlimentId);
            modelBuilder.Entity<Comporter>()
               .HasOne(d => d.Demande)
               .WithMany(c => c.Comporters)
               .HasForeignKey(c => c.DemandeId);
            //modelBuilder.Entity<Boisson>().ToTable("Boissons");
            //modelBuilder.Entity<HPrixUnitSBBoisson>().ToTable("HPrixUnitSBBoissons");
            //modelBuilder.Entity<HPrixUnitLBBoisson>().ToTable("HPrixUnitLBBoissons");
        }
    }
}
