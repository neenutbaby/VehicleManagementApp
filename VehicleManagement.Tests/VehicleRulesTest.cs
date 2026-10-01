using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehicleManagement.Web.Services;
using VehicleManagement.Web.Models;

namespace VehicleManagement.Tests
{
    public class VehicleRulesTest
    {
        [Fact]
        public void Two_decimal_weight_is_supported()
        {
            var bRCategory = new BRCategory();
            var categories = new[]
            {
            new VehicleCategoryModel { Name = "All", MinWeightKg = 0, MaxWeightKg = null, Icon = "🚗" }
        };

            Assert.Equal("All", bRCategory.FindCategory(categories, 1750.56m)!.Name);
        }

    }
}
