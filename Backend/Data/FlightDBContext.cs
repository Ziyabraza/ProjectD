using Microsoft.EntityFrameworkCore;

namespace ProjectD
{
    public class FlightDBContext : DbContext
    {
        public FlightDBContext(DbContextOptions<FlightDBContext> options) : base(options) { }

        public DbSet<Flight> Flights { get; set; }  
        public DbSet<Touchpoint> Touchpoints { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Hier wordt expliciet aangegeven dat de tabel in PostgreSQL 'vluchten' heet
            modelBuilder.Entity<Flight>().ToTable("Flights")
                .HasKey(f => f.Id);  // Definieer de primaire sleutel expliciet

            modelBuilder.Entity<Flight>()
                .Property(f => f.Id)
                .HasColumnName("Id");  // Zorg dat de kolomnaam overeenkomt met de kolom in PostgreSQL

            // Configuratie van de relatie tussen Flight en Touchpoint
            modelBuilder.Entity<Touchpoint>()
                .HasOne(t => t.Flight)
                .WithMany(f => f.Touchpoints)
                .HasForeignKey(t => t.FlightId);
        }
    }
}