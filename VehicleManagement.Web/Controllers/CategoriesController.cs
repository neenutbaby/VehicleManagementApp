using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleManagement.Web.Data;
using VehicleManagement.Web.Models;
using VehicleManagement.Web.Services;
using VehicleManagement.Web.ViewModes;

namespace VehicleManagement.Web.Controllers
{
    public class CategoriesController(VehicleManagementDbContext dbContext, BRCategory rules) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var categories = rules.Sort(await dbContext.VehicleCategories.AsNoTracking().ToListAsync());
            ViewBag.Validation = rules.ValidateCategory(categories);
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create() => View(new NewCategoryModel { Icon = "🚗" });

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewCategoryModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var existingCategories = await dbContext.VehicleCategories.ToListAsync();

            var newCategory = new VehicleCategoryModel
            {
                Name = model.Name?.Trim(),
                MinWeightKg = model.MinWeightKg,
                MaxWeightKg = model.MaxWeightKg,
                Icon = model.Icon?.Trim()
            };

            // Validate including the new category
            existingCategories.Add(newCategory);
            var error = rules.ValidateCategory(existingCategories);
            if (error != null)
            {
                ModelState.AddModelError(string.Empty, error);
                return View(model);
            }

            // Add the new entity to the DbContext so it will be saved
            dbContext.VehicleCategories.Add(newCategory);
            await dbContext.SaveChangesAsync();

            TempData["Success"] = "Category created.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)               
        {
            var c = await dbContext.VehicleCategories.FindAsync(id);
            if (c is null) return NotFound();

            return View(new NewCategoryModel
            {
                Id = c.Id,
                Name = c.Name,
                MinWeightKg = c.MinWeightKg,
                MaxWeightKg = c.MaxWeightKg,
                Icon = c.Icon
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(NewCategoryModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var categories = await dbContext.VehicleCategories.ToListAsync();
            var existing = categories.FirstOrDefault(x => x.Id == model.Id);
            if (existing is null) return NotFound();

            existing.Name = model.Name.Trim();
            existing.MinWeightKg = model.MinWeightKg;
            existing.MaxWeightKg = model.MaxWeightKg;
            existing.Icon = model.Icon.Trim();

            var error = rules.ValidateCategory(categories);
            if (error != null)
            {
                ModelState.AddModelError(string.Empty, error);
                return View(model);
            }

            await dbContext.SaveChangesAsync();
            TempData["Success"] = "Category updated. Vehicle categories are recalculated from the new ranges.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await dbContext.VehicleCategories.FindAsync(id);
            if (category is null) return NotFound();

            var categories = await dbContext.VehicleCategories.Where(x => x.Id != id).ToListAsync();
            var error = rules.ValidateCategory(categories);
            if (error != null)
            {
                TempData["Error"] = $"Category cannot be deleted: {error}";
                return RedirectToAction(nameof(Index));
            }

            dbContext.VehicleCategories.Remove(category);
            await dbContext.SaveChangesAsync();
            TempData["Success"] = "Category deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
