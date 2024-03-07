using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shoppest.DataAccess.Repository.IRepository;
using Shoppest.Models;
using Shoppest.Models.ViewModels.HomeVM;
using System.Diagnostics;
using System.Security.Claims;

namespace Shoppest.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;

        }

        public IActionResult Index()
        {
            var viewModel = new HomeIndexVM()
            {
                Products = _unitOfWork.Products.GetAll()
            };
            return View(viewModel);
        }

        public IActionResult Details(int id)
        {
            var viewModel = new HomeDetailsVM()
            {
                ShoppingCart = new()
                {
                    Product = _unitOfWork.Products.Get(p => p.Id == id, includeProperties: "ProductCategory"),
                    Count = 1,
                    ProductId = id
                }
            };



            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId = userId;

            var product = _unitOfWork.Products.Get(p => p.Id == shoppingCart.ProductId);
            var cartFromDb = _unitOfWork.ShoppingCarts.Get(
                c => c.ProductId == shoppingCart.ProductId
                && c.ApplicationUserId == shoppingCart.ApplicationUserId);

            if (cartFromDb == null)
            {
                if (shoppingCart.Count > product.Quantity)
                {
                    ModelState.AddModelError("ShoppingCart.Count", "Can't exceed the quantity in stock.");
                }
            }
            else
            {
                if (shoppingCart.Count + cartFromDb.Count > product.Quantity)
                {
                    ModelState.AddModelError("ShoppingCart.Count", "Can't exceed the quantity in stock.");
                }
            }
            if (!ModelState.IsValid)
            {
                var viewModel = new HomeDetailsVM()
                {
                    ShoppingCart = new()
                    {
                        Product = _unitOfWork.Products.Get(p => p.Id == shoppingCart.ProductId, includeProperties: "ProductCategory"),
                        Count = 1,
                        ProductId = shoppingCart.ProductId
                    }
                };
                return View(viewModel);
            }

            if (cartFromDb != null)
            {
                cartFromDb.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCarts.Update(cartFromDb);
            }
            else
            {
                shoppingCart.Selected = false;
                _unitOfWork.ShoppingCarts.Add(shoppingCart);
            }

            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
