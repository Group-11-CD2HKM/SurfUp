using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SurfBoardManager.Data;
using SurfBoardManager.Models;
using System.Globalization;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<SurfBoardManagerContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SurfBoardManagerContext") ?? throw new InvalidOperationException("Connection string 'SurfBoardManagerContext' not found.")));

builder.Services.AddDefaultIdentity<SurfUpUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<SurfBoardManagerContext>();

//builder.Services.AddMvc().AddMvcOptions(options => options.ModelBinderProviders.Insert(0, new DecimalModelBinder());

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

var defaultCulture = new CultureInfo("da-DK");
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(defaultCulture),
    SupportedCultures = new List<CultureInfo> { defaultCulture },
    SupportedUICultures = new List<CultureInfo> { defaultCulture }
};
app.UseRequestLocalization(localizationOptions);

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    SeedData.Initialize(services);
}

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
app.UseAuthentication();;

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
