using System.Data;
using VehicleManagement.Web.Data;
using VehicleManagement.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace VehicleManagement.Web.Services
{
    public class VehicleService
    {
        private readonly VehicleManagementDbContext db;
        private readonly BRCategory rules;

        public VehicleService(VehicleManagementDbContext db, BRCategory rules)
        {
            this.db = db;
            this.rules = rules;
        }

        public async Task<IReadOnlyList<VehicleList>> GetVehiclesAsync(
        string sort = "owner", string direction = "asc")
        {
            var vehicles = await db.Vehicles
                .AsNoTracking()
                .Include(v => v.Manufacturer)
                .ToListAsync();

            var categories = await db.VehicleCategories.AsNoTracking().ToListAsync();
            var descending = direction.Equals("desc", StringComparison.OrdinalIgnoreCase);

            IEnumerable<VehicleModel> ordered = sort.ToLowerInvariant() switch
            {
                "manufacturer" => descending
                    ? vehicles.OrderByDescending(v => v.Manufacturer!.Name)
                    : vehicles.OrderBy(v => v.Manufacturer!.Name),
                "year" => descending
                    ? vehicles.OrderByDescending(v => v.YearOfManufacture)
                    : vehicles.OrderBy(v => v.YearOfManufacture),
                "weight" => descending
                    ? vehicles.OrderByDescending(v => v.WeightKg)
                    : vehicles.OrderBy(v => v.WeightKg),
                _ => descending
                    ? vehicles.OrderByDescending(v => v.OwnerName)
                    : vehicles.OrderBy(v => v.OwnerName)
            };

            return ordered.Select(v =>
            {
                var category = rules.FindCategory(categories, v.WeightKg);
                return new VehicleList(
                    v.Id, v.OwnerName, v.Manufacturer!.Name, v.YearOfManufacture,
                    v.WeightKg, category?.Name ?? "Uncategorised", category?.Icon ?? "⚠️");
            }).ToList();
        }

        public async Task<VehicleModel?> GetAsync(int id) =>
            await db.Vehicles.Include(v => v.Manufacturer).FirstOrDefaultAsync(v => v.Id == id);

        public async Task AddAsync(VehicleModel vehicle)
        {
            ValidateWeight(vehicle.WeightKg);
            db.Vehicles.Add(vehicle);
            await db.SaveChangesAsync();
        }

        private static void ValidateWeight(decimal weight)
        {
            if (weight <= 0) throw new ArgumentException("Weight must be positive.");
            if (decimal.Round(weight, 2) != weight)
                throw new ArgumentException("Weight must have no more than two decimal places.");
        }
    }

    public record VehicleList(
    int Id,
    string OwnerName,
    string Manufacturer,
    int YearOfManufacture,
    decimal WeightKg,
    string CategoryName,
    string CategoryIcon);
}

