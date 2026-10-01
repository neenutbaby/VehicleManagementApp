using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace VehicleManagement.Web.ViewModes
{
    public class NewVehicleModel
    {
        [Required, StringLength(120)]
        [Display(Name = "Owner's Name")]
        public string OwnerName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Manufacturer")]
        public int? ManufacturerId { get; set; }

        [Required, Range(1886, 2026)]
        [Display(Name = "Year of Manufacture")]
        public int YearOfManufacture { get; set; }

        [Required, Range(typeof(decimal), "0.01", "999999999")]
        [Display(Name = "Weight (kg)")]
        public decimal WeightKg { get; set; }

        public IEnumerable<SelectListItem> Manufacturers { get; set; } = [];
    }
}
