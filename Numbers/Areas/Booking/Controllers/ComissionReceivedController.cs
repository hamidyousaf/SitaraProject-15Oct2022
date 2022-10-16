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
    public class CommissionReceivedController : Controller
    {
        private readonly NumbersDbContext _dbContext;


        public CommissionReceivedController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;

        }
        public IActionResult Index()
        {

            IList<BkgCommissionReceived> commissionReceiveds = _dbContext.BkgCommissionReceiveds
                .Include(b => b.Vehicle)
                .Include(b => b.Vehicle.Customer)
                .Where(x => x.CompanyId == HttpContext.Session.GetInt32("CompanyId")).OrderByDescending(o => o.Id).ToList();
            return View(commissionReceiveds);
        }
        public IActionResult Create(int? id)
        {
            BkgCommissionReceived commissionReceived = new BkgCommissionReceived();
            if (id != null)
            {
                ViewBag.EntityState = "Update";
                commissionReceived = _dbContext.BkgCommissionReceiveds.Find(id);
                string configValues = _dbContext.AppCompanyConfigs
                                .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                                .Select(c => c.ConfigValue)
                                .FirstOrDefault();
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                ViewBag.ReportPath = string.Concat(configValues, "Viewer", "?Report=Voucher&cId=", companyId, "&id={0}");
                return View("Create", commissionReceived);

            }
            else
            {
                commissionReceived.ReceivedDate = DateTime.Now;
                ViewBag.EntityState = "Create";
                return View("Create", commissionReceived);
            }
        }
        [HttpPost]
        public async Task <IActionResult> Create(BkgCommissionReceived commissionReceived)
        {
            try
            {
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                string userId = HttpContext.Session.GetString("UserId");

                if (ModelState.IsValid)
                {
                    if (commissionReceived.Id == 0)
                    {
                        commissionReceived.CompanyId = companyId;
                        commissionReceived.CreatedBy = userId;
                        commissionReceived.CreatedDate = DateTime.Now;
                        commissionReceived.Status = "Created";
                        _dbContext.BkgCommissionReceiveds.Add(commissionReceived);
                    }
                    else
                    {
                        var data = _dbContext.BkgCommissionReceiveds.Find(commissionReceived.Id);
                        data.ReceivedDate = commissionReceived.ReceivedDate;
                        data.BookingCommission = commissionReceived.BookingCommission;
                        data.BankCashAccountId = commissionReceived.BankCashAccountId;
                        data.AreaCommission = commissionReceived.AreaCommission;
                        data.VehicleId = commissionReceived.VehicleId;
                        data.DeliveryCommission = commissionReceived.DeliveryCommission;
                        data.Description = commissionReceived.Description;
                        data.UpdatedBy = userId;
                        data.UpdatedDate = DateTime.Now;
                        var entry = _dbContext.BkgCommissionReceiveds.Update(data);
                        entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                    }
                  await  _dbContext.SaveChangesAsync();

                    TempData["error"] = "false";
                    TempData["message"] = "Commission has been saved successfully.";
                    return RedirectToAction("Index");
                }
                else
                    return View("Create", commissionReceived);
            }
            catch (Exception Exp)
            {
                TempData["error"] = "true";
                TempData["message"] = (Exp.InnerException == null ? Exp.Message : Exp.InnerException.Message);
                return View("Create", commissionReceived);
            }
        }
        public IActionResult Detail(int id)
        {
            BkgCommissionReceived commissionReceived = _dbContext.BkgCommissionReceiveds.Include(b => b.Vehicle)
                .Include(b => b.Vehicle.Customer).Where(x => x.Id == id).FirstOrDefault();
            return View("Detail", commissionReceived);
        }
        public async Task <IActionResult> Approve(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            BkgCommissionReceived commission = _dbContext.BkgCommissionReceiveds
                .Where(a => a.Status == "Created" && a.CompanyId == companyId && a.Id == id)
                .Include(c => c.Vehicle.Customer)
                .FirstOrDefault();

            //Create Voucher
            VoucherHelper voucher = new VoucherHelper(_dbContext, HttpContext);
            GLVoucher voucherMaster = new GLVoucher();
            List<GLVoucherDetail> voucherDetails = new List<GLVoucherDetail>();
            voucherMaster.VoucherType = "BK-CR";
            voucherMaster.VoucherDate = commission.CreatedDate;
            voucherMaster.Reference = commission.Vehicle.BookingNo;
            voucherMaster.Currency = "PKR";
            voucherMaster.CurrencyExchangeRate = 1;
            voucherMaster.Description = commission.Description;
            voucherMaster.Status = "Approved";
            voucherMaster.IsSystem = true;
            voucherMaster.ModuleName = "Booking Voucher";
            voucherMaster.ModuleId = id;

            //Voucher Details

            //Debit Entry
            GLVoucherDetail voucherDetail = new GLVoucherDetail();
            voucherDetail.AccountId = commission.Vehicle.Customer.AccountId;
            voucherDetail.Sequence = 10;
            voucherDetail.Description = commission.Description;
            voucherDetail.Debit = commission.AreaCommission;
            voucherDetail.Credit = 0;
            voucherDetail.IsDeleted = false;
            voucherDetail.CreatedBy = userId;
            voucherDetail.CreatedDate = DateTime.Now;
            voucherDetails.Add(voucherDetail);

            //Credit Entry
            voucherDetail = new GLVoucherDetail();
            voucherDetail.AccountId = commission.Vehicle.Customer.AccountId;
            voucherDetail.Sequence = 20;
            voucherDetail.Description = commission.Description;
            voucherDetail.Debit = 0;
            voucherDetail.Credit = commission.AreaCommission;
            voucherDetail.IsDeleted = false;
            voucherDetail.CreatedBy = userId;
            voucherDetail.CreatedDate = DateTime.Now;
            voucherDetails.Add(voucherDetail);

            //Create Voucher 
            int voucherId = voucher.CreateVoucher(voucherMaster, voucherDetails);
            commission.VoucherId = voucherId;
            commission.Status = "Approved";
            commission.UpdatedBy = userId;
            commission.UpdatedDate = DateTime.Now;

            // Update BkgVehicle
            TempData["error"] = "false";
            TempData["message"] = "Commission has been approved successfully";

            // Approve/Update OGP
            var entry = _dbContext.BkgCommissionReceiveds.Update(commission);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}