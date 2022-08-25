using Blazored.LocalStorage;
using Chess.AppDesign.ILinkers;
using Chess.AppDesign.IServices;
using Chess.WebAsm.Client;
using Chess.WebAsm.Client.Linkers;
using Chess.WebAsm.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<IJsService, JsService>();
builder.Services.AddScoped<MemoryService>();
builder.Services.AddSingleton<HttpService>();
builder.Services.AddScoped<IUserLinker, UserLinker>();
builder.Services.AddScoped<IChessHubService, ChessHubService>();
builder.Services.AddScoped<IUserService, UserService>();

await builder.Build().RunAsync();
