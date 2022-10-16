using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Numbers.Entity.Models;
using Numbers.Helpers;

namespace Numbers.Areas.Booking.Controllers
{
    [Area("booking")]
    public class AdministrationController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public AdministrationController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ChangeBookingInformation()
        {
            return View();
        }
        public async Task<IActionResult> UpdateBooking(BkgVehicle bkgVehicle)
        {
            //Comparing with Id because Id is having temporary assigned value to get reference of entry.
            BkgVehicle vehicle = _dbContext.BkgVehicles.Where(v => v.BookingNo == bkgVehicle.BankName).FirstOrDefault();
            if (bkgVehicle.BankName != null)
            {
                if (bkgVehicle.BookingNo != vehicle.BookingNo)
                {
                    vehicle.BookingNo = bkgVehicle.BookingNo;
                }
                if (bkgVehicle.PaymentAmount != vehicle.PaymentAmount)
                {
                    vehicle.PaymentAmount = bkgVehicle.PaymentAmount;
                }
                if (bkgVehicle.ReferenceNo != vehicle.ReferenceNo)
                {
                    vehicle.ReferenceNo = bkgVehicle.ReferenceNo;
                }
                if (bkgVehicle.Price != vehicle.Price)
                {
                    vehicle.Price = bkgVehicle.Price;
                }
                if (bkgVehicle.ItemId != vehicle.ItemId)
                {
                    vehicle.ItemId = bkgVehicle.ItemId;
                }
                vehicle.UpdatedBy = HttpContext.Session.GetString("UserId");
                vehicle.UpdatedDate = DateTime.Now;
                var entry = _dbContext.BkgVehicles.Update(vehicle);
                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                TempData["error"] = "false";
                TempData["message"] = string.Format("Booking No {0} has been updated successfully",
                vehicle.BookingNo, bkgVehicle.BookingNo);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(ChangeBookingInformation));
            }

            return RedirectToAction("ChangeBookingInformation");
        }
        public IActionResult ChangeCustomerCNIC()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangeCustomerCNIC(BkgCustomer bkgCustomer)
        {
            BkgCustomer customer = _dbContext.BkgCustomers.Where(c => c.Id == bkgCustomer.Id && c.CompanyId == HttpContext.Session.GetInt32("CompanyId").Value).FirstOrDefault();
            if (bkgCustomer.CNIC != "")
            {
                customer.CNIC = bkgCustomer.CNIC;
                var entry = _dbContext.BkgCustomers.Update(customer);
                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                await _dbContext.SaveChangesAsync();
                TempData["error"] = "false";
                TempData["message"] = "CNIC has been updated successfully";
                return View();
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Can Not Update CNIC";
                return View(bkgCustomer);
            }
        }
        [HttpGet]
        public IActionResult AdvanceBooking(string type)
        {
            if (type == "Sales")
                {
                    ViewBag.Type = "Advance Booking Sales";
                }
            else if (type == "Purchase")
                {
                    ViewBag.Type = "Advance Booking Purchase";
                }
            return View();
        }
        [HttpPost]
        public async Task <IActionResult> AdvanceBooking(BkgAdvanceBooking bkgAdvanceBooking)
        {
            var oldValues = _dbContext.BkgVehicles.Find(bkgAdvanceBooking.Id);
            var advanceBooking = new BkgAdvanceBooking();
            advanceBooking.VehicleId = bkgAdvanceBooking.Id;
            advanceBooking.BookingNo = bkgAdvanceBooking.BookingNo;
            advanceBooking.BookingDate = bkgAdvanceBooking.BookingDate;
            advanceBooking.CustomerId = bkgAdvanceBooking.CustomerId;
            advanceBooking.PaymentAmount = bkgAdvanceBooking.PaymentAmount;
            advanceBooking.Remarks = bkgAdvanceBooking.Remarks;
            advanceBooking.Type = bkgAdvanceBooking.Type;
            advanceBooking.CreatedBy = HttpContext.Session.GetString("UserId");
            advanceBooking.CreatedDate = DateTime.Now;
            //assigning old values
            advanceBooking.OldBookingNo = oldValues.BookingNo;
            advanceBooking.OldCustomerId = oldValues.CustomerId;
            advanceBooking.OldPaymentAmount = oldValues.PaymentAmount;
            _dbContext.BkgAdvanceBookings.Add(advanceBooking);
            await _dbContext.SaveChangesAsync();
            TempData["error"] = "false";
            TempData["message"]="Entry has been saved successfully...";
            return RedirectToAction("Index", "Dashboard");
        }
        public IActionResult CancelBooking()
        {
            return View("CancelBooking");
        }
        [HttpPost]
        public async Task<IActionResult>CancelBooking(BkgVehicle bkgVehicle)
        {
            var vehicle = _dbContext.BkgVehicles.Find(bkgVehicle.Id);
            if (bkgVehicle.Id != 0)
            {
                vehicle.CancelRemarks = bkgVehicle.CancelRemarks;
                vehicle.Status = "Cancel";
                vehicle.CancelledBy = HttpContext.Session.GetString("UserId");
                vehicle.CancelledDate = bkgVehicle.CancelledDate;
                vehicle.CancelVoucherId = CreateCancelBookingVoucher(vehicle);
                var entry = _dbContext.BkgVehicles.Update(vehicle);
                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                await _dbContext.SaveChangesAsync();
                TempData["error"] = "false";
                TempData["message"] = "Bookin has been cencelled successfully.";
                return RedirectToAction("Index", "Dashboard");
            }
            else
            TempData["error"] = "false";
            TempData["message"] = "cancellation is not allowed for this Booking.";
            return View(bkgVehicle);

        }
        public int CreateCancelBookingVoucher(BkgVehicle vehicle)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");

            #region  "Create Voucher"
            VoucherHelper voucher = new VoucherHelper(_dbContext, HttpContext);
            GLVoucher voucherMaster = new GLVoucher();
            List<GLVoucherDetail> voucherDetails = new List<GLVoucherDetail>();
            var bankAccountId = _dbContext.GLBankCashAccounts
                              .Where(c => c.Id == vehicle.Customer.AccountId)
                              .Select(a => a.AccountId);


            voucherMaster.VoucherType = "CB-V";
            voucherMaster.VoucherDate = vehicle.TransDate;
            //voucherMaster.Reference = cashPurchaseSale.Reference;
            voucherMaster.Currency = "PKR";
            voucherMaster.CurrencyExchangeRate = 1;
            voucherMaster.Description = vehicle.CancelRemarks;
            //voucherMaster.Status = "Approved";
            //voucherMaster.IsSystem = true;
            voucherMaster.ModuleName = "Cancel Booking";
            voucherMaster.ModuleId = vehicle.Id;

            //Voucher Details

            //Debit Entry
            GLVoucherDetail voucherDetail = new GLVoucherDetail();
            voucherDetail.AccountId = vehicle.CustomerId;
            voucherDetail.Sequence = 10;
            voucherDetail.Description = vehicle.CancelRemarks;
            voucherDetail.Debit = 0;
            voucherDetail.Credit = vehicle.PaymentAmount;
            voucherDetail.IsDeleted = false;
            voucherDetail.CreatedBy = userId;
            voucherDetail.CreatedDate = DateTime.Now;
            voucherDetails.Add(voucherDetail);

            //Credit Entry
            voucherDetail = new GLVoucherDetail();
            voucherDetail.AccountId = vehicle.CustomerId;
            voucherDetail.Sequence = 20;
            voucherDetail.Description = vehicle.CancelRemarks;
            voucherDetail.Debit = vehicle.PaymentAmount;
            voucherDetail.Credit = 0;
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
        public IActionResult UnPost(string name)
        {
            if (name == "IGP")
                {
                    ViewBag.Type = "IGP";
                }
            else if (name=="OGP")
                {
                    ViewBag.Type = "OGP";
                }
                return View();
        }
        [HttpPost]
        public async Task<IActionResult> UnPost(BkgVehicleIGP bkgVehicleIGP)
        {
            if (bkgVehicleIGP.Id != 0)
            {
                if (bkgVehicleIGP.Description.Contains("IGP"))
                {
                    var igp = _dbContext.BkgVehicleIGPs.Find(bkgVehicleIGP.Id);

                    TempData["error"] = "false";
                    TempData["message"] =string.Format("IGP having Id {0} has been Un-Posted",igp.Id);
                    return RedirectToAction("Index", "IGP");

                }
                else if (bkgVehicleIGP.Description.Contains("OGP"))
                {
                    var ogp = _dbContext.BkgVehicleOGPs.Find(bkgVehicleIGP.Id);

                    _dbContext.Update(ogp);
                    await _dbContext.SaveChangesAsync();

                    TempData["error"] = "false";
                    TempData["message"] =string.Format("OGP having Id {0} has been Un-Posted", ogp.Id);
                    return RedirectToAction("Index", "OGP");
                }
                return RedirectToAction("Index", "IGP");
            }
            else
            {
                TempData["error"] = "true";
               TempData["message"] = "No Record Found.";
                return RedirectToAction("Index","Dashboard");
            }
        }
    }

}