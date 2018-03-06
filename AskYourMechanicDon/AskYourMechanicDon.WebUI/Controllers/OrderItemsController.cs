using AskYourMechanicDon.Core.Contracts;
using AskYourMechanicDon.Core.Models;
using AskYourMechanicDon.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace AskYourMechanicDon.WebUI.Controllers
{
    public class OrderItemsController : Controller
    {
        IRepository<Order> orderContext;
        IRepository<OrderItem> orderItemsContext;

        IRepository<Customer> customersContext;

        public OrderItemsController(IRepository<Order> order , IRepository<OrderItem> orderItems,
             IRepository<Customer> customers)
        {
            this.orderContext = order;
            this.orderItemsContext = orderItems;
            this.customersContext = customers;

        }
        [Authorize(Roles = RoleName.AskAdmin + "," + RoleName.AskUser)]
        public ActionResult Details(string id)
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

        [Authorize(Roles = RoleName.AskUser)]
        public ActionResult Index()
        {
            ViewBag.IsIndexHome = false;
            List<OrderItem> orderItems = orderItemsContext.Collection().ToList();

            return View(orderItems);
        }
        [Authorize(Roles = RoleName.AskAdmin)]
        public ActionResult IndexAdmin()
        {
            ViewBag.IsIndexHome = false;
            List<OrderItem> orderItems = orderItemsContext.Collection().ToList();

            return View(orderItems);
        }

        [Authorize(Roles = RoleName.AskAdmin)]
        public ActionResult Edit(string Id)
        {
            ViewBag.IsIndexHome = false;
            OrderItem orderItem = orderItemsContext.Find(Id);
            if (orderItem == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(orderItem);
            }
        }
        [HttpPost]
        [Authorize(Roles = RoleName.AskAdmin)]
        public ActionResult Edit(OrderItem orderItem, string Id)
        {
            ViewBag.IsIndexHome = false;
            OrderItem orderItemToEdit = orderItemsContext.Find(Id);
            if (orderItem == null)
            {
                return HttpNotFound();
            }
            else
            {
                if (!ModelState.IsValid)
                {
                    return View(orderItem);
                }

                orderItemToEdit.AnswerSubject = orderItem.AnswerSubject;
                orderItemToEdit.AnswerContent = orderItem.AnswerContent;
                orderItemToEdit.AnswerTags = orderItem.AnswerTags;
                orderItemToEdit.AnswerCompleted = DateTime.Now;

                orderItemsContext.Commit();

                //Email Customer
                string CustomerEmail = User.Identity.Name; 

                var subject = "AskYourMechanicDon.com Order has been Answered: " + orderItem.OrderId;
                var fromAddress = "admin@askyourmechanicdon.com";
                var toAddress = CustomerEmail;
                var emailBody = "Email From: AskYourMechanicDon.com Message: the answer to your order: " 
                    + orderItem.Question + " " + orderItem.AnswerContent;

                var smtp = new SmtpClient();
                {
                    smtp.Host = "smtp.askyourmechanicdon.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = false;
                    smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                    smtp.Credentials = new NetworkCredential("admin@askyourmechanicdon.com", "TtLUVAz5");
                    smtp.Timeout = 20000;
                }

                smtp.Send(fromAddress, toAddress, subject, emailBody);

                return RedirectToAction("Index");
            }

        }
    }
}