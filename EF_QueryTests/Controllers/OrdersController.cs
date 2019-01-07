using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EF_QueryTests.Models;

namespace EF_QueryTests.Controllers
{
    public class OrdersController : Controller
    {
        private TSQL2012Entities db = new TSQL2012Entities();

        // GET: Orders
        public async Task<ActionResult> Index()
        {
            GetOrdersInfo7();
            var orders = db.Orders.Include(o => o.Employee).Include(o => o.Customer).Include(o => o.Shipper);
            return View(await orders.ToListAsync());
        }

        //SELECT custid, YEAR(orderdate)
        //FROM Sales.Orders
        //ORDER BY 1, 2;
        private void GetOrdersInfo()
        {
            var number = 0;
            var orders = db.Orders.Select(p => new
            {
                CustId = p.custid,
                Year = p.orderdate.Year
            }).OrderBy(p => p.CustId).ThenBy(p => p.Year);
            foreach (var p in orders)
                number = p.Year;
        }

        //SELECT custid, MAX(orderid) AS maxorderid
        //FROM Sales.Orders
        //GROUP BY custid;
        //var groups = db.Phones.GroupBy(p=>p.Company.Name)
        //                  .Select(g => new { Name = g.Key, Count = g.Count()});
        private void GetOrdersInfo1()
        {
            var number = 0;
            var orders = db.Orders.GroupBy(p=>p.custid).
                Select(g => new
            {
                CustId = g.Key,
                Maxorderid = g.Select(x => x.orderid).Max()
            });
            foreach (var p in orders)
                number = p.Maxorderid;
        }

        //SELECT shipperid, SUM(freight) AS totalfreight
        //FROM Sales.Orders
        //GROUP BY shipperid
        //HAVING SUM(freight) > 20000.00;
        private void GetOrdersInfo2()
        {
            decimal number = 0;
            var orders = db.Orders.GroupBy(p => p.shipperid).
                Select(g => new
                {
                    Shipperid = g.Key,
                    Totalfreight = g.Select(x => x.freight).Sum()
                }).Where(x=>x.Totalfreight > 20000);
            foreach (var p in orders)
                number = p.Totalfreight;
        }

        //SELECT orderid, orderdate, custid, empid
        //FROM Sales.Orders
        //WHERE shippeddate = NULL;
        private void GetOrdersInfo3()
        {
            int number = 0;
            var orders = db.Orders.Select(o => new
            {
                Orderid = o.orderid,
                Orderdate = o.orderdate,
                Custid = o.custid,
                Empid = o.empid,
                Shippeddate = o.shippeddate
            }).Where(x => x.Shippeddate == null );
            foreach (var p in orders)
                number = p.Orderid;
        }

        //SELECT orderid, orderdate, custid, empid
        //FROM Sales.Orders
        //WHERE orderdate >= '20080211' AND orderdate < '20080213';
        private void GetOrdersInfo4()
        {
            int number = 0;
            var orders = db.Orders.Select(o => new
            {
                Orderid = o.orderid,
                Orderdate = o.orderdate,
                Custid = o.custid,
                Empid = o.empid
            }).Where(x => x.Orderdate >= new DateTime(2008, 2, 11) && x.Orderdate < new DateTime(2008, 2, 13));
            foreach (var p in orders)
                number = p.Orderid;
        }

        //SELECT TOP(9) orderid, orderdate, custid, empid
        //FROM Sales.Orders
        //ORDER BY orderdate DESC;
        private void GetOrdersInfo5()
        {
            int number = 0;
            var orders = from o in db.Orders
                         orderby o.orderdate
                         select new
                         {
                             Orderid = o.orderid,
                             Orderdate = o.orderdate,
                             Custid = o.custid,
                             Empid = o.empid
                         };

            var topOrders = orders.OrderByDescending(o => o.Orderdate).Take(9);
            foreach (var p in topOrders)
                number = p.Orderid;
        }

        //SELECT orderid, orderdate, custid, empid
        //FROM Sales.Orders
        //ORDER BY orderdate DESC, orderid DESC
        //OFFSET 50 ROWS FETCH NEXT 25 ROWS ONLY;
        private void GetOrdersInfo6()
        {
            int number = 0;
            var orders = db.Orders.OrderByDescending(o => o.orderdate).ThenByDescending(o => o.orderid).Skip(50).Take(25).ToList();

            foreach (var p in orders)
                number = p.orderid;
        }

        //SELECT C.custid, C.companyname, O.orderid, O.orderdate
        //FROM Sales.Customers AS C
        //INNER JOIN Sales.Orders AS O 
        //ON C.custid = O.custid;
        private void GetOrdersInfo7()
        {
            int number = 0;
            var orders = db.Orders.Join(db.Customers, // второй набор
                    o => o.custid, // свойство-селектор объекта из первого набора
                    c => c.custid, // свойство-селектор объекта из второго набора
                    (o, c) => new // результат
                    {
                        Custid = c.custid,
                        Companyname = c.companyname,
                        Orderid = o.orderid,
                        Orderdate = o.orderdate
                    });

            foreach (var o in orders)
                number = o.Orderid;
        }

        // GET: Orders/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            ViewBag.empid = new SelectList(db.Employees, "empid", "lastname");
            ViewBag.custid = new SelectList(db.Customers, "custid", "companyname");
            ViewBag.shipperid = new SelectList(db.Shippers, "shipperid", "companyname");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "orderid,custid,empid,orderdate,requireddate,shippeddate,shipperid,freight,shipname,shipaddress,shipcity,shipregion,shippostalcode,shipcountry")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.empid = new SelectList(db.Employees, "empid", "lastname", order.empid);
            ViewBag.custid = new SelectList(db.Customers, "custid", "companyname", order.custid);
            ViewBag.shipperid = new SelectList(db.Shippers, "shipperid", "companyname", order.shipperid);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.empid = new SelectList(db.Employees, "empid", "lastname", order.empid);
            ViewBag.custid = new SelectList(db.Customers, "custid", "companyname", order.custid);
            ViewBag.shipperid = new SelectList(db.Shippers, "shipperid", "companyname", order.shipperid);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "orderid,custid,empid,orderdate,requireddate,shippeddate,shipperid,freight,shipname,shipaddress,shipcity,shipregion,shippostalcode,shipcountry")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.empid = new SelectList(db.Employees, "empid", "lastname", order.empid);
            ViewBag.custid = new SelectList(db.Customers, "custid", "companyname", order.custid);
            ViewBag.shipperid = new SelectList(db.Shippers, "shipperid", "companyname", order.shipperid);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Order order = await db.Orders.FindAsync(id);
            db.Orders.Remove(order);
            await db.SaveChangesAsync();
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
