using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Mobile.Account;
using Mobile.Models;
using Mobile.Utilities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<Db>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddEntityFrameworkStores<Db>()
    .AddDefaultTokenProviders();

builder.Services.Configure<PasswordHasherOptions>(options =>
{
    options.CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV2;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Colton Added for Branding/CSS themes based on URI.
app.Use(async (context, next) =>
{
    var requestUri = new Uri($"{context.Request.Scheme}://{context.Request.Host}");

    var brandKey = URIUtility.GetURIBranding(requestUri);
    var brandInfo = BrandFactory.Create(brandKey);

    context.Items["Brand"] = brandInfo;
    context.Items["LogoPath"] = $"/branding/{brandKey}/banner.png";
    context.Items["CssPath"] = $"/branding/{brandKey}/style.css";

    await next();
});

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
