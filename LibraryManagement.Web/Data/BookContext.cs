using LibraryManagement.Web.Entity;
using LibraryManagement.Web.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LibraryManagement.Web.Data
{
    public class BookContext : DbContext
    {
        public BookContext(DbContextOptions<BookContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<BookSuggestion> BookSuggestions { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<BookReview> BookReviews { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public object CartItems { get; internal set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>()
                .HasMany(b => b.Genres)
                .WithMany(g => g.Books);

            modelBuilder.Entity<Book>()
                .Property(b => b.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.ShippingFee)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.GrandTotal)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.DiscountAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Coupon>()
                .Property(c => c.DiscountValue)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Coupon>()
                .Property(c => c.MinimumOrderAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Address)
                .WithMany()
                .HasForeignKey(o => o.AddressId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.TotalPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<BookReview>()
                .HasOne(r => r.Book)
                .WithMany(b => b.Reviews)
                .HasForeignKey(r => r.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BookReview>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BookReview>()
                .HasIndex(r => new { r.BookId, r.UserId })
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }

      
    }
}
