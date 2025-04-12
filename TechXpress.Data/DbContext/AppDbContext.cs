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

            #region Order & OrderItem
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId);

            // تحديد العلاقة بين OrderItem و Order باستخدام OrderId
            modelBuilder.Entity<OrderItem>()
                .HasKey(oi => new { oi.OrderId, oi.ProductId }); // المفتاح المركب بين OrderId و ProductId

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)  // العلاقة بين OrderItem و Order
                .WithMany(o => o.Items)  // مع الـ Items في الـ Order
                .HasForeignKey(oi => oi.OrderId)  // الربط بين OrderItem و Order بواسطة الـ OrderId
                .OnDelete(DeleteBehavior.Cascade);  // حذف الـ OrderItem عند حذف الـ Order

            // تحديد العلاقة بين OrderItem و Product
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)  // العلاقة بين OrderItem و Product
                .WithMany()  // مع الـ Products بشكل عام
                .HasForeignKey(oi => oi.ProductId);  // الربط بين OrderItem و Product بواسطة الـ ProductId
            #endregion

            #region CartItem
            modelBuilder.Entity<CartItem>()
                .HasKey(ci => new { ci.CartId, ci.ProductId }); // المفتاح المركب بين CartId و ProductId

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
            modelBuilder.Entity<User>().OwnsOne(u => u.Address);

            #endregion
        }

       
    }
}
