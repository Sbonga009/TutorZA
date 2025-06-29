using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EducationalServices.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

namespace EducationalServices.Controllers
{
    public class TutorModulesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TutorModules
        public ActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                var tutorModules = db.TutorModules.Include(t => t.Module).Include(x => x.User);
                return View(tutorModules.ToList());
            }
            else
            {
                var UserId = User.Identity.GetUserId();
                var tutorModules = db.TutorModules.Include(t => t.Module).Include(x => x.User).Where(x=>x.TutorId== UserId);
                return View(tutorModules.ToList());
            }
        }

        // GET: TutorModules/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TutorModule tutorModule = db.TutorModules.Find(id);
            if (tutorModule == null)
            {
                return HttpNotFound();
            }
            return View(tutorModule);
        }

        // GET: TutorModules/Create
        public ActionResult Create()
        {
            ViewBag.ModuleId = new SelectList(db.Modules, "ModuleId", "Subject");
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var tutorRole = db.Roles.SingleOrDefault(r => r.Name == "Tutor");

            var tutorRoleUsers = tutorRole.Users.Select(ru => ru.UserId).ToList();
            var tutors = userManager.Users.Where(u => tutorRoleUsers.Contains(u.Id)).ToList();
            ViewBag.Tutors = tutors;
            return View();
        }

        // POST: TutorModules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TutorModuleId,ModuleId,TutorId")] TutorModule tutorModule)
        {
            if (ModelState.IsValid)
            {
                db.TutorModules.Add(tutorModule);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ModuleId = new SelectList(db.Modules, "ModuleId", "Subject", tutorModule.ModuleId);
            return View(tutorModule);
        }

        // GET: TutorModules/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TutorModule tutorModule = db.TutorModules.Find(id);
            if (tutorModule == null)
            {
                return HttpNotFound();
            }
            ViewBag.ModuleId = new SelectList(db.Modules, "ModuleId", "Subject", tutorModule.ModuleId);
            return View(tutorModule);
        }

        // POST: TutorModules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TutorModuleId,ModuleId,TutorId")] TutorModule tutorModule)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tutorModule).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ModuleId = new SelectList(db.Modules, "ModuleId", "Subject", tutorModule.ModuleId);
            return View(tutorModule);
        }

        // GET: TutorModules/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TutorModule tutorModule = db.TutorModules.Find(id);
            if (tutorModule == null)
            {
                return HttpNotFound();
            }
            return View(tutorModule);
        }

        // POST: TutorModules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TutorModule tutorModule = db.TutorModules.Find(id);
            db.TutorModules.Remove(tutorModule);
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
