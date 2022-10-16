using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Controllers
{
    public class CompanyController : Controller
    {
     
        private readonly NumbersDbContext _dbContext;
        public CompanyController(NumbersDbContext context)
        {
            _dbContext = context;
        }
        public ActionResult Index()
        {
            var company = _dbContext.AppCompanies.ToList();
            return View(company);
        }

        public ActionResult Create(int id)
        {
            AppCompany company = new AppCompany();
            if (id == 0)
            {

            }
            else
            {
                company = _dbContext.AppCompanies.Where(x => x.Id == id).FirstOrDefault();
            }

            return View(company);
        }
 
        [HttpPost]
        public async Task<IActionResult> Create(AppCompany model, IFormFile img, IFormCollection collection)
        {
            var userId = HttpContext.Session.GetString("UserId");
            try
            {
                if (img != null)
                {
                    if (img.Length > 0)
                    {
                        var fileName = Path.GetFileName(img.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\company", fileName);
                        using (var Fstream = new FileStream(filePath, FileMode.Create))
                        {
                            await img.CopyToAsync(Fstream);
                            var fullPath = "/img/company/" + fileName;
                            //registerViewModel.Photo = fullPath;
                             model.Logo = fullPath;
                        }
                    }
                }
                if (model.Id==0)
                {
                    model.CreatedBy = userId;
                    model.CreatedDate = DateTime.Now;
                    _dbContext.AppCompanies.Add(model);
                     await _dbContext.SaveChangesAsync();
                }
                else
                {
                    var obj = _dbContext.AppCompanies.Find(model.Id);
                    obj.Name = model.Name;
                    obj.AddressLine1 = model.AddressLine1;
                    obj.AddressLine2 = model.AddressLine2;
                    obj.Phone1 = model.Phone1;
                    obj.Phone2 = model.Phone2;
                    obj.Logo = model.Logo;
                    obj.NationalTaxNumber = model.NationalTaxNumber;
                    obj.SalesTaxNumber = model.SalesTaxNumber;
                    obj.UpdatedBy = userId;
                    obj.UpdatedDate = DateTime.Now;
                    _dbContext.AppCompanies.Update(obj);
                    await  _dbContext.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            catch(Exception e)
            {
                return View();
            } 
        }
 
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                var obj = _dbContext.AppCompanies.Find(id);
                obj.IsDeleted = false;
                _dbContext.AppCompanies.Update(obj);
                _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }


    }
}
