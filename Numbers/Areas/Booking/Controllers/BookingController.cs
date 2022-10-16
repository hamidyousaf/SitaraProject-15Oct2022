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
//using Numbers.Models;
//using Numbers.ViewModels;

namespace Numbers.Controllers
{
    [Authorize]
    [Area("Booking")]
    //[Route("Booking/Customer")]
    public class BookingController : Controller
    {

        UserManager<ApplicationUser> _userManager;
        private readonly NumbersDbContext _dbContext;


        public BookingController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;

        }

        #region "Customer"

        //public IActionResult CustomerList()
        //{
        //    if (TempData["success"] != null)
        //    {
        //        ViewBag.success = TempData["success"];
        //        ViewBag.message = TempData["message"];
        //    }

        //    IList<BkgCustomer> bkgCustomers = _dbContext.BkgCustomers
        //        .Include(b => b.Account).Where(x => x.CompanyId == Utility.ActiveCompanyId).OrderByDescending(o => o.Id).ToList();

        //    return View("/Views/Booking/Customer/CustomerList", bkgCustomers);
        //}
        //public IActionResult CustomerCreate(int? id)
        //{
        //    BkgCustomer bkgCustomer = new BkgCustomer();
        //    if (id != null)
        //    {
        //        ViewBag.EntityState = "Update";
        //        bkgCustomer = _dbContext.BkgCustomers.Find(id);
        //        return View("Customer/CustomerCreate", bkgCustomer);

        //    }
        //    else
        //    {
        //        ViewBag.EntityState = "Create";
        //        bkgCustomer.IsActive = true;

        //        return View("Customer/CustomerCreate", bkgCustomer);
        //    }
        //}

        //[HttpPost]
        //public IActionResult CustomerCreate(BkgCustomer bkgCustomer)
        //{
        //    try
        //    {
        //        BkgCustomer bkgCustomerViewModel = new BkgCustomer();

        //        if (ModelState.IsValid)
        //        {
        //            if (bkgCustomer.Id == 0)
        //            {
        //                bkgCustomer.CompanyId = Utility.ActiveCompanyId;
        //                bkgCustomer.CreatedBy = Utility.ActiveUserId;
        //                bkgCustomer.CreatedDate = DateTime.Now;
        //                _dbContext.BkgCustomers.Add(bkgCustomer);
        //            }
        //            else
        //            {

        //                _dbContext.Entry(bkgCustomer).State = EntityState.Modified;
        //                _dbContext.Entry(bkgCustomer).Property("CompanyId").IsModified = false;
        //                _dbContext.Entry(bkgCustomer).Property("CreatedBy").IsModified = false;
        //                _dbContext.Entry(bkgCustomer).Property("CreatedDate").IsModified = false;
        //                bkgCustomer.UpdatedBy = Utility.ActiveUserId;
        //                bkgCustomer.UpdatedDate = DateTime.Now;

        //            }
        //            _dbContext.SaveChanges();

        //            TempData["success"] = true;
        //            TempData["message"] = "The operation completed successfully.";
        //            return RedirectToAction("CustomerList");
        //        }
        //        else
        //        {
        //            if (bkgCustomer.Id != 0)
        //            {
        //                ViewBag.EntityState = "Update";
        //                bkgCustomer = _dbContext.BkgCustomers.Find(bkgCustomer.Id);
        //                return View("Customer/CustomerCreate", bkgCustomer);
        //            }
        //            else
        //            {
        //                ViewBag.EntityState = "Create";
        //                bkgCustomer.IsActive = true;
        //                return View("Customer/CustomerCreate", bkgCustomer);
        //            }
        //        }
        //        //return View("Customer/CustomerCreate", bkgCustomer);
        //    }
        //    catch (Exception Exp)
        //    {
        //        ViewBag.success = false;
        //        ViewBag.message = (Exp.InnerException == null ? Exp.Message : Exp.InnerException.Message);

        //        return View("Customer/CustomerCreate", bkgCustomer);
        //    }
        //}

        //public IActionResult CustomerDetail(int id)
        //{
        //    BkgCustomer bkgCustomer = _dbContext.BkgCustomers
        //        .Include(b => b.Account).Where(x => x.Id == id).FirstOrDefault();
        //    return View("Customer/CustomerDetail", bkgCustomer);
        //}

        #endregion

        #region "Vehicle"

        //public IActionResult VehicleList()
        //{
        //    if (TempData["success"] != null)
        //    {
        //        ViewBag.success = TempData["success"];
        //        ViewBag.message = TempData["message"];
        //    }
        //    IList<BkgVehicle> BkgVehicles = _dbContext.BkgVehicles
        //        .Include(b => b.Customer)
        //        .Include(b => b.Item).Where(x => x.CompanyId == Utility.ActiveCompanyId).OrderByDescending(o=>o.Id).ToList();
        //    return View("Vehicle/VehicleList", BkgVehicles);
        //}
        //public IActionResult VehicleCreate(int? id)
        //{
        //    BkgVehicle BkgVehicle = new BkgVehicle();
        //    if (id != null)
        //    {
        //        ViewBag.EntityState = "Update";
        //        BkgVehicle = _dbContext.BkgVehicles.Find(id);
        //        return View("Vehicle/VehicleCreate", BkgVehicle);

        //    }
        //    else
        //    {
        //        BkgVehicle.BookingDate = DateTime.Now;

        //        ViewBag.EntityState = "Create";
        //        return View("Vehicle/VehicleCreate", BkgVehicle);
        //    }
        //}
        //[HttpPost]
        //public IActionResult VehicleCreate(BkgVehicle BkgVehicle)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            if (BkgVehicle.Id == 0)
        //            {
        //                BkgVehicle.Status = "Booking";
        //                BkgVehicle.CompanyId = Utility.ActiveCompanyId;
        //                BkgVehicle.CreatedBy = Utility.ActiveUserId;
        //                BkgVehicle.CreatedDate = DateTime.Now;
        //                _dbContext.BkgVehicles.Add(BkgVehicle);
        //            }
        //            else
        //            {

        //                _dbContext.Entry(BkgVehicle).State = EntityState.Modified;
        //                _dbContext.Entry(BkgVehicle).Property("CompanyId").IsModified = false;
        //                _dbContext.Entry(BkgVehicle).Property("CreatedBy").IsModified = false;
        //                _dbContext.Entry(BkgVehicle).Property("CreatedDate").IsModified = false;
        //                _dbContext.Entry(BkgVehicle).Property("Status").IsModified = false;

        //                //BkgVehicle.CompanyId = Utility.ActiveCompanyId;
        //                BkgVehicle.UpdatedBy = Utility.ActiveUserId;
        //                BkgVehicle.UpdatedDate = DateTime.Now;
        //            }
        //            _dbContext.SaveChanges();

        //            TempData["success"] = true;
        //            TempData["message"] = "The operation completed successfully.";
        //            return RedirectToAction("VehicleList");
        //        }
        //        else

        //            return View("Vehicle/VehicleCreate", BkgVehicle);
        //    }
        //    catch (Exception Exp)
        //    {
        //        ViewBag.success = false;
        //        ViewBag.message = (Exp.InnerException == null ? Exp.Message : Exp.InnerException.Message);
        //        return View("Vehicle/VehicleCreate", BkgVehicle);
        //    }
        //}
        //public IActionResult VehicleDetail(int id)
        //{
        //    BkgVehicle BkgVehicle = _dbContext.BkgVehicles.Include(b => b.Customer)
        //        .Include(b => b.Item).Where(x => x.Id == id).FirstOrDefault();
        //    return View("Vehicle/VehicleDetail", BkgVehicle);
        //}

        //public IActionResult AssignBookingNoList()
        //{
        //    if (TempData["success"] != null)
        //    {
        //        ViewBag.success = TempData["success"];
        //        ViewBag.message = TempData["message"];
        //    }
        //    IList<BkgVehicle> BkgVehicles = _dbContext.BkgVehicles
        //        .Where(w => w.Status == "PendingBooking")
        //        .Include(b => b.Customer)
        //        .Include(b => b.Item).Where(x => x.CompanyId == Utility.ActiveCompanyId).ToList();
        //    return View("Vehicle/AssignBookingNoList", BkgVehicles);
        //}
        //public IActionResult AssignBookingNo(int? id)
        //{
        //    BkgVehicle BkgVehicle = new BkgVehicle();

        //    ViewBag.EntityState = "Update";
        //    BkgVehicle = _dbContext.BkgVehicles
        //       .Where(x => x.Status == "PendingBooking" && x.Id == id && x.CompanyId == Utility.ActiveCompanyId)
        //       .Include(b => b.Customer)
        //       .Include(b => b.Item)
        //       .FirstOrDefault();
        //    if (BkgVehicle == null)
        //    {
        //        IList<BkgVehicle> BkgVehicles = _dbContext.BkgVehicles
        //        .Where(w => w.Status == "PendingBooking")
        //        .Include(b => b.Customer)
        //        .Include(b => b.Item).Where(x => x.CompanyId == Utility.ActiveCompanyId).ToList();
        //        ViewBag.success = false;
        //        ViewBag.message = ("Please select a valid value.");
        //        return View("Vehicle/AssignBookingNoList", BkgVehicles);
        //    }
        //    else
        //    {
        //        BkgVehicle.BookingDate = BkgVehicle.BookingDate;
        //        return View("Vehicle/AssignBookingNo", BkgVehicle);
        //    }
        //}
        //[HttpPost]
        //public IActionResult AssignBookingNo(BkgVehicle BkgVehicle)
        //{
        //    try
        //    {
        //        BkgVehicle savedBkgVehicle = _dbContext.BkgVehicles.Where(w => w.Id == BkgVehicle.Id).FirstOrDefault();
        //        savedBkgVehicle.UpdatedBy = Utility.ActiveUserId;
        //        savedBkgVehicle.UpdatedDate = DateTime.Now;
        //        savedBkgVehicle.Status = "Booked";

        //        savedBkgVehicle.BookingNo = BkgVehicle.BookingNo;
        //        savedBkgVehicle.BookingDate = BkgVehicle.BookingDate;
        //        savedBkgVehicle.GLAccountId = BkgVehicle.GLAccountId;
        //        savedBkgVehicle.ReferenceNo = BkgVehicle.ReferenceNo;
        //        savedBkgVehicle.BookingComission = BkgVehicle.BookingComission;
        //        savedBkgVehicle.AreaComission = BkgVehicle.AreaComission;
        //        savedBkgVehicle.CompanyReceiptNo = BkgVehicle.CompanyReceiptNo;

        //        _dbContext.SaveChanges();
        //        TempData["success"] = true;
        //        TempData["message"] = "The operation completed successfully.";
        //        return RedirectToAction("AssignBookingNoList");
        //    }
        //    catch (Exception Exp)
        //    {
        //        ViewBag.success = false;
        //        ViewBag.message = (Exp.InnerException == null ? Exp.Message : Exp.InnerException.Message);
        //        return View("Vehicle/AssignBookingNo", BkgVehicle);
        //    }

        //}

        //public IActionResult CashOrBankReceiptConfirmationList()
        //{
        //    if (TempData["success"] != null)
        //    {
        //        ViewBag.success = TempData["success"];
        //        ViewBag.message = TempData["message"];
        //    }
        //    IList<BkgVehicle> BkgVehicles = _dbContext.BkgVehicles
        //        .Where(w => w.Status == "Booking")
        //        .Include(b => b.Customer)
        //        .Include(b => b.Item).Where(x => x.CompanyId == Utility.ActiveCompanyId).ToList();

        //    return View("Vehicle/CashOrBankReceiptConfirmationList", BkgVehicles);
        //}
        //public IActionResult CashOrBankReceiptConfirmation(int? id)
        //{
        //    BkgVehicle BkgVehicle = new BkgVehicle();

        //    ViewBag.EntityState = "Update";
        //    BkgVehicle = _dbContext.BkgVehicles.Include(b => b.Customer)
        //       .Include(b => b.Item).Where(x => x.Id == id).FirstOrDefault();
        //    BkgVehicle.CashBankReceiptDate = DateTime.Now;
        //    return View("Vehicle/CashOrBankReceiptConfirmation", BkgVehicle);


        //}
        //[HttpPost]
        //public IActionResult CashOrBankReceiptConfirmation(BkgVehicle BkgVehicle)
        //{
        //    try
        //    {
        //        BkgVehicle savedBkgVehicle = _dbContext.BkgVehicles.Where(w => w.Id == BkgVehicle.Id).FirstOrDefault();
        //        savedBkgVehicle.UpdatedBy = Utility.ActiveUserId;
        //        savedBkgVehicle.UpdatedDate = DateTime.Now;
        //        savedBkgVehicle.Status = "PendingBooking";

        //        savedBkgVehicle.CashBankReceiptDate = BkgVehicle.CashBankReceiptDate;
        //        savedBkgVehicle.GLAccountId = BkgVehicle.GLAccountId;

        //        _dbContext.SaveChanges();
        //        TempData["success"] = true;
        //        TempData["message"] = "The operation completed successfully.";
        //        return RedirectToAction("CashOrBankReceiptConfirmationList");
        //    }
        //    catch (Exception Exp)
        //    {
        //        ViewBag.success = false;
        //        ViewBag.message = (Exp.InnerException == null ? Exp.Message : Exp.InnerException.Message);
        //        return View("Vehicle/CashOrBankReceiptConfirmation", BkgVehicle);
        //    }
        //}
        #endregion

        #region "Receipt"

        //public IActionResult ReceiptList()
        //{
        //    if (TempData["success"] != null)
        //    {
        //        ViewBag.success = TempData["success"];
        //        ViewBag.message = TempData["message"];
        //    }
        //    IList<BkgReceipt> BkgReceipts = _dbContext.BkgReceipts
        //        .Include(b => b.BkgVehicle)
        //        .Include(b => b.BkgVehicle.Customer).Where(x => x.CompanyId == Utility.ActiveCompanyId).OrderByDescending(o => o.Id).ToList();
        //    return View("Receipt/ReceiptList", BkgReceipts);
        //}
        //public IActionResult ReceiptCreate(int? id)
        //{
        //    BkgReceipt BkgReceipt = new BkgReceipt();
        //    if (id != null)
        //    {
        //        ViewBag.EntityState = "Update";
        //        BkgReceipt = _dbContext.BkgReceipts.Find(id);
        //        return View("Receipt/ReceiptCreate", BkgReceipt);

        //    }
        //    else
        //    {
        //        BkgReceipt.ReceiptDate = DateTime.Now;
        //        ViewBag.EntityState = "Create";
        //        return View("Receipt/ReceiptCreate", BkgReceipt);
        //    }
        //}
        //[HttpPost]
        //public IActionResult ReceiptCreate(BkgReceipt BkgReceipt)
        //{
        //    try
        //    {


        //        if (ModelState.IsValid)
        //        {
        //            if (BkgReceipt.Id == 0)
        //            {
        //                BkgReceipt.CompanyId = Utility.ActiveCompanyId;
        //                BkgReceipt.CreatedBy = Utility.ActiveUserId;
        //                BkgReceipt.CreatedDate = DateTime.Now;
        //                BkgReceipt.Status = "Created";
        //                _dbContext.BkgReceipts.Add(BkgReceipt);
        //            }
        //            else
        //            {

        //                _dbContext.Entry(BkgReceipt).State = EntityState.Modified;

        //                _dbContext.Entry(BkgReceipt).Property("CompanyId").IsModified = false;
        //                _dbContext.Entry(BkgReceipt).Property("CreatedBy").IsModified = false;
        //                _dbContext.Entry(BkgReceipt).Property("CreatedDate").IsModified = false;
        //                _dbContext.Entry(BkgReceipt).Property("Status").IsModified = false;

        //                BkgReceipt.UpdatedBy = Utility.ActiveUserId;
        //                BkgReceipt.UpdatedDate = DateTime.Now;
        //            }
        //            _dbContext.SaveChanges();

        //            TempData["success"] = true;
        //            TempData["message"] = "The operation completed successfully.";
        //            return RedirectToAction("ReceiptList");
        //        }
        //        else
        //            return View("Receipt/ReceiptCreate", BkgReceipt);
        //    }
        //    catch (Exception Exp)
        //    {
        //        ViewBag.success = false;
        //        ViewBag.message = (Exp.InnerException == null ? Exp.Message : Exp.InnerException.Message);
        //        return View("Receipt/ReceiptCreate", BkgReceipt);
        //    }
        //}
        //public IActionResult ReceiptDetail(int id)
        //{
        //    BkgReceipt BkgReceipt = _dbContext.BkgReceipts.Include(b => b.BkgVehicle)
        //        .Include(b => b.BkgVehicle.Customer).Where(x => x.Id == id).FirstOrDefault();
        //    return View("Receipt/ReceiptDetail", BkgReceipt);
        //}

        #endregion

        #region "Payment"

        //public IActionResult PaymentList()
        //{
        //    if (TempData["success"] != null)
        //    {
        //        ViewBag.success = TempData["success"];
        //        ViewBag.message = TempData["message"];
        //    }
        //    IList<BkgPayment> BkgPayments = _dbContext.BkgPayments
        //        .Include(b => b.BkgVehicle)
        //        .Include(b => b.BkgVehicle.Customer)
        //        .Include(b => b.GLAccount).Where(x => x.CompanyId == Utility.ActiveCompanyId).OrderByDescending(o => o.Id).ToList();
        //    return View("Payment/PaymentList", BkgPayments);
        //}
        //public IActionResult PaymentCreate(int? id)
        //{
        //    BkgPayment BkgPayment = new BkgPayment();
        //    if (id != null)
        //    {
        //        ViewBag.EntityState = "Update";
        //        BkgPayment = _dbContext.BkgPayments.Find(id);
        //        return View("Payment/PaymentCreate", BkgPayment);

        //    }
        //    else
        //    {
        //        BkgPayment.PaymentDate = DateTime.Now;
        //        ViewBag.EntityState = "Create";
        //        return View("Payment/PaymentCreate", BkgPayment);
        //    }
        //}
        //[HttpPost]
        //public IActionResult PaymentCreate(BkgPayment BkgPayment)
        //{
        //    try
        //    {

        //        if (ModelState.IsValid)
        //        {
        //            if (BkgPayment.Id == 0)
        //            {
        //                BkgPayment.CompanyId = Utility.ActiveCompanyId;
        //                BkgPayment.CreatedBy = Utility.ActiveUserId;
        //                BkgPayment.CreatedDate = DateTime.Now;
        //                _dbContext.BkgPayments.Add(BkgPayment);
        //            }
        //            else
        //            {
        //                _dbContext.Entry(BkgPayment).State = EntityState.Modified;

        //                _dbContext.Entry(BkgPayment).Property("CompanyId").IsModified = false;
        //                _dbContext.Entry(BkgPayment).Property("CreatedBy").IsModified = false;
        //                _dbContext.Entry(BkgPayment).Property("CreatedDate").IsModified = false;
        //                _dbContext.Entry(BkgPayment).Property("Status").IsModified = false;

        //                BkgPayment.UpdatedBy = Utility.ActiveUserId;
        //                BkgPayment.UpdatedDate = DateTime.Now;
        //            }
        //            _dbContext.SaveChanges();

        //            TempData["success"] = true;
        //            TempData["message"] = "The operation completed successfully.";
        //            return RedirectToAction("PaymentList");
        //        }
        //        else
        //            return View("Payment/PaymentCreate", BkgPayment);
        //    }
        //    catch (Exception Exp)
        //    {
        //        ViewBag.success = false;
        //        ViewBag.message = (Exp.InnerException == null ? Exp.Message : Exp.InnerException.Message);
        //        return View("Payment/PaymentCreate", BkgPayment);
        //    }
        //}
        //public IActionResult PaymentDetail(int id)
        //{
        //    BkgPayment BkgPayment = _dbContext.BkgPayments.Include(b => b.BkgVehicle)
        //        .Include(b => b.BkgVehicle.Customer)
        //        .Include(b => b.GLAccount).Where(x => x.Id == id).FirstOrDefault();
        //    return View("Payment/PaymentDetail", BkgPayment);
        //}

        #endregion

        #region "VehicleIGP"

        //public IActionResult VehicleIGPList()
        //{
        //    if (TempData["success"] != null)
        //    {
        //        ViewBag.success = TempData["success"];
        //        ViewBag.message = TempData["message"];
        //    }
        //    IList<BkgVehicleIGP> BkgVehicleIGPs = _dbContext.BkgVehicleIGPs
        //        .Include(b => b.BkgVehicle)
        //        .Include(b => b.BkgVehicle.Customer)
        //        .Include(b => b.BkgVehicle.Item)
        //        .Where(x => x.CompanyId == Utility.ActiveCompanyId)
        //        .OrderByDescending(o => o.Id)
        //        .ToList();
        //    return View("VehicleIGP/VehicleIGPList", BkgVehicleIGPs);
        //}
        //public IActionResult VehicleIGPCreate(int? id)
        //{
        //    BkgVehicleIGP BkgVehicleIGP = new BkgVehicleIGP();
        //    if (id != null)
        //    {
        //        ViewBag.EntityState = "Update";
        //        BkgVehicleIGP = _dbContext.BkgVehicleIGPs.Find(id);
        //        return View("VehicleIGP/VehicleIGPCreate", BkgVehicleIGP);

        //    }
        //    else
        //    {
        //        BkgVehicleIGP.IGPDate = DateTime.Now;
        //        ViewBag.EntityState = "Create";
        //        return View("VehicleIGP/VehicleIGPCreate", BkgVehicleIGP);
        //    }
        //}
        //[HttpPost]
        //public IActionResult VehicleIGPCreate(BkgVehicleIGP BkgVehicleIGP)
        //{
        //    try
        //    {

        //        if (ModelState.IsValid)
        //        {
        //            if (BkgVehicleIGP.Id == 0)
        //            {
        //                BkgVehicleIGP.CompanyId = Utility.ActiveCompanyId;
        //                BkgVehicleIGP.CreatedBy = Utility.ActiveUserId;
        //                BkgVehicleIGP.CreatedDate = DateTime.Now;
        //                _dbContext.BkgVehicleIGPs.Add(BkgVehicleIGP);
        //            }
        //            else
        //            {
        //                _dbContext.Entry(BkgVehicleIGP).State = EntityState.Modified;
        //                _dbContext.Entry(BkgVehicleIGP).Property("CompanyId").IsModified = false;
        //                _dbContext.Entry(BkgVehicleIGP).Property("CreatedBy").IsModified = false;
        //                _dbContext.Entry(BkgVehicleIGP).Property("CreatedDate").IsModified = false;
        //                _dbContext.Entry(BkgVehicleIGP).Property("Status").IsModified = false;

        //                BkgVehicleIGP.UpdatedBy = Utility.ActiveUserId;
        //                BkgVehicleIGP.UpdatedDate = DateTime.Now;
        //            }
        //            _dbContext.SaveChanges();

        //            TempData["success"] = true;
        //            TempData["message"] = "The operation completed successfully.";
        //            return RedirectToAction("VehicleIGPList");
        //        }
        //        else
        //            return View("VehicleIGP/VehicleIGPCreate", BkgVehicleIGP);
        //    }
        //    catch (Exception Exp)
        //    {
        //        ViewBag.success = false;
        //        ViewBag.message = (Exp.InnerException == null ? Exp.Message : Exp.InnerException.Message);
        //        return View("VehicleIGP/VehicleIGPCreate", BkgVehicleIGP);
        //    }
        //}
        //public IActionResult VehicleIGPDetail(int id)
        //{
        //    BkgVehicleIGP BkgVehicleIGP = _dbContext.BkgVehicleIGPs.Include(b => b.BkgVehicle)
        //        .Include(b => b.BkgVehicle.Customer)
        //        .Include(b => b.BkgVehicle.Item).Where(w => w.Id == id).FirstOrDefault();

        //    return View("VehicleIGP/VehicleIGPDetail", BkgVehicleIGP);
        //}

        #endregion

        #region "VehicleOGP"

        //public IActionResult VehicleOGPList()
        //{
        //    if (TempData["success"] != null)
        //    {
        //        ViewBag.success = TempData["success"];
        //        ViewBag.message = TempData["message"];
        //    }
        //    IList<BkgVehicleOGP> BkgVehicleOGPs = _dbContext.BkgVehicleOGPs
        //        .Include(b => b.BkgVehicle)
        //        .Include(b => b.BkgVehicle.Customer)
        //        .Include(b => b.BkgVehicle.Item)
        //        .Where(x => x.CompanyId == Utility.ActiveCompanyId)
        //        .OrderByDescending(o => o.Id)
        //        .ToList();
        //    return View("VehicleOGP/VehicleOGPList", BkgVehicleOGPs);
        //}
        //public IActionResult VehicleOGPCreate(int? id)
        //{
        //    BkgVehicleOGP BkgVehicleOGP = new BkgVehicleOGP();
        //    if (id != null)
        //    {
        //        ViewBag.EntityState = "Update";
        //        BkgVehicleOGP = _dbContext.BkgVehicleOGPs.Find(id);
        //        return View("VehicleOGP/VehicleOGPCreate", BkgVehicleOGP);

        //    }
        //    else
        //    {
        //        BkgVehicleOGP.OGPDate = DateTime.Now;
        //        ViewBag.EntityState = "Create";
        //        return View("VehicleOGP/VehicleOGPCreate", BkgVehicleOGP);
        //    }
        //}
        //[HttpPost]
        //public IActionResult VehicleOGPCreate(BkgVehicleOGP BkgVehicleOGP)
        //{
        //    try
        //    {

        //        if (ModelState.IsValid)
        //        {
        //            if (BkgVehicleOGP.Id == 0)
        //            {
        //                BkgVehicleOGP.CompanyId = Utility.ActiveCompanyId;
        //                BkgVehicleOGP.CreatedBy = Utility.ActiveUserId;
        //                BkgVehicleOGP.CreatedDate = DateTime.Now;
        //                _dbContext.BkgVehicleOGPs.Add(BkgVehicleOGP);
        //            }
        //            else
        //            {

        //                _dbContext.Entry(BkgVehicleOGP).State = EntityState.Modified;
        //                _dbContext.Entry(BkgVehicleOGP).Property("CompanyId").IsModified = false;
        //                _dbContext.Entry(BkgVehicleOGP).Property("CreatedBy").IsModified = false;
        //                _dbContext.Entry(BkgVehicleOGP).Property("CreatedDate").IsModified = false;
        //                _dbContext.Entry(BkgVehicleOGP).Property("Status").IsModified = false;

        //                BkgVehicleOGP.UpdatedBy = Utility.ActiveUserId;
        //                BkgVehicleOGP.UpdatedDate = DateTime.Now;
        //            }
        //            _dbContext.SaveChanges();

        //            TempData["success"] = true;
        //            TempData["message"] = "The operation completed successfully.";
        //            return RedirectToAction("VehicleOGPList");
        //        }
        //        else
        //            return View("VehicleOGP/VehicleOGPCreate", BkgVehicleOGP);
        //    }
        //    catch (Exception Exp)
        //    {
        //        ViewBag.success = false;
        //        ViewBag.message = (Exp.InnerException == null ? Exp.Message : Exp.InnerException.Message);
        //        return View("VehicleOGP/VehicleOGPCreate", BkgVehicleOGP);
        //    }
        //}
        //public IActionResult VehicleOGPDetail(int id)
        //{
        //    BkgVehicleOGP BkgVehicleOGP = _dbContext.BkgVehicleOGPs.Include(b => b.BkgVehicle)
        //        .Include(b => b.BkgVehicle.Customer)
        //        .Include(b => b.BkgVehicle.Item).Where(w => w.Id == id).FirstOrDefault();

        //    return View("VehicleOGP/VehicleOGPDetail", BkgVehicleOGP);
        //}

        #endregion

        #region "ComissionReceived"


        //public IActionResult ComissionReceivedList()
        //{
        //    if (TempData["success"] != null)
        //    {
        //        ViewBag.success = TempData["success"];
        //        ViewBag.message = TempData["message"];
        //    }
        //    IList<BkgComissionReceived> BkgComissionReceiveds = _dbContext.BkgComissionReceiveds
        //        .Include(b => b.BkgVehicle)
        //        .Include(b => b.BkgVehicle.Customer)

        //        .Where(x => x.CompanyId == Utility.ActiveCompanyId).OrderByDescending(o => o.Id).ToList();
        //    return View("ComissionReceived/ComissionReceivedList", BkgComissionReceiveds);
        //}
        //public IActionResult ComissionReceivedCreate(int? id)
        //{
        //    BkgComissionReceived BkgComissionReceived = new BkgComissionReceived();
        //    if (id != null)
        //    {
        //        ViewBag.EntityState = "Update";
        //        BkgComissionReceived = _dbContext.BkgComissionReceiveds.Find(id);
        //        return View("ComissionReceived/ComissionReceivedCreate", BkgComissionReceived);

        //    }
        //    else
        //    {
        //        BkgComissionReceived.ReceivedDate = DateTime.Now;
        //        ViewBag.EntityState = "Create";
        //        return View("ComissionReceived/ComissionReceivedCreate", BkgComissionReceived);
        //    }
        //}
        //[HttpPost]
        //public IActionResult ComissionReceivedCreate(BkgComissionReceived BkgComissionReceived)
        //{
        //    try
        //    {


        //        if (ModelState.IsValid)
        //        {
        //            if (BkgComissionReceived.Id == 0)
        //            {
        //                BkgComissionReceived.CompanyId = Utility.ActiveCompanyId;
        //                BkgComissionReceived.CreatedBy = Utility.ActiveUserId;
        //                BkgComissionReceived.CreatedDate = DateTime.Now;
        //                _dbContext.BkgComissionReceiveds.Add(BkgComissionReceived);
        //            }
        //            else
        //            {
        //                _dbContext.Entry(BkgComissionReceived).State = EntityState.Modified;
        //                _dbContext.Entry(BkgComissionReceived).Property("CompanyId").IsModified = false;
        //                _dbContext.Entry(BkgComissionReceived).Property("CreatedBy").IsModified = false;
        //                _dbContext.Entry(BkgComissionReceived).Property("CreatedDate").IsModified = false;
        //                _dbContext.Entry(BkgComissionReceived).Property("Status").IsModified = false;
        //                BkgComissionReceived.UpdatedBy = Utility.ActiveUserId;
        //                BkgComissionReceived.UpdatedDate = DateTime.Now;
        //            }
        //            _dbContext.SaveChanges();

        //            TempData["success"] = true;
        //            TempData["message"] = "The operation completed successfully.";
        //            return RedirectToAction("ComissionReceivedList");
        //        }
        //        else
        //            return View("ComissionReceived/ComissionReceivedCreate", BkgComissionReceived);
        //    }
        //    catch (Exception Exp)
        //    {
        //        ViewBag.success = false;
        //        ViewBag.message = (Exp.InnerException == null ? Exp.Message : Exp.InnerException.Message);
        //        return View("ComissionReceived/ComissionReceivedCreate", BkgComissionReceived);
        //    }
        //}
        //public IActionResult ComissionReceivedDetail(int id)
        //{
        //    BkgComissionReceived BkgComissionReceived = _dbContext.BkgComissionReceiveds.Include(b => b.BkgVehicle)
        //        .Include(b => b.BkgVehicle.Customer).Where(x => x.Id == id).FirstOrDefault();
        //    return View("ComissionReceived/ComissionReceivedDetail", BkgComissionReceived);
        //}

        #endregion

        #region "ComissionPayment"

        //public IActionResult ComissionPaymentList()
        //{
        //    if (TempData["success"] != null)
        //    {
        //        ViewBag.success = TempData["success"];
        //        ViewBag.message = TempData["message"];
        //    }
        //    IList<BkgComissionPayment> BkgComissionPayments = _dbContext.BkgComissionPayments
        //        .Include(b => b.BkgVehicle)
        //        .Include(b => b.BkgVehicle.Customer)
        //        .Where(x => x.CompanyId == Utility.ActiveCompanyId).OrderByDescending(o => o.Id).ToList();
        //    return View("ComissionPayment/ComissionPaymentList", BkgComissionPayments);
        //}
        //public IActionResult ComissionPaymentCreate(int? id)
        //{
        //    BkgComissionPayment BkgComissionPayment = new BkgComissionPayment();
        //    if (id != null)
        //    {
        //        ViewBag.EntityState = "Update";
        //        BkgComissionPayment = _dbContext.BkgComissionPayments.Find(id);
        //        return View("ComissionPayment/ComissionPaymentCreate", BkgComissionPayment);

        //    }
        //    else
        //    {
        //        BkgComissionPayment.PaymentDate = DateTime.Now;
        //        ViewBag.EntityState = "Create";
        //        return View("ComissionPayment/ComissionPaymentCreate", BkgComissionPayment);
        //    }
        //}
        //[HttpPost]
        //public IActionResult ComissionPaymentCreate(BkgComissionPayment BkgComissionPayment)
        //{
        //    try
        //    {

        //        if (ModelState.IsValid)
        //        {
        //            if (BkgComissionPayment.Id == 0)
        //            {
        //                BkgComissionPayment.CompanyId = Utility.ActiveCompanyId;
        //                BkgComissionPayment.CreatedBy = Utility.ActiveUserId;
        //                BkgComissionPayment.CreatedDate = DateTime.Now;
        //                _dbContext.BkgComissionPayments.Add(BkgComissionPayment);
        //            }
        //            else
        //            {
        //                _dbContext.Entry(BkgComissionPayment).State = EntityState.Modified;
        //                _dbContext.Entry(BkgComissionPayment).Property("CompanyId").IsModified = false;
        //                _dbContext.Entry(BkgComissionPayment).Property("CreatedBy").IsModified = false;
        //                _dbContext.Entry(BkgComissionPayment).Property("CreatedDate").IsModified = false;
        //                _dbContext.Entry(BkgComissionPayment).Property("Status").IsModified = false;

        //                BkgComissionPayment.UpdatedBy = Utility.ActiveUserId;
        //                BkgComissionPayment.UpdatedDate = DateTime.Now;
        //            }
        //            _dbContext.SaveChanges();

        //            TempData["success"] = true;
        //            TempData["message"] = "The operation completed successfully.";
        //            return RedirectToAction("ComissionPaymentList");
        //        }
        //        else
        //            return View("ComissionPayment/ComissionPaymentCreate", BkgComissionPayment);
        //    }
        //    catch (Exception Exp)
        //    {
        //        ViewBag.success = false;
        //        ViewBag.message = (Exp.InnerException == null ? Exp.Message : Exp.InnerException.Message);
        //        return View("ComissionPayment/ComissionPaymentCreate", BkgComissionPayment);
        //    }
        //}
        //public IActionResult ComissionPaymentDetail(int id)
        //{
        //    BkgComissionPayment BkgComissionPayment = _dbContext.BkgComissionPayments.Include(b => b.BkgVehicle)
        //        .Include(b => b.BkgVehicle.Customer)
        //       .Where(x => x.Id == id).FirstOrDefault();
        //    return View("ComissionPayment/ComissionPaymentDetail", BkgComissionPayment);
        //}

        #endregion

        #region "CashPurchaseSale"
        //public IActionResult CashPurchaseSaleCreate(int? id)
        //{
        //    BkgCashPurchaseSale BkgCashPurchaseSale = new BkgCashPurchaseSale();
        //    if (id != null)
        //    {
        //        ViewBag.EntityState = "Update";
        //        BkgCashPurchaseSale = _dbContext.BkgCashPurchaseSales.Find(id);
        //        return View("CashPurchaseSale/CashPurchaseSaleCreate", BkgCashPurchaseSale);

        //    }
        //    else
        //    {
        //        BkgCashPurchaseSale.TransDate = DateTime.Now;
        //        BkgCashPurchaseSale.BookingDate = DateTime.Now;
        //        BkgCashPurchaseSale.IGPDate = DateTime.Now;
        //        BkgCashPurchaseSale.OGPDate = DateTime.Now;
        //        BkgCashPurchaseSale.PurchaseDate = DateTime.Now;
        //        BkgCashPurchaseSale.SalesDate = DateTime.Now;
        //        ViewBag.EntityState = "Create";
        //        return View("CashPurchaseSale/CashPurchaseSaleCreate", BkgCashPurchaseSale);
        //    }
        //}
        //public IActionResult CashPurchaseSaleList()
        //{
        //    if (TempData["success"] != null)
        //    {
        //        ViewBag.success = TempData["success"];
        //        ViewBag.message = TempData["message"];
        //    }
        //    IList<BkgCashPurchaseSale> BkgCashPurchaseSale = _dbContext.BkgCashPurchaseSales
        //        .Include(b => b.Customer)
        //        .Include(b => b.Item)
        //        .Where(x => x.CompanyId == Utility.ActiveCompanyId).OrderByDescending(o => o.Id).
        //        ToList();
        //    return View("CashPurchaseSale/CashPurchaseSaleList", BkgCashPurchaseSale);
        //}
        //[HttpPost]
        //public IActionResult CashPurchaseSaleCreate(BkgCashPurchaseSale BkgCashPurchaseSale)
        //{
        //    try
        //    {

        //        if (ModelState.IsValid)
        //        {
        //            if (BkgCashPurchaseSale.Id == 0)
        //            {
        //                BkgCashPurchaseSale.CompanyId = Utility.ActiveCompanyId;
        //                BkgCashPurchaseSale.CreatedBy = Utility.ActiveUserId;
        //                BkgCashPurchaseSale.CreatedDate = DateTime.Now;
        //                _dbContext.BkgCashPurchaseSales.Add(BkgCashPurchaseSale);
        //            }
        //            else
        //            {
        //                _dbContext.Entry(BkgCashPurchaseSale).State = EntityState.Modified;
        //                _dbContext.Entry(BkgCashPurchaseSale).Property("CompanyId").IsModified = false;
        //                _dbContext.Entry(BkgCashPurchaseSale).Property("CreatedBy").IsModified = false;
        //                _dbContext.Entry(BkgCashPurchaseSale).Property("CreatedDate").IsModified = false;
        //                _dbContext.Entry(BkgCashPurchaseSale).Property("Status").IsModified = false;

        //                BkgCashPurchaseSale.UpdatedBy = Utility.ActiveUserId;
        //                BkgCashPurchaseSale.UpdatedDate = DateTime.Now;
        //            }
        //            _dbContext.SaveChanges();

        //            TempData["success"] = true;
        //            TempData["message"] = "The operation completed successfully.";
        //            return RedirectToAction("CashPurchaseSaleList");
        //        }
        //        else
        //            return View("CashPurchaseSale/CashPurchaseSaleCreate", BkgCashPurchaseSale);
        //    }
        //    catch (Exception Exp)
        //    {
        //        ViewBag.success = false;
        //        ViewBag.message = (Exp.InnerException == null ? Exp.Message : Exp.InnerException.Message);
        //        return View("CashPurchaseSale/CashPurchaseSaleCreate", BkgCashPurchaseSale);
        //    }
        //}

        #endregion

        #region "LOVs"
        //[HttpGet]
        //public IActionResult GetCustomers(string q = "")
        //{
        //    var customers = _dbContext.BkgCustomers.Where(
        //                                        a => a.IsActive == true && a.Company.Id == Utility.ActiveCompanyId
        //                                        && (a.Name.Contains(q)
        //                                       ))
        //                                       .Select(a => new
        //                                       {
        //                                           id = a.Id,
        //                                           text = string.Concat(a.CNIC, " - ", a.Name)
        //                                       })
        //                                       .OrderBy(a => a.text)
        //                                       .Take(25)
        //                                       .ToList();
        //    return Ok(customers);
        //}
        //[HttpGet]
        //public IActionResult GetCustomer(int id)
        //{
        //    var customer = _dbContext.BkgCustomers.Where(a => a.Company.Id == Utility.ActiveCompanyId && a.Id == id)
        //                                       .Select(a => new
        //                                       {
        //                                           id = a.Id,
        //                                           text = string.Concat(a.CNIC, " - ", a.Name)
        //                                       })
        //                                       .FirstOrDefault();
        //    return Ok(customer);
        //}
        

        //[HttpGet]
        //public IActionResult GetItems(string q = "")
        //{
        //    var items = _dbContext.BkgItems.Where(
        //                                        (a => a.Description.Contains(q)
        //                                       ))
        //                                       .Select(a => new
        //                                       {
        //                                           id = a.Id,
        //                                           text = string.Concat(a.Description)
        //                                       })
        //                                       .OrderBy(a => a.text)
        //                                       .Take(25)
        //                                       .ToList();
        //    return Ok(items);
        //}
        //[HttpGet]
        //public IActionResult GetItem(int id)
        //{
        //    var items = _dbContext.BkgItems.Where(a => a.Id == id)
        //                                       .Select(a => new
        //                                       {
        //                                           id = a.Id,
        //                                           text = string.Concat(a.Description)
        //                                       })
        //                                       .FirstOrDefault();
        //    return Ok(items);
        //}

        //[HttpGet]
        //public IActionResult GetBkgVehicles(string q = "")
        //{
        //    var vehicles = _dbContext.BkgVehicles.Where(
        //                                        a => (a.Customer.CNIC.Contains(q)
        //                                       ))
        //                                       .Select(a => new
        //                                       {
        //                                           id = a.Id,
        //                                           text = string.Concat(a.BookingNo, " - ", a.Customer.Name)
        //                                       })
        //                                       .OrderBy(a => a.text)
        //                                       .Take(25)
        //                                       .ToList();
        //    return Ok(vehicles);
        //}
        //[HttpGet]
        //public IActionResult GetBkgVehicle(int id)
        //{
        //    var vehicle = _dbContext.BkgVehicles.Where(a => a.Id == id)
        //                                       .Select(a => new
        //                                       {
        //                                           id = a.Id,
        //                                           text = string.Concat(a.Customer.CNIC, " - ", a.Customer.Name)
        //                                       })
        //                                       .FirstOrDefault();
        //    return Ok(vehicle);
        //}
        //[HttpGet]
        //public IActionResult GetReceiptTypes(string q = "")
        //{
        //    var receiptTypes = _dbContext.AppCompanyConfigs.Where(
        //                                        (a => a.ConfigValue.Contains(q)
        //                                       )).Where(w => w.ConfigName == "Receipt Type")

        //                                       .Select(a => new
        //                                       {
        //                                           id = a.Id,
        //                                           text = a.ConfigValue
        //                                       })
        //                                       .OrderBy(a => a.text)
        //                                      .ToList();
        //    return Ok(receiptTypes);
        //}

        //[HttpGet]
        //public IActionResult GetReceiptType(int id)
        //{
        //    var receiptTypes = _dbContext.AppCompanyConfigs.Where(w => w.ConfigName == "Receipt Type" && w.Id == id)
        //                                       .Select(a => new
        //                                       {
        //                                           id = a.Id,
        //                                           text = a.ConfigValue
        //                                       })
        //                                       .FirstOrDefault();
        //    return Ok(receiptTypes);
        //}
        #endregion

        #region "Detail Info"
        //[HttpGet]
        //public IActionResult GetReceivedAmount(int id)
        //{
        //    var receivedAmount = _dbContext.BkgVehicles.Where(a => a.Id == id)
        //        .Select(a => new
        //        {
        //            id = a.Customer.Name,
        //            text = a.ReceivedAmount

        //        })
        //        .FirstOrDefault();
        //    return Ok(receivedAmount);
        //}
        //[HttpGet]
        //public IActionResult GetIGPDetail(int id)
        //{
        //    var IGPDetail = _dbContext.BkgVehicleIGPs.Where(a => a.BkgVehicle.Id == id)
        //        .Select(a => new
        //        {
        //            id = a.EngineNo,
        //            text = a.ChassisNo

        //        })
        //        .FirstOrDefault();
        //    return Ok(IGPDetail);
        //}
        
        //public JsonResult checkBookingNoAlreadyExist(string bookingNo)
        //{
        //    System.Threading.Thread.Sleep(200);
        //    if (bookingNo.Length == 0)
        //        return Json(0);

        //    var chkbookingNo = _dbContext.BkgVehicles.Where(a => a.BookingNo == bookingNo)
        //                                       .Select(a => a.BookingNo)
        //                                       .FirstOrDefault();
        //    return Json(chkbookingNo == null ? 0 : 1);
        //}

        //public JsonResult checkCustomerCNICAlreadyExist(string cnic)
        //{
        //    System.Threading.Thread.Sleep(200);
        //    if (cnic.Length == 0)
        //        return Json(0);

        //    var chkCNIC = _dbContext.BkgCustomers.Where(a => a.CNIC == cnic)
        //                                       .Select(a => a.CNIC)
        //                                       .FirstOrDefault();
        //    return Json(chkCNIC == null ? 0 : 1);
        //}

        #endregion
    }
}