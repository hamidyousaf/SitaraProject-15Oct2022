using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using Numbers.Entity.Models;
using Numbers.Repository.Helpers;
using Numbers.Repository.HR;

namespace Numbers.Areas.HR.Controllers
{
    [Area("HR")]
    [Authorize]
    public class LoanController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public LoanController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            IEnumerable<HRLoan> list = LoanRepo.GetAll();
            return View(list);       
        }

        [HttpGet]
        public IActionResult Create(int id)
        {

            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var configValues = new ConfigValues(_dbContext);
            ViewBag.DeductionTypes = configValues.GetConfigValues("HR", "Deduction Type", companyId);
            ViewBag.Accounts = LoanRepo.GetAccounts();
            if (id == 0)
            {
                ViewBag.EntityState = "Create";
                var model = new HRLoan(); 
                TempData["LoanNo"] = LoanRepo.GetLoanNo(id);
                return View(model);
            }
            else
            {
                ViewBag.EntityState = "Update";
                HRLoan loan = LoanRepo.GetById(id);
                TempData["LoanNo"] = loan.LoanNo;
                return View(loan);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(HRLoan model, IFormCollection collection)
        {
            if (model.Id == 0)
            {
                var success = await LoanRepo.Create(model, collection);
                if (success)
                {
                    TempData["error"] = "false";
                    TempData["message"] = "Loan has been Added Successfully";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Something went wrong while adding loan";
                    return View();
                } 
            }
            else
            {
                var success =await LoanRepo.Update(model, collection);
                if (success)
                {
                    TempData["error"] = "false";
                    TempData["message"] = "Loan has been Updated Successfully";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Something went wrong while updating loan";
                    return View();
                }
            }
        }

        public IActionResult Delete(int id)
        {
            dynamic isSuccess = LoanRepo.Delete(id);
            if (isSuccess = true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Loan has been deleted successfully.";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong.";
            }
            return RedirectToAction(nameof(Index));
        }

    }
}