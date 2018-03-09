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
        IRepository<Order> order;
        IRepository<Customer> customers;

        public OrderController(IOrderService OrderService, IRepository<OrderItem> orderItemsContext,
             IRepository<Customer> customersContext, IRepository<Order> orderContext) {
            this.orderService = OrderService;
            this.orderItems = orderItemsContext;
            this.customers = customersContext;
            this.order = orderContext;

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
            List<Order> orders = order.Collection().ToList();
            IEnumerable<Order> orderx = orders.Where(o => o.CustomerUserId.CompareTo(customer.Id) <1);

            CustomerOrdersViewModel viewModel = new CustomerOrdersViewModel
            {
                Customer = customer,
                Orders = orderx
            };

            return View(viewModel);
        }

    }
}