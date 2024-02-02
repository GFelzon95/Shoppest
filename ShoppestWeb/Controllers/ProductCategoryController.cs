﻿using Microsoft.AspNetCore.Mvc;
using ShoppestWeb.Data;
using ShoppestWeb.Models;
using ShoppestWeb.ViewModels;

namespace ShoppestWeb.Controllers
{
    public class ProductCategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ProductCategoryController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var viewModel = new ProductCategoryIndex() { ProductCategories = _context.ProductCategories.ToList() };
            return View(viewModel);
        }

        public IActionResult Create()
        {
            var viewModel = new ProductCategoryForm()
            {
                ProductCategory = new ProductCategory() { Id = 0 },
                FormSettings = new FormSettings()
                {
                    Title = "Create Category",
                    ButtonStr = "Create"
                }
            };
            return View("CategoryForm", viewModel);
        }


    }
}
