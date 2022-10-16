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
using Numbers.Repository.Setup;

namespace Numbers.Areas.AR.Controllers
{
    [Authorize]
    [Area("AR")]
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
            ViewBag.NavbarHeading = "List of Service Invoices";
            List<ARInvoice> model;
            model = _dbContext.ARInvoices.Where(c => c.CompanyId == companyId && c.TransactionType == "Service" && !c.IsClosed && !c.IsDeleted)
                                            .Include(c => c.Customer)
                                            .ToList();
            return View(model);
        }
        public IActionResult Create(int id)
        {

            ViewBag.Counter = 0;
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var appTaxRepo = new AppTaxRepo(_dbContext);
            ViewBag.Customer = (from a in _dbContext.ARCustomers.Where(x => x.CompanyId == companyId) select a.Name).ToList();
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
            AppConfigHelper helper = new AppConfigHelper(_dbContext, HttpContext);
            ViewBag.WareHouse = helper.GetWareHouses();
            if (id == 0)
            {
                int maxInvoiceNo = 1;
                var invoices = _dbContext.ARInvoices.Where(c => c.CompanyId == companyId && c.TransactionType == "Service").ToList();
                if (invoices.Count > 0)
                {
                    maxInvoiceNo = invoices.Max(v => Convert.ToInt32(v.InvoiceNo));
                    TempData["InvoiceNo"] = maxInvoiceNo + 1;
                }
                else
                {
                    TempData["InvoiceNo"] = maxInvoiceNo;
                }
                var model = new ARInvoiceViewModel();
                model.TaxList = appTaxRepo.GetTaxes(companyId);
                model.Currencies = AppCurrencyRepo.GetCurrencies();

                ViewBag.Status = "Created";
                ViewBag.EntityState = "Create";
                ViewBag.NavbarHeading = "Create Service Invoice";
                ViewBag.InvoiceItems = null;
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
                viewModel.CurrencyExchangeRate = invoice.CurrencyExchangeRate;
                //
                viewModel.TotalDiscountAmount = invoice.DiscountAmount;
                viewModel.TotalSaleSTaxAmount = invoice.SalesTaxAmount;
                viewModel.GrandTotal = invoice.GrandTotal;
                viewModel.Total = invoice.Total;
                TempData["InvoiceNo"] = invoice.InvoiceNo;
                ViewBag.Id = id;
                var invoiceItems = _dbContext.ARInvoiceItems
                                    .Include(i => i.Invoice)
                                    .Where(i => i.InvoiceId == id && i.IsDeleted == false)
                                    .ToList();
                ViewBag.InvoiceItems = invoiceItems;
                //ViewBag.EntityState = "Update";
                if (viewModel.Status != "Approved")
                {
                    ViewBag.EntityState = "Update";
                    ViewBag.NavbarHeading = "Service Invoice";
                    ViewBag.TitleStatus = "Created";
                }
                //else if (viewModel.Status == "Approved")
                //{
                //    ViewBag.NavbarHeading = "Service Invoice";
                //    ViewBag.TitleStatus = "Approved";
                //}
                viewModel.TaxList = appTaxRepo.GetTaxes(companyId);
                viewModel.Currencies = AppCurrencyRepo.GetCurrencies();
                return View(viewModel);
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
            return PartialView("_partialServiceInvoice", model);
        }
        [HttpPost]
        public async Task <IActionResult> Create(ARInvoiceViewModel model, IFormCollection collection)
        {
            
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            if (collection["ServiceAccountId"].Count > 0)
            {
                ARInvoice invoice = new ARInvoice();
                //Master Table data
                invoice.Id = model.Id;
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
                invoice.TransactionType = "Service";
                invoice.InvoiceType = "INV";
                invoice.Total = Convert.ToDecimal(collection["Total"]);
                invoice.DiscountAmount = Convert.ToDecimal(collection["TotalDiscountAmount"]);
                invoice.SalesTaxAmount = Convert.ToDecimal(collection["TotalSalesTaxAmount"]);
                invoice.GrandTotal = Convert.ToDecimal(collection["GrandTotal"]);
                invoice.CreatedBy = userId;
                invoice.CreatedDate = DateTime.Now;
                invoice.CompanyId = companyId;
                invoice.Status = "Created";
                _dbContext.ARInvoices.Add(invoice);
                await _dbContext.SaveChangesAsync();
                for (int i = 0; i < collection["ServiceAccountId"].Count; i++)
                {
                    var invoiceItem = new ARInvoiceItem();
                    invoiceItem.InvoiceId = invoice.Id;
                    invoiceItem.ServiceAccountId = Convert.ToInt32(collection["ServiceAccountId"][i]);
                    invoiceItem.Qty = 1;
                    invoiceItem.Rate = Convert.ToDecimal(collection["Total_"][i]);
                    invoiceItem.Total = Convert.ToDecimal(collection["Total_"][i]);
                    invoiceItem.TaxSlab = Convert.ToInt32(collection["TaxSlab"][i]);
                    invoiceItem.DiscountPercentage = Convert.ToDecimal(collection["DiscountPercentage"][i]);
                    invoiceItem.DiscountAmount = Convert.ToDecimal(collection["DiscountAmount"][i]);
                    invoiceItem.SalesTaxPercentage = Convert.ToDecimal(collection["SalesTaxPercentage"][i]);
                    invoiceItem.SalesTaxAmount = Convert.ToDecimal(collection["SalesTaxAmount"][i]);
                    invoiceItem.LineTotal = Convert.ToDecimal(collection["LineTotal"][i]);
                    invoiceItem.Remarks = collection["Remarks"][i + 1];
                    _dbContext.ARInvoiceItems.Add(invoiceItem);
                    await _dbContext.SaveChangesAsync();
                }
                TempData["error"] = "false";
                TempData["message"] = "Service Invoice has been created successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "No any Service Invoice has been Created. It must contain atlest one item";
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpGet]
        public IActionResult GetInvoiceItems(int id, int itemId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var appTaxRepo = new AppTaxRepo(_dbContext);
            var item = _dbContext.ARInvoiceItems.Include(i => i.Invoice).Include(i => i.ServiceAccount).Where(i => i.Id == id && i.IsDeleted != true).FirstOrDefault();
            ViewBag.Counter = id;
            ARInvoiceViewModel viewModel = new ARInvoiceViewModel();
            if (item != null)
            {
                viewModel.InvoiceItemId = item.Id;
                viewModel.ItemId = item.ItemId;
                viewModel.Qty = item.Qty;
                viewModel.Stock = item.Stock;
                viewModel.Rate = item.Rate;
                viewModel.Total_ = item.Total;
                viewModel.TaxSlab = item.TaxSlab;
                viewModel.DiscountPercentage = item.DiscountPercentage;
                viewModel.DiscountAmount = item.DiscountAmount;
                viewModel.SalesTaxPercentage = item.SalesTaxPercentage;
                viewModel.SalesTaxAmount = item.SalesTaxAmount;
                viewModel.ExciseTaxPercentage = item.ExciseTaxPercentage;
                viewModel.ExciseTaxAmount = item.ExciseTaxAmount;
                viewModel.LineTotal = item.LineTotal;
                viewModel.InvoiceItemRemarks = item.Remarks;
                ViewBag.ItemId = itemId;
                viewModel.TaxList = appTaxRepo.GetTaxes(companyId);
            }
            return PartialView("_partialServiceInvoice", viewModel);
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
            if (collection["ServiceAccountId"].Count > 0)
            {
                //updating existing data
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                string userId = HttpContext.Session.GetString("UserId");
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
                invoice.DiscountAmount = Convert.ToDecimal(collection["TotalDiscountAmount"]);
                invoice.SalesTaxAmount = Convert.ToDecimal(collection["TotalSalesTaxAmount"]);
                invoice.GrandTotal = Convert.ToDecimal(collection["GrandTotal"]);
                invoice.UpdatedBy = userId;
                invoice.UpdatedDate = DateTime.Now;
                var entry = _dbContext.ARInvoices.Update(invoice);
                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                await _dbContext.SaveChangesAsync();
                //Update Invoice detail items/ Invoice items if any
                var list = _dbContext.ARInvoiceItems.Where(i => i.InvoiceId == Convert.ToInt32(collection["Id"])).ToList();
                if (list != null)
                {
                    for (int i = 0; i < collection["ServiceAccountId"].Count; i++)
                    {
                        var invoiceItem = _dbContext.ARInvoiceItems
                            .Where(j => j.InvoiceId == model.Id && j.Id == Convert.ToInt32(collection["InvoiceItemId"][i] == "" ? 0 : Convert.ToInt32(collection["InvoiceItemId"][i]))).FirstOrDefault();
                        // Extract coresponding values from form collection
                        var accountId = Convert.ToInt32(collection["ServiceAccountId"][i]);
                        var rate = Convert.ToDecimal(collection["Total_"][i]);
                        var total = Convert.ToDecimal(collection["Total_"][i]);
                        var taxSlab = Convert.ToInt32(collection["TaxSlab"][i]);
                        var discountPercentage = Convert.ToDecimal(collection["DiscountPercentage"][i]);
                        var discountAmount = Convert.ToDecimal(collection["DiscountAmount"][i]);
                        var salesTaxPercentage = Convert.ToDecimal(collection["SalesTaxPercentage"][i]);
                        var salesTaxAmount = Convert.ToDecimal(collection["SalesTaxAmount"][i]);
                        var linetotal = Convert.ToDecimal(collection["LineTotal"][i]);
                        var remarks = collection["Remarks"][i + 1];
                        if (invoiceItem != null && accountId != 0)
                        {
                            //below phenomenon prevents Id from being marked as modified
                            var entityEntry = _dbContext.Entry(invoiceItem);
                            entityEntry.State = EntityState.Modified;
                            entityEntry.Property(p => p.Id).IsModified = false;
                            invoiceItem.ServiceAccountId = accountId;
                            invoiceItem.InvoiceId = model.Id;
                            invoiceItem.Qty = 1;
                            invoiceItem.Rate = rate;
                            invoiceItem.Total = total;
                            invoiceItem.TaxSlab = taxSlab;
                            invoiceItem.DiscountPercentage = discountPercentage;
                            invoiceItem.DiscountAmount = discountAmount;
                            invoiceItem.SalesTaxPercentage = salesTaxPercentage;
                            invoiceItem.SalesTaxAmount = salesTaxAmount;
                            invoiceItem.LineTotal = linetotal;
                            invoiceItem.Remarks = remarks;

                            var dbEntry = _dbContext.ARInvoiceItems.Update(invoiceItem);
                            dbEntry.OriginalValues.SetValues(await entityEntry.GetDatabaseValuesAsync());

                        }
                        //check if user created new item while updating
                        else if (accountId != 0 && invoiceItem == null)//itemId is invitem, if this is null or zero rest of the information for this item will not be saved.
                        {
                            var newItem = new ARInvoiceItem();
                            newItem.InvoiceId = model.Id;
                            newItem.ServiceAccountId = accountId;
                            newItem.Qty = 1;
                            newItem.Rate = rate;
                            newItem.Total = total;
                            newItem.TaxSlab = taxSlab;
                            newItem.DiscountPercentage = discountPercentage;
                            newItem.DiscountAmount = discountAmount;
                            newItem.SalesTaxPercentage = salesTaxPercentage;
                            newItem.SalesTaxAmount = salesTaxAmount;
                            newItem.LineTotal = linetotal;
                            newItem.Remarks = remarks;
                            _dbContext.ARInvoiceItems.Add(newItem);
                        }

                        TempData["error"] = "false";
                        TempData["message"] = "Invoice has been updated successfully";
                        await _dbContext.SaveChangesAsync();
                    }

                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = "false";
                TempData["message"] = "No any Service Invoice has been updated. It must contain atlest one item";
                return RedirectToAction(nameof(Index));
            }
        }
        public async Task<IActionResult> Approve(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");

            //BkgVehicleIGP bkgIGP = _dbContext.BkgVehicleIGPs.Find(id);
            ARInvoice invoice = _dbContext.ARInvoices
             .Include(c => c.Customer)
             .Where(a => a.Status == "Created" && a.TransactionType=="Service" && a.CompanyId == _companyId && a.Id == id && a.IsDeleted == false)
             .FirstOrDefault();
            try
            {
                //Create Voucher
                VoucherHelper voucher = new VoucherHelper(_dbContext, HttpContext);
                GLVoucher voucherMaster = new GLVoucher();
                List<GLVoucherDetail> voucherDetails = new List<GLVoucherDetail>();
                string voucherDescription = string.Format(
                "Invoice # : {0} of  " +
                "{1} {2}",
                invoice.InvoiceNo,
                invoice.Customer.Name, invoice.Remarks);

                int voucherId;
                voucherMaster.VoucherType = "I-SRV";
                voucherMaster.VoucherDate = invoice.InvoiceDate;
                voucherMaster.Reference = invoice.ReferenceNo;
                voucherMaster.Currency = invoice.Currency;
                voucherMaster.CurrencyExchangeRate = invoice.CurrencyExchangeRate;
                voucherMaster.Description = voucherDescription;
                voucherMaster.Status = "Approved";
                voucherMaster.ApprovedBy = _userId;
                voucherMaster.ApprovedDate = DateTime.Now;
                voucherMaster.IsSystem = true;
                voucherMaster.ModuleName = "AR/Service";
                voucherMaster.ModuleId = id;
                voucherMaster.Amount = invoice.GrandTotal;

                //Voucher Details
                var invoiceItems = _dbContext.ARInvoiceItems.Where(i => i.InvoiceId == invoice.Id && !i.IsDeleted).ToList();
                var amount = invoiceItems.Sum(s => s.LineTotal);
                var discount = invoiceItems.Sum(s => s.DiscountAmount);
                //Debit Entry
                GLVoucherDetail voucherDetail = new GLVoucherDetail();

                voucherDetail.AccountId = invoice.Customer.AccountId;
                voucherDetail.Sequence = 10;
                voucherDetail.Description = voucherDescription;
                voucherDetail.Debit = amount;
                voucherDetail.Credit = 0;
                voucherDetail.IsDeleted = false;
                voucherDetail.CreatedBy = _userId;
                voucherDetail.CreatedDate = DateTime.Now;
                voucherDetails.Add(voucherDetail);
                //if (discount > 0)
                //{
                //    var accountCode = _dbContext.AppCompanyConfigs.Where(c => c.CompanyId == _companyId && c.ConfigName == "Discount" && c.IsActive).FirstOrDefault().UserValue1;
                //    var discountAccount = _dbContext.GLAccounts.Where(a => a.Code == accountCode && a.CompanyId == _companyId && a.IsActive).FirstOrDefault().Id;
                //    voucherDetail = new GLVoucherDetail();
                //    voucherDetail.AccountId = discountAccount;
                //    voucherDetail.Sequence = 10;
                //    voucherDetail.Description = voucherDescription;
                //    voucherDetail.Debit = discount;
                //    voucherDetail.Credit = 0;
                //    voucherDetail.IsDeleted = false;
                //    voucherDetail.CreatedBy = _userId;
                //    voucherDetail.CreatedDate = DateTime.Now;
                //    voucherDetails.Add(voucherDetail);
                //}
                #region Sale Account
                //Credit Entry
                //var lineItems = _dbContext.ARInvoiceItems.Where(li => li.InvoiceId == id && !li.IsDeleted).ToList();
                var lineItems = (from li in _dbContext.ARInvoiceItems
                                 where li.InvoiceId == id
                                 select new
                                 {
                                     li.Total,
                                     li.DiscountAmount,
                                     li.ServiceAccountId
                                 }).GroupBy(l => l.ServiceAccountId)
                                           .Select(li => new ARInvoiceItem
                                           {
                                               Total = li.Sum(c => c.Total)-li.Sum(c=>c.DiscountAmount),
                                               InvoiceId = li.FirstOrDefault().ServiceAccountId //invoice id is temporarily containing SalesTaxAccountId
                                           })
                                       .ToList();


                var saleTaxAccounts = (from li in _dbContext.ARInvoiceItems
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
                                           })
                                       .ToList();
                foreach (var item in saleTaxAccounts)
                {
                    if (item.InvoiceId != 0 && item.SalesTaxAmount>0)
                    {                        
                        voucherDetail = new GLVoucherDetail();
                        voucherDetail.AccountId = item.InvoiceId;
                        voucherDetail.Sequence = 20;
                        voucherDetail.Description = string.Format("Sales Tax Service Invoice # {0}",invoice.InvoiceNo);
                        voucherDetail.Debit = 0;
                        voucherDetail.Credit = item.SalesTaxAmount;
                        voucherDetail.IsDeleted = false;
                        voucherDetail.CreatedBy = _userId;
                        voucherDetail.CreatedDate = DateTime.Now;
                        voucherDetails.Add(voucherDetail);
                    }
                }
                foreach (var item in lineItems)
                {
                    voucherDetail = new GLVoucherDetail();
                    voucherDetail.AccountId = item.InvoiceId;
                    voucherDetail.Sequence = 20;
                    voucherDetail.Description = voucherDescription;
                    if (item.Total > 0)//+ve
                    {
                        voucherDetail.Debit = 0;
                        voucherDetail.Credit = item.Total; 
                    }
                    else//-ve
                    {
                        voucherDetail.Debit = Math.Abs(item.Total);
                        voucherDetail.Credit = 0;
                    }                                      
                    voucherDetail.IsDeleted = false;
                    voucherDetail.CreatedBy = _userId;
                    voucherDetail.CreatedDate = DateTime.Now;
                    voucherDetails.Add(voucherDetail);
                }
            
                #endregion Sale Account
                //Create Voucher 
                voucherId = voucher.CreateVoucher(voucherMaster, voucherDetails);
                if (voucherId != 0)
                {
                    invoice.VoucherId = voucherId;
                    invoice.Status = "Approved";
                    invoice.ApprovedBy = _userId;
                    invoice.ApprovedDate = DateTime.Now;

                    //On approval updating Invoice
                    TempData["error"] = "false";
                    TempData["message"] = "Invoice has been approved successfully";
                    var entry = _dbContext.Update(invoice);
                    entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                    await _dbContext.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Cannot generate voucher please verify debit and credit entries.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                TempData["error"] = "true";
                TempData["message"] =ex.InnerException.Message==null? ex.Message.ToString():ex.InnerException.Message.ToString();
                return RedirectToAction(nameof(Index));
            }
        }
        public ActionResult UnApprove()
        {
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            string userId = HttpContext.Session.GetString("UserId");
            ViewBag.UnApprove = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == resp_Id).FirstOrDefault().UnApprove;

            var list = _dbContext.ARInvoices
               .Where(v => v.Status == "Approved" && v.IsDeleted == false && v.TransactionType == "Service" && v.CompanyId == HttpContext.Session.GetInt32("CompanyId")).ToList();
            ViewBag.NavbarHeading = "Un-Approve Service Invoice";
            return View(list);
        }
        public async Task<IActionResult> UnApproveVoucher(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var voucher = _dbContext.ARInvoices
                            .Where(v => v.IsDeleted == false && v.Id == id && v.Status == "Approved" && v.IsClosed == false && v.CompanyId == companyId).FirstOrDefault();
            if (voucher == null)
            {
                TempData["error"] = "true";
                TempData["message"] = "Voucher not found";
            }
            else
            {
                var voucherDetail = _dbContext.GLVoucherDetails.Where(v => v.VoucherId == voucher.VoucherId).ToList();
                foreach(var item in voucherDetail)
                {
                    var tracker = _dbContext.GLVoucherDetails.Remove(item);
                    tracker.OriginalValues.SetValues(await tracker.GetDatabaseValuesAsync());
                    await _dbContext.SaveChangesAsync();
                }
                voucher.Status = "Created";
                voucher.ApprovedBy = null;
                voucher.ApprovedDate = null;
                var entry = _dbContext.ARInvoices.Update(voucher);
                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                await _dbContext.SaveChangesAsync();
                TempData["error"] = "false";
                TempData["message"] = string.Format("Invoice Id. {0} has been Un-Approved successfully", voucher.InvoiceNo);
            }
            return RedirectToAction(nameof(UnApprove));
        }
        //ali
        public async Task<IActionResult> Delete(int id)
        {
            var invoice = _dbContext.ARInvoices.Find(id);
            invoice.IsDeleted = true;
            invoice.VoucherId = 0;
            var entry = _dbContext.ARInvoices.Update(invoice);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            TempData["error"] = "false";
            TempData["message"] = string.Format("Invoice No {0} has been deleted", invoice.InvoiceNo);
            return RedirectToAction(nameof(Index));
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
            ViewBag.ReportPath2 = string.Concat(configValues, "Viewer", "?Report=Service&cId=", companyId, "&id={0}");

            var invoice = _dbContext.ARInvoices.Include(i => i.Customer)
            .Where(i => i.Id == id).FirstOrDefault();
            var invoiceItem = _dbContext.ARInvoiceItems
                                .Include(i=>i.ServiceAccount)
                                .Include(i => i.Invoice)
                                .Where(i => i.InvoiceId == id && i.IsDeleted == false)
                                .ToList();
            ViewBag.NavbarHeading = "Service Invoice";
            ViewBag.TitleStatus = "Approved";

            TempData["Detail"] = invoiceItem;
            return View(invoice);
        }
    }
}