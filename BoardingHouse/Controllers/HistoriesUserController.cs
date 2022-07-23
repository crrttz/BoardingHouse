using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BoardingHouse.Models;

namespace BoardingHouse.Controllers
{
    public class HistoriesUserController : Controller
    {
        private BOARDING_HOUSEEntities db = new BOARDING_HOUSEEntities();

        // GET: HistoriesUser
        public ActionResult Index()
        {
            var histories = db.Histories.Include(h => h.Account).Include(h => h.Room);
            return View(histories.ToList());
        }

        // GET: HistoriesUser/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            History history = db.Histories.Find(id);
            if (history == null)
            {
                return HttpNotFound();
            }
            return View(history);
        }

        // GET: HistoriesUser/Create
        public ActionResult Create()
        {
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Email");
            ViewBag.RoomId = new SelectList(db.Rooms, "Id", "Title");
            return View();
        }

        // POST: HistoriesUser/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AccountId,RoomId,StartDate,EndDate,Is_Delete")] History history)
        {
            if (ModelState.IsValid)
            {
                history.Id = Guid.NewGuid();
                db.Histories.Add(history);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Email", history.AccountId);
            ViewBag.RoomId = new SelectList(db.Rooms, "Id", "Title", history.RoomId);
            return View(history);
        }

        // GET: HistoriesUser/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            History history = db.Histories.Find(id);
            if (history == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Email", history.AccountId);
            ViewBag.RoomId = new SelectList(db.Rooms, "Id", "Title", history.RoomId);
            return View(history);
        }

        // POST: HistoriesUser/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AccountId,RoomId,StartDate,EndDate,Is_Delete")] History history)
        {
            if (ModelState.IsValid)
            {
                db.Entry(history).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Email", history.AccountId);
            ViewBag.RoomId = new SelectList(db.Rooms, "Id", "Title", history.RoomId);
            return View(history);
        }

        // GET: HistoriesUser/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            History history = db.Histories.Find(id);
            if (history == null)
            {
                return HttpNotFound();
            }
            return View(history);
        }

        // POST: HistoriesUser/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            History history = db.Histories.Find(id);
            db.Histories.Remove(history);
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
