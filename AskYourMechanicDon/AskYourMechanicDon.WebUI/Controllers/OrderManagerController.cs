using AskYourMechanicDon.Core.Contracts;
using AskYourMechanicDon.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyShop.WebUI.Controllers
{
    [Authorize(Roles = RoleName.AskAdmin + "," + RoleName.AskUser)]
    public class OrderManagerController : Controller
    {
        IOrderService orderService;

        public OrderManagerController(IOrderService OrderService) {
            this.orderService = OrderService;
        }
        // GET: OrderManager
        public ActionResult Index()
        {
            ViewBag.IsIndexHome = false;
            List<Order> orders = orderService.GetOrderList();

            return View(orders);
        }

        public ActionResult UpdateOrder(string Id) {
            ViewBag.StatusList = new List<string>() {
                "Order Created",
                "Payment Processed",
                "Order Shipped",
                "Order Complete"
            };
            Order order = orderService.GetOrder(Id);
            ViewBag.IsIndexHome = false;
            return View(order);
        }

        [HttpPost]
        public ActionResult UpdateOrder(Order updatedOrder, string Id) {
            Order order = orderService.GetOrder(Id);

            order.OrderStatus = updatedOrder.OrderStatus;
            orderService.UpdateOrder(order);

            ViewBag.IsIndexHome = false;
            return RedirectToAction("Index");
        }
    }
}