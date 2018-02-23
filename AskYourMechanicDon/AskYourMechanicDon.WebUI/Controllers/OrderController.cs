using AskYourMechanicDon.Core.Contracts;
using AskYourMechanicDon.Core.Models;
using AskYourMechanicDon.Core.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyShop.WebUI.Controllers
{
    [Authorize(Roles = RoleName.AskAdmin + "," + RoleName.AskUser)]
    public class OrderController : Controller
    {
        IOrderService orderService;
        IRepository<OrderItem> orderItems;

        IRepository<Customer> customers;

        public OrderController(IOrderService OrderService, IRepository<OrderItem> orderItemsContext,
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

        public ActionResult Orders(string id)
        {
            ViewBag.IsIndexHome = false;
            if (id == null)
            {
                id = User.Identity.GetUserId();
            }
            Customer customer = customers.Find(id);
            CustomerOrdersViewModel viewModel = new CustomerOrdersViewModel
            {
                Customer = customer,
                Orders = customer.Orders
            };

            return View(viewModel);
        }

    }
}