using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EducationalServices.Models;
using Microsoft.AspNet.Identity;

namespace EducationalServices.Controllers
{
    public class ModulesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        [HttpPost]
        public ActionResult Cancel()
        {
            TempData["Cancel"] = "Your Order Has Been Cancelled";
            return RedirectToAction("RegisterForModule");
        }
        [HttpGet]
        public ActionResult RegisteredStudent()
        {
            var UserId = User.Identity.GetUserId();
            if (User.IsInRole("Admin"))
            {
                var StudentModules = db.StudentModules.ToList();
                return View(StudentModules);
            }
            else
            {
                var StudentModules = db.StudentModules.Where(x=>x.StudentId== UserId).ToList();
                return View(StudentModules);
            }
        }
        [HttpGet]
        public ActionResult RegisterForModule()
        {
            var modules = db.Modules.ToList();
            return View(modules);
        }
        [HttpGet]
        public ActionResult Payment(int moduleId)
        {
            var modules = db.Modules.Where(x => x.ModuleId == moduleId).FirstOrDefault();
            return View(modules);
        }

        [HttpGet]
        public ActionResult MakePayment(int? CardId,int ModuleId,decimal Price)
        {
            ViewBag.ModuleId = ModuleId;
            ViewBag.Price = Price;
            var Card = db.Cards.Where(x => x.CardId == CardId).FirstOrDefault();
            return View(Card);
        }
        [HttpPost]
        public ActionResult MakePayment(int ModuleId)
        {
            var Modules = db.Modules.Find(ModuleId);

            StudentModule studentModule = new StudentModule();
            studentModule.ModuleId = ModuleId;
            studentModule.StudentId = User.Identity.GetUserId();
            studentModule.Status = "Paid";
            studentModule.TotalPaid = Modules.Price;
            db.StudentModules.Add(studentModule);
            db.SaveChanges();

            TempData["Success"] = "Your Order Has Place. Thank You For Your Time!";
            return RedirectToAction("RegisteredStudent");
        }


        // GET: Modules
        public ActionResult Index()
        {
            return View(db.Modules.ToList());
        }

        // GET: Modules/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Module module = db.Modules.Find(id);
            if (module == null)
            {
                return HttpNotFound();
            }
            return View(module);
        }

        // GET: Modules/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Modules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ModuleId,Subject,SubjectCode,Description,DurationInHours,Difficulty,Price")] Module module)
        {
            if (ModelState.IsValid)
            {
                db.Modules.Add(module);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(module);
        }

        // GET: Modules/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Module module = db.Modules.Find(id);
            if (module == null)
            {
                return HttpNotFound();
            }
            return View(module);
        }

        // POST: Modules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ModuleId,Subject,SubjectCode,Description,DurationInHours,Difficulty,Price")] Module module)
        {
            if (ModelState.IsValid)
            {
                db.Entry(module).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(module);
        }

        // GET: Modules/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Module module = db.Modules.Find(id);
            if (module == null)
            {
                return HttpNotFound();
            }
            return View(module);
        }

        // POST: Modules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Module module = db.Modules.Find(id);
            db.Modules.Remove(module);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
