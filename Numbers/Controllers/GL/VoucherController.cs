using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
//using Numbers.Models;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;

namespace Numbers.Controllers
{
    [Authorize]
    public class VoucherController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public VoucherController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: GLVoucher
        public ActionResult Index()
        {
            return View();
        }

        // GET: GLVoucher/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: GLVoucher/Create
        public ActionResult Create()
        {

            var list = new[]
            {
                new
                {
                    id="5296",
                    name="33011445 - MUHAMMAD ZAFAR PASCO 2010"
                },
                new
                {
                    id="5297",
                    name="33011456 - JILAL & SONS"
                }
            };
            var accounts = _dbContext.GLAccounts.Where(
                                                a => a.IsDeleted == false && a.Company.Id == 1 && a.AccountLevel == 4
                                               )
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   name = string.Concat(a.Code, " - ", a.Name),
                                                   text = string.Concat(a.Code, " - ", a.Name)
                                               })
                                               .OrderBy(a => a.name)
                                               .ToList();
            ViewBag.Accounts = accounts;
            return View();
        }

        // POST: GLVoucher/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: GLVoucher/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: GLVoucher/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: GLVoucher/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: GLVoucher/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}