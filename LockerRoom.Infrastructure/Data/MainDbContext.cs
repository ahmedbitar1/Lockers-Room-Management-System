using LockerRoom.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace LockerRoom.Infrastructure.Data
{
    public class MainDbContext : DbContext
    {
        public MainDbContext(DbContextOptions<MainDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<Locker> Lockers { get; set; }
        public DbSet<LockerGroup> LockerGroups { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<CurrencyRate> CurrencyRates { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<LockerLog> LockerLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // علاقة LockerGroup -> Lockers
            modelBuilder.Entity<LockerGroup>()
                .HasMany(g => g.Lockers)
                .WithOne(l => l.Group)
                .HasForeignKey(l => l.GroupID);

            // علاقة Locker -> Booking
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Locker)
                .WithMany()
                .HasForeignKey(b => b.LockerID);

            // علاقة CurrencyRate -> Booking
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Currency)
                .WithMany()
                .HasForeignKey(b => b.CurrencyID);
        }
    }
}
