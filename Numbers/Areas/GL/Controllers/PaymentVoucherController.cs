using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Repository.Setup;
using System.Linq.Dynamic.Core;
using Numbers.Helpers;

namespace Numbers.Areas.GL.Controllers
{
    [Authorize]
    [Area("GL")]
    public class PaymentVoucherController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public PaymentVoucherController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        //public ActionResult Index(string type)
        //{
        //    int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
        //    if (type == null)
        //    {
        //        TempData["error"] = "true";
        //        TempData["message"] = "Missing Voucher type parameter";
        //        return Redirect("/Dashboard/Index");
        //    }
        //    string configValues = _dbContext.AppCompanyConfigs
        //       .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
        //       .Select(c => c.ConfigValue)
        //       .FirstOrDefault();
        //    ViewBag.ReportPath = string.Concat(configValues, "Viewer", "?Report=Voucher&cId=", companyId, "&id={0}");

        //    var voucherType = _dbContext.GLVoucherTypes
        //         .Where(v => v.VoucherType == type && v.IsSystem == false && v.CompanyId == companyId)
        //        .FirstOrDefault();

        //    if (voucherType == null)
        //    {
        //        TempData["error"] = "true";
        //        TempData["message"] = "Invalid Voucher type";
        //        return Redirect("/Dashboard/Index");
        //    }
        //    else
        //    {
        //        ViewBag.VoucherType = voucherType.VoucherType;
        //        ViewBag.Title = voucherType.Description;
        //    }
        //    var month = DateTime.Now.Month;
        //    var vouchers = _dbContext.GLVouchers
        //        .Where(v => v.VoucherType == type && v.CompanyId == companyId && v.VoucherDate.Month == month && v.IsDeleted == false);
        //        /*.OrderByDescending(v => v.VoucherNo)*/
        //    return View(vouchers);
        //}
        [HttpGet]
        public IActionResult Index(Dictionary<string, string> parameters)
        {
            var type = parameters["type"];
            ViewBag.Type = type;
            var searchDate = "";
            if (parameters.Count == 1)
            {
                searchDate = DateTime.Now.ToString("MMM-yy");
            }
            else
            {
                searchDate = parameters["Date"];

            }
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            string userid = HttpContext.Session.GetString("UserId");
            ViewBag.Approve = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userid && x.IsDeleted == false && x.ResponsibilityId == resp_Id).FirstOrDefault().Approve;
            // var BranchIds = _dbContext.AppUserBranches.Where(x => x.UserId == userid).Select(x => x.BranchId).ToList();
            if (type == null)
            {
                TempData["error"] = "true";
                TempData["message"] = "Missing Voucher type parameter";
                return Redirect("/Dashboard/Index");
            }
            string configValues = _dbContext.AppCompanyConfigs
               .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
               .Select(c => c.ConfigValue)
               .FirstOrDefault();
            ViewBag.ReportPath = string.Concat(configValues, "Viewer", "?Report=Voucher&cId=", companyId, "&id=");

            var voucherType = _dbContext.GLVoucherTypes
                 .Where(v => v.VoucherType == type && v.IsSystem == false)
                .FirstOrDefault();

            if (voucherType == null)
            {
                TempData["error"] = "true";
                TempData["message"] = "Invalid Voucher type";
                return Redirect("/Dashboard/Index");
            }
            else
            {
                ViewBag.VoucherType = voucherType.VoucherType;
                ViewBag.Title = voucherType.Description;
                ViewBag.NavbarHeading = voucherType.Description;
            }
            DateTime date = Convert.ToDateTime(searchDate);
            var month = date.Month;
            var vouchers = _dbContext.GLVouchers
                .Where(v => v.VoucherType == type && v.CompanyId == companyId && v.IsDeleted == false && v.VoucherDate.Month == month);
            //.Include(d => d.GLVoucherDetail)
            //.OrderByDescending(v => v.VoucherNo);
            return View(vouchers);

        }
        public IActionResult Approve()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userid = HttpContext.Session.GetString("UserId");
            //var BranchIds = _dbContext.AppUserBranches.Where(x => x.UserId == userid).Select(x => x.BranchId).ToList();
            string configValues = _dbContext.AppCompanyConfigs
              .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
              .Select(c => c.ConfigValue)
              .FirstOrDefault();
            ViewBag.ReportPath = string.Concat(configValues, "Viewer", "?Report=Voucher&cId=", companyId, "&id={0}");
            var voucher = _dbContext.GLVouchers
               .Where(v => v.Status == "Created" && v.IsDeleted == false && v.CompanyId == HttpContext.Session.GetInt32("CompanyId") && v.VoucherType != "JV"
               && v.IsDeleted == false);
            ViewBag.NavbarHeading = "Approve Payment Voucher";
            return View(voucher);
        }
        public ActionResult UnApprove()
        {
            try
            {
                string userid = HttpContext.Session.GetString("UserId");
                //var BranchIds = _dbContext.AppUserBranches.Where(x => x.UserId == userid).Select(x => x.BranchId).ToList();
                var voucher = _dbContext.GLVouchers
                   .Where(v => v.Status == "Approved" && v.IsSystem == false && v.IsDeleted == false && v.CompanyId == HttpContext.Session.GetInt32("CompanyId").Value && v.VoucherType != "JV"
                   && v.IsDeleted == false);
                ViewBag.NavbarHeading = "Un-Approve Voucher";
                return View(voucher);
            }
            catch (Exception exc)
            {
                TempData["error"] = "true";
                TempData["message"] = exc.Message == null ? exc.InnerException.Message.ToString() : exc.Message.ToString();
                return RedirectToAction("Index", "Dashboard");
            }

        }
        public async Task<IActionResult> UnApproveVoucher(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var voucher = _dbContext.GLVouchers
                            .Where(v => v.IsDeleted == false && v.Id == id && v.Status == "Approved" && v.IsSystem == false && v.CompanyId == companyId).FirstOrDefault();
            if (voucher == null)
            {

                TempData["error"] = "true";
                TempData["message"] = "Voucher not found";
            }
            else
            {
                voucher.Status = "Created";
                voucher.ApprovedBy = null;
                voucher.ApprovedDate = null;
                var entry = _dbContext.GLVouchers.Update(voucher);
                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                await _dbContext.SaveChangesAsync();
                TempData["error"] = "false";
                TempData["message"] = string.Format("Voucher Id. {0} has been Un-Approved successfully", voucher.Id);
            }

            return RedirectToAction(nameof(UnApprove));
        }
        [HttpGet]
        public async Task<IActionResult> ApproveVoucher(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var voucher = _dbContext.GLVouchers
               .Where(v => v.Status == "Created" && v.CompanyId == companyId
               && v.IsDeleted == false && v.IsSystem == false && v.Id == id).FirstOrDefault();
            if (voucher == null)
            {

                TempData["error"] = "true";
                TempData["message"] = "Voucher not found";
            }
            else
            {
                voucher.Status = "Approved";
                voucher.ApprovedBy = userId;
                voucher.ApprovedDate = DateTime.Now;
                var entry = _dbContext.GLVouchers.Update(voucher);
                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                await _dbContext.SaveChangesAsync();

                TempData["error"] = "false";
                TempData["message"] = string.Format("Voucher No. {0}-{1} has been Approved", voucher.VoucherType, voucher.VoucherNo);
            }
            string route = string.Format("/GL/PaymentVoucher/Index?type={0}", voucher.VoucherType);
            return Redirect(route);
        }
        public async Task<bool> VouchersApproved(List<int> Ids)
        {
            foreach (var item in Ids)
            {
                await ApproveVouchersAll(item);
            }
            return true;
        }

        public async Task<bool> ApproveVouchersAll(int Id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var voucher = _dbContext.GLVouchers
               .Where(v => v.Status == "Created" && v.CompanyId == companyId
               && v.IsDeleted == false && v.IsSystem == false && v.Id == Id).FirstOrDefault();

            voucher.Status = "Approved";
            voucher.ApprovedBy = userId;
            voucher.ApprovedDate = DateTime.Now;
            var entry = _dbContext.GLVouchers.Update(voucher);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<IActionResult> Delete(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var voucher = _dbContext.GLVouchers
               .Where(v => v.Status == "Created" && v.CompanyId == companyId
               && v.IsDeleted == false && v.IsSystem == false && v.Id == id).FirstOrDefault();
            if (voucher == null)
            {

                TempData["error"] = "true";
                TempData["message"] = "Voucher not found";
            }
            else
            {
                voucher.Status = "Deleted";
                voucher.IsDeleted = true;
                var entry = _dbContext.GLVouchers.Update(voucher);
                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                await _dbContext.SaveChangesAsync();

                TempData["error"] = "false";
                TempData["message"] = string.Format("Voucher No. {0}-{1} has been Deleted", voucher.VoucherType, voucher.VoucherNo);
            }
            string route = string.Format("/GL/PaymentVoucher/Index?type={0}", voucher.VoucherType);
            return Redirect(route);
        }
        [HttpGet]
        public IActionResult Create(int? id, string type)
        {
            TempData["Buton"] = "";
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            string userid = HttpContext.Session.GetString("UserId");

            ViewBag.SubAccount = _dbContext.GLSubAccounts.Where(x=>x.IsActive==true && x.IsApproved==true).ToList();
           
            //ViewBag.BankCashAccounts;
            var SubAccounts = _dbContext.GLSubAccountDetails.Where(x=>x.Description!="" ).ToList();
            var Costcenters = _dbContext.CostCenter.Where(x => x.IsDeleted == false && x.IsActive == true && x.IsApproved == true).ToList();
            var Departments = _dbContext.GLDivision.Where(x => x.IsDeleted == false && x.IsActive == true && x.IsApproved == true).ToList();
            var SubDepartments = _dbContext.GLSubDivision.Where(x => x.IsDeleted == false ).ToList();

            //Sub Accounts

            GLSubAccountDetail subaccount = new GLSubAccountDetail(); 
            subaccount.Id = 0;
            subaccount.Description = "";
            SubAccounts.Add(subaccount);

            //costcenter
            CostCenter costCenter = new CostCenter();
            GLDivision depart = new GLDivision();
            GLSubDivision subdepart = new GLSubDivision();
            costCenter.Id = 0;
            costCenter.Description = "";
            Costcenters.Add(costCenter);
            //department
            depart.Id = 0;
            depart.Name = "";
            Departments.Add(depart);
            //sub depart
            subdepart.Id = 0;
            subdepart.Name = "";
            SubDepartments.Add(subdepart);

            ViewBag.Costcenters = Costcenters.OrderBy(x => x.Id);
            ViewBag.Departments = Departments.OrderBy(x => x.Id);
            ViewBag.SubDepartments = SubDepartments.OrderBy(x => x.Id);

            var APLC = _dbContext.APLC.Where(x => !x.IsDeleted).ToList();
            APLC aPLCs = new APLC();
            aPLCs.Id = 0;
            aPLCs.LCNo = "";
            APLC.Add(aPLCs);
            //ViewBag.LC = APLC.OrderBy(x => x.Id);
            ViewBag.LC = SubAccounts.OrderBy(x => x.Id);
            //ViewBag.Branch = new CommonDDL(_dbContext).GetBranchesbyId(userid);
            //new voucher return voucher type 
            if (id == null || id == 0)
            {
                VoucherHelper helper = new VoucherHelper(_dbContext, HttpContext);


                ViewBag.EntityState = "Submit";
                ViewBag.BankAccounts = (new SelectList(_dbContext.GLBankCashAccounts.Where(x=> x.IsActive == true && x.IsApproved == true && x.CompanyId == companyId && (x.PaymentVoucherType == type || x.VoucherType == type))
                           //.Where(b => && )
                           .Where(b => b.AccountName.Contains(""))
                           .Select(b => new
                           {
                               id = b.Id,
                               text = string.Concat(b.AccountNumber, " - ", b.AccountName)
                           })
                           .OrderBy(a => a.text)
                           //.Take(25)
                           .ToList(), "id", "text"));
                var voucherType = _dbContext.GLVoucherTypes
                             .Where(v =>  v.VoucherType == type && v.IsActive == true && v.IsSystem == false).FirstOrDefault();
                if (voucherType == null)
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Invalid Voucher type";
                    return Redirect("/Dashboard/Index");
                }
                else
                {

                    ViewBag.VoucherType = voucherType.VoucherType;
                    ViewBag.VoucherTypeDescription = voucherType.Description;
                    ViewBag.NavbarHeading = voucherType.Description;
                    ViewBag.VoucherTypeReferenceNarration = voucherType.ReferenceNarration;
                    ViewBag.VoucherDetail = "[]";
                    var voucher = new GLVoucher();
                    voucher.VoucherDate = DateTime.Now;
                    int periodId = helper.GetPeriodId(voucher.VoucherDate);
                    voucher.VoucherType = voucherType.VoucherType;
                    //voucher.VoucherNo = helper.GetNewVoucherNo(voucher.VoucherType, periodId);
                    voucher.Currencies = AppCurrencyRepo.GetCurrencies();

                    return View(voucher);
                }
            }
            else
            {
                //ViewBag.EntityState = "Update";
                //ViewBag.TitleStatus = "Created";
                var voucher = _dbContext.GLVouchers.Find(id);
                voucher.Currencies = AppCurrencyRepo.GetCurrencies();
                var voucherDetail = (from d in _dbContext.GLVoucherDetails
                                     join a in _dbContext.GLAccounts on d.AccountId equals a.Id
                                     select new
                                     {
                                         Id = d.Id,
                                         VoucherId = d.VoucherId,
                                         Sequence = d.Sequence,
                                         AccountId = a.Id,
                                         AccountName = string.Concat(a.Code, " - ", a.Name),
                                         SubAccountId = d.SubAccountId,
                                         Debit = d.Debit,
                                         Credit = d.Credit,
                                         SubAccountIdName = d.SubAccountIdName,
                                         CostCenterName = d.CostCenterName,
                                         Description = d.Description,
                                         IsDeleted = d.IsDeleted,
                                         DepartmentId = d.DepartmentId,
                                         SubDepartmentId = d.SubDepartmentId,
                                     }
                                 )
                                 .Where(v => v.IsDeleted == false && v.VoucherId == id && v.Sequence != 555)
                                 .OrderByDescending(v => v.Sequence)
                                 .ThenBy(v => v.Id)
                                 .ToArray();

                if (voucher.VoucherType == "CRV" || voucher.VoucherType == "BRV")
                {
                    voucherDetail = (from d in _dbContext.GLVoucherDetails
                                     join a in _dbContext.GLAccounts on d.AccountId equals a.Id
                                     select new
                                     {
                                         Id = d.Id,
                                         VoucherId = d.VoucherId,
                                         Sequence = d.Sequence,
                                         AccountId = a.Id,
                                         AccountName = string.Concat(a.Code, " - ", a.Name),
                                         SubAccountId = d.SubAccountId,
                                         Debit = d.Credit,
                                         Credit = d.Credit,
                                         SubAccountIdName = d.SubAccountIdName,
                                         CostCenterName = d.CostCenterName,
                                         Description = d.Description,
                                         IsDeleted = d.IsDeleted,
                                         DepartmentId = d.DepartmentId,
                                         SubDepartmentId = d.SubDepartmentId,
                                     }
                                )
                                .Where(v => v.IsDeleted == false && v.VoucherId == id && v.Sequence != 555)
                                .OrderBy(v => v.Sequence)
                                .ThenBy(v => v.Id)
                                .ToArray();
                }
                else if (voucher.VoucherType == "CPV" || voucher.VoucherType == "BPV")
                {
                    voucherDetail = (from d in _dbContext.GLVoucherDetails
                                     join a in _dbContext.GLAccounts on d.AccountId equals a.Id
                                     select new
                                     {
                                         Id = d.Id,
                                         VoucherId = d.VoucherId,
                                         Sequence = d.Sequence,
                                         AccountId = a.Id,
                                         AccountName = string.Concat(a.Code, " - ", a.Name),
                                         SubAccountId = d.SubAccountId,
                                         Debit = d.Debit,
                                         Credit = d.Credit,
                                         SubAccountIdName = d.SubAccountIdName,
                                         CostCenterName = d.CostCenterName,
                                         Description = d.Description,
                                         IsDeleted = d.IsDeleted,
                                         DepartmentId = d.DepartmentId,
                                         SubDepartmentId = d.SubDepartmentId,
                                     }
                                )
                                .Where(v => v.IsDeleted == false && v.VoucherId == id && v.Sequence != 555)
                                .OrderBy(v => v.Sequence)
                                .ThenBy(v => v.Id)
                                .ToArray();
                }
                ViewBag.VoucherDetail = JsonConvert.SerializeObject(voucherDetail);

                var voucherType = _dbContext.GLVoucherTypes
                             .Where(v =>  v.VoucherType == voucher.VoucherType && v.IsActive == true && v.IsSystem == false).FirstOrDefault();
                 
                ViewBag.BankAccounts = (new SelectList(_dbContext.GLBankCashAccounts
                            .Where(b => b.CompanyId == companyId && b.IsApproved == true && b.IsActive == true && (b.PaymentVoucherType == voucher.VoucherType || b.VoucherType == voucher.VoucherType))
                            .Where(b => b.AccountName.Contains(""))
                            .Select(b => new
                            {
                                id = b.Id,
                                text = string.Concat(b.AccountNumber, " - ", b.AccountName)
                            })
                            .OrderBy(a => a.text)
                            //.Take(25)
                            .ToList(), "id", "text"));

                ViewBag.VoucherType = voucherType.VoucherType;
                ViewBag.VoucherTypeDescription = voucherType.Description;
                ViewBag.NavbarHeading = voucherType.Description;
                ViewBag.VoucherTypeReferenceNarration = voucherType.ReferenceNarration;
                if (voucher.Status != "Approved")
                {
                    ViewBag.EntityState = "Update";
                    ViewBag.TitleStatus = "Created";
                }
                //else if (voucher.Status == "Approved")
                //{
                //    ViewBag.TitleStatus = "Approved";
                //}
                return View(voucher);
            }
        }

        // POST: GLVoucher/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Post(int id, VoucherViewModel viewModal, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var count = 0;
            //VoucherViewModel model = new VoucherViewModel();
            //_dbContext.Database.BeginTransaction();
            GLVoucher voucher;
            VoucherHelper helper = new VoucherHelper(_dbContext, HttpContext);

            int periodId = helper.GetPeriodId(viewModal.VoucherDate);
            if (periodId == 0)
            {
                TempData["error"] = "true";
                TempData["message"] = "Period is not define or has been closed for entry.";
                return (RedirectToAction(nameof(Create)));
            }


            if (id == 0)//New Voucher
            {
                voucher = new GLVoucher();
                var voucherNo = helper.GetNewVoucherNo(viewModal.VoucherType, periodId);
                voucher.VoucherNo = voucherNo;
            }
            else
            {
                voucher = _dbContext.GLVouchers.Find(id);
            }

            voucher.VoucherDate = viewModal.VoucherDate;
            // voucher.BranchId = viewModal.BranchId;
            voucher.PeriodId = periodId;
            voucher.VoucherType = viewModal.VoucherType;
            voucher.Reference = viewModal.Reference;
            voucher.Description = viewModal.Description;
            voucher.Currency = viewModal.Currency;
            voucher.Amount = viewModal.Amount;
            voucher.CurrencyExchangeRate = viewModal.CurrencyExchangeRate;
            voucher.IsDeleted = false;
            voucher.CompanyId = companyId;
            voucher.CreatedBy = userId;
            voucher.CreatedDate = DateTime.Now;
            voucher.Status = "Created";
            voucher.BankCashAccountId = viewModal.BankCashAccountId;

            if (id == 0) // New Voucher
            { _dbContext.GLVouchers.Add(voucher); }
            else
            {
                var entry = _dbContext.GLVouchers.Update(voucher);
                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            }
            await _dbContext.SaveChangesAsync();
            int voucherId = voucher.Id;

            //Create Voucher Detail

            VoucherDetailViewModel[] voucherDetail = JsonConvert.DeserializeObject<VoucherDetailViewModel[]>(viewModal.VoucherDetail);
            foreach (var item in voucherDetail)
            {
                if (item.Id != 0) // New Voucher Detail Line
                {

                    if (count == 0)
                    {
                        GLVoucherDetail detail = new GLVoucherDetail();
                        //string[] account = item.AccountName.Split("-");
                        //string accountCode = account[0].Trim();
                        detail = _dbContext.GLVoucherDetails.Where(x => x.IsDeleted == false && x.VoucherId == voucher.Id && x.Sequence == Convert.ToInt16(555)).FirstOrDefault();

                        item.AccountId = _dbContext.GLBankCashAccounts
                                                   .Where(a => a.CompanyId == companyId && a.Id == voucher.BankCashAccountId)
                                                   .FirstOrDefault().AccountId;

                        detail.AccountId = item.AccountId;
                        detail.VoucherId = voucherId;
                        detail.Sequence = Convert.ToInt16(555);
                        detail.SubAccountId = item.SubAccountId;
                        detail.Description = item.Description;
                        if (voucher.VoucherType == "CRV" || voucher.VoucherType == "BRV")
                        {
                            detail.Debit = voucher.Amount;
                            detail.Credit = 0;
                        }
                        else
                        {
                            detail.Debit = 0;
                            detail.Credit = voucher.Amount;
                        }
                        detail.SubAccountIdName = item.SubAccountIdName;
                        detail.CostCenterName = item.CostCenterName;
                        detail.DepartmentId = item.DepartmentId;
                        detail.SubDepartmentId = item.SubDepartmentId;
                        detail.UpdatedBy = userId;
                        detail.UpdatedDate = DateTime.Now;
                        detail.IsDeleted = false;
                        var ent = _dbContext.GLVoucherDetails.Update(detail);
                        ent.OriginalValues.SetValues(await ent.GetDatabaseValuesAsync());
                        count++;
                    }
                    GLVoucherDetail detail2 = new GLVoucherDetail();
                    string[] account = item.AccountName.Split("-");
                    string accountCode = account[0].Trim();
                    detail2 = _dbContext.GLVoucherDetails.Find(item.Id);

                    item.AccountId = _dbContext.GLAccounts
                               .Where(a => a.Code == accountCode && a.IsDeleted == false)
                               .FirstOrDefault().Id;

                    detail2.AccountId = item.AccountId;
                    detail2.VoucherId = voucherId;
                    detail2.Sequence = item.Sequence;
                    detail2.SubAccountId = item.SubAccountId;
                    detail2.Description = item.Description;
                    if (voucher.VoucherType == "CRV" || voucher.VoucherType == "BRV")
                    {
                        detail2.Debit = 0;
                        detail2.Credit = item.Debit; ;
                    }
                    else
                    {
                        detail2.Debit = item.Debit; ;
                        detail2.Credit = 0;

                    }
                    //detail2.Debit = item.Debit;
                    //detail2.Credit= 0;
                    detail2.SubAccountIdName = item.SubAccountIdName;
                    detail2.CostCenterName = item.CostCenterName;
                    detail2.DepartmentId = item.DepartmentId;
                    detail2.SubDepartmentId = item.SubDepartmentId;
                    detail2.UpdatedBy = userId;
                    detail2.UpdatedDate = DateTime.Now;
                    detail2.IsDeleted = false;
                    var entry = _dbContext.GLVoucherDetails.Update(detail2);
                    entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());


                }
                else
                {
                    //if (voucher.VoucherType == "BPV" || voucher.VoucherType == "CPV")
                    //{
                    if (count == 0)
                    {
                        GLVoucherDetail detail = new GLVoucherDetail();
                        //get AccountId from AccountName using split [AccountCode] - [AccountName]
                        //string[] account = item.AccountName.Split("-");
                        //string accountCode = account[0].Trim();
                        item.AccountId = _dbContext.GLBankCashAccounts
                                        .Where(a => a.CompanyId == companyId && a.Id == voucher.BankCashAccountId)
                                        .FirstOrDefault().AccountId;

                        detail.AccountId = item.AccountId;
                        detail.VoucherId = voucherId;
                        detail.Sequence = Convert.ToInt16(555);
                        detail.SubAccountId = item.SubAccountId;
                        detail.Description = item.Description;
                        if (voucher.VoucherType == "CRV" || voucher.VoucherType == "BRV")
                        {
                            detail.Debit = voucher.Amount;
                            detail.Credit = 0;
                        }
                        else
                        {
                            detail.Debit = 0;
                            detail.Credit = voucher.Amount;
                        }
                        //  detail.Credit = Convert.ToDecimal(collection["TCredit"]);
                        detail.Account = null;
                        detail.SubAccountIdName = item.SubAccountIdName;
                        detail.CostCenterName = item.CostCenterName;
                        detail.DepartmentId = item.DepartmentId;
                        detail.SubDepartmentId = item.SubDepartmentId;
                        detail.CreatedBy = userId;
                        detail.CreatedDate = DateTime.Now;
                        detail.IsDeleted = false;
                        _dbContext.GLVoucherDetails.Add(detail);
                        count++;
                    }
                    GLVoucherDetail detail2 = new GLVoucherDetail();
                    //get AccountId from AccountName using split [AccountCode] - [AccountName]
                    string[] account = item.AccountName.Split("-");
                    string accountCode = account[0].Trim();
                    item.AccountId = _dbContext.GLAccounts
                                    .Where(a => a.Code == accountCode && a.IsDeleted == false)
                                    .FirstOrDefault().Id;

                    //detail.AccountId = voucher.BankCashAccountId;
                    detail2.AccountId = item.AccountId;
                    detail2.Account = null;
                    detail2.VoucherId = voucherId;
                    detail2.Sequence = item.Sequence;
                    detail2.SubAccountId = item.SubAccountId;
                    detail2.Description = item.Description;
                    if (voucher.VoucherType == "CRV" || voucher.VoucherType == "BRV")
                    {
                        detail2.Debit = 0;
                        detail2.Credit = item.Debit;
                    }
                    else
                    {
                        detail2.Debit = item.Debit; ;
                        detail2.Credit = 0;
                    }
                    //detail2.Debit = item.Debit;
                    //detail2.Credit = 0;
                    detail2.SubAccountIdName = item.SubAccountIdName;
                    detail2.CostCenterName = item.CostCenterName;
                    detail2.DepartmentId = item.DepartmentId;
                    detail2.SubDepartmentId = item.SubDepartmentId;
                    detail2.CreatedBy = userId;
                    detail2.CreatedDate = DateTime.Now;
                    detail2.IsDeleted = false;
                    _dbContext.GLVoucherDetails.Add(detail2);
                    //}


                }
                await _dbContext.SaveChangesAsync();

            }



            //Delete any voucher line which doesn't match
            if (viewModal.DeletedVoucherDetail != null)
            {
                var deletedVoucherDetail = viewModal.DeletedVoucherDetail.Split(",");
                foreach (var item in deletedVoucherDetail)
                {
                    GLVoucherDetail deletedDetail = _dbContext.GLVoucherDetails
                                                    .Where(d => d.VoucherId == voucher.Id && d.Id == Convert.ToInt32(item)).FirstOrDefault();
                    deletedDetail.IsDeleted = true;
                    deletedDetail.UpdatedBy = userId;
                    deletedDetail.UpdatedDate = DateTime.Now;
                    var entry = _dbContext.GLVoucherDetails.Update(deletedDetail);
                    entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());

                }
            }
            var vDetail = _dbContext.GLVoucherDetails.Where(d => d.VoucherId == voucherId && d.IsDeleted == false).Sum(s => s.Debit);
            var voucherAmount = _dbContext.GLVouchers.Find(voucherId);
            voucherAmount.Amount = vDetail;
            _dbContext.GLVouchers.Update(voucherAmount);
            await _dbContext.SaveChangesAsync();

            TempData["error"] = "false";
            TempData["message"] = string.Format("Voucher No. {0} has been saved successfully.", voucher.VoucherNo);
            var buttonType = Convert.ToString(collection["buttonType"]);
            if (buttonType == "SaveNew")
            {
                return (RedirectToAction(nameof(Create), new { id = 0, type = voucher.VoucherType }));
            }

            return (RedirectToAction(nameof(Create), new { id = voucher.Id }));

        }

        public IActionResult GetGLVouchers(string type)
        {
            try
            {
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                int respId = HttpContext.Session.GetInt32("Resp_ID").Value;
                string userId = HttpContext.Session.GetString("UserId");
                var approve = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == respId).FirstOrDefault().Approve;
                var unApprove = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == respId).FirstOrDefault().UnApprove;

                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                var searchValueVoucherNo = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchValueVoucherDate = Request.Form["columns[1][search][value]"].FirstOrDefault();
                //var searchValueReference = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchValueDescription = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchValueCurrency = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchValueAmount = Request.Form["columns[4][search][value]"].FirstOrDefault();
                var searchCreatedBy = Request.Form["columns[5][search][value]"].FirstOrDefault();
                var searchApprovedBy = Request.Form["columns[6][search][value]"].FirstOrDefault();


                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                //var Data = (from temp in _dbContext.GLVouchers.Where(v => v.CompanyId == companyId && v.VoucherType == type && v.IsDeleted == false) select temp);
                var Data =  _dbContext.GLVouchers.Include(x => x.User).Include(x => x.ApprovalUser).Where(v => v.CompanyId == companyId && v.VoucherType == type && v.IsDeleted == false);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    Data = Data.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    Data = Data.Where(m => m.VoucherDate.ToShortDateString().Contains(searchValue)
                                                    || m.Currency.ToString().Contains(searchValue)
                                                    || m.Amount.ToString().Contains(searchValue)
                                                    || m.Status.ToString().Contains(searchValue)
                                                  );

                }

                Data = !string.IsNullOrEmpty(searchValueVoucherNo) ? Data.Where(m => m.VoucherNo.ToString().ToLower().Contains(searchValueVoucherNo.ToLower())) : Data;
                Data = !string.IsNullOrEmpty(searchValueVoucherDate) ? Data.Where(m => m.VoucherDate.ToString(Helpers.CommonHelper.DateFormat).ToLower().Contains(searchValueVoucherDate.ToLower())) : Data;
                //Data = !string.IsNullOrEmpty(searchValueReference) ? Data.Where(m => (m.Reference != null ? m.Reference.ToString().ToLower().Contains(searchValueReference.ToLower()) : false)) : Data;
                Data = !string.IsNullOrEmpty(searchValueDescription) ? Data.Where(m => (m.Description != null ? m.Description.ToString().ToLower().Contains(searchValueDescription.ToLower()) : false)) : Data;
                Data = !string.IsNullOrEmpty(searchValueCurrency) ? Data.Where(m => m.Currency.ToString().ToLower().Contains(searchValueCurrency.ToLower())) : Data;
                Data = !string.IsNullOrEmpty(searchValueAmount) ? Data.Where(m => m.Amount.ToString().ToLower().Contains(searchValueAmount.ToLower())) : Data;
                Data = !string.IsNullOrEmpty(searchCreatedBy) ? Data.Where(m => m.User.UserName.ToString().ToLower().Contains(searchCreatedBy.ToLower())) : Data;
                Data = !string.IsNullOrEmpty(searchApprovedBy) ? Data.Where(m => m.ApprovedBy != null ? m.ApprovalUser.UserName.ToString().ToUpper().Contains(searchApprovedBy.ToUpper()) : false) : Data;

                recordsTotal = Data.Count();
                var data = Data.Skip(skip).Take(pageSize).ToList();
                List<GLVoucher> Details = new List<GLVoucher>();
                foreach (var grp in data)
                {
                    GLVoucher detail = new GLVoucher();
                    detail = grp;
                    detail.Auser = grp.ApprovedBy != null ? _dbContext.Users.FirstOrDefault(x => x.Id == grp.ApprovedBy).UserName : "";

                    detail.shortdate = grp.VoucherDate.ToString(Helpers.CommonHelper.DateFormat);
                    detail.Description = grp.Description;
                    detail.CreatedBy = _dbContext.Users.FirstOrDefault(x => x.Id == grp.CreatedBy).FullName;
                    detail.Approve = approve;
                    detail.Unapprove = unApprove;
                    Details.Add(detail);

                }
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = Details.OrderByDescending(x=>x.VoucherDate).OrderByDescending(x=>x.VoucherNo) };
                return Ok(jsonData);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IActionResult> DeleteVoucher(int id)
        {
            var voucher = _dbContext.GLVouchers
                .Where(v => v.IsDeleted == false && v.Id == id && v.Status == "Created").FirstOrDefault();
            if (voucher == null)
            {

                TempData["error"] = "true";
                TempData["message"] = "Voucher not found";
            }
            else
            {
                voucher.IsDeleted = true;
                var entry = _dbContext.GLVouchers.Update(voucher);
                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                await _dbContext.SaveChangesAsync();

                TempData["error"] = "false";
                TempData["message"] = string.Format("Voucher Id. {0} has been deleted successfully", voucher.Id);
            }
            return RedirectToAction(nameof(Approve));
        }
        public IActionResult SearchFilter(string searchDate, string type)
        {
            return View("Index");
        }

        [HttpGet]
        public IActionResult PreviewVoucher(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var voucher = _dbContext.GLVouchers.Where(v => v.Id == id && v.IsDeleted == false && v.CompanyId == companyId)
                .FirstOrDefault();
            var voucherDetails = _dbContext.GLVoucherDetails.Include(v => v.Voucher).Include(v => v.Account)
                .Where(v => v.VoucherId == id && v.IsDeleted == false).OrderByDescending(x=>x.Debit).ToList();
            
            TempData["Details"] = voucherDetails;
            return PartialView("_VoucherDetailPopup", voucher);

        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            GLVoucherViewModel gLVoucherViewModel = new GLVoucherViewModel();
            gLVoucherViewModel.GLVouchers = _dbContext.GLVouchers
                .Include(i => i.BankCashAccount)
                .Where(i => i.Id == id)
                .FirstOrDefault();
  
            var voucherViewModel =
                  (from v in _dbContext.GLVouchers.Where(x => x.IsDeleted == false)

                   join vd in _dbContext.GLVoucherDetails.Include(x => x.Account).Where(v => v.VoucherId == id && v.IsDeleted == false && v.Sequence != 555).Where(x => x.IsDeleted == false) on v.Id equals vd.VoucherId into Ass
                   from vd in Ass

                   join sa in _dbContext.GLSubAccountDetails on vd.SubAccountIdName equals sa.Id into sas
                     //from sdResult in sds.DefaultIfEmpty()
                     from sa in sas.DefaultIfEmpty()
                   join d in _dbContext.GLDivision on vd.DepartmentId equals d.Id into sds
                     //from sdResult in sds.DefaultIfEmpty()
                     from d in sds.DefaultIfEmpty()

                   join sd in _dbContext.GLSubDivision on vd.SubDepartmentId equals sd.Id into wes
                   from sd in wes.DefaultIfEmpty()

                   join c in _dbContext.CostCenter on vd.CostCenterName equals c.Id into sqs
                   from c in sqs.DefaultIfEmpty()

                   orderby v.Id
                   // from As in Ass.DefaultIfEmpty()
                   select new GLVoucherViewModel
                   {
                       GLVouchers = v,
                       Account = vd.Account.Name,
                       VoucherType = v.VoucherType,
                       Seq = vd.Sequence.ToString(),
                       subAccount = sa.Description,
                       Department = d.Description,
                       subDepartment = sd.Name,
                       CostCenter = c.Description,
                       Debit = vd.Debit,
                       Credit = vd.Credit,
                       Difference = 0
                   }).Distinct().ToList();

            //if (voucherViewModel.v == "CPV" || gLVoucherViewModel.GLVouchers.VoucherType == "CRV")
            //{
            //    gLVoucherViewModel.Debit = gLVoucherViewModel.GLVoucherDetails.Sum(x => x.Debit);
            //    gLVoucherViewModel.Credit = gLVoucherViewModel.GLVoucherDetails.Sum(x => x.Debit);
            //    gLVoucherViewModel.Difference = gLVoucherViewModel.Debit - gLVoucherViewModel.Credit;
            //}
            //else
            //{
            //    gLVoucherViewModel.Debit = gLVoucherViewModel.GLVoucherDetails.Sum(x => x.Credit);
            //    gLVoucherViewModel.Credit = gLVoucherViewModel.GLVoucherDetails.Sum(x => x.Credit);
            //    gLVoucherViewModel.Difference = gLVoucherViewModel.Debit - gLVoucherViewModel.Credit;
            //}


            ViewBag.NavbarHeading = "Voucher Detail";
            ViewBag.TitleStatus = "Approved";

            return View(voucherViewModel);
        }
    }
}