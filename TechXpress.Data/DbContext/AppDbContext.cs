using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TechXpress.Data.Entities;

namespace TechXpress.Data.DbContext
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // إعداد العلاقات في قاعدة البيانات
            #region Product
            modelBuilder.Entity<Product>()
                .HasKey(p => p.Id);
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);
            #endregion
            #region Category
            modelBuilder.Entity<Category>()
                .HasKey(p => p.Id);
            #endregion
            #region Order&OrderItem
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId);

            modelBuilder.Entity<OrderItem>()
                .HasKey(oi => new { oi.ProductId, oi.Quantity });
            modelBuilder.Entity<OrderItem>()
               .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId);
            #endregion
            #region CartItem
            // إعداد العلاقات في CartItem
            modelBuilder.Entity<CartItem>()
                .HasKey(ci => new { ci.CartId, ci.ProductId }); // مفتاح مركب

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CartId);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId);
            #endregion
            #region User
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);
            modelBuilder.Entity<IdentityUserLogin<int>>()
                .HasKey(x => new { x.LoginProvider, x.ProviderKey });
            modelBuilder.Entity<IdentityUserRole<int>>()
                .HasKey(x => new { x.UserId, x.RoleId });
            modelBuilder.Entity<IdentityUserToken<int>>()
                .HasKey(x => new { x.UserId, x.LoginProvider, x.Name });
            #endregion
        }
    }
}