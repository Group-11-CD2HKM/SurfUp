using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.ApiAuthorization.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurfUpLibary;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using SharedModel.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("SurfBoardManagerContext");
builder.Services.AddDbContext<SurfBoardManagerContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<SurfUpUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<SurfBoardManagerContext>();

builder.Services.AddIdentityServer()
    .AddApiAuthorization<SurfUpUser, SurfBoardManagerContext>();

builder.Services.AddAuthentication()
    .AddIdentityServerJwt();

builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = ApiVersion.Default;
    options.ReportApiVersions = true;
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();
app.AnonIp();

app.UseIdentityServer();
app.UseAuthentication();
app.UseAuthorization();
app.Use(async (context, next) =>
{
    CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("en-US");
    CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");

    await next.Invoke(context);
});

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
