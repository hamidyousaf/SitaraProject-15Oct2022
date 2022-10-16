using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Helpers;
using Numbers.Repository.AP;
using Numbers.Repository.Helpers;
using System.Linq.Dynamic.Core;
using System.IO;
using Newtonsoft.Json;

namespace Numbers.Areas.AP.Controllers
{
    [Area("AP")]
    [Authorize]
    public class LetterOfCreditController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        private readonly APLCRepository _APLCRepository;
        private readonly APGRNExpenseRepository _APGRNExpenseRepository;
        private readonly APInsuranceInfoRepository _APInsuranceInfoRepository;
        public LetterOfCreditController(NumbersDbContext context, APLCRepository APLCRepository,APInsuranceInfoRepository APInsuranceInfoRepository,APGRNExpenseRepository APGRNExpenseRepository)
        {
            _dbContext = context;
            _APLCRepository = APLCRepository;
            _APGRNExpenseRepository = APGRNExpenseRepository;
            _APInsuranceInfoRepository = APInsuranceInfoRepository;
        }
        public IActionResult Index(int id)
        {
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var configValues = new ConfigValues(_dbContext);
            ViewBag.Bank= new SelectList(_dbContext.GLBankCashAccounts.Where(x => x.IsActive == true).ToList(), "Id", "AccountName");
            ViewBag.Currencies = new SelectList(_dbContext.AppCurrencies.Where(x => x.IsActive).ToList(), "Id", "Name");
            ViewBag.LCType = configValues.GetConfigValues("AP", "Letter Of Credit", companyId);
            ViewBag.FreightType = configValues.GetConfigValues("AP", "Freight Type", companyId);
            ViewBag.Insurance = configValues.GetConfigValues("AP", "Insurance Company", companyId);
            var Id = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.CompanyId == companyId && x.ConfigValue == "Import").Id;
          //  ViewBag.Po= new SelectList(_dbContext.APPurchaseOrders.Where(x => x.IsDeleted == false && x.POTypeId==Id && x.Status== "Approved"&& x.LCNumber == null).ToList(), "Id", "PONo");
            ViewBag.GLCode = new SelectList(_dbContext.GLAccounts.Where(a => a.CompanyId == companyId && a.IsDeleted == false && a.AccountLevel == 4 && a.Code.Contains("6.01.01")).Select(a => new
            {
                id = a.Id,
                text = string.Concat(a.Code, " - ", a.Name),
                code = a.Code,
                name = a.Name
            }).ToList(), "id", "text");
            APLC aPLC = new APLC();
            if (id == 0)
            {
                ViewBag.Selectlist = 0;
                ViewBag.Po = new SelectList(_dbContext.APPurchaseOrders.Where(x => x.IsDeleted == false && x.POTypeId == Id && x.Status == "Approved" && x.LCNumber == null).ToList(), "Id", "PONo");
                var result = _APLCRepository.Get(x => x.IsActive).ToList();
                if (result.Count > 0)
                {
                    ViewBag.Id = _dbContext.APLC.Select(x => x.TransctionNo).Max() + 1;
                }
                else
                {
                    ViewBag.Id = 1;
                }
            }
            else
            {
                aPLC = _APLCRepository.Get(x => x.Id == id).FirstOrDefault();
                ViewBag.Expense = _dbContext.APGRNExpense.Where(x => x.LCId == id).ToList();
                ViewBag.Insurnces = _dbContext.APInsuranceInfo.Where(x => x.LCId == id).FirstOrDefault();
                ViewBag.Accounts = _dbContext.GLAccounts.Where(a => a.CompanyId == companyId && a.IsDeleted == false && a.AccountLevel == 4).ToList();
                //var ResultPO= Convert.ToInt32(_dbContext.APPurchaseOrders.Where(x => x.POTypeId == Id).FirstOrDefault());
                ViewBag.Selectlist = 1;
                ViewBag.Po = new SelectList(_dbContext.APPurchaseOrders.Where(x => x.IsDeleted == false && x.POTypeId == Id && x.Status == "Approved" && x.LCNumber !=null).ToList(), "Id", "PONo");
                aPLC.VendorName = _dbContext.APSuppliers.FirstOrDefault(x => x.Id == aPLC.VendorId).Name;
                aPLC.CurrencyName = _dbContext.AppCurrencies.FirstOrDefault(x => x.Id == aPLC.CurrencyId).Name;
              //  aPPurchaseItemViewModel.UOmName = new List<string>();
                //foreach (var grp in model)
                //{
                //    string UOmName = null;
                //    UOmName = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == grp.UOM).ConfigValue;
                //    aPPurchaseItemViewModel.UOmName.Add(UOmName);
                //    aPPurchaseItemViewModel.APPurchaseRequisitionDetails.Add(grp);
                //}
            }
            return View(aPLC);
        }
        public IActionResult GetPO(int id)
        {
            APPurchaseOrder aPPurchaseOrder = _dbContext.APPurchaseOrders.FirstOrDefault(x => x.Id == id);
            aPPurchaseOrder.VendorName = _dbContext.APSuppliers.FirstOrDefault(x => x.Id == aPPurchaseOrder.SupplierId).Name;
            aPPurchaseOrder.CurrencyName = "";
            if (aPPurchaseOrder.Currency != null)
            {
                aPPurchaseOrder.CurrencyName = _dbContext.AppCurrencies.FirstOrDefault(x => x.Id == aPPurchaseOrder.Currency).Name;
            }
               
            return Ok(aPPurchaseOrder);
        }
        public IActionResult List()
        {
            return View();
        }
        public IActionResult GetLC()
        {
            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var LCData = (from tempcustomer in _dbContext.APLC.Where(x => x.IsDeleted == false) select tempcustomer);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    LCData = LCData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    LCData = LCData.Where(m => m.LCOpenDate.ToShortDateString().Contains(searchValue)
                                                    || m.LCNo.ToString().Contains(searchValue)
                                                    || _dbContext.APPurchaseOrders.FirstOrDefault(x=>x.PONo==m.POId).PONo.ToString().Contains(searchValue)                                                    || m.TransctionNo.ToString().Contains(searchValue)
                                                    || _dbContext.APSuppliers.FirstOrDefault(x => x.Id == m.VendorId).Name.Contains(searchValue)
                                                    || m.FCAmount.ToString().Contains(searchValue)
                                                    || m.PKRAmount.ToString().Contains(searchValue)
                                                    || _dbContext.Users.FirstOrDefault(x => x.Id == m.CreatedBy).UserName.Contains(searchValue)
                                                  );

                }
                recordsTotal = LCData.Count();
                var data = LCData.Skip(skip).Take(pageSize).ToList();
                List<APLC> Details = new List<APLC>();
                foreach (var grp in data)
                {
                    APLC aPLC = new APLC();
                    grp.CreatedBy = _dbContext.Users.FirstOrDefault(x => x.Id == grp.CreatedBy).UserName;
                    grp.VendorName = _dbContext.APSuppliers.FirstOrDefault(x => x.Id == grp.VendorId).Name;
                    grp.Date = grp.LCOpenDate.ToString(Helpers.CommonHelper.DateFormat);
                    grp.BankName = _dbContext.GLBankCashAccounts.FirstOrDefault(x => x.Id == grp.BankId).AccountName;
                    grp.LCName = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == grp.LCTypeId).ConfigValue;
                    grp.POId = _dbContext.APPurchaseOrders.FirstOrDefault(x => x.Id == grp.POId).PONo;
                    aPLC = grp;
                    Details.Add(aPLC);

                }
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = Details };
                return Ok(jsonData);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<IActionResult> Submit(APLC aPLC, IFormFile File, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            if (aPLC.Id == 0)
            {
                try
                {
                    aPLC.VendorId = _dbContext.APSuppliers.FirstOrDefault(x => x.Name == aPLC.VendorName).Id;
                    aPLC.CurrencyId = _dbContext.AppCurrencies.FirstOrDefault(x => x.Symbol == aPLC.CurrencyName).Id;
                    if (collection["total"] != "")
                        aPLC.TotalAmount = Convert.ToDecimal(collection["total"][0]);
                    aPLC.CreatedBy = userId;
                    aPLC.CreatedDate = DateTime.UtcNow;
                    aPLC.CompanyId = companyId;
                    aPLC.Resp_Id = resp_Id;
                    var SubAccountId = (from a in _dbContext.AppCompanySetups.Where(x => x.Name == "LC Account" && x.CompanyId == companyId)
                                         join b in _dbContext.GLAccounts on a.Value equals b.Code 
                                         where b.CompanyId == companyId select b.SubAccountId).FirstOrDefault();
                    aPLC.SubAccountId = Convert.ToInt32(SubAccountId);
                    aPLC.IsActive = true;
                    if (!ReferenceEquals(File, null))
                        aPLC.Attachment = await UploadFile(File);
                    await _APLCRepository.CreateAsync(aPLC);
                    var po = _dbContext.APPurchaseOrders.Where(x => x.Id == aPLC.POId).FirstOrDefault();
                    po.LCNumber = aPLC.LCNo;
                    _dbContext.APPurchaseOrders.Update(po);
                    List<APGRNExpense> expenses = new List<APGRNExpense>();
                    APGRNExpense[] Expense = JsonConvert.DeserializeObject<APGRNExpense[]>(collection["expenseDetails"]);
                    foreach (var item in Expense)
                    {
                        APGRNExpense exp = new APGRNExpense();
                        exp.CreatedBy = userId;
                        exp.CreatedDate = DateTime.UtcNow;
                        exp.CompanyId = companyId;
                        exp.LCId = aPLC.Id;
                        exp.AccountName = item.AccountName;
                        exp.GLCode = item.GLCode;
                        exp.Remarks = item.Remarks;
                        exp.ExpenseAmount = item.ExpenseAmount;
                        exp.ExpenseFavour = item.ExpenseFavour;
                        exp.ExpenseDate = Convert.ToDateTime(item.ExpenseDate);
                        expenses.Add(exp);
                    }
                    _dbContext.AddRange(expenses);
                    _dbContext.SaveChanges();
                    if (collection["coverNo"]!="")
                    {
                        APInsuranceInfo info = new APInsuranceInfo();
                        info.LCId = aPLC.Id;
                        info.InsuranceCompanyId= Convert.ToInt32(collection["companyName"]);
                        info.CoverNoteNo = Convert.ToString(collection["coverNo"]);
                        info.Charges = Convert.ToDecimal(collection["charges"]);
                        info.CoverNoteDate = Convert.ToDateTime(collection["coverNoteDate"]);
                         await _APInsuranceInfoRepository.CreateAsync(info);
                    }
                    TempData["error"] = "false";
                    TempData["message"] = "Letter Of Credit has been created successfully.";
                }
                catch (Exception ex)
                {
                    await _APLCRepository.DeleteAsync(aPLC);
                    var DeleteExp = _APGRNExpenseRepository.Get(x => x.LCId == aPLC.Id).ToList();
                    var DeleteInfo = _APInsuranceInfoRepository.Get(x => x.LCId == aPLC.Id).ToList();
                    await _APGRNExpenseRepository.DeleteRangeAsync(DeleteExp);
                    await _APInsuranceInfoRepository.DeleteRangeAsync(DeleteInfo);
                    TempData["error"] = "true";
                    TempData["message"] = "Went Something wrong";
                }
            }
            else
            {
                APLC LC = _dbContext.APLC.FirstOrDefault(x => x.Id == aPLC.Id);
                LC.UpdatedBy = userId;
                LC.UpdatedDate = DateTime.UtcNow;
                LC.CompanyId = companyId;
                LC.Resp_Id = resp_Id;
                LC.IsActive = true;
                LC.LatestShipmentDate = aPLC.LatestShipmentDate;
                LC.LCCloseDate = aPLC.LCCloseDate;
                LC.LCOpenDate = aPLC.LCOpenDate;
                LC.EIFDate = aPLC.EIFDate;
                LC.EIFNo = aPLC.EIFNo;
                LC.LCExpiryDate = aPLC.LCExpiryDate;
                LC.BankId = aPLC.BankId;
              //  LC.Currency = aPLC.Currency;
                LC.FCAmount = aPLC.FCAmount;
                LC.ExchangeRate = aPLC.ExchangeRate;
                LC.LCNo = aPLC.LCNo;
                LC.LCTypeId = aPLC.LCTypeId;
                LC.PKRAmount = aPLC.PKRAmount;
                LC.Remarks = aPLC.Remarks;
                LC.TransctionNo = aPLC.TransctionNo;
                var SubAccountId = (from a in _dbContext.AppCompanySetups.Where(x => x.Name == "LC Account" && x.CompanyId == companyId)
                                    join b in _dbContext.GLAccounts on a.Value equals b.Code
                                    where b.CompanyId == companyId
                                    select b.SubAccountId).FirstOrDefault();
                LC.SubAccountId = Convert.ToInt32(SubAccountId);
                if (collection["TotalAmount"] != "")
                    LC.TotalAmount = Convert.ToDecimal(collection["TotalAmount"]);
                if (!ReferenceEquals(File, null))
                    LC.Attachment = await UploadFile(File);
                await _APLCRepository.UpdateAsync(LC);
                var po = _dbContext.APPurchaseOrders.Where(x => x.Id == LC.POId).FirstOrDefault();
                po.LCNumber = aPLC.LCNo;
                _dbContext.APPurchaseOrders.Update(po);
                List<APGRNExpense> expenses = new List<APGRNExpense>();
                List<APGRNExpense> expensesUpdate = new List<APGRNExpense>();
                var ExpenseList = _dbContext.APGRNExpense.Where(x => x.LCId == aPLC.Id).ToList();
                List<APGRNExpense> aPGRNExpenses = new List<APGRNExpense>();
                APGRNExpense[] Expense = JsonConvert.DeserializeObject<APGRNExpense[]>(collection["expenseDetails"]);
                foreach(var itm in Expense)
                {
                    expensesUpdate.Add(itm);
                }
                foreach(var item in ExpenseList)
                {
                    bool isExis = expensesUpdate.Exists(x => x.Id == item.Id);
                    if (!isExis)
                    {
                        var delete = _dbContext.APGRNExpense.Where(x => x.Id == item.Id).FirstOrDefault();
                        _dbContext.Remove(delete);
                        _dbContext.SaveChanges();
                    }
                }
                foreach (var item in Expense)
                {
                    if (item.Id == 0)
                    {
                        APGRNExpense exp = new APGRNExpense();
                        exp.CreatedBy = userId;
                        exp.CreatedDate = DateTime.UtcNow;
                        exp.CompanyId = companyId;
                        exp.LCId = aPLC.Id;
                        exp.AccountName = item.AccountName;
                        exp.GLCode = item.GLCode;
                        exp.Remarks = item.Remarks;
                        exp.ExpenseAmount = item.ExpenseAmount;
                        exp.ExpenseDate = Convert.ToDateTime(item.ExpenseDate);
                        exp.ExpenseFavour = item.ExpenseFavour;
                        expenses.Add(exp);
                    }
                    else
                    {
                        APGRNExpense exp = _dbContext.APGRNExpense.Where(x => x.Id == item.Id).FirstOrDefault();
                        exp.UpdatedBy = userId;
                        exp.UpdatedDate = DateTime.UtcNow;
                        exp.CompanyId = companyId;
                        exp.LCId = aPLC.Id;
                        exp.AccountName = item.AccountName;
                        exp.GLCode = item.GLCode;
                        exp.Remarks = item.Remarks;
                        exp.ExpenseAmount = item.ExpenseAmount;
                        exp.ExpenseDate = Convert.ToDateTime(item.ExpenseDate);
                        exp.ExpenseFavour = item.ExpenseFavour;
                        _dbContext.Update(exp);
                        _dbContext.SaveChanges();
                    }
                }
                _dbContext.AddRange(expenses);
                _dbContext.SaveChanges();
                if (collection["coverNo"] != "")
                {
                    var id=Convert.ToInt32(collection["InsId"]);
                    var info = _dbContext.APInsuranceInfo.Where(x=>x.Id== id).FirstOrDefault();
                    info.LCId = aPLC.Id;
                    info.InsuranceCompanyId = Convert.ToInt32(collection["companyName"]);
                    info.CoverNoteNo = Convert.ToString(collection["coverNo"]);
                    info.Charges = Convert.ToDecimal(collection["charges"]);
                    info.CoverNoteDate = Convert.ToDateTime(collection["coverNoteDate"]);
                    await _APInsuranceInfoRepository.UpdateAsync(info);
                }
                TempData["error"] = "false";
                TempData["message"] = "Letter Of Credit has been updated successfully.";

            }
            return RedirectToAction("Index", "LetterOfCredit");
        }
        public async Task<IActionResult> Delete(int id)
        {
            if (id != 0)
            {
                APLC aPLC = _APLCRepository.Get(x => x.Id == id).FirstOrDefault();
                aPLC.IsDeleted = true;
                await _APLCRepository.UpdateAsync(aPLC);
                var Detail = _APInsuranceInfoRepository.Get(x => x.LCId == id).ToList();
                if (!ReferenceEquals(Detail, null))
                    await _APInsuranceInfoRepository.DeleteRangeAsync(Detail);
                TempData["error"] = "false";
                TempData["message"] = "Letter Of Credit has been deleted successfully.";
            }
            return RedirectToAction("List", "LetterOfCredit");
        }
        public async Task<IActionResult> Approve(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            APLC aPLC = _APLCRepository.Get(x => x.Id == id).FirstOrDefault();
            aPLC.ApprovedBy = _userId;
            aPLC.ApprovedDate = DateTime.UtcNow;
            aPLC.IsApproved = true;
            await _APLCRepository.UpdateAsync(aPLC);
            TempData["error"] = "false";
            TempData["message"] = "Letter Of Credit has been approved successfully.";
            return RedirectToAction("List", "LetterOfCredit");
        }
        public async Task<IActionResult> UnApprove(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
          //  bool check = _dbContext.APPurchaseRequisitionDetails.Any(x => x.APPurchaseRequisitionId == id && x.IsCSCreated == false && x.IsPOCreated == false);
            if (id !=0)
            {
                APLC aPLC = _APLCRepository.Get(x => x.Id == id).FirstOrDefault();
                aPLC.UnApprovedBy = _userId;
                aPLC.UnApprovedDate = DateTime.UtcNow;
                aPLC.IsApproved = false;
                await _APLCRepository.UpdateAsync(aPLC);
                TempData["error"] = "false";
                TempData["message"] = "Letter Of Credit has been UnApproved sucessfully.";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went Wrong";
            }
            return RedirectToAction("List", "LetterOfCredit");
        }
        public async Task<string> UploadFile(IFormFile img)
        {
            string filesList = "";
            if (img != null)
            {
                if (img.Length > 0)
                {
                    var fileName = Path.GetFileName(img.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads\\item-images", fileName);
                    using (var Fstream = new FileStream(filePath, FileMode.Create))
                    {
                        await img.CopyToAsync(Fstream);
                        var fullPath = "/uploads/item-images/" + fileName;
                        filesList += fullPath;
                    }
                }
            }
            return filesList;
        }

        public IActionResult Search(string id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var LCId = _dbContext.APLC.Where(x => x.LCNo == id).Select(x => x.Id).FirstOrDefault();
            var list = _dbContext.APGRNExpense.Where(x => x.LCId == LCId).ToList();
            List<APGRNExpense> aPGRNExpenses = new List<APGRNExpense>();
            foreach (var obj in list)
            {
                var acount = _dbContext.GLAccounts.Where(a => a.Id==Convert.ToInt32(obj.GLCode)).FirstOrDefault();
                obj.GLCode = acount.Code + "-" + acount.Name;
                obj.ExpenseDate = obj.ExpenseDate.Date;
                aPGRNExpenses.Add(obj);
            }
            return Ok(list);

        }
    }
}
