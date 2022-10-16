using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Numbers.Entity.Models;
using Numbers.Repository.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity.ViewModels;

namespace Numbers.Areas.AR.Controllers
{
    [Area("AR")]
    [Authorize]
    public class AdvanceController : Controller
    {
        private readonly NumbersDbContext _dbContext;

        public AdvanceController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;

        }
        //[HttpGet]
        public IActionResult Index(int Id)
        {
            ViewBag.EntityState = "Create";
            ViewBag.NavbarHeading = "Create Advance";
            ARAdvance model = new ARAdvance();
            var configValues = new ConfigValues(_dbContext);
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string configValue = _dbContext.AppCompanyConfigs
                              .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                              .Select(c => c.ConfigValue)
                              .FirstOrDefault();
            ViewBag.ReportPath = string.Concat(configValue, "Viewer", "?Report=Voucher&cId=", companyId, "&id=");
            model.dtoCustomer = new SelectList(_dbContext.ARCustomers.Where(x =>/* x.CompanyId == companyId &&*/ x.IsDeleted != true ).ToList(), "Id", "Name");
            model.dtoAdvanceAccount = new SelectList(_dbContext.GLAccounts.Where(x => x.IsDeleted != true ).ToList(), "Id", "Name");
            model.dtoGLBankCash = new SelectList(_dbContext.GLBankCashAccounts.Where(x => x.CompanyId == companyId && x.Status == "Approved").ToList(), "Id", "AccountName");
            if (Id != 0)
            {
                model = _dbContext.ARAdvances.Where(x => x.Id == Id).FirstOrDefault();

                var CustomerAccount = (from a in _dbContext.GLAccounts
                                       join b in _dbContext.ARCustomers.Where(x => x.Id == model.CustomerId) on a.Id equals b.AccountId
                                       select new
                                       {
                                           a.Id,
                                           a.Name
                                       }).ToList();

                model.dtoCustomer = new SelectList(_dbContext.ARCustomers.Where(x => x.IsDeleted != true).ToList(), "Id", "Name");
                model.dtoAdvanceAccount = new SelectList(CustomerAccount, "Id", "Name");
                model.dtoGLBankCash = new SelectList(_dbContext.GLBankCashAccounts.Where(x => x.Status == "Approved").ToList(), "Id", "AccountName");

            }

            return View(model);
        }
        [HttpGet]
        public IActionResult GetAccountIdbyCustomer(int id)
        {
           // ARAdvance model = new ARAdvance();
            //var configValues = new ConfigValues(_dbContext);

            var CustomerAccount = (from a in _dbContext.GLAccounts
                                   join b in _dbContext.ARCustomers.Where(x => x.Id==id) on a.Id equals b.AccountId
                                   select new
                                   {
                                       a.Id,
                                       a.Name
                                   }).ToList();


         //  var customer = new SelectList(CustomerAccount, "Id", "Name");
            
          

            return Ok(CustomerAccount);
        }



        public IActionResult Details(int Id)
        {
            ViewBag.EntityState = "Detail";
            ViewBag.NavbarHeading = "Detail Advance";
            ARAdvance model = new ARAdvance();
            var configValues = new ConfigValues(_dbContext);
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
          
            if (Id != 0)
            {
                model = _dbContext.ARAdvances.Where(x => x.Id == Id).FirstOrDefault();
                model.dtoCustomer = new SelectList(_dbContext.ARCustomers.Where(x => x.IsDeleted != true).ToList(), "Id", "Name");
                model.dtoAdvanceAccount = new SelectList(_dbContext.GLAccounts.Where(x => x.IsDeleted != true).ToList(), "Id", "Name");
                model.dtoGLBankCash = new SelectList(_dbContext.GLBankCashAccounts.Where(x => x.Status == "Approved").ToList(), "Id", "AccountName");

            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ARAdvance model)
        {

            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var items = _dbContext.InvItems.Where(x => x.IsDeleted == false).ToList();
            ARAdvance advance = new ARAdvance();
            if (model.Id == 0)
            {
                int TransactionNo = 1;
                var list = _dbContext.ARAdvances.ToList();
                if (list.Count != 0)
                {
                    TransactionNo = list.Select(x => x.TransactionNo).Max() + 1;
                }
                advance.TransactionNo = TransactionNo;
                advance.TransactionDate = model.TransactionDate;
                advance.CustomerId = model.CustomerId;
                advance.AdvanceAccountId = model.AdvanceAccountId;
                advance.GLBankCashId = model.GLBankCashId;

                advance.IsActive = true;
                advance.IsDeleted = false;
                advance.CompanyId = companyId;
                advance.Resp_Id = resp_Id;
                advance.CreatedBy = userId;
                advance.CreatedDate = DateTime.Now;
                advance.RefrenceNo = model.RefrenceNo;
                advance.ReferanceDate = model.ReferanceDate;
                model.UpdatedDate = model.UpdatedDate;
                advance.Amount = model.Amount;
                advance.Remarks = model.Remarks;
                advance.Status = "Created";

                _dbContext.Add(advance);
                await _dbContext.SaveChangesAsync();
                TempData["error"] = "false";
                TempData["message"] = "Advance # "+ advance.TransactionNo +" has been saved successfully.";
            }
            else
            {
                advance = _dbContext.ARAdvances.Find(model.Id);
                advance.TransactionNo = model.TransactionNo;
                advance.TransactionDate = model.TransactionDate;
                advance.CustomerId = model.CustomerId;
                advance.AdvanceAccountId = model.AdvanceAccountId;
                advance.GLBankCashId = model.GLBankCashId;
                
                advance.IsActive = true;
                advance.IsDeleted = false;
                advance.CompanyId = companyId;
                advance.Resp_Id = resp_Id;
                advance.CreatedBy = userId;
                advance.CreatedDate = DateTime.Now;
                advance.RefrenceNo = model.RefrenceNo;
                advance.ReferanceDate = model.ReferanceDate;
                model.UpdatedDate = model.UpdatedDate;
                advance.Status = "Created";
                advance.Amount = model.Amount;
                advance.Remarks = model.Remarks;
                _dbContext.Update(advance);
                await _dbContext.SaveChangesAsync();
                TempData["error"] = "false";
                TempData["message"] = "Advance # " + advance.TransactionNo + " has been Updated successfully.";

            }
            return RedirectToAction(nameof(Index));
        }

        //public bool Checkadvance(string desc)
        //{
        //    int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
        //    string _userId = HttpContext.Session.GetString("UserId");
        //    ARAdvance model = _dbContext.ARAdvances.Where(x => x.Description == desc).FirstOrDefault();

        //    if (model != null)
        //    {
        //        return false;

        //    }
        //    return true;
        //}
        public IActionResult GetList()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var approve = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == resp_Id).FirstOrDefault().Approve;
            var unApprove = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == resp_Id).FirstOrDefault().UnApprove;

            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            //var searchValue = Request.Form["search[value]"].FirstOrDefault();
            var searchTransactionNo = Request.Form["columns[0][search][value]"].FirstOrDefault();
            var searchTransactionDate = Request.Form["columns[1][search][value]"].FirstOrDefault();
            var searchDescription = Request.Form["columns[2][search][value]"].FirstOrDefault();
            var searchGSM = Request.Form["columns[3][search][value]"].FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;
            var Data = _dbContext.ARAdvances.Where(x => x.CompanyId == companyId && x.IsDeleted == false);
            if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
            {
                Data = Data.OrderBy(sortColumn + " " + sortColumnDirection);
            }

            Data = !string.IsNullOrEmpty(searchTransactionNo) ? Data.Where(m => m.TransactionNo.ToString().Contains(searchTransactionNo)) : Data;
            Data = !string.IsNullOrEmpty(searchTransactionDate) ? Data.Where(m => m.TransactionDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchTransactionDate.ToUpper())) : Data;
             
            recordsTotal = Data.Count();
            var data = Data.ToList();
            if (pageSize == -1)
            {
                data = Data.Skip(skip).Take(recordsTotal == 0 ? 1 : pageSize == -1 ? recordsTotal : 1).ToList();
            }
            else
            {
                data = Data.Skip(skip).Take(pageSize).ToList();
            }

            //var data = Data.Skip(skip).Take(pageSize).ToList();
            List<ARAdvance> details = new List<ARAdvance>();
            foreach (var item in data)
            {
                var advance = new ARAdvance();
                advance.UpdatedBy = item.TransactionDate.ToString(Helpers.CommonHelper.DateFormat);
                advance.Approve = approve;
                advance.Unapprove = unApprove;
                advance.TransactionNo = item.TransactionNo;
                advance.CreatedBy = (_dbContext.ARCustomers.Where(x => x.Id == item.CustomerId).Select(x => x.Name).FirstOrDefault());
                advance.Amount = item.Amount;
                advance.Id = item.Id;
                advance.ARAdvanceModel = item;
                details.Add(advance);

                //item.UpdatedBy = item.TransactionDate.ToString(Helpers.CommonHelper.DateFormat);
            }
            var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = details.OrderByDescending(x => x.TransactionNo), };
            return Ok(jsonData);
        }

        


        //public async Task<IActionResult> Approve(int id)
        //{
        //    int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
        //    string _userId = HttpContext.Session.GetString("UserId");
        //    ARAdvance model = _dbContext.ARAdvances.Include(x=>x.Customer).Include(x=>x.AdvanceAccount).Where(x=>x.Id==id).FirstOrDefault();
        //    model.ApprovedBy = _userId;
        //    model.ApprovedDate = DateTime.UtcNow;
        //    model.IsApproved = true;
        //    model.Status = "Approved";
        //    if (model.Id == 0)
        //    {
        //        if (model.GRCategory.ItemCategoryId != 0 && model.GRConstruction.ItemCategoryId != 0)
        //        {

        //            var itemModel = await CreateItem(model);
        //            if (itemModel != null)
        //            {
        //                model.ItemId = itemModel.Id;
        //            }
        //            else
        //            {
        //                model.ItemId = 0;
        //            }
        //        }
        //        else
        //        {
        //            model.ItemId = 0;
        //        }

        //    }
        //    else
        //    {
        //        await UpdateItem(model);

        //    }
        //    _dbContext.Update(model);
        //   await _dbContext.SaveChangesAsync();
        //    TempData["error"] = "false";
        //    TempData["message"] = "advance has been approved successfully.";
        //    return RedirectToAction("Index", "advance");
        //}
        //public IActionResult UnApprove(int id)
        //{
        //    int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
        //    string _userId = HttpContext.Session.GetString("UserId");
        //    ARAdvance model = _dbContext.Find(id);


        //    var checkCatgryRfrnc = _dbContext.GRWeavingContracts.Where(x => x.GreigeadvanceId == id || x.GreigeadvanceLoomId == id).ToList();
        //    var checkCatgryRfrnc2 = _dbContext.GRPurchaseContract.Where(x => x.PurchaseARAdvanceId == id || x.ContractARAdvanceId == id).ToList();
        //    if (checkCatgryRfrnc.Count == 0)
        //    {
        //        model.ApprovedBy = _userId;
        //        model.ApprovedDate = DateTime.UtcNow;
        //        model.IsApproved = false;
        //        model.Status = "Created";
        //        _dbContext.Update(model);
        //        _dbContext.SaveChanges();
        //        TempData["error"] = "false";
        //        TempData["message"] = "Advance has been UnApproved successfully.";

        //    }
        //    if (checkCatgryRfrnc.Count != 0)
        //    {
        //        TempData["error"] = "true";
        //        TempData["message"] = "Transaction No is Used in Weaving contract..!";
        //    }
        //    if (checkCatgryRfrnc2.Count != 0)
        //    {
        //        TempData["error"] = "true";
        //        TempData["message"] = "Transaction No is Used in Purchase contract..!";
        //    }

 
        //    return RedirectToAction("Index", "advance");
        //}
        public IActionResult Delete(int id)
        {
            var advance = new ARAdvance { Id = id };
            if (advance != null)
            {
                var record = _dbContext.ARAdvances.Find(advance.Id);
                record.IsActive = false;
                record.IsDeleted = true;
                _dbContext.Update(record); 
                _dbContext.SaveChanges();

                TempData["error"] = "false";
                TempData["message"] = "Advance # "+ record.TransactionNo +" has been deleted successfully.";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong.";
            }
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                var repo = await ApproveRepo(id);
                if (repo == true)
                {
                    //On approval updating Invoice
                    TempData["error"] = "false";
                    TempData["message"] = "AR Advance has been approved successfully";
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "AR Advance was not Approved";
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                TempData["error"] = "true";
                TempData["message"] = ex.ToString();
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<bool> ApproveRepo(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            ARAdvance receipt = _dbContext.ARAdvances
             .Include(c => c.Customer)
             .Include(c => c.GLBankCash)
             .Where(a => a.Status == "Created" && a.CompanyId == companyId && a.Id == id && a.IsDeleted == false)
             .FirstOrDefault();
            try
            {
                //Create Voucher
                VoucherHelper voucher = new VoucherHelper(_dbContext, HttpContext);
                GLVoucher voucherMaster = new GLVoucher();
                List<GLVoucherDetail> voucherDetails = new List<GLVoucherDetail>();
                string voucherDescription = string.Format(
                "AR Advance # : {0} of  " +
                "{1} {2}",
                receipt.TransactionNo,
                receipt.Customer.Name, receipt.Remarks);

                int voucherId;
                voucherMaster.VoucherType = "AR-Adv";
                voucherMaster.VoucherDate = receipt.TransactionDate;
                //voucherMaster.Reference = receipt.reference;
                voucherMaster.Currency = "PKR";
                voucherMaster.CurrencyExchangeRate = 1;
                voucherMaster.Description = receipt.Remarks;
                voucherMaster.Status = "Approved";
                voucherMaster.ApprovedBy = userId;
                voucherMaster.ApprovedDate = DateTime.Now;
                voucherMaster.IsSystem = true;
                voucherMaster.ModuleName = "AR/Advance";
                voucherMaster.ModuleId = id;
                voucherMaster.ReferenceId = receipt.CustomerId;
                //Voucher Details
                var amount = receipt.Amount;
                //Credit Entry
                GLVoucherDetail voucherDetail;

                #region Cash Bank  (DEBIT)
                //Receipt Voucher Debit Entry
                voucherDetail = new GLVoucherDetail();
                voucherDetail.AccountId = receipt.GLBankCash.AccountId;
                voucherDetail.Sequence = 10;
                voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                voucherDetail.Debit = amount;
                voucherDetail.Credit = 0;
                voucherDetail.IsDeleted = false;
                voucherDetail.CreatedBy = userId;
                voucherDetail.CreatedDate = DateTime.Now;
                voucherDetails.Add(voucherDetail);

                #endregion Line Items


                #region CUSTOMER (CREDIT)
                voucherDetail = new GLVoucherDetail();
                voucherDetail.AccountId = receipt.Customer.AccountId;
                voucherDetail.Sequence = 20;
                voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                voucherDetail.Debit = 0;
                voucherDetail.Credit = receipt.Amount;
                voucherDetail.IsDeleted = false;
                voucherDetail.CreatedBy = userId;
                voucherDetail.CreatedDate = DateTime.Now;
                voucherDetails.Add(voucherDetail);
                #endregion 

                //Create Voucher 
                voucherId = voucher.CreateVoucher(voucherMaster, voucherDetails);
                if (voucherId != 0)
                {

                    receipt.VoucherId = voucherId;
                    receipt.Status = "Approved";
                    receipt.IsApproved = true;
                    receipt.ApprovedBy = userId;
                    receipt.ApprovedDate = DateTime.Now;
                    using (var transaction = _dbContext.Database.BeginTransaction())
                    {
                        //On approval updating Invoice
                        var entry = _dbContext.Update(receipt);
                        entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                        await _dbContext.SaveChangesAsync();
                        transaction.Commit();
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                return false;
            }
        }

        public async Task<IActionResult> UnApprove(int id)
        {
            var advance = new APAdvance { Id = id };
            if (advance != null)
            {
                var record = _dbContext.ARAdvances.Find(advance.Id);
                var voucherDetail = _dbContext.GLVoucherDetails.Where(v => v.VoucherId == record.VoucherId).ToList();

                foreach (var item in voucherDetail)
                {
                    var tracker = _dbContext.GLVoucherDetails.Remove(item);
                    tracker.OriginalValues.SetValues(await tracker.GetDatabaseValuesAsync());
                    await _dbContext.SaveChangesAsync();
                }
                record.Status = "Created";
                record.VoucherId = 0;
                _dbContext.Update(record);
                _dbContext.SaveChanges();

                TempData["error"] = "false";
                TempData["message"] = "AR Advance # " + record.TransactionNo + " has been Un Approved successfully.";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong.";
            }
            return RedirectToAction("Index");
        }

    }
}
