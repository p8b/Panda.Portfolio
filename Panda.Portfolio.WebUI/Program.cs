using System.Security.Cryptography.X509Certificates;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

using MudBlazor.Services;

using Panda.BlazorCore.Services;
using Panda.BlazorCore.Services.Interfaces;
using Panda.Portfolio.WebUI.DataProtection;

using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddSerilog();

try
{
    Log.Information("Application starting");

    ConfigureServices();
    ConfigureRequests();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

void ConfigureRequests()
{
    var App = builder.Build();
    if (!App.Environment.IsDevelopment())
    {
        App.UseExceptionHandler("/Error");
        App.UseHsts();
    }

    App.UseHttpsRedirection();

    App.UseStaticFiles();

    App.UseRouting();

    App.MapBlazorHub();
    App.MapFallbackToPage("/_Host");

    App.Run();
}
void ConfigureServices()
{
    var Services = builder.Services;
    string? certPass = builder.Configuration["CertificatePassword"];
    if (!string.IsNullOrWhiteSpace(certPass))
    {
        Services.AddDbContext<DataProtectionDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DataProtectionDatabase")));
        string certPath = Directory.GetCurrentDirectory() + "\\certificate.pfx";

        // Create a collection object and populate it using the PFX file
        var collection = new X509Certificate2Collection();
        collection.Import(certPath, certPass, X509KeyStorageFlags.PersistKeySet);
        var cert = collection.FirstOrDefault()!;

        Services.AddDataProtection()
            .SetApplicationName("p8b-portfolio")
            .PersistKeysToDbContext<DataProtectionDbContext>()
            .ProtectKeysWithCertificate(cert);
    }

    Services.AddMudServices();

    Services.AddRazorPages(options => options.RootDirectory = "/App");

    Services.AddServerSideBlazor();

    Services.AddTransient<IJSService, JSService>();
}