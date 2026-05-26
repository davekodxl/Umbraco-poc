
using Microsoft.AspNetCore.DataProtection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);


var keyPath = Path.Combine(builder.Environment.ContentRootPath,
    "App_Data", "DataProtection-Keys");

// Log to confirm path at startup
Console.WriteLine($"[DataProtection] Key path: {keyPath}");
Console.WriteLine($"[DataProtection] Directory exists: {Directory.Exists(keyPath)}");

// Create it if missing — don't rely on manual creation
Directory.CreateDirectory(keyPath);

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keyPath))
    .SetApplicationName("itweb.kodxl.com");


builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddComposers()
    .Build();

WebApplication app = builder.Build();


await app.BootUmbracoAsync();


app.UseUmbraco()
    .WithMiddleware(u =>
    {
        u.UseBackOffice();
        u.UseWebsite();
    })
    .WithEndpoints(u =>
    {
        u.UseBackOfficeEndpoints();
        u.UseWebsiteEndpoints();
    });

await app.RunAsync();
