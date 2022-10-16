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

namespace Numbers.Areas.AP.Controllers
{
    [Area("AP")]
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
            APAdvance model = new APAdvance();
            var configValues = new ConfigValues(_dbContext);
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string configValue = _dbContext.AppCompanyConfigs
                                .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                                .Select(c => c.ConfigValue)
                                .FirstOrDefault();
            ViewBag.ReportPath = string.Concat(configValue, "Viewer", "?Report=Voucher&cId=", companyId, "&id=");
            model.dtoSupplier = new SelectList(_dbContext.APSuppliers.Where(x => x.IsActive == true && x.IsApproved==true ).ToList(), "Id", "Name");
            model.dtoAdvanceAccount = new SelectList(_dbContext.GLAccounts.Where(x => x.IsDeleted != true ).ToList(), "Id", "Name");
            model.dtoGLBankCash = new SelectList(_dbContext.GLBankCashAccounts.Where(x =>  x.Status == "Approved" && x.CompanyId == companyId).ToList(), "Id", "AccountName");
            if (Id != 0)
            {
                model = _dbContext.APAdvances.Where(x => x.Id == Id).FirstOrDefault();

                var CustomerAccount = (from a in _dbContext.GLAccounts
                                       join b in _dbContext.ARCustomers.Where(x => x.Id == model.SupplierId) on a.Id equals b.AccountId
                                       select new
                                       {
                                           a.Id,
                                           a.Name
                                       }).ToList();

                model.dtoSupplier = new SelectList(_dbContext.APSuppliers.Where(x => x.IsApproved == true).ToList(), "Id", "Name");
                model.dtoAdvanceAccount = new SelectList(CustomerAccount, "Id", "Name");
                model.dtoGLBankCash = new SelectList(_dbContext.GLBankCashAccounts.Where(x => x.Status == "Approved").ToList(), "Id", "AccountName");

            }

            return View(model);
        }
        [HttpGet]
        public IActionResult GetAccountIdbySupplier(int id)
        {
           // APAdvance model = new APAdvance();
            //var configValues = new ConfigValues(_dbContext);

            var CustomerAccount = (from a in _dbContext.GLAccounts
                                   join b in _dbContext.APSuppliers.Where(x => x.Id==id) on a.Id equals b.AccountId
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
            VMAPAdvances model = new VMAPAdvances();
            var configValues = new ConfigValues(_dbContext);
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
          
            if (Id != 0)
            {
                model.aPAdvance = _dbContext.APAdvances.Where(x => x.Id == Id).FirstOrDefault();
                model.dtoCustomer = new SelectList(_dbContext.ARCustomers.Where(x => x.IsDeleted != true).ToList(), "Id", "Name");
                model.dtoAdvanceAccount = new SelectList(_dbContext.GLAccounts.Where(x => x.IsDeleted != true).ToList(), "Id", "Name");
                model.dtoGLBankCash = new SelectList(_dbContext.GLBankCashAccounts.Where(x => x.Status == "Approved").ToList(), "Id", "AccountName");

            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(APAdvance model)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var items = _dbContext.InvItems.Where(x => x.IsDeleted == false).ToList();
            APAdvance advance = new APAdvance();
            if (model.Id == 0)
            {
                int TransactionNo = 1;
                var list = _dbContext.APAdvances.ToList();
                if (list.Count != 0)
                {
                    TransactionNo = list.Select(x => x.TransactionNo).Max() + 1;
                }
                advance.TransactionNo = TransactionNo;
                advance.TransactionDate = model.TransactionDate;
                advance.SupplierId = model.SupplierId;
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
                advance = _dbContext.APAdvances.Find(model.Id);
                advance.TransactionNo = model.TransactionNo;
                advance.TransactionDate = model.TransactionDate;
                advance.SupplierId = model.SupplierId;
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
        //    APAdvance model = _dbContext.APAdvances.Where(x => x.Description == desc).FirstOrDefault();

        //    if (model != null)
        //    {
        //        return false;

        //    }
        //    return true;
        //}
        public IActionResult GetList()
        {
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            string userId = HttpContext.Session.GetString("UserId");
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
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
            var searchCreatedBy = Request.Form["columns[4][search][value]"].FirstOrDefault();
            var searchApprovalBy = Request.Form["columns[5][search][value]"].FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;
            var Data = _dbContext.APAdvances.Include(x => x.CreatedUser).Include(x => x.ApprovalUser).Where(x => x.IsDeleted == false && x.CompanyId == companyId);
            if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
            {
                Data = Data.OrderBy(sortColumn + " " + sortColumnDirection);
            }

            Data = !string.IsNullOrEmpty(searchTransactionNo) ? Data.Where(m => m.TransactionNo.ToString().Contains(searchTransactionNo)) : Data;
            Data = !string.IsNullOrEmpty(searchTransactionDate) ? Data.Where(m => m.TransactionDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchTransactionDate.ToUpper())) : Data;
            Data = !string.IsNullOrEmpty(searchCreatedBy) ? Data.Where(m => m.CreatedUser.UserName.ToString().ToUpper().Contains(searchCreatedBy.ToUpper())) : Data;
            Data = !string.IsNullOrEmpty(searchApprovalBy) ? Data.Where(m => m.ApprovedBy != null ? m.ApprovalUser.UserName.ToString().ToUpper().Contains(searchApprovalBy.ToUpper()):false) : Data;
             
            recordsTotal = Data.Count();
            var data = Data.Skip(skip).Take(pageSize).ToList();
            List<VMAPAdvances> details = new List<VMAPAdvances>();
            foreach (var item in data)
            {
                var advance = new VMAPAdvances();
                advance.UpdatedBy = item.TransactionDate.ToString(Helpers.CommonHelper.DateFormat);
                advance.Approve = approve;
                advance.Unapprove = unApprove;
                advance.TransactionNo = item.TransactionNo;
                advance.CreatedBy = (_dbContext.APSuppliers.Where(x => x.Id == item.SupplierId).Select(x => x.Name).FirstOrDefault());
                advance.Amount = item.Amount;
                advance.Id = item.Id;
                advance.aPAdvance = item;
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
        //    APAdvance model = _dbContext.APAdvances.Include(x=>x.Customer).Include(x=>x.AdvanceAccount).Where(x=>x.Id==id).FirstOrDefault();
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
        //    APAdvance model = _dbContext.Find(id);


        //    var checkCatgryRfrnc = _dbContext.GRWeavingContracts.Where(x => x.GreigeadvanceId == id || x.GreigeadvanceLoomId == id).ToList();
        //    var checkCatgryRfrnc2 = _dbContext.GRPurchaseContract.Where(x => x.PurchaseAPAdvanceId == id || x.ContractAPAdvanceId == id).ToList();
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
            var advance = new APAdvance { Id = id };
            if (advance != null)
            {
                var record = _dbContext.APAdvances.Find(advance.Id);
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
                var repo = await   ApproveRepo(id);
                if (repo == true)
                {
                    //On approval updating Invoice
                    TempData["error"] = "false";
                    TempData["message"] = "AP Advance has been approved successfully";
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "AP Advance was not Approved";
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
            APAdvance receipt = _dbContext.APAdvances
             .Include(c => c.Supplier)
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
                "AP Advance # : {0} of  " +
                "{1} {2}",
                receipt.TransactionNo,
                receipt.Supplier.Name, receipt.Remarks);

                int voucherId;
                voucherMaster.VoucherType = "AP-Adv";
                voucherMaster.VoucherDate = receipt.TransactionDate;
                //voucherMaster.Reference = receipt.reference;
                voucherMaster.Currency = "PKR";
                voucherMaster.CurrencyExchangeRate = 1;
                voucherMaster.Description = receipt.Remarks;
                voucherMaster.Status = "Approved";
                voucherMaster.ApprovedBy = userId;
                voucherMaster.ApprovedDate = DateTime.Now;
                voucherMaster.IsSystem = true;
                voucherMaster.ModuleName = "AP/Advance";
                voucherMaster.ModuleId = id;
                voucherMaster.ReferenceId = receipt.SupplierId;
                //Voucher Details
                var amount = receipt.Amount;
                //Debit Entry
                GLVoucherDetail voucherDetail;

                #region Supplier (DEBIT)
                voucherDetail = new GLVoucherDetail();
                voucherDetail.AccountId = receipt.Supplier.AccountId;
                voucherDetail.Sequence = 10;
                voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                voucherDetail.Debit = receipt.Amount;
                voucherDetail.Credit = 0;
                voucherDetail.IsDeleted = false;
                voucherDetail.CreatedBy = userId;
                voucherDetail.CreatedDate = DateTime.Now;
                voucherDetails.Add(voucherDetail);
                #endregion 

                //Credit Entry
                #region Cash Bank  (CREDIT)
                //Receipt Voucher Debit Entry
                voucherDetail = new GLVoucherDetail();
                voucherDetail.AccountId = receipt.GLBankCash.AccountId;
                voucherDetail.Sequence = 20;
                voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                voucherDetail.Debit = 0;
                voucherDetail.Credit = amount;
                voucherDetail.IsDeleted = false;
                voucherDetail.CreatedBy = userId;
                voucherDetail.CreatedDate = DateTime.Now;
                voucherDetails.Add(voucherDetail);

                #endregion Line Items

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

        public async Task<IActionResult>  UnApprove(int id)
        {
            var advance = new APAdvance { Id = id };
            if (advance != null)
            {
                var record = _dbContext.APAdvances.Find(advance.Id);
                var voucherDetail = _dbContext.GLVoucherDetails.Where(v => v.VoucherId == record.VoucherId).ToList();
 
                foreach (var item in voucherDetail)
                {
                    var tracker = _dbContext.GLVoucherDetails.Remove(item);
                    tracker.OriginalValues.SetValues(await tracker.GetDatabaseValuesAsync());
                    await _dbContext.SaveChangesAsync();
                }
                record.Status = "Created";
                record.ApprovedBy = null;
                record.VoucherId = 0;
                _dbContext.Update(record);
                _dbContext.SaveChanges();

                TempData["error"] = "false";
                TempData["message"] = "AP Advance # " + record.TransactionNo + " has been Un Approved successfully.";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong.";
            }
            return RedirectToAction("Index");
        }

        public async Task<bool> UnApproveVoucher(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var receipt = _dbContext.ARReceipts
                            .Where(v => v.IsDeleted == false && v.Id == id && v.Status == "Approved" && v.CompanyId == companyId).FirstOrDefault();
            if (receipt == null)
            {
                return false;
            }
            else
            {
                using (var transaction = _dbContext.Database.BeginTransaction())
                {
                    try
                    {

                        var voucherDetail = _dbContext.GLVoucherDetails.Where(v => v.VoucherId == receipt.VoucherId).ToList();
                        var items = _dbContext.ARReceiptInvoices.Where(r => r.IsDeleted == false && r.ReceiptId == id).ToList();
                        foreach (var item in items)
                        {
                            var invoice = _dbContext.ARInvoices.Find(item.InvoiceId);
                            invoice.ReceiptTotal = invoice.ReceiptTotal - item.LineTotal;
                            var dbEntry = _dbContext.ARInvoices.Update(invoice);
                            dbEntry.OriginalValues.SetValues(await dbEntry.GetDatabaseValuesAsync());
                            await _dbContext.SaveChangesAsync();
                        }
                        foreach (var item in voucherDetail)
                        {
                            var tracker = _dbContext.GLVoucherDetails.Remove(item);
                            tracker.OriginalValues.SetValues(await tracker.GetDatabaseValuesAsync());
                            await _dbContext.SaveChangesAsync();
                        }
                        receipt.Status = "Created";
                        receipt.ApprovedBy = null;
                        receipt.ApprovedDate = null;
                        var entry = _dbContext.ARReceipts.Update(receipt);
                        entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                        await _dbContext.SaveChangesAsync();
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine(exc);
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }


    }
}
