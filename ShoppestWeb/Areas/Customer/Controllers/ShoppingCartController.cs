using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shoppest.DataAccess.Repository.IRepository;
using Shoppest.Models;
using Shoppest.Models.ViewModels.ShoppingCartVM;
using Shoppest.Utility;
using Stripe.Checkout;
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
            //var applicationUser = _unitOfWork.ApplicationUsers.Get(u => u.Id == userId);
            var shoppingCartList = _unitOfWork.ShoppingCarts.GetAll(c => c.ApplicationUserId == userId && c.Selected == true, includeProperties: "Product");

            if (!ModelState.IsValid)
            {
                var viewModel = new ShoppingCartSummaryVM()
                {
                    ShoppingCartList = shoppingCartList,
                    OrderHeader = orderHeader
                };

                viewModel.OrderHeader.OrderTotal = GetTotalPrice(viewModel.ShoppingCartList);

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
            var domain = "https://localhost:7180/";
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                SuccessUrl = domain + $"customer/ShoppingCart/orderconfirmation?id={orderHeader.Id}",
                CancelUrl = domain + "customer/ShoppingCart/index",
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                Mode = "payment",
            };

            foreach (var cart in shoppingCartList)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(cart.Product.Price * cart.Count * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = cart.Product.Name
                        }
                    },
                    Quantity = cart.Count
                };
                options.LineItems.Add(sessionLineItem);
            }

            var service = new SessionService();
            Session session = service.Create(options);

            _unitOfWork.OrderHeaders.UpdateStripePaymentID(orderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        public IActionResult OrderConfirmation(int id)
        {
            var orderHeader = _unitOfWork.OrderHeaders.Get(o => o.Id == id, includeProperties: "ApplicationUser");
            var service = new SessionService();
            Session session = service.Get(orderHeader.SessionId);

            if (session.PaymentStatus.ToLower() == "paid")
            {
                _unitOfWork.OrderHeaders.UpdateStripePaymentID(orderHeader.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.OrderHeaders.UpdateStatus(orderHeader.Id, SD.StatusApproved, SD.PaymentStatusApproved);
                _unitOfWork.Save();
            }
            //Remove Items from Shopping Cart
            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCarts
                .GetAll(c => c.ApplicationUserId == orderHeader.ApplicationUserId && c.Selected == true).ToList();

            _unitOfWork.ShoppingCarts.RemoveRange(shoppingCarts);

            var orderDetails = _unitOfWork.OrderDetails.GetAll(o => o.OrderHeaderId == orderHeader.Id, includeProperties: "Product");

            foreach (var order in orderDetails)
            {
                var product = _unitOfWork.Products.Get(p => p.Id == order.ProductId);
                product.Quantity -= order.Count;
                _unitOfWork.Products.Update(product);
            }
            _unitOfWork.Save();

            return View(id);
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
