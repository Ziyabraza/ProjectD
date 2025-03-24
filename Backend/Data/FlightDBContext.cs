using Microsoft.EntityFrameworkCore;

namespace ProjectD
{
    public class FlightDBContext : DbContext
    {
        public FlightDBContext(DbContextOptions<FlightDBContext> options) : base(options) { }

        public DbSet<Flight> Flights { get; set; }  // Entity Framework zal deze map naar de tabel 'vluchten'
        public DbSet<Touchpoint> Touchpoints { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Hier wordt expliciet aangegeven dat de tabel in PostgreSQL 'vluchten' heet
            modelBuilder.Entity<Flight>().ToTable("vluchten");

            // Configuratie van de relatie tussen Flight en Touchpoint
            modelBuilder.Entity<Touchpoint>()
                .HasOne(t => t.Flight)
                .WithMany(f => f.Touchpoints)
                .HasForeignKey(t => t.FlightId);
        }
    }
}
