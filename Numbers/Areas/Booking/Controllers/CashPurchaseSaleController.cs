using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

    public class CashPurchaseSaleController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        //string authRoles = "";// CommonHelper.getAuthorizeRoles("CashPurchaseSaleController", "Edit", "", 0);
        public CashPurchaseSaleController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;                        
        }

        public IActionResult Index()
        {
           int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
           string  userId = HttpContext.Session.GetString("UserId");
           IList <BkgCashPurchaseSale>cashPurchaseSale = _dbContext.BkgCashPurchaseSales
                .Include(c => c.Customer)
                .Include(i => i.Item)
                //.Include(s => s.SalePayment)
                //.Include(s => s.SaleReceipt)
                .Where(x =>  x.CompanyId == companyId).OrderByDescending(o => o.Id).ToList();
            return View(cashPurchaseSale);
        }
        [HttpGet]
        public IActionResult Create(int? id)
        {
            BkgCashPurchaseSale cashPurchaseSale = new BkgCashPurchaseSale();
            if (id != null)
            {
                ViewBag.EntityState = "Update";
                var dataIndex = _dbContext.BkgCashPurchaseSales.Find(id);
                return View(dataIndex);

            }
            else
            {
                cashPurchaseSale.TransDate = DateTime.Now;
                cashPurchaseSale.BookingDate = DateTime.Now;
                cashPurchaseSale.IGPDate = DateTime.Now;
                cashPurchaseSale.OGPDate = DateTime.Now;
                cashPurchaseSale.PurchaseDate = DateTime.Now;
                cashPurchaseSale.SalesDate = DateTime.Now;
                ViewBag.EntityState = "Create";
                return View("Create", cashPurchaseSale);
            }
        }

        [HttpPost]
        public async Task <IActionResult> Create(BkgCashPurchaseSale cashPurchaseSale)
        {
           int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
           string userId = HttpContext.Session.GetString("UserId");
            try
            {
                //bkgCashPurchaseSale = BookingHelper.updateBkgCashPurchaseSaleModel(bkgCashPurchaseSaleViewModel);
                if (ModelState.IsValid)
                {
                    if (cashPurchaseSale.Id == 0)
                    {
                        //cashPurchaseSale.VoucherId = CreateCashPurchaseSaleVoucher(cashPurchaseSale) ;
                        //basic data
                        cashPurchaseSale.CompanyId = companyId;
                        cashPurchaseSale.CreatedBy = userId;
                        cashPurchaseSale.Status = "Created";
                        cashPurchaseSale.CreatedDate = DateTime.Now;
                        _dbContext.BkgCashPurchaseSales.Add(cashPurchaseSale);
                        TempData["error"] = "false";
                        TempData["message"] = "Entry has been saved succsessfully...!";
                    }
                    else
                    {
                        var updateIndex = _dbContext.BkgCashPurchaseSales.Find(cashPurchaseSale.Id);
                        updateIndex.TransDate = cashPurchaseSale.TransDate;
                        updateIndex.ItemId = cashPurchaseSale.ItemId;
                        updateIndex.BookingDate = cashPurchaseSale.BookingDate;
                        updateIndex.CustomerId = cashPurchaseSale.CustomerId;
                        updateIndex.BookingNo = cashPurchaseSale.BookingNo;
                        updateIndex.EstimatedPurchase = cashPurchaseSale.EstimatedPurchase;
                        updateIndex.BookingRemarks = cashPurchaseSale.BookingRemarks;
                        updateIndex.IGPDate = cashPurchaseSale.IGPDate;
                        updateIndex.IGPNo = cashPurchaseSale.IGPNo;
                        updateIndex.EngineNo = cashPurchaseSale.EngineNo;
                        updateIndex.ChassisNo = cashPurchaseSale.ChassisNo;
                        updateIndex.InvoiceNo = cashPurchaseSale.InvoiceNo;
                        updateIndex.ReceivingRemarks = cashPurchaseSale.ReceivingRemarks;
                        updateIndex.UpdatedBy = userId;
                        updateIndex.UpdatedDate = DateTime.Now;
                        var entry = _dbContext.BkgCashPurchaseSales.Update(updateIndex);
                        entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                        await _dbContext.SaveChangesAsync();
                        TempData["error"] = "false";
                        TempData["message"] = "Entry has been updated succsessfully...!";
                    }
                    await _dbContext.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.EntityState = "Create";
                    return View("Create", cashPurchaseSale);
                }
            }

            catch (Exception Exp)
            {

                TempData["error"] = "true";
                TempData["message"] = (Exp.InnerException == null ? Exp.Message : Exp.InnerException.Message);
                return View("Create", cashPurchaseSale);
            }
        }
        public async Task<IActionResult>Approve(int id)
        {
            BkgCashPurchaseSale voucher = _dbContext.BkgCashPurchaseSales.Where(v => v.Id == id).Include(i => i.Item).Include(c => c.Customer).Include(v => v.Vehicle).FirstOrDefault();
            voucher.Status = "Approved";
            voucher.VoucherId = CreateCashPurchaseSaleVoucher(voucher);
            var entry = _dbContext.BkgCashPurchaseSales.Update(voucher);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            TempData["error"] = "false";
            TempData["message"] = "Estimated Purchase has been approved successfully";
            return RedirectToAction(nameof(Index));
        }

        //[Authorize(Roles= authRoles)]
        //On creating entry in cashPurchaseSale Voucher following code segment creates voucher for 'Estimated Purchase'.
        public int CreateCashPurchaseSaleVoucher(BkgCashPurchaseSale cashPurchaseSale)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");

            #region  "Create Voucher"
            VoucherHelper voucher = new VoucherHelper(_dbContext, HttpContext);
            GLVoucher voucherMaster = new GLVoucher();
            List<GLVoucherDetail> voucherDetails = new List<GLVoucherDetail>();
            var bankAccountId = _dbContext.GLBankCashAccounts
                              .Where(c => c.Id == cashPurchaseSale.Customer.AccountId)
                              .Select(a => a.AccountId);

            string voucherDescription = string.Format(
                    "Credit/Cash P/O Model {0} " +
                    "@ {1} Est. agt. " +
                    "IGP # {2}" +
                    "PA # {3} " +
                    "S.P ID #{4} " +
                    "Booking # {5} " +
                    "Chassis # {6} " +
                    "Engine # {7} " +
                    "Inv # {8} " +
                    "From {9} " +
                    "RTP # {10} " ,
                    cashPurchaseSale.Item.Name,                       //0
                    cashPurchaseSale.EstimatedPurchase,                                       //1
                    cashPurchaseSale.IGPNo,                                  //2
                    cashPurchaseSale.Vehicle.PaymentAdviceNo,                         //3
                    cashPurchaseSale.Vehicle.BookingNo,                          //4
                    cashPurchaseSale.BookingNo,                          //5
                    cashPurchaseSale.ChassisNo,                           //6
                    cashPurchaseSale.EngineNo,                                    //7
                    cashPurchaseSale.InvoiceNo,                       //8
                    cashPurchaseSale.Customer.Name,                        //9
                    cashPurchaseSale.Id);                        //10


            voucherMaster.VoucherType = "CP-V";
            voucherMaster.VoucherDate = cashPurchaseSale.TransDate;
            //voucherMaster.Reference = cashPurchaseSale.Reference;
            voucherMaster.Currency = "PKR";
            voucherMaster.CurrencyExchangeRate = 1;
            voucherMaster.Description = voucherDescription;
            //voucherMaster.Status = "Approved";
            //voucherMaster.IsSystem = true;
            voucherMaster.ModuleName = "Booking Cash Purchase Sale";
            voucherMaster.ModuleId = cashPurchaseSale.Id;

            //Voucher Details

            //Debit Entry
            GLVoucherDetail voucherDetail = new GLVoucherDetail();
            voucherDetail.AccountId = cashPurchaseSale.Item.StockAccountId;
            voucherDetail.Sequence = 10;
            voucherDetail.Description = voucherDescription;
            voucherDetail.Debit = cashPurchaseSale.EstimatedPurchase;
            voucherDetail.Credit = 0;
            voucherDetail.IsDeleted = false;
            voucherDetail.CreatedBy = userId;
            voucherDetail.CreatedDate = DateTime.Now;
            voucherDetails.Add(voucherDetail);

            //Credit Entry
            voucherDetail = new GLVoucherDetail();
            voucherDetail.AccountId = cashPurchaseSale.Item.CustomerStockAccountId;
            voucherDetail.Sequence = 20;
            voucherDetail.Description = voucherDescription;
            voucherDetail.Debit = 0;
            voucherDetail.Credit = cashPurchaseSale.EstimatedPurchase;
            voucherDetail.IsDeleted = false;
            voucherDetail.CreatedBy = userId;
            voucherDetail.CreatedDate = DateTime.Now;
            voucherDetails.Add(voucherDetail);

            //Create Voucher
            int voucherId = 0;
            voucherId = voucher.CreateVoucher(voucherMaster, voucherDetails);
            return voucherId;
            #endregion
        }
        [HttpGet]
        public IActionResult Edit(int? id)
        {

            BkgCashPurchaseSaleViewModel cashPurchaseSaleViewModel = new BkgCashPurchaseSaleViewModel();

            ViewBag.EntityState = "Update";
            cashPurchaseSaleViewModel = BookingHelper.getBkgCashPurchaseSaleViewModel(_dbContext, id.Value);

            if (cashPurchaseSaleViewModel.PurchasePayments.Count > 0)
                cashPurchaseSaleViewModel.SavePurchaseInformation = true;
            if (cashPurchaseSaleViewModel.SaleReceipts.Count > 0)
                cashPurchaseSaleViewModel.SaveSalesInformation = true;
            string configValues = _dbContext.AppCompanyConfigs
                    .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                    .Select(c => c.ConfigValue)
                    .FirstOrDefault();
            ViewBag.ReportPath = string.Concat(string.Concat(configValues, "Viewer"), "?Report=Voucher&id={0}");

            return View("Edit", cashPurchaseSaleViewModel);
        }
        [HttpPost]
        public IActionResult Update(BkgCashPurchaseSaleViewModel cashPurchaseSale)
        {
            string userId = HttpContext.Session.GetString("UserId");
            try
            {
                if (ModelState.IsValid)
                {
                    
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Model State Not Valid";
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception Exp)
            {
                TempData["error"] = "true";
                TempData["message"] = (Exp.InnerException == null ? Exp.Message : Exp.InnerException.Message);
                return View("Edit", cashPurchaseSale);
            }
        }
        //[HttpPost]
        //public IActionResult Edit(BkgCashPurchaseSaleViewModel cashPurchaseSaleViewModel)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            bool success = false;
        //            if (cashPurchaseSaleViewModel.SavePurchaseInformation)
        //                success = BookingHelper.saveBkgCashPurchasePurchaseInformation(_dbContext, cashPurchaseSaleViewModel,
        //                    HttpContext.Session.GetInt32("ActiveUserId").Value, HttpContext.Session.GetInt32("ActiveCompanyId").Value);
        //            if (cashPurchaseSaleViewModel.SaveSalesInformation)
        //                success = BookingHelper.saveBkgCashPurchaseSaleInformation(_dbContext, cashPurchaseSaleViewModel,
        //                    HttpContext.Session.GetInt32("ActiveUserId").Value, HttpContext.Session.GetInt32("ActiveCompanyId").Value);

        //            if (success)
        //            {
        //                _dbContext.SaveChanges();

        //                TempData["error"] = "false";
        //                TempData["message"] = "Entry has been saved successfully.";
        //            }
        //            else
        //            {
        //                TempData["error"] = "true";
        //                TempData["message"] = "Data not saved please contact to administrator.";
        //            }

        //            return RedirectToAction("Edit", cashPurchaseSaleViewModel);
        //        }
        //        else
        //            return View("Edit", cashPurchaseSaleViewModel);
        //    }
        //    catch (Exception Exp)
        //    {

        //        TempData["error"] = "true";
        //        TempData["message"] = (Exp.InnerException == null ? Exp.Message : Exp.InnerException.Message);
        //        return View("Edit", cashPurchaseSaleViewModel);
        //    }
        //}

        [HttpPost]
        public async Task <IActionResult> savePurchaseInformation(BkgCashPurchaseSaleViewModel cashPurchaseSaleViewModel)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            string configValues = _dbContext.AppCompanyConfigs
                .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                .Select(c => c.ConfigValue)
                .FirstOrDefault();
            ViewBag.ReportPath = string.Concat(string.Concat(configValues, "Viewer"), "?Report=Voucher&id={0}");
            try
            {

                bool success = false;

                success = BookingHelper.SaveBkgCashPurchasePurchaseInformation(_dbContext, cashPurchaseSaleViewModel, userId, companyId);

                if (success)
                {
                    //selecting index to be updated when purchase voucher is created
                    var purchaseVoucher = _dbContext.BkgCashPurchaseSales.Find(cashPurchaseSaleViewModel.Id);
                    // Calling Voucher Function
                    purchaseVoucher.PurchaseVoucherId = CreateCashPurchaseVoucher(cashPurchaseSaleViewModel);
                    _dbContext.BkgCashPurchaseSales.Update(purchaseVoucher);
                    await _dbContext.SaveChangesAsync();

                    TempData["error"] = "false";
                    TempData["message"] = "Entry has been saved successfully.";
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Data not saved please contact to administrator.";
                }
                return RedirectToAction(nameof(Edit), BookingHelper.getBkgCashPurchaseSaleViewModel(_dbContext, cashPurchaseSaleViewModel.Id));

            }
            catch (Exception Exp)
            {
                TempData["error"] = "true";
                TempData["message"] = (Exp.InnerException == null ? Exp.Message : Exp.InnerException.Message);
                return View("Edit", BookingHelper.getBkgCashPurchaseSaleViewModel(_dbContext, cashPurchaseSaleViewModel.Id));
            }

        }
        //On entering Purchase Information in bookingCashPurchaseSale Following segment creates voucher for 'Purchase Voucher'
        public int CreateCashPurchaseVoucher(BkgCashPurchaseSaleViewModel cashPurchase)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");

            BkgCashPurchaseSale cps = _dbContext.BkgCashPurchaseSales
                     .Include(i => i.Customer)
                     .Include(c => c.Item)
                     .Include(v=>v.Vehicle)
                     .Where(a => a.CompanyId == companyId && a.Id == cashPurchase.Id)
                     .FirstOrDefault();

            #region  "Create Voucher"
            VoucherHelper voucher = new VoucherHelper(_dbContext, HttpContext);
            GLVoucher voucherMaster = new GLVoucher();
            List<GLVoucherDetail> voucherDetails = new List<GLVoucherDetail>();
            string voucherDescription = string.Format(
                    "Actual purchase of MF-240 from {0} " +
                    "@ {1}/  " +
                    "Est. rate @ {2}/ " +
                    "Booking # {3} " +
                    "Chassis #{4} " +
                    "Engine # {5} " +
                    "Inv. # {6} " +
                    "PA # {7} " +
                    "agt. IGP # {8} " +
                    "Trans. ID # {9} " ,
                    cps.Customer.Name,                                               //0
                    cashPurchase.EstimatedPurchase + cashPurchase.PurchaseRate,       //1
                    cashPurchase.EstimatedPurchase,                                   //2
                    cashPurchase.BookingNo,                                           //3
                    cashPurchase.ChassisNo,                                           //4
                    cashPurchase.EngineNo,                                            //5
                    cashPurchase.InvoiceNo,                                           //6
                    cps.Vehicle.PaymentAdviceNo,                                      //7
                    cashPurchase.IGPNo,                                               //8
                    cashPurchase.Id);                                                 //9


            voucherMaster.VoucherType = "CP-P";
            voucherMaster.VoucherDate = cashPurchase.PurchaseDate;
            voucherMaster.Reference = cashPurchase.PurchaseParty;
            voucherMaster.Currency = "PKR";
            voucherMaster.CurrencyExchangeRate = 1;
            voucherMaster.Description = voucherDescription;
            voucherMaster.Status = "Created";
            //voucherMaster.IsSystem = true;
            voucherMaster.ModuleName = "Booking Cash Purchase Sale";
            voucherMaster.ModuleId = cashPurchase.Id;

            //Voucher Details

            //Debit Entry
            GLVoucherDetail voucherDetail = new GLVoucherDetail();

            voucherDetail.AccountId = cps.Item.CostofSaleAccountId;
            voucherDetail.Sequence = 10;
            voucherDetail.Description = voucherDescription;
            voucherDetail.Debit = cashPurchase.PurchaseRate;
            voucherDetail.Credit = 0;
            voucherDetail.IsDeleted = false;
            voucherDetail.CreatedBy = userId;
            voucherDetail.CreatedDate = DateTime.Now;
            voucherDetails.Add(voucherDetail);

            //Credit Entry
            voucherDetail = new GLVoucherDetail();
            //var accountId = (from c in _dbContext.AppCompanyConfigs
            //                 where c.ConfigName == "Booking Type" && c.ConfigValue == cps.Vehicle.BookingType
            //                 select c.UserValue1).First();
            voucherDetail.AccountId = cps.Customer.AccountId;
            voucherDetail.Sequence = 20;
            voucherDetail.Description = voucherDescription;
            voucherDetail.Debit = 0;
            voucherDetail.Credit = cashPurchase.PurchaseRate;
            voucherDetail.IsDeleted = false;
            voucherDetail.CreatedBy = userId;
            voucherDetail.CreatedDate = DateTime.Now;
            voucherDetails.Add(voucherDetail);

            //Create Voucher
            int voucherId = 0;
            voucherId = voucher.CreateVoucher(voucherMaster, voucherDetails);
            return voucherId;
            #endregion
        }
        [HttpPost]
        public async Task <IActionResult> saveSaleInformation(BkgCashPurchaseSaleViewModel cashPurchaseSaleViewModel)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            try
            {

                bool success = false;

                success = BookingHelper.saveBkgCashPurchaseSaleInformation(_dbContext, cashPurchaseSaleViewModel, userId, companyId);

                if (success)
                {
                   //selecting index to be updated when sales voucher is created
                    var salesVoucher = _dbContext.BkgCashPurchaseSales.Find(cashPurchaseSaleViewModel.Id);
                    //Calling Voucher function
                    salesVoucher.SalesVoucherId = CreateCashSalesVoucher(cashPurchaseSaleViewModel);
                    _dbContext.BkgCashPurchaseSales.Update(salesVoucher);
                    await _dbContext.SaveChangesAsync();

                    TempData["error"] = "false";
                    TempData["message"] = "Entry has been saved successfully.";
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Data not saved please contact to administrator.";
                }

                return RedirectToAction("Edit", BookingHelper.getBkgCashPurchaseSaleViewModel(_dbContext, cashPurchaseSaleViewModel.Id));
            }
            catch (Exception Exp)
            {

                TempData["error"] = "true";
                TempData["message"] = (Exp.InnerException == null ? Exp.Message : Exp.InnerException.Message);
                return View("Edit", BookingHelper.getBkgCashPurchaseSaleViewModel(_dbContext, cashPurchaseSaleViewModel.Id));
            }
        }
        public int CreateCashSalesVoucher(BkgCashPurchaseSaleViewModel cashSales)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");

            #region  "Create Voucher"
            VoucherHelper voucher = new VoucherHelper(_dbContext, HttpContext);
            GLVoucher voucherMaster = new GLVoucher();
            List<GLVoucherDetail> voucherDetails = new List<GLVoucherDetail>();


            voucherMaster.VoucherType = "CP-S";
            voucherMaster.VoucherDate = cashSales.SalesDate;
            voucherMaster.Reference = cashSales.SalesParty;
            voucherMaster.Currency = "PKR";
            voucherMaster.CurrencyExchangeRate = 1;
            voucherMaster.Description = cashSales.SalesRemarks;
            voucherMaster.Status = "Created";
            //voucherMaster.IsSystem = true;
            voucherMaster.ModuleName = "Booking Cash Purchase Sale";
            voucherMaster.ModuleId = cashSales.Id;

            //Voucher Details

            //Debit Entry
            GLVoucherDetail voucherDetail = new GLVoucherDetail();
            voucherDetail.AccountId = cashSales.SalesPartyAccount;
            voucherDetail.Sequence = 10;
            voucherDetail.Description = cashSales.SalesRemarks;
            voucherDetail.Debit = cashSales.SalesRate;
            voucherDetail.Credit = 0;
            voucherDetail.IsDeleted = false;
            voucherDetail.CreatedBy = userId;
            voucherDetail.CreatedDate = DateTime.Now;
            voucherDetails.Add(voucherDetail);

            //Credit Entry
            voucherDetail = new GLVoucherDetail();
            voucherDetail.AccountId = cashSales.SalesPartyAccount;
            voucherDetail.Sequence = 20;
            voucherDetail.Description = cashSales.SalesRemarks;
            voucherDetail.Debit = 0;
            voucherDetail.Credit = cashSales.SalesRate;
            voucherDetail.IsDeleted = false;
            voucherDetail.CreatedBy = userId;
            voucherDetail.CreatedDate = DateTime.Now;
            voucherDetails.Add(voucherDetail);

            //Create Voucher
            int voucherId = 0;
            voucherId = voucher.CreateVoucher(voucherMaster, voucherDetails);
            return voucherId;
            #endregion
        }

        [HttpPost]
        public async Task <IActionResult> savePayment(BkgCashPurchaseSaleViewModel cashPurchaseSaleViewModel)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            try
            {
                if (BookingHelper.checkPurchaseInformationSaved(_dbContext, cashPurchaseSaleViewModel.Id))
                {
                    bool success = false;

                    success = BookingHelper.saveCashPurchaseSalePayment(_dbContext,HttpContext, cashPurchaseSaleViewModel, userId, companyId);

                    if (success)
                    {
                        //its voucher is in Voucher helper
                        await _dbContext.SaveChangesAsync();

                        TempData["error"] = "false";
                        TempData["message"] = "Entry has been saved successfully.";
                    }
                    else
                    {
                        TempData["error"] = "true";
                        TempData["message"] = "Data not saved please contact to administrator.";
                    }

                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Purchase information not saved.";
                }

                return RedirectToAction("Edit", BookingHelper.getBkgCashPurchaseSaleViewModel(_dbContext, cashPurchaseSaleViewModel.Id));
            }
            catch (Exception Exp)
            {

                TempData["error"] = "true";
                TempData["message"] = (Exp.InnerException == null ? Exp.Message : Exp.InnerException.Message);
                return View("Edit", BookingHelper.getBkgCashPurchaseSaleViewModel(_dbContext, cashPurchaseSaleViewModel.Id));
            }
        }

        [HttpPost]
        public async Task <IActionResult> saveReceipt(BkgCashPurchaseSaleViewModel cashPurchaseSaleViewModel)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            try
            {
                if (BookingHelper.checkSalesInformationSaved(_dbContext, cashPurchaseSaleViewModel.Id))
                {

                    bool success = false;

                    success = BookingHelper.saveCashPurchaseSaleReceipt(_dbContext,HttpContext, cashPurchaseSaleViewModel, userId, companyId);

                    if (success)
                    {
                        // its Voucher is in Voucher Helper
                        await _dbContext.SaveChangesAsync();

                        TempData["error"] = "false";
                        TempData["message"] = "Entry has been saved successfully.";
                    }
                    else
                    {
                        TempData["error"] = "true";
                        TempData["message"] = "Data not saved please contact to administrator.";
                    }
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Sale information not saved.";
                }
                return RedirectToAction("Edit", BookingHelper.getBkgCashPurchaseSaleViewModel(_dbContext, cashPurchaseSaleViewModel.Id));
            }
            catch (Exception Exp)
            {

                TempData["error"] = "true";
                TempData["message"] = (Exp.InnerException == null ? Exp.Message : Exp.InnerException.Message);
                return View("Edit", BookingHelper.getBkgCashPurchaseSaleViewModel(_dbContext, cashPurchaseSaleViewModel.Id));
            }
        }
        public JsonResult CheckBookingNoAlreadyExists(string bookingNo)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
           // System.Threading.Thread.Sleep(200);
            if (bookingNo.Length == 0)
                return Json(0);
            var returnVal = _dbContext.BkgVehicles
                                    .Where(a => a.CompanyId == companyId && a.BookingNo == bookingNo).FirstOrDefault();
            return Json(returnVal);
        }
        public JsonResult CheckIgp(int bookingNo)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            // System.Threading.Thread.Sleep(200);
            var returnVal = _dbContext.BkgVehicleIGPs.Where
                (a => a.CompanyId == companyId && a.VehicleId == bookingNo).FirstOrDefault();
            return Json(returnVal);
        }
    }
}