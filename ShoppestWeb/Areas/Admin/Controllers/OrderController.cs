using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shoppest.DataAccess.Repository.IRepository;
using Shoppest.Models;
using Shoppest.Models.ViewModels.OrderVM;
using Shoppest.Utility;
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

        #region API CALLS

        [HttpGet]
        [Authorize]
        public IActionResult GetAll()
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

            return Json(new { data = orderHeaders });
        }

        #endregion
    }
}
