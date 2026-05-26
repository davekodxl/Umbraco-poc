
using Microsoft.AspNetCore.DataProtection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);


// Use logs folder as fallback — app pool can already write here
var keyPath = Path.Combine(builder.Environment.ContentRootPath,
    "App_Data", "DataProtection-Keys");

// Try App_Data first, fall back to logs if permissions fail
if (!Directory.Exists(keyPath))
{
    try 
    {
        Directory.CreateDirectory(keyPath);
        // Test write permission
        var testFile = Path.Combine(keyPath, "test.tmp");
        File.WriteAllText(testFile, "test");
        File.Delete(testFile);
    }
    catch
    {
        // Fallback to logs directory which is already writable
        keyPath = Path.Combine(builder.Environment.ContentRootPath, "logs", "dp-keys");
        Directory.CreateDirectory(keyPath);
    }
}


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
