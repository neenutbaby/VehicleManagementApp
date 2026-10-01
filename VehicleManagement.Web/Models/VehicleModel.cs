using System.ComponentModel.DataAnnotations;

namespace VehicleManagement.Web.Models
{
    public class VehicleModel
    {
        public int Id { get; set; }

        [Required, StringLength(120)]
        public string OwnerName { get; set; } = string.Empty;

        [Required]
        public int ManufacturerId { get; set; }

        public ManufacturerModel? Manufacturer { get; set; }

        [Range(1886, 2100)]
        public int YearOfManufacture { get; set; }

        [Range(typeof(decimal), "0.01", "999999999")]
        public decimal WeightKg { get; set; }
    }
}
