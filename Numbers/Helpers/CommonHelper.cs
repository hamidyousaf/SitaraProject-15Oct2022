using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
//using Numbers.Models;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Helpers
{
    public class CommonHelper
    {
        private HttpContext HttpContext { get; }
        private readonly NumbersDbContext _dbContext;
        private readonly int _companyId;
        private readonly string _userId;
        public CommonHelper(NumbersDbContext dbContext, HttpContext httpContext)
        {
            _dbContext = dbContext;
            HttpContext = httpContext;
            _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            _userId = httpContext.Session.GetString("UserId");

        }
        //public static int ActiveCompanyId;
        //public static int ActiveUserId;// = Convert.ToInt32(_session.GetString("userId"));


        //public static int ActiveUserId
        //{
        //    get
        //    {
        //        return Convert.ToInt32(_session.GetString("ActiveUserId"));
        //    }
        //}
        //public static int ActiveCompanyId
        //{
        //    get
        //    {
        //        return Convert.ToInt32(_session.GetString("ActiveCompanyId"));
        //    }
        //}

        public static SelectList getAppCompanyConfigLists(NumbersDbContext _dbContext, string module, string configName, int activeCompanyID)
        {
            var configValues = new SelectList(_dbContext.AppCompanyConfigs
                .Where(w => w.Module == module && w.ConfigName == configName && w.CompanyId == activeCompanyID)
                .ToList(),"ConfigValue", "ConfigValue");
            return configValues;
        }
        public string GetConfigValue(string module, string configName)
        {
            var configValue = _dbContext.AppCompanyConfigs
                              .Where(x => x.Module == module && x.ConfigName == configName && x.CompanyId == _companyId)
                              .FirstOrDefault();
            return configValue.ConfigValue;
        }
        public static string DateFormat {
            get
            {
                string _dateFormat = "dd-MMM-yyyy";
                return _dateFormat;
            }

        }
        public static string CurrentDate
        {
            get
            {
                
                string _currentDate = DateTime.Now.Date.ToString("dd-MMM-yyyy");
                return _currentDate;
            }
        }
        public static string MonthYear
        {
            get
            {

                string _currentDate = DateTime.Now.Date.ToString("MMM-yyyy");
                return _currentDate;
            }
        }
        public static string GetReportPath(NumbersDbContext dbContext)
        {
                string configValues = dbContext.AppCompanyConfigs
                    .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                    .Select(c => c.ConfigValue)
                    .FirstOrDefault();
                return configValues;

        }

        public static string Core { get; private set; }

        public static string FormatDate(DateTime date)
        {
            return date.ToString(DateFormat);

        }

        public static string getAuthorizeRoles(string controller, string action,string activeUserID, int activeCompanyID)
        {
            var result = "Member,Test,Administrator,Admin,User,Manager";
            string[] roles = result.Split(',');
            return result;
        }
    }
}
