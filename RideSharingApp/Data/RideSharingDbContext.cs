using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RideSharingApp.Models;

namespace RideSharingApp.Data
{
    public class RideSharingDbContext : IdentityDbContext
    {
        public RideSharingDbContext(DbContextOptions<RideSharingDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<RideBooking> RideBookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RideBooking>()
                .HasOne(r => r.PickupLocation)
                .WithMany()
                .HasForeignKey(r => r.PickupLocationID);

            modelBuilder.Entity<RideBooking>()
                .HasOne(r => r.DropoffLocation)
                .WithMany()
                .HasForeignKey(r => r.DropoffLocationID);

            modelBuilder.Entity<RideBooking>()
                .HasOne(r => r.Customer)
                .WithMany(c => c.RideBookings)
                .HasForeignKey(r => r.CustomerID);

            modelBuilder.Entity<RideBooking>()
                .HasOne(r => r.Driver)
                .WithMany(d => d.RideBookings)
                .HasForeignKey(r => r.DriverID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<RideBooking>()
                .HasOne(r => r.Payment)
                .WithMany()
                .HasForeignKey(r => r.PaymentID);

            modelBuilder.Entity<Driver>()
                .HasOne(d => d.Vehicle)
                .WithMany()
                .HasForeignKey(d => d.VehicleID);

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Customer)
                .WithMany(c => c.Feedbacks)
                .HasForeignKey(f => f.CustomerID);

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");
        }
    }
}