using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Repository.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numbers.Repository.AP
{
    public class PurchaseReturnRepo
    {
        private HttpContext HttpContext { get; }

        private readonly NumbersDbContext _dbContext;
        public PurchaseReturnRepo(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public PurchaseReturnRepo(NumbersDbContext dbContext, HttpContext httpContext)
        {
            _dbContext = dbContext;
            HttpContext = httpContext;
        }
        public IEnumerable<APPurchase> GetAll(int companyId)
        {
            IEnumerable<APPurchase> listRepo = _dbContext.APPurchases.Include(p => p.Supplier).Where(p => p.IsDeleted == false && p.TransactionType == "Purchase Return" && p.CompanyId == companyId)
                .ToList();
            return listRepo;
        }
        public IEnumerable<APPurchase> GetAll(string transType, int companyId)
        {
            IEnumerable<APPurchase> listRepo = _dbContext.APPurchases.Include(s => s.Supplier).Where(s => s.CompanyId == companyId && s.TransactionType == transType && s.IsDeleted == false)
                .ToList();
            return listRepo;
        }

        public List<APPurchaseItem> GetPurchaseReturnItems(int id)
        {
            List<APPurchaseItem> items = _dbContext.APPurchaseItems.Where(i=>i.IsDeleted==false && i.PurchaseId==id).ToList();
            return items;
        }
        public APPurchaseReturnViewModel GetById(int id)
        {
            APPurchase purchaseReturn = _dbContext.APPurchases.Find(id);
            var viewModel = new APPurchaseReturnViewModel();
            viewModel.Id = purchaseReturn.Id;
            viewModel.ReturnNo = purchaseReturn.PurchaseNo;
            viewModel.ReturnDate = purchaseReturn.PurchaseDate;
            viewModel.WareHouseId = purchaseReturn.WareHouseId;
            viewModel.ReferenceNo = purchaseReturn.ReferenceNo;
            viewModel.SupplierId = purchaseReturn.SupplierId;
            viewModel.Currency = purchaseReturn.Currency;
            viewModel.CurrencyExchangeRate = purchaseReturn.CurrencyExchangeRate;
            viewModel.Remarks = purchaseReturn.Remarks;
            viewModel.VoucherId = purchaseReturn.VoucherId;
            viewModel.Total = purchaseReturn.Total;
            viewModel.TotalSaleTaxAmount = purchaseReturn.TotalSalesTaxAmount;
            viewModel.GrandTotal = purchaseReturn.GrandTotal;
            viewModel.IsDeleted = purchaseReturn.IsDeleted;
            viewModel.Status = purchaseReturn.Status;
            return viewModel;
        }

        [HttpPost]
        public async Task<bool> Create(APPurchaseReturnViewModel model, IFormCollection collection)
        {
            
            string module = (from c in _dbContext.Sys_Responsibilities where c.Resp_Id == model.Resp_Id select c.Resp_Name).FirstOrDefault();
            try
            {
                var purchaseReturn = new APPurchase();
                purchaseReturn.PurchaseNo = model.ReturnNo;
                purchaseReturn.PurchaseDate = model.ReturnDate;
                purchaseReturn.WareHouseId = model.WareHouseId;
                purchaseReturn.ReferenceNo = model.ReferenceNo;
                purchaseReturn.SupplierId = model.SupplierId;
                purchaseReturn.Currency = model.Currency;
                purchaseReturn.CurrencyExchangeRate = model.CurrencyExchangeRate;
                purchaseReturn.Remarks = model.Remarks;
                purchaseReturn.Total = model.Total;
                purchaseReturn.TotalSalesTaxAmount = model.TotalSaleTaxAmount;
                purchaseReturn.GrandTotal = model.GrandTotal;
                purchaseReturn.CompanyId = model.CompanyId;
                purchaseReturn.CreatedBy = model.CreatedBy;
                purchaseReturn.CreatedDate = DateTime.Now;
                purchaseReturn.IsDeleted = false;
                purchaseReturn.Status = "Created";
                purchaseReturn.TransactionType = "Purchase Return";
                purchaseReturn.Resp_ID = model.Resp_Id;
                _dbContext.APPurchases.Add(purchaseReturn);
                await _dbContext.SaveChangesAsync();

                //partialView's data saving in dbContext
                for (int i = 0; i < collection["ItemId"].Count; i++)
                {
                    var returnItem = new APPurchaseItem();
                    returnItem.PurchaseId = purchaseReturn.Id;
                    returnItem.ItemId = Convert.ToInt32(collection["ItemId"][i]);
                    returnItem.ReturnPurchaseNo = Convert.ToInt32(collection["InvoiceNo"][i]);
                    returnItem.ReturnInvoiceItemId = Convert.ToInt32(collection["PurchaseInvoiceItemId"][i]);
                    //returnItem.Qty = -Math.Abs(Convert.ToDecimal(collection["Qty"][i]));
                    returnItem.Qty = Math.Abs(Convert.ToDecimal(collection["Qty"][i]));
                    returnItem.InvoiceQty = Convert.ToDecimal(collection["InvoiceQty"][i]);
                    returnItem.Rate = Convert.ToDecimal(collection["Rate"][i]);
                    //returnItem.Total = Math.Abs(Convert.ToDecimal(collection["Total_"][i]));
                    returnItem.Total = Math.Abs(Convert.ToDecimal(collection["Total_"][i]));
                    returnItem.TaxId = Convert.ToInt32(collection["TaxId"][i]);
                    //returnItem.SalesTaxAmount = -Math.Abs(Convert.ToDecimal(collection["SalesTaxAmount"][i]));
                    returnItem.SalesTaxAmount = Math.Abs(Convert.ToDecimal(collection["SalesTaxAmount"][i]));
                    returnItem.SalesTaxPercentage = Convert.ToDecimal(collection["SalesTaxPercentage"][i]);
                    //returnItem.DiscountAmount = -Math.Abs(Convert.ToDecimal(collection["DiscountAmount"][i]));
                   // returnItem.LineTotal = -Math.Abs(Convert.ToDecimal(collection["LineTotal"][i]));
                    returnItem.LineTotal = Math.Abs(Convert.ToDecimal(collection["LineTotal"][i]));
                    returnItem.BrandId = module == "Yarn Purchase" ? Convert.ToInt32(collection["BrandId"][i]) : 0;
                    returnItem.IsDeleted = false;

                    APPurchaseItem aPPurchaseItem = _dbContext.APPurchaseItems.FirstOrDefault(x=>x.Id == Convert.ToInt32(collection["PurchaseInvoiceItemId"][i]));
                    aPPurchaseItem.Qty = aPPurchaseItem.Qty - returnItem.Qty;
                    aPPurchaseItem.SalesTaxAmount = aPPurchaseItem.SalesTaxAmount - returnItem.SalesTaxAmount;
                    aPPurchaseItem.LineTotal = aPPurchaseItem.LineTotal - returnItem.LineTotal;
                    _dbContext.APPurchaseItems.Update(aPPurchaseItem);

                    _dbContext.APPurchaseItems.Add(returnItem);
                    await _dbContext.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                string message = ex.Message.ToString();
                return false;
            }
        }
        [HttpPost]
        public async Task<bool> Update(APPurchaseReturnViewModel model, IFormCollection collection)
        {
            //for partial-items removal
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
            //updating existing data
            var obj = _dbContext.APPurchases.Find(model.Id);
            obj.PurchaseNo = model.ReturnNo;
            obj.PurchaseDate = model.ReturnDate;
            obj.WareHouseId = model.WareHouseId;
            obj.ReferenceNo = model.ReferenceNo;
            obj.SupplierId = model.SupplierId;
            obj.Currency = model.Currency;
            obj.CurrencyExchangeRate = model.CurrencyExchangeRate;
            obj.Remarks = model.Remarks;
            obj.Total = model.Total;
            obj.TotalSalesTaxAmount = model.TotalSaleTaxAmount;
            obj.GrandTotal = model.GrandTotal;
            obj.IsDeleted = model.IsDeleted;
            obj.CompanyId = model.CompanyId;
            obj.UpdatedBy = model.UpdatedBy;
            obj.UpdatedDate = DateTime.Now;
            var entry = _dbContext.APPurchases.Update(obj);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();

            var list = _dbContext.APPurchaseItems.Where(l => l.PurchaseId == Convert.ToInt32(collection["Id"])).ToList();
            if (list != null)
            {
                for (int i = 0; i < collection["ItemId"].Count; i++)
                {
                    var returnItem = _dbContext.APPurchaseItems
                        .Where(j => j.PurchaseId == model.Id && j.Id == Convert.ToInt32(collection["PRItemId"][i] == "" ? 0 : Convert.ToInt32(collection["PRItemId"][i]))).FirstOrDefault();
                    // Extract coresponding values from form collection
                    var itemId = Convert.ToInt32(collection["ItemId"][i]);
                    var returnPurchaseNo = Convert.ToInt32(collection["InvoiceNo"][i]);
                    var returnInvoiceItemId = Convert.ToInt32(collection["PurchaseInvoiceItemId"][i]);
                    var qty = Convert.ToDecimal(collection["Qty"][i]);
                    var invoiceQty = Convert.ToDecimal(collection["InvoiceQty"][i]);
                    var rate = Convert.ToDecimal(collection["Rate"][i]);
                    var total = Convert.ToDecimal(collection["Total_"][i]);
                    var lineTotal = Convert.ToDecimal(collection["LineTotal"][i]);
                    var taxId = Convert.ToInt32(collection["TaxId"][i]);
                    var salesTaxAmount = Convert.ToDecimal(collection["SalesTaxAmount"][i]);
                    var salesTaxPercentage = Convert.ToDecimal(collection["SalesTaxPercentage"][i]);
                    //var discountAmount = Convert.ToDecimal(collection["DiscountAmount"][i]);
                    if (returnItem != null && itemId != 0)
                    {
                        var entityEntry = _dbContext.Entry(returnItem);
                        entityEntry.State = EntityState.Modified;
                        entityEntry.Property(p => p.Id).IsModified = false;
                        returnItem.ItemId = itemId;
                        returnItem.ReturnPurchaseNo = returnPurchaseNo;
                        returnItem.ReturnInvoiceItemId = returnInvoiceItemId;
                        returnItem.PurchaseId = model.Id;
                        //returnItem.Qty = -Math.Abs(qty);
                        returnItem.Qty = Math.Abs(qty);
                        returnItem.InvoiceQty = invoiceQty;
                        returnItem.Rate = rate;
                        //returnItem.Total = -Math.Abs(total);
                        returnItem.Total = Math.Abs(total);
                        //returnItem.LineTotal = -Math.Abs(lineTotal);
                        returnItem.LineTotal = Math.Abs(lineTotal);
                        returnItem.TaxId = taxId;
                        //returnItem.SalesTaxAmount = -Math.Abs(salesTaxAmount);
                        returnItem.SalesTaxAmount = Math.Abs(salesTaxAmount);
                        returnItem.SalesTaxPercentage = salesTaxPercentage;
                        //returnItem.DiscountAmount = -Math.Abs(discountAmount);
                        var dbEntry = _dbContext.APPurchaseItems.Update(returnItem);
                        dbEntry.OriginalValues.SetValues(await entityEntry.GetDatabaseValuesAsync());
                    }
                    else if (returnItem == null && itemId != 0)
                    {
                        APPurchaseItem newItem = new APPurchaseItem();
                        newItem.ItemId = itemId;
                        newItem.ReturnPurchaseNo = returnPurchaseNo;
                        newItem.ReturnInvoiceItemId = returnInvoiceItemId;
                        newItem.PurchaseId = model.Id;
                        //newItem.Qty = -Math.Abs(qty);
                        newItem.Qty = Math.Abs(qty);
                        newItem.InvoiceQty = invoiceQty;
                        newItem.Rate = rate;
                        newItem.Total = total;
                        newItem.LineTotal = lineTotal;
                        newItem.TaxId = taxId;
                        //newItem.SalesTaxAmount = -Math.Abs(salesTaxAmount);
                        newItem.SalesTaxAmount = Math.Abs(salesTaxAmount);
                        newItem.SalesTaxPercentage = salesTaxPercentage;
                        //newItem.DiscountAmount = discountAmount;
                        _dbContext.APPurchaseItems.Add(newItem);
                    }
                    await _dbContext.SaveChangesAsync();
                }
            }
            return true;
        }

        public dynamic GetItemDetails(int id)
        {
            dynamic items = from item in _dbContext.InvItems
                            join config in _dbContext.AppCompanyConfigs on item.Id equals id
                            where item.Id == id && config.ConfigName == "UOM" && config.Module == "Inventory" && config.Id == item.Unit
                            select new
                            {
                                uom = config.ConfigValue,
                                id = config.Id,
                                rate = item.PurchaseRate,
                                stock = item.StockAccountId
                            };
            return items;
        }
        public APPurchaseReturnViewModel GetReturnItems(int id, int itemId)
        {
            var item = _dbContext.APPurchaseItems.Include(i => i.Purchase).Include(i => i.Item).Where(i => i.Id == id && i.IsDeleted != true).FirstOrDefault();
            APPurchaseReturnViewModel viewModel = new APPurchaseReturnViewModel();
            viewModel.PRItemId = item.Id;
            viewModel.PurchaseInvoiceItemId = item.Id;
            viewModel.ItemId = item.ItemId;
            viewModel.InvoiceNo = item.ReturnPurchaseNo;
            viewModel.Qty = +Math.Abs(item.Qty);
            viewModel.InvoiceQty = item.InvoiceQty;
            viewModel.Rate = item.Rate;
            viewModel.TaxId = item.TaxId;
            viewModel.SalesTaxAmount = +Math.Abs(item.SalesTaxAmount);
            viewModel.SalesTaxPercentage = item.SalesTaxPercentage;
            //viewModel.DiscountAmount = +Math.Abs(item.DiscountAmount);
            viewModel.LineTotal = +Math.Abs(item.LineTotal);
            viewModel.Total_ = +Math.Abs(item.Total);
            return viewModel;
        }

        public async Task<bool> Delete(int id)
        {
            var deletePurchase = _dbContext.APPurchases.Find(id);
            deletePurchase.IsDeleted = true;
            var entry = _dbContext.APPurchases.Update(deletePurchase);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public int GetReturnNo(int companyId)
        {
            int maxReturnNo = 1;
            var items = _dbContext.APPurchases.Where(c => c.CompanyId == companyId && c.TransactionType == "Purchase Return").ToList();
            if (items.Count > 0)
            {
                maxReturnNo = items.Max(o => o.PurchaseNo);
                return maxReturnNo + 1;
            }
            else
            {
                return maxReturnNo;
            }
        }

        public APPurchaseReturnViewModel GetPurchaseInvoiceItems(int id)
        {
            var item = _dbContext.APPurchaseItems.Where(x=>x.Id == id).Include(i => i.Item).Include(i => i.Purchase).Where(i => i.IsDeleted == false).FirstOrDefault();
            APPurchaseReturnViewModel viewModel = new APPurchaseReturnViewModel();
            viewModel.InvoiceNo = item.Purchase.PurchaseNo;
            viewModel.SalesTaxAmount = item.SalesTaxAmount;
            viewModel.LineTotal = item.LineTotal;
            viewModel.PurchaseInvoiceItemId = item.Id;
            viewModel.ItemId = item.ItemId;
            viewModel.InvoiceQty = item.Qty;
            viewModel.Rate = item.Rate;
            viewModel.TaxId = item.TaxId;
            if (item.BrandId != 0)
            {
                viewModel.BrandId = item.BrandId;
                viewModel.BrandName = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == item.BrandId).ConfigValue;
            }
            viewModel.BrandId = item.BrandId;
            return viewModel;
        }
        [HttpGet]
        public dynamic GetInvoicesToReturnBySupplierId(int id, int[] skipIds,int companyId)
        {
            var purchaseItems = _dbContext.APPurchaseItems.Include(i => i.Purchase).Include(i => i.Item)
                .Where(i => i.Purchase.TransactionType == "Purchase" && i.Purchase.CompanyId == companyId && i.IsDeleted == false && i.Purchase.SupplierId == id)
                .Where(i => !skipIds.Contains(i.Id) && i.Qty > 0).ToList();
            return purchaseItems;
        }

        public IEnumerable<APPurchase> GetApprovedVouchers()
        {
            return _dbContext.APPurchases
               .Where(v => v.Status == "Approved" && v.IsDeleted == false && v.TransactionType == "Purchase Return" && v.CompanyId == HttpContext.Session.GetInt32("CompanyId")).ToList();
        }
        public async Task<bool> UnApproveVoucher(int id)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                    var voucher = _dbContext.APPurchases
                                    .Where(v => v.IsDeleted == false && v.Id == id && v.Status == "Approved" && v.CompanyId == companyId).FirstOrDefault();
                    if (voucher != null)
                    {
                        var voucherDetail = _dbContext.GLVoucherDetails.Where(v => v.VoucherId == voucher.VoucherId).ToList();
                        foreach (var item in voucherDetail)
                        {
                            var tracker = _dbContext.GLVoucherDetails.Remove(item);
                            tracker.OriginalValues.SetValues(await tracker.GetDatabaseValuesAsync());
                            await _dbContext.SaveChangesAsync();
                        }
                        voucher.Status = "Created";
                        voucher.ApprovedBy = null;
                        voucher.ApprovedDate = null;

                        var purchaseItems = _dbContext.APPurchaseItems.Where(p => p.IsDeleted == false && p.PurchaseId == id).ToList();
                        foreach (var purchaseItem in purchaseItems)
                        {
                            var item = _dbContext.InvItems.Find(purchaseItem.ItemId);
                            item.StockQty = item.StockQty - purchaseItem.Qty;
                            item.StockValue = item.StockValue - (purchaseItem.Rate * purchaseItem.Qty);
                            if (item.StockQty != 0)
                            {
                                item.AvgRate = Math.Round(item.StockValue / item.StockQty, 6);
                            }
                            var dbEntry = _dbContext.InvItems.Update(item);
                            dbEntry.OriginalValues.SetValues(await dbEntry.GetDatabaseValuesAsync());
                            await _dbContext.SaveChangesAsync();
                        }
                        var entry = _dbContext.APPurchases.Update(voucher);
                        entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                        await _dbContext.SaveChangesAsync();
                        transaction.Commit();
                    }
                    return true;
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc);
                    return false;
                }
            }
                
        }
    }
}
