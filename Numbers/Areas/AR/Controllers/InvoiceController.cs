using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Helpers;
using Numbers.Repository.Helpers;
using Numbers.Repository.Setup;
using Numbers.Repository.Vouchers;
using System.Linq.Dynamic.Core;

namespace Numbers.Areas.AR.Controllers
{
    [Authorize]
    [Area("AR")]
    public class InvoiceController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public InvoiceController(NumbersDbContext context)
        {
            _dbContext = context;
        }

        public IActionResult Index()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string configValues = _dbContext.AppCompanyConfigs
                                            .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                                            .Select(c => c.ConfigValue)
                                            .FirstOrDefault();
            ViewBag.ReportPath = string.Concat(configValues, "Viewer", "?Report=Voucher&cId=", companyId, "&id={0}");
            ViewBag.ReportUrl = configValues;
            ViewBag.NavbarHeading = "List of Sales Invoices";
            List<ARInvoice> model;
             model= _dbContext.ARInvoices.Include(s => s.Customer).Where(c => c.CompanyId == companyId && c.TransactionType == "Sale"  && !c.IsClosed && !c.IsDeleted)
                                           
                                             .ToList();
            
            return View(model);
        }

        [HttpGet]
        public IActionResult Create(int id)
        {            
            ViewBag.Counter = 0;
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var appTaxRepo = new AppTaxRepo(_dbContext);
            ViewBag.Customers = (from a in _dbContext.ARCustomers.Where(x =>/* x.CompanyId == companyId*/x.IsDeleted != true && x.IsActive != false) select a.Name).ToList();
            //ViewBag.Customers = new SelectList(_dbContext.ARCustomers.Where(s => s.IsActive==true && s.CompanyId == companyId).ToList(), "Id", "Name").ToList();
            string configValues = _dbContext.AppCompanyConfigs
                                .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                                .Select(c => c.ConfigValue)
                                .FirstOrDefault();
            ViewBag.ReportPath = string.Concat(configValues, "Viewer", "?Report=Voucher&cId=", companyId, "&id={0}");
            ViewBag.Items = _dbContext.InvItems.Where(a => a.CompanyId == companyId && a.IsDeleted == false && a.IsActive == true)
                                   .Select(a => new
                                   {
                                       id = a.Id,
                                       name = a.Name
                                   });
            var configValue = new ConfigValues(_dbContext);
            ViewBag.WareHouse = configValue.GetConfigValues("Inventory", "Ware House", companyId);
            ViewBag.CostCenter = new SelectList(_dbContext.CostCenter.Where(a => a.IsDeleted == false).ToList(), "Id", "Description");
            ViewBag.Customer = new SelectList(_dbContext.ARCustomers.Where(a => /*a.CompanyId == companyId &&*/ a.IsActive != false && a.IsDeleted != true).ToList(), "Id", "Name");
            ViewBag.ItemList = new SelectList(_dbContext.InvItems.Where(a => a.Id == id && a.CompanyId == companyId && a.IsDeleted == false).ToList(), "Id", "Name");
            if (id == 0)
            {
                int maxInvoiceNo = 1;
                var invoices = _dbContext.ARInvoices.Where(c => c.CompanyId == companyId && c.TransactionType=="Sale").ToList();
                //if (invoices.Count > 0)
                //{
                //    //maxInvoiceNo = invoices.Max(v => Convert.v.InvoiceNo);
                //    ViewBag.Id = _dbContext.ARInvoices.Select(x => x.InvoiceNo).Max() + 1;

                //    TempData["InvoiceNo"] = "";
                //        }
                //else
                //{
                //    ViewBag.Id = 1;
                //    TempData["InvoiceNo"] = maxInvoiceNo;
                //}
                var model = new ARInvoiceViewModel();
                model.TaxList = appTaxRepo.GetTaxes(companyId);
                model.Currencies = AppCurrencyRepo.GetCurrencies();
                model.CostCenterList = _dbContext.CostCenter.Where(t => t.CompanyId == companyId && t.IsDeleted == false).ToList();
                ViewBag.Status = "Created";
                ViewBag.EntityState = "Create";
                ViewBag.NavbarHeading = "Create Sales Invoice";
                return View(model);
            }
            else
            {
                var invoice = _dbContext.ARInvoices.Find(id);
                var viewModel = new ARInvoiceViewModel();
                viewModel.Id = invoice.Id;
                viewModel.InvoiceNo = invoice.InvoiceNo;
                viewModel.InvoiceDate = invoice.InvoiceDate;
                viewModel.InvoiceDueDate = invoice.InvoiceDueDate;
                viewModel.WareHouseId = invoice.WareHouseId;
                viewModel.CustomerId = invoice.CustomerId;
                viewModel.ReferenceNo = invoice.ReferenceNo;
                viewModel.CustomerPONo = invoice.CustomerPONo;
                viewModel.OGPNo = invoice.OGPNo;
                viewModel.Vehicle = invoice.Vehicle;
                viewModel.Remarks = invoice.Remarks;
                viewModel.Currency = invoice.Currency;
                viewModel.VoucherId = invoice.VoucherId;
                viewModel.Status = invoice.Status;
                viewModel.CostCenter = invoice.CostCenter;
                viewModel.CurrencyExchangeRate = invoice.CurrencyExchangeRate;

                TempData["InvoiceNo"] = invoice.InvoiceNo;
                ViewBag.Id = id;
                var invoiceItems = _dbContext.ARInvoiceItems
                                    .Include(i => i.Item)
                                    .Include(i => i.Invoice)
                                    .Where(i => i.InvoiceId == id && i.IsDeleted == false)
                                    .ToList();
                ViewBag.InvoiceItems = invoiceItems;
                //ViewBag.EntityState = "Update";
                //viewModel.TaxList = appTaxRepo.GetTaxes(companyId);
                viewModel.Currencies = AppCurrencyRepo.GetCurrencies();
                viewModel.CostCenterList = _dbContext.CostCenter.Where(t => t.CompanyId == companyId && t.IsDeleted == false).ToList();
                if (viewModel.Status != "Approved")
                {
                    ViewBag.EntityState = "Update";
                    ViewBag.NavbarHeading = "Sale Invoice";
                    ViewBag.TitleStatus = "Created";
                }
                //else if (viewModel.Status == "Approved")
                //{
                //    ViewBag.NavbarHeading = "Sale Invoice";
                //    ViewBag.TitleStatus = "Approved";
                //}
                return View(viewModel);
            }
        }

        [HttpGet]
        public IActionResult GetInvoiceItems(int id, int itemId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var appTaxRepo = new AppTaxRepo(_dbContext);
            var item = _dbContext.ARInvoiceItems.Include(i => i.Invoice).Include(i => i.Item).Where(i => i.Id == id && i.IsDeleted != true).FirstOrDefault();
            ViewBag.Counter = id;
            ARInvoiceViewModel viewModel = new ARInvoiceViewModel();
            if (item != null)
            {
                viewModel.InvoiceItemId = item.Id;
                viewModel.DCNo = item.DCNo;
                viewModel.DCItemId = item.DCItemId;
                viewModel.ItemId = item.ItemId;
                viewModel.Qty = item.Qty;
                viewModel.Stock = item.Stock;
                viewModel.Rate = item.Rate;
                viewModel.Meters = item.Meters;
                viewModel.TotalMeter = item.TotalMeter;
                viewModel.TotalMeterAmount = item.TotalMeterAmount;
                viewModel.IssueRate = item.IssueRate;
                viewModel.CostofSales = item.CostofSales;
                viewModel.Total_ = item.Total;
                viewModel.TaxSlab = item.TaxSlab;
                viewModel.DiscountPercentage = item.DiscountPercentage;
                viewModel.DiscountAmount = item.DiscountAmount;
                viewModel.SalesTaxPercentage = item.SalesTaxPercentage;
                viewModel.SalesTaxAmount = item.SalesTaxAmount;
                viewModel.ExciseTaxPercentage = item.ExciseTaxPercentage;
                viewModel.ExciseTaxAmount = item.ExciseTaxAmount;
                viewModel.LineTotal = item.LineTotal;
                viewModel.DetailCostCenter = item.DetailCostCenter;
                viewModel.InvoiceItemRemarks = item.Remarks;
                ViewBag.ItemId = itemId;
                viewModel.TaxList = appTaxRepo.GetTaxes(companyId);
                viewModel.CostCenterList = _dbContext.CostCenter.Where(t => t.CompanyId == companyId && t.IsDeleted == false).ToList();
            }
            return PartialView("_partialInvoiceItems", viewModel);
        }
        public async Task<IActionResult> Create(ARInvoiceViewModel model, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;

            var customer = _dbContext.ARCustomers.Where(m => m.Id == model.CustomerId).FirstOrDefault();
            var max = _dbContext.ARInvoices.Where(x => x.CompanyId == companyId).Max(x => x.InvoiceNo) + 1;
            if (collection["ItemId"].Count > 0)
            {
                ARInvoice invoice = new ARInvoice();
                //Master Table data
                invoice.Id = model.Id;
                invoice.InvoiceNo = max;
                invoice.InvoiceDate = model.InvoiceDate;
                invoice.InvoiceDueDate = model.InvoiceDueDate;
                invoice.WareHouseId = model.WareHouseId;
                invoice.CustomerId = model.CustomerId;
                invoice.SalesPersonId = customer.SalesPersonId;
                invoice.ReferenceNo = model.ReferenceNo;
                invoice.CustomerPONo = model.CustomerPONo;
                invoice.OGPNo = model.OGPNo;
                invoice.Vehicle = model.Vehicle;
                invoice.Remarks = collection["Remarks"][0];
                invoice.Currency = model.Currency;
                invoice.CurrencyExchangeRate = model.CurrencyExchangeRate;
                invoice.TransactionType = "Sale";
                invoice.InvoiceType = "INV";
                invoice.Total = Convert.ToDecimal(collection["Total"]);
                invoice.DiscountAmount = Convert.ToDecimal(collection["totalDiscountAmount"]);
                invoice.SalesTaxAmount = Convert.ToDecimal(collection["totalSalesTaxAmount"]);
                invoice.ExciseTaxAmount = Convert.ToDecimal(collection["totalExciseTaxAmount"]);
                invoice.GrandTotal = Convert.ToDecimal(collection["GrandTotal"]);
                invoice.CreatedBy = userId;
                invoice.CreatedDate = DateTime.Now;
                invoice.CompanyId = companyId;
                invoice.ResponsibilityId = resp_Id;
                invoice.Status = "Created";
                invoice.CostCenter = model.CostCenter;

                _dbContext.ARInvoices.Add(invoice);
                await _dbContext.SaveChangesAsync();

                for (int i = 0; i < collection["ItemId"].Count; i++)
                {
                    var invoiceItem = new ARInvoiceItem();
                    invoiceItem.InvoiceId = invoice.Id;
                    invoiceItem.DCNo= Convert.ToString(collection["DCNo"][i]);
                    invoiceItem.ItemId = Convert.ToInt32(collection["ItemId"][i]);
                    invoiceItem.Qty = Convert.ToDecimal(collection["Qty"][i]);
                    invoiceItem.AvgRate = _dbContext.InvItems.Find(invoiceItem.ItemId).AvgRate;
                    invoiceItem.Rate = Convert.ToDecimal(collection["Rate"][i]);

                    invoiceItem.Meters = Convert.ToDecimal(collection["Meters"][i]);
                    invoiceItem.TotalMeter = Convert.ToDecimal(collection["TotalMeter"][i]);
                    invoiceItem.TotalMeterAmount = Convert.ToDecimal(collection["TotalMeterAmount"][i]);

                    invoiceItem.Stock = Convert.ToDecimal(collection["Stock"][i]);
                    invoiceItem.IssueRate = Convert.ToDecimal(collection["IssueRate"][i]);
                    invoiceItem.CostofSales = Convert.ToDecimal(collection["IssueRate"][i]) * Convert.ToDecimal(collection["Qty"][i]);
                    invoiceItem.Total = Convert.ToDecimal(collection["Total_"][i]);
                    //invoiceItem.TaxSlab = Convert.ToInt32(collection["TaxSlab"][i]);
                    invoiceItem.DiscountPercentage = Convert.ToDecimal(collection["DiscountPercentage"][i]);
                    invoiceItem.DiscountAmount = Convert.ToDecimal(collection["DiscountAmount"][i]);
                    invoiceItem.SalesTaxPercentage = Convert.ToDecimal(collection["SalesTaxPercentage"][i]);
                    invoiceItem.SalesTaxAmount = Convert.ToDecimal(collection["SalesTaxAmount"][i]);
                    invoiceItem.ExciseTaxPercentage = Convert.ToDecimal(collection["ExciseTaxPercentage"][i]);
                    invoiceItem.ExciseTaxAmount = Convert.ToDecimal(collection["ExciseTaxAmount"][i]);
                    var total = Convert.ToDecimal(collection["SalesTaxAmount"][i]) + Convert.ToDecimal(collection["LineTotal"][i]);
                    invoiceItem.LineTotal = total;
                    invoiceItem.Remarks = collection["Remarks"][i + 1];
                    invoiceItem.DCItemId = Convert.ToInt32(collection["DCItemId"][i]);
                    invoiceItem.SalesOrderItemId = Convert.ToInt32(collection["SalesOrderItemId"][i]);
                    invoiceItem.DetailCostCenter = Convert.ToInt32(collection["DetailCostCenter"][i]);
                    if (invoiceItem.DetailCostCenter == 0)
                    {
                        invoiceItem.DetailCostCenter = invoice.CostCenter;
                    }
                    _dbContext.ARInvoiceItems.Add(invoiceItem);
                    await _dbContext.SaveChangesAsync();

                    var deliverItems = _dbContext.ARDeliveryChallanItems.Where(x => x.Id == invoiceItem.DCItemId).FirstOrDefault();
                    deliverItems.SaleQty = deliverItems.SaleQty + invoiceItem.Qty;
                      _dbContext.ARDeliveryChallanItems.Update(deliverItems);
                    await _dbContext.SaveChangesAsync();
                }
                TempData["error"] = "false";
                TempData["message"] = string.Format("Invoice No. {0} has been created successfully", max);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "No any Invoice has been Created. It must contain atleast one item";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update(ARInvoiceViewModel model, IFormCollection collection)
        {
            //deleting invoices
            string[] idsDeleted = Convert.ToString(collection["IdsDeleted"]).Split(",");
            if (!idsDeleted.Contains(""))
            {
                for (int j = 0; j < idsDeleted.Length; j++)
                {
                    if (idsDeleted[j] != "0")
                    {
                        var itemToRemove = _dbContext.ARInvoiceItems.Find(Convert.ToInt32(idsDeleted[j]));
                        itemToRemove.IsDeleted = true;
                        _dbContext.ARInvoiceItems.Update(itemToRemove);
                        await _dbContext.SaveChangesAsync();
                    }
                }
            }
            if (collection["ItemId"].Count > 0)
            {
                //updating existing data
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                string userId = HttpContext.Session.GetString("UserId");
                int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
                ARInvoice invoice = _dbContext.ARInvoices.Find(model.Id);
                invoice.InvoiceNo = model.InvoiceNo;
                invoice.InvoiceDate = model.InvoiceDate;
                invoice.InvoiceDueDate = model.InvoiceDueDate;
                invoice.WareHouseId = model.WareHouseId;
                invoice.CustomerId = model.CustomerId;
                invoice.ReferenceNo = model.ReferenceNo;
                invoice.CustomerPONo = model.CustomerPONo;
                invoice.OGPNo = model.OGPNo;
                invoice.Vehicle = model.Vehicle;
                invoice.Remarks = collection["Remarks"][0];
                invoice.Currency = model.Currency;
                invoice.CurrencyExchangeRate = model.CurrencyExchangeRate;
                invoice.Total = Convert.ToDecimal(collection["Total"]);
                invoice.DiscountAmount = Convert.ToDecimal(collection["totalDiscountAmount"]);
                invoice.SalesTaxAmount = Convert.ToDecimal(collection["totalSalesTaxAmount"]);
                invoice.ExciseTaxAmount = Convert.ToDecimal(collection["totalExciseTaxAmount"]);
                invoice.GrandTotal = Convert.ToDecimal(collection["GrandTotal"]);
                invoice.UpdatedBy = userId;
                invoice.UpdatedDate = DateTime.Now;
                invoice.CostCenter = model.CostCenter; 
                invoice.CompanyId = companyId;
                invoice.ResponsibilityId = resp_Id;
                var entry = _dbContext.ARInvoices.Update(invoice);
                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                await _dbContext.SaveChangesAsync();
                //Update Invoice detail items/ Invoice items if any
                for (int i = 0; i < collection["ItemId"].Count; i++)
                {
                    var invoiceItem = _dbContext.ARInvoiceItems
                        .Where(j => j.InvoiceId == model.Id && j.Id == Convert.ToInt32(collection["InvoiceItemId"][i] == "" ? 0 : Convert.ToInt32(collection["InvoiceItemId"][i]))).FirstOrDefault();
                    // Extract coresponding values from form collection
                    var itemId = Convert.ToInt32(collection["ItemId"][i]);
                    var DCNo = Convert.ToString(collection["DCNo"][i]);
                    var avgRate = _dbContext.InvItems.Find(itemId).AvgRate;
                    var qty = Convert.ToDecimal(collection["Qty"][i]);
                    var stock = Convert.ToDecimal(collection["Stock"][i]);
                    var rate = Convert.ToDecimal(collection["Rate"][i]);
                    var issueRate = Convert.ToDecimal(collection["IssueRate"][i]);
                    var costofSales = Convert.ToDecimal(collection["CostofSales"][i]);
                    var total = Convert.ToDecimal(collection["Total_"][i]);
                    //var taxSlab = Convert.ToInt32(collection["TaxSlab"][i]);
                    var discountPercentage = Convert.ToDecimal(collection["DiscountPercentage"][i]);
                    var discountAmount = Convert.ToDecimal(collection["DiscountAmount"][i]);
                    var salesTaxPercentage = Convert.ToDecimal(collection["SalesTaxPercentage"][i]);
                    var salesTaxAmount = Convert.ToDecimal(collection["SalesTaxAmount"][i]);
                    var exciseTaxPercentage = Convert.ToDecimal(collection["ExciseTaxPercentage"][i]);
                    var exciseTaxAmount = Convert.ToDecimal(collection["ExciseTaxAmount"][i]);
                    var linetotal = Convert.ToDecimal(collection["LineTotal"][i]);
                    var remarks = collection["Remarks"][i + 1];
                    var saleOrderItemId = Convert.ToInt32(collection["SalesOrderItemId"][i]);
                    var costcenter = Convert.ToInt32(collection["DetailCostCenter"][i]);

                    var meters = Convert.ToDecimal(collection["Meters"][i]);
                    var totalMeter = Convert.ToDecimal(collection["TotalMeter"][i]);
                    var totalMeterAmount = Convert.ToDecimal(collection["TotalMeterAmount"][i]);
                    var dCItemId = Convert.ToInt32(collection["DCItemId"][i]);

                    if (invoiceItem != null && itemId != 0)
                    {
                        //below phenomenon prevents Id from being marked as modified
                        var entityEntry = _dbContext.Entry(invoiceItem);
                        entityEntry.State = EntityState.Modified;
                        entityEntry.Property(p => p.Id).IsModified = false;
                        invoiceItem.ItemId = itemId;
                        invoiceItem.DCNo = DCNo;
                        invoiceItem.DCItemId = dCItemId;
                        invoiceItem.InvoiceId = model.Id;
                        invoiceItem.Qty = qty;
                        invoiceItem.Stock = stock;
                        invoiceItem.Bonus = 0;
                        invoiceItem.DiscountAmount = 0;
                        invoiceItem.Rate = rate;
                        invoiceItem.IssueRate = issueRate;
                        invoiceItem.CostofSales = costofSales;
                        invoiceItem.AvgRate = avgRate;
                        invoiceItem.Total = total;
                        //invoiceItem.TaxSlab = taxSlab;
                        invoiceItem.DiscountPercentage = discountPercentage;
                        invoiceItem.DiscountAmount = discountAmount;
                        invoiceItem.SalesTaxPercentage = salesTaxPercentage;
                        invoiceItem.SalesTaxAmount = salesTaxAmount;
                        invoiceItem.ExciseTaxPercentage = exciseTaxPercentage;
                        invoiceItem.ExciseTaxAmount = exciseTaxAmount;
                        invoiceItem.LineTotal = linetotal;
                        invoiceItem.Remarks = remarks;
                        invoiceItem.Meters = meters;
                        invoiceItem.TotalMeter = totalMeter;
                        invoiceItem.TotalMeterAmount = totalMeterAmount;
                        invoiceItem.DetailCostCenter = costcenter;
                        if (invoiceItem.DetailCostCenter == 0)
                        {
                            invoiceItem.DetailCostCenter = invoice.CostCenter;
                        }
                        var dbEntry = _dbContext.ARInvoiceItems.Update(invoiceItem);
                        dbEntry.OriginalValues.SetValues(await entityEntry.GetDatabaseValuesAsync());

                    }
                    //check if user created new item while updating
                    else if (itemId != 0 && invoiceItem == null)//itemId is invitem, if this is null or zero rest of the information for this item will not be saved.
                    {
                        var newItem = new ARInvoiceItem();
                        newItem.InvoiceId = model.Id;
                        newItem.ItemId = itemId;
                        newItem.DCItemId = dCItemId;
                        newItem.DCNo = DCNo;
                        newItem.Qty = qty;
                        newItem.Stock = stock;
                        newItem.Rate = rate;
                        newItem.IssueRate = issueRate;
                        newItem.CostofSales = costofSales;
                        newItem.AvgRate = avgRate;
                        newItem.Total = total;
                        //newItem.TaxSlab = taxSlab;
                        newItem.DiscountPercentage = discountPercentage;
                        newItem.DiscountAmount = discountAmount;
                        newItem.SalesTaxPercentage = salesTaxPercentage;
                        newItem.SalesTaxAmount = salesTaxAmount;
                        newItem.ExciseTaxPercentage = exciseTaxPercentage;
                        newItem.ExciseTaxAmount = exciseTaxAmount;
                        newItem.LineTotal = linetotal;
                        newItem.Remarks = remarks;
                        newItem.Meters = meters;
                        newItem.TotalMeter = totalMeter;
                        newItem.TotalMeterAmount = totalMeterAmount;
                        newItem.DetailCostCenter = costcenter;
                        if (newItem.DetailCostCenter == 0)
                        {
                            newItem.DetailCostCenter = invoice.CostCenter;
                        }
                        newItem.SalesOrderItemId = saleOrderItemId;
                        _dbContext.ARInvoiceItems.Add(newItem);
                    }                    
                }
                TempData["error"] = "false";
                TempData["message"] = string.Format("Invoice No. {0} has been updated successfully", invoice.InvoiceNo);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "No any Invoice has been updated. It must contain atleast one item";
                return RedirectToAction(nameof(Index));
            }
        }
        public IActionResult GetUOM(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var items = from item in _dbContext.InvItems
                        join config in _dbContext.AppCompanyConfigs on item.Id equals id
                        where item.Id == id && config.ConfigName == "UOM" && config.Module == "Inventory" && config.Id == item.Unit && config.CompanyId == companyId
                        select new {
                            uom = config.ConfigValue,
                            id = config.Id,
                            rate = item.PurchaseRate,
                            avgRate = item.AvgRate,
                            stock=item.StockAccountId
                         };
            return Ok(items);
        }
        public async Task <IActionResult> Approve(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");

            //BkgVehicleIGP bkgIGP = _dbContext.BkgVehicleIGPs.Find(id);
            ARInvoice invoice = _dbContext.ARInvoices
             .Include(c => c.Customer)
             .Where(a => a.Status == "Created" && a.CompanyId == _companyId && a.Id == id && a.IsDeleted == false)
             .FirstOrDefault();
            try
            {
                //Create Voucher
                Numbers.Helpers.VoucherHelper voucher = new Numbers.Helpers.VoucherHelper(_dbContext, HttpContext);
                GLVoucher voucherMaster = new GLVoucher();
                List<GLVoucherDetail> voucherDetails = new List<GLVoucherDetail>();
                string voucherDescription = string.Format(
                "Invoice # : {0} of  " +
                "{1} {2}",
                invoice.InvoiceNo,
                invoice.Customer.Name,invoice.Remarks);

                int voucherId;
                voucherMaster.VoucherType = "INV";
                voucherMaster.VoucherDate = invoice.InvoiceDate;
                voucherMaster.Reference = invoice.ReferenceNo;
                voucherMaster.Currency = invoice.Currency;
                voucherMaster.CurrencyExchangeRate = invoice.CurrencyExchangeRate;
                voucherMaster.Description = voucherDescription.Substring(0, voucherDescription.Length);
                voucherMaster.Status = "Approved";
                voucherMaster.ApprovedBy = _userId;
                voucherMaster.ApprovedDate = DateTime.Now;
                voucherMaster.IsSystem = true;
                voucherMaster.ModuleName = "AR/Invoice";
                voucherMaster.ModuleId = id;
                voucherMaster.Amount = invoice.GrandTotal;
                voucherMaster.ReferenceId = invoice.CustomerId;
                //Voucher Details
                var invoiceItems = _dbContext.ARInvoiceItems.Where(i => i.InvoiceId == invoice.Id && !i.IsDeleted).ToList();
                var amount = invoiceItems.Sum(s=>s.LineTotal);
                var discount = invoiceItems.Sum(s => s.DiscountAmount);
                //Debit Entry
                GLVoucherDetail voucherDetail = new GLVoucherDetail();

                voucherDetail.AccountId = invoice.Customer.AccountId;
                voucherDetail.Sequence = 10;
                voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                voucherDetail.Debit = amount;  
                voucherDetail.Credit = 0;
                voucherDetail.IsDeleted = false;
                voucherDetail.CreatedBy = _userId;
                voucherDetail.CreatedDate = DateTime.Now;
                voucherDetails.Add(voucherDetail);
                //for discount
                //if (discount > 0)
                //{
                //    var accountCode = _dbContext.AppCompanyConfigs.Where(c => c.CompanyId == _companyId && c.ConfigName == "Discount" && c.IsActive).FirstOrDefault().UserValue1;
                //    var discountAccount = _dbContext.GLAccounts.Where(a => a.Code == accountCode && a.CompanyId == _companyId && a.IsActive).FirstOrDefault().Id;
                //    voucherDetail = new GLVoucherDetail();
                //    voucherDetail.AccountId = discountAccount;
                //    voucherDetail.Sequence = 10;
                //    voucherDetail.Description = invoice.Remarks;
                //    voucherDetail.Debit = discount;
                //    voucherDetail.Credit = 0;
                //    voucherDetail.IsDeleted = false;
                //    voucherDetail.CreatedBy = _userId;
                //    voucherDetail.CreatedDate = DateTime.Now;
                //    voucherDetails.Add(voucherDetail);
                //}
                #region Sale Account
                //Credit Entry
                
                var itemAccounts = (from li in _dbContext.ARInvoiceItems
                                    join i in _dbContext.InvItems.Include(a => a.InvItemAccount) on li.ItemId equals i.Id
                                    where li.InvoiceId == id && li.IsDeleted==false
                                    select new
                                    {
                                        li.Total,
                                        li.DiscountAmount,
                                        i.InvItemAccount.GLSaleAccountId
                                    }).GroupBy(l => l.GLSaleAccountId)
                                    .Select(li => new ARInvoiceItem{
                               Total = li.Sum(c => c.Total)- li.Sum(d=>d.DiscountAmount),
                               InvoiceId = li.FirstOrDefault().GLSaleAccountId //invoice id is temporarily containing GLSaleAccountId
                           }).ToList();

                foreach (var item in itemAccounts)
                {
                    if (item.Total > 0)
                    {
                        voucherDetail = new GLVoucherDetail();
                        voucherDetail.AccountId = item.InvoiceId;
                        voucherDetail.Sequence = 20;
                        voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                        voucherDetail.Debit = 0;
                        voucherDetail.Credit = item.Total;
                        voucherDetail.IsDeleted = false;
                        voucherDetail.CreatedBy = _userId;
                        voucherDetail.CreatedDate = DateTime.Now;
                        voucherDetails.Add(voucherDetail);
                    }
                }
                #endregion Sale Account
                #region Sales Tax 
                var saleTaxAccounts = (from li in _dbContext.ARInvoiceItems
                                       join i in _dbContext.InvItems on li.ItemId equals i.Id
                                       join t in _dbContext.AppTaxes on li.TaxSlab equals t.Id
                                       where li.InvoiceId == id
                                       select new
                                       {
                                           li.SalesTaxAmount,
                                           t.SalesTaxAccountId
                                       }).GroupBy(l => l.SalesTaxAccountId)
                                        .Select(li => new ARInvoiceItem
                                        {
                                            SalesTaxAmount = li.Sum(c => c.SalesTaxAmount),
                                            InvoiceId = li.FirstOrDefault().SalesTaxAccountId //invoice id is temporarily containing SalesTaxAccountId
                                        }).ToList();
                foreach (var item in saleTaxAccounts)
                {
                    if (item.SalesTaxAmount > 0)
                    {
                        voucherDetail = new GLVoucherDetail();
                        voucherDetail.AccountId = item.InvoiceId;
                        voucherDetail.Sequence = 20;
                        voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                        voucherDetail.Debit = 0;
                        voucherDetail.Credit = item.SalesTaxAmount;
                        voucherDetail.IsDeleted = false;
                        voucherDetail.CreatedBy = _userId;
                        voucherDetail.CreatedDate = DateTime.Now;
                        voucherDetails.Add(voucherDetail);
                    }
                }
                #endregion Sales Tax
                #region Excise Tax 
                var exciseTaxAccounts = (from li in _dbContext.ARInvoiceItems
                                       join i in _dbContext.InvItems on li.ItemId equals i.Id
                                       join t in _dbContext.AppTaxes on li.TaxSlab equals t.Id
                                       where li.InvoiceId == id
                                       select new
                                       {
                                           li.ExciseTaxAmount,
                                           t.ExciseTaxAccountId
                                       }).GroupBy(l => l.ExciseTaxAccountId)
                                        .Select(li => new ARInvoiceItem
                                        {
                                            ExciseTaxAmount = li.Sum(c => c.ExciseTaxAmount),
                                            InvoiceId = li.FirstOrDefault().ExciseTaxAccountId //invoice id is temporarily containing SalesTaxAccountId
                                        }).ToList();
                foreach (var item in exciseTaxAccounts)
                {
                    if (item.ExciseTaxAmount > 0)
                    {
                        voucherDetail = new GLVoucherDetail();
                        voucherDetail.AccountId = item.InvoiceId;
                        voucherDetail.Sequence = 20;
                        voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                        voucherDetail.Debit = 0;
                        voucherDetail.Credit = item.ExciseTaxAmount;
                        voucherDetail.IsDeleted = false;
                        voucherDetail.CreatedBy = _userId;
                        voucherDetail.CreatedDate = DateTime.Now;
                        voucherDetails.Add(voucherDetail);
                    }
                }
                #endregion Excise Tax
                #region Cost of Sale Account
                //Debit Entry
                var costOfSaleAccount = (from li in _dbContext.ARInvoiceItems
                                    join i in _dbContext.InvItems.Include(a => a.InvItemAccount) on li.ItemId equals i.Id
                                    where li.InvoiceId == id && li.IsDeleted == false
                                    select new
                                    {
                                        li.CostofSales,
                                        i.InvItemAccount.GLCostofSaleAccountId
                                    }).GroupBy(l => l.GLCostofSaleAccountId)
                                    .Select(li => new ARInvoiceItem
                                    {
                                        CostofSales = li.Sum(c => c.CostofSales),
                                        InvoiceId = li.FirstOrDefault().GLCostofSaleAccountId //invoice id is temporarily containing GLCostofSaleAccountId
                                    }).ToList();

                foreach (var item in costOfSaleAccount)
                {
                    if (item.CostofSales > 0)
                    {
                        voucherDetail = new GLVoucherDetail();
                        voucherDetail.AccountId = item.InvoiceId;
                        voucherDetail.Sequence = 20;
                        voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                        voucherDetail.Debit = item.CostofSales;
                        voucherDetail.Credit = 0;
                        voucherDetail.IsDeleted = false;
                        voucherDetail.CreatedBy = _userId;
                        voucherDetail.CreatedDate = DateTime.Now;
                        voucherDetails.Add(voucherDetail);
                    }
                }
                #endregion Cost of Sale Account
                #region Asset/Stock Account
                //Credit Entry
                var assetAcount = (from li in _dbContext.ARInvoiceItems
                                         join i in _dbContext.InvItems.Include(a => a.InvItemAccount) on li.ItemId equals i.Id
                                         where li.InvoiceId == id && li.IsDeleted == false
                                         select new
                                         {
                                             li.CostofSales,
                                             i.InvItemAccount.GLAssetAccountId
                                         }).GroupBy(l => l.GLAssetAccountId)
                                    .Select(li => new ARInvoiceItem
                                    {
                                        CostofSales = li.Sum(c => c.CostofSales),
                                        InvoiceId = li.FirstOrDefault().GLAssetAccountId //invoice id is temporarily containing GLAssetAccountId
                                    }).ToList();

                foreach (var item in assetAcount)
                {
                    if (item.CostofSales > 0)
                    {
                        voucherDetail = new GLVoucherDetail();
                        voucherDetail.AccountId = item.InvoiceId;
                        voucherDetail.Sequence = 20;
                        voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                        voucherDetail.Debit = 0;
                        voucherDetail.Credit = item.CostofSales;
                        voucherDetail.IsDeleted = false;
                        voucherDetail.CreatedBy = _userId;
                        voucherDetail.CreatedDate = DateTime.Now;
                        voucherDetails.Add(voucherDetail);
                    }
                }
                #endregion Asset/Stock Account
                //Create Voucher 
                voucherId = voucher.CreateVoucher(voucherMaster, voucherDetails);
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
                    using (var transaction = _dbContext.Database.BeginTransaction())
                    {
                        await _dbContext.SaveChangesAsync();
                        foreach (var invoiceItem in invoiceItems)
                        {
                            var item = _dbContext.InvItems.Find(invoiceItem.ItemId);
                            item.StockQty = item.StockQty - invoiceItem.Qty;
                            item.StockValue = item.StockValue - (item.AvgRate * invoiceItem.Qty);
                            var dbEntry = _dbContext.InvItems.Update(item);
                            dbEntry.OriginalValues.SetValues(await dbEntry.GetDatabaseValuesAsync());
                            await _dbContext.SaveChangesAsync();
                        }
                        await _dbContext.SaveChangesAsync();
                        var lineItems = _dbContext.ARInvoiceItems.Where(i => i.IsDeleted == false && i.InvoiceId == id).ToList();
                        foreach (var item in lineItems)
                        {
                            if (item.SalesOrderItemId != 0)
                            {
                                var saleOrderItem = _dbContext.ARSaleOrderItems.Find(item.SalesOrderItemId);
                                saleOrderItem.SaleQty = saleOrderItem.SaleQty + item.Qty;
                                var dbEntry = _dbContext.ARSaleOrderItems.Update(saleOrderItem);
                                dbEntry.OriginalValues.SetValues(await dbEntry.GetDatabaseValuesAsync());
                                await _dbContext.SaveChangesAsync();
                            }
                        }
                        transaction.Commit();
                    }
                    TempData["error"] = "false";
                    TempData["message"] = "Invoice has been approved successfully";
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

        public async Task<IActionResult> ApproveAuto(int id,int _companyId,string _userId)
        {
            //int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            //string _userId = HttpContext.Session.GetString("UserId");

            //BkgVehicleIGP bkgIGP = _dbContext.BkgVehicleIGPs.Find(id);
            ARInvoice invoice = _dbContext.ARInvoices
             .Include(c => c.Customer)
             .Where(a => a.Status == "Created" && a.CompanyId == _companyId && a.Id == id && a.IsDeleted == false)
             .FirstOrDefault();
            try
            {
                //Create Voucher
                Numbers.Helpers.VoucherHelper voucher = new Numbers.Helpers.VoucherHelper(_dbContext);
                GLVoucher voucherMaster = new GLVoucher();
                List<GLVoucherDetail> voucherDetails = new List<GLVoucherDetail>();
                string voucherDescription = string.Format(
                "Invoice # : {0} of  " +
                "{1} {2}",
                invoice.InvoiceNo,
                invoice.Customer.Name, invoice.Remarks);

                int voucherId;
                voucherMaster.VoucherType = "INV";
                voucherMaster.VoucherDate = invoice.InvoiceDate;
                voucherMaster.Reference = invoice.ReferenceNo;
                voucherMaster.Currency = invoice.Currency;
                voucherMaster.CurrencyExchangeRate = invoice.CurrencyExchangeRate;
                voucherMaster.Description = voucherDescription.Substring(0, voucherDescription.Length);
                voucherMaster.Status = "Approved";
                voucherMaster.ApprovedBy = _userId;
                voucherMaster.ApprovedDate = DateTime.Now;
                voucherMaster.IsSystem = true;
                voucherMaster.ModuleName = "AR/Invoice";
                voucherMaster.ModuleId = id;
                voucherMaster.Amount = invoice.GrandTotal;
                voucherMaster.ReferenceId = invoice.CustomerId;
                //Voucher Details
                var invoiceItems = _dbContext.ARInvoiceItems.Where(i => i.InvoiceId == invoice.Id && !i.IsDeleted).ToList();
                var amount = invoiceItems.Sum(s => s.LineTotal);
                var discount = invoiceItems.Sum(s => s.DiscountAmount);
                //Debit Entry
                GLVoucherDetail voucherDetail = new GLVoucherDetail();

                voucherDetail.AccountId = invoice.Customer.AccountId;
                voucherDetail.Sequence = 10;
                voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                voucherDetail.Debit = amount;
                voucherDetail.Credit = 0;
                voucherDetail.IsDeleted = false;
                voucherDetail.CreatedBy = _userId;
                voucherDetail.CreatedDate = DateTime.Now;
                voucherDetails.Add(voucherDetail);
                //for discount
                //if (discount > 0)
                //{
                //    var accountCode = _dbContext.AppCompanyConfigs.Where(c => c.CompanyId == _companyId && c.ConfigName == "Discount" && c.IsActive).FirstOrDefault().UserValue1;
                //    var discountAccount = _dbContext.GLAccounts.Where(a => a.Code == accountCode && a.CompanyId == _companyId && a.IsActive).FirstOrDefault().Id;
                //    voucherDetail = new GLVoucherDetail();
                //    voucherDetail.AccountId = discountAccount;
                //    voucherDetail.Sequence = 10;
                //    voucherDetail.Description = invoice.Remarks;
                //    voucherDetail.Debit = discount;
                //    voucherDetail.Credit = 0;
                //    voucherDetail.IsDeleted = false;
                //    voucherDetail.CreatedBy = _userId;
                //    voucherDetail.CreatedDate = DateTime.Now;
                //    voucherDetails.Add(voucherDetail);
                //}
                #region Sale Account
                //Credit Entry

                var itemAccounts = (from li in _dbContext.ARInvoiceItems
                                    join i in _dbContext.InvItems.Include(a => a.InvItemAccount) on li.ItemId equals i.Id
                                    where li.InvoiceId == id && li.IsDeleted == false
                                    select new
                                    {
                                        li.Total,
                                        li.DiscountAmount,
                                        i.InvItemAccount.GLSaleAccountId
                                    }).GroupBy(l => l.GLSaleAccountId)
                                    .Select(li => new ARInvoiceItem
                                    {
                                        Total = li.Sum(c => c.Total) - li.Sum(d => d.DiscountAmount),
                                        InvoiceId = li.FirstOrDefault().GLSaleAccountId //invoice id is temporarily containing GLSaleAccountId
                                    }).ToList();

                foreach (var item in itemAccounts)
                {
                    if (item.Total > 0)
                    {
                        voucherDetail = new GLVoucherDetail();
                        voucherDetail.AccountId = item.InvoiceId;
                        voucherDetail.Sequence = 20;
                        voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                        voucherDetail.Debit = 0;
                        voucherDetail.Credit = item.Total;
                        voucherDetail.IsDeleted = false;
                        voucherDetail.CreatedBy = _userId;
                        voucherDetail.CreatedDate = DateTime.Now;
                        voucherDetails.Add(voucherDetail);
                    }
                }
                #endregion Sale Account
                #region Sales Tax 
                var saleTaxAccounts = (from li in _dbContext.ARInvoiceItems
                                       join i in _dbContext.InvItems on li.ItemId equals i.Id
                                       join t in _dbContext.AppTaxes on li.TaxSlab equals t.Id
                                       where li.InvoiceId == id
                                       select new
                                       {
                                           li.SalesTaxAmount,
                                           t.SalesTaxAccountId
                                       }).GroupBy(l => l.SalesTaxAccountId)
                                        .Select(li => new ARInvoiceItem
                                        {
                                            SalesTaxAmount = li.Sum(c => c.SalesTaxAmount),
                                            InvoiceId = li.FirstOrDefault().SalesTaxAccountId //invoice id is temporarily containing SalesTaxAccountId
                                        }).ToList();
                foreach (var item in saleTaxAccounts)
                {
                    if (item.SalesTaxAmount > 0)
                    {
                        voucherDetail = new GLVoucherDetail();
                        voucherDetail.AccountId = item.InvoiceId;
                        voucherDetail.Sequence = 20;
                        voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                        voucherDetail.Debit = 0;
                        voucherDetail.Credit = item.SalesTaxAmount;
                        voucherDetail.IsDeleted = false;
                        voucherDetail.CreatedBy = _userId;
                        voucherDetail.CreatedDate = DateTime.Now;
                        voucherDetails.Add(voucherDetail);
                    }
                }
                #endregion Sales Tax
                #region Excise Tax 
                var exciseTaxAccounts = (from li in _dbContext.ARInvoiceItems
                                         join i in _dbContext.InvItems on li.ItemId equals i.Id
                                         join t in _dbContext.AppTaxes on li.TaxSlab equals t.Id
                                         where li.InvoiceId == id
                                         select new
                                         {
                                             li.ExciseTaxAmount,
                                             t.ExciseTaxAccountId
                                         }).GroupBy(l => l.ExciseTaxAccountId)
                                        .Select(li => new ARInvoiceItem
                                        {
                                            ExciseTaxAmount = li.Sum(c => c.ExciseTaxAmount),
                                            InvoiceId = li.FirstOrDefault().ExciseTaxAccountId //invoice id is temporarily containing SalesTaxAccountId
                                        }).ToList();
                foreach (var item in exciseTaxAccounts)
                {
                    if (item.ExciseTaxAmount > 0)
                    {
                        voucherDetail = new GLVoucherDetail();
                        voucherDetail.AccountId = item.InvoiceId;
                        voucherDetail.Sequence = 20;
                        voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                        voucherDetail.Debit = 0;
                        voucherDetail.Credit = item.ExciseTaxAmount;
                        voucherDetail.IsDeleted = false;
                        voucherDetail.CreatedBy = _userId;
                        voucherDetail.CreatedDate = DateTime.Now;
                        voucherDetails.Add(voucherDetail);
                    }
                }
                #endregion Excise Tax
                #region Cost of Sale Account
                //Debit Entry
                var costOfSaleAccount = (from li in _dbContext.ARInvoiceItems
                                         join i in _dbContext.InvItems.Include(a => a.InvItemAccount) on li.ItemId equals i.Id
                                         where li.InvoiceId == id && li.IsDeleted == false
                                         select new
                                         {
                                             li.CostofSales,
                                             i.InvItemAccount.GLCostofSaleAccountId
                                         }).GroupBy(l => l.GLCostofSaleAccountId)
                                    .Select(li => new ARInvoiceItem
                                    {
                                        CostofSales = li.Sum(c => c.CostofSales),
                                        InvoiceId = li.FirstOrDefault().GLCostofSaleAccountId //invoice id is temporarily containing GLCostofSaleAccountId
                                    }).ToList();

                foreach (var item in costOfSaleAccount)
                {
                    if (item.CostofSales > 0)
                    {
                        voucherDetail = new GLVoucherDetail();
                        voucherDetail.AccountId = item.InvoiceId;
                        voucherDetail.Sequence = 20;
                        voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                        voucherDetail.Debit = item.CostofSales;
                        voucherDetail.Credit = 0;
                        voucherDetail.IsDeleted = false;
                        voucherDetail.CreatedBy = _userId;
                        voucherDetail.CreatedDate = DateTime.Now;
                        voucherDetails.Add(voucherDetail);
                    }
                }
                #endregion Cost of Sale Account
                #region Asset/Stock Account
                //Credit Entry
                var assetAcount = (from li in _dbContext.ARInvoiceItems
                                   join i in _dbContext.InvItems.Include(a => a.InvItemAccount) on li.ItemId equals i.Id
                                   where li.InvoiceId == id && li.IsDeleted == false
                                   select new
                                   {
                                       li.CostofSales,
                                       i.InvItemAccount.GLAssetAccountId
                                   }).GroupBy(l => l.GLAssetAccountId)
                                    .Select(li => new ARInvoiceItem
                                    {
                                        CostofSales = li.Sum(c => c.CostofSales),
                                        InvoiceId = li.FirstOrDefault().GLAssetAccountId //invoice id is temporarily containing GLAssetAccountId
                                    }).ToList();

                foreach (var item in assetAcount)
                {
                    if (item.CostofSales > 0)
                    {
                        voucherDetail = new GLVoucherDetail();
                        voucherDetail.AccountId = item.InvoiceId;
                        voucherDetail.Sequence = 20;
                        voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                        voucherDetail.Debit = 0;
                        voucherDetail.Credit = item.CostofSales;
                        voucherDetail.IsDeleted = false;
                        voucherDetail.CreatedBy = _userId;
                        voucherDetail.CreatedDate = DateTime.Now;
                        voucherDetails.Add(voucherDetail);
                    }
                }
                #endregion Asset/Stock Account
                //Create Voucher 
                voucherId = voucher.CreateVoucher(voucherMaster, voucherDetails, _companyId, _userId);
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
                    using (var transaction = _dbContext.Database.BeginTransaction())
                    {
                        await _dbContext.SaveChangesAsync();
                        foreach (var invoiceItem in invoiceItems)
                        {
                            var item = _dbContext.InvItems.Find(invoiceItem.ItemId);
                            item.StockQty = item.StockQty - invoiceItem.Qty;
                            item.StockValue = item.StockValue - (item.AvgRate * invoiceItem.Qty);
                            var dbEntry = _dbContext.InvItems.Update(item);
                            dbEntry.OriginalValues.SetValues(await dbEntry.GetDatabaseValuesAsync());
                            await _dbContext.SaveChangesAsync();
                        }
                        await _dbContext.SaveChangesAsync();
                        var lineItems = _dbContext.ARInvoiceItems.Where(i => i.IsDeleted == false && i.InvoiceId == id).ToList();
                        foreach (var item in lineItems)
                        {
                            if (item.SalesOrderItemId != 0)
                            {
                                var saleOrderItem = _dbContext.ARSaleOrderItems.Find(item.SalesOrderItemId);
                                saleOrderItem.SaleQty = saleOrderItem.SaleQty + item.Qty;
                                var dbEntry = _dbContext.ARSaleOrderItems.Update(saleOrderItem);
                                dbEntry.OriginalValues.SetValues(await dbEntry.GetDatabaseValuesAsync());
                                await _dbContext.SaveChangesAsync();
                            }
                        }
                        transaction.Commit();
                    }
                    //TempData["error"] = "false";
                    //TempData["message"] = "Invoice has been approved successfully";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    //TempData["error"] = "true";
                    //TempData["message"] = "Voucher Not Created";
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
            var model = new ARInvoiceViewModel();
            model.TaxList = appTaxRepo.GetTaxes(companyId);
            model.CostCenterList = _dbContext.CostCenter.Where(t => t.CompanyId == companyId && t.IsDeleted == false).ToList();
            return  PartialView("_partialInvoiceItems", model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var invoice = _dbContext.ARInvoices.Find(id);
            invoice.IsDeleted = true;
            var entry = _dbContext.ARInvoices.Update(invoice);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            TempData["error"] = "false";
            TempData["message"] = string.Format("Invoice No {0} has been deleted", invoice.InvoiceNo);
            return RedirectToAction(nameof(Index));
        }
        public IActionResult UnApprove()
        {
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            string userId = HttpContext.Session.GetString("UserId");
            ViewBag.UnApprove = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == resp_Id).FirstOrDefault().UnApprove;

            ViewBag.NavbarHeading = "Un-Approve Sale Invoice";
            return View(_dbContext.ARInvoices
               .Where(v => v.Status == "Approved" && v.IsDeleted == false && v.TransactionType == "Sale" && v.CompanyId == HttpContext.Session.GetInt32("CompanyId")).ToList());
        }
        public async Task<IActionResult> UnApproveVoucher(int id)
        {
            try
            {
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                var voucher = _dbContext.ARInvoices
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
                        var invoiceItems = _dbContext.ARInvoiceItems.Where(i => i.InvoiceId == id && !i.IsDeleted).ToList();
                        foreach (var invoiceItem in invoiceItems)
                        {
                            var item = _dbContext.InvItems.Find(invoiceItem.ItemId);
                            item.StockQty = item.StockQty + invoiceItem.Qty;
                            item.StockValue = item.StockValue + (item.AvgRate * invoiceItem.Qty);
                            var dbEntry = _dbContext.InvItems.Update(item);
                            dbEntry.OriginalValues.SetValues(await dbEntry.GetDatabaseValuesAsync());
                            await _dbContext.SaveChangesAsync();
                        }
                        foreach (var item in invoiceItems)
                        {
                            if (item.SalesOrderItemId != 0)
                            {
                                var saleOrderItem = _dbContext.ARSaleOrderItems.Find(item.SalesOrderItemId);
                                saleOrderItem.SaleQty = saleOrderItem.SaleQty - item.Qty;
                                var dbEntry = _dbContext.ARSaleOrderItems.Update(saleOrderItem);
                                dbEntry.OriginalValues.SetValues(await dbEntry.GetDatabaseValuesAsync());
                                await _dbContext.SaveChangesAsync();
                            }
                        }

                        voucher.Status = "Created";
                        voucher.ApprovedBy = null;
                        voucher.ApprovedDate = null;
                        var entry = _dbContext.ARInvoices.Update(voucher);
                        entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                        await _dbContext.SaveChangesAsync();
                        TempData["error"] = "false";
                        TempData["message"] = "Invoice has been Un-Approved successfully";
                        transaction.Commit();
                    }
                }
            }
            catch (Exception exc)
            {
                TempData["error"] = "true";
                TempData["message"] = exc.Message==null?exc.InnerException.Message.ToString(): exc.Message.ToString();
            }
            return RedirectToAction(nameof(UnApprove));
        }
        [HttpPost]
        public IActionResult GetDCByCustomerId(int id, int[] skipIds)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var configValues = new ConfigValues(_dbContext);
            var invoice = GetAllSaleOrdersByCustomerId(id, skipIds, companyId);
            var DCs = GetAllDCByCustomerId(id, skipIds, companyId);
            var DcNo = 0;
            List<ARInvoiceViewModel> list = new List<ARInvoiceViewModel>();
            foreach (var item in invoice)
            {
                foreach(var data in DCs)
                {
                    var model = new ARInvoiceViewModel();
                    var Dc = DCs.Where(c => c.ItemId == item.ItemId && c.Id==data.Id&&c.SaleOrderId==item.Id).FirstOrDefault();
                    if (Dc!=null)
                    {
                        DcNo = _dbContext.ARDeliveryChallans.Where(x =>  x.Id == Dc.DeliveryChallanId).FirstOrDefault().DCNo;
                        model.SalesOrderItemId = item.Id;
                        model.Meters = item.Meters;
                        model.TotalMeter = item.TotalMeter;
                        model.TotalMeterAmount = item.TotalMeterAmount;
                        model.InvoiceNo = item.SaleOrder.SaleOrderNo;
                        model.DCNo = Convert.ToString(DcNo);
                        model.DCItemId = Dc.Id;
                        model.InvoiceDate = item.SaleOrder.SaleOrderDate;
                        model.ItemId = item.ItemId;
                        model.Item = item.Item;
                        model.Remarks = configValues.GetUom(item.Item.Unit);
                        model.Qty = Dc.Qty - Dc.SaleQty;
                        model.Rate = item.Rate;
                        model.BaleNumber = item.BaleNumber;
                        model.GrandTotal = Math.Round(model.Rate * model.Qty, 2);
                        model.TotalDiscountAmount = item.SaleQty;
                        list.Add(model);
                    }
                    //model.PurchaseInvoiceId = item.Purchase.Id;
                    /*model.SalesOrderItemId = item.Id;
                    model.InvoiceNo = item.SaleOrder.SaleOrderNo;
                    model.DCNo =Convert.ToString( DcNo);
                    model.DCItemId = Dc.Id;
                    model.InvoiceDate = item.SaleOrder.SaleOrderDate;
                    model.ItemId = item.ItemId;
                    model.Item = item.Item;
                    model.Remarks = configValues.GetUom(item.Item.Unit);
                    if (Dc != null)
                    {
                        model.Qty = Dc.Qty -Dc.SaleQty;
                    }*/
                    /*else
                    {
                        model.Qty = item.Qty;
                    }*/
                    
                }
            }
            var DCNo = _dbContext.ARInvoiceItems.Select(x => x.DCNo).ToList();


            var Records = list.Where(x=> !DCNo.Contains(x.DCNo)).GroupBy(x => new { x.Item.Code, x.Meters })
                 .Select(item => new ARInvoiceViewModel
                 {
                     SalesOrderItemId = item.Select(x => x.SalesOrderItemId).FirstOrDefault(),
                     Meters = item.Select(x => x.Meters).FirstOrDefault(),
                     InvoiceNo = item.Select(x => x.InvoiceNo).FirstOrDefault(),
                     DCNo = item.Select(x => x.DCNo).FirstOrDefault(),
                     DCItemId = item.Select(x => x.DCItemId).FirstOrDefault(),
                     InvoiceDate = item.Select(x => x.InvoiceDate).FirstOrDefault(),
                     ItemId = item.Select(x => x.ItemId).FirstOrDefault(),
                     Item = item.Select(x => x.Item).FirstOrDefault(),
                     Remarks = item.Select(x => x.Remarks).FirstOrDefault(),
                     Qty = item.Select(x => x.Qty).Sum(),
                     Rate = item.Select(x => x.Rate).FirstOrDefault(),
                     BaleNumber = item.Select(x => x.BaleNumber).FirstOrDefault(),
                     GrandTotal = item.Select(x => x.GrandTotal).FirstOrDefault(),
                     TotalMeter = item.Select(x => x.Qty).Sum() * item.Select(x => x.Meters).FirstOrDefault(),
                     TotalMeterAmount = item.Select(x => x.Qty).Sum() * item.Select(x => x.Meters).FirstOrDefault() * item.Select(x => x.Rate).FirstOrDefault(),
                     TotalDiscountAmount = item.Select(x => x.TotalDiscountAmount).FirstOrDefault()
                 }).OrderByDescending(x => x.InvoiceNo);
            return PartialView("_SaleOrderPopUp", Records.ToList().Distinct());
        }
        public List<ARDeliveryChallanItem> GetAllDCByCustomerId(int id, int[] skipIds, int companyId)
        {

            var DCs = _dbContext.ARDeliveryChallans.Where(x => x.CustomerId == id && x.IsDeleted == false && x.CompanyId == companyId && x.Status== "Approved").Select(c=>c.Id).ToList();
            var DCItems = _dbContext.ARDeliveryChallanItems
                .Include(x=>x.Item).Where(i => DCs.Contains(i.DeliveryChallanId) && i.IsDeleted==false && i.Qty > i.SaleQty)
                .Where(i => !skipIds.Contains(i.Item.Id)).ToList();
            return DCItems;
        }
        [HttpPost]
        public IActionResult GetSaleOrdersByCustomerId(int id, int[] skipIds)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var configValues = new ConfigValues(_dbContext);
            var invoice = GetAllSaleOrdersByCustomerId(id, skipIds, companyId);
            List<ARInvoiceViewModel> list = new List<ARInvoiceViewModel>();
            var DCs = GetAllDCByCustomerId(id, skipIds, companyId);
            foreach (var item in invoice)
            {
                var model = new ARInvoiceViewModel();
                var Dc = DCs.Where(c => c.ItemId == item.ItemId).FirstOrDefault();
                //model.PurchaseInvoiceId = item.Purchase.Id;
                model.SalesOrderItemId = item.Id;
                model.InvoiceNo = item.SaleOrder.SaleOrderNo;
                model.InvoiceDate = item.SaleOrder.SaleOrderDate;
                model.ItemId = item.ItemId;
                model.Item = item.Item;
                model.Remarks = configValues.GetUom(item.Item.Unit);
                model.Qty = Dc.Qty - model.SaleQty;
                model.Rate = item.Rate;
                model.GrandTotal = Math.Round(model.Rate * model.Qty, 2);
                model.TotalDiscountAmount = item.SaleQty;
                list.Add(model);
            }
            return PartialView("_SaleOrderPopUp", list.ToList());
        }

        [HttpPost]
        public List<ARInvoiceViewModel> GetSaleOrdersByCustomerIdForAutoInv(int id,int DCNo,int companyId)
        {
            //int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var configValues = new ConfigValues(_dbContext);
            var invoice = GetAllSaleOrdersByCustomerIdForAutoInv(id, companyId);
            List<ARInvoiceViewModel> list = new List<ARInvoiceViewModel>();
            var DCs = GetAllSaleOrdersByCustomerIdForAutoInv(id, companyId);
            foreach (var item in invoice)
            {
                var model = new ARInvoiceViewModel();
                var Dc = DCs.Where(c => c.ItemId == item.ItemId).FirstOrDefault();
                //model.PurchaseInvoiceId = item.Purchase.Id;
                model.SalesOrderItemId = item.Id;
                model.InvoiceNo = item.SaleOrder.SaleOrderNo;
                model.InvoiceDate = item.SaleOrder.SaleOrderDate;
                model.ItemId = item.ItemId;
                model.Item = item.Item;
                model.Remarks = configValues.GetUom(item.Item.Unit);
                model.Qty = Dc.Qty - model.SaleQty;
                model.Rate = item.Rate;
                model.GrandTotal = Math.Round(model.Rate * model.Qty, 2);
                model.TotalDiscountAmount = item.SaleQty;
                list.Add(model);
            }
            return list.ToList();
        }
        public List<ARSaleOrderItem> GetAllSaleOrdersByCustomerId(int id, int[] skipIds, int companyId)
        {
            var purchaseItems = _dbContext.ARSaleOrderItems.Include(i => i.SaleOrder).Include(i => i.Item)
                .Where(i => !skipIds.Contains(i.Item.Id) && i.SaleOrder.CompanyId == companyId && i.IsDeleted == false && i.SaleOrder.CustomerId == id&&i.SaleOrder.Status== "Approved"
                // )
                //     && i.Qty > i.SaleQty)
                //.Where(i => !skipIds.Contains(i.Id)
                ).ToList();
            return purchaseItems;
        }

        public List<ARSaleOrderItem> GetAllSaleOrdersByCustomerIdForAutoInv(int id, int companyId)
        {
            var purchaseItems = _dbContext.ARSaleOrderItems.Include(i => i.SaleOrder).Include(i => i.Item)
                .Where(i => i.SaleOrder.CompanyId == companyId && i.IsDeleted == false && i.SaleOrder.CustomerId == id && i.SaleOrder.Status == "Approved"
                // )
                //     && i.Qty > i.SaleQty)
                //.Where(i => !skipIds.Contains(i.Id)
                ).ToList();
            return purchaseItems;
        }
        [HttpPost]
        public IActionResult GetSaleOrderItems(int saleOrderItemId, int counter)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var appTaxRepo = new AppTaxRepo(_dbContext);
            ARInvoiceViewModel item = new ARInvoiceViewModel();
            // var saleorder = _dbContext.ARSaleOrderItems.Where(x => x.Id == saleOrderItemId).Select(c => c.SaleOrderId).FirstOrDefault();

            //var item = GetSaleOrderItem(saleOrderItemId);
            //var dc = _dbContext.ARDeliveryChallanItems.Where(x => x.Id == saleOrderItemId && x.ItemId == item.ItemId).FirstOrDefault();

            var dcList = _dbContext.ARDeliveryChallanItems.Include(x=>x.ARSaleOrderItem).Include(x=>x.Item).Include(x=>x.Bale).Where(x => x.SaleOrderId == saleOrderItemId).ToList();
            var Records = dcList.GroupBy(x => new { x.Item.Code, x.Bale.Meters })
                 .Select(itm => new ARInvoiceViewModel
                 {
                     DeliveryChallanId = itm.Select(x=>x.DeliveryChallanId).FirstOrDefault(),
                     DCItemId = itm.Select(x=>x.Id).FirstOrDefault(),
                     SalesOrderItemId = itm.Select(x=>x.ARSaleOrderItem.Id).FirstOrDefault(),
                     Qty = itm.Select(x => x.Qty).Sum(),
                     Rate = itm.Select(x => x.ARSaleOrderItem.Rate).FirstOrDefault(),
                     Meters = itm.Select(x => x.Bale.Meters).FirstOrDefault(),
                     ItemId = itm.Select(x => x.ItemId).FirstOrDefault(),
                     TotalMeter = itm.Select(x => x.Qty).Sum() * itm.Select(x => x.Bale.Meters).FirstOrDefault(),
                     TotalMeterAmount = itm.Select(x => x.Qty).Sum() * itm.Select(x => x.Bale.Meters).FirstOrDefault() * itm.Select(x => x.ARSaleOrderItem.Rate).FirstOrDefault(),
                 });
            
            item.DCNo =Convert.ToString(_dbContext.ARDeliveryChallans.Where(x => x.Id == Records.Select(x => x.DeliveryChallanId).FirstOrDefault()).Select(d => d.DCNo).FirstOrDefault());
            item.DCItemId = Records.Select(x => x.DCItemId).FirstOrDefault();
            item.SalesOrderItemId = Records.Select(x => x.SalesOrderItemId).FirstOrDefault();
            //item.Qty = dc.Qty-dc.SaleQty;
            item.Qty = Records.Select(x=>x.Qty).FirstOrDefault();
            item.Rate = Records.Select(x=>x.Rate).FirstOrDefault();
            item.Meters = Records.Select(x=>x.Meters).FirstOrDefault();
            item.TotalMeter = Records.Select(x=>x.TotalMeter).FirstOrDefault();
            item.TotalMeterAmount = Records.Select(x=>x.TotalMeterAmount).FirstOrDefault();
            item.ItemId = Records.Select(x=>x.ItemId).FirstOrDefault();
            item.Total_ = Convert.ToDecimal(item.TotalMeterAmount);
            item.Total = Convert.ToDecimal(item.TotalMeterAmount);
            ViewBag.Counter = counter;
            item.LineTotal = item.Total;
            var percentage = item.SalesTaxPercentage;
            item.SalesTaxAmount= ((percentage/100)* item.Total_);
            //item.TaxSlab = item.;
            ViewBag.ItemId = item.ItemId;
            item.TaxList = appTaxRepo.GetTaxes(companyId);
            item.CostCenterList = _dbContext.CostCenter.Where(t => t.CompanyId == companyId && t.IsDeleted == false).ToList();
            return PartialView("_partialInvoiceItems", item);
        }

        [HttpPost]
        public ARInvoiceViewModel GetSaleOrderItemsForAutoInv(int saleOrderItemId,int deliveryChallanId,int companyId)
        {
            //int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var appTaxRepo = new AppTaxRepo(_dbContext);
            ARInvoiceViewModel item = new ARInvoiceViewModel();
            // var saleorder = _dbContext.ARSaleOrderItems.Where(x => x.Id == saleOrderItemId).Select(c => c.SaleOrderId).FirstOrDefault();

            //var item = GetSaleOrderItem(saleOrderItemId);
            //var dc = _dbContext.ARDeliveryChallanItems.Where(x => x.Id == saleOrderItemId && x.ItemId == item.ItemId).FirstOrDefault();

            var dcList = _dbContext.ARDeliveryChallanItems.Where(x=>x.DeliveryChallanId==deliveryChallanId).Include(x => x.ARSaleOrderItem).Include(x => x.Item).Include(x => x.Bale).Where(x => x.SaleOrderId == saleOrderItemId).ToList();
            var Records = dcList.GroupBy(x => new { x.Item.Code, x.Bale.Meters })
                 .Select(itm => new ARInvoiceViewModel
                 {
                     DeliveryChallanId = itm.Select(x => x.DeliveryChallanId).FirstOrDefault(),
                     DCItemId = itm.Select(x => x.Id).FirstOrDefault(),
                     SalesOrderItemId = itm.Select(x => x.ARSaleOrderItem.Id).FirstOrDefault(),
                     Qty = itm.Select(x => x.Qty).Sum(),
                     Rate = itm.Select(x => x.ARSaleOrderItem.Rate).FirstOrDefault(),
                     Meters = itm.Select(x => x.Bale.Meters).FirstOrDefault(),
                     ItemId = itm.Select(x => x.ItemId).FirstOrDefault(),
                     TotalMeter = itm.Select(x => x.Qty).Sum() * itm.Select(x => x.Bale.Meters).FirstOrDefault(),
                     TotalMeterAmount = itm.Select(x => x.Qty).Sum() * itm.Select(x => x.Bale.Meters).FirstOrDefault() * itm.Select(x => x.ARSaleOrderItem.Rate).FirstOrDefault(),
                 });

            item.DCNo = Convert.ToString(_dbContext.ARDeliveryChallans.Where(x => x.Id == Records.Select(x => x.DeliveryChallanId).FirstOrDefault()).Select(d => d.DCNo).FirstOrDefault());
            item.DCItemId = Records.Select(x => x.DCItemId).FirstOrDefault();
            item.SalesOrderItemId = Records.Select(x => x.SalesOrderItemId).FirstOrDefault();
            //item.Qty = dc.Qty-dc.SaleQty;
            item.Qty = Records.Select(x => x.Qty).FirstOrDefault();
            item.Rate = Records.Select(x => x.Rate).FirstOrDefault();
            item.Meters = Records.Select(x => x.Meters).FirstOrDefault();
            item.TotalMeter = Records.Select(x => x.TotalMeter).FirstOrDefault();
            item.TotalMeterAmount = Records.Select(x => x.TotalMeterAmount).FirstOrDefault();
            item.ItemId = Records.Select(x => x.ItemId).FirstOrDefault();
            item.Total_ = Convert.ToDecimal(item.TotalMeterAmount);
            item.Total = Convert.ToDecimal(item.TotalMeterAmount);
            item.LineTotal = item.Total;
            var percentage = item.SalesTaxPercentage;
            item.SalesTaxAmount = ((percentage / 100) * item.Total_);
            //item.TaxSlab = item.;
            ViewBag.ItemId = item.ItemId;
            item.TaxList = appTaxRepo.GetTaxes(companyId);
            item.CostCenterList = _dbContext.CostCenter.Where(t => t.CompanyId == companyId && t.IsDeleted == false).ToList();
            return  item;
        }

        public ARInvoiceViewModel GetSaleOrderItem(int id)
        {
            var deliveryitems = _dbContext.ARDeliveryChallanItems.Where(x => x.Id == id).FirstOrDefault();
            var item = _dbContext.ARSaleOrderItems
                       .Include(i => i.SaleOrder)
                       .Include(i => i.Item)
                       .Where(i => i.Id == deliveryitems.SaleOrderId && i.IsDeleted == false&&i.ItemId==deliveryitems.ItemId)
                       .FirstOrDefault();
            ARInvoiceViewModel viewModel = new ARInvoiceViewModel();
            viewModel.DCItemId = 0;
            viewModel.SalesOrderId = item.SaleOrderId;
            viewModel.SalesOrderItemId = item.Id;
            viewModel.ItemId = item.ItemId;
            viewModel.Qty = item.Qty - item.SaleQty;
            viewModel.IssueRate = item.Item.AvgRate;
            viewModel.CostofSales = item.Item.AvgRate * (item.Qty - item.SaleQty);
            viewModel.Rate = item.Rate;
            viewModel.Total_ = Math.Round(viewModel.Rate * viewModel.Qty, 2);
            viewModel.LineTotal = item.LineTotal;
            viewModel.SalesTaxPercentage= _dbContext.AppTaxes.Where(x => x.Id == item.TaxId).Select(x => x.SalesTaxPercentage).FirstOrDefault();
            viewModel.TaxSlab = item.TaxId;
            viewModel.SalesTaxAmount = item.TaxAmount;
            viewModel.DetailCostCenter = viewModel.DetailCostCenter;
            return viewModel;
        }
        public ARInvoiceViewModel GetSaleOrderItem2(int id)
        {
            var item = _dbContext.ARSaleOrderItems
                       .Include(i => i.SaleOrder)
                       .Include(i => i.Item)
                       .Where(i => i.Id == id && i.IsDeleted == false)
                       .FirstOrDefault();
            ARInvoiceViewModel viewModel = new ARInvoiceViewModel();
            viewModel.DCItemId = 0;
            viewModel.SalesOrderId = item.SaleOrderId;
            viewModel.SalesOrderItemId = item.Id;
            viewModel.ItemId = item.ItemId;
            viewModel.Qty = item.Qty - item.SaleQty;
            viewModel.IssueRate = item.Item.AvgRate;
            viewModel.CostofSales = item.Item.AvgRate * (item.Qty - item.SaleQty);
            viewModel.Rate = item.Rate;
            viewModel.Total_ = Math.Round(viewModel.Rate * viewModel.Qty, 2);
            viewModel.LineTotal = item.LineTotal;
            viewModel.TaxSlab = item.TaxId;
            viewModel.SalesTaxAmount = item.TaxAmount;
            viewModel.DetailCostCenter = viewModel.DetailCostCenter;
            return viewModel;
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string configValues = _dbContext.AppCompanyConfigs
                                .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                                .Select(c => c.ConfigValue)
                                .FirstOrDefault();
            ViewBag.ReportPath = string.Concat(configValues, "Viewer", "?Report=Voucher&cId=", companyId, "&id={0}");
            ViewBag.ReportPath2 = string.Concat(configValues, "Viewer", "?Report=SalesInvoice&cId=", companyId, "&id={0}");
            var viewModel = new ARInvoiceViewModel ();
            //var invoice= _dbContext.ARInvoices.Include(i => i.Customer).Include(i => i.WareHouse)
            //.Where(i=>i.Id==id).FirstOrDefault();
            var invoice= _dbContext.ARInvoices.Include(i => i.Customer)
            .Where(i=>i.Id==id).FirstOrDefault();
            //var invoice = _dbContext.ARInvoices.Find(id);
            var invoiceItem= _dbContext.ARInvoiceItems
                                .Include(i => i.Item)
                                .Include(i => i.Invoice)
                                .Where(i => i.InvoiceId == id && i.IsDeleted == false)
                                .ToList();
            ViewBag.NavbarHeading = "Sale Invoice";
            ViewBag.TitleStatus = "Approved";

            TempData["Detail"] = invoiceItem;
            return View(invoice);
        }
        public IActionResult GetSI()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            try
            {
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
                var searchInvNo = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchInvDate = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchCustomer = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchOGP = Request.Form["columns[4][search][value]"].FirstOrDefault();
                var searchSalesTax = Request.Form["columns[5][search][value]"].FirstOrDefault();
                var searchGrandTotal = Request.Form["columns[6][search][value]"].FirstOrDefault();
                var searchStatus = Request.Form["columns[7][search][value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var SIData = (from SI in _dbContext.ARInvoices.Where(x => x.IsDeleted == false && x.CompanyId == companyId) select SI);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    SIData = SIData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                SIData = !string.IsNullOrEmpty(searchInvNo) ? SIData.Where(m => m.InvoiceNo.ToString().Contains(searchInvNo)) : SIData;
                SIData = !string.IsNullOrEmpty(searchInvDate) ? SIData.Where(m => m.InvoiceDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchInvDate.ToUpper())) : SIData;
                SIData = !string.IsNullOrEmpty(searchCustomer) ? SIData.Where(m => _dbContext.ARCustomers.FirstOrDefault(x => x.Id == Convert.ToInt32(m.CustomerId)).Name.ToUpper().Contains(searchCustomer.ToUpper())) : SIData;
                SIData = !string.IsNullOrEmpty(searchOGP) ? SIData.Where(m => (m.OGPNo != null ? m.OGPNo.ToString().Contains(searchOGP) : m.InvoiceNo.ToString().Contains(searchInvNo))) : SIData;
                SIData = !string.IsNullOrEmpty(searchSalesTax) ? SIData.Where(m => m.SalesTaxAmount.ToString().Contains(searchSalesTax)) : SIData;
                SIData = !string.IsNullOrEmpty(searchGrandTotal) ? SIData.Where(m => m.GrandTotal.ToString().Contains(searchGrandTotal)) : SIData;
                SIData = !string.IsNullOrEmpty(searchStatus) ? SIData.Where(m => m.Status.ToString().ToUpper().Contains(searchStatus.ToUpper())) : SIData;

                recordsTotal = SIData.Count();
                var data = SIData.ToList();
                if (pageSize == -1)
                {
                    data = SIData.Skip(skip).Take(recordsTotal == 0 ? 1 : pageSize == -1 ? recordsTotal : 1).ToList();
                }
                else
                {
                    data = SIData.Skip(skip).Take(pageSize).ToList();
                }
                List<ARInvoiceViewModel> Details = new List<ARInvoiceViewModel>();
                foreach (var grp in data)
                {
                    ARInvoiceViewModel aRInvoiceViewModel = new ARInvoiceViewModel();
                    aRInvoiceViewModel.CustomerName = _dbContext.ARCustomers.FirstOrDefault(x => x.Id == grp.CustomerId).Name;
                    aRInvoiceViewModel.InvDate = grp.InvoiceDate.ToString(Helpers.CommonHelper.DateFormat);
                    aRInvoiceViewModel.Invoice = grp;
                    aRInvoiceViewModel.Invoice.Approve = approve;
                    aRInvoiceViewModel.Invoice.Unapprove = unApprove;
                    Details.Add(aRInvoiceViewModel);
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