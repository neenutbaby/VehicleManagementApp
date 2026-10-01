namespace VehicleManagement.Web.Models
{
    public class VehicleCategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // WeightRange: [MinWeightKg, MaxWeightKg)
         public decimal MinWeightKg { get; set; }

        // MaxWeightKg = null => no upper bound
        public decimal? MaxWeightKg { get; set; }

        public string Icon { get; set; } = string.Empty;
        
    }
}
