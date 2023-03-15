using System.Security.Cryptography.X509Certificates;

using Microsoft.AspNetCore.DataProtection;

using MudBlazor.Services;

using Panda.BlazorCore.Services;
using Panda.BlazorCore.Services.Interfaces;

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
    if (!string.IsNullOrWhiteSpace(builder.Configuration["CertificatePassword"]))
    {
        Services.AddDataProtection()
            .SetApplicationName("p8b-portfolio")
            .PersistKeysToFileSystem(new DirectoryInfo(builder.Configuration["PersistKeysLocation"] ?? "PersistKeys"))
            .ProtectKeysWithCertificate(
        new X509Certificate2("certificate.pfx", builder.Configuration["CertificatePassword"]));
    }

    Services.AddMudServices();

    Services.AddRazorPages(options => options.RootDirectory = "/App");

    Services.AddServerSideBlazor();

    Services.AddTransient<IJSService, JSService>();
}