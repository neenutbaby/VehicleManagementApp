
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VehicleManagement.Web.Data;
using VehicleManagement.Web.Models;
using VehicleManagement.Web.Services;
using VehicleManagement.Web.ViewModes;

public class VehiclesController(VehicleManagementDbContext dbContext, VehicleService vehicles) : Controller
{
    
    // GET: VEHICLEMODELS
    public async Task<IActionResult> Index(string sort = "owner", string direction = "asc")
    {
        var model = await vehicles.GetVehiclesAsync(sort, direction);
        ViewBag.Sort = sort;
        ViewBag.Direction = direction;
        return View(model);
    }

    // GET: VEHICLEMODELS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var vehiclemodel = await dbContext.Vehicles
            .FirstOrDefaultAsync(m => m.Id == id);
        if (vehiclemodel == null)
        {
            return NotFound();
        }

        return View(vehiclemodel);
    }

    // GET: VEHICLEMODELS/Create
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        return View(await BuildCreateModelAsync());
    }

    // POST: VEHICLEMODELS/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NewVehicleModel model)
    {
        if (model.WeightKg > 0 && decimal.Round(model.WeightKg, 2) != model.WeightKg)
            ModelState.AddModelError(nameof(model.WeightKg), "Weight supports a maximum of two decimal places.");
        if (!ModelState.IsValid)
            return View(await BuildCreateModelAsync(model));

        try
        {
            await vehicles.AddAsync(new VehicleModel
            {
                OwnerName = model.OwnerName.Trim(),
                ManufacturerId = model.ManufacturerId!.Value,
                YearOfManufacture = model.YearOfManufacture,
                WeightKg = model.WeightKg
            });
            TempData["Success"] = "Vehicle added successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex) when (ex is ArgumentException or DbUpdateException)
        {
            ModelState.AddModelError(string.Empty, "The vehicle could not be saved. Please check the entered information.");
            return View(await BuildCreateModelAsync(model));
        }
    }

    private async Task<NewVehicleModel> BuildCreateModelAsync(NewVehicleModel? model = null)
    {
        model ??= new NewVehicleModel { YearOfManufacture = DateTime.UtcNow.Year };
        model.Manufacturers = await dbContext.Manufacturers.AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
            .ToListAsync();
        return model;
    }

    // GET: VEHICLEMODELS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var vehiclemodel = await dbContext.Vehicles.FindAsync(id);
        if (vehiclemodel == null)
        {
            return NotFound();
        }
        return View(vehiclemodel);
    }

    // POST: VEHICLEMODELS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,OwnerName,ManufacturerId,Manufacturer,YearOfManufacture,WeightKg")] VehicleModel vehiclemodel)
    {
        if (id != vehiclemodel.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                dbContext.Update(vehiclemodel);
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VehicleModelExists(vehiclemodel.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(vehiclemodel);
    }

    // GET: VEHICLEMODELS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var vehiclemodel = await dbContext.Vehicles
            .FirstOrDefaultAsync(m => m.Id == id);
        if (vehiclemodel == null)
        {
            return NotFound();
        }

        return View(vehiclemodel);
    }

    // POST: VEHICLEMODELS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var vehiclemodel = await dbContext.Vehicles.FindAsync(id);
        if (vehiclemodel != null)
        {
            dbContext.Vehicles.Remove(vehiclemodel);
        }

        await dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool VehicleModelExists(int? id)
    {
        return dbContext.Vehicles.Any(e => e.Id == id);
    }
}
