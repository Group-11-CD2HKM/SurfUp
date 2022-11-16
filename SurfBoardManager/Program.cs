using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SurfUpLibary;
using SurfBoardManager.Areas.Identity;
using SurfBoardManager.Models;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using SharedModel.Middleware;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
//Angiver vores connectionString til databasen 
builder.Services.AddDbContext<SurfBoardManagerContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SurfBoardManagerContext") ?? throw new InvalidOperationException("Connection string 'SurfBoardManagerContext' not found.")));

builder.Services.AddIdentity<SurfUpUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<SurfBoardManagerContext>().AddDefaultUI()
    .AddTokenProvider<DataProtectorTokenProvider<SurfUpUser>>(TokenOptions.DefaultProvider);


builder.Services.AddRazorPages();

builder.Services.AddHttpClient("client",options =>
{
options.BaseAddress = new Uri("https://localhost:7175/api/");
});


//builder.Services.AddMvc().AddMvcOptions(options => options.ModelBinderProviders.Insert(0, new DecimalModelBinder());

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication().AddGoogle(googleOptions =>
{
    googleOptions.ClientId = configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
});

builder.Services.AddAuthentication().AddFacebook(facebookOptions =>
{
    facebookOptions.AppId = configuration["Authentication:Facebook:AppId"];
    facebookOptions.AppSecret = configuration["Authentication:Facebook:AppSecret"];
});

var app = builder.Build();  

//Seeder databasen, hvis denne er tom.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    await SeedData.Initialize(services);
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

app.UseAuthentication();

app.UseAuthorization();

app.AnonIp();

app.Use(async (context, next) =>
{
    CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("en-US");
    CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");

    await next.Invoke(context);
});

//Default "Start side" når programmet køre
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
