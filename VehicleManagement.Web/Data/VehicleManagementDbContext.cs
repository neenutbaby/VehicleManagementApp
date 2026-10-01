using Microsoft.EntityFrameworkCore;
using VehicleManagement.Web.Models;

namespace VehicleManagement.Web.Data
{
    public class VehicleManagementDbContext : DbContext
    {
        public VehicleManagementDbContext(DbContextOptions<VehicleManagementDbContext> options) : base(options)
        {
        }
        // DbSets for entities 
        public DbSet<VehicleModel> Vehicles => Set<VehicleModel>();
        public DbSet<ManufacturerModel> Manufacturers => Set<ManufacturerModel>();
        public DbSet<VehicleCategoryModel> VehicleCategories => Set<VehicleCategoryModel>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ManufacturerModel>(entity =>
            {
                entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
                entity.HasIndex(x => x.Name).IsUnique();
            });

            modelBuilder.Entity<VehicleModel>(entity =>
            {
                entity.Property(x => x.OwnerName).HasMaxLength(200).IsRequired();
                entity.Property(x => x.YearOfManufacture).IsRequired();
                entity.Property(x => x.WeightKg).HasPrecision(12, 2).IsRequired();

                entity.HasOne(x => x.Manufacturer)
                    .WithMany()
                    .HasForeignKey(x => x.ManufacturerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<VehicleCategoryModel>(entity =>
            {
                entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
                entity.Property(x => x.Icon).HasMaxLength(20).IsRequired();
                entity.Property(x => x.MinWeightKg).HasPrecision(12, 2).IsRequired();
                entity.Property(x => x.MaxWeightKg).HasPrecision(12, 2);
            });
        }
    }
}
