using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Helpers;
using Numbers.Repository.Setup;
using System.Linq.Dynamic.Core;
namespace Numbers.Areas.GL.Controllers
{
    [Authorize]
    [Area("GL")]
    public class VoucherController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        private readonly IWebHostEnvironment hostingEnvironment;
        public VoucherController(NumbersDbContext dbContext, IWebHostEnvironment hostingEnvironment)
        {
            _dbContext = dbContext;
            this.hostingEnvironment = hostingEnvironment;
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
                .Include(p=>p.GLVoucherDetail)
                .Where(v => v.VoucherType == type && v.CompanyId == companyId && v.IsDeleted == false && v.VoucherDate.Month == month);
            //.Include(d => d.GLVoucherDetail)
            //.OrderByDescending(v => v.VoucherNo);
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            string userid = HttpContext.Session.GetString("UserId");
            ViewBag.Approve = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userid && x.IsDeleted == false && x.ResponsibilityId == resp_Id).FirstOrDefault().Approve;
            return View(vouchers);

        }
        public IActionResult Approve()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string configValues = _dbContext.AppCompanyConfigs
              .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
              .Select(c => c.ConfigValue)
              .FirstOrDefault();
            ViewBag.ReportPath = string.Concat(configValues, "Viewer", "?Report=Voucher&cId=", companyId, "&id={0}");
            var voucher = _dbContext.GLVouchers
               .Where(v => v.Status == "Created" && v.IsDeleted == false && v.CompanyId == HttpContext.Session.GetInt32("CompanyId") && v.VoucherType == "JV"
               && v.IsDeleted == false);
            ViewBag.NavbarHeading = "Approve Voucher";
            return View(voucher);
        }
        public ActionResult UnApprove()
        {
            try
            {
                var voucher = _dbContext.GLVouchers
                   .Where(v => v.Status == "Approved" && v.IsSystem==false && v.IsDeleted == false && v.CompanyId == HttpContext.Session.GetInt32("CompanyId").Value
                   && v.IsDeleted == false);
                ViewBag.NavbarHeading = "Un-Approve Voucher";
                return View(voucher);
            }
            catch(Exception exc)
            {
                TempData["error"] = "true";
                TempData["message"] = exc.Message == null ? exc.InnerException.Message.ToString() : exc.Message.ToString();
                return RedirectToAction("Index","Dashboard");
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
            string userId = HttpContext.Session.GetString("CompanyId");
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
            string route = string.Format("/GL/Voucher/Index?type={0}", voucher.VoucherType);
            return Redirect(route);
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
            string route = string.Format("/GL/Voucher/Index?type={0}", voucher.VoucherType);
            return Redirect(route);
        }
        [HttpGet]
        public IActionResult Create(int? id, string type)
        {
            VoucherHelper helper = new VoucherHelper(_dbContext, HttpContext);
            ViewBag.SubAccount=  _dbContext.GLSubAccounts.ToList();
            ViewBag.SubAccounts =  _dbContext.GLSubAccountDetails.ToList();
            var SubAccounts = _dbContext.GLSubAccountDetails.Where(x => x.Description != "").ToList();
            var Costcenters = _dbContext.CostCenter.Where(x => x.IsDeleted == false && x.IsActive == true && x.IsApproved == true).ToList();
            var Departments = _dbContext.GLDivision.Where(x => x.IsDeleted == false && x.IsActive == true && x.IsApproved == true).ToList();
            var SubDepartments = _dbContext.GLSubDivision.Where(x => x.IsDeleted == false).ToList();

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
            ViewBag.Costcenters = Costcenters.OrderBy(x=>x.Id);
            ViewBag.Departments = Departments.OrderBy(x => x.Id);
            ViewBag.SubDepartments = SubDepartments.OrderBy(x => x.Id);
            ViewBag.SubAccounts = SubAccounts.OrderBy(x => x.Id);
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            //new voucher return voucher type 

            if (id == null || id == 0)
            {
                TempData["View"] = "Create";
                ViewBag.EntityState = "Save";
                var voucherType = _dbContext.GLVoucherTypes
                             .Where(v => v.VoucherType == type && v.IsActive == true && v.IsSystem == false).FirstOrDefault();
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
                    voucher.VoucherNo = helper.GetNewVoucherNo(voucher.VoucherType, periodId);
                    voucher.Currencies = AppCurrencyRepo.GetCurrencies();
                    return View(voucher);
                }
            }
            else
            {
                TempData["View"] = "Create";
                //ViewBag.EntityState = "Update";
                //ViewBag.TitleStatus = "Created";
                var voucher = _dbContext.GLVouchers.Find(id);
                voucher.Currencies = AppCurrencyRepo.GetCurrencies(); ;
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
                                         SubAccountIdName=d.SubAccountIdName,
                                         CostCenterName=d.CostCenterName,
                                         Description = d.Description,
                                         IsDeleted = d.IsDeleted,
                                         DepartmentId =d.DepartmentId,
                                         SubDepartmentId =d.SubDepartmentId,
                                     }
                              )
                              .Where(v => v.IsDeleted == false && v.VoucherId == id)
                              .OrderByDescending(v => v.Sequence)
                              .ThenBy(v => v.Id)
                              .ToArray();
                ViewBag.VoucherDetail = JsonConvert.SerializeObject(voucherDetail);

                var voucherType = _dbContext.GLVoucherTypes
                             .Where(v =>  v.VoucherType == voucher.VoucherType && v.IsActive == true && v.IsSystem == false).FirstOrDefault();

                ViewBag.VoucherType = voucherType.VoucherType;
                voucher.file = _dbContext.AppAttachments.Where(c => c.Source == "Voucher Entry" && c.SourceId == voucher.Id).Select(c => c.FileName).ToList();
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
        public async Task<IActionResult> Post(int id, VoucherViewModel viewModal,IFormCollection collection, IFormFile img)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            string uniqueFileName = null;
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
            voucher.PeriodId = periodId;
            voucher.VoucherType = viewModal.VoucherType;
            voucher.Reference = viewModal.Reference;
            voucher.Description = viewModal.Description;
            voucher.Currency = viewModal.Currency;
            voucher.CurrencyExchangeRate = viewModal.CurrencyExchangeRate;
            voucher.IsDeleted = false;
            voucher.CompanyId = companyId;
            voucher.CreatedBy = userId;
            voucher.CreatedDate = DateTime.Now;
            voucher.Status = "Created";

            if (id == 0) // New Voucher
            { _dbContext.GLVouchers.Add(voucher); }
            else
            {
                var entry = _dbContext.GLVouchers.Update(voucher);
                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            }
            await _dbContext.SaveChangesAsync();
            int voucherId = voucher.Id;  
            if (viewModal.Attachments != null && viewModal.Attachments.Count > 0)
            {
                var oldattach = _dbContext.AppAttachments.Where(c => c.Source == "Voucher Entry" && c.SourceId == voucherId).ToList();
                foreach (var item in oldattach)
                {
                    _dbContext.AppAttachments.Remove(item);
                    _dbContext.SaveChanges();
                }

                // Loop thru each selected file
                foreach (IFormFile photo in viewModal.Attachments)
                {
                    // The file must be uploaded to the images folder in wwwroot
                    // To get the path of the wwwroot folder we are using the injected
                    // IHostingEnvironment service provided by ASP.NET Core
                    string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "img");
                    // To make sure the file name is unique we are appending a new
                    // GUID value and and an underscore to the file name
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    // Use CopyTo() method provided by IFormFile interface to
                    // copy the file to wwwroot/images folder
                    photo.CopyTo(new FileStream(filePath, FileMode.Create));
                    AppAttachment appAttachment = new AppAttachment();
                    appAttachment.Source = "Voucher Entry";
                    appAttachment.SourceId = voucher.Id;
                    appAttachment.CreatedBy = userId;
                    appAttachment.CreatedDate = DateTime.Now;
                    appAttachment.FileName = uniqueFileName;
                    appAttachment.FilePath = uniqueFileName;
                    _dbContext.AppAttachments.Add(appAttachment);
                    _dbContext.SaveChanges();
                }
            }
            //Create Voucher Detail
            VoucherDetailViewModel[] voucherDetail = JsonConvert.DeserializeObject<VoucherDetailViewModel[]>(viewModal.VoucherDetail);
            foreach (var item in voucherDetail)
            {
                if (item.Id != 0) // New Voucher Detail Line
                {
                    GLVoucherDetail detail = new GLVoucherDetail();
                    detail = _dbContext.GLVoucherDetails.Find(item.Id);
                    string[] account = item.AccountName.Split("-");
                    string accountCode = account[0].Trim();
                    item.AccountId = _dbContext.GLAccounts
                                    .Where(a => a.Code == accountCode && a.IsDeleted == false)
                                    .FirstOrDefault().Id;

                    detail.AccountId = item.AccountId;
                    detail.VoucherId = voucherId;
                    detail.Sequence = item.Sequence;
                    detail.SubAccountId = item.SubAccountId;
                    detail.Description = item.Description;
                    detail.Debit = item.Debit;
                    detail.Credit = item.Credit;
                    detail.SubAccountIdName = item.SubAccountIdName;
                    detail.CostCenterName = item.CostCenterName;
                    detail.DepartmentId = item.DepartmentId;
                    detail.SubDepartmentId = item.SubDepartmentId;
                    detail.UpdatedBy = userId;
                    detail.UpdatedDate = DateTime.Now;
                    detail.IsDeleted = false;
                    var entry = _dbContext.GLVoucherDetails.Update(detail);
                    entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                }
                else
                {
                    GLVoucherDetail detail = new GLVoucherDetail();
                    //get AccountId from AccountName using split [AccountCode] - [AccountName]
                    string[] account = item.AccountName.Split("-");
                    string accountCode = account[0].Trim();
                    item.AccountId = _dbContext.GLAccounts
                                    .Where(a =>  a.Code == accountCode && a.IsDeleted == false)
                                    .FirstOrDefault().Id;
                    detail.AccountId = item.AccountId;
                    detail.VoucherId = voucherId;
                    detail.Sequence = item.Sequence;
                    detail.SubAccountId = item.SubAccountId;
                    detail.Description = item.Description;
                    detail.SubAccountIdName = item.SubAccountIdName;
                    detail.CostCenterName = item.CostCenterName;
                    detail.Debit = item.Debit;
                    detail.Credit = item.Credit;
                    detail.CreatedBy = userId;
                    detail.CreatedDate = DateTime.Now;
                    detail.IsDeleted = false;
                    detail.DepartmentId = item.DepartmentId;
                    detail.SubDepartmentId = item.SubDepartmentId;

                    _dbContext.GLVoucherDetails.Add(detail);
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

                var searchVoucherNo = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchVoucherDate = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchDescription = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchCurrency = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchAmount = Request.Form["columns[4][search][value]"].FirstOrDefault();
                var searchCreatedBy = Request.Form["columns[5][search][value]"].FirstOrDefault();
                var searchApprovedBy = Request.Form["columns[6][search][value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var Data = (from temp in _dbContext.GLVouchers.Include(x => x.User).Include(x => x.ApprovalUser).Where(v => v.CompanyId == companyId && v.VoucherType == type && v.IsDeleted == false) select temp);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    Data = Data.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                Data = !string.IsNullOrEmpty(searchVoucherNo) ? Data.Where(m => m.VoucherNo.ToString().ToLower().Contains(searchVoucherNo.ToLower())) : Data;
                Data = !string.IsNullOrEmpty(searchVoucherDate) ? Data.Where(m => m.VoucherDate.ToString(Helpers. CommonHelper.DateFormat).ToLower().Contains(searchVoucherDate.ToLower())) : Data;
                Data = !string.IsNullOrEmpty(searchDescription) ? Data.Where(m => (m.Description != null ? m.Description.ToString().ToLower().Contains(searchDescription.ToLower()) : false)) : Data;
                Data = !string.IsNullOrEmpty(searchCurrency) ? Data.Where(m => m.Currency.ToString().ToLower().Contains(searchCurrency.ToLower())) : Data;
                Data = !string.IsNullOrEmpty(searchAmount) ? Data.Where(m => m.Amount.ToString().Contains(searchAmount)) : Data;
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
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = Details };
                return Ok(jsonData);
            }
            catch (Exception e)
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
                .Where(v => v.VoucherId == id && v.IsDeleted == false).ToList();
            TempData["Details"] = voucherDetails;
            return PartialView("_VoucherDetailPopup", voucher);

        }

        public IActionResult VoucherRealizaton()
        {

            var bank =  _dbContext.GLBankCashAccounts.ToList();
            return View();

        }

        public IActionResult GetSubaccount(int id)
        {
            var accounts = _dbContext.GLAccounts.Where(x => x.Id == id).FirstOrDefault();
            if (accounts != null)
            {
                if (accounts.RequireSubAccount)
                {
                    var subaccount = _dbContext.GLSubAccountDetails.Where(x => x.SubAccountId.Id == Convert.ToInt32(accounts.SubAccountId)).ToList().Select(c =>
                   new
                   {
                       Value = c.Id,
                       Text = c.Code + " " + c.Description
                   });
                    return Json(subaccount);
                }
                else
                {
                    var subaccount = _dbContext.AppCompanyConfigs.Where(x => x.BaseId == 12 && x.Id == 79).ToList().Select(c =>
               new
               {
                   Value = 0,
                   Text = ""
               });
                    return Json(subaccount);
                }
            }
            return Ok();
        }


        //public IActionResult GetSubaccounts(int id)
        //{
        //    var subdepartments = _dbContext.GLSubAccountDetails.Where(x => x.Id == id && x.IsDelete == false).ToList();
        //    string subAccount = (from a in _dbContext.GLAccounts.Where(x => x.Id == id && x.RequireSubAccount == true) select a.SubAccountId).FirstOrDefault();
        //    string subAccountCostCenter = (from a in _dbContext.GLAccounts.Where(x => x.Id == id && x.RequireCostCenter == true) select a.SubAccountId).FirstOrDefault();
        //    if (subAccount!=null)
        //    {
        //          subdepartments = _dbContext.GLSubAccountDetails.Where(x => x.GLSubAccountId ==Convert.ToInt32(subAccount) && x.IsDelete == false).ToList();

        //    }
        //    if (subAccountCostCenter != null)
        //    {
        //        subdepartments.FirstOrDefault().IsDelete = true;
        //    }
        //    else
        //    {
        //        subdepartments.FirstOrDefault().IsDelete = false;
        //    }
        //    return Ok(subdepartments);
        // }


        public IActionResult GetSubaccounts(int id)
        {
            var subAccountId = _dbContext.GLAccounts.Where(x => x.Id == id).FirstOrDefault().SubAccountId;
            var subccountLength = _dbContext.GLSubAccounts.Where(x => x.Id ==Convert.ToInt32(subAccountId) && x.IsActive == true && x.IsApproved == true).Count();
            var list = (from a in _dbContext.GLAccounts.Where(x => x.Id == 0)
                        from sb in _dbContext.GLSubAccountDetails.Where(x => x.GLSubAccountId == Convert.ToInt32(a.SubAccountId))
                        select new
                        {
                            Id = sb.Id,
                            Code = sb.Code,
                            Description = sb.LCID != 0 ? _dbContext.APLC.Where(x => x.Id == sb.LCID).Select(x => x.LCNo).FirstOrDefault() : sb.Description,
                            IsDelete = sb.IsDelete,
                            RequireSubAccount = a.RequireSubAccount,
                            RequireCostCenter = a.RequireCostCenter
                        }).ToList();

            if (subccountLength != 0)
            {
                var subdepartments = _dbContext.GLSubAccountDetails.Where(x => x.Id == id && x.IsDelete == false).ToList();
                string subAccount = (from a in _dbContext.GLAccounts.Where(x => x.Id == id && x.RequireSubAccount == true) select a.SubAccountId).FirstOrDefault();
                /*,*/
                  list = (from a in _dbContext.GLAccounts.Where(x => x.Id == id)
                            from sb in _dbContext.GLSubAccountDetails.Where(x => x.GLSubAccountId == Convert.ToInt32(a.SubAccountId))
                            select new
                            {
                                Id = sb.Id,
                                Code = sb.Code,
                                Description = sb.LCID != 0 ? _dbContext.APLC.Where(x => x.Id == sb.LCID).Select(x => x.LCNo).FirstOrDefault() : sb.Description,
                                IsDelete = sb.IsDelete,
                                RequireSubAccount = a.RequireSubAccount,
                                RequireCostCenter = a.RequireCostCenter
                            }).ToList();
            }
            /*foreach(var lc in list)
            {
                if (lc.LC != 0)
                {
                    lc.Description = _dbContext.APLC.Where(x => x.Id == lc.LC).Select(x => x.LCNo).FirstOrDefault();
                }
            }
             */
            return Ok(list);
        }

        public IActionResult GetCostCenter(int id)
        {
            var accounts = _dbContext.GLAccounts.Where(x => x.Id == id).FirstOrDefault();
            if (accounts != null)
            {
                if (accounts.RequireCostCenter)
                {
                    var subaccount = _dbContext.CostCenter.Where(x => x.IsDeleted == false).ToList().Select(c =>
                new
                {
                    Value = c.Id,
                    Text = c.Description 
                });
                    return Json(subaccount);
                }
                else
                {
                    var subaccount = _dbContext.AppCompanyConfigs.Where(x => x.BaseId == 12 && x.Id == 79).ToList().Select(c =>
                new
                {
                    Value = 0,
                    Text = ""
                });
                    return Json(subaccount);
                }
            }
            return Ok();
        }
         
        public IActionResult GetSubaccountById(int id)
        {
           
              var  subdepartments = _dbContext.GLSubAccountDetails.Where(x => x.Id == id ).ToList();
            
            return Ok(subdepartments);
        }

        //public IActionResult GetDepartment(int id)
        //{
        //    var departments = _dbContext.GLDivision.Where(x => x.Id == id && x.IsDeleted == false).Select(c=>c.Name).FirstOrDefault();

        //    return Ok(departments);
        //}

        public IActionResult GetSubDepartment(int id)
        {
            var subdepartments = _dbContext.GLSubDivision.Where(x => x.GLDivisionId == id && x.IsDeleted == false).ToList();

            return Ok(subdepartments);

        }
        public IActionResult GetSubDepartmentById(int id)
        {
            var subdepartments = _dbContext.GLSubDivision.Where(x => x.Id == id && x.IsDeleted == false).ToList();

            return Ok(subdepartments);

        }
        public IActionResult GetDepartmentById(int id)
        {
            var subdepartments = _dbContext.GLDivision.Where(x => x.Id == id && x.IsDeleted == false).ToList();

            return Ok(subdepartments);

        }

        public IActionResult GetCostCentersBySubdiv(int id)
        {
            var subdepartments = _dbContext.CostCenter.Where(x => x.SubDivisionId == id && x.IsDeleted == false && x.IsActive==true && x.IsApproved==true).ToList();

            return Ok(subdepartments);

        }
        public IActionResult GetCostCenters(int id)
        {
            var subdepartments = _dbContext.CostCenter.Where(x => x.Id == id && x.IsDeleted == false).ToList();

            return Ok(subdepartments);

        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            GLVoucherViewModel gLVoucherViewModel = new GLVoucherViewModel();
            gLVoucherViewModel.GLVouchers = _dbContext.GLVouchers
                //.Include(i => i.BankCashAccount)
                .Where(i => i.Id == id)
                .FirstOrDefault();

            gLVoucherViewModel.GLVoucherDetails = _dbContext.GLVoucherDetails
                .Include(x => x.Account)
                .Where(v => v.VoucherId == id && v.IsDeleted == false && v.Sequence != 555)
                .ToList();
                gLVoucherViewModel.Debit = gLVoucherViewModel.GLVoucherDetails.Sum(x => x.Debit);
                gLVoucherViewModel.Credit = gLVoucherViewModel.GLVoucherDetails.Sum(x => x.Credit);
                gLVoucherViewModel.Difference = gLVoucherViewModel.Debit - gLVoucherViewModel.Credit;

            ViewBag.NavbarHeading = "Voucher Detail";
            ViewBag.TitleStatus = "Approved";

            return View(gLVoucherViewModel);
        }
        public IActionResult GetList()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            int respId = HttpContext.Session.GetInt32("Resp_ID").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var approve = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == respId).FirstOrDefault().Approve;
            var unApprove = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == respId).FirstOrDefault().UnApprove;
            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchVoucherNo = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchVoucherDate = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchReference = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchDescription = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchCurrency = Request.Form["columns[4][search][value]"].FirstOrDefault();
                var searchAmount = Request.Form["columns[5][search][value]"].FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var vouchers = (from voucher in _dbContext.GLVouchers.Where(v => v.Status == "Approved" && v.IsSystem == false && v.IsDeleted == false && v.CompanyId == companyId
                   && v.IsDeleted == false) select voucher);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    vouchers = vouchers.OrderBy(sortColumn + " " + sortColumnDirection);
                }


                //if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                //{
                //    if (sortColumn == "CustomerName")
                //    {
                //        DCData = DCData.OrderBy("CustomerId" + " " + sortColumnDirection);
                //    }
                //    else
                //    {
                //        DCData = DCData.OrderBy(sortColumn + " " + sortColumnDirection);
                //    }
                //}

                vouchers = !string.IsNullOrEmpty(searchVoucherNo) ? vouchers.Where(m => m.VoucherNo.ToString().Contains(searchVoucherNo) || m.VoucherType.ToString().ToLower().Contains(searchVoucherNo.ToLower())) : vouchers;
                vouchers = !string.IsNullOrEmpty(searchVoucherDate) ? vouchers.Where(m => m.VoucherDate.ToString(Helpers.CommonHelper.DateFormat).ToLower().Contains(searchVoucherDate.ToLower())) : vouchers;
                vouchers = !string.IsNullOrEmpty(searchReference) ? vouchers.Where(m => (m.Reference != null ? m.Reference.ToString().ToLower().Contains(searchReference.ToLower()) : false)) : vouchers;
                vouchers = !string.IsNullOrEmpty(searchDescription) ? vouchers.Where(m => (m.Description != null ? m.Description.ToString().ToLower().Contains(searchDescription.ToLower()) : false)) : vouchers;
                vouchers = !string.IsNullOrEmpty(searchCurrency) ? vouchers.Where(m => m.Currency.ToString().ToLower().Contains(searchCurrency.ToLower())) : vouchers;
                vouchers = !string.IsNullOrEmpty(searchAmount) ? vouchers.Where(m => m.Amount.ToString().Contains(searchAmount)) : vouchers;

                recordsTotal = vouchers.Count();
                var data = vouchers.Skip(skip).Take(pageSize).ToList();
                List<GLVoucherViewModel> Details = new List<GLVoucherViewModel>();
                foreach (var grp in data)
                {
                    GLVoucherViewModel gLVoucherViewModel = new GLVoucherViewModel();
                    gLVoucherViewModel.VoucherDate = grp.VoucherDate.ToString(Helpers.CommonHelper.DateFormat);
                    gLVoucherViewModel.Voucher = grp.VoucherType + " - " + grp.VoucherNo;
                    gLVoucherViewModel.Approve = approve;
                    gLVoucherViewModel.Unapprove = unApprove;
                    gLVoucherViewModel.GLVouchers = grp;
                    gLVoucherViewModel.GLVouchers.Approve = approve;
                    gLVoucherViewModel.GLVouchers.Unapprove = unApprove;
                    Details.Add(gLVoucherViewModel);
                }
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = Details.OrderByDescending(x => x.VoucherDate) };
                return Ok(jsonData);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
