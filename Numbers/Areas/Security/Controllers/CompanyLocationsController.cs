using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Numbers.Entity.Models;

namespace Numbers.Areas.Security.Controllers
{
    [Area("Security")]
    [Authorize]
    public class CompanyLocationsController : Controller
    {
        private readonly NumbersDbContext _dbContext;

        public CompanyLocationsController(NumbersDbContext db)
        {
            _dbContext = db;

        }
        public IActionResult Index(int? id)
        {
            if (id == null)
            {
                ViewBag.EntityState = "Create";
                ViewBag.NavbarHeading = "Create Company";
                ViewBag.Id = null;

                AppCompany appCompany = new AppCompany();
                return View(appCompany);
            }
            else
            {
                ViewBag.EntityState = "Update";
                ViewBag.NavbarHeading = "Update Company";

                AppCompany appCompany = new AppCompany();
                AppCompany Res = _dbContext.AppCompanies.Find(id);

                return View(Res);
            }
        }
        public async Task<IActionResult> List()
        {
            IEnumerable<AppCompany> app = await _dbContext.AppCompanies.ToListAsync();
            return View(app);
        }
        [HttpPost]
        public async Task<IActionResult> Create(AppCompany appCompany, IFormFile img, IFormCollection collection)
        {
            AppCompany app = new AppCompany();

            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            if (appCompany.Id == 0)
            {
                app.Name = appCompany.Name;
                app.AddressLine1 = appCompany.AddressLine1;
                app.AddressLine2 = appCompany.AddressLine2;
                app.Country = appCompany.Country;
                app.City = appCompany.City;
                app.Email = appCompany.Email;
                app.Phone1 = appCompany.Phone1;
                app.Phone2 = appCompany.Phone2;
                app.Fax = appCompany.Fax;
                app.Country = appCompany.Country;
                app.CreatedBy = userId;
                app.CreatedDate = DateTime.Now;
                app.IsActive = appCompany.IsActive;
                if (img != null)
                {
                    app.Logo = await UploadFile(img);
                }
               
                _dbContext.AppCompanies.Add(app);
                TempData["error"] = "false";
                TempData["message"] = "Company has been added successfully.";
            }
            else
            {
                var Comp = _dbContext.AppCompanies.Find(appCompany.Id);
                Comp.Name = appCompany.Name;
                Comp.AddressLine1 = appCompany.AddressLine1;
                Comp.AddressLine2 = appCompany.AddressLine2;
                Comp.Country = appCompany.Country;
                Comp.City = appCompany.City;
                Comp.Email = appCompany.Email;
                Comp.Phone1 = appCompany.Phone1;
                Comp.Phone2 = appCompany.Phone2;
                Comp.Fax = appCompany.Fax;
                Comp.Country = appCompany.Country;
                Comp.UpdatedBy = userId;
                Comp.UpdatedDate = DateTime.Now;
                Comp.IsActive = appCompany.IsActive;
                //  var entry = _dbContext.Sys_Responsibilities.Update(config);
                // entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                _dbContext.AppCompanies.Update(Comp);

                //  await _dbContext.SaveChangesAsync();
                TempData["error"] = "false";
                TempData["message"] = "Company has been Updated successfully.";


            }
            await _dbContext.SaveChangesAsync();
            return Redirect("/Security/CompanyLocations/Index");
        }
        public async Task<string> UploadFile(IFormFile img)
        {
            string filesList = "";
            if (img != null)
            {
                if (img.Length > 0)
                {
                    var fileName = Path.GetFileName(img.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads\\item-images", fileName);
                    using (var Fstream = new FileStream(filePath, FileMode.Create))
                    {
                        await img.CopyToAsync(Fstream);
                        var fullPath = "/uploads/item-images/" + fileName;
                        filesList += fullPath;
                    }
                }
            }
            return filesList;
        }
    }
}
