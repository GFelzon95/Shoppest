using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shoppest.DataAccess.Repository.IRepository;
using Shoppest.Models;
using Shoppest.Models.ViewModels;

namespace ShoppestWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
        public IActionResult Upsert(ProductFormVM productForm)
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
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unitOfWork.Products.Get(p => p.Id == id);

            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "The product couldn't be deleted." });
            }

            _unitOfWork.Products.Remove(productToBeDeleted);
            _unitOfWork.Save();
            TempData["success"] = "Product successfully deleted.";

            return Json(new { success = true, message = "Product Successfully deleted." });
        }
        #endregion
    }
}
