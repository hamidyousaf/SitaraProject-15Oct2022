using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Helpers;

namespace Numbers.Areas.Booking.Controllers
{
    [Authorize]
    [Area("Booking")]
    public class DetailInfoController : Controller
    {
        private readonly NumbersDbContext _dbContext;


        public DetailInfoController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;

        }

        [HttpGet]
        public IActionResult GetReceivedAmount(int id)
        {
            var receivedAmount = _dbContext.BkgVehicles.Where(a => a.Id == id)
                .Select(a => new
                {
                    id = a.Customer.Name,
                    text = a.ReceivedAmount

                })
                .FirstOrDefault();
            return Ok(receivedAmount);
        }
        [HttpGet]
        public IActionResult GetIGPDetail(int id)
        {
            var IGPDetail = _dbContext.BkgVehicleIGPs.Where(a => a.Vehicle.Id == id)
                .Select(a => new
                {
                    id = a.EngineNo,
                    text = a.ChassisNo

                })
                .FirstOrDefault();
            return Ok(IGPDetail);
        }
        public JsonResult checkBookingNoAlreadyExist(string bookingNo)
        {
            System.Threading.Thread.Sleep(200);
            if (bookingNo.Length == 0)
                return Json(0);

            var chkbookingNo = _dbContext.BkgVehicles.Where(a => a.BookingNo == bookingNo)
                                               .Select(a => a.BookingNo)
                                               .FirstOrDefault();
            return Json(chkbookingNo == null ? 0 : 1);
        }   
        public JsonResult checkCustomerCNICAlreadyExist(string cnic)
        {
            System.Threading.Thread.Sleep(200);
            if (cnic.Length == 0)
                return Json(0);

            var chkCNIC = _dbContext.BkgCustomers.Where(a => a.CNIC == cnic)
                                               .Select(a => a.CNIC)
                                               .FirstOrDefault();
            return Json(chkCNIC == null ? 0 : 1);
        }
    }
}