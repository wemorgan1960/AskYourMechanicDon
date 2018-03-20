using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AskYourMechanicDon.Core.Contracts;
using AskYourMechanicDon.Core.Models;
using AskYourMechanicDon.Core.ViewModels;
using Microsoft.AspNet.Identity;

namespace AskYourMechanicDon.WebUI.Controllers
{

    public class BasketController : Controller
    {
        IRepository<Customer> customersContext;
        IBasketService basketService;
        IOrderService orderService;
        IRepository<Product> productContext;
        IRepository<ProductCategory> productCategoryContext;
        IRepository<Request> requestContext;
        IRepository<Error> errorContext;

        public BasketController(IBasketService basketService, IOrderService orderService,IRepository<Product> product,
            IRepository<ProductCategory> productCategory,
            IRepository<Customer> customer,
            IRepository<Request> requests, IRepository<Error> errors)
        {
            this.basketService = basketService;
            this.orderService = orderService;
            this.productContext = product;
            this.productCategoryContext = productCategory;
            this.customersContext = customer;
            this.requestContext = requests;
            this.errorContext = errors;
        }


        public ActionResult Index()
        {
            ViewBag.IsIndexHome = false;
            var model = basketService.GetBasketItems(this.HttpContext);
            if(model.Count!=0)
            {
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Products");
            }
            
        }

        [HttpPost]
        public ActionResult AddToBasket(string Id, string vin, string question)
        {
            ViewBag.IsIndexHome = false;
            basketService.AddToBasket(this.HttpContext, Id, vin, question);

            return RedirectToAction("Index");
        }


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

        //[Authorize]
        //public ActionResult Checkout()
        //{
        //    ViewBag.IsIndexHome = false;
        //    Customer customer = customersContext.Collection().FirstOrDefault(c => c.Email == User.Identity.Name);

        //    if (customer != null)
        //    {
        //        var model = basketService.GetBasketItems(this.HttpContext);
        //        if (model != null)
        //        {
        //            return View(customer);
        //        }
        //        else
        //        {
        //            return RedirectToAction("Index", "Products");
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("Error");
        //    }

        //}

        [Authorize]
        public ActionResult PlaceOrder()
        {
            ViewBag.IsIndexHome = false;

            var basketItems = basketService.GetBasketItems(this.HttpContext);

            if (basketItems != null)
                
            {
                if (basketItems.Count != 0)
                {

                OrderNumberOrderItemsViewModel model = new OrderNumberOrderItemsViewModel();

                //Get the customer UserId
                Customer customer = customersContext.Collection().FirstOrDefault(c => c.Email == User.Identity.Name);

                //Set Order Number and Create a new order
                var order = new Order
                {
                    CustomerUserId = customer.UserId,
                    OrderNumber = customer.UserId + Common.GetRandomInvoiceNumber(),
                    OrderStatus = "Order Created",
                    OrderStatusDate = @DateTime.Now
                };

                //Get the Invoice 
                order.InvoiceNumber = "AskDon" + @DateTime.Now.Year + order.OrderNumber;

                //Create the Order
                orderService.CreateOrder(order, basketItems);

                //Get the new Order
                string orderNumber = order.OrderNumber;
                order = orderService.GetOrderFromOrderNumber(orderNumber);

                model.OrderNumber = orderNumber;
                model.OrderItems = orderService.GetOrderItemListFromOrderNumber(orderNumber);

                //Clear the basket
                basketService.ClearBasket(this.HttpContext);

                return View(model);
                }
                else
                {
                    return RedirectToAction("Index", "Products");
                }
            }
            else
            {
                return RedirectToAction("Index", "Products");
            }

        }
        ////[HttpPost]
        //public ActionResult ReturnFromPayPal()
        //{
        //    //Recieve IPN request

        //    var formVals = new Dictionary<string, string>();
        //    formVals.Add("cmd", "_notify-sych");
        //    formVals.Add("at", "eG_hkGDHC-hYxU7d0u6yM5Nl_e-Uk7IdiTUUaCRV1AvL0PfYFHUZt1ZUK6y");
        //    formVals.Add("tx", Request["tx"]);

        //    string response = GetPayPalResponse(formVals, true);

        //    if (response.Contains("SUCCESS"))
        //    {
        //        string transactionId = GetPDTValue(response, "txn_id");
        //        string sAmountPaid = GetPDTValue(response, "mc_gross");
        //        string deviceId = GetPDTValue(response, "custom");
        //        string paypayEmail = GetPDTValue(response, "payer_email");
        //        string Item = GetPDTValue(response, "item_name");
        //        string Id = GetPDTValue(response, "item_number");

        //        //Get Order

        //        Order order = orderService.GetOrder(Id);

        //        //Valid Order?
        //        if (order != null)
        //        {
        //            //Update the Payment Processed.
        //            order.OrderStatus = "Payment Processed";
        //            order.OrderStatusDate = @DateTime.Now;
        //            orderService.UpdateOrder(order);

        //            var email = new EmailService();
        //            var message = new IdentityMessage();

        //            message.Destination = paypayEmail;
        //            message.Subject = "AskYourMechanicDon.com New Order: " + order.OrderNumber + " Recieved";
        //            message.Body = "Email From: AskYourMechanicDon.com Message: Thank you for your order: " + order.OrderNumber;

        //            email.SendAsync(message);

        //            //Email Admin 
        //            message.Destination = "admin@askyourmechanicdon.com,donmorgan@shaw.ca";
        //            message.Subject = "AskYourMechanicDon.com New Question: " + order.OrderNumber;
        //            message.Body = "Email From: AskYourMechanicDon.com Message: A New Question: " + order.OrderNumber;
        //        }
        //        else
        //        {
        //            return View();
        //        }
        //    }
        //    else
        //    {
        //        LogRequest(response);
        //        ViewBag.Message = "Something Went Wrong";
        //    }

        //    ViewBag.Message = "Success";
        //    return View();

        //}

        //private string GetPayPalResponse(Dictionary<string, string> formVals, bool useSandbox)
        //{
        //    string paypalURL = useSandbox ? "https://www.sandbox.paypal.com/cgi-bin/webscr" : "https://www.paypal.com/cgi-bin/webscr";

        //    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(paypalURL);

        //    //Set values for the request back

        //    req.Method = "POST";
        //    req.ContentType = "application/x-www-fomr-urlendcoded";
        //    byte[] parm = Request.BinaryRead(Request.ContentLength);
        //    string strRequest = Encoding.ASCII.GetString(parm);

        //    StringBuilder sb = new StringBuilder();
        //    sb.Append(strRequest);

        //    foreach (string key in formVals.Keys)
        //    {
        //        sb.AppendFormat("&{0}={1}", key, formVals[key]);
        //    }

        //    strRequest += sb.ToString();
        //    req.ContentType = strRequest.Length.ToString();

        //    string response = "";
        //    using (StreamWriter streamOut = new StreamWriter(req.GetRequestStream(), System.Text.Encoding.ASCII))
        //    {
        //        streamOut.Write(strRequest);
        //        streamOut.Close();
        //        using (StreamReader streamIn = new StreamReader(req.GetResponse().GetResponseStream()))
        //        {
        //            response = streamIn.ReadToEnd();
        //        }
        //    }

        //    return response;


        //}
        //private string GetPDTValue(string pdt, string key)
        //{
        //    string[] keys = pdt.Split('\n');
        //    string thisVal = "";
        //    string thisKey = "";
        //    foreach (string s in keys)
        //    {
        //        string[] bits = s.Split('=');
        //        if (bits.Length > 1)
        //        {
        //            thisVal = bits[1];
        //            thisKey = bits[0];
        //            if (thisKey.Equals(key, StringComparison.InvariantCultureIgnoreCase))
        //                break;
        //        }

        //    }
        //    return thisVal;
        //}

        //private void LogRequest(string requestIn)
        //{
        //    Request r = new Request
        //    {
        //        Message = requestIn
        //    };
        //    requestContext.Insert(r);
        //    requestContext.Commit();
        //}

    }
}