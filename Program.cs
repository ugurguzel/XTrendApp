using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using XTrendApp.Web.Connectors.Amazon;
using XTrendApp.Web.Connectors.Wayfair;
using XTrendApp.Web.Data;
using XTrendApp.Web.Engines.Amazon;
using XTrendApp.Web.Parsers.Amazon;
using XTrendApp.Web.Repositories.ProductAttribute;
using XTrendApp.Web.Repositories.Brand;
using XTrendApp.Web.Repositories.Category;
using XTrendApp.Web.Repositories.Collection;
using XTrendApp.Web.Repositories.Product;
using XTrendApp.Web.Repositories.ProductDocument;
using XTrendApp.Web.Repositories.ProductImage;
using XTrendApp.Web.Repositories.ProductVariation;
using XTrendApp.Web.Repositories.ProductVariationOption;
using XTrendApp.Web.Repositories.ScanJob;
using XTrendApp.Web.Repositories.Snapshot;
using XTrendApp.Web.Repositories.Source;
using XTrendApp.Web.Repositories.User;
using XTrendApp.Web.Services.Product;
using XTrendApp.Web.Services.ScanJob;
using XTrendApp.Web.Services.User;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(
        new DirectoryInfo(
            Path.Combine(
                builder.Environment.ContentRootPath,
                "App_Data",
                "DataProtectionKeys")));

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<DapperContext>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<UserService>();

builder.Services.Configure<AmazonOptions>(
    builder.Configuration.GetSection("Amazon"));

builder.Services.AddScoped<AmazonSession>();

builder.Services.AddScoped<AmazonConnector>();

builder.Services.Configure<WayfairOptions>(
    builder.Configuration.GetSection("Wayfair"));

builder.Services.AddScoped<WayfairSession>();

//builder.Services.AddScoped<WayfairConnector>();

builder.Services.AddScoped<ScanJobRepository>();

builder.Services.AddScoped<ScanJobService>();

builder.Services.AddScoped<AmazonSearchParser>();

builder.Services.AddScoped<AmazonDetailParser>();

builder.Services.AddScoped<AmazonVariationEngine>();

builder.Services.AddScoped<AmazonVariationScanner>();

builder.Services.AddScoped<AmazonVariationNavigator>();

//builder.Services.AddScoped<AmazonVariationWaiter>();

builder.Services.AddScoped<AmazonDropdownParser>();

builder.Services.AddScoped<AmazonButtonParser>();

builder.Services.AddScoped<AmazonColorParser>();

builder.Services.AddScoped<AmazonImageParser>();

#region Repositories

builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<ICollectionRepository, CollectionRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ISourceRepository, SourceRepository>();

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductVariationRepository, ProductVariationRepository>();
builder.Services.AddScoped<IProductVariationOptionRepository, ProductVariationOptionRepository>();

builder.Services.AddScoped<IProductSnapshotRepository, ProductSnapshotRepository>();
builder.Services.AddScoped<IProductAttributeRepository, ProductAttributeRepository>();
builder.Services.AddScoped<IProductImageRepository, ProductImageRepository>();
builder.Services.AddScoped<IProductDocumentRepository, ProductDocumentRepository>();

#endregion

builder.Services.AddScoped<ProductImportService>();

builder.Services.AddScoped<ProductService>();

builder.Services.AddAuthentication(
        CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";

        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;

        options.Events = new CookieAuthenticationEvents
        {
            OnValidatePrincipal = async context =>
            {
                var userId = context.Principal?.FindFirst("UserId")?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    context.RejectPrincipal();
                    await context.HttpContext.SignOutAsync();
                    return;
                }

                var repository = context.HttpContext.RequestServices
                    .GetRequiredService<UserRepository>();

                var user = await repository.GetUserAsync(int.Parse(userId));

                if (user == null || !user.IsActive)
                {
                    context.RejectPrincipal();
                    await context.HttpContext.SignOutAsync();
                }
            }
        };
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
