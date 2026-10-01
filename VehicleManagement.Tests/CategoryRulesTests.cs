using VehicleManagement.Web.Models;
using VehicleManagement.Web.Services;
namespace VehicleManagement.Tests;

public class CategoryRulesTests
{
    private readonly BRCategory bRCategory = new();

    public CategoryRulesTests()
    {   
        bRCategory = new BRCategory();
    }

    private static List<VehicleCategoryModel> ValidCategory() =>
    [
        new() { Id = 1, Name = "Light", MinWeightKg = 0, MaxWeightKg = 500, Icon = "🚗" },
        new() { Id = 2, Name = "Medium", MinWeightKg = 500, MaxWeightKg = 2500, Icon = "🚙" },
        new() { Id = 3, Name = "Heavy", MinWeightKg = 2500, MaxWeightKg = null, Icon = "🚚" }
    ];

    [Fact]
    public void Valid_category_rules_has_full_coverage() =>
    Assert.Null(bRCategory.ValidateCategory(ValidCategory()));

    [Fact]
    public void Weight_500kg_belongs_to_medium()
    {
        var category = bRCategory.FindCategory(ValidCategory(), 500m);
        Assert.Equal("Medium", category!.Name);
    }

    [Fact]
    public void Weight_2500kg_belongs_to_heavy()
    {
        var category = bRCategory.FindCategory(ValidCategory(), 2500m);
        Assert.Equal("Heavy", category!.Name);
    }

    [Fact]
    public void Gap_is_not_allowed()
    {
        var categories = ValidCategory();
        categories[1].MinWeightKg = 600;
        Assert.Contains("contiguous", bRCategory.ValidateCategory(categories)!);
    }

    [Fact]
    public void Overlap_is_not_allowed()
    {
        var categories = ValidCategory();
        categories[0].MaxWeightKg = 600;
        Assert.Contains("contiguous", bRCategory.ValidateCategory(categories)!);
    }

    [Fact]
    public void Vehicle_category_changes_when_range_change()
    {
        var categories = ValidCategory();
        Assert.Equal("Medium", bRCategory.FindCategory(categories, 2400m)!.Name);

        categories[1].MaxWeightKg = 2000;
        categories[2].MinWeightKg = 2000;

        Assert.Equal("Heavy", bRCategory.FindCategory(categories, 2200m)!.Name);
    }
}
