using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Numbers.Entity.Models;

namespace Numbers.Areas.Booking.Controllers
{
    public class ReportController : Controller
    { private readonly NumbersDbContext _dbContext;
        public ReportController(NumbersDbContext context)
        {
            _dbContext = context;
        }
        public IActionResult BookingLedger()
        {
                return View ();
        }
        public IActionResult BookingRegister()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            //ok
            ViewBag.Items = new MultiSelectList(_dbContext.InvItems.Where(a=>a.CompanyId==companyId)
                                                    .ToList(), "Id", "Name");

            //var items =new SelectList( _dbContext.InvItems
            //                            .Where(a => a.CompanyId == companyId)
            //                             .Select(a => new
            //                             {
            //                                 id = a.Id,
            //                                 text = string.Concat(a.Name)
            //                             })
            //                             .OrderBy(a => a.text)
            //                             .Take(25)
            //                             .ToList());
            //ViewBag.Items = items;
            //ApiController api = new ApiController(_dbContext);
            //ViewBag.Items = api.GetItems();
            return View();
        }
        public IActionResult BookingReceipt()
        {
            return View();
        }
        public IActionResult CustomerBookingLedger()
        {
            return View();
        }
        public IActionResult VehicleDeliveryRegister()
        {
            return View();
        }
        public IActionResult ReceiptAgainstBooking()
        {
            return View();
        }
        public IActionResult CashPurchaseSale()
        {
            return View();
        }
        public IActionResult Ranges(string type)
        {
            ViewBag.ReportType = type;
            return View();
        }
    }
}