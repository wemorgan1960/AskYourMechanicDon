using AskYourMechanicDon.Core.Contracts;
using AskYourMechanicDon.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AskYourMechanicDon.WebUI.Controllers
{
    [Authorize(Roles = RoleName.AskAdmin)]
    public class RequestController : Controller
    {
        IRepository<Request> requestContext;

        public RequestController(IRepository<Request> Request)
        {
            this.requestContext = Request;
        }
        // GET: Request
        public ActionResult Index()
        {
            ViewBag.IsIndexHome = false;
            List<Request> requests = requestContext.Collection().ToList();
            return View(requests);
        }

        // GET: Request/Details/5
        public ActionResult Details(string id)
        {
            ViewBag.IsIndexHome = false;
            Request request               = requestContext.Find(id);
            if (request == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(request);
            }
        }

        // GET: Request/Edit/5
        public ActionResult Edit(string id)
        {
            ViewBag.IsIndexHome = false;
            Request request = requestContext.Find(id);
            if (request == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(request);
            }
        }

        // POST: Request/Edit/5
        [HttpPost]
        public ActionResult Edit(Request request, string id, FormCollection collection)
        {
            ViewBag.IsIndexHome = false;
            Request ToEdit = requestContext.Find(id);
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

                ToEdit.Message = request.Message;


                requestContext.Commit();

                return RedirectToAction("Index");
            }
        }

    }
}
