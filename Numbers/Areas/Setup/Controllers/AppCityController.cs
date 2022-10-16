using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Numbers.Entity.Models;

namespace Numbers.Areas.Setup.Controllers
{
    [Area("Setup")]
    public class AppCityController : Controller
    {
        private readonly NumbersDbContext _dbContext;

        public AppCityController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            var selectList = GetCountries();
            ViewBag.CountryList = selectList;
            ViewBag.NavbarHeading = "Add City";
            return View();
        }
        private List<AppCountry> GetCountries()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            List<AppCountry> countries = (from b in _dbContext.AppCountries orderby b.Name select b).ToList();
            return countries;
        }

        public async Task<int> Submit(IFormCollection collection)
        {
            int id = Convert.ToInt32(collection["countryId"].ToString());
            string deletedIds = collection["deletedIds"].ToString();

            AppCitiy[] data = JsonConvert.DeserializeObject<AppCitiy[]>(collection["data"]);
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            foreach (AppCitiy item in data)
            {
                if (item.Id == 0)
                {
                    item.CountryId = id;
                    await _dbContext.AppCities.AddAsync(item);
                }
                else
                {
                    await _dbContext.AppCities.AddAsync(item);
                    _dbContext.AppCities.Update(item);
                }
            }
            foreach (var item in deletedIds.Split(","))
            {
                if (item != "")
                {
                    AppCitiy config = _dbContext.AppCities.Find(Convert.ToInt32(item));
                    _dbContext.AppCities.Update(config);
                }
            }

            await _dbContext.SaveChangesAsync();
            TempData["error"] = false;

            return 1;

        }
    }
}