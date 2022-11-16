using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.ApiAuthorization.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurfUpLibary;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Stop circular reference from being traversed, when returning an object with circular references. (i.e "return Ok(bordpost);")
builder.Services.AddControllers()
    .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHttpClient("weatherApiClient", client => client.BaseAddress = new Uri("https://api.openweathermap.org"));

//builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<SurfBoardManagerContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SurfBoardManagerContext") ?? throw new InvalidOperationException("Connection string 'SurfBoardManagerContext' not found.")));

builder.Services.AddIdentity<SurfUpUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<SurfBoardManagerContext>()
    .AddTokenProvider<DataProtectorTokenProvider<SurfUpUser>>(TokenOptions.DefaultProvider);

builder.Services.AddIdentityServer()
    .AddApiAuthorization<SurfUpUser, SurfBoardManagerContext>();

builder.Services.AddAuthentication()
    .AddIdentityServerJwt();

builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = ApiVersion.Default;
    options.ReportApiVersions = true;
}

);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
}

app.UseHttpsRedirection();
//app.Use(async (HttpContext x, RequestDelegate y) =>
//{
//    // For debugging, place a debug point on next line.
//    await y.Invoke(x);
//});
app.UseIdentityServer();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
