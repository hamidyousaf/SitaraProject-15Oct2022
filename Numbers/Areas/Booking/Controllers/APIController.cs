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
using Microsoft.AspNetCore.Http;

namespace Numbers.Areas.Booking.Controllers
{
    [Authorize]
    [Area("Booking")]
    public class ApiController : Controller
    {
        private readonly NumbersDbContext _dbContext;


        public ApiController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;

        }

        [HttpGet]
        public IActionResult GetCustomers(string type, string q = "")
        {
            var customers = _dbContext.BkgCustomers
                                               .Where(a => a.IsActive == true && a.CompanyId == HttpContext.Session.GetInt32("CompanyId") && a.Name.Contains(q))
                                               .Where(a => a.IsBooking == (type == "Booking" ? true : a.IsBooking))
                                               .Where(a => a.IsCustomer == (type == "Customer" ? true : a.IsCustomer))
                                               .Where(a => a.IsSupplier == (type == "Supplier" ? true : a.IsSupplier))
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = string.Concat(a.CNIC, " - ", a.Name)
                                               })
                                               .OrderBy(a => a.text)
                                               .Take(25)
                                               .ToList();
            return Ok(customers);
        }
        [HttpGet]
        public IActionResult GetCustomer(int id)
        {
            var customer = _dbContext.BkgCustomers.Where(a => a.CompanyId == HttpContext.Session.GetInt32("CompanyId") && a.Id == id)
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = string.Concat(a.CNIC, " - ", a.Name)
                                               })
                                               .FirstOrDefault();
            return Ok(customer);
        }


        [HttpGet]
        public IActionResult GetItems(string q = "")
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var items = _dbContext.InvItems.Where(
                                                (a => a.CompanyId == companyId && a.IsDeleted==false && a.Name.Contains(q)
                                               ))
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = string.Concat(a.Name)
                                               })
                                               .OrderBy(a => a.text)
                                               .Take(25)
                                               .ToList();
            return Ok(items);
        }
        [HttpGet]
        public IActionResult GetItem(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var items = _dbContext.InvItems.Where(a => a.Id == id && a.CompanyId == companyId && a.IsDeleted == false)
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = string.Concat(a.Name)
                                               })
                                               .FirstOrDefault();
            return Ok(items);
        }

        [HttpGet]
        public IActionResult GetVehicles(string q = "")
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string notInStatus = "Cancel,Reversed,Booking,Pending Booking";
            string[] notInStatusA = notInStatus.Split(',');
            var vehicles = _dbContext.BkgVehicles.Where(
                                                a => ((a.Customer.Name.Contains(q) || a.BookingNo.Contains(q))&& !notInStatusA.Contains(a.Status) && a.CompanyId==companyId
                                               ))
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = string.Concat(a.BookingNo, " - ", a.Customer.Name)
                                               })
                                               .OrderBy(a => a.text)
                                               .Take(25)
                                               .ToList();
            return Ok(vehicles);
        }
        [HttpGet]
        public IActionResult GetVehicle(int id)
        {
            var vehicle = _dbContext.BkgVehicles.Where(a => a.Id == id)
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = string.Concat(a.BookingNo, " - ", a.Customer.Name)
                                               })
                                               .FirstOrDefault();
            return Ok(vehicle);
        }
        [HttpGet]
        public IActionResult GetBookingNo(string q = "")
        {
            var vehicles = _dbContext.BkgVehicles.Where(
                                                a => ((a.Customer.Name.Contains(q) || a.BookingNo.Contains(q))
                                               ))
                                               .Select(a => new
                                               {
                                                   id = a.BookingNo,
                                                   text = string.Concat(a.BookingNo, " - ", a.Customer.Name)
                                               })
                                               .OrderBy(a => a.text)
                                               .Take(25)
                                               .ToList();
            return Ok(vehicles);
        }
        [HttpGet]
        public IActionResult GetVehicleIGPs(string q = "")
        {
            string status = "Booked";
            var vehicles = _dbContext.BkgVehicles.Where(
                                                a => ((a.Customer.Name.Contains(q) || a.BookingNo.Contains(q)) && a.Status.Contains(status)
                                               ))
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = string.Concat(a.BookingNo, " - ", a.Customer.Name)
                                               })
                                               .OrderBy(a => a.text)
                                               .Take(25)
                                               .ToList();
            return Ok(vehicles);
        }
        [HttpGet]
        public IActionResult GetVehicleOGPs(string q = "")
        {
            string status = "Received";
            var vehicles = _dbContext.BkgVehicles.Where(
                                                a => ((a.Customer.Name.Contains(q) || a.BookingNo.Contains(q)) &&                                   a.Status.Contains(status)
                                               ))
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = string.Concat(a.BookingNo, " - ", a.Customer.Name)
                                               })
                                               .OrderBy(a => a.text)
                                               .Take(25)
                                               .ToList();
            return Ok(vehicles);
        }
        [HttpGet]
        public IActionResult GetUnpostVehicleIGPs(string q = "")
        {
            string status = "Approved";
            var vehicles = _dbContext.BkgVehicleIGPs.Where(
                 i => ((i.Description.Contains(q)) && i.Status.Contains(status)
                                               ))
                .Select(i => new
                {
                    id = i.Id,
                text = string.Concat("Trx.Id: ", i.Id, " - IGPNo: ", i.IGPNo," - Engine#: ",i.EngineNo, " - Chassis#: ",i.ChassisNo)
                })
                .OrderBy(a => a.text)
                .Take(25)
                .ToList();
            return Ok(vehicles);
        }
        [HttpGet]
        public IActionResult GetUnpostVehicleOGPs(string q = "")
        {
            string status = "Approved";
            var vehicles = _dbContext.BkgVehicleOGPs.Where(
                 o => ((o.Description.Contains(q)) && o.Status.Contains(status)
                                               ))
                .Select(o => new
                {
                    id = o.Id,
                    text = string.Concat("Trx.Id: ", o.Id, " - OGPNo: ", o.OGPNo, " - Narration: ", o.Description, " - Received By: ", o.ReceivedBy)
                })
                .OrderBy(a => a.text)
                .Take(25)
                .ToList();
            return Ok(vehicles);
        }
        [HttpGet]
        public IActionResult GetReceivableVehicles(string q = "")
        {
            string notInStatus = "Cancel,Reversed,Booking,Pending Booking";
            string[] notInStatusA = notInStatus.Split(',');
            var vehicles = _dbContext.BkgVehicles.Where(
                                                a => ((a.Customer.Name.Contains(q) || a.BookingNo.Contains(q)) && a.Price - a.ReceivedAmount > 0 && !notInStatusA.Contains(a.Status)
                                               ))
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = string.Concat(a.BookingNo, " - ", a.Customer.Name)
                                               })
                                               .OrderBy(a => a.text)
                                               .Take(25)
                                               .ToList();
            return Ok(vehicles);
        }
        [HttpGet]
        public IActionResult GetPayableVehicles(string q = "")
        {
            string notInStatus = "Booking,Pending Booking";
            string[] notInStatusA = notInStatus.Split(',');
            var vehicles = _dbContext.BkgVehicles.Where(
                                                a => (a.Customer.Name.Contains(q) || a.BookingNo.Contains(q)) && (a.Price - a.PaymentAmount) > 0 && !notInStatusA.Contains(a.Status)
                                               )
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = string.Concat(a.BookingNo, " - ", a.Customer.Name)
                                               })
                                               .OrderBy(a => a.text)
                                               .Take(25)
                                               .ToList();
            return Ok(vehicles);
        }

        [HttpGet]
        public IActionResult GetReceiptTypes(string q = "")
        {
            var receiptTypes = _dbContext.AppCompanyConfigs.Where(
                                                (a => a.ConfigValue.Contains(q)
                                               )).Where(w => w.ConfigName == "Receipt Type")

                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = a.ConfigValue
                                               })
                                               .OrderBy(a => a.text)
                                              .ToList();
            return Ok(receiptTypes);
        }
        [HttpGet]
        public IActionResult GetVoucherTypes(string q = "")
        {
            var voucherTypes = _dbContext.GLVoucherTypes.Where((a => a.VoucherType.Contains(q)))
                .Select(a => new{id = a.Id,text = a.VoucherType})
                .OrderBy(a => a.text)
                .ToList();
            return Ok(voucherTypes);
        }
        [HttpGet]
        public IActionResult GetReceiptType(int id)
        {
            var receiptTypes = _dbContext.AppCompanyConfigs.Where(w => w.ConfigName == "Receipt Type" && w.Id == id)
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = a.ConfigValue
                                               })
                                               .FirstOrDefault();
            return Ok(receiptTypes);
        }

        [HttpGet]
        public IActionResult PartialDetail(int id)
        {
            BkgVehicle vehicles = _dbContext.BkgVehicles.Include(b => b.Customer).Include(b => b.Item).Where(x => x.Id == id).FirstOrDefault();
            return PartialView("_Details", vehicles);           
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            BkgVehicle vehicles = _dbContext.BkgVehicles.Include(b => b.Customer).Include(b => b.Item).Where(x => x.Id == id).FirstOrDefault();
            return Json(vehicles);
        }
        [HttpGet]
        public JsonResult CheckBookingNoAlreadyExists(string bkgNo)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            // System.Threading.Thread.Sleep(200);
            //if (bookingNo.Length == 0)
            //    return Json(0);
            var returnVal = _dbContext.BkgVehicles
                           .Where(a => a.CompanyId == companyId && a.BookingNo == Convert.ToString(bkgNo)).FirstOrDefault();
            return Json(returnVal);
        }
        [HttpGet]
        public JsonResult GetBookingInformation(int bkgId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            // System.Threading.Thread.Sleep(200);
            //if (bookingNo.Length == 0)
            //    return Json(0);
            var returnVal = _dbContext.BkgVehicles
                           .Where(a => a.CompanyId == companyId && a.Id == bkgId).FirstOrDefault();
            return Json(returnVal);
        }
        [HttpGet]
        public JsonResult CheckCustomerCNICAlreadyExist(string cnic)
        {
            //System.Threading.Thread.Sleep(200);
            if (cnic.Length == 0)
                return Json(0);

            var chkCNIC = _dbContext.BkgCustomers.Where(a => a.CNIC == cnic)
                                               .Select(a => a.CNIC)
                                               .FirstOrDefault();
            return Json(chkCNIC == null ? 0 : 1);
        }
        public IActionResult GetVehiclesById(string q = "")
        {
            string status = "Cancel";
            var vehicles = _dbContext.BkgVehicles.Where(
                         a => a.Status != status && a.CompanyId == HttpContext.Session.GetInt32("CompanyId").Value)
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = string.Concat(a.Id, " - ", a.BookingNo)
                                               })
                                               .OrderBy(a => a.text)
                                               .Take(25)
                                               .ToList();
            return Ok(vehicles);
        }

    }
}