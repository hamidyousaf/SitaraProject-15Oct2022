using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Numbers.Helpers;
namespace Numbers.Controllers
{
    public class SessionController : Controller
    {



        [HttpGet]
        public SessionResult Index()
        {
            SessionResult result = new SessionResult();
            result.CompanyID = HttpContext.Session.GetInt32("CompanyId").Value;
            result.UserID =Convert.ToInt32( HttpContext.Session.GetString("UserId"));
            result.DateFormat = HttpContext.Session.GetString("DateFormat");

            return result;
        }
        public class SessionResult
        {
            public int CompanyID { get; set; }
            public int UserID { get; set; }
            public string CompanyName { get; set; }
            public string DateFormat { get; set; }
        }

    }
}