using Microsoft.EntityFrameworkCore;
using VehicleManagement.Web.Models;


namespace VehicleManagement.Web.Data
{
    public class DatabaseInitializer
    {
        public static async Task InitializeAsync(VehicleManagementDbContext dbContext)
        {
            // Applies EF Core migrations. This is the preferred application startup path.
            await dbContext.Database.MigrateAsync();

            await SeedAsync(dbContext);
        }

        private static async Task SeedAsync(VehicleManagementDbContext dbContext)
        {
            if (!await dbContext.Manufacturers.AnyAsync())
            {
                dbContext.Manufacturers.AddRange(
                    new ManufacturerModel { Name = "Mazda" },
                    new ManufacturerModel { Name = "Mercedes" },
                    new ManufacturerModel { Name = "Honda" },
                    new ManufacturerModel { Name = "Ferrari" },
                    new ManufacturerModel { Name = "Toyota" });
            }

            if (!await dbContext.VehicleCategories.AnyAsync())
            {
                dbContext.VehicleCategories.AddRange(
                    new VehicleCategoryModel
                    {
                        Name = "Light",
                        MinWeightKg = 0m,
                        MaxWeightKg = 500m,
                        Icon = "🚗"
                    },
                    new VehicleCategoryModel
                    {
                        Name = "Medium",
                        MinWeightKg = 500m,
                        MaxWeightKg = 2500m,
                        Icon = "🚙"
                    },
                    new VehicleCategoryModel
                    {
                        Name = "Heavy",
                        MinWeightKg = 2500m,
                        MaxWeightKg = null,
                        Icon = "🚚"
                    });
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
