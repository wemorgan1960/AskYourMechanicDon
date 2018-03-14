using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AskYourMechanicDon.Core.Contracts;
using AskYourMechanicDon.Core.Models;
using AskYourMechanicDon.Core.ViewModels;

namespace AskYourMechanicDon.WebUI.Controllers
{

    public class BasketController : Controller
    {
        IRepository<Customer> customers;
        IBasketService basketService;
        IOrderService orderService;
        IRepository<Request> requestContext;
        IRepository<Error> errorContext;

        public BasketController(IBasketService BasketService, IOrderService OrderService, IRepository<Customer> Customers, 
            IRepository<Request> requestcontext, IRepository<Error> errorcontext)
        {
            this.basketService = BasketService;
            this.orderService = OrderService;
            this.customers = Customers;
            this.requestContext = requestcontext;
            this.errorContext = errorcontext;
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
        public ActionResult ReviewPlaceOrder()
        {
            ViewBag.IsIndexHome = false;

            var basketItems = basketService.GetBasketItems(this.HttpContext);

            if (basketItems != null)
            {
                OrderNumberOrderItemsViewModel model = new OrderNumberOrderItemsViewModel();

                //Get the customer UserId
                Customer customer = customers.Collection().FirstOrDefault(c => c.Email == User.Identity.Name);

                //Set Order Number and Create a new order
                var order = new Order
                {
                    CustomerUserId = customer.UserId,
                    OrderNumber = customer.UserId + Common.GetRandomInvoiceNumber(),
                    OrderStatus = "Order Created Not Commited",
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
                model.OrderItems = orderService.GetOrderItemList(order.Id);

                //Clear the basket
                basketService.ClearBasket(this.HttpContext);

                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Products");
            }

        }
        [HttpPost]
        public ActionResult ReturnFromPayPal()
        {
            var ret = GetResponse();

            ViewBag.Cancel = ret;

            return View();

        }


        [HttpPost]
        public void PlaceOrder(Order order)
        {
            //var basketItems = basketService.GetBasketItems(this.HttpContext);
            //var order = new Order
            //{
            //    OrderNumber = Common.GetRandomInvoiceNumber()
            //};

            ////Get the customer UserId
            //Customer customer = customers.Collection().FirstOrDefault(c => c.Email == User.Identity.Name);
            //order.CustomerUserId = customer.UserId;

            ////Get the Invoice and Status
            //order.InvoiceNumber = "AskDon" + @DateTime.Now.Year + order.OrderNumber;
            //order.OrderStatus = "Order Created";
            //order.OrderStatusDate = @DateTime.Now;

            ////Create the Order
            //orderService.CreateOrder(order, basketItems);

            //Update the Payment Processed.
            //order.OrderStatus = "Payment Pending";
            //order.OrderStatusDate = @DateTime.Now;
            //orderService.UpdateOrder(order);



            ////Email Customer
            //string CustomerEmail = User.Identity.Name; ;

            //var email = new EmailService();
            //var message = new IdentityMessage();

            //message.Destination = CustomerEmail;
            //message.Subject = "AskYourMechanicDon.com New Order: " + order.OrderNumber + " Recieved";
            //message.Body = "Email From: AskYourMechanicDon.com Message: Thank you for your order: " + order.OrderNumber;

            //email.SendAsync(message);

            ////Email Admin 
            //message.Destination = "admin@askyourmechanicdon.com,donmorgan@shaw.ca;
            //message.Subject = "AskYourMechanicDon.com New Question: " + order.OrderNumber;
            //message.Body= "Email From: AskYourMechanicDon.com Message: A New Question: " + order.OrderNumber;


            //basketService.ClearBasket(this.HttpContext);

        }

        [HttpPost]
        public ActionResult CancelFromPayPal()
        {
            var ret = GetResponse();

            ViewBag.Cancel = ret;

            return View();
        }

        [HttpPost]
        public ActionResult NotifyFromPayPal()
        {
           var ret = GetResponse();

            ViewBag.Notify = ret;

            return View();
        }






        //IPN Listener

        [HttpPost]
        public HttpStatusCodeResult IPNPayPal()
        {
            //Store the IPN received from PayPal
            LogRequest(Request);

            //Fire and forget verification task
            Task.Run(() => VerifyTask(Request));

            //Reply back a 200 code
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        private void VerifyTask(HttpRequestBase ipnRequest)
        {
            var verificationResponse = string.Empty;

            try
            {
                var verificationRequest = (HttpWebRequest)WebRequest.Create("https://www.sandbox.paypal.com/cgi-bin/webscr");

                //Set values for the verification request
                verificationRequest.Method = "POST";
                verificationRequest.ContentType = "application/x-www-form-urlencoded";
                var param = Request.BinaryRead(ipnRequest.ContentLength);
                var strRequest = Encoding.ASCII.GetString(param);

                //Add cmd=_notify-validate to the payload
                strRequest = "cmd=_notify-validate&" + strRequest;
                verificationRequest.ContentLength = strRequest.Length;

                //Attach payload to the verification request
                var streamOut = new StreamWriter(verificationRequest.GetRequestStream(), Encoding.ASCII);
                streamOut.Write(strRequest);
                streamOut.Close();

                //Send the request to PayPal and get the response
                var streamIn = new StreamReader(verificationRequest.GetResponse().GetResponseStream());
                verificationResponse = streamIn.ReadToEnd();
                streamIn.Close();

            }
            catch (Exception e)
            {
                //Capture exception for manual investigation
                Error err = new Error
                {
                    Message = e.ToString()
                };
                errorContext.Insert(err);
                errorContext.Commit();
            }

            ProcessVerificationResponse(verificationResponse);
        }


        private void LogRequest(HttpRequestBase request)
        {
            Request r = new Request
            {
                Message = request.ToString()
            };
            requestContext.Insert(r);
            requestContext.Commit();
        }

        private void ProcessVerificationResponse(string verificationResponse)
        {
            if (verificationResponse.Equals("VERIFIED"))
            {
                // check that Payment_status=Completed
                // check that Txn_id has not been previously processed
                // check that Receiver_email is your Primary PayPal email
                // check that Payment_amount/Payment_currency are correct
                // process payment
            }
            else if (verificationResponse.Equals("INVALID"))
            {
                //Log for manual investigation
            }
            else
            {
                //Log error
            }
        }



        //Read Response

        private string status;
        private WebRequest request;
        private Stream dataStream;

        public String Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
            }
        }

        public string GetResponse()
        {
            // Get the original response.
            WebResponse response = request.GetResponse();

            this.Status = ((HttpWebResponse)response).StatusDescription;

            // Get the stream containing all content returned by the requested server.
            dataStream = response.GetResponseStream();

            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);

            // Read the content fully up to the end.
            string responseFromServer = reader.ReadToEnd();

            // Clean up the streams.
            reader.Close();
            dataStream.Close();
            response.Close();

            return responseFromServer;
        }

    }
}