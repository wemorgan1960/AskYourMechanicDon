﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using AskYourMechanicDon.Core.Contracts;
using AskYourMechanicDon.Core.Models;
using AskYourMechanicDon.Core.ViewModels;
using AskYourMechanicDon.WebUI.Models;

namespace AskYourMechanicDon.WebUI.Controllers
{
    public class HomeController : Controller
    {
        IRepository<Product> context;
        IRepository<ProductCategory> productCategories;

        public HomeController(IRepository<Product> productContext, IRepository<ProductCategory> productCategoryContext)
        {
            context = productContext;
            productCategories = productCategoryContext;
        }
        public ActionResult Index()
        {
            ViewBag.IsIndexHome = true;
            return View() ;
        }

        public ActionResult Products(string Category = null)
        {
            ViewBag.IsIndexHome = false;

            List<Product> products = context.Collection().ToList();
            List<ProductCategory> categories = productCategories.Collection().ToList();

            if (Category == null)
            {
            products = context.Collection().ToList();
            }
            else
            {
                products = context.Collection().Where(p => p.Category == Category).ToList();
            }

            ProductListViewModel model = new ProductListViewModel();
            model.Products = products;
            model.ProductCategories = categories;

            return View(model);
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

        public ActionResult About()
        {
            ViewBag.IsIndexHome = false;
            ViewBag.Message = "About askyourmechanicdon.com";

            return View();
        }
        public ActionResult Disclaimer()
        {
            ViewBag.IsIndexHome = false;
            ViewBag.Message = "Disclaimer";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.IsIndexHome = false;
            ViewBag.Message = "askyourmechanicdon.com contact page.";

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ContactAsync(EmailFormModel model)
        {
            ViewBag.IsIndexHome = false;
            if (ModelState.IsValid)
            {
                var subject = "AskYourMechanicDon.com Contact Form Email";
                var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
                var fromAddress = model.FromEmail;
                var toAddress = "admin@askyourmechanicdon.com";
                var emailBody = string.Format(body, model.FromName, model.FromEmail, model.Message);

                var smtp = new SmtpClient();
                {
                    smtp.Host = "smtp.askyourmechanicdon.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = false;
                    smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                    smtp.Credentials = new NetworkCredential("admin@askyourmechanicdon.com", "TtLUVAz5");
                    smtp.Timeout = 20000;
                }

                smtp.Send(toAddress, toAddress, subject, emailBody);
                return RedirectToAction("Sent");
            }
            else
            {
                return View();
            }

        }
        public ActionResult Sent()
        {
            ViewBag.IsIndexHome = false;
            return View();
        }
    }
}