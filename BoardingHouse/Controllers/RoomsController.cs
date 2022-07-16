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
    public class RoomsController : Controller
    {
        private BOARDING_HOUSEEntities db = new BOARDING_HOUSEEntities();

        // GET: Rooms
        public ActionResult Index()
        {
            var rooms = db.Rooms.Include(r => r.Account).Include(r => r.Category).Include(r => r.District);
            return View(rooms.ToList());
        }

        // GET: Rooms/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Room room = db.Rooms.Find(id);
            if (room == null)
            {
                return HttpNotFound();
            }
            return View(room);
        }

        // GET: Rooms/Create
        public ActionResult Create()
        {
            ViewBag.OwnerId = new SelectList(db.Accounts, "Id", "Email");
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name");
            ViewBag.DistrictId = new SelectList(db.Districts, "Id", "Name");
            return View();
        }

        // POST: Rooms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,OwnerId,CategoryId,DistrictId,Title,Description,Address,Coordinates,Price,Images,CreateAt,UpdateAt,Is_Rented,Is_Delete")] Room room)
        {
            if (ModelState.IsValid)
            {
                room.Id = Guid.NewGuid();
                db.Rooms.Add(room);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.OwnerId = new SelectList(db.Accounts, "Id", "Email", room.OwnerId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", room.CategoryId);
            ViewBag.DistrictId = new SelectList(db.Districts, "Id", "Name", room.DistrictId);
            return View(room);
        }

        // GET: Rooms/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Room room = db.Rooms.Find(id);
            if (room == null)
            {
                return HttpNotFound();
            }
            ViewBag.OwnerId = new SelectList(db.Accounts, "Id", "Email", room.OwnerId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", room.CategoryId);
            ViewBag.DistrictId = new SelectList(db.Districts, "Id", "Name", room.DistrictId);
            return View(room);
        }

        // POST: Rooms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,OwnerId,CategoryId,DistrictId,Title,Description,Address,Coordinates,Price,Images,CreateAt,UpdateAt,Is_Rented,Is_Delete")] Room room)
        {
            if (ModelState.IsValid)
            {
                db.Entry(room).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OwnerId = new SelectList(db.Accounts, "Id", "Email", room.OwnerId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", room.CategoryId);
            ViewBag.DistrictId = new SelectList(db.Districts, "Id", "Name", room.DistrictId);
            return View(room);
        }

        // GET: Rooms/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Room room = db.Rooms.Find(id);
            if (room == null)
            {
                return HttpNotFound();
            }
            return View(room);
        }

        // POST: Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Room room = db.Rooms.Find(id);
            db.Rooms.Remove(room);
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
