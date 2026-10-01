using System.ComponentModel.DataAnnotations;

namespace VehicleManagement.Web.ViewModes
{
    public class NewCategoryModel
    {
        public int Id { get; set; }

        [Required, StringLength(80)]
        public string Name { get; set; } = string.Empty;

        [Range(typeof(decimal), "0", "999999999")]
        [Display(Name = "Minimum kg")]
        public decimal MinWeightKg { get; set; }

        [Display(Name = "Maximum kg (leave blank for unlimited)")]
        public decimal? MaxWeightKg { get; set; }

        [Required, StringLength(20)]
        public string Icon { get; set; } = string.Empty;
    }
}
