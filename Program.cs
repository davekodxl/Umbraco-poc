
using Microsoft.AspNetCore.DataProtection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddComposers()
    .Build();

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(
        new DirectoryInfo(
            Path.Combine(builder.Environment.ContentRootPath, "umbraco", "DataProtection")
        )
    )
    .SetApplicationName("MyCompanyWebsite"); // ← must be the exact same string after every deploy


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
