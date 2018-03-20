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

        public BasketController(IBasketService basketService, IOrderService orderService, IRepository<Product> product,
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
            if (model.Count != 0)
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

        public ActionResult Return()
        {
            ViewBag.IsIndexHome = false;

            return View();
        }

        public ActionResult Cancel()
        {
            ViewBag.IsIndexHome = false;

            return View();
        }

    }
}