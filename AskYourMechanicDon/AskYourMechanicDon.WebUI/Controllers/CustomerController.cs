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
        IRepository<Customer> customerContext;
        IRepository<Order> orderContext;
        IRepository<OrderItem> orderItemContext;


        public CustomerController(IRepository<Customer> Customers, IRepository<Order> order, IRepository<OrderItem> orderItem)
        {
            this.customerContext = Customers;
            this.orderContext = order;
            this.orderItemContext = orderItem;
        }
        // GET: Customer
        [Authorize(Roles = RoleName.AskAdmin)]
        public ActionResult Index()
        {
            ViewBag.IsIndexHome = false;
            List<Customer> customers = customerContext.Collection().ToList();
            return View(customers);
        }

        public ActionResult Details(string id)
        {
            ViewBag.IsIndexHome = false;
            if (id == null)
            {
                id = User.Identity.GetUserId();
            }
            Customer customer = customerContext.Find(id);

            return View(customer);
        }

        [Authorize]
        public ActionResult DetailsCheckOut(string id)
        {
            ViewBag.IsIndexHome = false;
            if (id == null)
            {
                id = User.Identity.GetUserId();
            }
            Customer customer = customerContext.Find(id);

            return View(customer);
        }

        public ActionResult EditCheckOut(string Id)
        {
            ViewBag.IsIndexHome = false;
            Customer customer = customerContext.Find(Id);
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
        public ActionResult EditCheckOut(Customer customer, string Id, HttpPostedFileBase file)
        {
            ViewBag.IsIndexHome = false;
            Customer customerToEdit = customerContext.Find(Id);
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

                customerContext.Commit();

                return RedirectToAction("PlaceOrder", "Basket");
            }
        }

        public ActionResult Edit(string Id)
        {
            ViewBag.IsIndexHome = false;
            Customer customer = customerContext.Find(Id);
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
            Customer customerToEdit = customerContext.Find(Id);
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

                customerContext.Commit();

                return RedirectToAction("Index");
            }

        }


    }


}
