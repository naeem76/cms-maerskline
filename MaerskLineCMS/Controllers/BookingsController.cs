using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MaerskLineCMS.Models;
using Microsoft.Data.OData.Query.SemanticAst;

namespace MaerskLineCMS.Controllers
{
    [Authorize]
    public class BookingsController : Controller
    {
        private dbCMSEntities db = new dbCMSEntities();

        // GET: Bookings
        public ActionResult Index()
        {
            var bookings = db.Bookings.Include(b => b.Cargo).Include(b => b.Shipping).Include(b => b.Warehouse);
            return View(bookings.ToList());
        }

        // GET: Bookings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        // GET: Bookings/Create
        public ActionResult Create()
        {
            var cargoCust = db.Cargoes.Where(s => s.CargoID != 0).ToList();
            IEnumerable<SelectListItem> lstCargoCust = from s in cargoCust
                                                          select new SelectListItem
                                                          {
                                                              Value = s.CargoID.ToString(),
                                                              Text = s.Name.ToString() + " (" + s.Weight.ToString() + "kg)"

                                                          };
            ViewBag.CargoID = new SelectList(lstCargoCust, "Value", "Text");

            var shippingCust = db.Shippings.Where(s => s.ShippingID != 0).ToList();
            IEnumerable<SelectListItem> lstShippingCust = from s in shippingCust
                                                     select new SelectListItem
                                                     {
                                                         Value = s.ShippingID.ToString(),
                                                         Text = s.ShippingDate.ToString("yyyy-MM-dd") + " (" + s.ShippingSource.ToString() + "->" + s.ShippingDestination.ToString() + ")"

                                                     };
            ViewBag.ShippingID = new SelectList(lstShippingCust, "Value", "Text");

            var warehouseCust = db.Warehouses.Where(s => s.WarehouseID != 0).ToList();
            IEnumerable<SelectListItem> lstWarehouseCust = from s in warehouseCust
                                                          select new SelectListItem
                                                          {
                                                              Value = s.WarehouseID.ToString(),
                                                              Text = s.Zone.ToString() + "-" + s.LotNo.ToString() + "-" + s.ContainerNo.ToString()

                                                          };
            ViewBag.WarehouseID = new SelectList(lstWarehouseCust, "Value", "Text");



            //ViewBag.CargoID = new SelectList(db.Cargoes, "CargoID", "Name");
            //ViewBag.ShippingID = new SelectList(db.Shippings, "ShippingID", "ShippingSource");
            //ViewBag.WarehouseID = new SelectList(db.Warehouses, "WarehouseID", "Zone");
            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BookingID,BookingDate,CargoID,ShippingID,WarehouseID")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                booking.BookingDate = DateTime.Now;
                db.Bookings.Add(booking);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var cargoCust = db.Cargoes.Where(s => s.CargoID != 0).ToList();
            IEnumerable<SelectListItem> lstCargoCust = from s in cargoCust
                                                       select new SelectListItem
                                                       {
                                                           Value = s.CargoID.ToString(),
                                                           Text = s.Name.ToString() + " (" + s.Weight.ToString() + "kg)"

                                                       };
            ViewBag.CargoID = new SelectList(lstCargoCust, "Value", "Text", booking.CargoID);

            var shippingCust = db.Shippings.Where(s => s.ShippingID != 0).ToList();
            IEnumerable<SelectListItem> lstShippingCust = from s in shippingCust
                                                          select new SelectListItem
                                                          {
                                                              Value = s.ShippingID.ToString(),
                                                              Text = s.ShippingDate.ToString("yyyy-MM-dd") + " (" + s.ShippingSource.ToString() + "->" + s.ShippingDestination.ToString() + ")"

                                                          };
            ViewBag.ShippingID = new SelectList(lstShippingCust, "Value", "Text", booking.ShippingID);

            var warehouseCust = db.Warehouses.Where(s => s.WarehouseID != 0).ToList();
            IEnumerable<SelectListItem> lstWarehouseCust = from s in warehouseCust
                                                           select new SelectListItem
                                                           {
                                                               Value = s.WarehouseID.ToString(),
                                                               Text = s.Zone.ToString() + "-" + s.LotNo.ToString() + "-" + s.ContainerNo.ToString()

                                                           };
            ViewBag.WarehouseID = new SelectList(lstWarehouseCust, "Value", "Text", booking.WarehouseID);
            //ViewBag.CargoID = new SelectList(db.Cargoes, "CargoID", "Name", booking.CargoID);
            //ViewBag.ShippingID = new SelectList(db.Shippings, "ShippingID", "ShippingSource", booking.ShippingID);
            //ViewBag.WarehouseID = new SelectList(db.Warehouses, "WarehouseID", "Zone", booking.WarehouseID);
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }


            var cargoCust = db.Cargoes.Where(s => s.CargoID != 0).ToList();
            IEnumerable<SelectListItem> lstCargoCust = from s in cargoCust
                                                       select new SelectListItem
                                                       {
                                                           Value = s.CargoID.ToString(),
                                                           Text = s.Name.ToString() + " (" + s.Weight.ToString() + "kg)"

                                                       };
            ViewBag.CargoID = new SelectList(lstCargoCust, "Value", "Text", booking.CargoID);

            var shippingCust = db.Shippings.Where(s => s.ShippingID != 0).ToList();
            IEnumerable<SelectListItem> lstShippingCust = from s in shippingCust
                                                          select new SelectListItem
                                                          {
                                                              Value = s.ShippingID.ToString(),
                                                              Text = s.ShippingDate.ToString("yyyy-MM-dd") + " (" + s.ShippingSource.ToString() + "->" + s.ShippingDestination.ToString() + ")"

                                                          };
            ViewBag.ShippingID = new SelectList(lstShippingCust, "Value", "Text", booking.ShippingID);

            var warehouseCust = db.Warehouses.Where(s => s.WarehouseID != 0).ToList();
            IEnumerable<SelectListItem> lstWarehouseCust = from s in warehouseCust
                                                           select new SelectListItem
                                                           {
                                                               Value = s.WarehouseID.ToString(),
                                                               Text = s.Zone.ToString() + "-" + s.LotNo.ToString() + "-" + s.ContainerNo.ToString()

                                                           };
            ViewBag.WarehouseID = new SelectList(lstWarehouseCust, "Value", "Text", booking.WarehouseID);

            //ViewBag.CargoID = new SelectList(db.Cargoes, "CargoID", "Name", booking.CargoID);
            //ViewBag.ShippingID = new SelectList(db.Shippings, "ShippingID", "ShippingSource", booking.ShippingID);
            //ViewBag.WarehouseID = new SelectList(db.Warehouses, "WarehouseID", "Zone", booking.WarehouseID);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BookingID,BookingDate,CargoID,ShippingID,WarehouseID")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                db.Entry(booking).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var cargoCust = db.Cargoes.Where(s => s.CargoID != 0).ToList();
            IEnumerable<SelectListItem> lstCargoCust = from s in cargoCust
                                                       select new SelectListItem
                                                       {
                                                           Value = s.CargoID.ToString(),
                                                           Text = s.Name.ToString() + " (" + s.Weight.ToString() + "kg)"

                                                       };
            ViewBag.CargoID = new SelectList(lstCargoCust, "Value", "Text", booking.CargoID);

            var shippingCust = db.Shippings.Where(s => s.ShippingID != 0).ToList();
            IEnumerable<SelectListItem> lstShippingCust = from s in shippingCust
                                                          select new SelectListItem
                                                          {
                                                              Value = s.ShippingID.ToString(),
                                                              Text = s.ShippingDate.ToString("yyyy-MM-dd") + " (" + s.ShippingSource.ToString() + "->" + s.ShippingDestination.ToString() + ")"

                                                          };
            ViewBag.ShippingID = new SelectList(lstShippingCust, "Value", "Text", booking.ShippingID);

            var warehouseCust = db.Warehouses.Where(s => s.WarehouseID != 0).ToList();
            IEnumerable<SelectListItem> lstWarehouseCust = from s in warehouseCust
                                                           select new SelectListItem
                                                           {
                                                               Value = s.WarehouseID.ToString(),
                                                               Text = s.Zone.ToString() + "-" + s.LotNo.ToString() + "-" + s.ContainerNo.ToString()

                                                           };
            ViewBag.WarehouseID = new SelectList(lstWarehouseCust, "Value", "Text", booking.WarehouseID);
            //ViewBag.CargoID = new SelectList(db.Cargoes, "CargoID", "Name", booking.CargoID);
            //ViewBag.ShippingID = new SelectList(db.Shippings, "ShippingID", "ShippingSource", booking.ShippingID);
            //ViewBag.WarehouseID = new SelectList(db.Warehouses, "WarehouseID", "Zone", booking.WarehouseID);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Booking booking = db.Bookings.Find(id);
            db.Bookings.Remove(booking);
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
