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
    public class RatingsUserController : Controller
    {
        private BOARDING_HOUSEEntities db = new BOARDING_HOUSEEntities();

        // GET: RatingsUser
        public ActionResult Index()
        {
            var ratings = db.Ratings.Include(r => r.Account).Include(r => r.Room);
            return View(ratings.ToList());
        }

        // GET: RatingsUser/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rating rating = db.Ratings.Find(id);
            if (rating == null)
            {
                return HttpNotFound();
            }
            return View(rating);
        }

        // GET: RatingsUser/Create
        public ActionResult Create()
        {
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Email");
            ViewBag.RoomId = new SelectList(db.Rooms, "Id", "Title");
            return View();
        }

        // POST: RatingsUser/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,RoomId,AccountId,Rating1,CreateAt,UpdateAt,Is_Delete")] Rating rating)
        {
            if (ModelState.IsValid)
            {
                rating.Id = Guid.NewGuid();
                db.Ratings.Add(rating);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Email", rating.AccountId);
            ViewBag.RoomId = new SelectList(db.Rooms, "Id", "Title", rating.RoomId);
            return View(rating);
        }

        // GET: RatingsUser/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rating rating = db.Ratings.Find(id);
            if (rating == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Email", rating.AccountId);
            ViewBag.RoomId = new SelectList(db.Rooms, "Id", "Title", rating.RoomId);
            return View(rating);
        }

        // POST: RatingsUser/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,RoomId,AccountId,Rating1,CreateAt,UpdateAt,Is_Delete")] Rating rating)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rating).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Email", rating.AccountId);
            ViewBag.RoomId = new SelectList(db.Rooms, "Id", "Title", rating.RoomId);
            return View(rating);
        }

        // GET: RatingsUser/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rating rating = db.Ratings.Find(id);
            if (rating == null)
            {
                return HttpNotFound();
            }
            return View(rating);
        }

        // POST: RatingsUser/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Rating rating = db.Ratings.Find(id);
            db.Ratings.Remove(rating);
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
