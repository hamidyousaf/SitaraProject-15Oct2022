using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Helpers;
//using Numbers.Models;
//using Numbers.ViewModels;

namespace Numbers.Areas.Booking.Controllers
{
    [Authorize]
    [Area("Booking")]
    public class CustomerController : Controller
    {
        private readonly NumbersDbContext _dbContext;

        public CustomerController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;

        }
        //[Route("Index")]
        public IActionResult Index()
        {
            IList<BkgCustomer> customers = _dbContext.BkgCustomers
                .Include(b => b.Account)
                .Where(x => x.CompanyId == HttpContext.Session.GetInt32("CompanyId")).OrderByDescending(o => o.Id).ToList();
            return View(customers);
        }

        //[Authorize ("Admin")]
        public IActionResult Create(int? id)
        {
            BkgCustomer customer = new BkgCustomer();
            if (id != null)
            {
                ViewBag.EntityState = "Update";
                customer = _dbContext.BkgCustomers.Find(id);
                return View("Create", customer);
            }
            else
            {
                ViewBag.EntityState = "Create";
                customer.IsActive = true;

                return View("Create", customer);
            }
        }

        [HttpPost]
        public async Task <IActionResult> Create(int id, BkgCustomer customer, IFormFile img)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            try
            {

                if (ModelState.IsValid)
                {

                    if (customer.Id == 0)
                    {
                        customer.CompanyId = companyId;
                        customer.CreatedBy = userId;
                        customer.CreatedDate = DateTime.Now;
                        if (img != null && img.Length > 0)
                        {
                            customer.Photo = UploadImage(img);
                        }
                        _dbContext.BkgCustomers.Add(customer);
                    }
                   
                    else
                    {  
                        //avoiding null insertion in model attributes
                        var data = _dbContext.BkgCustomers.Find(customer.Id);
                        data.AccountId = customer.AccountId;
                        data.Address = customer.Address;
                        data.CNIC = customer.CNIC;
                        data.Name = customer.Name;
                        data.FatherName = customer.FatherName;
                        data.IsActive = customer.IsActive;
                        data.IsBooking = customer.IsBooking;
                        data.IsCustomer = customer.IsCustomer;
                        data.IsSupplier = customer.IsSupplier;
                        data.Mobile = customer.Mobile;
                        data.Name = customer.Name;
                        data.NTNNo = customer.NTNNo;
                        data.Remarks = customer.Remarks;
                        data.Phone = customer.Phone;
                        data.UpdatedBy = userId;
                        data.CompanyId = companyId;
                        data.UpdatedDate = DateTime.Now;
                        if (img != null && img.Length > 0)
                        {
                            data.Photo = UploadImage(img);
                        }
                        else
                        {
                            _dbContext.Entry(data).Property(x => x.Photo).IsModified = false;
                        }
                        var entry = _dbContext.BkgCustomers.Update(data);
                        entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                    }
                    await _dbContext.SaveChangesAsync();

                    TempData["error"] = "false";
                    TempData["message"] = "Customer has been saved successfully.";
                    return RedirectToAction("Index");
                }
                else
                {
                    if (customer.Id != 0)
                    {
                        ViewBag.EntityState = "Update";
                        customer = _dbContext.BkgCustomers.Find(customer.Id);
                        return View("Create", customer);
                    }
                    else
                    {
                        ViewBag.EntityState = "Create";
                        customer.IsActive = true;
                        return View("Create", customer);
                    }
                }
            }
            catch (Exception Exp)
            {
                TempData["error"] = "true";
                TempData["message"] = (Exp.InnerException == null ? Exp.Message : Exp.InnerException.Message);

                return  View("Create", customer);
            }
        }
        public string UploadImage(IFormFile img)
        {
                //if (img.Length > 0)
                //{
                    var fileName = Path.GetFileName(img.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads\\customer-images", fileName);
            using (var Fstream = new FileStream(filePath, FileMode.Create))
            {
                img.CopyToAsync(Fstream);
                var fullPath = "/uploads/customer-images/" + fileName;
                return fullPath;
            }
        }

        [Route("Detail")]
        public IActionResult Detail(int id)
        {
            BkgCustomer customer = _dbContext.BkgCustomers
                .Include(b => b.Account).Where(x => x.Id == id).FirstOrDefault();
            return View(customer);
        }
    }
}