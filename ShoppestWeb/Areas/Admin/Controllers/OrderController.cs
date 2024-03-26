using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shoppest.DataAccess.Repository.IRepository;
using Shoppest.Models;
using Shoppest.Models.ViewModels.OrderVM;
using Shoppest.Utility;
using Stripe;
using System.Security.Claims;

namespace ShoppestWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Details(int id)
        {
            var viewModel = new OrderDetailsVM()
            {
                OrderHeader = _unitOfWork.OrderHeaders.Get(o => o.Id == id, includeProperties: "ApplicationUser"),
                OrderDetails = _unitOfWork.OrderDetails.GetAll(o => o.OrderHeaderId == id, includeProperties: "Product")
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult UpdateOrderDetail(OrderDetailsVM orderDetailsVM)
        {
            var orderFromDb = _unitOfWork.OrderHeaders.Get(o => o.Id == orderDetailsVM.OrderHeader.Id, includeProperties: "ApplicationUser");
            orderFromDb.Name = orderDetailsVM.OrderHeader.Name;
            orderFromDb.PhoneNumber = orderDetailsVM.OrderHeader.PhoneNumber;
            orderFromDb.Region = orderDetailsVM.OrderHeader.Region;
            orderFromDb.Province = orderDetailsVM.OrderHeader.Province;
            orderFromDb.City = orderDetailsVM.OrderHeader.City;
            orderFromDb.Barangay = orderDetailsVM.OrderHeader.Barangay;
            orderFromDb.StreetAddress = orderDetailsVM.OrderHeader.StreetAddress;
            orderFromDb.PostalCode = orderDetailsVM.OrderHeader.PostalCode;
            if (!string.IsNullOrEmpty(orderDetailsVM.OrderHeader.Carrier))
            {
                orderFromDb.Carrier = orderDetailsVM.OrderHeader.Carrier;
            }
            if (!string.IsNullOrEmpty(orderDetailsVM.OrderHeader.TrackingNumber))
            {
                orderFromDb.TrackingNumber = orderDetailsVM.OrderHeader.TrackingNumber;
            }

            _unitOfWork.OrderHeaders.Update(orderFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Details), new { id = orderDetailsVM.OrderHeader.Id });
        }


        #region API CALLS

        [HttpGet]
        [Authorize]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> orderHeaders;

            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                orderHeaders = _unitOfWork.OrderHeaders.GetAll(includeProperties: "ApplicationUser");
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                orderHeaders = _unitOfWork.OrderHeaders.GetAll(o => o.ApplicationUserId == userId);
            }

            switch (status)
            {
                case "pending":
                    orderHeaders = orderHeaders.Where(o => o.OrderStatus == SD.StatusPending);
                    break;
                case "inprocess":
                    orderHeaders = orderHeaders.Where(o => o.OrderStatus == SD.StatusInProcess);
                    break;
                case "completed":
                    orderHeaders = orderHeaders.Where(o => o.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    orderHeaders = orderHeaders.Where(o => o.OrderStatus == SD.StatusApproved);
                    break;
                default:
                    break;
            }

            return Json(new { data = orderHeaders });
        }

        [HttpDelete]
        [Authorize]
        public IActionResult CancelOrder(int id)
        {
            var orderHeader = _unitOfWork.OrderHeaders.Get(o => o.Id == id);

            if (orderHeader.PaymentStatus == SD.PaymentStatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId
                };

                var service = new RefundService();
                Refund refund = service.Create(options);

                _unitOfWork.OrderHeaders.UpdateStatus(id, SD.StatusCancelled, SD.StatusRefunded);
            }
            else
            {
                _unitOfWork.OrderHeaders.UpdateStatus(id, SD.StatusCancelled, SD.StatusCancelled);

            }
            _unitOfWork.Save();
            TempData["success"] = "Order Cancelled Successfully.";
            return Json(new { success = true });
        }

        #endregion
    }
}
