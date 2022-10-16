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
    public class OGPController : Controller
    {
        private readonly NumbersDbContext _dbContext;


        public OGPController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;

        }
        public IActionResult Index()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            IList<BkgVehicleOGP> list = _dbContext.BkgVehicleOGPs
                .Include(b => b.Vehicle)
                .Include(b => b.Vehicle.Customer)
                .Include(b => b.Vehicle.Item)
                .Where(x => x.CompanyId == companyId)
                .OrderByDescending(o => o.Id).ToList();
                
            return View(list);
        }
        public IActionResult Create(int? id)
        {
            //ViewBag.Status = new SelectList("A,B,C", "ReferenceNo", "ReferenceNo");
            BkgVehicleOGP ogp = new BkgVehicleOGP();
            if (id != null)
            {
                ViewBag.EntityState = "Update";
                ogp = _dbContext.BkgVehicleOGPs.Find(id);
                string configValues = _dbContext.AppCompanyConfigs
                            .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                            .Select(c => c.ConfigValue)
                            .FirstOrDefault();
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                ViewBag.ReportPath = string.Concat(configValues, "Viewer", "?Report=Voucher&cId=", companyId, "&id={0}");
                return View("Create", ogp);

            }
            else
            {
                ogp.OGPDate = DateTime.Now;
                ViewBag.EntityState = "Create";
                return View("Create", ogp);
            }
        }
        [HttpPost]
        public async Task <IActionResult> Create(BkgVehicleOGP ogp)
        {
            int companyId=HttpContext.Session.GetInt32("CompanyId").Value;
            string userId=HttpContext.Session.GetString("UserId");
            try
            {

                //if (ModelState.IsValid)
                //{
                    if (ogp.Id == 0)
                    {
                        ogp.CompanyId = companyId;
                        ogp.CreatedBy = userId;
                        ogp.CreatedDate = DateTime.Now;
                        ogp.Status = "Created";
                        _dbContext.BkgVehicleOGPs.Add(ogp);
                    }
                    else
                    {
                        var data = _dbContext.BkgVehicleOGPs.Find(ogp.Id);
                        data.OGPDate = ogp.OGPDate;
                        data.OGPNo = ogp.OGPNo;
                        data.VehicleId = ogp.VehicleId;
                        data.BiltyNo = ogp.BiltyNo;
                        data.ReceivedBy = ogp.ReceivedBy;
                        data.DeliveryStatus = ogp.DeliveryStatus;
                        //data.VehicleNo = ogp.VehicleNo;
                        data.InsuranceReceiptNo = ogp.InsuranceReceiptNo;
                        data.InsuranceAmount = ogp.InsuranceAmount;
                        data.RTP = ogp.RTP;
                        data.MFIGP = ogp.MFIGP;
                        data.MFOGP = ogp.MFOGP;
                        data.DLNo = ogp.DLNo;
                        data.PANo = ogp.PANo;
                        data.Description = ogp.Description;
                        data.UpdatedBy = userId;
                        data.UpdatedDate = DateTime.Now;

                        var entry = _dbContext.BkgVehicleOGPs.Update(data);
                        entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                    }

                    await _dbContext.SaveChangesAsync();

                    TempData["error"] = "false";
                    TempData["message"] = "OGP has been saved successfully.";
                    return RedirectToAction("Index");
                //}
                //else
                //    return View("Create", ogp);          
        }
            catch (Exception Exp)
            {
                TempData["error"] = "true";
                TempData["message"] = (Exp.InnerException == null ? Exp.Message : Exp.InnerException.Message);
                return View("Create", ogp);
            }
        }
        public async Task <IActionResult> Approve(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            //BkgVehicleIGP bkgIGP = _dbContext.BkgVehicleIGPs.Find(id);
            BkgVehicleOGP ogp = _dbContext.BkgVehicleOGPs
                .Include(o => o.Vehicle)
                .Include(o => o.Vehicle.Customer)
                .Include(o => o.Vehicle.Item)
                .Where(a => a.Status == "Created" && a.CompanyId == companyId && a.Id == id)
                .FirstOrDefault();
            int voucherId = 0;

            if (!ogp.RTP && !ogp.Vehicle.BookingType.Contains("Own"))
            {
                //Voucher
                VoucherHelper voucher = new VoucherHelper(_dbContext, HttpContext);
                GLVoucher voucherMaster = new GLVoucher();
                List<GLVoucherDetail> voucherDetails = new List<GLVoucherDetail>();
                string voucherDescription = string.Format(
                    "D/O MF-240 to {0} " +
                    "agt. OGP # {1} " +
                    "Booking ID {2}" +
                    " R. # {3} " +
                    "Booking #{4} " +
                    "Chassis # {5} " +
                    "Engine # {6} " +
                    "Invoice # {7} " +
                    "Ins.R # {8}", 
                    ogp.Vehicle.Customer.Name,                       //0
                    ogp.OGPNo,                                       //1
                    ogp.Vehicle.Id,                                  //2
                    ogp.Vehicle.ReferenceNo,                         //3
                    ogp.Vehicle.BookingNo,                          //4
                    ogp.Vehicle.ChassisNo,                          //5
                    ogp.Vehicle.EngineNo,                           //6
                    ogp.BiltyNo,                                    //7
                    ogp.InsuranceReceiptNo);                        //8
                voucherMaster.VoucherType = "BKO";
                voucherMaster.VoucherDate = ogp.OGPDate;
                voucherMaster.Reference = ogp.OGPNo.ToString();
                voucherMaster.Currency = "PKR";
                voucherMaster.CurrencyExchangeRate = 1;
                voucherMaster.Description = voucherDescription.Substring(0, voucherDescription.Length);
                voucherMaster.Status = "Approved";
                voucherMaster.IsSystem = true;
                voucherMaster.ModuleName = "Booking OGP";
                voucherMaster.ModuleId = id;

                //Voucher Details

                //Debit Entry
                GLVoucherDetail voucherDetail = new GLVoucherDetail();
                voucherDetail.AccountId = ogp.Vehicle.Customer.AccountId;

                voucherDetail.Sequence = 10;
                voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                voucherDetail.Debit = ogp.Vehicle.ReceivedAmount;
                voucherDetail.Credit = 0;  // for payment payment.PaymentAmount
                voucherDetail.IsDeleted = false;
                voucherDetail.CreatedBy = userId;
                voucherDetail.CreatedDate = DateTime.Now;
                voucherDetails.Add(voucherDetail);

                /*
                var accountId = (from c in _dbContext.AppCompanyConfigs
                                 where c.ConfigName == "Booking Type" && c.ConfigValue == ogp.Vehicle.BookingType
                                 select c.UserValue1).First();
                                 */
                //Credit Entry
                voucherDetail = new GLVoucherDetail();
                voucherDetail.AccountId = ogp.Vehicle.Item.CustomerStockAccountId;
                voucherDetail.Sequence = 20;
                voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                voucherDetail.Debit = 0; // for payment payment.PaymentAmount
                voucherDetail.Credit = ogp.Vehicle.ReceivedAmount; // for payment 0 
                voucherDetail.IsDeleted = false;
                voucherDetail.CreatedBy = userId;
                voucherDetail.CreatedDate = DateTime.Now;
                voucherDetails.Add(voucherDetail);

                //Create Voucher 
                voucherId = voucher.CreateVoucher(voucherMaster, voucherDetails);
            }

            ogp.VoucherId = voucherId;
            ogp.Status = "Approved";
            ogp.UpdatedBy = userId;
            ogp.UpdatedDate = DateTime.Now;

            // Update BkgVehicle
            ogp.Vehicle.Status = ogp.DeliveryStatus;
            ogp.Vehicle.DeliveryDate = ogp.OGPDate;
            ogp.Vehicle.OGPNo = ogp.OGPNo;
            TempData["error"] = "false";
            TempData["message"] = "OGP has been approved successfully";
            
            // Approve/Update OGP

            var entry = _dbContext.Update(ogp);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            
        }
        public IActionResult Detail(int id)
        {
            BkgVehicleOGP ogp = _dbContext.BkgVehicleOGPs.Include(b => b.Vehicle)
                .Include(b => b.Vehicle.Customer)
                .Include(b => b.Vehicle.Item).Where(w => w.Id == id).FirstOrDefault();

            return View("Detail", ogp);
        }
        public int  GetInsuranceAmount(int vehicleId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            //System.Threading.Thread.Sleep(2000);
            if (vehicleId == 0)
                return 0;
            var returnVal = _dbContext.BkgVehicleIGPs.Where(v => v.CompanyId == companyId && v.VehicleId == vehicleId)
                                                     .Select(c => c.InsuranceAmount)
                                                     .FirstOrDefault();
            return Convert.ToInt32(returnVal);
        }
    }
}