using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AskYourMechanicDon.Core.Contracts;
using AskYourMechanicDon.Core.Models;

namespace AskYourMechanicDon.WebUI.Controllers
{
    [Authorize(Roles = RoleName.AskAdmin)]
    public class ErrorController : Controller
    {
        IRepository<Error> errorContext;

        public ErrorController(IRepository<Error> Errors)
        {
            this.errorContext = Errors;
        }
        // GET: Error
        public ActionResult Index()
        {
            ViewBag.IsIndexHome = false;
            List<Error> errors = errorContext.Collection().ToList();
            return View(errors);
        }

        // GET: Error/Details/5
        public ActionResult Details(string id)
        {
            ViewBag.IsIndexHome = false;
            Error errors = errorContext.Find(id);
            if (errors == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(errors);
            }
        }

      
        // GET: Error/Edit/5
        public ActionResult Edit(string id)
        {
            ViewBag.IsIndexHome = false;
            Error error = errorContext.Find(id);
            if (error == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(error);
            }
        }

        // POST: Error/Edit/5
        [HttpPost]
        public ActionResult Edit(Error error, string id, FormCollection collection)
        {
            ViewBag.IsIndexHome = false;
            Error ToEdit = errorContext.Find(id);
            if (ToEdit == null)
            {
                return HttpNotFound();
            }
            else
            {
                if (!ModelState.IsValid)
                {
                    return View(ToEdit);
                }

                ToEdit.Message = error.Message;


                errorContext.Commit();

                return RedirectToAction("Index");
            }
        }

    }
}
