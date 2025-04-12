using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TechXpress.Data.DbContext;
using TechXpress.Data.Entities;
using TechXpress.Data.Repositories;
using TechXpress.Data.RepositoriesInterfaces;
using System.Threading.Tasks;
using TechXpress.Data.ValueObjects;
using TechXpress.Data.UnitOfWork; // مهم جداً

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Database connection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IProductRepository<>), typeof(ProductRepository<>));

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.Zero;
});
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/User/Login";
    options.LogoutPath = "/User/Logout";
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.SlidingExpiration = true;
});

var app = builder.Build();

// Configure middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// ✅ Create admin user and roles
await SeedAdminUserAsync(app);

// Routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();

async Task SeedAdminUserAsync(IHost app)
{
    using var scope = app.Services.CreateScope();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string adminEmail = "admin@techxpress.com";
    string adminPassword = "@dm1n&";
    string adminName = "Admin";


    if (!await roleManager.RoleExistsAsync("Admin"))
        await roleManager.CreateAsync(new IdentityRole("Admin"));

    if (!await roleManager.RoleExistsAsync("Customer"))
        await roleManager.CreateAsync(new IdentityRole("Customer"));

    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        var user = new User
        {
            Name = adminName,
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            Address = new Address("Admin", "Admin", "Admin", "Admin", "Admin")

        };

        var result = await userManager.CreateAsync(user, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "Admin");
        }
    }
}
