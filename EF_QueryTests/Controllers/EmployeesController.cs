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
    public class EmployeesController : Controller
    {
        private TSQL2012Entities db = new TSQL2012Entities();

        // GET: Employees
        public async Task<ActionResult> Index()
        {
            GetDistinctCoutry();
            GetEmployeesInfo1();
            var employees = db.Employees.Include(e => e.Employee1);
            return View(await employees.ToListAsync());
        }

        //SELECT DISTINCT country
        //FROM HR.Employees;
        private void GetDistinctCoutry()
        {
            var currentCountry = "";
            var coutries = db.Employees.Select(p => new
            {
                Country = p.country
            }).Distinct().ToList();
            foreach (var p in coutries)
                currentCountry = p.Country;
        }

        //SELECT empid, lastname
        //FROM HR.Employees
        //ORDER BY empid;
        private void GetEmployeesInfo1()
        {
            var name = "";
            var employees = db.Employees.Select(p => new
            {
                Empid = p.empid,
                Name = p.firstname + " " + p.lastname
            }).OrderBy(p => p.Empid);
            foreach (var p in employees)
                name = p.Name;
        }

        // GET: Employees/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = await db.Employees.FindAsync(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            ViewBag.mgrid = new SelectList(db.Employees, "empid", "lastname");
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "empid,lastname,firstname,title,titleofcourtesy,birthdate,hiredate,address,city,region,postalcode,country,phone,mgrid")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Employees.Add(employee);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.mgrid = new SelectList(db.Employees, "empid", "lastname", employee.mgrid);
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = await db.Employees.FindAsync(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            ViewBag.mgrid = new SelectList(db.Employees, "empid", "lastname", employee.mgrid);
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "empid,lastname,firstname,title,titleofcourtesy,birthdate,hiredate,address,city,region,postalcode,country,phone,mgrid")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employee).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.mgrid = new SelectList(db.Employees, "empid", "lastname", employee.mgrid);
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = await db.Employees.FindAsync(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Employee employee = await db.Employees.FindAsync(id);
            db.Employees.Remove(employee);
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
