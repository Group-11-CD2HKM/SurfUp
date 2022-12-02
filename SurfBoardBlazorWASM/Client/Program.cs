using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SurfBoardBlazorWASM.Client;
using SurfBoardBlazorWASM.Client.Services;
using System;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("SurfBoardBlazorWASM.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();
builder.Services.AddHttpClient("SurfBoardBlazorWASM.PublicServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
builder.Services.AddHttpClient("WeatherClient", client => client.BaseAddress = new Uri("https://localhost:7175/"));


// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("SurfBoardBlazorWASM.ServerAPI"));
builder.Services.AddScoped<IBoardPostService,BoardPostService>();
builder.Services.AddScoped<WeatherService>();

// Handle state for rent
builder.Services.AddScoped<BoardPostRentState>();

builder.Services.AddApiAuthorization();

await builder.Build().RunAsync();
