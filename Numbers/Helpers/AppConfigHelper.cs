using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Helpers
{
    public class AppConfigHelper
    {
        private HttpContext HttpContext { get; }
        private readonly NumbersDbContext _dbContext;
        public AppConfigHelper(NumbersDbContext dbContext)
        {
            _dbContext = new NumbersDbContext();
        }
        public AppConfigHelper(NumbersDbContext dbcontext, HttpContext httpContext)
        {
            _dbContext = dbcontext;
            HttpContext = httpContext;
        }
        //Approach this code in view
        public string GetReportPath
        {
            get
            {
                string configValues = _dbContext.AppCompanyConfigs
                               .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                               .Select(c => c.ConfigValue)
                               .FirstOrDefault();
                //var returnVal = "http://localhost:53525/ReportViewer/Viewer?Report=Voucher&id={0}";
                //http://localhost:53525/ReportViewer/Viewer?Report=Voucher&id?Id=c8d844b3-dbf8-413b-a555-e552c6808c35
                return string.Concat(configValues, "/Viewer?Report=Voucher&id={0}");
            }
        }
        //This code can be accessed anywhere in the application
        public string GetReportUrl()
        {
            string configValues = _dbContext.AppCompanyConfigs
                            .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                            .Select(c => c.ConfigValue)
                            .FirstOrDefault();
            return configValues;

        }
        public SelectList GetCommissionType()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var configValues = new SelectList(_dbContext.AppCompanyConfigs
                .Where(w => w.Module == "Booking" && w.ConfigName == "Commission Type" && w.CompanyId == companyId)
                .ToList(), "Id", "ConfigValue");
            return configValues;
        }
        public SelectList GetWareHouses()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var configValues = new SelectList((from b in _dbContext.AppCompanyConfigBases
                                               join c in _dbContext.AppCompanyConfigs on b.Id equals c.BaseId
                                               where c.CompanyId == companyId && c.IsActive && c.IsDeleted == false && b.Name == "Ware House" && b.Module == "Inventory"
                                               select c
                                  ).ToList(), "Id", "ConfigValue");
            return configValues;

        }

    }
}
