using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LibraryBackOffice.Models;

namespace LibraryBackOffice.Data
{
    public class LibraryContext : DbContext
    {
        public LibraryContext (DbContextOptions<LibraryContext> options)
            : base(options){}

        public DbSet<LibraryBackOffice.Models.Admin> Admin { get; set; } = default!;
        public DbSet<TypeLivre> TypeLivres { get; set; }
        public DbSet<TypeAbonnement> TypeAbonnements { get; set; }
        public DbSet<ModePaiement> ModePaiements { get; set; }
        public DbSet<Livre> Livres { get; set; }
        public DbSet<Utilisateur> Utilisateurs { get; set; }
        public DbSet<HistoriqueAbonnement> HistoriqueAbonnements { get; set; }
        public DbSet<HistoriqueLecture> HistoriqueLectures { get; set; }
        public DbSet<HistoriquePaiementLivre> HistoriquePaiementLivres { get; set; }
        public DbSet<ImportHistory> ImportHistories { get; set; }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TypeLivre>(e =>
            {
                e.ToTable("typeLivre");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id").UseIdentityColumn();
                e.Property(x => x.Type_Livre).HasColumnName("type_livre").HasMaxLength(255);
            });

            modelBuilder.Entity<TypeAbonnement>(e =>
            {
                e.ToTable("typeAbonnement");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id").UseIdentityColumn();
                e.Property(x => x.Type_Abonnement).HasColumnName("type_abonnement").HasMaxLength(255);
            });

            modelBuilder.Entity<ModePaiement>(e =>
            {
                e.ToTable("modePaiement");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id").UseIdentityColumn();
                e.Property(x => x.Mode).HasColumnName("mode").HasMaxLength(255);
            });

            modelBuilder.Entity<Livre>(e =>
            {
                e.ToTable("livre");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id").UseIdentityColumn();
                e.Property(x => x.Id_TypeLivre).HasColumnName("id_typelivre");
                e.Property(x => x.Titre).HasColumnName("titre").HasMaxLength(255);
                e.Property(x => x.Sous_Titre).HasColumnName("sous_titre").HasMaxLength(255);
                e.Property(x => x.Saison).HasColumnName("saison").HasMaxLength(100);
                e.Property(x => x.Auteur).HasColumnName("auteur").HasMaxLength(255);
                e.Property(x => x.Date_Edition).HasColumnName("date_edition").HasColumnType("date");
                e.Property(x => x.Description).HasColumnName("description").HasColumnType("nvarchar(max)");
                e.Property(x => x.Image).HasColumnName("image").HasColumnType("nvarchar(max)");
                e.Property(x => x.Document).HasColumnName("document").HasColumnType("nvarchar(max)");
                e.Property(x => x.Etat).HasColumnName("etat").HasDefaultValue(0);

                e.HasOne(x => x.TypeLivre)
                .WithMany(t => t.Livres)
                .HasForeignKey(x => x.Id_TypeLivre);
            });

            modelBuilder.Entity<Utilisateur>(e =>
            {
                e.ToTable("utilisateur");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id").UseIdentityColumn();
                e.Property(x => x.Nom).HasColumnName("nom").HasMaxLength(255);
                e.Property(x => x.Prenom).HasColumnName("prenom").HasMaxLength(255);
                e.Property(x => x.Telephone).HasColumnName("telephone").HasMaxLength(255);
                e.Property(x => x.Mail).HasColumnName("mail").HasMaxLength(255);
                e.Property(x => x.Mdp).HasColumnName("mdp").HasMaxLength(255);
                e.Property(x => x.Etat).HasColumnName("etat").HasDefaultValue(0);
                e.Property(x => x.DateEntree).HasColumnName("dateentree").HasColumnType("date");
            });

            modelBuilder.Entity<HistoriqueAbonnement>(e =>
            {
                e.ToTable("historiqueabonnement");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id").UseIdentityColumn();
                e.Property(x => x.Date_Paiement).HasColumnName("date_paiement").HasColumnType("date");
                e.Property(x => x.Id_TypeAbonnement).HasColumnName("id_typeabonnement");
                e.Property(x => x.Id_ModePaiement).HasColumnName("id_modepaiement");
                e.Property(x => x.Id_Utilisateur).HasColumnName("id_utilisateur");
                e.Property(x => x.Date_Expiration).HasColumnName("date_expiration").HasColumnType("date");

                e.HasOne(x => x.TypeAbonnement)
                .WithMany(t => t.HistoriqueAbonnements)
                .HasForeignKey(x => x.Id_TypeAbonnement);

                e.HasOne(x => x.ModePaiement)
                .WithMany(m => m.HistoriqueAbonnements)
                .HasForeignKey(x => x.Id_ModePaiement);

                e.HasOne(x => x.Utilisateur)
                .WithMany(u => u.HistoriqueAbonnements)
                .HasForeignKey(x => x.Id_Utilisateur);
            });

            modelBuilder.Entity<HistoriqueLecture>(e =>
            {
                e.ToTable("historiquelecture");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id").UseIdentityColumn();
                e.Property(x => x.Date_Lecture).HasColumnName("date_lecture").HasColumnType("date");
                e.Property(x => x.Id_Livre).HasColumnName("id_livre");
                e.Property(x => x.Id_Utilisateur).HasColumnName("id_utilisateur");

                e.HasOne(x => x.Livre)
                .WithMany(l => l.HistoriqueLectures)
                .HasForeignKey(x => x.Id_Livre);

                e.HasOne(x => x.Utilisateur)
                .WithMany(u => u.HistoriqueLectures)
                .HasForeignKey(x => x.Id_Utilisateur);
            });

            modelBuilder.Entity<HistoriquePaiementLivre>(e =>
            {
                e.ToTable("historiquepaiementlivre");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id").UseIdentityColumn();
                e.Property(x => x.Date_Lecture).HasColumnName("date_lecture").HasColumnType("date");
                e.Property(x => x.Id_Livre).HasColumnName("id_livre");
                e.Property(x => x.Id_Utilisateur).HasColumnName("id_utilisateur");
                e.Property(x => x.Prix).HasColumnName("prix");

                e.HasOne(x => x.Livre)
                .WithMany(l => l.HistoriquePaiementLivres)
                .HasForeignKey(x => x.Id_Livre);

                e.HasOne(x => x.Utilisateur)
                .WithMany(u => u.HistoriquePaiementLivres)
                .HasForeignKey(x => x.Id_Utilisateur);
            });

            modelBuilder.Entity<ImportHistory>(e =>
            {
                e.ToTable("importHistory");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id").UseIdentityColumn();
                e.Property(x => x.FileName).HasColumnName("file_name").HasMaxLength(255);
                e.Property(x => x.ImportDate).HasColumnName("import_date").HasColumnType("datetime2");
                e.Property(x => x.Notes).HasColumnName("notes").HasMaxLength(500);
                e.Property(x => x.IsDryRun).HasColumnName("is_dry_run").HasDefaultValue(false);
            });
        }
    }
}
