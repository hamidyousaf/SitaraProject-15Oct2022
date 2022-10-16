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
    public class CommissionPaymentController : Controller
    {
        private readonly NumbersDbContext _dbContext;


        public CommissionPaymentController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;

        }

        public IActionResult Index()
        {
            IList<BkgCommissionPayment> commissionPayments = _dbContext.BkgCommissionPayments
                .Include(b => b.Vehicle)
                .Include(b => b.Vehicle.Customer)                
                .Where(x => x.CompanyId == HttpContext.Session.GetInt32("CompanyId")).OrderByDescending(o => o.Id).ToList();
            return View(commissionPayments);
        }
        public IActionResult Create(int? id)
        {
            BkgCommissionPayment bkgCommissionPayment = new BkgCommissionPayment();
            AppConfigHelper helper = new AppConfigHelper(_dbContext, HttpContext);
            ViewBag.CommissionType = helper.GetCommissionType();
            if (id != null)
            {
                ViewBag.EntityState = "Update";
                bkgCommissionPayment = _dbContext.BkgCommissionPayments.Find(id);
                string configValues = _dbContext.AppCompanyConfigs
                .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                .Select(c => c.ConfigValue)
                .FirstOrDefault();
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                ViewBag.ReportPath = string.Concat(configValues, "Viewer", "?Report=Voucher&cId=", companyId, "&id={0}");
                return View("Create", bkgCommissionPayment);

            }
            else
            {
                bkgCommissionPayment.PaymentDate = DateTime.Now;
                ViewBag.EntityState = "Create";
                return View("Create", bkgCommissionPayment);
            }
        }
        [HttpPost]
        public async Task <IActionResult> Create(BkgCommissionPayment commissionPayment)
        {
            try
            {
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                string userId = HttpContext.Session.GetString("UserId");

                if (ModelState.IsValid)
                {
                    if (commissionPayment.Id == 0)
                    {
                        commissionPayment.CompanyId = companyId;
                        commissionPayment.CreatedBy = userId;
                        commissionPayment.CreatedDate = DateTime.Now;
                        commissionPayment.Status = "Created";
                        _dbContext.BkgCommissionPayments.Add(commissionPayment);
                    }
                    else
                    {
                        var data = _dbContext.BkgCommissionPayments.Find(commissionPayment.Id);
                        data.PaymentDate = commissionPayment.PaymentDate;
                        data.AccountId = commissionPayment.AccountId;
                        data.Description = commissionPayment.Description;
                        data.Commission = commissionPayment.Commission;
                        data.CommissionId = commissionPayment.CommissionId;
                        data.UpdatedBy = userId;
                        data.UpdatedDate = DateTime.Now;
                        var entry = _dbContext.BkgCommissionPayments.Update(data);
                        entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                    }
                  await  _dbContext.SaveChangesAsync();

                    TempData["error"] = "false";
                    TempData["message"] = "Payment has been saved successfully.";

                    return RedirectToAction("Index");
                }
                else
                    return View("Create", commissionPayment);
            }
            catch (Exception Exp)
            {
                ViewBag.success = false;
                ViewBag.message = (Exp.InnerException == null ? Exp.Message : Exp.InnerException.Message);
                return View("Create", commissionPayment);
            }
        }
        public async Task <IActionResult> Approve(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            //BkgVehicleIGP bkgIGP = _dbContext.BkgVehicleIGPs.Find(id);
            BkgCommissionPayment commission = _dbContext.BkgCommissionPayments
                .Include(s => s.Vehicle.Customer)
                .Where(a => a.Status == "Created" && a.CompanyId == companyId && a.Id == id).FirstOrDefault();

            //Create Voucher
            VoucherHelper voucher = new VoucherHelper(_dbContext, HttpContext);
            GLVoucher voucherMaster = new GLVoucher();
            List<GLVoucherDetail> voucherDetails = new List<GLVoucherDetail>();
            voucherMaster.VoucherType = "BK-CP";
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
           // var accountId = _dbContext.GLBankCashAccounts.Find(commission.Vehicle.Customer.AccountId).AccountId;
            //Debit Entry
            GLVoucherDetail voucherDetail = new GLVoucherDetail();
            voucherDetail.AccountId = commission.Vehicle.Customer.AccountId;
            voucherDetail.Sequence = 10;
            voucherDetail.Description = commission.Description;
            voucherDetail.Debit = commission.Commission;
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
            voucherDetail.Credit = commission.Commission;
            voucherDetail.IsDeleted = false;
            voucherDetail.CreatedBy = userId;
            voucherDetail.CreatedDate = DateTime.Now;
            voucherDetails.Add(voucherDetail);

            //Create Voucher 
            int voucherId = voucher.CreateVoucher(voucherMaster, voucherDetails);
            commission.VoucherId = voucherId;

            commission.Status = "Approved";
            commission.ApprovedBy = userId;
            commission.ApprovedDate = DateTime.Now;

            var entry=_dbContext.Update(commission);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();

            TempData["error"] = "false";
            TempData["message"] = "Commission has been approved successfully";           
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Detail(int id)
        {
            BkgCommissionPayment commissionPayment = _dbContext.BkgCommissionPayments.Include(b => b.Vehicle)
                .Include(b => b.Vehicle.Customer)
               .Where(x => x.Id == id).FirstOrDefault();
            return View("Detail", commissionPayment);
        }
    }
}