using Microsoft.EntityFrameworkCore;
using TaskManager.Models;

namespace TaskManager.Data
{
    public class TaskDbContext : DbContext
    {
        public DbSet<Utilisateur> Utilisateurs { get; set; }
        public DbSet<Tache> Taches { get; set; }
        public DbSet<Projet> Projets { get; set; }
        public DbSet<UtilisateurProjet> UtilisateursProjets { get; set; }
        public DbSet<TacheUtilisateur> TachesUtilisateurs { get; set; } // ✅ ajouté

        public TaskDbContext()
        {
            string dbPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "taskmanager.db");
            Console.WriteLine($"📌 Utilisation de la base de données : {dbPath}");
        }

        public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string dbPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "taskmanager.db");
                Console.WriteLine($"📌 Utilisation de la base de données : {dbPath}");
                optionsBuilder.UseSqlite($"Data Source={dbPath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Utilisateur>().ToTable("Utilisateurs");
            modelBuilder.Entity<Tache>().ToTable("Taches");
            modelBuilder.Entity<Projet>().ToTable("Projets");
            modelBuilder.Entity<UtilisateurProjet>().ToTable("UtilisateursProjets");
            modelBuilder.Entity<TacheUtilisateur>().ToTable("TachesUtilisateurs");

            // ✅ Relation Projet -> Tâches
            modelBuilder.Entity<Projet>()
                .HasMany(p => p.Taches)
                .WithOne(t => t.Projet)
                .HasForeignKey(t => t.ProjetId)
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ Relation Many-to-Many Utilisateurs <-> Projets
            modelBuilder.Entity<UtilisateurProjet>()
                .HasOne(up => up.Utilisateur)
                .WithMany(u => u.UtilisateursProjets)
                .HasForeignKey(up => up.UtilisateurId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UtilisateurProjet>()
                .HasOne(up => up.Projet)
                .WithMany(p => p.UtilisateursProjets)
                .HasForeignKey(up => up.ProjetId)
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ Projet -> Créateur
            modelBuilder.Entity<Projet>()
                .HasOne(p => p.Createur)
                .WithMany()
                .HasForeignKey(p => p.CreateurId)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ Relation Many-to-Many Tâches <-> Utilisateurs
            modelBuilder.Entity<TacheUtilisateur>()
                .HasKey(tu => new { tu.TacheId, tu.UtilisateurId });

            modelBuilder.Entity<TacheUtilisateur>()
                .HasOne(tu => tu.Tache)
                .WithMany(t => t.TachesUtilisateurs)
                .HasForeignKey(tu => tu.TacheId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TacheUtilisateur>()
                .HasOne(tu => tu.Utilisateur)
                .WithMany(u => u.TachesUtilisateurs)
                .HasForeignKey(tu => tu.UtilisateurId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
