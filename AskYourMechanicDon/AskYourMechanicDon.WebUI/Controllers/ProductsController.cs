using AskYourMechanicDon.Core.Contracts;
using AskYourMechanicDon.Core.Models;
using AskYourMechanicDon.Core.ViewModels;
using AskYourMechanicDon.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AskYourMechanicDon.WebUI.Controllers
{
    public class ProductsController : Controller
    {
        IRepository<Product> context;
        IRepository<ProductCategory> productCategories;
        IBasketService basketService;

        public ProductsController(IBasketService BasketService, IRepository<Product> productContext, IRepository<ProductCategory> productCategoryContext)
        {
            this.context = productContext;
            this.productCategories = productCategoryContext;
            this.basketService = BasketService;
        }
        public ActionResult Index()
        {
            ViewBag.IsIndexHome = false;

            List<Product> products = context.Collection().ToList();

            return View(products);
        }

        public ActionResult Details(string Id)
        {
            ViewBag.IsIndexHome = false;
            Product product = context.Find(Id);
            if (product == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(product);
            }

        }
        public ActionResult AddToCart(string Id)
        {
            ViewBag.IsIndexHome = false;
            Product product = context.Find(Id);
            if (product == null)
            {
                return HttpNotFound();
            }
            else
            {
                ProductManagerViewModel viewModel = new ProductManagerViewModel
                {
                    Product = product,
                    ProductCategories = productCategories.Collection()
                };

                return View(viewModel);
            }
        }
        [HttpPost]
        public ActionResult AddToCart(Product product, string Id, HttpPostedFileBase file)
        {
            ViewBag.IsIndexHome = false;

            string vin = product.VIN;
            string question = product.Question;


            basketService.AddToBasket(this.HttpContext, Id, vin, question);


            return RedirectToAction("Index");

        }
    }
}