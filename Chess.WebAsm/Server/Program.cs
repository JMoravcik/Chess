using Chess.Core;
using Chess.Entities;
using Chess.Shared.Interfaces;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Chess.WebAsm.Server.Hubs;
using Chess.Shared;
using Microsoft.AspNetCore.SignalR;
using Chess.WebAsm.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddDbContext<IDatabase, ChessDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("Isonsoft.Chain.Api.Entities"));

});
builder.Services.AddChessCore();
builder.Services.AddSignalR()
    .AddHubOptions<ChessHub>(options =>
    {

        options.AddFilter<ChessHubFilter>();
    });

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddHostedService<ChessHubService>();
var app = builder.Build();
UpdateDatabase(app);
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
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


app.MapRazorPages();
app.MapHub<ChessHub>(Routes.ChessHub);
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();

void UpdateDatabase(IHost buildedApp)
{
    using var serviceScope = buildedApp.Services.CreateScope();
    using var databaseContext = new ChessDbContext();
    Console.WriteLine($"count of pending migrations: {databaseContext.Database.GetPendingMigrations().Count()}");
    databaseContext.Database.Migrate();
}