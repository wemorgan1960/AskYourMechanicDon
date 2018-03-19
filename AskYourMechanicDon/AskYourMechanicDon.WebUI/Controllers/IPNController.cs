using AskYourMechanicDon.Core.Contracts;
using AskYourMechanicDon.Core.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AskYourMechanicDon.WebUI.Controllers
{
    public class IPNController : Controller
    {
        IRepository<Customer> customersContext;
        IBasketService basketService;
        IOrderService orderService;
        IRepository<Product> productContext;
        IRepository<ProductCategory> productCategoryContext;
        IRepository<Request> requestContext;
        IRepository<Error> errorContext;

        public IPNController(IBasketService basketService, IOrderService orderService, IRepository<Product> product,
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


        [HttpPost]
        public HttpStatusCodeResult Receive()
        {
            //Store the IPN received from PayPal
            string result = Request.ToString();
            LogRequest(result);

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
            catch (Exception exception)
            {
                //Capture exception for manual investigation
                Error e = new Error();
                e.Message= exception.Message;
                errorContext.Commit();

            }

            ProcessVerificationResponse(verificationResponse);
        }


        private void LogRequest(string requestIn)
        {
            Request r = new Request
            {
                Message = requestIn
            };
            requestContext.Insert(r);
            requestContext.Commit();
        }

        private void ProcessVerificationResponse(string verificationResponse)
        {
            var email = new EmailService();
            var message = new IdentityMessage();

            if (verificationResponse.Equals("VERIFIED"))
            {
                // check that Payment_status=Completed
                string transactionId = GetPDTValue(verificationResponse, "txn_id");
                string sAmountPaid = GetPDTValue(verificationResponse, "mc_gross");
                string deviceId = GetPDTValue(verificationResponse, "custom");
                string paypayEmail = GetPDTValue(verificationResponse, "payer_email");
                string Item = GetPDTValue(verificationResponse, "item_name");
                string Id = GetPDTValue(verificationResponse, "item_number");

                // check that Txn_id has not been previously processed
                //Get the Order
                if (Id != null)
                {
                    var order = new Order();
                    order = orderService.GetOrderFromOrderNumber(Id);

                    if (order != null)
                    {
                        if (order.PayPalTxnId == null && orderService.IsOrderPayPalTranxFound(transactionId))
                        {

                            // check that Receiver_email is your Primary PayPal email
                            List<Customer> customers = customersContext.Collection().ToList();
                            var result = (from c in customers
                                           where c.Email == paypayEmail
                                           select c.Id ).FirstOrDefault();

                            if (paypayEmail !=result)
                            {
                                //update the customer email
                                Customer customer = customersContext.Find(result);

                                customer.Email = paypayEmail;
                                customersContext.Update(customer);
                            }

                            // check that Payment_amount/Payment_currency are correct
                            decimal OrderTotal = orderService.OrderTotalFromOrderNumber(order.OrderNumber);
                            string warningAmount = "";

                            if (OrderTotal.ToString() != sAmountPaid)
                            {
                                warningAmount = "Amount paid different than Order";
                            }

                            //Update the Payment Processed.
                            order.PayPalTxnId = transactionId;
                            order.PayPalPaidDate = @DateTime.Now;
                            order.OrderStatus = "Payment Processed";
                            order.OrderStatusDate = @DateTime.Now;
                            order.AmountPaid = sAmountPaid;
                            orderService.UpdateOrder(order);

                            // Email the Reciever

                            message.Destination = paypayEmail;
                            message.Subject = "AskYourMechanicDon.com New Order: " + order.OrderNumber + " Recieved";

                            var a1 = "AskYourMechanicDon.com";
                            var a2 = "Your Order: " + order.OrderNumber + " was processed through PayPal via Transaction: " + order.PayPalTxnId;
                            var a3 = "For the amount of CAD $" + sAmountPaid;
                            var a4 = "Thank you for your order!";

                            var body = "<p>Email From: {0} </p><p>Message:</p><p>{1}</p><p>{2}</p><p>{3}</p></br></br><p>{a4}</p>";
                            var emailBody = string.Format(body,a1, a2, a3, a4);

                            message.Body = emailBody;

                            email.SendAsync(message);

                            //Email Admin 
                            message.Destination = "admin@askyourmechanicdon.com,donmorgan@shaw.ca";
                            message.Subject = "AskYourMechanicDon.com New Question: " + order.OrderNumber;
                            message.Body = "Email From: AskYourMechanicDon.com Message: A New Question: " + order.OrderNumber + " " + warningAmount;

                            email.SendAsync(message);
                        }

                    }
                }
            }
            else if (verificationResponse.Equals("INVALID"))
            {
                //Log for manual investigation

                //Email Admin 
                message.Destination = "admin@askyourmechanicdon.com,donmorgan@shaw.ca";
                message.Subject = "AskYourMechanicDon.com INVALID PayPal Payment";
                message.Body = "Check the Request Table for the Return from PayPal Response";

                email.SendAsync(message);
            }
            else
            {
                //Log error
                Error e = new Error();
                e.Message = verificationResponse;
                errorContext.Commit();

            }
        }
        private string GetPDTValue(string pdt, string key)
        {
            string[] keys = pdt.Split('\n');
            string thisVal = "";
            string thisKey = "";
            foreach (string s in keys)
            {
                string[] bits = s.Split('=');
                if (bits.Length > 1)
                {
                    thisVal = bits[1];
                    thisKey = bits[0];
                    if (thisKey.Equals(key, StringComparison.InvariantCultureIgnoreCase))
                        break;
                }

            }
            return thisVal;
        }
    }
}