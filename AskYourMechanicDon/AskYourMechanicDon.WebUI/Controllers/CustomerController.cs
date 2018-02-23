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
    [Authorize(Roles = RoleName.AskAdmin + "," + RoleName.AskUser)]
    public class CustomerController : Controller
    {
        IRepository<Customer> context;
        IRepository<Order> orderContext;
        IRepository<OrderItem> orderItemContext;

        
        public CustomerController(IRepository<Customer> Customers, IRepository<Order> order, IRepository<OrderItem> orderItem  )
        {
            this.context = Customers;
            this.orderContext = order;
            this.orderItemContext = orderItem;
        }
        // GET: Customer
        [Authorize(Roles = RoleName.AskAdmin )]
        public ActionResult Index()
        {
            ViewBag.IsIndexHome = false;
            List<Customer> customers = context.Collection().ToList();
            return View(customers);
        }

        public ActionResult Details(string id)
        {
            ViewBag.IsIndexHome = false;
            if (id == null)
            {
                id = User.Identity.GetUserId();
            }
            Customer customer = context.Find(id);

            return View(customer); 
            }

        public ActionResult Edit(string Id)
        {
            ViewBag.IsIndexHome = false;
            Customer customer = context.Find(Id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(customer);
            }
        }
        [HttpPost]
        public ActionResult Edit(Customer customer, string Id, HttpPostedFileBase file)
        {
            ViewBag.IsIndexHome = false;
            Customer customerToEdit = context.Find(Id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            else
            {
                if (!ModelState.IsValid)
                {
                    return View(customer);
                }

                customerToEdit.FirstName = customer.FirstName;
                customerToEdit.LastName = customer.LastName;
                customerToEdit.Street = customer.Street;
                customerToEdit.City = customer.City;
                customerToEdit.Province = customer.Province;
                customerToEdit.Country = customer.Country;

                context.Commit();

                return RedirectToAction("Index");
            }

        }


    }


}
