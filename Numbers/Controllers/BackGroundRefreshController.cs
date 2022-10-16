using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Net.Mail;
using Numbers.Entity.Models;
using Microsoft.EntityFrameworkCore;
using Numbers.Controllers;

namespace Numbers.Controllers
{
    public class BackGroundRefreshController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        private readonly IBackGroundRefresh emailService;
        public BackGroundRefreshController(NumbersDbContext dbContext, IBackGroundRefresh emailService)
        {
            _dbContext = dbContext;
            this.emailService = emailService;
            
        }

        

       
       
    }
}
