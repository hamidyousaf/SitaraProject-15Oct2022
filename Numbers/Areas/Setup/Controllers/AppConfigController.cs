using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Numbers.Entity.Models;

namespace Numbers.Areas.Setup.Controllers
{
    [Area("Setup")]
    public class AppConfigController : Controller
    {
        private readonly NumbersDbContext _dbContext;

        public AppConfigController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;

        }
        public IActionResult Index()
        {   
            var list = GetConfigBase();
            ViewBag.CategoriesList = list;
            ViewBag.InvOrganization = new SelectList(_dbContext.AppCompanyConfigs.Where(x=>x.BaseId== 1066), "Id", "ConfigValue");
            ViewBag.NavbarHeading = "Application Configuration";
            return View();
        }
        private List<AppCompanyConfigBase> GetConfigBase()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            List<AppCompanyConfigBase> configs = _dbContext.AppCompanyConfigBases.
                                                  Where(b => b.IsActive && !b.IsDeleted && b.CompanyId==companyId).ToList();

            return configs;
        }
        public async Task<int> Submit(IFormCollection collection)
        {
            int id = Convert.ToInt32(collection["baseId"].ToString());
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            string deletedIds = collection["deletedIds"].ToString();
            foreach (var item in deletedIds.Split(","))
            {
                if (item != "")
                {
                    AppCompanyConfig config = _dbContext.AppCompanyConfigs.Find(Convert.ToInt32(item));
                    config.IsDeleted = true;
                    config.IsActive = false;
                    config.UpdatedBy = userId;
                    config.UpdatedDate = DateTime.Now;
                    _dbContext.AppCompanyConfigs.Update(config);
                    await _dbContext.SaveChangesAsync();
                }
            }
            AppCompanyConfig[] data = JsonConvert.DeserializeObject<AppCompanyConfig[]>(collection["data"]);
            
             
            foreach (AppCompanyConfig item in data)
            {
               
                if (item.Id == 0)
                {
                    var appCompanyConfigs = _dbContext.AppCompanyConfigs.Where(x =>x.ConfigValue.Trim()==item.ConfigValue.Trim() && x.BaseId==id && x.IsDeleted == false).FirstOrDefault();
                    if (appCompanyConfigs==null)
                    {
                        item.BaseId = id;
                        item.CompanyId = companyId;
                        item.CreatedBy = userId;
                        item.CreatedDate = DateTime.Now;
                        await _dbContext.AppCompanyConfigs.AddAsync(item);
                    }
                }    
                else
                {
                    item.UpdatedBy = userId;
                    item.UpdatedDate = DateTime.Now;
                    await _dbContext.AppCompanyConfigs.AddAsync(item);
                    _dbContext.AppCompanyConfigs.Update(item);
                }
            }
            

            await _dbContext.SaveChangesAsync();
            TempData["error"] = false;
            
            return 1;
        }

    }
}