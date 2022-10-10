using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SurfUpLibary;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<SurfBoardManagerContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SurfBoardManagerContext") ?? throw new InvalidOperationException("Connection string 'SurfBoardManagerContext' not found.")));

builder.Services.AddIdentity<SurfUpUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<SurfBoardManagerContext>()
    .AddTokenProvider<DataProtectorTokenProvider<SurfUpUser>>(TokenOptions.DefaultProvider);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
