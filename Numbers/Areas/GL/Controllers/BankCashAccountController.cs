using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
//using Numbers.Models;
//using Numbers.ViewModels;
using Numbers.Helpers;
using Newtonsoft;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Numbers.Repository.Helpers;

namespace Numbers.Areas.GL.Controllers
{
    [Authorize]
    [Area("GL")]
   // [Route("GL/BankCashAccount")]
    public class BankCashAccountController : Controller
    {
        private readonly NumbersDbContext _dbContext;


        public BankCashAccountController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;

        }
       // [Route("Index")]
        public IActionResult Index()
        {
            IList<GLBankCashAccount> bankCashAccounts = _dbContext.GLBankCashAccounts
                .Where(x => x.CompanyId == HttpContext.Session.GetInt32("CompanyId").Value)
                .OrderByDescending(o => o.Id).ToList();
            ViewBag.NavbarHeading = "List of Bank/Cash Accounts";
            return View(bankCashAccounts);
        }
        //[Route("Create")]
        public IActionResult Create(int? id)
        {
            try
            {
                GLBankCashAccount bankCashAccount = new GLBankCashAccount();
                var voucherTypes = _dbContext.GLVoucherTypes
                    .Where((a => a.IsActive == true && a.IsSystem == false))
                    .Select(a => new { id = a.Id, text = a.VoucherType })
                    .OrderBy(a => a.text)
                    .ToList();
                ViewBag.voucherTypes = voucherTypes;
                if (id != null)
                {
                    ViewBag.EntityState = "Update";
                    ViewBag.NavbarHeading = "Update Bank/Cash Account";
                    bankCashAccount = _dbContext.GLBankCashAccounts.Find(id);
                    return View("Create", bankCashAccount);

                }
                else
                {
                    ViewBag.EntityState = "Create";
                    ViewBag.NavbarHeading = "Create Bank/Cash Account";
                    return View("Create", bankCashAccount);
                }
            }
            catch (Exception Exp)
            {
                TempData["error"] = "true";
                TempData["message"] = (Exp.InnerException == null ? Exp.Message : Exp.InnerException.Message);
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task <IActionResult> Create(GLBankCashAccount bankCashAccount)
        {
            try
            {
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                string userId = HttpContext.Session.GetString("UserId");

                if (ModelState.IsValid)
                {
                    if (bankCashAccount.Id == 0)
                    {
                        
                        //GLBankCashAccount.CompanyId = Utility.ActiveCompanyId;
                        bankCashAccount.CreatedBy = userId;
                        bankCashAccount.CompanyId = companyId;
                        bankCashAccount.CreatedDate = DateTime.Now;
                        //if AccountId == 0 then we will create auto AccountId
                        if (bankCashAccount.AccountId == 0)
                        {

                            var configvalue = new ConfigValues(_dbContext);
                            int supplierControlAccount = 0;
                           // int supplierControlAccount = configvalue.CreateGLAccountByParentCode("Bank", bankCashAccount.AccountName, companyId, userId);
                           // bankCashAccount.AccountId = supplierControlAccount;

                            if (bankCashAccount.VoucherType.Contains("CRV") && bankCashAccount.PaymentVoucherType.Contains("CPV"))
                            {
                                  supplierControlAccount = configvalue.CreateGLAccountByParentCode("Cash", bankCashAccount.AccountName, companyId, userId);
                                bankCashAccount.AccountId = supplierControlAccount;
                            }
                            else
                            {
                                supplierControlAccount = configvalue.CreateGLAccountByParentCode("Bank", bankCashAccount.AccountName, companyId, userId);
                                bankCashAccount.AccountId = supplierControlAccount;
                            }
                            

                        }
                        _dbContext.GLBankCashAccounts.Add(bankCashAccount);
                    }
                    else
                    {
                        var data = _dbContext.GLBankCashAccounts.Find(bankCashAccount.Id);
                        data.Date = bankCashAccount.Date;
                        data.AccountName = bankCashAccount.AccountName;
                        data.AccountNumber = bankCashAccount.AccountNumber;
                        data.VoucherType = bankCashAccount.VoucherType;//Receipt Voucher Type
                        data.PaymentVoucherType = bankCashAccount.PaymentVoucherType;
                        data.AccountId = bankCashAccount.AccountId;
                        data.IsActive = bankCashAccount.IsActive;
                        data.Date = bankCashAccount.Date;
                        data.UpdatedBy = userId;                      
                        data.UpdatedDate = DateTime.Now;
                        var entry = _dbContext.GLBankCashAccounts.Update(data);
                        entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                    }
                    await _dbContext.SaveChangesAsync();
                    
                    TempData["error"] = "false";
                    TempData["message"] = string.Format("Bank/Cash Account Id. {0} has been created successfully", bankCashAccount.Id);
                    return RedirectToAction("Index");
                }
                else
                {
                    if (bankCashAccount.Id != 0)
                    {
                        ViewBag.EntityState = "Update";
                        bankCashAccount = _dbContext.GLBankCashAccounts.Find(bankCashAccount.Id);
                        return View("Create", bankCashAccount);
                    }
                    else
                    {
                        ViewBag.EntityState = "Create";
                        bankCashAccount.IsActive = true;
                        return View("Create", bankCashAccount);
                    }
                }
                //return View("GLBankCashAccount/Create", GLBankCashAccount);
            }
            catch (Exception Exp)
            {
                TempData["error"] = "true"; // "true" for green bar "false" for Red bar
                TempData["message"] = (Exp.InnerException == null ? Exp.Message : Exp.InnerException.Message);

                return View("Create", bankCashAccount);
            }
        }

        [Route("Detail")]
        public IActionResult Detail(int id)
        {
            GLBankCashAccount bankCashAccount = _dbContext.GLBankCashAccounts
                .Include(b => b.Account).Where(x => x.Id == id).FirstOrDefault();
            return View(bankCashAccount);
        }

        public IActionResult GetBankCash()
        {
            try
            {
                int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
                string userId = HttpContext.Session.GetString("UserId");
                var approve = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == resp_Id).FirstOrDefault().Approve;
                var unApprove = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == resp_Id).FirstOrDefault().UnApprove;

                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                //var searchValue = Request.Form["search[value]"].FirstOrDefault();
                var searchId = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchDate = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchAccountName = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchAccountNumber = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchRecieptVoucher = Request.Form["columns[4][search][value]"].FirstOrDefault();
                var searchPaymentVoucher = Request.Form["columns[5][search][value]"].FirstOrDefault();
                var searchStatus = Request.Form["columns[6][search][value]"].FirstOrDefault();


                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
               // var Data = (from temp in _dbContext.GLBankCashAccounts.Include(x=>x.Account).Where(x => x.CompanyId==companyId) select temp);

                var Data = (from temp in _dbContext.GLBankCashAccounts.Include(x => x.Account).Where(x => x.CompanyId == companyId)
                                           select new GLBankCashAccount
                                           {
                                               Id = temp.Id,
                                               AccountName = temp.Account.Code + "-"+ temp.Account.Name,
                                               Date = temp.Date,
                                               AccountNumber = temp.AccountNumber,
                                               VoucherType = temp.VoucherType,
                                               PaymentVoucherType = temp.PaymentVoucherType,
                                               Status = temp.Status,
                                               IsActive = temp.IsActive
                                           });


                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    Data = Data.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                
                Data = !string.IsNullOrEmpty(searchId) ? Data.Where(m => m.Id.ToString().Contains(searchId)) : Data;
                Data = !string.IsNullOrEmpty(searchDate) ? Data.Where(m => m.Date.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchDate.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchAccountName) ? Data.Where(m => m.AccountName.ToString().ToUpper().Contains(searchAccountName.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchAccountNumber) ? Data.Where(m => m.AccountNumber.ToString().Contains(searchAccountNumber)) : Data;
                Data = !string.IsNullOrEmpty(searchRecieptVoucher) ? Data.Where(m => m.VoucherType.ToString().ToUpper().Contains(searchRecieptVoucher.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchPaymentVoucher) ? Data.Where(m => m.PaymentVoucherType.ToString().ToUpper().Contains(searchPaymentVoucher.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchStatus) ? Data.Where(m => (m.Status != null ? m.Status.ToString().ToUpper().Contains(searchStatus.ToUpper()) : m.Id.ToString().Contains(searchId))) : Data;

                recordsTotal = Data.Count();
                var data = Data.Skip(skip).Take(pageSize).ToList();
                List<GLBankCashAccountViewModel> Details = new List<GLBankCashAccountViewModel>();
                foreach (var grp in data)
                {
                    GLBankCashAccountViewModel gLBankCashAccountViewModel = new GLBankCashAccountViewModel();
                    gLBankCashAccountViewModel.ShortDate = grp.Date.ToString(Helpers.CommonHelper.DateFormat);
                    gLBankCashAccountViewModel.GLBankCashAccount = grp;
                    gLBankCashAccountViewModel.GLBankCashAccount.Approve = approve;
                    gLBankCashAccountViewModel.GLBankCashAccount.Unapprove = unApprove;
                    Details.Add(gLBankCashAccountViewModel);

                }
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = Details };
                return Ok(jsonData);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IActionResult Approve(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            GLBankCashAccount model = _dbContext.GLBankCashAccounts.Find(id);
            model.ApprovedBy = _userId;
            model.ApprovedDate = DateTime.UtcNow;
            model.IsApproved = true;
            model.Status = "Approved";
            _dbContext.GLBankCashAccounts.Update(model);
            _dbContext.SaveChanges();
            TempData["error"] = "false";
            TempData["message"] = "Bank/Cash Accounts has been approved successfully.";
            return RedirectToAction("Index", "BankCashAccount");
        }
        public IActionResult UnApprove(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            GLBankCashAccount model = _dbContext.GLBankCashAccounts.Find(id);
            model.ApprovedBy = _userId;
            model.ApprovedDate = DateTime.UtcNow;
            model.IsApproved = false;
            model.Status = "Created";
            _dbContext.GLBankCashAccounts.Update(model);
            _dbContext.SaveChanges();
            TempData["error"] = "false";
            TempData["message"] = "Bank/Cash Accounts has been UnApproved successfully.";
            return RedirectToAction("Index", "BankCashAccount");
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var gLBankCashAccounts = _dbContext.GLBankCashAccounts
                .Include(x => x.Account)
                .Where(x => x.Id == id && x.CompanyId == companyId)
                .FirstOrDefault();
            ViewBag.NavbarHeading = "Bank/Cash Detail";
            ViewBag.TitleStatus = "Approved";
            return View(gLBankCashAccounts);
        }
    }
}