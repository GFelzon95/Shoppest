﻿using Microsoft.EntityFrameworkCore;
using ShoppestWeb.Models;

namespace ShoppestWeb.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        DbSet<ProductCategory> ProductCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductCategory>().HasData(
                new ProductCategory { Id = 1, Name = "Fashion" },
                new ProductCategory { Id = 2, Name = "Electronics" },
                new ProductCategory { Id = 3, Name = "Toys" },
                new ProductCategory { Id = 4, Name = "Sports" }
                );
        }
    }
}
