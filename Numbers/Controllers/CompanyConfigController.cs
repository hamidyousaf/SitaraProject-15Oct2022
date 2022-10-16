using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
//using Numbers.Models;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Repository.Helpers;

namespace Numbers.Controllers
{
    public class CompanyConfigController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public CompanyConfigController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            var companyConfig = _dbContext.AppCompanyConfigBases.Where(c => c.IsDeleted == false && c.IsActive);
            if (companyConfig != null)
            {
                return View(companyConfig);
            }
            return RedirectToAction(nameof(Create));
        }
        [HttpGet]
        public IActionResult Create(int id)
        {
            ViewBag.listOfCompanies = new SelectList(_dbContext.AppCompanies.ToList(),"Id","Name");
            if (id != 0)
            {
                ViewBag.ConfigGrid = JsonConvert.SerializeObject(_dbContext.AppCompanyConfigs.Where(c => c.BaseId == id));
                AppCompanyConfigBase companyConfig = _dbContext.AppCompanyConfigBases.Find(id);
                return View(companyConfig);
            }
            ViewBag.ConfigGrid = "[]";
            return View(new AppCompanyConfigBase());
        }

        [HttpPost]
        public async Task<IActionResult> Create(AppCompanyConfigBase appCompanyConfig,IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");

            if (ModelState.IsValid)
            {
                if (appCompanyConfig.Id == 0)
                {
                    appCompanyConfig.CompanyId = companyId;
                    appCompanyConfig.CreatedBy = userId;
                    appCompanyConfig.CreatedDate = DateTime.Now;
                    TempData["error"] = "false";
                    TempData["message"] = "Company Config has been added successfully.";
                    _dbContext.AppCompanyConfigBases.Add(appCompanyConfig);
                }
                else
                {
                    var companyConfig = _dbContext.AppCompanyConfigBases.Find(appCompanyConfig.Id);
                    companyConfig.UpdatedBy = userId;
                    companyConfig.UpdatedDate = DateTime.Now;
                    companyConfig.Module = appCompanyConfig.Module;
                    companyConfig.CompanyId = appCompanyConfig.CompanyId;
                    companyConfig.Name = appCompanyConfig.Name;
                    //companyConfig.ConfigValue = appCompanyConfig.ConfigValue;
                    companyConfig.Description = appCompanyConfig.Description;
                    //companyConfig.UserValue1 = appCompanyConfig.UserValue1;
                    //companyConfig.UserValue2 = appCompanyConfig.UserValue2;
                    //companyConfig.UserValue3 = appCompanyConfig.UserValue3;
                    //companyConfig.UserValue4 = appCompanyConfig.UserValue4;
                    companyConfig.IsActive = appCompanyConfig.IsActive;
                    var entry = _dbContext.AppCompanyConfigBases.Update(companyConfig);
                    entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                    TempData["error"] = "false";
                    TempData["message"] = "Company Config has been updated successfully.";

                }
                await _dbContext.SaveChangesAsync();
                AppCompanyConfig[] configurations = JsonConvert.DeserializeObject<AppCompanyConfig[]>(collection["ConfigGrid"]);
                foreach (var configuration in configurations)
                {
                    AppCompanyConfig config;
                    if (configuration.Id == 0)
                    {
                        config = new AppCompanyConfig();
                        config = configuration;
                        config.BaseId = appCompanyConfig.Id;
                        config.CreatedBy = userId;
                        config.CreatedDate = DateTime.Now;
                        config.CompanyId = companyId;
                        _dbContext.AppCompanyConfigs.Add(config);
                    }
                    else
                    {
                        config = _dbContext.AppCompanyConfigs.Find(configuration.Id);
                        config = configuration;
                        config.UpdatedBy = userId;
                        config.UpdatedDate = DateTime.Now;
                        var dbEntry = _dbContext.AppCompanyConfigs.Update(config);
                        dbEntry.OriginalValues.SetValues(await dbEntry.GetDatabaseValuesAsync());
                    }
                }
                await _dbContext.SaveChangesAsync();
                return Redirect("/CompanyConfig/Index");
            }
            return View(appCompanyConfig);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var config = _dbContext.AppCompanyConfigBases.Find(id);
            config.UpdatedBy = HttpContext.Session.GetString("UserId");
            config.UpdatedDate = DateTime.Now;
            config.IsDeleted = true;
            config.IsActive = false;
            var entry = _dbContext.Update(config);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            TempData["error"] = "false";
            TempData["message"] = "Company Config has been deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<AppCompanyConfig>>> Get(string module, string name)
        {
            return await _dbContext.AppCompanyConfigs.Where(c => c.Module == module && c.ConfigName == name).ToListAsync();

        }
        public async Task<string> GetConfigByName(string module, string name)
        {
            AppCompanyConfig config = await _dbContext.AppCompanyConfigs.Where(c => c.Module == module && c.ConfigName == name).FirstOrDefaultAsync();
            return config.ConfigValue;
        }
        public IActionResult GetConfigurations(string q = "")
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;

            var configs = _dbContext.AppCompanyConfigBases
                                    .Where(a => a.Module.Contains(q) || a.Name.Contains(q) || a.Description.Contains(q))
                                    .Where(c => c.CompanyId==companyId)
                                    .Select(a => new
                                               {
                                                   id = a.Id,
                                                   module = a.Module,
                                                   name = a.Name,
                                                   description = a.Description,
                                                   text = string.Concat(a.Module, " - ", a.Name),
                                    })
                                               .OrderBy(a => a.module)
                                               .Take(25)
                                               .ToList();
            return Ok(configs);
        }

    }
}