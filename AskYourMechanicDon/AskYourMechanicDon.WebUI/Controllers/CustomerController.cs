using AskYourMechanicDon.Core.Contracts;
using AskYourMechanicDon.Core.Models;
using AskYourMechanicDon.Core.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AskYourMechanicDon.WebUI.Controllers
{
    public class CustomerController : Controller
    {
        IRepository<Customer> context;
        IRepository<Order> orderContext;

        public CustomerController(IRepository<Customer> Customers, IRepository<Order> order   )
        {
            this.context = Customers;
            this.orderContext = order;
        }

        // GET: Customer
        public ActionResult Index()
        {
            ViewBag.IsIndexHome = false;
            List<Customer> customers = context.Collection().ToList();
            return View(customers);
        }


        // GET: Customer/Details/5
        public ActionResult Orders(string id)
        {
            ViewBag.IsIndexHome = false;
            if (id == null)
            {
                id = User.Identity.GetUserId();
            }
            Customer customer = context.Find(id);
            CustomerOrdersViewModel viewModel = new CustomerOrdersViewModel
            {
                Customer = customer,
                Orders = customer.Orders
            };

            return View(viewModel);
        }

        public ActionResult OrderItems(string id)
        {
            ViewBag.IsIndexHome = false;
            Order order = orderContext.Find(id);
            OrderOrderItemViewModel viewModel = new OrderOrderItemViewModel
            {
                Order = order,
                OrderItems = order.OrderItems
            };
            List<OrderItem> orderItems = order.OrderItems.ToList();


            return View(viewModel);
        }

    }
}
