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

namespace Numbers.Areas.AP.Controllers
{
    [Area("AP")]
    [Authorize]
    public class ServiceInvoiceController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public ServiceInvoiceController(NumbersDbContext context)
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
            ViewBag.ReportPath2 = string.Concat(configValues, "Viewer", "?Report=Pur&cId=", companyId, "&id={0}");
            ViewBag.NavbarHeading = "List of Purchase Service Invoices";
            return View(_dbContext.APPurchases.Where(c => c.CompanyId == companyId && c.TransactionType == "Service" &&  c.IsDeleted == false)
                                              .Include(s => s.Supplier)
                                              .ToList());
        }
        [HttpGet]
        public IActionResult Create(int id)
        {
            try
            {
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
                var configValues = new ConfigValues(_dbContext);
                ViewBag.Suppliers = configValues.Supplier(companyId);  
                AppConfigHelper helper = new AppConfigHelper(_dbContext, HttpContext);
                ViewBag.WareHouse = helper.GetWareHouses();
                var model = new APPurchaseItemViewModel();
                ViewBag.SubAccounts = new SelectList(_dbContext.GLSubAccountDetails.Where(x => !x.IsDelete).ToList(),"Id", "Description");
                ViewBag.Department = new SelectList(_dbContext.GLDivision.Where(x => x.CompanyId == companyId && x.IsDeleted == false).ToList(), "Id", "Name");
                ViewBag.CostCenter = new SelectList(_dbContext.CostCenter.Where(x => x.CompanyId == companyId && x.IsDeleted == false).ToList(), "Id", "Description");
                ViewBag.OperatingUnit = configValues.GetOrgValues(resp_Id, "Operating Unit", companyId);
                if (id == 0)
                {
                    ViewBag.Counter = 0;
                    ViewBag.EntityState = "Create";
                    ViewBag.NavbarHeading = "Create Purchase Service Invoice";
                    int maxPurchaseNo = 1;
                    var purchases = _dbContext.APPurchases.Where(c => c.CompanyId == companyId && c.TransactionType == "Service").ToList();
                    if (purchases.Count > 0)
                    {
                        maxPurchaseNo = purchases.Max(v => v.PurchaseNo);
                        TempData["PurchaseNo"] = maxPurchaseNo + 1;
                        ViewBag.PurchaseNo= maxPurchaseNo + 1;
                    }
                    else
                    {
                        TempData["PurchaseNo"] = maxPurchaseNo;
                    }
                    var appTaxRepo = new AppTaxRepo(_dbContext);
                    model.TaxList = appTaxRepo.GetTaxes(companyId);
                    model.Currencies = AppCurrencyRepo.GetCurrencies();
               //     return View(model);
                }
                else
                {
                    var purchase = _dbContext.APPurchases.Find(id);
                    TempData["PurchaseNo"] = purchase.PurchaseNo;
                    ViewBag.Id = id;
                    //ViewBag.EntityState = "Update";              
                    ViewBag.Counter = 0;
                    var list = _dbContext.APPurchaseItems.Include(i => i.Purchase).Where(i => i.PurchaseId == id && i.IsDeleted==false).ToArray();
                    ViewBag.ServiceInvoices = list;
                    model.Id = purchase.Id;
                    model.PurchaseNo = purchase.PurchaseNo;
                    model.PurchaseDate = purchase.PurchaseDate;
                    model.IGPNo = purchase.IGPNo;
                    model.SupplierInvoiceNo = purchase.SupplierInvoiceNo;
                    model.SupplierId = purchase.SupplierId;
                    model.ReferenceNo = purchase.ReferenceNo;
                    model.SupplierInvoiceDate = purchase.SupplierInvoiceDate;
                    model.Currency = purchase.Currency;
                    model.CurrencyExchangeRate = purchase.CurrencyExchangeRate;
                    model.SupplierId = purchase.SupplierId;
                    model.Remarks = purchase.Remarks;
                    model.Status = purchase.Status;
                    model.VoucherId = purchase.VoucherId;
                    //Page Lever total
                    model.DepartmentId = purchase.DepartmentId;
                    model.Total = purchase.Total;
                    model.TotalDiscountAmount = purchase.TotalDiscountAmount;
                    model.TotalSalesTaxAmount = purchase.TotalSalesTaxAmount;
                    model.GrandTotal = purchase.GrandTotal;
                    var appTaxRepo = new AppTaxRepo(_dbContext);
                    model.TaxList = appTaxRepo.GetTaxes(companyId);
                    model.Currencies = AppCurrencyRepo.GetCurrencies();
                    if (model.Status != "Approved")
                    {
                        ViewBag.EntityState = "Update";
                        ViewBag.NavbarHeading = "Purchase Service Invoice";
                        ViewBag.TitleStatus = "Created";
                    }
                }
                return View(model);
            }
            catch(Exception ex)
            {
                TempData["error"] = "true";
                TempData["message"] = ex.Message == null ? ex.InnerException.Message.ToString() : ex.Message.ToString();
                return RedirectToAction(nameof(Index));
            }
        }

        public int GetMax()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            int maxPurchaseNo = 1;
            var purchases = _dbContext.APPurchases.Where(c => c.CompanyId == companyId && c.TransactionType == "Service").ToList();
            if (purchases.Count > 0)
            {
                maxPurchaseNo = purchases.Max(v => v.PurchaseNo);
                maxPurchaseNo = maxPurchaseNo + 1;
                return maxPurchaseNo;
            }
            return maxPurchaseNo;
        }



        [HttpPost]
        public async Task<IActionResult> Create(APPurchase model, IFormCollection collection)
        {
            try
            {
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                string userId = HttpContext.Session.GetString("UserId");
                if (collection["ServiceAccountId"].Count > 0)
                {
                    model.CreatedBy = userId;
                    model.CompanyId = companyId;
                    model.CreatedDate = DateTime.Now;
                    model.TransactionType = "Service";
                    model.Status = "Created";
                    model.PurchaseNo = GetMax();
                    _dbContext.APPurchases.Add(model);
                    await _dbContext.SaveChangesAsync();

                    for (int i = 0; i < collection["ServiceAccountId"].Count; i++)
                    {
                        var invoiceItem = _dbContext.APPurchaseItems
                            .Where(j => j.PurchaseId == model.Id && j.Id == Convert.ToInt32(collection["PurchaseItemId"][i] == "" ? 0 : Convert.ToInt32(collection["PurchaseItemId"][i]))).FirstOrDefault();
                        // Extract coresponding values from form collection
                        var accountId = Convert.ToInt32(collection["ServiceAccountId"][i]);
                        var subAccountId = Convert.ToInt32(collection["SubAccountId"][i]);
                        var departId = Convert.ToInt32(collection["Department"][i]);
                        var subDepartId = Convert.ToInt32(collection["SubDepartment"][i]);
                        var costCenter = Convert.ToInt32(collection["CostCenter"][i]);
                        var rate = Convert.ToDecimal(collection["Total_"][i]);
                        var total = Convert.ToDecimal(collection["Total_"][i]);
                        var taxId = Convert.ToInt32(collection["TaxId"][i]);
                        var discountPercentage = Convert.ToDecimal(collection["DiscountPercentage"][i]);
                        var discountAmount = Convert.ToDecimal(collection["DiscountAmount"][i]);
                        var salesTaxPercentage = Convert.ToDecimal(collection["SalesTaxPercentage"][i]);
                        var salesTaxAmount = Convert.ToDecimal(collection["SalesTaxAmount"][i]);
                        var linetotal = Convert.ToDecimal(collection["LineTotal"][i]);
                        var remarks = collection["Remarks"][i + 1];

                        var newItem = new APPurchaseItem();
                        newItem.PurchaseId = model.Id;
                        newItem.ServiceAccountId = accountId;
                        newItem.Qty = 1;
                        newItem.Rate = rate;
                        newItem.Total = total;
                        newItem.TaxId = taxId;
                        newItem.DiscountPercentage = discountPercentage;
                        newItem.DiscountAmount = discountAmount;
                        newItem.SalesTaxPercentage = salesTaxPercentage;
                        newItem.SalesTaxAmount = salesTaxAmount;
                        newItem.LineTotal = linetotal;
                        newItem.Remarks = remarks;
                        newItem.SubAccountId = subAccountId;
                        newItem.DepartmentId = departId;
                        newItem.SubDepartmentId = subDepartId;
                        newItem.CostCneterId = costCenter;
                        _dbContext.APPurchaseItems.Add(newItem);

                        TempData["error"] = "false";
                        TempData["message"] = "Service Invoice "+ model.PurchaseNo +" has been created successfully";
                        await _dbContext.SaveChangesAsync();
                    }
                    return RedirectToAction("Index", "ServiceInvoice", new { area = "AP" });
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "No any Service Invoice has been created. It must contain atlest one item";
                    return RedirectToAction("Index", "ServiceInvoice", new { area = "AP" });
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = "true";
                TempData["message"] = ex.Message == null ? ex.InnerException.Message.ToString() : ex.Message.ToString();
                return RedirectToAction("Index", "ServiceInvoice", new { area = "AP" });
            }
        }
        [HttpPost]
        public async Task<IActionResult> Update(APPurchaseItemViewModel model, IFormCollection collection)
        {
            //deleting invoices
            string[] idsDeleted = Convert.ToString(collection["IdsDeleted"]).Split(",");
            if (!idsDeleted.Contains(""))
            {
                for (int j = 0; j < idsDeleted.Length; j++)
                {
                    if (idsDeleted[j] != "0")
                    {
                        var itemToRemove = _dbContext.APPurchaseItems.Find(Convert.ToInt32(idsDeleted[j]));
                        itemToRemove.IsDeleted = true;
                        _dbContext.APPurchaseItems.Update(itemToRemove);
                        await _dbContext.SaveChangesAsync();
                    }
                }
            }
            if (collection["ServiceAccountId"].Count > 0)
            {
                //updating existing data
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                string userId = HttpContext.Session.GetString("UserId");
                APPurchase purchase = _dbContext.APPurchases.Find(model.Id);
                purchase.PurchaseNo = model.PurchaseNo;
                purchase.PurchaseDate = model.PurchaseDate;
                purchase.IGPNo = model.IGPNo;
                purchase.SupplierInvoiceNo = model.SupplierInvoiceNo;
                purchase.ReferenceNo = model.ReferenceNo;
                purchase.SupplierId = model.SupplierId;
                purchase.SupplierInvoiceDate = model.SupplierInvoiceDate;
                purchase.Remarks = collection["Remarks"][0];
                purchase.Currency = model.Currency;
                purchase.CurrencyExchangeRate = model.CurrencyExchangeRate;
                purchase.Total = Convert.ToDecimal(collection["Total"]);
                purchase.TotalDiscountAmount = Convert.ToDecimal(collection["TotalDiscountAmount"]);
                purchase.TotalSalesTaxAmount = Convert.ToDecimal(collection["TotalSalesTaxAmount"]);
                purchase.GrandTotal = Convert.ToDecimal(collection["GrandTotal"]);
                purchase.UpdatedBy = userId;
                purchase.UpdatedDate = DateTime.Now;
                var entry = _dbContext.APPurchases.Update(purchase);
                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                await _dbContext.SaveChangesAsync();
                //Update Invoice detail items/ Invoice items if any
                for (int i = 0; i < collection["ServiceAccountId"].Count; i++)
                {
                    var purchaseItem = _dbContext.APPurchaseItems
                                .Where(j => j.PurchaseId == model.Id && j.Id == Convert.ToInt32(collection["PurchaseItemId"][i] == "" ? 0 : Convert.ToInt32(collection["PurchaseItemId"][i]))).FirstOrDefault();
                    // Extract coresponding values from form collection
                    var accountId = Convert.ToInt32(collection["ServiceAccountId"][i]);
                    var rate = Convert.ToDecimal(collection["Total_"][i]);
                    var total = Convert.ToDecimal(collection["Total_"][i]);
                    var taxId = Convert.ToInt32(collection["TaxId"][i]);
                    var discountPercentage = Convert.ToDecimal(collection["DiscountPercentage"][i]);
                    var discountAmount = Convert.ToDecimal(collection["DiscountAmount"][i]);
                    var salesTaxPercentage = Convert.ToDecimal(collection["SalesTaxPercentage"][i]);
                    var salesTaxAmount = Convert.ToDecimal(collection["SalesTaxAmount"][i]);
                    var linetotal = Convert.ToDecimal(collection["LineTotal"][i]);
                    var subAccountId = Convert.ToInt32(collection["SubAccountId"][i]);
                    var departId = Convert.ToInt32(collection["Department"][i]);
                    var subDepartId = Convert.ToInt32(collection["SubDepartment"][i]);
                    var costCenter = Convert.ToInt32(collection["CostCenter"][i]);
                    var remarks = collection["Remarks"][i + 1];
                    if (purchaseItem != null && accountId != 0)
                    {
                        //below phenomenon prevents Id from being marked as modified
                        var entityEntry = _dbContext.Entry(purchaseItem);
                        entityEntry.State = EntityState.Modified;
                        entityEntry.Property(p => p.Id).IsModified = false;
                        purchaseItem.ServiceAccountId = accountId;
                        purchaseItem.PurchaseId = model.Id;
                        purchaseItem.Qty = 1;
                        purchaseItem.Rate = rate;
                        purchaseItem.Total = total;
                        purchaseItem.TaxId = taxId;
                        purchaseItem.DiscountPercentage = discountPercentage;
                        purchaseItem.DiscountAmount = discountAmount;
                        purchaseItem.SalesTaxPercentage = salesTaxPercentage;
                        purchaseItem.SalesTaxAmount = salesTaxAmount;
                        purchaseItem.LineTotal = linetotal;
                        purchaseItem.Remarks = remarks;
                        purchaseItem.CostCneterId = costCenter;
                        purchaseItem.SubAccountId = subDepartId;
                        purchaseItem.DepartmentId = departId;
                        purchaseItem.SubDepartmentId = subDepartId;
                        var dbEntry = _dbContext.APPurchaseItems.Update(purchaseItem);
                        dbEntry.OriginalValues.SetValues(await entityEntry.GetDatabaseValuesAsync());
                    }
                    //check if user created new item while updating
                    else if (accountId != 0 && purchaseItem == null)//itemId is invitem, if this is null or zero rest of the information for this item will not be saved.
                    {
                        var newItem = new APPurchaseItem();
                        newItem.PurchaseId = model.Id;
                        newItem.ServiceAccountId = accountId;
                        newItem.Qty = 1;
                        newItem.Rate = rate;
                        newItem.Total = total;
                        newItem.TaxId = taxId;
                        newItem.DiscountPercentage = discountPercentage;
                        newItem.DiscountAmount = discountAmount;
                        newItem.SalesTaxPercentage = salesTaxPercentage;
                        newItem.SalesTaxAmount = salesTaxAmount;
                        newItem.LineTotal = linetotal;
                        newItem.Remarks = remarks;
                        newItem.SubDepartmentId = subDepartId;
                        newItem.SubAccountId = subAccountId;
                        newItem.DepartmentId = departId;
                        newItem.CostCneterId = costCenter;
                        _dbContext.APPurchaseItems.Add(newItem);
                    }
                    TempData["error"] = "false";
                    TempData["message"] = "Service Invoice has been updated successfully";
                    await _dbContext.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "No any Service Invoice has been updated. It must contain atlest one item";
                return RedirectToAction(nameof(Index));
            }
        }
        public IActionResult PartialPurchaseService(int? counter)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            ViewBag.Counter = counter;
            ViewBag.SubAccounts = new SelectList(_dbContext.GLSubAccountDetails.Where(x => !x.IsDelete).ToList(), "Id", "Description");
            ViewBag.Department = new SelectList(_dbContext.GLDivision.Where(x => x.CompanyId == companyId && x.IsDeleted == false).ToList(), "Id", "Name");
            ViewBag.CostCenter = new SelectList(_dbContext.CostCenter.Where(x => x.CompanyId == companyId && x.IsDeleted == false).ToList(), "Id", "Description");
            var model = new APPurchaseItemViewModel();
            var appTaxRepo = new AppTaxRepo(_dbContext);
            model.TaxList = appTaxRepo.GetTaxes(companyId);
            return PartialView("_partialServiceInvoice", model);
        }
        [HttpPost]
        public IActionResult GetInvoiceItems(int id, int itemId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var appTaxRepo = new AppTaxRepo(_dbContext);
            var item = _dbContext.APPurchaseItems.Include(i => i.Purchase).Where(i => i.Id == id && i.IsDeleted != true).FirstOrDefault();
            ViewBag.Counter = id;
            ViewBag.SubAccounts = new SelectList(_dbContext.GLSubAccountDetails.Where(x => !x.IsDelete).ToList(), "Id", "Description");
            ViewBag.Department = new SelectList(_dbContext.GLDivision.Where(x => x.CompanyId == companyId && x.IsDeleted == false).ToList(), "Id", "Name");
            APPurchaseItemViewModel viewModel = new APPurchaseItemViewModel();
            if (item != null)
            {
                viewModel.PurchaseItemId = item.Id;
                viewModel.ItemId = item.ItemId;
                viewModel.Qty = item.Qty;
                viewModel.Rate = item.Rate;
                viewModel.Total_ = item.Total;
                viewModel.TaxId = item.TaxId;
                viewModel.SubDepartment = item.SubDepartmentId;
                viewModel.Department = item.DepartmentId;
                viewModel.SubAccountId = item.SubAccountId;
                viewModel.CostCenter = item.CostCneterId;
                viewModel.DiscountPercentage = item.DiscountPercentage;
                viewModel.DiscountAmount = item.DiscountAmount;
                viewModel.SalesTaxPercentage = item.SalesTaxPercentage;
                viewModel.SalesTaxAmount = item.SalesTaxAmount;
                viewModel.ExciseTaxPercentage = item.ExciseTaxPercentage;
                viewModel.ExciseTaxAmount = item.ExciseTaxAmount;
                viewModel.LineTotal = item.LineTotal;
                viewModel.Remarks = item.Remarks;
                ViewBag.ItemId = itemId;
                viewModel.TaxList = appTaxRepo.GetTaxes(companyId);
            }
            return PartialView("_partialServiceInvoice", viewModel);
        }
        public async Task<IActionResult> Approve(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            ///////////////////////////
            var purchaseVoucherRepo = new PurchaseVoucherRepo(_dbContext,HttpContext);
            try
            {
                await purchaseVoucherRepo.ApproveServiceInvoice(id, companyId, userId);
                TempData["error"] = "false";
                TempData["message"] = "Purchase Service Invoice has been approved successfully";
            }
            catch (Exception ex)
            {
                TempData["error"] = "true";
                TempData["message"] = ex.InnerException.Message == null ? ex.Message.ToString() : ex.InnerException.Message.ToString();
            }
            //BkgVehicleIGP bkgIGP = _dbContext.BkgVehicleIGPs.Find(id);
            return RedirectToAction(nameof(Index));
        }
        public ActionResult UnApprove()
        {
            var purchaseVoucherRepo = new PurchaseVoucherRepo(_dbContext, HttpContext);
            var list = purchaseVoucherRepo.GetApprovedVouchers();
            ViewBag.NavbarHeading = "Un-Approve Purchase Service Invoice";
            return View(list);
        }
        public async Task<IActionResult> UnApproveVoucher(int id)
        {
            var purchaseVoucherRepo = new PurchaseVoucherRepo(_dbContext, HttpContext);
            var voucher = await purchaseVoucherRepo.UnApproveServiceVoucher(id);
            if (voucher == false)
            {
                TempData["error"] = "true";
                TempData["message"] = "Voucher not found";
            }
            else
            {
                TempData["error"] = "false";
                TempData["message"] = "Purchase has been Un-Approved successfully";
            }
            return RedirectToAction(nameof(UnApprove));
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string configValue = _dbContext.AppCompanyConfigs
                       .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                       .Select(c => c.ConfigValue)
                       .FirstOrDefault();
            ViewBag.ReportPath = string.Concat(configValue, "Viewer", "?Report=Voucher&cId=", companyId, "&id={0}");
            ViewBag.ReportPath2 = string.Concat(configValue, "Viewer", "?Report=PurchaseService&cId=", companyId, "&id={0}");

            var purchase = _dbContext.APPurchases.Include(i => i.Supplier)
            .Where(i => i.Id == id).FirstOrDefault();
            var purchaseItem = _dbContext.APPurchaseItems
                                .Include(i => i.ServiceAccount)
                                .Include(i => i.Purchase)
                                .Where(i => i.PurchaseId == id && i.IsDeleted == false)
                                .ToList();
            ViewBag.NavbarHeading = "Purchase Service Invoice";
            ViewBag.TitleStatus = "Approved";

            TempData["Detail"] = purchaseItem;
            return View(purchase);
        }

        public IActionResult GetServiceInvoice()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            try
            {
                int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
                string userId = HttpContext.Session.GetString("UserId");
                var approve = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == resp_Id).FirstOrDefault().Approve;
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var PurchasesData = (from Purchases in _dbContext.APPurchases.Where(c => c.CompanyId == companyId && c.TransactionType == "Service" && c.IsDeleted == false).Include(s => s.Supplier) select Purchases);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    PurchasesData = PurchasesData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    PurchasesData = PurchasesData.Where(m => m.PurchaseNo.ToString().Contains(searchValue)
                                                    || m.PurchaseDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchValue.ToUpper())
                                                    //|| (m.ReferenceNo != null ? m.ReferenceNo.ToString().Contains(searchValue) : m.PurchaseNo.ToString().Contains(searchValue))
                                                    || (m.IGPNo != null ? m.IGPNo.ToString().Contains(searchValue) : m.PurchaseNo.ToString().Contains(searchValue))
                                                    || m.Total.ToString().Contains(searchValue)
                                                    || m.TotalSalesTaxAmount.ToString().Contains(searchValue)
                                                    || m.GrandTotal.ToString().Contains(searchValue)
                                                    || m.Status.ToString().ToUpper().Contains(searchValue.ToUpper())
                                                  );

                }
                recordsTotal = PurchasesData.Count();
                var data = PurchasesData.ToList();
                if (pageSize == -1)
                {
                    data = PurchasesData.Skip(skip).Take(recordsTotal == 0 ? 1 : pageSize == -1 ? recordsTotal : 1).ToList();
                }
                else
                {
                    data = PurchasesData.Skip(skip).Take(pageSize).ToList();
                }
                List<APPurchaseViewModel> Details = new List<APPurchaseViewModel>();
                foreach (var grp in data)
                {
                    APPurchaseViewModel aPPurchaseViewModel = new APPurchaseViewModel();
                    aPPurchaseViewModel.PurchaseDate = grp.PurchaseDate.ToString(Helpers.CommonHelper.DateFormat);
                    aPPurchaseViewModel.APPurchases = grp;
                    aPPurchaseViewModel.APPurchases.Approve = approve;

                    Details.Add(aPPurchaseViewModel);

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