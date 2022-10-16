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
    public class PaymentController : Controller
    {
        private readonly NumbersDbContext _dbContext;


        public PaymentController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;

        }

        //[Route("Index")]
        public IActionResult Index()
        {
            int comPanyId = HttpContext.Session.GetInt32("CompanyId").Value;
            IList<BkgPayment> payments = _dbContext.BkgPayments
                .Include(b => b.Vehicle)
                .Include(b => b.Vehicle.Customer).Where(x => x.CompanyId == comPanyId).OrderByDescending(o => o.Id).ToList();

            //IList<BkgPayment> payments = _dbContext.BkgPayments.ToList();
            return View(payments);
        }
        public IActionResult Create(int? id)
        {
            BkgPayment payment = new BkgPayment();
            if (id != null)
            {
                ViewBag.EntityState = "Update";
                payment = _dbContext.BkgPayments.Find(id);
                //return View("PaymentCreate", payment);
                string configValues = _dbContext.AppCompanyConfigs
                            .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                            .Select(c => c.ConfigValue)
                            .FirstOrDefault();
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                ViewBag.ReportPath = string.Concat(configValues, "Viewer", "?Report=Voucher&cId=", companyId, "&id={0}"); return View("Create", payment);

            }
            else
            {
                payment.PaymentDate = DateTime.Now;
                ViewBag.EntityState = "Create";
                return View("Create", payment);
            }
        }
        [HttpPost]
          public async Task <IActionResult> Create(BkgPayment payment)
        {
            int comPanyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            //BkgPayment.Status=
            try
            {

                if (ModelState.IsValid)
                {
                    if (payment.Id == 0)
                    {
                        payment.CompanyId = comPanyId;
                        payment.CreatedBy = userId;
                        payment.Status = "Created";
                        payment.CreatedDate = DateTime.Now;
                        _dbContext.BkgPayments.Add(payment);
                    }
                    else
                    {
                        var dataToBeUpdated=_dbContext.BkgPayments.Find(payment.Id);
                        dataToBeUpdated.PaymentDate = payment.PaymentDate;
                        dataToBeUpdated.VehicleId = payment.VehicleId;
                        dataToBeUpdated.BankCashAccountId = payment.BankCashAccountId;
                        dataToBeUpdated.PaymentAmount = payment.PaymentAmount;
                        dataToBeUpdated.Reference = payment.Reference;
                        dataToBeUpdated.Description = payment.Description;
                        //_dbContext.Entry(payment).State = EntityState.Modified;
                        //_dbContext.Entry(payment).Property("CompanyId").IsModified = false;
                        //_dbContext.Entry(payment).Property("CreatedBy").IsModified = false;
                        //_dbContext.Entry(payment).Property("CreatedDate").IsModified = false;
                        //_dbContext.Entry(payment).Property("Status").IsModified = false;

                        dataToBeUpdated.UpdatedBy = userId;
                        dataToBeUpdated.UpdatedDate = DateTime.Now;
                        var entry = _dbContext.BkgPayments.Update(dataToBeUpdated);
                        entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                    }
                    await _dbContext.SaveChangesAsync();

                    TempData["error"] = "false";
                    TempData["message"] = "Payment has been saved successfully.";
                    return RedirectToAction("Index");
                }
                else
                    return View("Create", payment);
            }
            catch (Exception Exp)
            {
                TempData["error"] = "true";
                TempData["message"] = (Exp.InnerException == null ? Exp.Message : Exp.InnerException.Message);
                return View("Create", payment);
            }
        }
        public IActionResult Detail(int id)
        {
            BkgPayment payment = _dbContext.BkgPayments.Include(b => b.Vehicle)
                .Include(b => b.Vehicle.Customer)
                .Include(b => b.BankCashAccount).Where(x => x.Id == id).FirstOrDefault();
            return View("Detail", payment);
        }
        public IActionResult BookingConfirmation(int? id)
        {
            BkgPayment payment = new BkgPayment();

            ViewBag.EntityState = "Update";
            payment = _dbContext.BkgPayments
                .Include(b => b.Vehicle)
                .Include(b => b.Vehicle.Customer)
               .Include(b => b.Vehicle.Item).
               Where(x => x.Id == id).FirstOrDefault();

           
            return View("PaymentConfirmation", payment);


        }
        [HttpPost]
        public async Task <IActionResult> BookingConfirmation(BkgVehicle vehicle)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            try
            {
                BkgVehicle savedVehicle = _dbContext.BkgVehicles.Where(w => w.Id == vehicle.Id).FirstOrDefault();
                savedVehicle.UpdatedBy = userId;
                savedVehicle.UpdatedDate = DateTime.Now;
                savedVehicle.Status = "Pending Booking";

                savedVehicle.CashBankReceiptDate = vehicle.CashBankReceiptDate;
                savedVehicle.BankCashAccountId = vehicle.BankCashAccountId;
                var entry = _dbContext.BkgVehicles.Update(savedVehicle);
                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                await _dbContext.SaveChangesAsync();
                TempData["error"] = "false";
                TempData["message"] = "Data has been saved successfully.";
                return RedirectToAction("Approve");
            }
            catch (Exception Exp)
            {
                ViewBag.success = false;
                ViewBag.message = (Exp.InnerException == null ? Exp.Message : Exp.InnerException.Message);
                return View("BookingConfirmation", vehicle);
            }
        }
        public async Task <IActionResult> Approve(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            BkgPayment payment = _dbContext.BkgPayments
                                .Where(x => x.Id == id
                                && x.CompanyId == companyId)
                                .Include(p => p.Vehicle.Customer)
                                .FirstOrDefault();
            var accountId = (from c in _dbContext.AppCompanyConfigs
                             join v in _dbContext.BkgVehicles
                             on c.ConfigValue equals v.BookingType
                             where v.BookingType == c.ConfigValue && v.Id == payment.VehicleId && c.ConfigName == "Booking Type"
                             select c.UserValue1).First();
            if (payment.Status != "Created")
            {
                TempData["error"] = "true";
                TempData["message"] = string.Format("Payment against Id {0} has already been Approved", id);
                return RedirectToAction("Index");
            }
            else
            {
                #region "Create Voucher"
                VoucherHelper voucher = new VoucherHelper(_dbContext, HttpContext);
                GLVoucher voucherMaster = new GLVoucher();
                List<GLVoucherDetail> voucherDetails = new List<GLVoucherDetail>();
                var bankAccountId = _dbContext.GLBankCashAccounts.Find(payment.BankCashAccountId).AccountId;
                var vehicleData = _dbContext.BkgPayments
                                .Where(x => x.Id == id && x.CompanyId == companyId)
                                .Include(v => v.Vehicle)
                                .Include(c => c.Vehicle.Customer)
                                .Include(i => i.Vehicle.Item)
                                .FirstOrDefault();
                string voucherDescription = string.Format(
                    "Cash/Chq {0}" +
                    "Deposited agt. balance payment of Booking ID # {1} " +
                    "Booking # {2} " +
                    "Receipt # {3} " +
                    "Model {4} " +
                    "PA {5} " +
                    "of {6} ",
                    vehicleData.Vehicle.ReceiptType,
                    vehicleData.Id,
                    vehicleData.Vehicle.BookingNo,
                    vehicleData.Vehicle.ReferenceNo,
                    vehicleData.Vehicle.Item.Name,
                    vehicleData.Vehicle.PaymentAdviceNo,
                    vehicleData.Vehicle.Customer.Name);

                voucherMaster.VoucherType = "BKP";
                voucherMaster.VoucherDate = payment.PaymentDate;
                voucherMaster.Reference = payment.Reference;
                voucherMaster.Currency = "PKR";
                voucherMaster.CurrencyExchangeRate = 1;
                voucherMaster.Description = voucherDescription.Substring(0, voucherDescription.Length);
                voucherMaster.Status = "Approved";
                voucherMaster.IsSystem = true;
                voucherMaster.ModuleName = "Booking Payment";
                voucherMaster.ModuleId = id;

                //Voucher Details

                //Debit Entry
                GLVoucherDetail voucherDetail = new GLVoucherDetail();
                voucherDetail.AccountId = Convert.ToInt32(accountId);
                voucherDetail.Sequence = 10;
                voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                voucherDetail.Debit = payment.PaymentAmount; // for payment payment.PaymentAmount
                voucherDetail.Credit = 0; // for payment 0 
                voucherDetail.IsDeleted = false;
                voucherDetail.CreatedBy = userId;
                voucherDetail.CreatedDate = DateTime.Now;
                voucherDetails.Add(voucherDetail);


                //Credit Entry
                voucherDetail = new GLVoucherDetail();
                voucherDetail.AccountId = bankAccountId;
                voucherDetail.Sequence = 20;
                voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                voucherDetail.Debit = 0; // for payment 0 
                voucherDetail.Credit = payment.PaymentAmount;  // for payment payment.PaymentAmount
                voucherDetail.IsDeleted = false;
                voucherDetail.CreatedBy = userId;
                voucherDetail.CreatedDate = DateTime.Now;
                voucherDetails.Add(voucherDetail);


                //Create Voucher 
                int voucherId = voucher.CreateVoucher(voucherMaster, voucherDetails);

                #endregion

                payment.VoucherId = voucherId;
                payment.Status = "Approved";
                payment.UpdatedBy = userId;
                payment.UpdatedDate = DateTime.Now;
                _dbContext.BkgPayments.Update(payment);

                //Update total received amount to BkgVehicle
                BkgVehicle vehicle = _dbContext.BkgVehicles.Find(payment.VehicleId);
                vehicle.PaymentAmount = vehicle.PaymentAmount + payment.PaymentAmount;

                var entry = _dbContext.BkgVehicles.Update(vehicle);
                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                await _dbContext.SaveChangesAsync();

                TempData["error"] = "false";
                TempData["message"] = string.Format("Payment Against Id {0} has been Approved", id);
                return RedirectToAction("Index");
            }
        }
    }
}