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
    public class ReceiptController : Controller
    {
        private readonly NumbersDbContext _dbContext;


        public ReceiptController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;

        }
        public IActionResult Index()
        {
            int comPanyId = HttpContext.Session.GetInt32("CompanyId").Value;
            IList<BkgReceipt> BkgReceipts = _dbContext.BkgReceipts
                .Include(b => b.Vehicle)
                .Include(b => b.Vehicle.Customer)
                .Include(b => b.Vehicle.Item)
                .Where(x => x.CompanyId == comPanyId).OrderByDescending(o => o.Id)
                .OrderByDescending(x => x.Id)
                .ToList();
            return View(BkgReceipts);
        }

        public IActionResult Create(int? id)
        {
            BkgReceipt receipt = new BkgReceipt();
            if (id != null)
            {
                ViewBag.EntityState = "Update";
                receipt = _dbContext.BkgReceipts.Find(id);
               
                string configValues = _dbContext.AppCompanyConfigs
               .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
               .Select(c => c.ConfigValue)
               .FirstOrDefault();
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                ViewBag.ReportPath = string.Concat(configValues, "Viewer", "?Report=Voucher&cId=", companyId, "&id={0}"); return View("Create", receipt);
            }
            else
            {
                ViewBag.EntityState = "Create";
                return View("Create", receipt);
            }
        }
        [HttpPost]
        public async Task <IActionResult> Create(BkgReceipt receipt)
        {
            int comPanyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            try
            {
                if (ModelState.IsValid)
                {
                    if (receipt.Id == 0)
                    {
                        receipt.CompanyId = comPanyId;
                        receipt.CreatedBy = userId;
                        receipt.CreatedDate = DateTime.Now;
                        receipt.Status = "Created";
                        _dbContext.BkgReceipts.Add(receipt);
                    }
                    else
                    {
                        var dataToBeUpdated = _dbContext.BkgReceipts.Find(receipt.Id);
                        dataToBeUpdated.ReceiptDate = receipt.ReceiptDate;
                        dataToBeUpdated.VehicleId = receipt.VehicleId;
                        dataToBeUpdated.ReceiptAccount = receipt.ReceiptAccount;
                        dataToBeUpdated.ReceiptAmount = receipt.ReceiptAmount;
                        dataToBeUpdated.BankCashAccountId = receipt.BankCashAccountId;
                        dataToBeUpdated.Reference = receipt.Reference;
                        dataToBeUpdated.Description = receipt.Description;
                        /*
                        _dbContext.Entry(receipt).State = EntityState.Modified;
                        _dbContext.Entry(receipt).Property("CompanyId").IsModified = false;
                        _dbContext.Entry(receipt).Property("CreatedBy").IsModified = false;
                        _dbContext.Entry(receipt).Property("CreatedDate").IsModified = false;
                        _dbContext.Entry(receipt).Property("Status").IsModified = false;
                        */
                        dataToBeUpdated.UpdatedBy = userId;
                        dataToBeUpdated.UpdatedDate = DateTime.Now;
                        var entry = _dbContext.BkgReceipts.Update(dataToBeUpdated);
                        entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                    }
                   await _dbContext.SaveChangesAsync();

                    TempData["error"] = "false";
                    TempData["message"] = "Receipt has been saved successfully.";
                    return RedirectToAction("Index");
                }
                else
                    return View("Create", receipt);
            }
            catch (Exception Exp)
            {
                TempData["error"] = "true";
                TempData["message"] = (Exp.InnerException == null ? Exp.Message : Exp.InnerException.Message);
                return View("Create", receipt);
            }
        }
        public IActionResult Detail(int id)
        {
            BkgReceipt receipt = _dbContext.BkgReceipts.Include(b => b.Vehicle)
                .Include(b => b.Vehicle.Customer).Where(x => x.Id == id).FirstOrDefault();
            return View("ReceiptDetail", receipt);
        }        
        public async Task <IActionResult> Approve(int id)
        {
            int comPanyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            BkgReceipt receipt = _dbContext.BkgReceipts
                        .Where(x => x.Id == id && 
                        x.CompanyId == comPanyId)
                        .Include(r => r.Vehicle.Customer)
                        .FirstOrDefault();
            if (receipt.Status != "Created")
            {
                TempData["error"] = "true";
                TempData["message"] = string.Format("Receipt Id {0} has been already Approved", id);
                return RedirectToAction("Index");
            }
            else
            {
                var bankAccountId = _dbContext.GLBankCashAccounts.Find(receipt.BankCashAccountId).AccountId;
                var vehicleData = _dbContext.BkgVehicles.Include(i => i.Item).Include(c => c.Customer)
                    .Where(x => x.Id == receipt.VehicleId && x.CompanyId == HttpContext.Session.GetInt32("CompanyId"))
                    .FirstOrDefault();
                int voucherId = 0;
                if (!vehicleData.BookingType.Contains("Own"))
                {
                    #region  "Create Voucher"
                    VoucherHelper voucher = new VoucherHelper(_dbContext, HttpContext);
                    GLVoucher voucherMaster = new GLVoucher();
                    List<GLVoucherDetail> voucherDetails = new List<GLVoucherDetail>();

                      string voucherDescription = string.Format(
                     "Funds/Cash Receined agt.Booking ID #  {0} " +
                     "Receipt #  {1} " +
                     "Model  {2} " +
                     "Booking # {3} " +
                     "From {4}",
                     vehicleData.Id,
                     vehicleData.ReferenceNo,
                     vehicleData.Item.Name,
                     vehicleData.BookingNo,
                     vehicleData.Customer.Name);

                    voucherMaster.VoucherType = "BKR";
                    voucherMaster.VoucherDate = receipt.ReceiptDate;
                    voucherMaster.Reference = receipt.Reference;
                    voucherMaster.Currency = "PKR";
                    voucherMaster.CurrencyExchangeRate = 1;
                    voucherMaster.Description = voucherDescription.Substring(0, voucherDescription.Length);
                    voucherMaster.Status = "Approved";
                    voucherMaster.IsSystem = true;
                    voucherMaster.ModuleName = "Booking Receipt";
                    voucherMaster.ModuleId = id;

                    //Voucher Details

                    //Debit Entry
                    GLVoucherDetail voucherDetail = new GLVoucherDetail();
                    voucherDetail.AccountId = bankAccountId;
                    voucherDetail.Sequence = 10;
                    voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                    voucherDetail.Debit = receipt.ReceiptAmount; // for payment 0 
                    voucherDetail.Credit = 0;  // for payment payment.PaymentAmount
                    voucherDetail.IsDeleted = false;
                    voucherDetail.CreatedBy = userId;
                    voucherDetail.CreatedDate = DateTime.Now;
                    voucherDetails.Add(voucherDetail);

                    //Credit Entry
                    voucherDetail = new GLVoucherDetail();
                    voucherDetail.AccountId = receipt.Vehicle.Customer.AccountId;
                    voucherDetail.Sequence = 20;
                    voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                    voucherDetail.Debit = 0; // for payment payment.PaymentAmount
                    voucherDetail.Credit = receipt.ReceiptAmount; // for payment 0 
                    voucherDetail.IsDeleted = false;
                    voucherDetail.CreatedBy = userId;
                    voucherDetail.CreatedDate = DateTime.Now;
                    voucherDetails.Add(voucherDetail);

                    //Create Voucher 
                    voucherId = voucher.CreateVoucher(voucherMaster, voucherDetails);
                    #endregion
                }
                receipt.VoucherId = voucherId;
                receipt.Status = "Approved";
                receipt.UpdatedBy = userId;
                receipt.UpdatedDate = DateTime.Now;
                var entry = _dbContext.BkgReceipts.Update(receipt);
                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());

                //Update total received amount to BkgVehicle
                BkgVehicle vehicle = _dbContext.BkgVehicles.Find(receipt.VehicleId);
                vehicle.ReceivedAmount = vehicle.ReceivedAmount + receipt.ReceiptAmount;

                var dbEntry = _dbContext.BkgVehicles.Update(vehicle);
                dbEntry.OriginalValues.SetValues(await dbEntry.GetDatabaseValuesAsync());
                await _dbContext.SaveChangesAsync();

                TempData["error"] = "false";
                TempData["message"] = string.Format("Receipt Id {0} has been Approved", id);
                return RedirectToAction("Index");
            }
        }
    }
}