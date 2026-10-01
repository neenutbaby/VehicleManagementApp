using Microsoft.EntityFrameworkCore;
using VehicleManagement.Web.Data;
using VehicleManagement.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<VehicleManagementDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("VehicleManagementConnection")));

// Register application services
builder.Services.AddScoped<VehicleService>();
builder.Services.AddSingleton<BRCategory>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Vehicles}/{action=Index}/{id?}");

using var scope = app.Services.CreateScope();
VehicleManagementDbContext db = scope.ServiceProvider.GetRequiredService<VehicleManagementDbContext>();
await DatabaseInitializer.InitializeAsync(db);

app.Run();
