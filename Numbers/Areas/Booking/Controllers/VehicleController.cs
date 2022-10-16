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
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;

namespace Numbers.Areas.Booking.Controllers
{
    [Authorize]
    [Area("Booking")]
    public class VehicleController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public VehicleController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            if (TempData["success"] != null)
            {
                ViewBag.success = TempData["success"];
                ViewBag.message = TempData["message"];
            }
            IList<BkgVehicle> vehicles = _dbContext.BkgVehicles
                .Include(b => b.Customer)
                .Include(b => b.Item)
                .Where(x => x.CompanyId == companyId ).OrderByDescending(o => o.Id).ToList();
            return View(vehicles);
        }

        //[Route("Create")]
        public IActionResult Create(int? id)
        {
            BkgVehicle vehicle = new BkgVehicle();
            ViewBag.receiptTypes = CommonHelper.getAppCompanyConfigLists(_dbContext,"Booking", "Receipt Type", HttpContext.Session.GetInt32("CompanyId").Value);
            ViewBag.bookingType = CommonHelper.getAppCompanyConfigLists(_dbContext,"Booking", "Booking Type", HttpContext.Session.GetInt32("CompanyId").Value);

            ViewBag.EnableEditing = true;
            if (id != null)
            {
                ViewBag.EntityState = "Update";
                vehicle = _dbContext.BkgVehicles.Find(id);

                if (vehicle.Status == "Booking")
                    ViewBag.EnableEditing = true;
                else
                    ViewBag.EnableEditing = false;
                string configValues = _dbContext.AppCompanyConfigs
                            .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                            .Select(c => c.ConfigValue)
                            .FirstOrDefault();
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                ViewBag.ReportPath = string.Concat(configValues, "Viewer", "?Report=Voucher&cId=", companyId, "&id={0}"); return View("Create", vehicle);
            }
            else
            {
                vehicle.BookingDate = DateTime.Now;
                vehicle.Status = "Booking";
                ViewBag.EntityState = "Create";
                return View("Create", vehicle);
            }
        }
        [HttpPost]
        //[Route("Create")]
        public async Task <IActionResult> Create(BkgVehicle vehicle)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId=HttpContext.Session.GetString("UserId");

            ViewBag.EnableEditing = true;
            ViewBag.receiptTypes = CommonHelper.getAppCompanyConfigLists(_dbContext, "Booking", "Receipt Type",companyId);
            ViewBag.bookingType = CommonHelper.getAppCompanyConfigLists(_dbContext, "Booking", "Booking Type", companyId);

            vehicle.Status = "Booking";
            try
            {
                //ModelState.IsValid
                if (vehicle.Status == "Booking")
                {
                    if (vehicle.Id == 0)
                    {

                        vehicle.CompanyId = companyId;
                        vehicle.CreatedBy = userId;
                        vehicle.CreatedDate = DateTime.Now;
                        _dbContext.BkgVehicles.Add(vehicle);
                    }
                    else
                    {
                        if (vehicle.Status == "Booking")
                        {
                            var data = _dbContext.BkgVehicles.Find(vehicle.Id);
                            data.TransDate = vehicle.TransDate;
                            data.ReceiptType = vehicle.ReceiptType;
                            data.BookingType = vehicle.BookingType;
                            data.BankName = vehicle.BankName;
                            data.CustomerId = vehicle.CustomerId;
                            data.ReferenceNo = vehicle.ReferenceNo;
                            data.ItemId = vehicle.ItemId;
                            data.Price = vehicle.Price;
                            data.BankCashAccountId = vehicle.BankCashAccountId;
                            data.ReceivedAmount = vehicle.ReceivedAmount;
                            data.BookingNo = vehicle.BookingNo;
                            data.Remarks = vehicle.Remarks;
                            data.UpdatedBy = userId;
                            data.UpdatedDate = DateTime.Now;
                            var entry = _dbContext.Update(data);
                            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                        }
                    }
                   await _dbContext.SaveChangesAsync();

                    TempData["error"] = "false";
                    TempData["message"] = "The operation completed successfully.";
                    return RedirectToAction("Index");
                }
                else

                    return View("Create", vehicle);
            }
            catch (Exception Exp)
            {

                TempData["error"] = "true";
                TempData["message"] = (Exp.InnerException == null ? Exp.Message : Exp.InnerException.Message);
                return View("Create", vehicle);
            }
        }
        public IActionResult Detail(int id)
        {
           
            if (id != 0)
            {
                //setting report path
                string configValues = _dbContext.AppCompanyConfigs
                                .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                                .Select(c => c.ConfigValue)
                                .FirstOrDefault();
 int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
 ViewBag.ReportPath = string.Concat(configValues, "Viewer", "?Report=Voucher&cId=", companyId, "&id={0}");                //joining data to display in view
                BkgVehicle vehicle = _dbContext.BkgVehicles
                                    .Include(b => b.BankCashAccount)
                                    .Include(i => i.Item)
                                    .Include(c => c.Customer)
                                    .Where(i => i.Id == id).FirstOrDefault();
                vehicle = _dbContext.BkgVehicles.Find(id);
                //joining tables to show data in same view
                IList<BkgVehicle> vehicles = _dbContext.BkgVehicles
                                                                 .Include(b => b.BankCashAccount)
                                                                 .Include(c => c.Customer)
                                                                 .Include(i => i.Item)
                                                                 .Where(i => i.Id == id).ToList();
                IList<BkgReceipt> receipt = _dbContext.BkgReceipts
                                                                 .Where(i => i.VehicleId == id).ToList();
                IList<BkgPayment> payment = _dbContext.BkgPayments
                                                                .Include(b => b.BankCashAccount)
                                                                .Where(i => i.VehicleId == id).ToList();
                IList<BkgVehicleIGP> igp = _dbContext.BkgVehicleIGPs
                                                                .Where(i => i.VehicleId == id).ToList();
                IList<BkgVehicleOGP> ogp = _dbContext.BkgVehicleOGPs
                                                                .Where(i => i.VehicleId == id).ToList();
                IList<BkgCommissionReceived> commissionReceived = _dbContext.BkgCommissionReceiveds
                                                                .Include(b => b.BankCashAccount)
                                                                .Where(i => i.VehicleId == id).ToList();
                IList<BkgCommissionPayment> commissionPayment = _dbContext.BkgCommissionPayments
                                                                .Include(b => b.Account)
                                                                .Where(i => i.VehicleId == id).ToList();
                ViewBag.Vehicle = vehicles;
                ViewBag.Receipt = receipt;
                ViewBag.Payment = payment;
                ViewBag.IGP = igp;
                ViewBag.OGP = ogp;
                ViewBag.CommissionReceived = commissionReceived;
                ViewBag.CommissionPayment = commissionPayment;

                return View(vehicle);
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Record Not Found";
                return RedirectToAction(nameof(Index));
            }
        }
        //public IActionResult _Detail(int id)
        //{
        //    BkgVehicle vehicle = _dbContext.BkgVehicles.Include(b => b.Customer)
        //        .Include(b => b.Item).Where(x => x.Id == id).FirstOrDefault();
        //    return View("VehicleDetail", vehicle);
        //}

        public IActionResult AssignBookingNoList()
        {
            if (TempData["success"] != null)
            {
                ViewBag.success = TempData["success"];
                ViewBag.message = TempData["message"];
            }
            IList<BkgVehicle> vehicles = _dbContext.BkgVehicles
                .Where(w => w.Status == "Pending Booking")
                .Include(b => b.Customer)
                .Include(b => b.Item).Where(x => x.CompanyId == HttpContext.Session.GetInt32("CompanyId")).OrderByDescending(x => x.Id).ToList();
            return View("AssignBookingNoList", vehicles);
        }

        [HttpGet]
        public IActionResult AssignBookingNo(int id)
        {
            BkgVehicle vehicle = new BkgVehicle();

            ViewBag.EntityState = "Update";
            vehicle = _dbContext.BkgVehicles
               .Where(x => x.Status == "Pending Booking" && x.Id == id && x.CompanyId == HttpContext.Session.GetInt32("CompanyId"))
               .Include(b => b.Customer)
               .Include(b => b.Item)
               .FirstOrDefault();
            if (vehicle == null)
            {
                ViewBag.success = false;
                ViewBag.message = ("Couldn't find the Booking number");
                return RedirectToAction("AssignBookingNoList");
            }
            else
            {
                //vehicle.BookingDate = vehicle.BookingDate;
                //vehicle.Price = vehicle.Price;
                //vehicle.PaymentAmount = vehicle.PaymentAmount;
                return View("AssignBookingNo", vehicle);
                //return vehicle;
            }
        }
        [HttpPost]
        public async Task <IActionResult> AssignBookingNo(int id,BkgVehicle vehicle)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            try
            {

                var accountId =(from c in _dbContext.AppCompanyConfigs
                                 join v in _dbContext.BkgVehicles
                                 on c.ConfigValue equals v.BookingType
                                 where v.BookingType == c.ConfigValue && v.Id == id  && c.ConfigName== "Booking Type" 
                                select c.UserValue1).First();
                var bankAccountId = _dbContext.GLBankCashAccounts.Find(vehicle.PaymentBankCashAccountId).AccountId;
                #region "Create Voucher"
                VoucherHelper voucher = new VoucherHelper(_dbContext, HttpContext);
                GLVoucher voucherMaster = new GLVoucher();
                List<GLVoucherDetail> voucherDetails = new List<GLVoucherDetail>();
                var vehicleData = _dbContext.BkgVehicles
                .Where(x => x.Id == vehicle.Id && x.CompanyId == HttpContext.Session.GetInt32("CompanyId"))
                .Include(b => b.Customer)
                .Include(b => b.Item)
                .FirstOrDefault();
                string voucherDescription = string.Format(
                    "Cash/Chq {0}" +
                    "Deposited agt. Booking ID # {1} " +
                    "Booking # {2} " +
                    "Receipt # {3} " +
                    "Model {4} " +
                    "PA {5} " +
                    "of {6} " ,
                    vehicleData.ReceiptType,
                    vehicleData.Id,
                    vehicleData.BookingNo,
                    vehicleData.ReferenceNo,
                    vehicleData.Item.Name,
                    vehicleData.PaymentAdviceNo,
                    vehicleData.Customer.Name);

                voucherMaster.VoucherType = "BKP";
                voucherMaster.VoucherDate = vehicle.BookingDate;
                voucherMaster.Reference = vehicle.ReferenceNo;
                voucherMaster.Currency = "PKR";
                voucherMaster.CurrencyExchangeRate = 1;
                voucherMaster.Description = voucherDescription.Substring(0, voucherDescription.Length);
                voucherMaster.Status = "Approved";
                voucherMaster.IsSystem = true;
                voucherMaster.ModuleName = "Booking Assignment";
                voucherMaster.ModuleId = id;

                //Voucher Details

                //Debit Entry
                GLVoucherDetail voucherDetail = new GLVoucherDetail();
                voucherDetail.AccountId =Convert.ToInt32(accountId) ; 
                voucherDetail.Sequence = 10;
                voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                voucherDetail.Debit = vehicle.PaymentAmount;
                voucherDetail.Credit = 0; 
                voucherDetail.IsDeleted = false;
                voucherDetail.CreatedBy = userId;
                voucherDetail.CreatedDate = DateTime.Now;
                voucherDetails.Add(voucherDetail);


                //Credit Entry
                voucherDetail = new GLVoucherDetail();
                voucherDetail.AccountId = bankAccountId;
                voucherDetail.Sequence = 20;
                voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                voucherDetail.Debit = 0; 
                voucherDetail.Credit = vehicle.PaymentAmount; 
                voucherDetail.IsDeleted = false;
                voucherDetail.CreatedBy = userId;
                voucherDetail.CreatedDate = DateTime.Now;
                voucherDetails.Add(voucherDetail);

                int voucherId = voucher.CreateVoucher(voucherMaster, voucherDetails);
                #endregion

                BkgVehicle savedVehicle = _dbContext.BkgVehicles
                        .Where(w => w.Id == vehicle.Id
                         && w.Id == id).FirstOrDefault();
                //Assign Booking
                savedVehicle.UpdatedBy = userId;
                savedVehicle.UpdatedDate = DateTime.Now;
                savedVehicle.Status = "Booked";
                savedVehicle.PaymentAmount = vehicle.PaymentAmount;
                savedVehicle.BookingNo = vehicle.BookingNo;
                savedVehicle.BookingDate = vehicle.BookingDate;
                savedVehicle.ReferenceNo = vehicle.ReferenceNo;
                savedVehicle.CompanyReceiptNo = vehicle.CompanyReceiptNo;
                savedVehicle.PaymentBankCashAccountId = vehicle.PaymentBankCashAccountId;
                savedVehicle.PaymentVoucherId = voucherId;
                var entry = _dbContext.BkgVehicles.Update(savedVehicle);
                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                await  _dbContext.SaveChangesAsync();

                TempData["error"] = "false";
                TempData["message"] = "Booking information has been saved successfully.";
                return RedirectToAction("AssignBookingNoList");
            }
            catch (Exception Exp)
            {
                TempData["error"] = "true";
                TempData["message"] = (Exp.InnerException == null ? Exp.Message : Exp.InnerException.Message);
                return RedirectToAction("AssignBookingNoList", "Vehicle");
            }

        }
        public IActionResult Approve()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            if (TempData["success"] != null)
            {
                ViewBag.success = TempData["success"];
                ViewBag.message = TempData["message"];
            }
            IList<BkgVehicle> vehicles = _dbContext.BkgVehicles
                .Where(w => w.Status == "Booking")
                .Include(b => b.Customer)
                .Include(b => b.Item).Where(x => x.CompanyId == companyId)
                .OrderByDescending(w => w.Id)
                .ToList();
            return View(vehicles);
        }

        public IActionResult PopUpApproveBooking(int? id)
        {
            BkgVehicle vehicle = new BkgVehicle();

            ViewBag.EntityState = "Update";
            vehicle = _dbContext.BkgVehicles
                .Include(c => c.Customer)
                .Include(i => i.Item)
                .Include(b => b.BankCashAccount)               
                .Where(x => x.Id == id).FirstOrDefault();

            vehicle.CashBankReceiptDate = DateTime.Now;

            return View(vehicle);


        }

        [HttpPost]
        public async Task <IActionResult> PopUpApproveBooking(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            BkgVehicle vehicle = _dbContext.BkgVehicles
                    .Where(v => v.CompanyId == companyId && v.Id == id)
                    .Include(b => b.BankCashAccount)
                    .Include(c => c.Customer)
                    .Include(i => i.Item)
                    .FirstOrDefault();
            
            try
            {
                int voucherId = 0;
                if (!vehicle.BookingType.Contains("Own"))  // If Own Booking then No Voucher.
                {
                    #region "Create Voucher"
                    VoucherHelper voucher = new VoucherHelper(_dbContext, HttpContext,this);
                    GLVoucher voucherMaster = new GLVoucher();
                    List<GLVoucherDetail> voucherDetails = new List<GLVoucherDetail>();
                    
                    var vehicleData = _dbContext.BkgVehicles
                    .Where(x => x.Id == vehicle.Id && x.CompanyId == companyId)
                    .Include(b => b.Customer)
                    .Include(b => b.Item)
                    .FirstOrDefault();
                    string voucherDescription = string.Format(
                        "Funds/Cash Receined agt.Booking ID #  {0} " +
                        "Receipt #  {1} " +
                        "Model  {2} " +
                        "Booking # {3} " +
                        "From {4}" , 
                        vehicleData.Id, 
                        vehicleData.ReferenceNo, 
                        vehicleData.Item.Name, 
                        vehicleData.BookingNo, 
                        vehicleData.Customer.Name);

                    voucherMaster.VoucherType = "BKR";
                    voucherMaster.VoucherDate = vehicle.TransDate;
                    voucherMaster.Reference = vehicle.ReferenceNo;
                    voucherMaster.Currency = "PKR";
                    voucherMaster.CurrencyExchangeRate = 1;

                    voucherMaster.Description = voucherDescription.Substring(0,voucherDescription.Length);
                    voucherMaster.Status = "Approved";
                    voucherMaster.IsSystem = true;
                    voucherMaster.ModuleName = "Booking Voucher";
                    voucherMaster.ModuleId = id;

                    //Voucher Details

                    //Debit Entry
                    GLVoucherDetail voucherDetail = new GLVoucherDetail();
                    voucherDetail.AccountId = vehicle.BankCashAccount.AccountId;
                    voucherDetail.Sequence = 10;
                    voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                    voucherDetail.Debit = vehicle.ReceivedAmount;
                    voucherDetail.Credit = 0;
                    voucherDetail.IsDeleted = false;
                    voucherDetail.CreatedBy = userId;
                    voucherDetail.CreatedDate = DateTime.Now;
                    voucherDetails.Add(voucherDetail);

                    //Credit Entry
                    voucherDetail = new GLVoucherDetail();
                    voucherDetail.AccountId = vehicle.Customer.AccountId;
                    voucherDetail.Sequence = 20;
                    voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                    voucherDetail.Debit = 0;
                    voucherDetail.Credit = vehicle.ReceivedAmount;
                    voucherDetail.IsDeleted = false;
                    voucherDetail.CreatedBy = userId;
                    voucherDetail.CreatedDate = DateTime.Now;
                    voucherDetails.Add(voucherDetail);

                    //Create Voucher 

                    voucherId = voucher.CreateVoucher(voucherMaster, voucherDetails);
                    #endregion
                }
                BkgVehicle savedVehicle = _dbContext.BkgVehicles.Where(v => v.Id == vehicle.Id).FirstOrDefault();
                savedVehicle.UpdatedBy = userId;
                savedVehicle.UpdatedDate = DateTime.Now;
                savedVehicle.VoucherId = voucherId;
                savedVehicle.Status = "Pending Booking";
                var entry = _dbContext.Update(savedVehicle);
                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                await _dbContext.SaveChangesAsync();

                TempData["error"] = "false";
                TempData["message"] = string.Format("Booking # {0} information has been Approved successfully.", id);

                return RedirectToAction(nameof(Approve));
            }
            catch (Exception Exp)
            {
                ViewBag.success = false;
                ViewBag.message = (Exp.InnerException == null ? Exp.Message : Exp.InnerException.Message);
                return View(vehicle);
            }
        }

        public IActionResult CashOrBankReceiptConfirmationList()
        {
            if (TempData["success"] != null)
            {
                ViewBag.success = TempData["success"];
                ViewBag.message = TempData["message"];
            }
            IList<BkgVehicle> vehicles = _dbContext.BkgVehicles
                .Where(w => w.Status == "Booking")
                .Include(b => b.Customer)
                .Include(b => b.Item).Where(x => x.CompanyId == HttpContext.Session.GetInt32("CompanyId")).OrderByDescending(x => x.Id).ToList();

            return View("CashOrBankReceiptConfirmationList", vehicles);
            
        }

        public IActionResult CashOrBankReceiptConfirmation(int? id)
        {
            BkgVehicle vehicle = new BkgVehicle();

            ViewBag.EntityState = "Update";
            vehicle = _dbContext.BkgVehicles.Include(b => b.Customer)
               .Include(b => b.Item).Where(x => x.Id == id).FirstOrDefault();
            vehicle.CashBankReceiptDate = DateTime.Now;
            return View("CashOrBankReceiptConfirmation", vehicle);


        }
        [HttpPost]
        public async Task <IActionResult> CashOrBankReceiptConfirmation(BkgVehicle vehicle)
        {
            try
            {
                string userId = HttpContext.Session.GetString("UserId");
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
                TempData["message"] = "Booking information has been saved successfully.";
                return RedirectToAction("CashOrBankReceiptConfirmationList");
            }
            catch (Exception Exp)
            {
                TempData["error"] = "true";
                TempData["message"] = (Exp.InnerException == null ? Exp.Message : Exp.InnerException.Message);
                return View("CashOrBankReceiptConfirmation", vehicle);
            }
            finally { }
        }
        
    }
}