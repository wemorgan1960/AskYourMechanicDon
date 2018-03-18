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
        IRepository<Product> ProductContext;
        IRepository<ProductCategory> productCategoriesContext;

        public ProductsController( IRepository<Product> product, IRepository<ProductCategory> productCategory)
        {
            this.ProductContext = product;
            this.productCategoriesContext = productCategory;
        }


        public ActionResult Index()
        {
            ViewBag.IsIndexHome = false;

            List<Product> products = ProductContext.Collection().ToList();

            return View(products);
        }

        public ActionResult Details(string Id)
        {
            ViewBag.IsIndexHome = false;
            Product product = ProductContext.Find(Id);
            if (product == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(product);
            }

        }

    }
}