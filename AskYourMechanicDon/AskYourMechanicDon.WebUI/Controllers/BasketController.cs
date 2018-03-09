using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AskYourMechanicDon.Core.Contracts;
using AskYourMechanicDon.Core.Models;
using System.Net.Mail;
using System.Net;

namespace AskYourMechanicDon.WebUI.Controllers
{

    public class BasketController : Controller
    {
        //private PayPal.Api.Payment payment;

        IRepository<Customer> customers;
        IBasketService basketService;
        IOrderService orderService;

        public BasketController(IBasketService BasketService, IOrderService OrderService, IRepository<Customer> Customers)
        {
            this.basketService = BasketService;
            this.orderService = OrderService;
            this.customers = Customers;
        }
        // GET: Basket2
        public ActionResult Index()
        {
            ViewBag.IsIndexHome = false;
            var model = basketService.GetBasketItems(this.HttpContext);
            return View(model);
        }
        [Authorize(Roles = RoleName.AskAdmin + "," + RoleName.AskUser)]
        [HttpPost]
        public ActionResult AddToBasket(string Id, string vin, string question)
        {
            ViewBag.IsIndexHome = false;
            basketService.AddToBasket(this.HttpContext, Id, vin, question);

            return RedirectToAction("Index", "Products");
        }
        [Authorize(Roles = RoleName.AskAdmin + "," + RoleName.AskUser)]
        public ActionResult RemoveFromBasket(string Id)
        {
            ViewBag.IsIndexHome = false;
            basketService.RemoveFromBasket(this.HttpContext, Id);

            return RedirectToAction("Index", "Products");
        }

        public PartialViewResult BasketSummary()
        {
            ViewBag.IsIndexHome = false;
            var basketSummary = basketService.GetBasketSummary(this.HttpContext);

            return PartialView(basketSummary);
        }

        [Authorize(Roles = RoleName.AskAdmin + "," + RoleName.AskUser)]
        public ActionResult Checkout()
        {
            ViewBag.IsIndexHome = false;
            Customer customer = customers.Collection().FirstOrDefault(c => c.Email == User.Identity.Name);

            if (customer != null)
            {
                var model = basketService.GetBasketItems(this.HttpContext);
                if (model != null)
                {
                    return View(customer);
                }
                else
                {
                    return RedirectToAction("Index", "Products");
                }

            }
            else
            {
                return RedirectToAction("Error");
            }

        }
        [Authorize(Roles = RoleName.AskAdmin + "," + RoleName.AskUser)]
        public ActionResult ReviewOrder()
        {
            ViewBag.IsIndexHome = false;
            var model = basketService.GetBasketItems(this.HttpContext);
            if (model != null)
            {
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Products");
            }

        }

        [HttpPost]
        //[Authorize(Roles = RoleName.AskAdmin + "," + RoleName.AskUser)]
        public void PlaceOrder()
        {
            var basketItems = basketService.GetBasketItems(this.HttpContext);
            var order = new Order
            {
                OrderNumber = Common.GetRandomInvoiceNumber()
            };

            //Get the customer UserId
            Customer customer = customers.Collection().FirstOrDefault(c => c.Email == User.Identity.Name);
            order.CustomerUserId = customer.UserId;

            //Get the Invoice and Status
            order.InvoiceNumber = "AskDon" + @DateTime.Now.Year + order.OrderNumber;
            order.OrderStatus = "Order Created";
            order.OrderStatusDate = @DateTime.Now;

            //Create the Order
            orderService.CreateOrder(order, basketItems);

            //Update the Payment Processed.
            order.OrderStatus = "Payment Processed";
            order.OrderStatusDate = @DateTime.Now;
            orderService.UpdateOrder(order);

            //Email Customer
            string CustomerEmail = User.Identity.Name; ;

            var subject = "AskYourMechanicDon.com New Order: " + order.OrderNumber + " Recieved";
            var fromAddress = "admin@askyourmechanicdon.com";
            var toAddress = CustomerEmail;
            var emailBody = "Email From: AskYourMechanicDon.com Message: Thank you for your order: " + order.OrderNumber;

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

            string emailAddress = "admin@askyourmechanicdon.com";

            if (Production.IsProduction == true)
            {
                emailAddress = "admin@askyourmechanicdon.com,donmorgan@shaw.ca";
            }
            else
            {
                emailAddress = "admin@askyourmechanicdon.com";
            }
            //Email Admin 
            subject = "AskYourMechanicDon.com New Question: " + order.OrderNumber;
            fromAddress = "admin@askyourmechanicdon.com";
            toAddress = emailAddress;
            emailBody = "Email From: AskYourMechanicDon.com Message: A New Question: " + order.OrderNumber;

            var smtp1 = new SmtpClient();
            {
                smtp1.Host = "smtp.askyourmechanicdon.com";
                smtp1.Port = 587;
                smtp1.EnableSsl = false;
                smtp1.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                smtp1.Credentials = new NetworkCredential("admin@askyourmechanicdon.com", "TtLUVAz5");
                smtp1.Timeout = 20000;
            }

            smtp1.Send(fromAddress, toAddress, subject, emailBody);

            basketService.ClearBasket(this.HttpContext);

        }

    }
}