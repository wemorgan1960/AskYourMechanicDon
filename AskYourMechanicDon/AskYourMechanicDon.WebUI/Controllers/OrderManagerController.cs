using AskYourMechanicDon.Core.Contracts;
using AskYourMechanicDon.Core.Models;
using AskYourMechanicDon.Core.ViewModels;
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
        IRepository<OrderItem> orderItems;

        IRepository<Customer> customers;

        public OrderManagerController(IOrderService OrderService, IRepository<OrderItem> orderItemsContext,
             IRepository<Customer> customersContext) {
            this.orderService = OrderService;
            this.orderItems = orderItemsContext;
            this.customers = customersContext;

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