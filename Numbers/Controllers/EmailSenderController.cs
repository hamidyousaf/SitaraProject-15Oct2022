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
using System.Globalization;

namespace Numbers.Controllers
{
    public class EmailSenderController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        private readonly IEmailService emailService;
        public EmailSenderController(NumbersDbContext dbContext, IEmailService emailService)
        {
            _dbContext = dbContext;
            this.emailService = emailService;
            
        }

        [HttpPost]
        public IActionResult SendEmaiL()
        {
            //var so = _dbContext.ARSaleOrderItems.Include(x => x.SaleOrder).Where(x => x.IsDeleted == false && x.SaleOrder.SaleOrderDate>= DateTime.Now && x.SaleOrder.SaleOrderDate <= DateTime.Now).ToList();
            var so = _dbContext.ARSaleOrderItems.Include(x => x.SaleOrder).Where(x => x.IsDeleted == false && x.SaleOrder.SaleOrderDate==DateTime.Today ).ToList();
            var soTotal = so.Sum(x => x.TotalMeterAmount);
            var totalSo = so.GroupBy(x=>x.SaleOrderId).Distinct().Count();
            var sototalQty = so.Sum(x => x.Qty);
            var message = string.Concat("[0] ",DateTime.Now) + " Dear Sir" + Environment.NewLine + Environment.NewLine;
            message += "Today's Sale Order "+ Environment.NewLine + Environment.NewLine;
            message += "Total Transactions = " + totalSo + Environment.NewLine;
            message += "Total Qty = " + sototalQty.ToString("N", new CultureInfo("en-US")) + Environment.NewLine;
            message += "Total Amount = " + Convert.ToDecimal(soTotal).ToString("N", new CultureInfo("en-US")) + Environment.NewLine + Environment.NewLine;
            message += "Regards:" + Environment.NewLine + "Sajid Maher";
           
            //emailService.Send("Sajidtalib74@gmail.com", "Director@sitaratextile.com", "Sale Order Info", message);
            //emailService.Send("Sajidtalib74@gmail.com", "amin.kashif@gmail.com", "Sale Order Info", message);
            emailService.Send("Sajidtalib74@gmail.com", "Abdul_islam@sitaratextile.com", "Sale Order Info", message);
            emailService.Send("Sajidtalib74@gmail.com", "sarfrazsaleemi@gmail.com", "Sale Order Info", message);
            emailService.Send("Sajidtalib74@gmail.com", "Adnan.shahid@sitaratextile.com", "Sale Order Info", message);
            emailService.Send("Sajidtalib74@gmail.com", "umair.shafiq@sitaratextile.com", "Sale Order Info", message);
            return View();
        } 
        public IActionResult Index()
        {
            return View();
        }
    }
}
