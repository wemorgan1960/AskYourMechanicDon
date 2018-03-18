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
        IOrderService orderServiceContext;
        IRepository<OrderItem> orderItemsContext;
        IRepository<Order> orderContext;
        IRepository<Customer> customersContext;

        public OrderController(IOrderService OrderService, IRepository<OrderItem> orderItems,
             IRepository<Customer> customers, IRepository<Order> order) {
            this.orderServiceContext = OrderService;
            this.orderItemsContext = orderItems;
            this.customersContext = customers;
            this.orderContext = order;

        }
        // GET: OrderManager
        public ActionResult Index()
        {
            ViewBag.IsIndexHome = false;
            List<Order> orders = orderServiceContext.GetOrderList();

            return View(orders);
        }

        public ActionResult UpdateOrder(string Id) {
            ViewBag.StatusList = new List<string>() {
                "Order Created",
                "Payment Processed",
                "Order Complete"
            };
            Order order = orderServiceContext.GetOrder(Id);
            ViewBag.IsIndexHome = false;
            return View(order);
        }

        [HttpPost]
        public ActionResult UpdateOrder(Order updatedOrder, string Id) {
            Order order = orderServiceContext.GetOrder(Id);

            order.OrderStatus = updatedOrder.OrderStatus;
            orderServiceContext.UpdateOrder(order);

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
            Customer customer = customersContext.Find(id);
            List<Order> orders = orderContext.Collection().ToList();
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