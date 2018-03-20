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
            string result = Request.Params.ToString();
            LogRequest(result);

            var formVals = new Dictionary<string, string>();
            formVals.Add("cmd", "_notify-validate");
            formVals.Add("at", "7BiLzbRCzn_Ob8ZfKMNvliNH4R3MhaF_B7sptpteroc0D-glX2lhV4Ci3sS");
            formVals.Add("txn_id", Request["txn_id"]);
            formVals.Add("payment_status", Request["payment_status"]);
            formVals.Add("payer_email", Request["payer_email"]);
            formVals.Add("mc_gross", Request["mc_gross"]);
            formVals.Add("item_number", Request["item_number"]);

            //Fire and forget verification task
            Task.Run(() => VerifyTask(formVals, Request));

            //Reply back a 200 code
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        private void VerifyTask(Dictionary<string, string> formVals, HttpRequestBase ipnRequest)
        {
            var verificationResponse = string.Empty;
            var strRequest = string.Empty;

            try
            {
                var verificationRequest = (HttpWebRequest)WebRequest.Create("https://www.paypal.com/cgi-bin/webscr");

                //Set values for the verification request
                verificationRequest.Method = "POST";
                verificationRequest.ContentType = "application/x-www-form-urlencoded";
                byte[] param = Request.BinaryRead(ipnRequest.ContentLength);
                strRequest = Encoding.ASCII.GetString(param);

                //Add cmd=_notify-validate to the payload
                StringBuilder sb = new StringBuilder();
                sb.Append(strRequest);

                foreach(string key in formVals.Keys)
                {
                    sb.AppendFormat("&{0}={1}", key, formVals[key]);
                }
                strRequest += sb.ToString();

                //strRequest = "cmd=_notify-validate&" + strRequest;
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

            ProcessVerificationResponse(verificationResponse, strRequest);
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

        private void ProcessVerificationResponse(string verificationResponse, string request)
        {
            var email = new EmailService();
            var message = new IdentityMessage();

            if (verificationResponse.Contains("VERIFIED"))
            {
                // check that Payment_status=Completed
                string transactionId = GetPDTValue(request, "txn_id");
                string paymentStatus = GetPDTValue(request, "payment_status");
                string paypayEmail = GetPDTValue(request, "payer_email");
                string sAmountPaid = GetPDTValue(request, "mc_gross");
                string Id = GetPDTValue(request, "item_number1");

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
            string[] keys = pdt.Split('&');
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