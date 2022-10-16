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

namespace Numbers.Repository.AR
{
    public class SaleReturnRepo
    {
        private readonly NumbersDbContext _dbContext;
        public SaleReturnRepo(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<ARInvoice> GetAll(int companyId)
        {
            IEnumerable<ARInvoice> listRepo = _dbContext.ARInvoices.Include(s => s.Customer).Where(s => s.CompanyId == companyId && s.TransactionType=="Sale Return" && s.IsDeleted == false)
                .ToList();
            return listRepo;
        }

        public IEnumerable<ARInvoice> GetAll(string transType, int companyId)
        {
            IEnumerable<ARInvoice> listRepo = _dbContext.ARInvoices.Include(s => s.Customer).Where(s => s.CompanyId == companyId && s.TransactionType == transType && s.IsDeleted == false)
                .ToList();
            return listRepo;
        }

        public ARInvoiceItem[] GetSaleReturnItems(int id)
        {
            ARInvoiceItem[] items = _dbContext.ARInvoiceItems.Where(i => i.InvoiceId == id && i.IsDeleted == false).ToArray();
            return items;
        }
        public ARSaleReturnViewModel GetById(int id)
        {
            var saleReturn = _dbContext.ARInvoices.Find(id);
            ARSaleReturnViewModel viewModel = new ARSaleReturnViewModel();
            viewModel.ReturnNo = saleReturn.InvoiceNo;
            viewModel.ReturnDate = saleReturn.InvoiceDate;
            viewModel.WareHouseId = saleReturn.WareHouseId;
            viewModel.ReferenceNo = saleReturn.ReferenceNo;
            viewModel.CustomerId = saleReturn.CustomerId;
            viewModel.Currency = saleReturn.Currency;
            viewModel.VoucherId = saleReturn.VoucherId;
            viewModel.CurrencyExchangeRate = saleReturn.CurrencyExchangeRate;
            viewModel.Remarks = saleReturn.Remarks;
            viewModel.Total = saleReturn.Total;
            //viewModel.TotalDiscountAmount = saleReturn.DiscountAmount;
            viewModel.GrandTotal = saleReturn.GrandTotal;
            viewModel.TotalSalesTaxAmount = saleReturn.SalesTaxAmount;
            viewModel.Status = saleReturn.Status;
            return viewModel;
        }
         
        [HttpPost]
        public async Task<bool> Create(ARSaleReturnViewModel model, IFormCollection collection, int companyId)
        {
            try
            {
                var saleReturn = new ARInvoice();
                saleReturn.InvoiceNo = this.GetReturnNo(companyId);
                saleReturn.InvoiceDate = model.ReturnDate;
                saleReturn.WareHouseId = model.WareHouseId;
                saleReturn.ReferenceNo = model.ReferenceNo;
                saleReturn.CustomerId = model.CustomerId;
                saleReturn.Currency = model.Currency;
                saleReturn.CurrencyExchangeRate = model.CurrencyExchangeRate;
                saleReturn.Remarks = model.Remarks;
                saleReturn.Total = model.Total;
                //saleReturn.DiscountAmount = model.TotalDiscountAmount;
                saleReturn.SalesTaxAmount = model.TotalSalesTaxAmount;
                saleReturn.GrandTotal = model.GrandTotal;
                saleReturn.CompanyId = model.CompanyId;
                saleReturn.ResponsibilityId = model.ResponsibilityId;
                saleReturn.CreatedBy = model.CreatedBy;
                saleReturn.CreatedDate = DateTime.Now;
                saleReturn.IsDeleted = false;
                saleReturn.TransactionType = "Sale Return";
                saleReturn.Status = "Created";
                _dbContext.ARInvoices.Add(saleReturn);
                await _dbContext.SaveChangesAsync();

                //partialView's data saving in dbContext
                for (int i = 0; i < collection["ItemId"].Count; i++)
                {
                    var returnItem = new ARInvoiceItem();
                    returnItem.InvoiceId = saleReturn.Id;
                    returnItem.ItemId = Convert.ToInt32(collection["ItemId"][i]);
                    returnItem.ReturnInvoiceNo = Convert.ToInt32(collection["InvoiceNo"][i]);
                    returnItem.ReturnInvoiceItemId = Convert.ToInt32(collection["InvoiceItemId"][i]);
                    returnItem.Qty = -Math.Abs(Convert.ToDecimal(collection["Qty"][i]));
                    returnItem.InvoiceQty = Convert.ToDecimal(collection["InvoiceQty"][i]);
                    returnItem.Rate = Convert.ToDecimal(collection["Rate"][i]);
                    returnItem.IssueRate = Convert.ToDecimal(collection["IssueRate"][i]);
                    returnItem.CostofSales = -Math.Abs(Convert.ToDecimal(collection["CostofSales"][i]));
                    returnItem.Stock = Convert.ToDecimal(collection["Stock"][i]);
                    returnItem.Total = -Math.Abs(Convert.ToDecimal(collection["Total_"][i]));
                    returnItem.TaxSlab = Convert.ToInt32(collection["TaxId"][i]);
                    returnItem.SalesTaxAmount = -Math.Abs(Convert.ToDecimal(collection["SalesTaxAmount"][i]));
                    returnItem.SalesTaxPercentage = Convert.ToDecimal(collection["SalesTaxPercentage"][i]);
                    //returnItem.DiscountAmount = -Math.Abs(Convert.ToDecimal(collection["DiscountAmount"][i]));
                    returnItem.LineTotal = -Math.Abs(Convert.ToDecimal(collection["LineTotal"][i]));
                    returnItem.IsDeleted = false;
                    _dbContext.ARInvoiceItems.Add(returnItem);
                    await _dbContext.SaveChangesAsync();
                }
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                string message = ex.Message.ToString();
                return false;
            }
        }
        [HttpPost]
        public async Task<bool> Update(ARSaleReturnViewModel model, IFormCollection collection)
        {
            //for partial-items removal
            string[] idsDeleted = Convert.ToString(collection["IdsDeleted"]).Split(",");
            if (!idsDeleted.Contains(""))
            {
                for (int j = 0; j < idsDeleted.Length; j++)
                {
                    if (idsDeleted[j] != "0")
                    {
                        var itemToRemove = _dbContext.ARInvoiceItems.Find(Convert.ToInt32(idsDeleted[j]));
                        itemToRemove.IsDeleted = true;
                        var tracker = _dbContext.ARInvoiceItems.Update(itemToRemove);
                        tracker.OriginalValues.SetValues(await tracker.GetDatabaseValuesAsync());
                        await _dbContext.SaveChangesAsync();
                    }
                }
            }
            //updating existing data
            var obj = _dbContext.ARInvoices.Find(model.Id);
            obj.InvoiceNo = model.ReturnNo;
            obj.InvoiceDate = model.ReturnDate;
            obj.WareHouseId = model.WareHouseId;
            obj.ReferenceNo = model.ReferenceNo;
            obj.CustomerId = model.CustomerId;
            obj.Currency = model.Currency;
            obj.CurrencyExchangeRate = model.CurrencyExchangeRate;
            obj.Remarks = model.Remarks;
            obj.Total = model.Total;
            //obj.DiscountAmount = model.TotalDiscountAmount;
            obj.SalesTaxAmount = model.TotalSalesTaxAmount;
            obj.GrandTotal = model.GrandTotal;
            obj.IsDeleted = model.IsDeleted;
            obj.CompanyId = model.CompanyId;
            obj.ResponsibilityId = model.ResponsibilityId;
            obj.UpdatedBy = model.UpdatedBy;
            obj.UpdatedDate = DateTime.Now;
            obj.Status = "Created";
            obj.TransactionType = "Sale Return";
            var entry = _dbContext.ARInvoices.Update(obj);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();

            var list = _dbContext.ARInvoiceItems.Where(l => l.InvoiceId == Convert.ToInt32(collection["Id"])).ToList();
            if (list != null)
            {
                for (int i = 0; i < collection["ItemId"].Count; i++)
                {
                    var returnItem = _dbContext.ARInvoiceItems
                        .Where(j => j.InvoiceId == model.Id && j.Id == Convert.ToInt32(collection["SRItemId"][i] == "" ? 0 : Convert.ToInt32(collection["SRItemId"][i]))).FirstOrDefault();
                    // Extract coresponding values from form collection
                    var itemId = Convert.ToInt32(collection["ItemId"][i]);
                    var returnInvoiceNo = Convert.ToInt32(collection["InvoiceNo"][i]);
                    var returnInvoiceItemId = Convert.ToInt32(collection["InvoiceItemId"][i]);
                    var qty = Convert.ToDecimal(collection["Qty"][i]);
                    var invoiceQty = Convert.ToDecimal(collection["InvoiceQty"][i]);
                    var rate = Convert.ToDecimal(collection["Rate"][i]);
                    var issueRate = Convert.ToDecimal(collection["IssueRate"][i]);
                    var costofSales = -Math.Abs(Convert.ToDecimal(collection["CostofSales"][i]));
                    var stock = Convert.ToDecimal(collection["Stock"][i]);
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
                        returnItem.ReturnInvoiceNo = returnInvoiceNo;
                        returnItem.ReturnInvoiceItemId = returnInvoiceItemId;
                        returnItem.InvoiceId = model.Id;
                        returnItem.Qty = -Math.Abs(qty);
                        returnItem.InvoiceQty = invoiceQty;
                        returnItem.Rate = rate;
                        returnItem.IssueRate = issueRate;
                        returnItem.CostofSales = -Math.Abs(costofSales);
                        returnItem.Stock = stock;
                        returnItem.Total = -Math.Abs(total);
                        returnItem.LineTotal = -Math.Abs(lineTotal);
                        returnItem.TaxSlab = taxId;
                        returnItem.SalesTaxAmount = -Math.Abs(salesTaxAmount);
                        returnItem.SalesTaxPercentage = salesTaxPercentage;
                        //returnItem.DiscountAmount = -Math.Abs(discountAmount);
                        var dbEntry = _dbContext.ARInvoiceItems.Update(returnItem);
                        dbEntry.OriginalValues.SetValues(await entityEntry.GetDatabaseValuesAsync());
                    }
                    else if (returnItem == null && itemId != 0)
                    {
                        ARInvoiceItem newItem = new ARInvoiceItem();
                        newItem.ItemId = itemId;
                        newItem.ReturnInvoiceNo = returnInvoiceNo;
                        newItem.ReturnInvoiceItemId = returnInvoiceItemId;
                        newItem.InvoiceId = model.Id;
                        newItem.Qty = -Math.Abs(qty);
                        newItem.InvoiceQty = invoiceQty;
                        newItem.Rate = rate;
                        newItem.IssueRate = issueRate;
                        newItem.CostofSales = -Math.Abs(costofSales);
                        newItem.Stock = stock;
                        newItem.Total = -Math.Abs(total);
                        newItem.LineTotal = -Math.Abs(lineTotal);
                        newItem.TaxSlab = taxId;
                        newItem.SalesTaxAmount = -Math.Abs(salesTaxAmount);
                        newItem.SalesTaxPercentage = salesTaxPercentage;
                        //newItem.DiscountAmount = -Math.Abs(discountAmount);
                        _dbContext.ARInvoiceItems.Add(newItem);
                    }
                    await _dbContext.SaveChangesAsync();
                }
            }
            return true;
        }

        public async Task<bool> Delete(int id)
        {
            var returnDelete = _dbContext.ARInvoices.Find(id);
            returnDelete.IsDeleted = true;
            var entry = _dbContext.ARInvoices.Update(returnDelete);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
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
                                avgRate = item.AvgRate,
                                stock = item.StockAccountId
                            };
            return items;
        }

        public ARSaleReturnViewModel GetReturnItems(int id, int itemId)
        {
            var item = _dbContext.ARInvoiceItems.Include(i => i.Invoice).Include(i => i.Item).Where(i => i.Id == id && i.IsDeleted != true).FirstOrDefault();
            ARSaleReturnViewModel viewModel = new ARSaleReturnViewModel();
            viewModel.SaleReturnId = item.Invoice.Id;
            viewModel.InvoiceItemId = item.Id;
            viewModel.SRItemId = item.Id;
            viewModel.ItemId = item.ItemId;
            viewModel.InvoiceNo = item.ReturnInvoiceNo;
            viewModel.Qty = +Math.Abs(item.Qty);
            viewModel.InvoiceQty = item.InvoiceQty;
            viewModel.Rate = item.Rate;
            viewModel.IssueRate = item.IssueRate;
            viewModel.CostofSales = +Math.Abs(item.CostofSales);
            viewModel.Stock = item.Stock;
            viewModel.TaxId = item.TaxSlab;
            viewModel.SalesTaxAmount = +Math.Abs(item.SalesTaxAmount);
            viewModel.SalesTaxPercentage = item.SalesTaxPercentage;
            //viewModel.DiscountAmount = +Math.Abs(item.DiscountAmount);
            viewModel.LineTotal = +Math.Abs(item.LineTotal);
            viewModel.Total_ = +Math.Abs(item.Total);
            return viewModel;
        }

        public int GetReturnNo(int companyId)
        {
            int maxReturnNo = 1;
            var items = _dbContext.ARInvoices.Where(c => c.CompanyId == companyId && c.TransactionType== "Sale Return").ToList();
            if (items.Count > 0)
            {
                maxReturnNo = items.Max(o => Convert.ToInt32(o.InvoiceNo));
                return maxReturnNo + 1;
            }
            else
            {
                return maxReturnNo;
            }
        }

        public ARSaleReturnViewModel GetSaleInvoiceItems(int id)
        {
            var item = _dbContext.ARInvoiceItems.Include(i => i.Item).Include(i=>i.Invoice).Where(i => i.Id == id && i.IsDeleted == false).FirstOrDefault();
            ARSaleReturnViewModel viewModel = new ARSaleReturnViewModel();
            viewModel.SaleReturnId = item.Invoice.Id;
            viewModel.InvoiceNo = item.Invoice.InvoiceNo;
            viewModel.InvoiceItemId = item.Id;
            viewModel.ItemId = item.ItemId;
            viewModel.InvoiceQty = item.Qty;
            viewModel.Rate = item.Rate;
            return viewModel;
        }
        [HttpGet]
        public dynamic GetInvoicesToReturnByCustomerId(int id, int[] skipIds,int companyId)
        {
            var invoiceItems = _dbContext.ARInvoiceItems.Include(i => i.Invoice).Include(i=>i.Item)
                .Where(i =>i.Invoice.TransactionType == "Sale" && i.Invoice.CompanyId == companyId && i.IsDeleted == false && i.Invoice.CustomerId == id)
                .Where(i => !skipIds.Contains(i.Id)).ToList();
            return invoiceItems;
        }
        public int MaxSaleReturn(int companyId)
        {
            int max = _dbContext.ARInvoices.Where(s => s.CompanyId == companyId && s.TransactionType == "Sale Return" && s.IsDeleted == false).Max(x=>x.InvoiceNo);
            return max;
        }
    }
}
