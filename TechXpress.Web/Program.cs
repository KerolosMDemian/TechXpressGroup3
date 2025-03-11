using Microsoft.EntityFrameworkCore;
using TechXpress.Data.DbContext;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// DataBase Connection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// ✅ تفعيل ملفات static
app.UseStaticFiles();
app.UseDefaultFiles(); // يبحث تلقائياً عن index.html أو default.html

app.UseRouting();

app.UseAuthorization();

// ✅ Route للـ MVC Controllers
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

// ✅ لو حابة تعملي redirect للـ Design مباشرة
app.MapGet("/", async context =>
{
    context.Response.Redirect("/Design/index.html");
});

app.Run();
