using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AskYourMechanicDon.Core.Contracts;
using AskYourMechanicDon.Core.Models;
using AskYourMechanicDon.DataAccess.InMemory;

namespace AskYourMechanicDon.WebUI.Controllers
{
    [Authorize(Roles = RoleName.AskAdmin)]
    public class ProductCategoryManagerController : Controller
    {
        IRepository<ProductCategory> context;

        public ProductCategoryManagerController(IRepository<ProductCategory> productCategoryContext)
        {
            context = productCategoryContext;
        }
        // GET: ProductManager
        public ActionResult Index()
        {
            List<ProductCategory> productCategories = context.Collection().ToList();
            ViewBag.IsIndexHome = false;
            return View(productCategories);
        }
        public ActionResult Create()
        {
            ProductCategory productCategory = new ProductCategory();
            ViewBag.IsIndexHome = false;
            return View(productCategory);
        }
        [HttpPost]
        public ActionResult Create(ProductCategory productCategory)
        {
            if (!ModelState.IsValid)
            {
                return View(productCategory);

            }
            else
            {
                context.Insert(productCategory);
                context.Commit();
                ViewBag.IsIndexHome = false;
                return RedirectToAction("Index");
            }
        }
        public ActionResult Edit(string Id)
        {
            ViewBag.IsIndexHome = false;
            ProductCategory productCategory = context.Find(Id);
            if (productCategory == null)
            {
                
                return HttpNotFound();
            }
            else
            {
                return View(productCategory);
            }
        }
        [HttpPost]
        public ActionResult Edit(ProductCategory productCategory, string Id)
        {
            ViewBag.IsIndexHome = false;
            ProductCategory productCategoryToEdit = context.Find(Id);
            if (productCategory == null)
            {
                return HttpNotFound();
            }
            else
            {
                if (!ModelState.IsValid)
                {
                    return View(productCategory);
                }
                productCategoryToEdit.Category = productCategory.Category;

                context.Commit();

                return RedirectToAction("Index");
            }

        }
        public ActionResult Delete(string Id)
        {
            ViewBag.IsIndexHome = false;
            ProductCategory productCategoryToDelete = context.Find(Id);
            if (productCategoryToDelete == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(productCategoryToDelete);
            }
        }
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult ConfirmDelete(string Id)
        {
            ViewBag.IsIndexHome = false;
            ProductCategory productCategoryToDelete = context.Find(Id);
            if (productCategoryToDelete == null)
            {
                return HttpNotFound();
            }
            else
            {
                context.Delete(Id);
                context.Commit();
                return RedirectToAction("Index");
            }

        }
    }
}