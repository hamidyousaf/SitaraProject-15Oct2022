﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Numbers.Entity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Numbers.Helpers;
using Numbers.Entity.ViewModels;
using Newtonsoft.Json;
using Numbers.Repository.Helpers;
using Numbers.Repository.Vouchers;
using Numbers.Repository.Setup;
using System.Linq.Dynamic.Core;

namespace Numbers.Areas.AP.Controllers
{
    [Area("AP")]
    [Authorize]
    public class PurchaseController : Controller
    {
        private readonly NumbersDbContext _dbContext;


        public PurchaseController(NumbersDbContext context)
        {
            _dbContext = context;
        }
        [HttpGet]
        public IActionResult Index()
        {


            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userid = HttpContext.Session.GetString("UserId");
            TempData["ShowRate"] = true;
            string configValues = _dbContext.AppCompanyConfigs
                                            .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                                            .Select(c => c.ConfigValue)
                                            .FirstOrDefault();
            ViewBag.ReportPath = string.Concat(configValues, "Viewer", "?Report=Voucher&cId=", companyId, "&id=");
            ViewBag.ReportPath2 = string.Concat(configValues, "Viewer", "?Report=PurchaseInvoice&cId=", companyId, "&showRate=" + TempData["ShowRate"], "&id=");
            ViewBag.NavbarHeading = "List of Purchase Invoices";
            return View(_dbContext.APPurchases.Where(c => c.CompanyId == companyId &&  c.TransactionType == "Purchase" && c.IsDeleted == false)
                                              .Include(s => s.Supplier)
                                              .ToList());
        }
        [HttpGet]
        public IActionResult Create(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            TempData["RoleName"] = (from a in _dbContext.Roles
                                    join b in _dbContext.UserRoles.Where(x => x.UserId == userId) on a.Id equals b.RoleId
                                    select a.Name).FirstOrDefault();
            TempData["ShowRate"] = true;
            var configValues = new ConfigValues(_dbContext);
            string configValue = _dbContext.AppCompanyConfigs
                                            .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                                            .Select(c => c.ConfigValue)
                                            .FirstOrDefault();
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            ViewBag.Department = new SelectList(_dbContext.GLDivision.Where(x => x.CompanyId == companyId && x.IsDeleted == false).ToList(), "Id", "Name");
          //  ViewBag.SubAcount = new SelectList( _dbContext.GLSubAccountDetails.Where(x => x.IsDelete == false && x.Description != "" && x.IsActive != false).ToList(), "Id", "Description");
            //ViewBag.SubAcount = new SelectList((from a in _dbContext.GLSubAccountDetails
            //                                   join b in _dbContext.GLSubAccounts.Where(x => x.IsDeleted == false && x.Description != "" && x.IsActive != false) on a.SubAccountId equals b.Id
            //                                    ).ToList(), "Id", "Description");

            ViewBag.SubAcount = new SelectList( _dbContext.GLSubAccountDetails.Include(p=>p.SubAccountId).Where(p=>p.SubAccountId.IsDeleted != true && !string.IsNullOrEmpty(p.Description) && p.SubAccountId.IsActive != false ), "Id", "Description");
            ViewBag.OperatingUnit = configValues.GetOrgValues(resp_Id, "Operating Unit", companyId);
            ViewBag.InventoryOrganization = configValues.GetOrgValues(resp_Id, "Inventory Organization", companyId);
            ViewBag.SubAccounts = _dbContext.GLSubAccountDetails.Include(p => p.SubAccountId).Where(p => p.SubAccountId.IsDeleted != true && !string.IsNullOrEmpty(p.Description) && p.SubAccountId.IsActive != false).ToList();
            ViewBag.Suppliers = configValues.Supplier(companyId);
            ViewBag.Salesman = configValues.GetConfigValues("AP", "SalesPerson", companyId);
            AppConfigHelper helper = new AppConfigHelper(_dbContext, HttpContext);
            ViewBag.WareHouse = configValues.GetConfigValues("Inventory", "Ware House", companyId);
            ViewBag.ReportPath = string.Concat(configValue, "Viewer", "?Report=Voucher&cId=", companyId, "&id={0}");
            ViewBag.ReportPath2 = string.Concat(configValue, "Viewer", "?Report=PurchaseInvoice&cId=", companyId, "&showRate=" + TempData["ShowRate"], "&id={0}");
            APPurchaseItemViewModel viewModel = new APPurchaseItemViewModel();
            viewModel.Purchase = new APPurchase();
            viewModel.APPurchaseItems = new List<APPurchaseItem>();
            ViewBag.Responsibility = _dbContext.Sys_Responsibilities.FirstOrDefault(x => x.Resp_Id == resp_Id).Resp_Name.ToString();
            if (id == 0)
            {
                ViewBag.Counter = 0;
                ViewBag.EntityState = "Create";
                TempData["Mode"] = false;
                ViewBag.NavbarHeading = "Create Purchase Invoice";
                int maxPurchaseNo = 1;
                var purchases = _dbContext.APPurchases.Where(c => c.CompanyId == companyId && c.TransactionType == "Purchase").ToList();
                if (purchases.Count > 0)
                {
                    maxPurchaseNo = purchases.Max(v => v.PurchaseNo);
                    TempData["PurchaseNo"] = maxPurchaseNo + 1;
                    ViewBag.PurchaseNumber = maxPurchaseNo + 1;
                }
                else
                {
                    TempData["PurchaseNo"] = maxPurchaseNo;
                }
                var model = new APPurchaseItemViewModel();
                var appTaxRepo = new AppTaxRepo(_dbContext);
                model.TaxList = appTaxRepo.GetTaxes(companyId);
                model.Currencies = AppCurrencyRepo.GetCurrencies();
                return View(model);
            }
            else
            {
                TempData["Mode"] = true;
                var purchase = _dbContext.APPurchases.Find(id);
                viewModel = new APPurchaseItemViewModel();
                viewModel.Id = purchase.Id;
                viewModel.PurchaseNo = purchase.PurchaseNo;
                viewModel.PurchaseDate = purchase.PurchaseDate;
                viewModel.SupplierInvoiceDate = purchase.SupplierInvoiceDate;
                viewModel.WareHouseId = purchase.WareHouseId;
                viewModel.SupplierId = purchase.SupplierId;
                viewModel.ReferenceNo = purchase.ReferenceNo;
                viewModel.SupplierInvoiceNo = purchase.SupplierInvoiceNo;
               
                viewModel.IGPNo = purchase.IGPNo;
                viewModel.CurrencyExchangeRate = purchase.CurrencyExchangeRate;
                viewModel.Remarks = purchase.Remarks;
                viewModel.Currency = purchase.Currency;
                viewModel.DepartmentId = purchase.DepartmentId;
                viewModel.OperationId = purchase.OperationId;
                viewModel.GrandTotal = purchase.GrandTotal;
                viewModel.TotalDiscountAmount = purchase.TotalDiscountAmount;
                viewModel.TotalExciseTaxAmount = purchase.TotalExciseTaxAmount;
                viewModel.Total = purchase.Total;
                viewModel.TotalSalesTaxAmount = purchase.TotalSalesTaxAmount;
                viewModel.VoucherId = purchase.VoucherId;
                viewModel.Status = purchase.Status;

                TempData["PurchaseNo"] = purchase.PurchaseNo;
                ViewBag.Id = id;
                if (viewModel.Status != "Approved")
                {
                    ViewBag.EntityState = "Create";
                    ViewBag.NavbarHeading = "Purchase";
                    ViewBag.TitleStatus = "Created";
                }

                // APPurchaseItemViewModel aPPurchaseVM = new APPurchaseItemViewModel();
                viewModel.Purchase = _dbContext.APPurchases.Find(id);
                APPurchaseItem[] poItems = _dbContext.APPurchaseItems.Where(u => u.PurchaseId == id && u.IsDeleted==false).Include(x=>x.Item).ToArray();
                ViewBag.Items = poItems;
                viewModel.APPurchaseItems = poItems.ToList();
               // viewModel.APPurchaseItems = _dbContext.APPurchaseItems.Where(u => u.PurchaseId == id).ToList();
              //  viewModel.APPurchaseItems = _dbContext.APPurchaseItems.Where(u => u.PurchaseId == id).ToList();
                if (viewModel.APPurchaseItems != null)
                {
                    foreach (var item in viewModel.APPurchaseItems)
                    {
                        if (item.ItemId != 0)
                        {
                            item.CreatedBy = (from a in _dbContext.InvItems.Where(x => x.Id == item.ItemId) select a.Name).FirstOrDefault();
                        }
                    }
                }
                //APPurchaseItem[] items = _dbContext.APPurchaseItems.Where(i => i.PurchaseId == id && i.IsDeleted == false).ToArray();
                //ViewBag.Items = items;
                viewModel.Currencies = AppCurrencyRepo.GetCurrencies();
                return View(viewModel);
            }
        }
        //[HttpPost]
        //public async Task<IActionResult> Create(APPurchaseItemViewModel model, IFormCollection collection)
        //{
        //    int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
        //    string userId = HttpContext.Session.GetString("UserId");
        //    if (model.Id == 0)
        //    {
        //        if (collection["Details"].Count > 0)
        //        {
        //            APPurchase purchase = new APPurchase();
        //            //Master Table data
        //            purchase.PurchaseNo = model.PurchaseNo;
        //            purchase.PurchaseDate = model.PurchaseDate;
        //            purchase.SupplierInvoiceDate = model.SupplierInvoiceDate;
        //            purchase.WareHouseId = model.WareHouseId;
        //            purchase.SupplierId = model.SupplierId;
        //            purchase.ReferenceNo = model.ReferenceNo;
        //            purchase.SupplierInvoiceNo = model.SupplierInvoiceNo;
        //            purchase.IGPNo = model.IGPNo;
        //            purchase.Remarks = collection["Remarks"][0];
        //            purchase.Currency = model.Currency;
        //            purchase.CurrencyExchangeRate = model.CurrencyExchangeRate;
        //            purchase.Total = Convert.ToDecimal(collection["Total"]);
        //            purchase.TotalDiscountAmount = Convert.ToDecimal(collection["totalDiscountAmount"]);
        //            purchase.TotalSalesTaxAmount = Convert.ToDecimal(collection["totalSalesTaxAmount"]);
        //            purchase.TotalExciseTaxAmount = Convert.ToDecimal(collection["totalExciseTaxAmount"]);
        //            purchase.GrandTotal = Convert.ToDecimal(collection["GrandTotal"]);
        //            purchase.CreatedBy = userId;
        //            purchase.CreatedDate = DateTime.Now;
        //            purchase.CompanyId = companyId;
        //            purchase.Status = "Created";
        //            purchase.TransactionType = "Purchase";
        //            _dbContext.APPurchases.Add(purchase);
        //            await _dbContext.SaveChangesAsync();
        //            APPurchaseItem[] APInvcoice = JsonConvert.DeserializeObject<APPurchaseItem[]>(collection["Details"]);
        //            foreach (var items in APInvcoice)
        //            {
        //                if (items.Id != 0)
        //                {
        //                    APPurchaseItem[] data = _dbContext.APPurchaseItems.Where(i =>  i.IsDeleted == false && i.Id == items.Id).ToArray();
        //                    foreach (var i in data)
        //                    {

        //                        APPurchaseItem obj = new APPurchaseItem();
        //                        obj = i;
        //                        obj.PurchaseId = purchase.Id;
        //                        obj.Boxes = items.Boxes;
        //                        obj.ItemId = items.ItemId;
        //                        obj.LineTotal = items.LineTotal;
        //                        obj.Qty = items.Qty;
        //                        obj.Rate = items.Rate;
        //                        obj.SQM = items.SQM;

        //                        obj.Tiles = items.Tiles;
        //                        obj.Total = items.Total;

        //                        obj.DiscountPercentage = items.DiscountPercentage;


        //                        obj.UpdatedBy = userId;
        //                        obj.UpdatedDate = DateTime.Now;
        //                        _dbContext.APPurchaseItems.Update(obj);
        //                        _dbContext.SaveChanges();
        //                    }
        //                }
        //                else
        //                {
        //                    APPurchaseItem data = new APPurchaseItem();
        //                    data = items;
        //                    data.PurchaseId = purchase.Id;

        //                    data.UpdatedBy = userId;
        //                    data.UpdatedDate = DateTime.Now;
        //                    _dbContext.APPurchaseItems.Update(data);
        //                    //val.OriginalValues.SetValues(await val.GetDatabaseValuesAsync());
        //                    _dbContext.SaveChanges();
        //                }
        //            }

        //            await _dbContext.SaveChangesAsync();
        //            TempData["error"] = "false";
        //            TempData["message"] = "Purchase Invoice has been created successfully";
        //            return RedirectToAction(nameof(Index));
        //        }
        //        else
        //        {
        //            TempData["error"] = "true";
        //            TempData["message"] = "No any Purchase Invoice has been created. It must contain atlest one item";
        //            return RedirectToAction(nameof(Index));
        //        }
        //    }
        //    else
        //    {
        //        //for partial-items removal
        //        string[] idsDeleted = Convert.ToString(collection["IdsDeleted"]).Split(",");
        //        if (!idsDeleted.Contains(""))
        //        {
        //            for (int j = 0; j < idsDeleted.Length; j++)
        //            {
        //                if (idsDeleted[j] != "0")
        //                {
        //                    var itemToRemove = _dbContext.APPurchaseItems.Find(Convert.ToInt32(idsDeleted[j]));
        //                    itemToRemove.IsDeleted = true;
        //                    _dbContext.APPurchaseItems.Update(itemToRemove);
        //                    await _dbContext.SaveChangesAsync();
        //                }
        //            }
        //        }
        //        //updating existing data
        //        if (collection["ItemId"].Count > 0)
        //        {
        //            APPurchase purchase = _dbContext.APPurchases.Find(model.Id);
        //            purchase.PurchaseNo = model.PurchaseNo;
        //            purchase.PurchaseDate = model.PurchaseDate;
        //            purchase.SupplierInvoiceDate = model.SupplierInvoiceDate;
        //            purchase.WareHouse = model.WareHouse;
        //            purchase.SupplierId = model.SupplierId;
        //            purchase.ReferenceNo = model.ReferenceNo;
        //            purchase.IGPNo = model.IGPNo;
        //            purchase.SupplierInvoiceNo = model.SupplierInvoiceNo;
        //            purchase.Remarks = collection["Remarks"][0];
        //            purchase.Currency = model.Currency;
        //            purchase.CurrencyExchangeRate = model.CurrencyExchangeRate;
        //            purchase.Total = Convert.ToDecimal(collection["Total"]);
        //            purchase.TotalDiscountAmount = Convert.ToDecimal(collection["totalDiscountAmount"]);
        //            purchase.TotalSalesTaxAmount = Convert.ToDecimal(collection["totalSalesTaxAmount"]);
        //            purchase.TotalExciseTaxAmount = Convert.ToDecimal(collection["totalExciseTaxAmount"]);
        //            purchase.GrandTotal = Convert.ToDecimal(collection["GrandTotal"]);
        //            purchase.Status = "Created";
        //            purchase.UpdatedBy = userId;
        //            purchase.UpdatedDate = DateTime.Now;
        //            var entry = _dbContext.APPurchases.Update(purchase);
        //            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
        //            await _dbContext.SaveChangesAsync();
        //            //Update purchase detail items/ purchase items if any                   
        //                for (int i = 0; i < collection["ItemId"].Count; i++)
        //                {
        //                    var purchaseItem = _dbContext.APPurchaseItems
        //                        .Where(j => j.PurchaseId == model.Id && j.Id == Convert.ToInt32(collection["PurchaseItemId"][i] == "" ? 0 : Convert.ToInt32(collection["PurchaseItemId"][i]))).FirstOrDefault();
        //                    // Extract coresponding values from form collection
        //                    var itemId = Convert.ToInt32(collection["ItemId"][i]);
        //                    var qty = Convert.ToDecimal(collection["Qty"][i]);
        //                    var rate = Convert.ToDecimal(collection["Rate"][i]);
        //                    var total = Convert.ToDecimal(collection["Total_"][i]);
        //                    var taxId = Convert.ToInt32(collection["TaxId"][i]);
        //                    var discountPercentage = Convert.ToDecimal(collection["DiscountPercentage"][i]);
        //                    var discountAmount = Convert.ToDecimal(collection["DiscountAmount"][i]);
        //                    var salesTaxPercentage = Convert.ToDecimal(collection["SalesTaxPercentage"][i]);
        //                    var salesTaxAmount = Convert.ToDecimal(collection["SalesTaxAmount"][i]);
        //                    var exciseTaxPercentage = Convert.ToDecimal(collection["ExciseTaxPercentage"][i]);
        //                    var exciseTaxAmount = Convert.ToDecimal(collection["ExciseTaxAmount"][i]);
        //                    var lineTotal = Convert.ToDecimal(collection["LineTotal"][i]);
        //                    var remarks = collection["Remarks"][i + 1];
        //                    if (purchaseItem != null && itemId != 0)
        //                    {
        //                        //below phenomenon prevents Id from being marked as modified
        //                        var entityEntry = _dbContext.Entry(purchaseItem);
        //                        entityEntry.State = EntityState.Modified;
        //                        entityEntry.Property(p => p.Id).IsModified = false;
        //                        purchaseItem.ItemId = itemId;
        //                        purchaseItem.PurchaseId = model.Id;
        //                        purchaseItem.Qty = qty;
        //                        purchaseItem.DiscountAmount = 0;
        //                        purchaseItem.Rate = rate;
        //                        purchaseItem.Total = total;
        //                        purchaseItem.TaxId = taxId;
        //                        purchaseItem.DiscountPercentage = discountPercentage;
        //                        purchaseItem.DiscountAmount = discountAmount;
        //                        purchaseItem.SalesTaxPercentage = salesTaxPercentage;
        //                        purchaseItem.SalesTaxAmount = salesTaxAmount;
        //                        purchaseItem.ExciseTaxPercentage = exciseTaxPercentage;
        //                        purchaseItem.ExciseTaxAmount = exciseTaxAmount;
        //                        purchaseItem.LineTotal = lineTotal;
        //                        purchaseItem.Remarks = remarks;

        //                        var dbEntry = _dbContext.APPurchaseItems.Update(purchaseItem);
        //                        dbEntry.OriginalValues.SetValues(await entityEntry.GetDatabaseValuesAsync());

        //                    }
        //                    //check if user created new item while updating
        //                    else if (itemId != 0 && purchaseItem == null)//itemId is invitem, if this is null or zero rest of the information for this item will not be saved.
        //                    {
        //                        var newItem = new APPurchaseItem();
        //                        newItem.PurchaseId = model.Id;
        //                        newItem.PurchaseOrderItemId = model.PurchaseOrderItemId;
        //                        newItem.ItemId = itemId;
        //                        newItem.Qty = qty;
        //                        newItem.Rate = rate;
        //                        newItem.Total = total;
        //                        newItem.TaxId = taxId;
        //                        newItem.DiscountPercentage = discountPercentage;
        //                        newItem.DiscountAmount = discountAmount;
        //                        newItem.SalesTaxPercentage = salesTaxPercentage;
        //                        newItem.SalesTaxAmount = salesTaxAmount;
        //                        newItem.ExciseTaxPercentage = exciseTaxPercentage;
        //                        newItem.ExciseTaxAmount = exciseTaxAmount;
        //                        newItem.LineTotal = lineTotal;
        //                        newItem.Remarks = remarks;
        //                        _dbContext.APPurchaseItems.Add(newItem);
        //                    }
        //                    TempData["error"] = "false";
        //                    TempData["message"] = "Purchase Invoice has been updated successfully";
        //                    await _dbContext.SaveChangesAsync();
        //                }                        
        //                await _dbContext.SaveChangesAsync();
        //                return RedirectToAction(nameof(Index));
        //        }
        //        else
        //        {
        //            TempData["error"] = "true";
        //            TempData["message"] = "No any Purchase Invoice has been updated. It must contain atlest one item";
        //            return RedirectToAction(nameof(Index));
        //        }
        //    }
        //}
        //[HttpPost]
        //public IActionResult Create(APPurchaseItemViewModel vm, IFormCollection collection)
        //{
        //    int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
        //    var userId = HttpContext.Session.GetString("UserId");
        //    var result = "";
        //    try
        //    {
        //        if (vm.Id != 0)
        //        {
        //            APPurchase apPurFrm = _dbContext.APPurchases.Find(vm.Id);
        //            apPurFrm.Id = vm.Id;
        //            apPurFrm.PurchaseNo = vm.PurchaseNo;
        //            apPurFrm.PurchaseDate = vm.PurchaseDate;
        //            apPurFrm.ApprovedBy = userId;
        //            apPurFrm.Currency = vm.Currency;
        //            apPurFrm.CurrencyExchangeRate = vm.CurrencyExchangeRate;
        //            apPurFrm.GrandTotal = vm.GrandTotal;
        //            apPurFrm.IGPNo = vm.IGPNo;
        //            apPurFrm.PaymentTotal = vm.PaymentTotal;
        //            apPurFrm.PeriodId = vm.PeriodId;
        //            apPurFrm.Status = "Created";
        //            apPurFrm.Total = vm.Total;
        //            apPurFrm.TotalDiscountAmount = vm.TotalDiscountAmount;
        //            apPurFrm.TotalExciseTaxAmount = vm.TotalExciseTaxAmount;
        //            apPurFrm.TotalSalesTaxAmount = vm.TotalSalesTaxAmount;
        //            apPurFrm.TransactionType = "Purchase";
        //            apPurFrm.WareHouseId = vm.WareHouseId;
        //            apPurFrm.VoucherId = vm.VoucherId;

        //            apPurFrm.SupplierId = vm.SupplierId;

        //            apPurFrm.Remarks = vm.Remarks;

        //            apPurFrm.CompanyId = companyId;

        //            apPurFrm.IsDeleted = false;
        //            apPurFrm.UpdatedBy = userId;
        //            apPurFrm.UpdatedDate = DateTime.Now;

        //            _dbContext.APPurchases.Update(apPurFrm);
        //            _dbContext.SaveChanges();

        //            APPurchaseItem[] medicalsdata = JsonConvert.DeserializeObject<APPurchaseItem[]>(collection["details"]);
        //            foreach (var items in medicalsdata)
        //            {
        //                if (items.Id != 0)
        //                {
        //                    APPurchaseItem data = _dbContext.APPurchaseItems.Where(i => i.PurchaseId == vm.Id && i.IsDeleted == false && i.Id == items.Id).FirstOrDefault();
        //                    //foreach (var i in data)
        //                    //{

        //                    APPurchaseItem obj = new APPurchaseItem();
        //                    obj = data;
        //                    obj.PurchaseId = apPurFrm.Id;
        //                    obj.Boxes = items.Boxes;
        //                    obj.ItemId = items.ItemId;
        //                    obj.Total = items.Total;
        //                    obj.Qty = items.Qty;
        //                    obj.Rate = items.Rate;
        //                    obj.SQM = items.SQM;

        //                    obj.Tiles = items.Tiles;
        //                    obj.Total = items.Total;

        //                    obj.DiscountAmount = items.DiscountAmount;
        //                    obj.DiscountPercentage = items.DiscountPercentage;
        //                    obj.UpdatedBy = userId;
        //                    obj.UpdatedDate = DateTime.Now;
        //                    _dbContext.APPurchaseItems.Update(obj);
        //                    _dbContext.SaveChanges();
        //                    //}
        //                }
        //                else
        //                {
        //                    APPurchaseItem data = new APPurchaseItem();
        //                    data = items;
        //                    data.PurchaseId = apPurFrm.Id;


        //                    _dbContext.APPurchaseItems.Update(data);
        //                    //val.OriginalValues.SetValues(await val.GetDatabaseValuesAsync());
        //                    _dbContext.SaveChanges();
        //                }




        //            }



        //            //  return View(RedirectToAction(nameof(Index)));
        //            return RedirectToAction("Index", "Purchase");
        //        }
        //        else
        //        {

        //            APPurchase apPurFrom = new APPurchase();

        //            apPurFrom.Id = vm.Id;
        //            apPurFrom.PurchaseNo = vm.PurchaseNo;
        //            apPurFrom.PurchaseDate = vm.PurchaseDate;
        //            apPurFrom.ApprovedBy = userId;
        //            apPurFrom.Currency = vm.Currency;
        //            apPurFrom.CurrencyExchangeRate = vm.CurrencyExchangeRate;
        //            apPurFrom.GrandTotal = vm.GrandTotal;
        //            apPurFrom.IGPNo = vm.IGPNo;
        //            apPurFrom.PaymentTotal = vm.PaymentTotal;
        //            apPurFrom.PeriodId = vm.PeriodId;
        //            apPurFrom.Status = "Created";
        //            apPurFrom.TotalDiscountAmount = vm.TotalDiscountAmount;
        //            apPurFrom.TotalExciseTaxAmount = vm.TotalExciseTaxAmount;
        //            apPurFrom.TotalSalesTaxAmount = vm.TotalSalesTaxAmount;
        //            apPurFrom.TransactionType = "Purchase";
        //            apPurFrom.WareHouseId = vm.WareHouseId;
        //            apPurFrom.VoucherId = vm.VoucherId;

        //            apPurFrom.SupplierId = vm.SupplierId;

        //            apPurFrom.Remarks = vm.Remarks;

        //            apPurFrom.CompanyId = companyId;

        //            apPurFrom.IsDeleted = false;
        //            apPurFrom.CreatedBy = userId;
        //            apPurFrom.CreatedDate = DateTime.Now;


        //            _dbContext.APPurchases.Add(apPurFrom);
        //            _dbContext.SaveChanges();

        //            APPurchaseItem[] purchaseDetail = JsonConvert.DeserializeObject<APPurchaseItem[]>(collection["details"]);

        //            foreach (APPurchaseItem item in purchaseDetail)
        //            {
        //                //HRDeductionEmployee experience = new HRDeductionEmployee();
        //                //experience.DeductionId = model.Id;
        //                //experience = employeeExperince;
        //                //experience.EmployeeId = Convert.ToInt32(collection["employeeId"]);
        //                //experience.CreatedBy = _userId;
        //                //experience.CreatedDate = DateTime.Now;
        //                //_dbContext.HRDeductionEmployees.Add(experience);
        //                //await _dbContext.SaveChangesAsync();

        //                APPurchaseItem detail = new APPurchaseItem();
        //                detail = item;
        //                detail.PurchaseId = apPurFrom.Id;



        //                _dbContext.APPurchaseItems.Add(detail);
        //                _dbContext.SaveChanges();
        //            }


        //            // return View(nameof(Index));
        //            return RedirectToAction("Index", "Purchase");
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        result = "Error";
        //    }

        //    return Ok(result);
        //}
        public int GetMaxNo()
        {
            int maxInvoiceNo = 1;
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var invoices = _dbContext.APPurchases.Where(c => c.CompanyId == companyId && c.TransactionType == "Purchase").ToList();
            if (invoices.Count > 0)
            {
                maxInvoiceNo = invoices.Max(v => v.PurchaseNo);
                maxInvoiceNo = maxInvoiceNo + 1;
            }
            return maxInvoiceNo;
        }
        [HttpPost]
        public async Task<IActionResult> Create(APPurchaseItemViewModel vm, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            decimal grnQty =Convert.ToDecimal( 0.000);
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            var userId = HttpContext.Session.GetString("UserId");
            var result = "";
            bool Ratezero = false;
            string[] idsDeleted = Convert.ToString(collection["IdsDeleted"]).Split(",");
            if (!idsDeleted.Contains(""))
            {
                for (int j = 0; j < idsDeleted.Length; j++)
                {
                    if (idsDeleted[j] != "0")
                    {
                        var itemToRemove = _dbContext.APPurchaseItems.Find(Convert.ToInt32(idsDeleted[j]));
                        itemToRemove.IsDeleted = true;
                        var tracker = _dbContext.APPurchaseItems.Update(itemToRemove);
                        tracker.OriginalValues.SetValues(await tracker.GetDatabaseValuesAsync());
                        await _dbContext.SaveChangesAsync();
                    }
                }
            }
            try
            {
                if (vm.Id != 0)
                {
                    APPurchase apPurFrm = _dbContext.APPurchases.Find(vm.Id);
                    apPurFrm.Id = vm.Id;
                    apPurFrm.PurchaseNo = vm.PurchaseNo;
                    apPurFrm.PurchaseDate = vm.PurchaseDate;
                    apPurFrm.ApprovedBy = userId;
                    apPurFrm.Currency = vm.Currency;
                    apPurFrm.CurrencyExchangeRate = vm.CurrencyExchangeRate;
                    apPurFrm.GrandTotal = vm.GrandTotal;
                    apPurFrm.IGPNo = vm.IGPNo;
                    apPurFrm.SupplierInvoiceNo = vm.SupplierInvoiceNo;
                    apPurFrm.SupplierInvoiceDate = vm.SupplierInvoiceDate;
                    apPurFrm.PaymentTotal = vm.PaymentTotal;
                    apPurFrm.PeriodId = vm.PeriodId;
                    apPurFrm.Status = "Created";
                    apPurFrm.Total = vm.Total;
                    apPurFrm.TotalDiscountAmount = vm.TotalDiscountAmount;
                    apPurFrm.TotalExciseTaxAmount = vm.TotalExciseTaxAmount;
                    apPurFrm.TotalSalesTaxAmount = vm.TotalSalesTaxAmount;
                    apPurFrm.TransactionType = "Purchase";
                    apPurFrm.WareHouseId = vm.WareHouseId;
                    apPurFrm.DepartmentId = vm.DepartmentId;
                    apPurFrm.OperationId = vm.OperationId;
                    apPurFrm.Resp_ID = HttpContext.Session.GetInt32("Resp_ID").Value;
                    apPurFrm.VoucherId = vm.VoucherId;
                    apPurFrm.SupplierId = vm.SupplierId;
                    apPurFrm.Remarks = vm.Remarks;
                    apPurFrm.CompanyId = companyId;
                    apPurFrm.IsDeleted = false;
                    apPurFrm.UpdatedBy = userId;
                    apPurFrm.UpdatedDate = DateTime.Now;
                    _dbContext.APPurchases.Update(apPurFrm);
                    _dbContext.SaveChanges();


                    var list = _dbContext.APPurchaseItems.Where(l => l.PurchaseId == apPurFrm.Id).ToList();
                    if (list != null)
                    {
                        for (int i = 0; i < collection["ItemCode"].Count; i++)
                        {
                            var orderItem = _dbContext.APPurchaseItems
                                .Where(j => j.PurchaseId == apPurFrm.Id && j.Id == Convert.ToInt32(collection["PRItemId"][i] == "" ? 0 : Convert.ToInt32(collection["PRItemId"][i]))).FirstOrDefault();
                            // Extract coresponding values from form collection
                            var itemId = Convert.ToString(collection["ItemCode"][i]);

                            if (orderItem != null && itemId != "0")
                            {
                                grnQty = orderItem.Qty;
                                //var entityEntry = _dbContext.Entry(orderItem);
                                orderItem.PurchaseId = apPurFrm.Id;
                                orderItem.GRNItemId = Convert.ToInt32(collection["GRNItemId"][i]);
                                orderItem.ItemId = Convert.ToInt32(collection["ItemId"][i]);
                                //orderItem.ItemCode = Convert.ToString(collection["ItemCode"][i]);
                                //orderItem.ItemDescription = Convert.ToString(collection["ItemDescription"][i]);
                                orderItem.Qty = Convert.ToDecimal(collection["IndentQty"][i]);
                                orderItem.Rate = Convert.ToDecimal(collection["Rate"][i]);
                                orderItem.Total = Convert.ToDecimal(collection["Value"][i]);
                                orderItem.PrDetailId = Convert.ToInt32(collection["PrDetailId"][i]);
                                orderItem.SalesTaxAmount = Convert.ToDecimal(collection["SaleTaxValue"][i]);
                                orderItem.LineTotal = Convert.ToDecimal(collection["LineTotal"][i]);

                                if (collection["SubAccount"][i] != "Select...")
                                {
                                    orderItem.SubAccountId = Convert.ToInt32(collection["SubAccount"][i]);
                                }
                                else
                                {
                                    orderItem.SubAccountId = 0;
                                }
                           
                                orderItem.TaxId= Convert.ToInt32(collection["SaleTax"][i]);
                                _dbContext.APPurchaseItems.Update(orderItem);

                                var IRNDetail = _dbContext.APGRNItem.Find(orderItem.GRNItemId);
                                if (IRNDetail != null)
                                {
                                    IRNDetail.PurchaseQty = Convert.ToInt32(IRNDetail.PurchaseQty - grnQty + Convert.ToInt32(orderItem.Qty));
                                    _dbContext.APGRNItem.Update(IRNDetail);
                                }

                               
                                await _dbContext.SaveChangesAsync();
                                //dbEntry.OriginalValues.SetValues( entityEntry.GetDatabaseValuesAsync());
                            }
                            else if (orderItem == null && itemId != "0")
                            {
                                APPurchaseItem newItem = new APPurchaseItem();
                                newItem.PurchaseId = apPurFrm.Id;
                                newItem.GRNItemId = Convert.ToInt32(collection["GRNItemId"][i]);
                                newItem.ItemId = Convert.ToInt32(collection["ItemId"][i]);
                                //newItem.ItemCode = Convert.ToString(collection["ItemCode"][i]);
                                //newItem.ItemDescription = Convert.ToString(collection["ItemDescription"][i]);
                                newItem.Qty = Convert.ToDecimal(collection["IndentQty"][i]);
                                newItem.Rate = Convert.ToDecimal(collection["Rate"][i]);
                                newItem.Total = Convert.ToDecimal(collection["Value"][i]);
                                newItem.PrDetailId = Convert.ToInt32(collection["PRDetailId"][i]);
                                newItem.SalesTaxAmount = Convert.ToDecimal(collection["SaleTaxValue"][i]);
                                newItem.LineTotal = Convert.ToDecimal(collection["LineTotal"][i]);
                                if (collection["SubAccount"][i] != "Select...")
                                {
                                    orderItem.SubAccountId = Convert.ToInt32(collection["SubAccount"][i]);
                                }
                                else
                                {
                                    orderItem.SubAccountId = 0;
                                }

                                newItem.TaxId = Convert.ToInt32(collection["SaleTax"][i]);
                                _dbContext.APPurchaseItems.Add(newItem);

                                var IRNDetail = _dbContext.APGRNItem.Find(orderItem.GRNItemId);
                                if (IRNDetail != null)
                                {
                                    IRNDetail.PurchaseQty = Convert.ToInt32(IRNDetail.PurchaseQty + Convert.ToInt32(orderItem.Qty));
                                    _dbContext.APGRNItem.Update(IRNDetail);
                                }

                                await _dbContext.SaveChangesAsync();
                            }   
                        }
                    }
                    // return View(RedirectToAction(nameof(Index)));
                    TempData["error"] = "false";
                    TempData["message"] = "Purchase Updated Sucessfully.";
                    return RedirectToAction("Create");
                }
                else
                {

                    string module = (from c in _dbContext.Sys_Responsibilities where c.Resp_Id == resp_Id select c.Resp_Name).FirstOrDefault();
                    APPurchase apPurFrom = new APPurchase();
                    apPurFrom.Id = vm.Id;
                    apPurFrom.PurchaseNo = GetMaxNo();
                    apPurFrom.PurchaseDate = vm.PurchaseDate;
                    apPurFrom.ApprovedBy = userId;
                    apPurFrom.Currency = vm.Currency;
                    apPurFrom.CurrencyExchangeRate = vm.CurrencyExchangeRate;
                    apPurFrom.GrandTotal = vm.GrandTotal;
                    apPurFrom.IGPNo = vm.IGPNo;
                    apPurFrom.SupplierInvoiceNo = vm.SupplierInvoiceNo;
                    apPurFrom.SupplierInvoiceDate = vm.SupplierInvoiceDate;
                    apPurFrom.PaymentTotal = vm.PaymentTotal;
                    apPurFrom.PeriodId = vm.PeriodId;
                    apPurFrom.Status = "Created";
                    apPurFrom.Total = vm.Total;
                    apPurFrom.TotalDiscountAmount = vm.TotalDiscountAmount;
                    apPurFrom.TotalExciseTaxAmount = vm.TotalExciseTaxAmount;
                    apPurFrom.TotalSalesTaxAmount = vm.TotalSalesTaxAmount;
                    apPurFrom.TransactionType = "Purchase";
                    apPurFrom.WareHouseId = vm.WareHouseId;
                    apPurFrom.DepartmentId = vm.DepartmentId;
                    apPurFrom.OperationId = vm.OperationId;
                    apPurFrom.Resp_ID = HttpContext.Session.GetInt32("Resp_ID").Value;
                    apPurFrom.VoucherId = vm.VoucherId;
                    apPurFrom.SupplierId = vm.SupplierId;
                    apPurFrom.Remarks = vm.Remarks;
                    apPurFrom.CompanyId = companyId;
                    apPurFrom.IsDeleted = false;
                    apPurFrom.CreatedBy = userId;
                    apPurFrom.CreatedDate = DateTime.Now;


                    _dbContext.APPurchases.Add(apPurFrom);
                   await _dbContext.SaveChangesAsync();


                    //partialView's data saving in dbContext
                    for (int i = 0; i < collection["ItemId"].Count; i++)
                    {
                        var orderItem = new APPurchaseItem();
                        //var date = Convert.ToString(collection["DeliveryDate"][i]);
                        orderItem.PurchaseId = apPurFrom.Id;
                        orderItem.GRNItemId = Convert.ToInt32(collection["GRNItemId"][i]);
                        orderItem.ItemId = Convert.ToInt32(collection["ItemId"][i]);
                        //orderItem.ItemCode = Convert.ToString(collection["ItemCode"][i]);
                        //orderItem.ItemDescription = Convert.ToString(collection["ItemDescription"][i]);
                        orderItem.Qty = Convert.ToDecimal(collection["IndentQty"][i]);
                        orderItem.Rate = Convert.ToDecimal(collection["Rate"][i]);
                        orderItem.Total = Convert.ToDecimal(collection["Value"][i]);
                        orderItem.BalanceQty = Convert.ToDecimal(collection["BalanceQty"][i]);
                        orderItem.LineTotal = Convert.ToDecimal(collection["LineTotal"][i]);
                        orderItem.SalesTaxAmount = Convert.ToDecimal(collection["SaleTaxValue"][i]);
                        orderItem.PrDetailId = Convert.ToInt32(collection["PRDetailId"][i]);
                       
                        orderItem.BrandId = module == "Yarn Purchase" ? Convert.ToInt32(collection["BrandId"][i]) : 0;
                        if (collection["SubAccount"][i] != "Select...")
                        {
                            orderItem.SubAccountId = Convert.ToInt32(collection["SubAccount"][i]);
                        }
                        else
                        {
                            orderItem.SubAccountId = 0;
                        }
                        //try
                        //{
                        //    orderItem.SubAccountId = Convert.ToInt32(collection["SubAccount"][i]);
                        //}
                        //catch(Exception ex)
                        //{
                        //    var error = ex;
                        //}
                        orderItem.TaxId= Convert.ToInt32(collection["SaleTax"][i]);
                        //  orderItem.FCValue = Convert.ToDecimal(collection["FCValue"][i]);
                        // orderItem.DetailCostCenter = Convert.ToInt32(collection["DetailCostCenter"][i]);

                        _dbContext.APPurchaseItems.Add(orderItem);

                        var IRNDetail = _dbContext.APGRNItem.Find(orderItem.GRNItemId);
                        if (IRNDetail != null)
                        {
                            IRNDetail.PurchaseQty = Convert.ToInt32(IRNDetail.PurchaseQty + Convert.ToInt32(orderItem.Qty));
                            _dbContext.APGRNItem.Update(IRNDetail);
                        }
                        await _dbContext.SaveChangesAsync();
                    }
                    // return View(nameof(Index));
                    TempData["error"] = "false";
                    TempData["message"] = "Purchase "+ apPurFrom.PurchaseNo + " Created Sucessfully.";
                    return RedirectToAction("Create");
                }

              
            }
            catch (Exception e)
            {
                RedirectToAction("Create");
            }

            return RedirectToAction("Create");
        }
        public IActionResult GetItemDetails(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var items = from item in _dbContext.InvItems
                        join config in _dbContext.AppCompanyConfigs on item.Id equals id
                        where item.Id == id && config.ConfigName == "UOM" && config.Module == "Inventory" && config.Id == item.Unit
                        select new
                        {
                            uom = config.ConfigValue,
                            id = config.Id,
                            rate = item.PurchaseRate,
                            stock = item.StockAccountId
                        };
            return Ok(items);
        }
        public IActionResult PartialPurchaseItems(int? counter)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            ViewBag.Counter = counter;
            var model = new APPurchaseItemViewModel();
            var appTaxRepo = new AppTaxRepo(_dbContext);
            model.TaxList = appTaxRepo.GetTaxes(companyId);
            return PartialView("_partialPurchaseItems", model);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var purchase = _dbContext.APPurchases.Find(id);
            purchase.IsDeleted = true;
            var entry = _dbContext.APPurchases.Update(purchase);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();

            var GrnItem = _dbContext.APPurchaseItems.Where(x => x.IsDeleted == false && x.PurchaseId == id).ToList();
            foreach (var item in GrnItem)
            {
                if (item.GRNItemId != 0 && item.GRNItemId != null)
                {
                    var grnItem = _dbContext.APGRNItem.Find(item.GRNItemId);

                    grnItem.PurchaseQty = grnItem.PurchaseQty - Convert.ToInt32(item.Qty);
                    _dbContext.APGRNItem.Update(grnItem);
                    _dbContext.SaveChanges();
                }

            }
            TempData["error"] = "false";
            TempData["message"] = string.Format("Purchase No {0} has been deleted", purchase.PurchaseNo);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Approve(int id, string type)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var purchaseVoucherRepo = new PurchaseVoucherRepo(_dbContext, HttpContext);
            try
            {
                var result = await purchaseVoucherRepo.ApprovePurchaseInvoice(id, companyId, userId);
            
                if (result == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = "Purchase Invoice has been approved successfully";
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Purchase Invoice was not approved";
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = "true";
                TempData["message"] = ex.Message.ToString();
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult GetPurchaseItems(int id, int itemId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var appTaxRepo = new AppTaxRepo(_dbContext);
            var item = _dbContext.APPurchaseItems.Where(i => i.Id == id && i.IsDeleted != true).FirstOrDefault();
            ViewBag.Counter = id;
            ViewBag.ItemId = item.ItemId;
            APPurchaseItemViewModel viewModel = new APPurchaseItemViewModel();
            viewModel.PurchaseItemId = item.Id;
            viewModel.ItemId = item.ItemId;
            viewModel.Qty = item.Qty;
            viewModel.Rate = item.Rate;
            viewModel.Total_ = item.Total;
            viewModel.TaxId = item.TaxId;
            viewModel.DiscountPercentage = item.DiscountPercentage;
            viewModel.DiscountAmount = item.DiscountAmount;
            viewModel.SalesTaxPercentage = item.SalesTaxPercentage;
            viewModel.SalesTaxAmount = item.SalesTaxAmount;
            viewModel.ExciseTaxPercentage = item.ExciseTaxPercentage;
            viewModel.ExciseTaxAmount = item.ExciseTaxAmount;
            viewModel.LineTotal = item.LineTotal;
            viewModel.Remarks = item.Remarks;
            viewModel.TaxList = appTaxRepo.GetTaxes(companyId);
            return Ok(viewModel);
        }

        public ActionResult UnApprove()
        {
            var purchaseVoucherRepo = new PurchaseVoucherRepo(_dbContext, HttpContext);
            var list = purchaseVoucherRepo.GetApprovedPurchaseInvoices();
            ViewBag.NavbarHeading = "Un-Approve Purchase Invoice";
            return View(list);
        }
        public async Task<IActionResult> UnApprovePurchaseVoucher(int id)
        {
            var purchaseVoucherRepo = new PurchaseVoucherRepo(_dbContext, HttpContext);
            var voucher = await purchaseVoucherRepo.UnApprovePurchaseVoucher(id);
            if (voucher == false)
            {
                TempData["error"] = "true";
                TempData["message"] = "Purchase Invoice not found";
            }
            else
            {
                TempData["error"] = "false";
                TempData["message"] = "Purchase Invoice has been Un-Approved successfully";
            }
            return RedirectToAction(nameof(UnApprove));
        }
        [HttpPost]
        public IActionResult GetPurchaseOrdersBySupplierId(int id, int[] skipIds)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            var configValues = new ConfigValues(_dbContext);
            var invoice = GetAllPurchaseOrdersBySupplierId(id, skipIds, companyId);
            List<APGRNViewModel> list = new List<APGRNViewModel>();
            foreach (var item in invoice)
            {
                var model = new APGRNViewModel();
                //model.PurchaseInvoiceId = item.Purchase.Id;
                model.GRNItemId = item.Id;
                model.GRNNO = item.GRN.GRNNO;
                model.Date = item.GRN.GRNDate.ToString(Helpers.CommonHelper.DateFormat);
                model.ItemId = item.ItemId;
                model.Item = item.Item;
                model.Remarks = configValues.GetUom(item.Item.Unit);
              //  model.GRNQty = item.Qty - item.PurchaseQty;
                model.GRNQty = item.GRNQty - item.PurchaseQty;
                model.BalanceQty = item.GRNQty - item.PurchaseQty;
                model.Rate = item.Rate;
                //    model.GrandTotal = Math.Round(model.Rate * model.GRNQty, 2);
                model.GrandTotal = item.GRN.GrandTotal;
                model.TotalValue= item.PKRValue;
                model.TotalPKRValue= item.Total_;
                model.TotalSaleTax = item.SaleTaxAmount;
                model.TotalValueIncTax = item.TotalValue;
                model.PRDetailId = item.PRDetailId;
                model.SaleTaxId = item.SaleTax;
                if (item.BrandId != 0)
                {
                    model.BrandId = item.BrandId;
                    model.BrandName = _dbContext.AppCompanyConfigs.FirstOrDefault(x=>x.Id == item.BrandId).ConfigValue;
                }
                list.Add(model);
            }
            ViewBag.Responsibility = _dbContext.Sys_Responsibilities.FirstOrDefault(x => x.Resp_Id == resp_Id).Resp_Name;
            return PartialView("_PurchaseOrderPopUp", list.Where(x=>x.BalanceQty>0).ToList());
        }
        public List<APGRNItem> GetAllPurchaseOrdersBySupplierId(int id, int[] skipIds, int companyId)
        {
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            var purchaseItems = _dbContext.APGRNItem.Include(i => i.GRN).Include(i => i.Item)
                .Where(i => i.GRN.CompanyId == companyId && i.GRN.Resp_ID == resp_Id && i.IsDeleted == false && i.GRN.VendorName == id.ToString())
                .Where(i => !skipIds.Contains(i.Id)).ToList();
            return purchaseItems;
        }
        [HttpPost]
        public APPurchaseItemViewModel GetPurchaseOrderItems(int purchaseInvoiceItemId, int counter)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var appTaxRepo = new AppTaxRepo(_dbContext);
            var item = GetPurchaseOrderItem(purchaseInvoiceItemId);
            ViewBag.Counter = counter;
            ViewBag.ItemId = item.ItemId;
            item.TaxList = appTaxRepo.GetTaxes(companyId);
            return item;
        }
        public APPurchaseItemViewModel GetPurchaseOrderItem(int id)
        {
            var item = _dbContext.APPurchaseOrderItems.Include(i => i.PO).Where(i => i.Id == id && i.IsDeleted == false).FirstOrDefault();
            APPurchaseItemViewModel viewModel = new APPurchaseItemViewModel();
            //viewModel.PurchaseInvoiceId = item.Purchase.Id;
            viewModel.PurchaseItemId = item.Id;
            viewModel.PurchaseOrderItemId = item.Id;
            viewModel.ItemId = item.ItemId;
            viewModel.Qty = item.Qty - item.PurchaseQty;
            viewModel.Rate = item.Rate;
            viewModel.Total_ = item.Total;
            viewModel.LineTotal = item.LineTotal;
            viewModel.TaxId = item.TaxId;
            viewModel.SalesTaxAmount = item.TaxAmount;
            return viewModel;
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            TempData["RoleName"] = (from a in _dbContext.Roles
                                    join b in _dbContext.UserRoles.Where(x => x.UserId == userId) on a.Id equals b.RoleId
                                    select a.Name).FirstOrDefault();
            TempData["ShowRate"] = true;
            string configValue = _dbContext.AppCompanyConfigs
                     .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                     .Select(c => c.ConfigValue)
                     .FirstOrDefault();
            ViewBag.ReportPath = string.Concat(configValue, "Viewer", "?Report=Voucher&cId=", companyId, "&id={0}");
            ViewBag.ReportPath2 = string.Concat(configValue, "Viewer", "?Report=PurchaseInvoice&cId=", companyId, "&showRate=" + TempData["ShowRate"], "&id={0}");

            var purchase = _dbContext.APPurchases.Include(i => i.Supplier)
            .Where(i => i.Id == id).FirstOrDefault();
            var purchaseItem = _dbContext.APPurchaseItems
                                .Include(i => i.Item)
                                .Include(i => i.Purchase)
                                .Where(i => i.PurchaseId == id && i.IsDeleted == false)
                                .ToList();
            ViewBag.NavbarHeading = "Purchase Invoice";
            ViewBag.TitleStatus = "Approved";

            TempData["Detail"] = purchaseItem;
            return View(purchase);
        }

        public IActionResult GetSalesManBySupplierId(int supplierId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var configValues = new ConfigValues(_dbContext);
            var saleman = _dbContext.AppCompanyConfigs.Where(x => x.BaseId == 39 && x.IsDeleted == false).ToList();
            return Ok(saleman);
        }
        public IActionResult GetItemById(int id)
        {
            var obj = _dbContext.InvItems.Where(x => x.Id == id && x.IsDeleted == false).FirstOrDefault();
            string Code = (from a in _dbContext.AppCompanyConfigs.Where(x => x.Id == obj.Unit) select a.ConfigValue).FirstOrDefault();
            obj.Code = Code;
            return Json(obj);
        }
        public IActionResult GetPurchase()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value; 
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            try
            {
                string userId = HttpContext.Session.GetString("UserId");
                var approve = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == resp_Id).FirstOrDefault().Approve;
                var unApprove = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == resp_Id).FirstOrDefault().UnApprove;
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();

                var searchPurchaseNo = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchGRNno = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchPurchaseDate = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchSupplier = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchOperatingUnit = Request.Form["columns[4][search][value]"].FirstOrDefault();
                var searchGrandTotal = Request.Form["columns[5][search][value]"].FirstOrDefault();
                var searchStatus = Request.Form["columns[6][search][value]"].FirstOrDefault();



                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var customerData = (from tempcustomer in _dbContext.APPurchases.Where(x => x.CompanyId == companyId && x.Resp_ID == resp_Id && x.IsDeleted == false && x.TransactionType == "Purchase").Include(a => a.Supplier) select tempcustomer);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                customerData = !string.IsNullOrEmpty(searchPurchaseNo) ? customerData.Where(m => m.PurchaseNo.ToString().Contains(searchPurchaseNo)) : customerData;
                customerData = !string.IsNullOrEmpty(searchPurchaseDate) ? customerData.Where(m => m.PurchaseDate != null ? m.PurchaseDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchPurchaseDate.ToUpper()) : false) : customerData;
                customerData = !string.IsNullOrEmpty(searchSupplier) ? customerData.Where(m => m.SupplierId != 0 ? _dbContext.APSuppliers.FirstOrDefault(x => x.Id == Convert.ToInt32(m.SupplierId)).Name.ToUpper().Contains(searchSupplier.ToUpper()) : false)   : customerData;
                customerData = !string.IsNullOrEmpty(searchGrandTotal) ? customerData.Where(m => m.GrandTotal.ToString().Contains(searchGrandTotal)) : customerData;
                customerData = !string.IsNullOrEmpty(searchStatus) ? customerData.Where(m => m.Status.ToString().ToUpper().Contains(searchStatus.ToUpper())) : customerData;

                //if (!string.IsNullOrEmpty(searchValue))
                //{
                //    customerData = customerData.Where(m => m.PurchaseNo.ToString().Contains(searchValue)
                //                                    || m.PurchaseDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchValue.ToUpper())
                //                                    || _dbContext.APSuppliers.FirstOrDefault(x => x.Id == Convert.ToInt32(m.SupplierId)).Name.ToUpper().Contains(searchValue.ToUpper())
                //                                    || m.GrandTotal.ToString().Contains(searchValue)
                //                                    || m.Status.ToString().ToUpper().Contains(searchValue.ToUpper())
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
                List<APPurchaseItemViewModel> Details = new List<APPurchaseItemViewModel>();
                foreach (var grp in data)
                {
                    APPurchaseItemViewModel purchase = new APPurchaseItemViewModel();

                    purchase.Purchase = grp;
                    purchase.Purchase.Approve = approve;
                    purchase.Purchase.Unapprove = unApprove;
                    purchase.Purchase = grp;
                    var grmitemId = _dbContext.APPurchaseItems.Where(x => x.PurchaseId == grp.Id).FirstOrDefault();
                    purchase.PDate=grp.PurchaseDate.ToString(Helpers.CommonHelper.DateFormat);
                    purchase.Suplier= _dbContext.APSuppliers.FirstOrDefault(x => x.Id == grp.SupplierId).Name;
                    if (grmitemId != null)
                    {
                        purchase.GRNNo = (from a in _dbContext.APGRNItem.Where(x => x.Id == grmitemId.GRNItemId)
                                          join b in _dbContext.APGRN on a.GRNID equals b.Id
                                          select b.GRNNO).FirstOrDefault();
                        //purchase.OperationId = (from a in _dbContext.APPurchaseRequisitionDetails.Where(x => x.Id == grmitemId.PrDetailId)
                        //                        join b in _dbContext.APPurchaseRequisition on a.APPurchaseRequisitionId equals b.Id
                        //                        select b.OperationId).FirstOrDefault();

                        purchase.OperatingUnit = _dbContext.SysOrganization.Where(x => x.Organization_Id == purchase.Purchase.OperationId).Select(x => x.OrgName).FirstOrDefault();
                    }
                    else
                    {
                        purchase.GRNNo = 0;
                        purchase.OperatingUnit = "0";
                    }
                    
                    purchase.WareHose=(from c in _dbContext.AppCompanyConfigs where c.Id == grp.WareHouseId select c.ConfigValue).FirstOrDefault();
                    Details.Add(purchase);

                }
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = Details };
                return Ok(jsonData);
            }
            catch (Exception)
            {
                throw;
            }
        }
  
    
    }
}