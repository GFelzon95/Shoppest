using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shoppest.DataAccess.Repository.IRepository;
using Shoppest.Models;
using Shoppest.Models.ViewModels.ShoppingCartVM;
using Shoppest.Utility;
using System.Security.Claims;

namespace ShoppestWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private double GetTotalPrice(IEnumerable<ShoppingCart> cartList)
        {
            double totalPrice = 0;

            foreach (var cart in cartList)
            {
                double cartPrice = cart.Product.Price * cart.Count;
                totalPrice += cartPrice;
            }

            return totalPrice;
        }

        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var viewModel = new ShoppingCartIndexVM()
            {
                ShoppingCartList = _unitOfWork.ShoppingCarts.GetAll(c => c.ApplicationUserId == userId, includeProperties: "Product")
            };

            viewModel.TotalPrice = GetTotalPrice(viewModel.ShoppingCartList.Where(c => c.Selected == true));

            return View(viewModel);
        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var applicationUser = _unitOfWork.ApplicationUsers.Get(u => u.Id == userId);



            var viewModel = new ShoppingCartSummaryVM()
            {
                ShoppingCartList = _unitOfWork.ShoppingCarts.GetAll(s => s.ApplicationUserId == userId && s.Selected == true, includeProperties: "Product"),
            };

            var orderHeader = new OrderHeader()
            {
                ApplicationUser = applicationUser,

                Name = applicationUser.Name,
                PhoneNumber = applicationUser.PhoneNumber,
                Region = applicationUser.Region,
                Province = applicationUser.Province,
                City = applicationUser.City,
                Barangay = applicationUser.Barangay,
                StreetAddress = applicationUser.StreetAddress,
                PostalCode = applicationUser.PostalCode,
                OrderTotal = GetTotalPrice(viewModel.ShoppingCartList)
            };

            viewModel.OrderHeader = orderHeader;

            return View(viewModel);
        }

        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPOST(OrderHeader orderHeader)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var applicationUser = _unitOfWork.ApplicationUsers.Get(u => u.Id == userId);
            var shoppingCartList = _unitOfWork.ShoppingCarts.GetAll(c => c.ApplicationUserId == userId && c.Selected == true, includeProperties: "Product");

            if (!ModelState.IsValid)
            {
                var viewModel = new ShoppingCartSummaryVM()
                {
                    ShoppingCartList = shoppingCartList,
                    OrderHeader = orderHeader
                };

                return View(viewModel);
            }

            orderHeader.ApplicationUserId = userId;
            orderHeader.OrderDate = DateTime.Now;
            orderHeader.OrderTotal = GetTotalPrice(shoppingCartList);

            orderHeader.OrderStatus = SD.StatusPending;
            orderHeader.PaymentStatus = SD.PaymentStatusPending;

            _unitOfWork.OrderHeaders.Add(orderHeader);
            _unitOfWork.Save();

            foreach (var cart in shoppingCartList)
            {
                OrderDetail orderDetail = new OrderDetail()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = orderHeader.Id,
                    Count = cart.Count,
                    Price = cart.Product.Price * cart.Count
                };

                _unitOfWork.OrderDetails.Add(orderDetail);
                _unitOfWork.Save();
            }

            //Stripe logic


            return RedirectToAction(nameof(OrderConfirmation));
        }

        public IActionResult OrderConfirmation()
        {
            return View();
        }

        public IActionResult Plus(int? id)
        {
            var cartInDb = _unitOfWork.ShoppingCarts.Get(c => c.Id == id, includeProperties: "Product");
            if (cartInDb.Count < cartInDb.Product.Quantity)
            {
                cartInDb.Count++;
                _unitOfWork.ShoppingCarts.Update(cartInDb);
                _unitOfWork.Save();
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int? id)
        {
            var cartInDb = _unitOfWork.ShoppingCarts.Get(c => c.Id == id);
            if (cartInDb.Count <= 1)
            {
                _unitOfWork.ShoppingCarts.Remove(cartInDb);
            }
            else
            {
                cartInDb.Count--;
                _unitOfWork.ShoppingCarts.Update(cartInDb);
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int? id)
        {
            var cartInDb = _unitOfWork.ShoppingCarts.Get(c => c.Id == id);
            _unitOfWork.ShoppingCarts.Remove(cartInDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult SelectProduct(int? id)
        {
            var cartInDb = _unitOfWork.ShoppingCarts.Get(c => c.Id == id);

            if (cartInDb.Selected == false)
            {
                cartInDb.Selected = true;
            }
            else
            {
                cartInDb.Selected = false;
            }
            _unitOfWork.ShoppingCarts.Update(cartInDb);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }
    }
}
