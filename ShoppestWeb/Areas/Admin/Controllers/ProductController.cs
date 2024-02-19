using Microsoft.AspNetCore.Mvc;
using Shoppest.DataAccess.Repository.IRepository;
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
    }
}
