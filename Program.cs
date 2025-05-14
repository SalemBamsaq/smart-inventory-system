using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartInventorySystem.Data;
using SmartInventorySystem.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// 🔐 Identity with Role support, using ApplicationUser instead of IdentityUser
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// Configure Identity options
builder.Services.ConfigureApplicationCookie(options =>
{
    // Set the login path to redirect when user is not authenticated
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    // Redirect to dashboard after login
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.Redirect(options.LoginPath);
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.Redirect(options.AccessDeniedPath);
        return Task.CompletedTask;
    };
});

builder.Services.AddRazorPages(options =>
{
   
});

var app = builder.Build();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
    await RoleSeeder.SeedRolesAsync(services);

    // Seed default supplier
    if (!context.Suppliers.Any())
    {
        var defaultSupplier = new Supplier
        {
            Name = "Default Supplier",
            Email = "supplier@example.com",
            ContactPerson = "Default Contact",
            Phone = "555-123-4567"
        };
        context.Suppliers.Add(defaultSupplier);
        await context.SaveChangesAsync();
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

app.Run();