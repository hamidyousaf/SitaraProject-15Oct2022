using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Numbers.Entity.Models;
using Numbers.Helpers;

namespace Numbers.Areas.GL.Controllers
{
    [Authorize]
    [Area("GL")]
    public class OnScreenTrialBalanceController : Controller
    {
        private readonly NumbersDbContext _dbContext;

        public IActionResult Index()
        {
            return View();
        }
      
    }
}