using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Repository.Helpers;
using Numbers.Repository.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;

namespace Numbers.Areas.AR.Controllers
{
    [Authorize]
    [Area("AP")]
    public class GRNController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public GRNController(NumbersDbContext context)
        {
            _dbContext = context;
        }
        public IActionResult Index()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string configs = _dbContext.AppCompanyConfigs
                      .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                      .Select(c => c.ConfigValue)
                      .FirstOrDefault();
            ViewBag.ReportUrl = configs;
            ViewBag.ReportPath = string.Concat(configs, "Viewer", "?Report=Voucher&cId=", companyId, "&id=");
            ViewBag.NavbarHeading = "List of GRN";
            List<APGRN> model;
            model = _dbContext.APGRN.Where(c => c.CompanyId == companyId && !c.IsDeleted).ToList();

            return View(model);
        }
        [HttpGet]
        public IActionResult Create(int id)
        {
            var configValue = new ConfigValues(_dbContext);
            ViewBag.Counter = 0;
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var appTaxRepo = new AppTaxRepo(_dbContext);
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            var responcibility = _dbContext.Sys_Responsibilities.FirstOrDefault(x=>x.Resp_Id == resp_Id).Resp_Name;
            ViewBag.Department = new SelectList(_dbContext.GLDivision.Where(x => x.CompanyId == companyId && x.IsDeleted == false && x.IsActive !=false).ToList(), "Id", "Name");
            ViewBag.OperatingUnit = configValue.GetOrgValues(resp_Id, "Operating Unit", companyId);
            ViewBag.InventoryOrganization = configValue.GetOrgValues(resp_Id, "Inventory Organization", companyId);
            ViewBag.Customers = (from a in _dbContext.ARCustomers.Where(x => /*x.CompanyId == companyId*/x.IsActive == true) select a.Name).ToList();
            //ViewBag.Customers = new SelectList(_dbContext.ARCustomers.Where(s => s.IsActive==true && s.CompanyId == companyId).ToList(), "Id", "Name").ToList();
            ViewBag.CostCenter = new SelectList(_dbContext.CostCenter.Where(a => a.IsDeleted == false && a.IsActive !=false).ToList(), "Id", "Description");
            ViewBag.Warehouse = configValue.GetConfigValues("Inventory", "Ware House", companyId);
            ViewBag.PO = new SelectList(_dbContext.APPurchaseOrders.Where(a => a.CompanyId == companyId && a.IsDeleted == false), "Id", "PONo");
            //ViewBag.Vendor = new SelectList(_dbContext.APSuppliers.Where(a => a.CompanyId == companyId && a.IsActive != false), "Id", "Name");
            ViewBag.Vendor = new SelectList(responcibility == "Yarn Purchase" ?
                        (from Suppliers in _dbContext.APSuppliers.Where(x => x.IsActive == true && x.Account.Code == "2.02.04.0003" /*&& x.CompanyId == companyId*/).Include(a => a.Account) select Suppliers) :
                        (from Suppliers in _dbContext.APSuppliers.Where(x => x.IsActive == true && x.Account.Code != "2.02.04.0003" /*&& x.CompanyId == companyId*/).Include(a => a.Account) select Suppliers), "Id", "Name");
            ViewBag.Responsibility = _dbContext.Sys_Responsibilities.FirstOrDefault(x => x.Resp_Id == resp_Id).Resp_Name;
            ViewBag.InvoiceItems = 0;
            ViewBag.TaxList= ViewBag.TaxList = appTaxRepo.GetTaxes(companyId);
            ViewBag.Locations= configValue.GetConfigValues("Inventory", "Location", companyId);
            ViewBag.Tax= new SelectList(_dbContext.AppTaxes.Include(t => t.SalesTaxAccount)
                .Include(t => t.ExciseTaxAccount).Include(t => t.IncomeTaxAccount).Where(x => x.CompanyId == companyId && x.IsDeleted == false).ToList(), "Id", "Name");
            ViewBag.GLCode = new SelectList(_dbContext.GLAccounts.Where(a => a.IsDeleted == false && a.AccountLevel==4).Select(a => new
            {
                id = a.Id,
                text = string.Concat(a.Code, " - ", a.Name),
                code = a.Code,
                name = a.Name
            }).ToList(), "id", "text");
            APGRNViewModel viewModel = new APGRNViewModel();
            viewModel.APGRNItems = new List<APGRNItem>();
            if (id == 0)
            {
                int maxGRN = 1;
                var GRN = _dbContext.APGRN.Where(c => c.CompanyId == companyId).ToList();
                if (GRN.Count > 0)
                {
                    maxGRN = GRN.Max(v => v.GRNNO);
                    TempData["GRNNo"] = maxGRN + 1;
                }
                else
                {
                    TempData["GRNNo"] = maxGRN;
                }
                var model = new APGRNViewModel();
                model.TaxList = appTaxRepo.GetTaxes(companyId);
                model.Currencies = AppCurrencyRepo.GetCurrencies();
                model.CostCenterList = _dbContext.CostCenter.Where(t => t.CompanyId == companyId && t.IsDeleted == false).ToList();
                ViewBag.Status = "Created";
                ViewBag.EntityState = "Create";
                ViewBag.NavbarHeading = "Create GRN";
                return View(model);
            }
            else
            {
                var model = _dbContext.APGRN.Find(id);
                var GRN = new APGRNViewModel();
                GRN.GRNNO = model.GRNNO;
                GRN.GRNDate = model.GRNDate;
                GRN.IRNNo = model.IRNNo;
                GRN.Vendor = model.Vendor;
                GRN.VendorName = model.VendorName;
                GRN.Warehouse = model.Warehouse;
                GRN.OperationId = model.OperationId;
                GRN.DepartmentId = model.DepartmentId;
                GRN.Total = model.Total;
                GRN.TotalSaleTax = model.TotalTaxAmount;
                GRN.Freight = model.Freight;
                GRN.TotalExpense = model.ExpenseAmount;
                GRN.GrandTotal = model.GrandTotal;
                GRN.TotalValue = model.TotalValue;
                GRN.TotalPKRValue = model.TotalPKRValue;
                GRN.TotalSaleTax = model.TotalTaxAmount;
                GRN.CurrencyExchangeRate = model.CurrencyExchangeRate;
                GRN.Currency = model.Currency;
                GRN.Remarks = model.Remarks;
                //TempData["GRNNo"] = model.GRNNO;
                ViewBag.Id = GRN.GRNNO;
                var invoiceItems = _dbContext.APGRNItem
                                    .Include(i => i.Item)
                                    .Where(i => i.IsDeleted == false && i.GRNID == id)
                                    .ToList();
                ViewBag.InvoiceItems = invoiceItems;
                APGRNItem[] poItems = _dbContext.APGRNItem.Where(u => u.GRNID == id && u.IsDeleted == false).Include(x => x.Item).ToArray();
                ViewBag.Items = poItems;
                viewModel.APGRNItems = poItems.ToList();
                //ViewBag.EntityState = "Update";
                GRN.TaxList = appTaxRepo.GetTaxes(companyId);
                GRN.Currencies = AppCurrencyRepo.GetCurrencies();
                GRN.CostCenterList = _dbContext.CostCenter.Where(t => t.CompanyId == companyId && t.IsDeleted == false).ToList();
                if (GRN.Status != "Approved")
                {
                    ViewBag.EntityState = "Create";
                    ViewBag.NavbarHeading = "Good Receipt Note";
                    ViewBag.TitleStatus = "Created";
                }
                GRN.APGRNItems = poItems.ToList();
                //else if (viewModel.Status == "Approved")
                //{
                //    ViewBag.NavbarHeading = "Sale Invoice";
                //    ViewBag.TitleStatus = "Approved";
                //}
                ViewBag.Expenses = _dbContext.APGRNExpense.Where(a => a.GRNId == id && a.CompanyId == companyId).ToList();
                return View(GRN);
            }

        }

        [HttpGet]
        public IActionResult GetInvoiceItems(int id, int itemId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var appTaxRepo = new AppTaxRepo(_dbContext);
            var item = _dbContext.APGRNItem.Include(i => i.Item).Where(i => i.Id == id && i.IsDeleted != true).FirstOrDefault();
            
            ViewBag.Counter = id;
            APGRNViewModel GRNItem = new APGRNViewModel();
            if (item != null)
            {
                GRNItem.IRNItemId = item.IRNItemId;
                GRNItem.ItemId = item.ItemId;
                GRNItem.LocationId = item.LocationId;
                GRNItem.HSCode = item.HSCode;
                GRNItem.GRNQty = item.GRNQty;
                GRNItem.RejectedQty = item.RejectedQty;
                GRNItem.AcceptedQty = item.AcceptedQty;
                GRNItem.BalanceQty = item.BalanceQty;
                GRNItem.Rate = item.Rate;
                GRNItem.Total = item.Total_;
                GRNItem.FCValue = item.FCValue;
                GRNItem.Expense = item.Expense;
                GRNItem.PKRValue = item.PKRValue;
                GRNItem.PKRRate = item.PKRRate;
                GRNItem.DetailCostCenter = item.DetailCostCenter;
              


                //viewModel.InvoiceItemId = item.Id;
                //viewModel.ItemId = item.ItemId;
                //viewModel.Qty = item.Qty;
                //viewModel.Stock = item.Stock;
                //viewModel.Rate = item.Rate;
                //viewModel.IssueRate = item.IssueRate;
                //viewModel.CostofSales = item.CostofSales;
                //viewModel.Total_ = item.Total;
                //viewModel.TaxSlab = item.TaxSlab;
                //viewModel.DiscountPercentage = item.DiscountPercentage;
                //viewModel.DiscountAmount = item.DiscountAmount;
                //viewModel.SalesTaxPercentage = item.SalesTaxPercentage;
                //viewModel.SalesTaxAmount = item.SalesTaxAmount;
                //viewModel.ExciseTaxPercentage = item.ExciseTaxPercentage;
                //viewModel.ExciseTaxAmount = item.ExciseTaxAmount;
                //viewModel.LineTotal = item.LineTotal;
                //viewModel.InvoiceItemRemarks = item.Remarks;
                ViewBag.ItemId = itemId;
                GRNItem.TaxList = appTaxRepo.GetTaxes(companyId);
                GRNItem.CostCenterList = _dbContext.CostCenter.Where(t => t.CompanyId == companyId && t.IsDeleted == false).ToList();
            }
            return PartialView("_partialInvoiceItems", GRNItem);
        }


         public int GetMaxGRNNo(int companyId)
        {
            int GRN = 1;
            var GRNNo = _dbContext.APGRN.Where(c => c.CompanyId == companyId).ToList();
            if (GRNNo.Count > 0)
            {
                GRN = GRNNo.Max(r => r.GRNNO);
                return GRN + 1;
            }
            else
            {
                return GRN;
            }
        }




        [HttpPost]
        public async Task<IActionResult> Create(APGRNViewModel model, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;

            string module = (from c in _dbContext.Sys_Responsibilities where c.Resp_Id == resp_Id select c.Resp_Name).FirstOrDefault();

            var supplier = _dbContext.ARCustomers.Where(m => m.Id == model.SupplierId).FirstOrDefault();

            if (model.Id == 0)
            {
                if (collection["ItemId"].Count > 0)
                {
                    APGRN GRN = new APGRN();
                    GRN.GRNNO = GetMaxGRNNo(companyId);
                    GRN.GRNDate = model.GRNDate;
                    GRN.IRNNo = model.IRNNo;
                    GRN.Vendor = model.Vendor;
                    GRN.VendorName = model.VendorName;
                    GRN.Warehouse = model.Warehouse;
                    GRN.DepartmentId = model.DepartmentId;
                    GRN.OperationId = model.OperationId;
                    GRN.Resp_ID = HttpContext.Session.GetInt32("Resp_ID").Value;
                    GRN.CreatedBy = userId;
                    GRN.IsDeleted = false;
                    GRN.CreatedDate = DateTime.Now;
                    GRN.Status = "Created";
                    GRN.Total = Convert.ToDecimal(collection["Total1"]);
                    GRN.TotalTaxAmount = model.TotalSaleTax;
                    GRN.Freight = model.Freight;
                    GRN.ExpenseAmount = model.TotalExpense;
                    GRN.GrandTotal = Convert.ToDecimal(collection["GrandTotal"]);
                    GRN.CompanyId = companyId;
                    GRN.Remarks = model.Remarks;
                    GRN.TotalValue = Convert.ToDecimal(collection["Total1"]);
                    GRN.TotalPKRValue = Convert.ToDecimal(collection["TotalPKRValue"]);
                    GRN.TotalTaxAmount = Convert.ToDecimal(collection["totalSalesTax"]);
                    GRN.CurrencyExchangeRate = model.CurrencyExchangeRate;
                    GRN.Currency = model.Currency;
                    GRN.CostCenter = model.CostCenter;

                    _dbContext.APGRN.Add(GRN);
                   await _dbContext.SaveChangesAsync();

                    for (int i = 0; i < collection["ItemId"].Count; i++)
                    {
                        var GRNItem = new APGRNItem();
                        GRNItem.GRNID = GRN.Id;
                        GRNItem.IRNItemId = Convert.ToInt32(collection["IRNItemId"][i]);
                        GRNItem.ItemId = Convert.ToInt32(collection["ItemId"][i]);
                        GRNItem.BrandId = module == "Yarn Purchase" ? Convert.ToInt32(collection["BrandId"][i]) : 0;
                        GRNItem.LocationId = Convert.ToInt32(collection["LocationId"][i]);
                        //GRNItem.HSCode = Convert.ToString(collection["HSCode"][i]);
                        GRNItem.GRNQty = Convert.ToDecimal(collection["GRNQty"][i]);
                        GRNItem.RejectedQty = Convert.ToDecimal(collection["RejectedQty"][i]);
                        GRNItem.AcceptedQty = Convert.ToDecimal(collection["AcceptedQty"][i]);
                        GRNItem.BalanceQty = Convert.ToDecimal(collection["BalanceQty"][i]);
                        GRNItem.Rate = Convert.ToDecimal(collection["Rate"][i]);
                        GRNItem.Total_ = Convert.ToDecimal(collection["Value"][i]);
                        //GRNItem.FCValue = Convert.ToDecimal(collection["FCValue"][i]);
                        //GRNItem.Expense = Convert.ToDecimal(collection["Expense"][i]);
                        GRNItem.PKRValue = Convert.ToDecimal(collection["Value"][i]);
                        GRNItem.PRDetailId = Convert.ToInt32(collection["PRDetailId"][i]);

                        try
                        {
                            GRNItem.SaleTax = Convert.ToInt32(collection["saleTax"][i]);
                            GRNItem.SaleTaxAmount = Convert.ToDecimal(collection["saleTaxAmount"][i]);
                            GRNItem.TotalValue = Convert.ToDecimal(collection["totalValue"][i]);
                        }
                        catch
                        {

                        }
                        
                        
                        //GRNItem.PKRRate = Convert.ToDecimal(collection["PKRRate"][i]);
                        //GRNItem.DetailCostCenter = Convert.ToInt32(collection["DetailCostCenter"][i]);

                        if (GRNItem.DetailCostCenter == 0)
                        {
                            GRNItem.DetailCostCenter = GRN.CostCenter;
                        }

                        
                        GRN.CreatedBy = userId;
                        GRN.CreatedDate = DateTime.Now;
                         _dbContext.APGRNItem.Add(GRNItem);

                        var IRNDetail = _dbContext.APIRNDetails.Find(GRNItem.IRNItemId);
                        if (IRNDetail != null)
                        {
                            IRNDetail.GRNQty = Convert.ToInt32(IRNDetail.GRNQty + Convert.ToInt32(GRNItem.GRNQty));
                            _dbContext.APIRNDetails.Update(IRNDetail);
                        }
                     await   _dbContext.SaveChangesAsync();
                    }
                    APGRNExpense[] Expense = JsonConvert.DeserializeObject<APGRNExpense[]>(collection["expenseDetails"]);

                    foreach (var item in Expense)
                    {
                        APGRNExpense expenseModel = new APGRNExpense();
                        expenseModel.AccountName = item.AccountName;
                        expenseModel.GLCode = item.GLCode;
                        expenseModel.ExpenseAmount = item.ExpenseAmount;
                        expenseModel.GRNId = GRN.Id;
                        expenseModel.CompanyId = companyId;
                        expenseModel.CreatedBy = userId;
                        expenseModel.CreatedDate = DateTime.Now;
                        expenseModel.IsDeleted = false;
                        expenseModel.ExpenseFavour = item.ExpenseFavour;
                        expenseModel.ExpenseDate = Convert.ToDateTime(item.ExpenseDate);
                        expenseModel.Remarks = item.Remarks;
                        _dbContext.APGRNExpense.Add(expenseModel);
                      await  _dbContext.SaveChangesAsync();
                    }


                    TempData["error"] = "false";
                    TempData["message"] = "GRN "+ GRN.GRNNO + " has been created successfully";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "No any GRN has been Created. It must contain atleast one item";
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                decimal grnQty = Convert.ToDecimal(0.000);
                APGRN GRN = _dbContext.APGRN.Find(model.Id);

                GRN.GRNNO = model.GRNNO;
                GRN.GRNDate = model.GRNDate;
                GRN.IRNNo = model.IRNNo;
                GRN.Vendor = model.Vendor;
                GRN.VendorName = model.VendorName;
                GRN.Warehouse = model.Warehouse;
                GRN.DepartmentId = model.DepartmentId;
                GRN.OperationId = model.OperationId;
                GRN.Resp_ID = HttpContext.Session.GetInt32("Resp_ID").Value;
                GRN.CreatedBy = userId;
                GRN.IsDeleted = false;
                GRN.CreatedDate = DateTime.Now;
                GRN.Status = "Created";
                GRN.Total = Convert.ToDecimal(collection["Total1"]); ;
                GRN.TotalTaxAmount = model.TotalSaleTax;
                GRN.Freight = model.Freight;
                GRN.ExpenseAmount = model.TotalExpense;
                GRN.GrandTotal = Convert.ToDecimal(collection["GrandTotal"]);
                GRN.CompanyId = companyId;
                GRN.Remarks = model.Remarks;
                GRN.TotalValue = Convert.ToDecimal(collection["Total1"]);
                GRN.TotalPKRValue = Convert.ToDecimal(collection["TotalPKRValue"]);
                GRN.TotalTaxAmount = Convert.ToDecimal(collection["totalSalesTax"]);
                GRN.CurrencyExchangeRate = model.CurrencyExchangeRate;
                GRN.Currency = model.Currency;
                GRN.CostCenter = model.CostCenter;

                _dbContext.APGRN.Update(GRN);
                _dbContext.SaveChanges();

                var list = _dbContext.APGRNItem.Where(l => l.GRNID == GRN.Id).ToList();
                if (list != null)
                {
                    for (int i = 0; i < collection["ItemId"].Count; i++)
                    {
                        var orderItem = _dbContext.APGRNItem
                            .Where(j => j.GRNID == GRN.Id && j.Id == Convert.ToInt32(collection["GRNItemId"][i] == "" ? 0 : Convert.ToInt32(collection["GRNItemId"][i]))).FirstOrDefault();
                        // Extract coresponding values from form collection
                        var itemId = Convert.ToInt32(collection["ItemId"][i]);

                        if (orderItem != null && itemId != 0)
                        {
                            grnQty = orderItem.GRNQty;
                            //var entityEntry = _dbContext.Entry(orderItem);
                            orderItem.GRNID = GRN.Id;
                            orderItem.IRNItemId = Convert.ToInt32(collection["IRNItemId"][i]);
                            orderItem.ItemId = Convert.ToInt32(collection["ItemId"][i]);
                            orderItem.LocationId = Convert.ToInt32(collection["LocationId"][i]);
                            //orderItem.ItemCode = Convert.ToString(collection["ItemCode"][i]);
                            //orderItem.ItemDescription = Convert.ToString(collection["ItemDescription"][i]);
                            orderItem.GRNQty = Convert.ToDecimal(collection["GRNQty"][i]);
                            orderItem.RejectedQty = Convert.ToDecimal(collection["RejectedQty"][i]);
                            orderItem.AcceptedQty = Convert.ToDecimal(collection["AcceptedQty"][i]);
                            orderItem.BalanceQty = Convert.ToDecimal(collection["BalanceQty"][i]);
                            orderItem.Rate = Convert.ToDecimal(collection["Rate"][i]);
                            orderItem.Total_ = Convert.ToDecimal(collection["Value"][i]);
                            orderItem.PRDetailId= Convert.ToInt32(collection["pRDetailId"][i]);
                            orderItem.SaleTax = Convert.ToInt32(collection["saleTax"][i]);
                            orderItem.SaleTaxAmount = Convert.ToDecimal(collection["saleTaxAmount"][i]);
                            orderItem.TotalValue = Convert.ToDecimal(collection["totalValue"][i]);
                            _dbContext.APGRNItem.Update(orderItem);

                            var IRNDetail = _dbContext.APIRNDetails.Find(orderItem.IRNItemId);
                            if (IRNDetail != null)
                            {
                                IRNDetail.GRNQty = Convert.ToInt32(IRNDetail.GRNQty - grnQty + Convert.ToInt32(orderItem.GRNQty));
                                _dbContext.APIRNDetails.Update(IRNDetail);
                            }

                             await _dbContext.SaveChangesAsync();
                            //dbEntry.OriginalValues.SetValues( entityEntry.GetDatabaseValuesAsync());
                        }
                        else if (orderItem == null && itemId != 0)
                        {
                            APGRNItem newItem = new APGRNItem();
                            newItem.GRNID = GRN.Id;
                            newItem.IRNItemId = Convert.ToInt32(collection["IRNItemId"][i]);
                            newItem.ItemId = Convert.ToInt32(collection["ItemId"][i]);
                            //newItem.ItemCode = Convert.ToString(collection["ItemCode"][i]);
                            //newItem.ItemDescription = Convert.ToString(collection["ItemDescription"][i]);
                            newItem.GRNQty = Convert.ToDecimal(collection["GRNQty"][i]);
                            newItem.RejectedQty = Convert.ToDecimal(collection["RejectedQty"][i]);
                            newItem.AcceptedQty = Convert.ToDecimal(collection["AcceptedQty"][i]);
                            newItem.BalanceQty = Convert.ToDecimal(collection["BalanceQty"][i]);
                            newItem.Rate = Convert.ToDecimal(collection["Rate"][i]);
                            newItem.Total_ = Convert.ToDecimal(collection["Value"][i]);
                            newItem.PRDetailId= Convert.ToInt32(collection["PRDetailId"][i]);
                            newItem.SaleTax = Convert.ToInt32(collection["saleTax"][i]);
                            newItem.SaleTaxAmount = Convert.ToDecimal(collection["saleTaxAmount"][i]);
                            newItem.TotalValue = Convert.ToDecimal(collection["totalValue"][i]);
                            _dbContext.APGRNItem.Add(newItem);

                            var IRNDetail = _dbContext.APIRNDetails.Find(newItem.IRNItemId);
                            if (IRNDetail != null)
                            {
                                IRNDetail.GRNQty = Convert.ToInt32(IRNDetail.GRNQty + Convert.ToInt32(newItem.GRNQty));
                                _dbContext.APIRNDetails.Update(IRNDetail);
                            }
                             await _dbContext.SaveChangesAsync();
                        }
                    }
                }

                //for (int i = 0; i < collection["ItemId"].Count; i++)
                //{
                //    var GRNItem = new APGRNItem();
                //    GRNItem.GRNID = GRN.Id;
                //    GRNItem.IRNItemId = Convert.ToInt32(collection["IRNItemId"][i]);
                //    GRNItem.ItemId = Convert.ToInt32(collection["ItemId"][i]);
                //    GRNItem.HSCode = Convert.ToString(collection["HSCode"][i]);
                //    GRNItem.GRNQty = Convert.ToDecimal(collection["GRNQty"][i]);
                //    GRNItem.RejectedQty = Convert.ToDecimal(collection["RejectedQty"][i]);
                //    GRNItem.AcceptedQty = Convert.ToDecimal(collection["AcceptedQty"][i]);
                //    GRNItem.BalanceQty = Convert.ToDecimal(collection["BalanceQty"][i]);
                //    GRNItem.Rate = Convert.ToDecimal(collection["Rate"][i]);
                //    GRNItem.Total_ = Convert.ToDecimal(collection["Total"][i]);
                //    GRNItem.FCValue = Convert.ToDecimal(collection["FCValue"][i]);
                //    GRNItem.Expense = Convert.ToDecimal(collection["Expense"][i]);
                //    GRNItem.PKRValue = Convert.ToDecimal(collection["PKRValue"][i]);
                //    GRNItem.PKRRate = Convert.ToDecimal(collection["PKRRate"][i]);
                //    GRNItem.DetailCostCenter = Convert.ToInt32(collection["DetailCostCenter"][i]);
                //    if (GRNItem.DetailCostCenter == 0)
                //    {
                //        GRNItem.DetailCostCenter = GRN.CostCenter;
                //    }
                //    GRN.CreatedBy = userId;
                //    GRN.CreatedDate = DateTime.Now;
                //    _dbContext.APGRNItem.Add(GRNItem);

                //    var IRNDetail = _dbContext.APIRNDetails.Find(GRNItem.IRNItemId);
                //    if (IRNDetail != null)
                //    {
                //        IRNDetail.GRNQty = Convert.ToInt32(IRNDetail.GRNQty + Convert.ToInt32(GRNItem.GRNQty));
                //        _dbContext.APIRNDetails.Update(IRNDetail);
                //    }
                //    _dbContext.SaveChanges();
                //}



                var removeExpense = _dbContext.APGRNExpense.Where(u => u.GRNId == GRN.Id && u.CompanyId == companyId).ToList();

                foreach (var item in removeExpense)
                {
                    APGRNExpense removeModel = _dbContext.APGRNExpense.Find(item.Id);
                    _dbContext.APGRNExpense.Remove(removeModel);
                    _dbContext.SaveChanges();

                }

                APGRNExpense[] Expense = JsonConvert.DeserializeObject<APGRNExpense[]>(collection["expenseDetails"]);

                foreach (var item in Expense)
                {
                    APGRNExpense expenseModel = new APGRNExpense();
                    expenseModel.AccountName = item.AccountName;
                    expenseModel.GLCode = item.GLCode;
                    expenseModel.ExpenseAmount = item.ExpenseAmount;
                    expenseModel.GRNId = GRN.Id;
                    expenseModel.CompanyId = companyId;
                    expenseModel.CreatedBy = userId;
                    expenseModel.CreatedDate = DateTime.Now;
                    expenseModel.IsDeleted = false;
                    expenseModel.ExpenseFavour = item.ExpenseFavour;
                    expenseModel.ExpenseDate= Convert.ToDateTime(item.ExpenseDate);
                    expenseModel.Remarks= item.Remarks;
                    _dbContext.APGRNExpense.Add(expenseModel);
                  await  _dbContext.SaveChangesAsync();
                }


                TempData["error"] = "false";
                TempData["message"] = "GRN "+ GRN.Id + " has been created successfully";
                return RedirectToAction(nameof(Index));


            }
        }

        //[HttpPost]
        //public async Task<IActionResult> Update(ARInvoiceViewModel model, IFormCollection collection)
        //{
        //    //deleting invoices
        //    string[] idsDeleted = Convert.ToString(collection["IdsDeleted"]).Split(",");
        //    if (!idsDeleted.Contains(""))
        //    {
        //        for (int j = 0; j < idsDeleted.Length; j++)
        //        {
        //            if (idsDeleted[j] != "0")
        //            {
        //                var itemToRemove = _dbContext.ARInvoiceItems.Find(Convert.ToInt32(idsDeleted[j]));
        //                itemToRemove.IsDeleted = true;
        //                _dbContext.ARInvoiceItems.Update(itemToRemove);
        //                await _dbContext.SaveChangesAsync();
        //            }
        //        }
        //    }
        //    if (collection["ItemId"].Count > 0)
        //    {
        //        //updating existing data
        //        int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
        //        string userId = HttpContext.Session.GetString("UserId");
        //        ARInvoice invoice = _dbContext.ARInvoices.Find(model.Id);
        //        invoice.InvoiceNo = model.InvoiceNo;
        //        invoice.InvoiceDate = model.InvoiceDate;
        //        invoice.InvoiceDueDate = model.InvoiceDueDate;
        //        invoice.WareHouseId = model.WareHouseId;
        //        invoice.CustomerId = model.CustomerId;
        //        invoice.ReferenceNo = model.ReferenceNo;
        //        invoice.CustomerPONo = model.CustomerPONo;
        //        invoice.OGPNo = model.OGPNo;
        //        invoice.Vehicle = model.Vehicle;
        //        invoice.Remarks = collection["Remarks"][0];
        //        invoice.Currency = model.Currency;
        //        invoice.CurrencyExchangeRate = model.CurrencyExchangeRate;
        //        invoice.Total = Convert.ToDecimal(collection["Total"]);
        //        invoice.DiscountAmount = Convert.ToDecimal(collection["totalDiscountAmount"]);
        //        invoice.SalesTaxAmount = Convert.ToDecimal(collection["totalSalesTaxAmount"]);
        //        invoice.ExciseTaxAmount = Convert.ToDecimal(collection["totalExciseTaxAmount"]);
        //        invoice.GrandTotal = Convert.ToDecimal(collection["GrandTotal"]);
        //        invoice.UpdatedBy = userId;
        //        invoice.UpdatedDate = DateTime.Now;
        //        var entry = _dbContext.ARInvoices.Update(invoice);
        //        entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
        //        await _dbContext.SaveChangesAsync();
        //        //Update Invoice detail items/ Invoice items if any
        //        for (int i = 0; i < collection["ItemId"].Count; i++)
        //        {
        //            var invoiceItem = _dbContext.ARInvoiceItems
        //                .Where(j => j.InvoiceId == model.Id && j.Id == Convert.ToInt32(collection["InvoiceItemId"][i] == "" ? 0 : Convert.ToInt32(collection["InvoiceItemId"][i]))).FirstOrDefault();
        //            // Extract coresponding values from form collection
        //            var itemId = Convert.ToInt32(collection["ItemId"][i]);
        //            var avgRate = _dbContext.InvItems.Find(itemId).AvgRate;
        //            var qty = Convert.ToDecimal(collection["Qty"][i]);
        //            var stock = Convert.ToDecimal(collection["Stock"][i]);
        //            var rate = Convert.ToDecimal(collection["Rate"][i]);
        //            var issueRate = Convert.ToDecimal(collection["IssueRate"][i]);
        //            var costofSales = Convert.ToDecimal(collection["CostofSales"][i]);
        //            var total = Convert.ToDecimal(collection["Total_"][i]);
        //            var taxSlab = Convert.ToInt32(collection["TaxSlab"][i]);
        //            var discountPercentage = Convert.ToDecimal(collection["DiscountPercentage"][i]);
        //            var discountAmount = Convert.ToDecimal(collection["DiscountAmount"][i]);
        //            var salesTaxPercentage = Convert.ToDecimal(collection["SalesTaxPercentage"][i]);
        //            var salesTaxAmount = Convert.ToDecimal(collection["SalesTaxAmount"][i]);
        //            var exciseTaxPercentage = Convert.ToDecimal(collection["ExciseTaxPercentage"][i]);
        //            var exciseTaxAmount = Convert.ToDecimal(collection["ExciseTaxAmount"][i]);
        //            var linetotal = Convert.ToDecimal(collection["LineTotal"][i]);
        //            var remarks = collection["Remarks"][i + 1];
        //            var saleOrderItemId = Convert.ToInt32(collection["SalesOrderItemId"][i]);

        //            if (invoiceItem != null && itemId != 0)
        //            {
        //                //below phenomenon prevents Id from being marked as modified
        //                var entityEntry = _dbContext.Entry(invoiceItem);
        //                entityEntry.State = EntityState.Modified;
        //                entityEntry.Property(p => p.Id).IsModified = false;
        //                invoiceItem.ItemId = itemId;
        //                invoiceItem.InvoiceId = model.Id;
        //                invoiceItem.Qty = qty;
        //                invoiceItem.Stock = stock;
        //                invoiceItem.Bonus = 0;
        //                invoiceItem.DiscountAmount = 0;
        //                invoiceItem.Rate = rate;
        //                invoiceItem.IssueRate = issueRate;
        //                invoiceItem.CostofSales = costofSales;
        //                invoiceItem.AvgRate = avgRate;
        //                invoiceItem.Total = total;
        //                invoiceItem.TaxSlab = taxSlab;
        //                invoiceItem.DiscountPercentage = discountPercentage;
        //                invoiceItem.DiscountAmount = discountAmount;
        //                invoiceItem.SalesTaxPercentage = salesTaxPercentage;
        //                invoiceItem.SalesTaxAmount = salesTaxAmount;
        //                invoiceItem.ExciseTaxPercentage = exciseTaxPercentage;
        //                invoiceItem.ExciseTaxAmount = exciseTaxAmount;
        //                invoiceItem.LineTotal = linetotal;
        //                invoiceItem.Remarks = remarks;

        //                var dbEntry = _dbContext.ARInvoiceItems.Update(invoiceItem);
        //                dbEntry.OriginalValues.SetValues(await entityEntry.GetDatabaseValuesAsync());

        //            }
        //            //check if user created new item while updating
        //            else if (itemId != 0 && invoiceItem == null)//itemId is invitem, if this is null or zero rest of the information for this item will not be saved.
        //            {
        //                var newItem = new ARInvoiceItem();
        //                newItem.InvoiceId = model.Id;
        //                newItem.ItemId = itemId;
        //                newItem.Qty = qty;
        //                newItem.Stock = stock;
        //                newItem.Rate = rate;
        //                newItem.IssueRate = issueRate;
        //                newItem.CostofSales = costofSales;
        //                newItem.AvgRate = avgRate;
        //                newItem.Total = total;
        //                newItem.TaxSlab = taxSlab;
        //                newItem.DiscountPercentage = discountPercentage;
        //                newItem.DiscountAmount = discountAmount;
        //                newItem.SalesTaxPercentage = salesTaxPercentage;
        //                newItem.SalesTaxAmount = salesTaxAmount;
        //                newItem.ExciseTaxPercentage = exciseTaxPercentage;
        //                newItem.ExciseTaxAmount = exciseTaxAmount;
        //                newItem.LineTotal = linetotal;
        //                newItem.Remarks = remarks;
        //                newItem.SalesOrderItemId = saleOrderItemId;
        //                _dbContext.ARInvoiceItems.Add(newItem);
        //            }
        //        }
        //        TempData["error"] = "false";
        //        TempData["message"] = "Invoice has been updated successfully";
        //        await _dbContext.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    else
        //    {
        //        TempData["error"] = "true";
        //        TempData["message"] = "No any Invoice has been updated. It must contain atleast one item";
        //        return RedirectToAction(nameof(Index));
        //    }
        //}
        public IActionResult GetUOM(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var items = from item in _dbContext.InvItems
                        join config in _dbContext.AppCompanyConfigs on item.Id equals id
                        where item.Id == id && config.ConfigName == "UOM" && config.Module == "Inventory" && config.Id == item.Unit && config.CompanyId == companyId
                        select new
                        {
                            uom = config.ConfigValue,
                            id = config.Id,
                            rate = item.PurchaseRate,
                            avgRate = item.AvgRate,
                            stock = item.StockAccountId
                        };
            return Ok(items);
        }
        public async Task<IActionResult> Approve(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");

            //BkgVehicleIGP bkgIGP = _dbContext.BkgVehicleIGPs.Find(id);
            APGRN invoice = _dbContext.APGRN
           
             .Where(a => a.Status == "Created" && a.Id == id)
             .FirstOrDefault();
            AppTax appTax = _dbContext.AppTaxes.Where(x => x.Name == "GRN").FirstOrDefault();
            try
            {
                //Create Voucher
                Numbers.Helpers.VoucherHelper voucher = new Numbers.Helpers.VoucherHelper(_dbContext, HttpContext);
                GLVoucher voucherMaster = new GLVoucher();
                List<GLVoucherDetail> voucherDetails = new List<GLVoucherDetail>();
                string voucherDescription = string.Format(
                "GRN # : {0} of  " +
                "{1} {2}",
                invoice.GRNNO,
                (_dbContext.APSuppliers.Where(a=>a.Id==Convert.ToInt32(invoice.VendorName)).Select(a=>a.Name).FirstOrDefault()), invoice.Remarks);

                int voucherId;
                voucherMaster.Id = 0;
                voucherMaster.VoucherType = "GRN";
                voucherMaster.VoucherDate = invoice.GRNDate;
                // voucherMaster.Reference = invoice.ReferenceNo;
                voucherMaster.Currency ="PKR";
                voucherMaster.CurrencyExchangeRate =1;
                voucherMaster.Description = voucherDescription.Substring(0, voucherDescription.Length);
                voucherMaster.Status = "Approved";
                voucherMaster.ApprovedBy = _userId;
                voucherMaster.ApprovedDate = DateTime.Now;
                voucherMaster.IsSystem = true;
                voucherMaster.ModuleName = "AP/GRN";
                voucherMaster.ModuleId = id;
                voucherMaster.Amount = invoice.TotalValue;
                //Voucher Details
                var invoiceItems = _dbContext.APGRNItem.Where(i => i.GRNID == invoice.Id && !i.IsDeleted).ToList();
                var GrnExpense= _dbContext.APGRNExpense
                    .Where(i => i.GRNId == invoice.Id && !i.IsDeleted).ToList();
                var amount =_dbContext.APGRN.Where(i => i.Id == invoice.Id && !i.IsDeleted).Select(a=>a.GrandTotal).FirstOrDefault();
                //    var discount = invoiceItems.Sum(s => s.DiscountAmount);
                //Credit Entry
                GLVoucherDetail voucherDetail = new GLVoucherDetail();
                var accountId = (from setup in _dbContext.AppCompanySetups.Where(x => x.Name == "GRN Credit Account") join
                            account in _dbContext.GLAccounts.Where(x=>!x.IsDeleted) on setup.Value equals account.Code
                            select account.Id).FirstOrDefault();

                voucherDetail.AccountId = accountId; // ( UnBilled Purchases)  (GRN Credit Account)
              //(_dbContext.APSuppliers.Where(a => a.Id == Convert.ToInt32(invoice.VendorName)).Select(a => a.AccountId).FirstOrDefault());
                voucherDetail.Sequence = 10;
                voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                voucherDetail.Debit = 0;
               // voucherDetail.Credit = invoice.GrandTotal + invoice.ExpenseAmount;
                voucherDetail.Credit = invoice.Total;
               // voucherDetail.Credit = invoice.GrandTotal;
                voucherDetail.IsDeleted = false;
                voucherDetail.CreatedBy = _userId;
                voucherDetail.CreatedDate = DateTime.Now;
                voucherDetails.Add(voucherDetail);
                foreach (var item in invoiceItems)
                {
                    GLVoucherDetail voucherDetailitem = new GLVoucherDetail();
                    var itemaccid = _dbContext.InvItems.Where(a => a.Id == item.ItemId).Select(a => a.InvItemAccountId).FirstOrDefault();
                    voucherDetailitem.AccountId = (_dbContext.InvItemAccounts.Where(c=>c.Id==itemaccid).Select(v=>v.GLAssetAccountId).FirstOrDefault());
                    voucherDetailitem.Sequence = 10;
                    voucherDetailitem.Description = voucherDescription.Substring(0, voucherDescription.Length);
                    //voucherDetailitem.Debit = item.PKRValue;
                    voucherDetailitem.Debit += item.Total_;
                    voucherDetailitem.Credit = 0;
                    voucherDetailitem.IsDeleted = false;
                    voucherDetailitem.CreatedBy = _userId;
                    voucherDetailitem.CreatedDate = DateTime.Now;
                    voucherDetails.Add(voucherDetailitem);
                }
                var voucherDetailsGroup = voucherDetails.GroupBy(x => new { x.AccountId })
               .Select(item => new GLVoucherDetail
               {
                   AccountId = item.Select(x => x.AccountId).FirstOrDefault(),
                   Sequence = item.Select(x => x.Sequence).FirstOrDefault(),
                   Description = item.Select(x => x.Description).FirstOrDefault(),
                   Debit = item.Select(x => x.Debit).Sum(),
                   Credit = item.Select(x => x.Credit).Sum(),
                   IsDeleted = false,
                   CreatedBy = _userId,
                   CreatedDate = DateTime.Now,
            }).ToList();
                //GLVoucherDetail voucherDetalitem = new GLVoucherDetail();
                //voucherDetalitem.Debit = invoice.ExpenseAmount;
                //voucherDetalitem.Credit = 0;
                //voucherDetalitem.IsDeleted = false;
                //voucherDetalitem.CreatedBy = _userId;
                //voucherDetalitem.CreatedDate = DateTime.Now;
                //voucherDetails.Add(voucherDetalitem);

                //foreach (var item in GrnExpense)
                //{
                //    GLVoucherDetail voucherDetailex = new GLVoucherDetail();

                //    voucherDetailex.AccountId = Convert.ToInt32(item.GLCode);
                //    voucherDetailex.Sequence = 10;
                //    voucherDetailex.Description = voucherDescription.Substring(0, voucherDescription.Length);
                //    voucherDetailex.Debit = item.ExpenseAmount;
                //    voucherDetailex.Credit = 0;
                //    voucherDetailex.IsDeleted = false;
                //    voucherDetailex.CreatedBy = _userId;
                //    voucherDetailex.CreatedDate = DateTime.Now;
                //    voucherDetails.Add(voucherDetailex);
                //}

                //GLVoucherDetail voucherDetailSalesTax = new GLVoucherDetail();

                //voucherDetailSalesTax.AccountId = appTax.SalesTaxAccountId;
                //voucherDetailSalesTax.Sequence = 10;
                //voucherDetailSalesTax.Description = voucherDescription.Substring(0, voucherDescription.Length);
                //voucherDetailSalesTax.Debit = invoice.TotalTaxAmount;
                //voucherDetailSalesTax.Credit =0 ;
                //voucherDetailSalesTax.IsDeleted = false;
                //voucherDetailSalesTax.CreatedBy = _userId;
                //voucherDetailSalesTax.CreatedDate = DateTime.Now;
                //voucherDetails.Add(voucherDetailSalesTax);


                //GLVoucherDetail voucherDetailFreight = new GLVoucherDetail();

                //voucherDetailFreight.AccountId = appTax.IncomeTaxAccountId;
                //voucherDetailFreight.Sequence = 10;
                //voucherDetailFreight.Description = voucherDescription.Substring(0, voucherDescription.Length);
                //voucherDetailFreight.Debit = invoice.Freight;
                //voucherDetailFreight.Credit = 0;
                //voucherDetailFreight.IsDeleted = false;
                //voucherDetailFreight.CreatedBy = _userId;
                //voucherDetailFreight.CreatedDate = DateTime.Now;
                //voucherDetails.Add(voucherDetailFreight);

                //Create Voucher 


                voucherId = voucher.CreateVoucher(voucherMaster, voucherDetailsGroup);
                if (voucherId != 0)
                {
                    invoice.VoucherId = voucherId;
                    invoice.Status = "Approved";
                    invoice.ApprovedBy = _userId;
                    invoice.ApprovedDate = DateTime.Now;

                    //On approval updating Invoice                   
                    //var entry = 
                    _dbContext.Update(invoice);
                    //entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                    //On approval updating InvItems
                    var grnItems = _dbContext.APGRNItem.Where(p => p.IsDeleted == false && p.GRNID == id).ToList();
                    using (var transaction = _dbContext.Database.BeginTransaction())
                    {
                        await _dbContext.SaveChangesAsync();
                        foreach (var invoiceItem in invoiceItems)
                        {
                            var item = _dbContext.InvItems.Find(invoiceItem.ItemId);
                            item.StockQty = item.StockQty + invoiceItem.AcceptedQty;
                            item.StockValue = item.StockValue + (invoiceItem.Rate * invoiceItem.AcceptedQty);
                            if (item.StockQty != 0)
                            {
                                item.AvgRate = Math.Round(item.StockValue / item.StockQty, 6);
                            }   
                                //item.StockQty = item.StockQty - invoiceItem.AcceptedQty;
                                //item.StockValue = item.StockValue - (item.AvgRate * invoiceItem.AcceptedQty);
                                var dbEntry = _dbContext.InvItems.Update(item);
                            dbEntry.OriginalValues.SetValues(await dbEntry.GetDatabaseValuesAsync());
                            await _dbContext.SaveChangesAsync();
                        }
                        await _dbContext.SaveChangesAsync();
                        //var lineItems = _dbContext.ARInvoiceItems.Where(i => i.IsDeleted == false && i.InvoiceId == id).ToList();
                        //foreach (var item in lineItems)
                        //{
                        //    if (item.SalesOrderItemId != 0)
                        //    {
                        //        var saleOrderItem = _dbContext.ARSaleOrderItems.Find(item.SalesOrderItemId);
                        //        saleOrderItem.SaleQty = saleOrderItem.SaleQty + item.Qty;
                        //        var dbEntry = _dbContext.ARSaleOrderItems.Update(saleOrderItem);
                        //        dbEntry.OriginalValues.SetValues(await dbEntry.GetDatabaseValuesAsync());
                        //        await _dbContext.SaveChangesAsync();
                        //    }
                        //}
                        transaction.Commit();
                    }
                    TempData["error"] = "false";
                    TempData["message"] = "GRN has been approved successfully";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Voucher Not Created";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                TempData["error"] = "true";
                TempData["message"] = ex.Message.ToString();
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost]
        public IActionResult PartialInvoiceItems(int? counter)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var appTaxRepo = new AppTaxRepo(_dbContext);
            ViewBag.Counter = counter;
            var model = new APGRNViewModel();
            model.TaxList = appTaxRepo.GetTaxes(companyId);
            model.CostCenterList = _dbContext.CostCenter.Where(t => t.CompanyId == companyId && t.IsDeleted == false).ToList();
            return PartialView("_partialInvoiceItems", model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var invoice = _dbContext.APGRN.Find(id);
            invoice.IsDeleted = true;
            var entry = _dbContext.APGRN.Update(invoice);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();

            var GrnItem = _dbContext.APGRNItem.Where(x => x.IsDeleted == false && x.GRNID == id).ToList();
            foreach(var item in GrnItem)
            {
                if (item.IRNItemId != 0 && item.IRNItemId != null)
                {
                    var irnItem = _dbContext.APIRNDetails.Find(item.IRNItemId);

                    irnItem.GRNQty = irnItem.GRNQty - Convert.ToInt32( item.GRNQty);
                    _dbContext.APIRNDetails.Update(irnItem);
                    _dbContext.SaveChanges();
                }

            }
            TempData["error"] = "false";
            TempData["message"] = string.Format("GRN No {0} has been deleted", invoice.GRNNO);
            return RedirectToAction(nameof(Index));
        }
        public IActionResult UnApprove()
        {
            ViewBag.NavbarHeading = "Un-Approve GRN";
            return View(_dbContext.ARInvoices
               .Where(v => v.Status == "Approved" && v.IsDeleted == false && v.TransactionType == "Sale" && v.CompanyId == HttpContext.Session.GetInt32("CompanyId")).ToList());
        }
        public async Task<IActionResult> UnApproveVoucher(int id)
        {
            try
            {
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                var voucher = _dbContext.APGRN
                                .Where(v => v.IsDeleted == false && v.Id == id && v.Status == "Approved" && v.CompanyId == companyId).FirstOrDefault();
                if (voucher != null)
                {
                    using (var transaction = _dbContext.Database.BeginTransaction())
                    {
                        var voucherDetail = _dbContext.GLVoucherDetails.Where(v => v.VoucherId == voucher.VoucherId).ToList();
                        foreach (var item in voucherDetail)
                        {
                            var tracker = _dbContext.GLVoucherDetails.Remove(item);
                            tracker.OriginalValues.SetValues(await tracker.GetDatabaseValuesAsync());
                            await _dbContext.SaveChangesAsync();
                        }
                        var invoiceItems = _dbContext.APGRNItem.Where(i => i.GRNID == id && !i.IsDeleted).ToList();
                        //foreach (var invoiceItem in invoiceItems)
                        //{
                        //    var item = _dbContext.InvItems.Find(invoiceItem.ItemId);
                        //    item.StockQty = item.StockQty - invoiceItem.AcceptedQty;
                        //    item.StockValue = item.StockValue + (item.AvgRate * invoiceItem.AcceptedQty);
                        //    var dbEntry = _dbContext.InvItems.Update(item);
                        //    dbEntry.OriginalValues.SetValues(await dbEntry.GetDatabaseValuesAsync());
                        //    await _dbContext.SaveChangesAsync();
                        //}

                        foreach (var invoiceItem in invoiceItems)
                        {
                            var item = _dbContext.InvItems.Find(invoiceItem.ItemId);
                            item.StockQty = item.StockQty - invoiceItem.AcceptedQty;
                            item.StockValue = item.StockValue - (item.AvgRate * invoiceItem.AcceptedQty);
                            //new changings
                            if (item.StockQty != 0)
                            {
                                item.AvgRate = Math.Round(item.StockValue / item.StockQty, 6);
                            }
                            var dbEntry = _dbContext.InvItems.Update(item);
                            dbEntry.OriginalValues.SetValues(await dbEntry.GetDatabaseValuesAsync());
                            await _dbContext.SaveChangesAsync();
                        }

                        voucher.Status = "Created";
                        voucher.ApprovedBy = null;
                        voucher.ApprovedDate = DateTime.Now;
                        var entry = _dbContext.APGRN.Update(voucher);
                        entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                        await _dbContext.SaveChangesAsync();
                        TempData["error"] = "false";
                        TempData["message"] = "GRN has been Un-Approved successfully";
                        transaction.Commit();
                    }
                }
            }
            catch (Exception exc)
            {
                TempData["error"] = "true";
                TempData["message"] = exc.Message == null ? exc.InnerException.Message.ToString() : exc.Message.ToString();
            }
            return RedirectToAction(nameof(UnApprove));
        }
        //[HttpPost]
        //public IActionResult GetPOByCustomerId(int id, int[] skipIds)
        //{
        //    int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
        //    var configValues = new ConfigValues(_dbContext);
        //    //var invoice = GetPurchaseOrdersByCustomerId(id, skipIds, companyId);
        //    var POs = GetPurchaseOrdersByCustomerId(id, skipIds);
        //    List<ARInvoiceViewModel> list = new List<ARInvoiceViewModel>();
        //    foreach (var item in invoice)
        //    {
        //        var po = POs Where(c => c.ItemId == item.ItemId).FirstOrDefault();
        //        var model = new ARInvoiceViewModel();
        //        //model.PurchaseInvoiceId = item.Purchase.Id;
        //        model.SalesOrderItemId = item.Id;
        //        model.InvoiceNo = item.SaleOrder.SaleOrderNo;
        //        model.InvoiceDate = item.SaleOrder.SaleOrderDate;
        //        model.ItemId = item.ItemId;
        //        model.Item = item.Item;
        //        model.Remarks = configValues.GetUom(item.Item.Unit);
        //        model.Qty =Dc.Qty  ;
        //        model.Rate = item.Rate;
        //        model.GrandTotal = Math.Round(model.Rate * model.Qty, 2);
        //        model.TotalDiscountAmount = item.SaleQty;
        //        list.Add(model);
        //    }
        //    return PartialView("_PurchaseOrderPopUp", list.ToList());
        //}
        public List<ARDeliveryChallanItem> GetAllDCByCustomerId(int id, int[] skipIds, int companyId)
        {
            var DCs = _dbContext.ARDeliveryChallans.Where(x => x.CustomerId == id && x.IsDeleted == false && x.CompanyId == companyId).Select(c => c.Id).ToList();
            var DCItems = _dbContext.ARDeliveryChallanItems
                .Where(i => DCs.Contains(i.DeliveryChallanId) && i.IsDeleted == false
                        )
                .Where(i => !skipIds.Contains(i.Id)).ToList();
            return DCItems;
        }
        [HttpPost]
        public IActionResult GetPurchaseOrdersByCustomerId(int id, int[] skipIds)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var configValues = new ConfigValues(_dbContext);
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            var POId = _dbContext.APIRN.Where(a => a.CompanyId == companyId && a.Resp_ID == resp_Id && a.IsDeleted == false && a.VendorID == id).Select(a => a.Id).ToArray();
            var PO = GetAllIRNByVendorId(POId, skipIds, companyId);
            List<APGRNViewModel> list = new List<APGRNViewModel>();
            foreach (var item in PO)
            {
                var model = new APGRNViewModel();
                var PODetailId = _dbContext.APIGPDetails.Where(x => x.Id == item.IGPDetailId).Select(x => x.PoDetailId).FirstOrDefault();
                var PODetail = _dbContext.APPurchaseOrderItems.Where(x => x.Id == PODetailId).FirstOrDefault();
                if (PODetail != null)
                {
                    var IRN = _dbContext.APIRN.Where(a => a.Id == item.IRNID).FirstOrDefault();
                    model.Item = new InvItem();
                    model.Item.Id = item.ItemID;
                    model.PO = new APPurchaseOrder();
                    //model.PurchaseInvoiceId = item.Purchase.Id;
                    model.Item.Code = item.Item.Code;
                    model.SaleTaxId = PODetail.TaxId;
                    model.Item.PurchaseRate = PODetail.Rate;
                    model.Item.Name = item.Item.Name;
                    model.GRNQty = item.IGP_Qty;
                    model.IRNNo = IRN.IRNNo;
                    model.Freight = Convert.ToDecimal(_dbContext.APIGP.Where(x => x.IGP == IRN.IGPNo).Select(x => x.FreightAmount).FirstOrDefault());
                    model.PRDetailId = item.PrDetailId;
                    model.POId = item.IRNID;
                    model.Id = item.Id;
                    model.RejectedQty = item.Rejected_Qty;
                    model.AcceptedQty = item.Accepted_Qty;
                    model.BalanceQty = item.Accepted_Qty - item.GRNQty;
                    model.GrandTotal = Math.Round(model.Rate * model.GRNQty, 2);
                    model.LocationId = item.Item.LocationIfTrue;
                    //if (item.BrandId != 0) { 
                    //    model.BrandId = item.BrandId;
                    //    model.Brand = _dbContext.AppCompanyConfigs.FirstOrDefault(x=>x.Id == Convert.ToInt32(item.BrandId)).ConfigValue;
                    //}
                    //  = Convert.ToInt32(_dbContext.APPurchaseOrderItems.Where(a => a.PrDetailId == item.PrDetailId).Select(x => x.Rate).FirstOrDefault());
                    //item.Item.PurchaseRate;

                    //model.TotalDiscountAmount = item.SaleQty;
                    list.Add(model);
                }
            }
            return PartialView("_PurchaseOrderPopUp", list.ToList().OrderByDescending(x=>x.Id));
        }

        public List<APIRNDetails> GetAllIRNByVendorId(int[] id, int[] skipIds, int companyId)
        {
            var IRNItems = _dbContext.APIRNDetails.Include(i => i.Item)
                .Where(i => id.Contains(i.IRNID) && i.GRNQty != i.Accepted_Qty)
                .Where(i => !skipIds.Contains(i.Id)).ToList();
            return IRNItems;
        }
        [HttpPost]
        public IActionResult GetPurchaseOrderItems(int purchaseOrderItemId, int counter)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var appTaxRepo = new AppTaxRepo(_dbContext);
            // var saleorder = _dbContext.ARSaleOrderItems.Where(x => x.Id == saleOrderItemId).Select(c => c.SaleOrderId).FirstOrDefault();
            var dc = _dbContext.ARDeliveryChallanItems.Where(x => x.SaleOrderId == purchaseOrderItemId).FirstOrDefault();
            var item = GetPurchaseOrderItem(purchaseOrderItemId);
            //item.Qty = dc.Qty;
            ViewBag.Counter = counter;
            ViewBag.ItemId = item.ItemId;
            item.TaxList = appTaxRepo.GetTaxes(companyId);
            item.CostCenterList = _dbContext.CostCenter.Where(t => t.CompanyId == companyId && t.IsDeleted == false).ToList();
            return PartialView("_partialInvoiceItems", item);
        }

        public APGRNViewModel GetPurchaseOrderItem(int id)
        {
            var item = _dbContext.APIRNDetails
                       .Include(i => i.Item)
                       .Where(i => i.Id == id)
                       .FirstOrDefault();
            APGRNViewModel viewModel = new APGRNViewModel();
            viewModel.IRNItemId = item.Id;
            viewModel.ItemId = item.ItemID;
            viewModel.GRNQty = item.IGP_Qty-item.GRNQty;
            viewModel.BalanceQty = 0;
            viewModel.AcceptedQty = item.Accepted_Qty - item.GRNQty;
            viewModel.RejectedQty = item.Rejected_Qty;
            viewModel.IRNQty =Convert.ToInt32( viewModel.GRNQty );
            viewModel.HSCode = "";
            viewModel.Rate = item.Item.PurchaseRate;
            //viewModel.Total = item.Total;


            return viewModel;
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            APGRN aPGRN = new APGRN();
            aPGRN = _dbContext.APGRN
                .Include(i => i.Operation)
                .Where(i => i.Id == id).FirstOrDefault();
            aPGRN.VendorNme = _dbContext.APSuppliers.FirstOrDefault(x => x.Id == Convert.ToInt32(aPGRN.VendorName)).Name;
            aPGRN.WarehouseName = aPGRN.Warehouse != null ? _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == Convert.ToInt32(aPGRN.Warehouse)).ConfigValue : "N/A";
            var aPGRNItem = _dbContext.APGRNItem
                                .Include(i => i.Item)
                                .Where(i => i.GRNID == id && i.IsDeleted == false)
                                .ToList();
            ViewBag.NavbarHeading = "GRN";
            ViewBag.TitleStatus = "Approved";

            TempData["Detail"] = aPGRNItem;
            return View(aPGRN);
        }


        public string GetAccountName(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string account = _dbContext.GLAccounts.Where(a => a.Id == id && a.IsDeleted == false).Select(a => a.Name).FirstOrDefault();

            return account;

        }
        public IActionResult GetGRNList()
        {
            try
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

                var searchGRNNo = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchGRNDate = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchIRNNo = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchTotal = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchTotalTaxAmount = Request.Form["columns[4][search][value]"].FirstOrDefault();
                var searchFreight = Request.Form["columns[5][search][value]"].FirstOrDefault();
                var searchGrandTotal = Request.Form["columns[6][search][value]"].FirstOrDefault();
                var searchStatus = Request.Form["columns[7][search][value]"].FirstOrDefault();
                var searchCreatedBy = Request.Form["columns[8][search][value]"].FirstOrDefault();




                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var customerData = (from tempcustomer in _dbContext.APGRN.Where(x => x.IsDeleted == false && x.Resp_ID == resp_Id && x.CompanyId == companyId) select tempcustomer);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                customerData = !string.IsNullOrEmpty(searchGRNNo) ? customerData.Where(m => m.GRNNO.ToString().Contains(searchGRNNo)) : customerData;
                customerData = !string.IsNullOrEmpty(searchGRNDate) ? customerData.Where(m => m.GRNDate != null ? m.GRNDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchGRNDate.ToUpper()) : false) : customerData;
                customerData = !string.IsNullOrEmpty(searchIRNNo) ? customerData.Where(m => m.IRNNo.ToString().Contains(searchIRNNo)) : customerData;
                customerData = !string.IsNullOrEmpty(searchTotal) ? customerData.Where(m => m.Total.ToString().Contains(searchTotal)) : customerData;
                customerData = !string.IsNullOrEmpty(searchTotalTaxAmount) ? customerData.Where(m => m.TotalTaxAmount.ToString().Contains(searchTotalTaxAmount)) : customerData;
                customerData = !string.IsNullOrEmpty(searchFreight) ? customerData.Where(m => m.Freight.ToString().Contains(searchFreight)) : customerData;
                customerData = !string.IsNullOrEmpty(searchGrandTotal) ? customerData.Where(m => m.GrandTotal.ToString().Contains(searchGrandTotal)) : customerData;
                customerData = !string.IsNullOrEmpty(searchStatus) ? customerData.Where(m => m.Status.ToString().Contains(searchStatus)) : customerData;
                customerData = !string.IsNullOrEmpty(searchCreatedBy) ? customerData.Where(m =>  _dbContext.Users.FirstOrDefault(x => x.Id == m.CreatedBy).FullName.ToUpper().Contains(searchCreatedBy.ToUpper())) : customerData;


                //if (!string.IsNullOrEmpty(searchValue))
                //{
                //    customerData = customerData.Where(m => m.GRNNO.ToString().Contains(searchValue)
                //                                    || m.GRNDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchValue.ToUpper())
                //                                    || m.IRNNo.ToString().Contains(searchValue)
                //                                    || m.Status.ToString().ToUpper().Contains(searchValue.ToUpper())
                //                                    || m.Total.ToString().Contains(searchValue)
                //                                    || m.TotalTaxAmount.ToString().Contains(searchValue)
                //                                    || m.Freight.ToString().Contains(searchValue)
                //                                    || m.GrandTotal.ToString().Contains(searchValue)
                //                                    || _dbContext.Users.FirstOrDefault(x => x.Id == m.CreatedBy).FullName.ToUpper().Contains(searchValue.ToUpper())
                //                                    );

                //}
                recordsTotal = customerData.Count();
                var data = customerData.ToList();
                if (pageSize == -1)
                {
                    data = customerData.Skip(skip).Take(recordsTotal == 0 ? 1 : pageSize == -1 ? recordsTotal : 1).ToList();
                }
                else
                {
                    data = customerData.Skip(skip).Take(pageSize).ToList();
                }
                List<APGRNViewModel> Details = new List<APGRNViewModel>();
                foreach (var grp in data)
                {
                    APGRNViewModel grn = new APGRNViewModel();
                    grn.APGRN = grp;
                    grn.APGRN.Approve = approve;
                    grn.APGRN.Unapprove = unApprove;
                    grn.CreatedBy = _dbContext.Users.FirstOrDefault(x => x.Id == grp.CreatedBy).FullName;
                    grn.Date = grp.GRNDate.ToString(Helpers.CommonHelper.DateFormat);
                    Details.Add(grn);

                }
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = Details };
                return Ok(jsonData);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IActionResult Search(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var LCId = _dbContext.APGRN.Where(x => x.GRNNO == id).Select(x => x.Id).FirstOrDefault();
            var list = _dbContext.APGRNExpense.Where(x => x.LCId == LCId).ToList();
            List<APGRNExpense> aPGRNExpenses = new List<APGRNExpense>();
            foreach (var obj in list)
            {
                var acount = _dbContext.GLAccounts.Where(a => a.Id == Convert.ToInt32(obj.GLCode)).FirstOrDefault();
                obj.GLCode = acount.Code + "-" + acount.Name;
                obj.ExpenseDate = obj.ExpenseDate.Date;
                aPGRNExpenses.Add(obj);
            }
            return Ok(list);

        }
        [HttpGet]
        public IActionResult GetBrand(int IRNDetailId)
        {
            if (IRNDetailId != 0)
            {
                var data = _dbContext.APIRNDetails.Include(x=>x.Brand).FirstOrDefault(x => x.Id == IRNDetailId);
                if (data != null)
                {
                    return Ok(data);
                }
                return Ok(null);
            }
            return Ok(null);
        }
    }
}