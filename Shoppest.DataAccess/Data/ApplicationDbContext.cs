using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shoppest.Models;

namespace Shoppest.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProductCategory>().HasData(
                new ProductCategory { Id = 1, Name = "Fashion" },
                new ProductCategory { Id = 2, Name = "Electronics" },
                new ProductCategory { Id = 3, Name = "Toys" },
                new ProductCategory { Id = 4, Name = "Sports" }
                );

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Denim Jeans",
                    PictureUrl = "",
                    Description = "Jeans with no holes in it.",
                    Price = 30,
                    Quantity = 100,
                    ProductCategoryId = 1
                },
                new Product
                {
                    Id = 2,
                    Name = "PS6",
                    PictureUrl = "",
                    Description = "New Play Station with so advanced features that it's not even released yet.",
                    Price = 1200,
                    Quantity = 0,
                    ProductCategoryId = 2
                },
                new Product
                {
                    Id = 3,
                    Name = "Yo-Yo",
                    PictureUrl = "",
                    Description = "It's just a Yo-Yo.",
                    Price = 5,
                    Quantity = 200,
                    ProductCategoryId = 3
                },
                new Product
                {
                    Id = 4,
                    Name = "Soccer Shoes",
                    PictureUrl = "",
                    Description = "Latest pair of soccess shoes with advanced technology... But it's a pair of right foot shoes only.",
                    Price = 100,
                    Quantity = 20,
                    ProductCategoryId = 4
                }
                );
        }


    }
}
