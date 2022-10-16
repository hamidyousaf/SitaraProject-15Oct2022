using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Helpers;

namespace Numbers.Areas.Booking.Controllers
{
    [Authorize]
    [Area("Booking")]
   //[Route("Booking/IGP")]
    public class IGPController : Controller
    {
        private readonly NumbersDbContext _dbContext;


        public IGPController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;

        }

        // [Route("Index")]
        public IActionResult Index()
        {
            //BkgVehicleIGP igp = _dbContext.BkgVehicleIGPs.FirstOrDefault();
           

            IList<BkgVehicleIGP> list = _dbContext.BkgVehicleIGPs
                .Include(b => b.Vehicle)
                .Include(b => b.Vehicle.Customer)
                .Include(b => b.Vehicle.Item)
                .Where(x => x.CompanyId == HttpContext.Session.GetInt32("CompanyId"))
                .OrderByDescending(o => o.Id)
                .ToList();
            
            return View(list);

        }
        public IActionResult Create(int? id)
        {
            //ViewBag.Reference = new SelectList(_dbContext.BkgVehicles               
            //    .ToList(), "ReferenceNo", "ReferenceNo");
            BkgVehicleIGP igp = new BkgVehicleIGP();
            if (id != null)
            {
                ViewBag.EntityState = "Update";

                igp = _dbContext.BkgVehicleIGPs.Find(id);
                string configValues = _dbContext.AppCompanyConfigs
               .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
               .Select(c => c.ConfigValue)
               .FirstOrDefault();
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                ViewBag.ReportPath = string.Concat(configValues, "Viewer", "?Report=Voucher&cId=", companyId, "&id={0}");
                return View("Create", igp);

            }
            else
            {
                igp.IGPDate = DateTime.Now;
                ViewBag.EntityState = "Create";
                return View("Create", igp);
            }
        }
        [HttpPost]
        public async Task <IActionResult> Create(BkgVehicleIGP igp)
        {
            try
            {
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                string userId = HttpContext.Session.GetString("UserId");
                if (ModelState.IsValid)
                {
                    if (igp.Id == 0)
                    {
                        igp.CompanyId = companyId;
                        igp.CreatedBy = userId;
                        igp.CreatedDate = DateTime.Now;
                        igp.Status = "Created";                        
                        _dbContext.BkgVehicleIGPs.Add(igp);
                    }
                    else
                    {
                        var data = _dbContext.BkgVehicleIGPs.Find(igp.Id);
                        data.IGPDate = igp.IGPDate;
                        data.IGPNo = igp.IGPNo;
                        data.VehicleId = igp.VehicleId;
                        data.InvoiceNo = igp.InvoiceNo;
                        data.DeliveryCommission = igp.DeliveryCommission;
                        data.ChassisNo = igp.ChassisNo;
                        data.EngineNo = igp.EngineNo;
                        data.InsuranceAmount = igp.InsuranceAmount;
                        data.Description = igp.Description;
                        data.AdvanceBookingPurchase = igp.AdvanceBookingPurchase;
                        data.UpdatedBy = userId;
                        data.UpdatedDate = DateTime.Now;
                        var entry = _dbContext.BkgVehicleIGPs.Update(data);
                        entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                    }
                   await _dbContext.SaveChangesAsync();

                    TempData["error"] = "false";
                    TempData["message"] = "IGP has been saved successfully.";
                    return RedirectToAction("Index");
                }
                else
                    return View("Create", igp);
            }
            catch (Exception Exp)
            {
                TempData["error"] = "true";
                TempData["message"] = (Exp.InnerException == null ? Exp.Message : Exp.InnerException.Message);
                return View("Create", igp);
            }
        }

        public async Task <IActionResult> Approve(int id)
        {
            int companyId= HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");

            //BkgVehicleIGP bkgIGP = _dbContext.BkgVehicleIGPs.Find(id);
               BkgVehicleIGP igp = _dbContext.BkgVehicleIGPs
                .Include(v => v.Vehicle)
                .Include(i=> i.Vehicle.Item)
                .Include(c => c.Vehicle.Customer)
                .Where(a => a.Status == "Created" && a.CompanyId == companyId && a.Id == id)
                .FirstOrDefault();
            try
            {
                //Create Voucher
                int voucherId = 0;
                if (!igp.Vehicle.BookingType.Contains("Own"))
                { 
                    //Own Booking
                    VoucherHelper voucher = new VoucherHelper(_dbContext, HttpContext);
                    GLVoucher voucherMaster = new GLVoucher();
                    List<GLVoucherDetail> voucherDetails = new List<GLVoucherDetail>();
                    string voucherDescription = string.Format(
                        "D/O M-F 240 from MTL agt.IGP # {0} " +
                        "Booking ID # {1} " +
                        "R # {2} " +
                        "Booking # {3} " +
                        "Chassis # {4} " +
                        "Engine # {5} " +
                        "Invoice # {6} " +
                        "Invoice Date {7} " +
                        "Party Name {8} ",
                        igp.IGPNo,
                        igp.Vehicle.Id,
                        igp.Vehicle.ReferenceNo, 
                        igp.Vehicle.BookingNo, 
                        igp.ChassisNo, 
                        igp.EngineNo, 
                        igp.InvoiceNo,
                        igp.IGPDate, 
                        igp.Vehicle.Customer.Name);

                    voucherMaster.VoucherType = "BKI";
                    voucherMaster.VoucherDate = igp.IGPDate;
                    voucherMaster.Reference = igp.Vehicle.ReferenceNo;
                    voucherMaster.Currency = "PKR";
                    voucherMaster.CurrencyExchangeRate = 1;
                    voucherMaster.Description = voucherDescription.Substring(0, voucherDescription.Length);
                    voucherMaster.Status = "Approved";
                    voucherMaster.IsSystem = true;
                    voucherMaster.ModuleName = "Booking IGP";
                    voucherMaster.ModuleId = id;

                    //Voucher Details
                    //Debit Entry
                    GLVoucherDetail voucherDetail = new GLVoucherDetail();


                    if (igp.AdvanceBookingPurchase)
                        voucherDetail.AccountId = igp.Vehicle.Customer.AccountId;
                    else
                        voucherDetail.AccountId = igp.Vehicle.Item.CustomerStockAccountId;

                    voucherDetail.Sequence = 10;
                    voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                    voucherDetail.Debit = igp.Vehicle.Price; // for payment 0 
                    voucherDetail.Credit = 0;
                    voucherDetail.IsDeleted = false;
                    voucherDetail.CreatedBy = userId;
                    voucherDetail.CreatedDate = DateTime.Now;
                    voucherDetails.Add(voucherDetail);

                    //Credit Entry
                    var accountId = (from c in _dbContext.AppCompanyConfigs
                                     where c.ConfigName == "Booking Type" && c.ConfigValue == igp.Vehicle.BookingType
                                     select c.UserValue1).First();
                    voucherDetail = new GLVoucherDetail();
                    voucherDetail.AccountId = Convert.ToInt32(accountId);
                    voucherDetail.Sequence = 20;
                    voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                    voucherDetail.Debit = 0; // for payment payment.PaymentAmount
                    voucherDetail.Credit = igp.Vehicle.Price; // for payment 0 
                    voucherDetail.IsDeleted = false;
                    voucherDetail.CreatedBy = userId;
                    voucherDetail.CreatedDate = DateTime.Now;
                    voucherDetails.Add(voucherDetail);

                    //Create Voucher 
                    voucherId = voucher.CreateVoucher(voucherMaster, voucherDetails);
                }

                igp.VoucherId = voucherId;
                igp.Status = "Approved";
                igp.UpdatedBy = userId;
                igp.UpdatedDate = DateTime.Now;

                //Update Booking Status field in BkgVehicle
                igp.Vehicle.Status = "Received";
                igp.Vehicle.ChassisNo = igp.ChassisNo;
                igp.Vehicle.EngineNo = igp.EngineNo;

                //On approval updating bkgVehicle
                TempData["error"] = "false";
                TempData["message"] = "IGP has been approved successfully";
                var entry = _dbContext.Update(igp);
                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["error"] = true;
                TempData["message"] = ex.InnerException.Message;
                return RedirectToAction(nameof(Index));
            }
        }
            public IActionResult Detail(int id)
        {
            BkgVehicleIGP igp = _dbContext.BkgVehicleIGPs.Include(b => b.Vehicle)
                .Include(b => b.Vehicle.Customer)
                .Include(b => b.Vehicle.Item).Where(w => w.Id == id).FirstOrDefault();

            return View("Detail", igp);
        }
    }
}