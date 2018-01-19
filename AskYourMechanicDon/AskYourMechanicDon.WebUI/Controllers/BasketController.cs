﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AskYourMechanicDon.Core.Contracts;

namespace AskYourMechanicDon.WebUI.Controllers
{
    public class BasketController : Controller
    {
        IBasketService basketService;

        public BasketController(IBasketService BasketService)
        {
            this.basketService = BasketService;
        }

        // GET: Basket
        public ActionResult Index()
        {
            ViewBag.IsIndexHome = false;
            var model = basketService.GetBasketItems(this.HttpContext);

            return View(model);
        }

        public ActionResult AddToBasket(string Id, string Vin, string Question)
        {
            ViewBag.IsIndexHome = false;
            basketService.AddToBasket(this.HttpContext, Id, Vin, Question);

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

    }
}