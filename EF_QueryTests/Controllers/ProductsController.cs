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
    public class ProductsController : Controller
    {
        private TSQL2012Entities db = new TSQL2012Entities();

        // GET: Products
        public async Task<ActionResult> Index()
        {
            GetProductsInfo1();
            var products = db.Products.Include(p => p.Category).Include(p => p.Supplier);
            return View(await products.ToListAsync());
        }

        // SELECT productid,
        // RIGHT(REPLICATE('0', 10) + CAST(productid AS VARCHAR(10)), 10)
        // AS str_productid
        //FROM Production.Products;

        private void GetProductsInfo()
        {
            string str_productid = "";
            var products = db.Products.Select(p => new
                {
                    Productid = p.productid
                });
            foreach (var p in products)
                str_productid = String.Format("{0:D10}", p.Productid);
        }

        //SELECT productid, productname, unitprice
        //FROM Production.Products
        //WHERE unitprice = (SELECT MIN(unitprice)
        //                   FROM Production.Products)

        private void GetProductsInfo1()
        {
            string str_productid = "";
            var products = db.Products.Select(p => new
            {
                Productid = p.productid,
                Productname = p.productname,
                Unitprice = p.unitprice
            }).Where(p=>p.Unitprice == (db.Products.Min(x => x.unitprice)));
             foreach (var p in products)
                str_productid = String.Format("{0:D10}", p.Productid);
        }

        // GET: Products/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.categoryid = new SelectList(db.Categories, "categoryid", "categoryname");
            ViewBag.supplierid = new SelectList(db.Suppliers, "supplierid", "companyname");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "productid,productname,supplierid,categoryid,unitprice,discontinued")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.categoryid = new SelectList(db.Categories, "categoryid", "categoryname", product.categoryid);
            ViewBag.supplierid = new SelectList(db.Suppliers, "supplierid", "companyname", product.supplierid);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.categoryid = new SelectList(db.Categories, "categoryid", "categoryname", product.categoryid);
            ViewBag.supplierid = new SelectList(db.Suppliers, "supplierid", "companyname", product.supplierid);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "productid,productname,supplierid,categoryid,unitprice,discontinued")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.categoryid = new SelectList(db.Categories, "categoryid", "categoryname", product.categoryid);
            ViewBag.supplierid = new SelectList(db.Suppliers, "supplierid", "companyname", product.supplierid);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Product product = await db.Products.FindAsync(id);
            db.Products.Remove(product);
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
