using System.Net;
using System.Security.Cryptography;
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
    Services.AddDataProtection()
        .ProtectKeysWithCertificate(buildSelfSignedServerCertificate());
    Services.AddMudServices();

    Services.AddRazorPages(options => options.RootDirectory = "/App");

    Services.AddServerSideBlazor();

    Services.AddTransient<IJSService, JSService>();
}
X509Certificate2 buildSelfSignedServerCertificate()
{
    const string CertificateName = "p8b.uk";
    SubjectAlternativeNameBuilder sanBuilder = new();
    sanBuilder.AddIpAddress(IPAddress.Loopback);
    sanBuilder.AddIpAddress(IPAddress.IPv6Loopback);
    sanBuilder.AddDnsName("test.p8b.uk");
    sanBuilder.AddDnsName(Environment.MachineName);

    X500DistinguishedName distinguishedName = new($"CN={CertificateName}");

    using RSA rsa = RSA.Create(2048);
    var request = new CertificateRequest(distinguishedName, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

    request.CertificateExtensions.Add(
        new X509KeyUsageExtension(X509KeyUsageFlags.DataEncipherment | X509KeyUsageFlags.KeyEncipherment | X509KeyUsageFlags.DigitalSignature, false));

    request.CertificateExtensions.Add(
       new X509EnhancedKeyUsageExtension(
           new OidCollection { new Oid("1.3.6.1.5.5.7.3.1") }, false));

    request.CertificateExtensions.Add(sanBuilder.Build());

    var certificate = request.CreateSelfSigned(new DateTimeOffset(DateTime.UtcNow.AddDays(-1)), new DateTimeOffset(DateTime.UtcNow.AddDays(3650)));
    certificate.FriendlyName = CertificateName;

    return new X509Certificate2(certificate.Export(X509ContentType.Pfx, "WeNeedASaf3rPassword"), "WeNeedASaf3rPassword", X509KeyStorageFlags.MachineKeySet);
}