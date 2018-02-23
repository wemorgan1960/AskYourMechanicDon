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
        public ActionResult AddToBasket(string Id, string vin, string question)
        {
            ViewBag.IsIndexHome = false;
            basketService.AddToBasket(this.HttpContext, Id, vin, question);

            return RedirectToAction("Index");
        }
        [Authorize(Roles = RoleName.AskAdmin + "," + RoleName.AskUser)]
        public ActionResult RemoveFromBasket(string Id)
        {
            ViewBag.IsIndexHome = false;
            basketService.RemoveFromBasket(this.HttpContext, Id);

            return RedirectToAction("Index");
        }

        public PartialViewResult BasketSummary()
        {
            ViewBag.IsIndexHome = false;
            var basketSummary = basketService.GetBasketSummary(this.HttpContext);

            return PartialView(basketSummary);
        }

        [Authorize(Roles = RoleName.AskAdmin +","+ RoleName.AskUser)]
        public ActionResult Checkout()
        {
            ViewBag.IsIndexHome = false;
            Customer customer = customers.Collection().FirstOrDefault(c => c.Email == User.Identity.Name);

            if (customer != null)
            {

                //Core.Models.Order order = new Core.Models.Order()
                //{
                //    Email = customer.Email,
                //    City = customer.City,
                //    Province = customer.Province,
                //    Street = customer.Street,
                //    FirstName = customer.FirstName,
                //    Surname = customer.LastName,
                //    PostalCode = customer.PostalCode,

                //};
                

                return View(customer);
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
            return View(model);
        }

        [HttpPost]
        //[Authorize(Roles = RoleName.AskAdmin + "," + RoleName.AskUser)]
        public void PlaceOrder()
        {
            var basketItems = basketService.GetBasketItems(this.HttpContext);
            var order = new Core.Models.Order
            {
                OrderNumber = Common.GetRandomInvoiceNumber()
            };
            order.InvoiceNumber = "AskDon" + @DateTime.Now.Year + order.OrderNumber;
            order.OrderStatus = "Order Created";
            orderService.CreateOrder(order, basketItems);
            order.OrderStatus = "Payment Processed";
            //order.CompletedAt = DateTime.Now; 
            orderService.UpdateOrder(order);

            //Email Customer
            string CustomerEmail = User.Identity.Name; ;

            var subject = "AskYourMechanicDon.com New Order: " + order.OrderNumber + " Recieved";
            var fromAddress = "admin@askyourmechanicdon.com";
            var toAddress = CustomerEmail;
            var emailBody = "<p>Email From: AskYourMechanicDon.com </p><p>Message:</p><p>You have new order: " + order.OrderNumber + "</p>";

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


            //Email Admin 
            subject = "AskYourMechanicDon.com New Question: " + order.OrderNumber ;
            fromAddress = "admin@askyourmechanicdon.com";
            toAddress = "admin@askyourmechanicdon.com";
            emailBody = "<p>Email From: AskYourMechanicDon.com </p><p>Message:</p><p> A New Question: " + order.OrderNumber + "</p>";

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