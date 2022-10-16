using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//using Numbers.Models;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;

namespace Numbers.Controllers
{
    public class SubAccountController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public SubAccountController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<GLSubAccount>>> Get()
        {
            return await _dbContext.GLSubAccounts.Where(a => a.IsActive == true && a.IsDeleted == false).ToListAsync();
        }

    }
}