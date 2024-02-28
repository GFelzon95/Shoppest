using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shoppest.DataAccess.Repository.IRepository;
using Shoppest.Models;
using Shoppest.Models.ViewModels.ProductVM;
using Shoppest.Utility;

namespace ShoppestWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var viewModel = new ProductIndexVM()
            {
                Products = _unitOfWork.Products.GetAll(includeProperties: "ProductCategory")
            };

            return View(viewModel);
        }

        public IActionResult Upsert(int? id)
        {
            var viewModel = new ProductFormVM()
            {
                CategoryList = _unitOfWork.ProductCategories.GetAll().Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                })
            };

            if (id == null || id == 0)
            {
                viewModel.Product = new Product();
            }
            else
            {
                viewModel.Product = _unitOfWork.Products.Get(p => p.Id == id);
                if (viewModel.Product == null)
                {
                    return NotFound();
                }
            }
            return View("ProductForm", viewModel);
        }

        [HttpPost]
        public IActionResult Upsert(ProductFormVM productForm, IFormFile? file)
        {
            if (!ModelState.IsValid)
            {
                productForm.CategoryList = _unitOfWork.ProductCategories.GetAll().Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                });

                return View("ProductForm", productForm);
            }

            string wwwRootPath = _webHostEnvironment.WebRootPath;
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string productPath = Path.Combine(wwwRootPath, @"pictures\product");

                if (!string.IsNullOrEmpty(productForm.Product.PictureUrl))
                {
                    //delete old picture
                    var oldPicturePath = Path.Combine(wwwRootPath, productForm.Product.PictureUrl.TrimStart('\\'));

                    if (System.IO.File.Exists(oldPicturePath))
                    {
                        System.IO.File.Delete(oldPicturePath);
                    }
                }

                using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                productForm.Product.PictureUrl = @"\pictures\product\" + fileName;
            }

            var productId = productForm.Product.Id;

            if (productForm.Product.Id == 0)
            {
                _unitOfWork.Products.Add(productForm.Product);
            }
            else
            {
                _unitOfWork.Products.Update(productForm.Product);
            }
            _unitOfWork.Save();
            TempData["success"] = "Product successfully " + (productId == 0 ? "created" : "updated");
            return RedirectToAction(nameof(Index));
        }

        #region APICALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var productList = _unitOfWork.Products.GetAll(includeProperties: "ProductCategory");
            return Json(new { data = productList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unitOfWork.Products.Get(p => p.Id == id);

            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "The product couldn't be deleted." });
            }

            if (!string.IsNullOrEmpty(productToBeDeleted.PictureUrl))
            {
                //delete old picture
                var oldPicturePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.PictureUrl.TrimStart('\\'));

                if (System.IO.File.Exists(oldPicturePath))
                {
                    System.IO.File.Delete(oldPicturePath);
                }
            }

            _unitOfWork.Products.Remove(productToBeDeleted);
            _unitOfWork.Save();
            TempData["success"] = "Product successfully deleted.";

            return Json(new { success = true, message = "Product Successfully deleted." });
        }
        #endregion
    }
}
