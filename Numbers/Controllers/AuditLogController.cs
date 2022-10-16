using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Numbers.Entity.Models;

namespace Numbers.Controllers
{
    public class AuditLogController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public AuditLogController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task <IActionResult> Index()
        {

           
           return View(await _dbContext.AppAuditTrials.ToListAsync());
     
        }
    }
}