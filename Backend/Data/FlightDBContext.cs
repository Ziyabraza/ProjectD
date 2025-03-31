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
            modelBuilder.Entity<Flight>().HasKey(f => f.Id);
            modelBuilder.Entity<Touchpoint>()
                .HasOne(t => t.Flight)
                .WithMany(f => f.Touchpoints)
                .HasForeignKey(t => t.FlightId);
        }
    }
}