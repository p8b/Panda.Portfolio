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
    Services.AddMudServices(config =>
    {
    });

    Services.AddRazorPages(options =>
    {
        options.RootDirectory = "/App";
    });

    Services.AddServerSideBlazor();

    Services.AddTransient<IJSService, JSService>();
}