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
    public class PurchaseRepo
    {
        private readonly NumbersDbContext _dbContext;
        public PurchaseRepo(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<APPurchase> GetAll(int companyId)
        { 
            IEnumerable<APPurchase> listRepo = _dbContext.APPurchases.Include(p => p.WareHouse).Include(p => p.Supplier).Where(p => p.CompanyId == companyId && p.IsDeleted == false)
                                             .ToList();
            return listRepo;
        }

        public APPurchaseItem[] GetItems(int id)
        {
            APPurchaseItem[] items = _dbContext.APPurchaseItems.Where(i => i.PurchaseId == id && i.IsDeleted == false).ToArray();
            return items;
        }
        public APPurchaseItemViewModel GetById(int id)
        {
            var purchase = _dbContext.APPurchases.Find(id);
            var viewModel = new APPurchaseItemViewModel();
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
            viewModel.GrandTotal = purchase.GrandTotal;
            viewModel.TotalDiscountAmount = purchase.TotalDiscountAmount;
            viewModel.TotalExciseTaxAmount = purchase.TotalExciseTaxAmount;
            viewModel.Total = purchase.Total;
            viewModel.TotalSalesTaxAmount = purchase.TotalSalesTaxAmount;
            return viewModel;
        }

        [HttpPost]
        public async Task<bool> Create(APPurchaseItemViewModel modelRepo, IFormCollection collection)
        {
            try
            {
                APPurchase purchase = new APPurchase();
                //Master Table data
                purchase.PurchaseNo = modelRepo.PurchaseNo;
                purchase.PurchaseDate = modelRepo.PurchaseDate;
                purchase.SupplierInvoiceDate = modelRepo.SupplierInvoiceDate;
                purchase.WareHouseId = modelRepo.WareHouseId;
                purchase.SupplierId = modelRepo.SupplierId;
                purchase.ReferenceNo = modelRepo.ReferenceNo;
                purchase.SupplierInvoiceNo = modelRepo.SupplierInvoiceNo;
                purchase.IGPNo = modelRepo.IGPNo;
                purchase.Remarks = collection["Remarks"][0];
                purchase.Currency = modelRepo.Currency;
                purchase.CurrencyExchangeRate = modelRepo.CurrencyExchangeRate;
                purchase.Total = Convert.ToDecimal(collection["Total"]);
                purchase.TotalDiscountAmount = Convert.ToDecimal(collection["totalDiscountAmount"]);
                purchase.TotalSalesTaxAmount = Convert.ToDecimal(collection["totalSalesTaxAmount"]);
                purchase.TotalExciseTaxAmount = Convert.ToDecimal(collection["totalExciseTaxAmount"]);
                purchase.GrandTotal = Convert.ToDecimal(collection["GrandTotal"]);
                purchase.CreatedBy = modelRepo.CreatedBy;
                purchase.CreatedDate = DateTime.Now;
                purchase.CompanyId = modelRepo.CompanyId;
                purchase.Status = "Created";
                _dbContext.APPurchases.Add(purchase);
                await _dbContext.SaveChangesAsync();
                for (int i = 0; i < collection["ItemId"].Count; i++)
                {
                    var purchaseItem = new APPurchaseItem();
                    purchaseItem.PurchaseId = purchase.Id;
                    purchaseItem.ItemId = Convert.ToInt32(collection["ItemId"][i]);
                    purchaseItem.Qty = Convert.ToDecimal(collection["Qty"][i]);
                    purchaseItem.Rate = Convert.ToDecimal(collection["Rate"][i]);
                    purchaseItem.Total = Convert.ToDecimal(collection["Total_"][i]);
                    purchaseItem.TaxId = Convert.ToInt32(collection["TaxId"][i]);
                    purchaseItem.DiscountPercentage = Convert.ToDecimal(collection["DiscountPercentage"][i]);
                    purchaseItem.DiscountAmount = Convert.ToDecimal(collection["DiscountAmount"][i]);
                    purchaseItem.SalesTaxPercentage = Convert.ToDecimal(collection["SalesTaxPercentage"][i]);
                    purchaseItem.SalesTaxAmount = Convert.ToDecimal(collection["SalesTaxAmount"][i]);
                    purchaseItem.ExciseTaxPercentage = Convert.ToDecimal(collection["ExciseTaxPercentage"][i]);
                    purchaseItem.ExciseTaxAmount = Convert.ToDecimal(collection["ExciseTaxAmount"][i]);
                    purchaseItem.LineTotal = Convert.ToDecimal(collection["LineTotal"][i]);
                    purchaseItem.Remarks = collection["Remarks"][i + 1];
                    _dbContext.APPurchaseItems.Add(purchaseItem);
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
        public async Task<bool> Update(APPurchaseItemViewModel model, IFormCollection collection)
        {
            //for partial-items removal
            string[] idsDeleted = Convert.ToString(collection["IdsDeleted"]).Split(",");
            if (!idsDeleted.Contains(""))
            { 
                for (int j = 0; j < idsDeleted.Length; j++)
                {
                    var itemToRemove = _dbContext.APPurchaseItems.Find(Convert.ToInt32(idsDeleted[j]));
                    itemToRemove.IsDeleted = true;
                    var tracker = _dbContext.APPurchaseItems.Update(itemToRemove);
                    tracker.OriginalValues.SetValues(await tracker.GetDatabaseValuesAsync());
                    await _dbContext.SaveChangesAsync();
                }

            }
            //updating existing data
            APPurchase purchase = _dbContext.APPurchases.Find(model.Id);
            purchase.PurchaseNo = model.PurchaseNo;
            purchase.PurchaseDate = model.PurchaseDate;
            purchase.SupplierInvoiceDate = model.SupplierInvoiceDate;
            purchase.WareHouse = model.WareHouse;
            purchase.SupplierId = model.SupplierId;
            purchase.ReferenceNo = model.ReferenceNo;
            purchase.IGPNo = model.IGPNo;
            purchase.SupplierInvoiceNo = model.SupplierInvoiceNo;
            purchase.Remarks = collection["Remarks"][0];
            purchase.Currency = model.Currency;
            purchase.CurrencyExchangeRate = model.CurrencyExchangeRate;
            purchase.Total = Convert.ToDecimal(collection["Total"]);
            purchase.TotalDiscountAmount = Convert.ToDecimal(collection["totalDiscountAmount"]);
            purchase.TotalSalesTaxAmount = Convert.ToDecimal(collection["totalSalesTaxAmount"]);
            purchase.TotalExciseTaxAmount = Convert.ToDecimal(collection["totalExciseTaxAmount"]);
            purchase.GrandTotal = Convert.ToDecimal(collection["GrandTotal"]);
            purchase.Status = "Created";
            purchase.UpdatedBy = model.UpdatedBy;
            purchase.UpdatedDate = DateTime.Now;
            var entry = _dbContext.APPurchases.Update(purchase);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            //Update purchase detail items/ purchase items if any
            var list = _dbContext.APPurchaseItems.Where(i => i.PurchaseId == model.Id).ToList();
            if (list != null)
            {
                for (int i = 0; i < collection["ItemId"].Count; i++)
                {
                    var purchaseItem = _dbContext.APPurchaseItems
                        .Where(j => j.PurchaseId == model.Id && j.Id == Convert.ToInt32(collection["PurchaseItemId"][i] == "" ? 0 : Convert.ToInt32(collection["PurchaseItemId"][i]))).FirstOrDefault();
                    // Extract coresponding values from form collection
                    var itemId = Convert.ToInt32(collection["ItemId"][i]);
                    var qty = Convert.ToDecimal(collection["Qty"][i]);
                    var rate = Convert.ToDecimal(collection["Rate"][i]);
                    var total = Convert.ToDecimal(collection["Total_"][i]);
                    var taxId = Convert.ToInt32(collection["TaxId"][i]);
                    var discountPercentage = Convert.ToDecimal(collection["DiscountPercentage"][i]);
                    var discountAmount = Convert.ToDecimal(collection["DiscountAmount"][i]);
                    var salesTaxPercentage = Convert.ToDecimal(collection["SalesTaxPercentage"][i]);
                    var salesTaxAmount = Convert.ToDecimal(collection["SalesTaxAmount"][i]);
                    var exciseTaxPercentage = Convert.ToDecimal(collection["ExciseTaxPercentage"][i]);
                    var exciseTaxAmount = Convert.ToDecimal(collection["ExciseTaxAmount"][i]);
                    var lineTotal = Convert.ToDecimal(collection["LineTotal"][i]);
                    var remarks = collection["Remarks"][i + 1];
                    if (purchaseItem != null && itemId != 0)
                    {
                        //below phenomenon prevents Id from being marked as modified
                        var entityEntry = _dbContext.Entry(purchaseItem);
                        entityEntry.State = EntityState.Modified;
                        entityEntry.Property(p => p.Id).IsModified = false;
                        purchaseItem.ItemId = itemId;
                        purchaseItem.PurchaseId = model.Id;
                        purchaseItem.Qty = qty;
                        purchaseItem.DiscountAmount = 0;
                        purchaseItem.Rate = rate;
                        purchaseItem.Total = total;
                        purchaseItem.TaxId = taxId;
                        purchaseItem.DiscountPercentage = discountPercentage;
                        purchaseItem.DiscountAmount = discountAmount;
                        purchaseItem.SalesTaxPercentage = salesTaxPercentage;
                        purchaseItem.SalesTaxAmount = salesTaxAmount;
                        purchaseItem.ExciseTaxPercentage = exciseTaxPercentage;
                        purchaseItem.ExciseTaxAmount = exciseTaxAmount;
                        purchaseItem.LineTotal = lineTotal;
                        purchaseItem.Remarks = remarks;

                        var dbEntry = _dbContext.APPurchaseItems.Update(purchaseItem);
                        dbEntry.OriginalValues.SetValues(await entityEntry.GetDatabaseValuesAsync());

                    }
                    //check if user created new item while updating
                    else if (itemId != 0 && purchaseItem == null)//itemId is invitem, if this is null or zero rest of the information for this item will not be saved.
                    {
                        var newItem = new APPurchaseItem();
                        newItem.PurchaseId = model.Id;
                        newItem.ItemId = itemId;
                        newItem.Qty = qty;
                        newItem.Rate = rate;
                        newItem.Total = total;
                        newItem.TaxId = taxId;
                        newItem.DiscountPercentage = discountPercentage;
                        newItem.DiscountAmount = discountAmount;
                        newItem.SalesTaxPercentage = salesTaxPercentage;
                        newItem.SalesTaxAmount = salesTaxAmount;
                        newItem.ExciseTaxPercentage = exciseTaxPercentage;
                        newItem.ExciseTaxAmount = exciseTaxAmount;
                        newItem.LineTotal = lineTotal;
                        newItem.Remarks = remarks;
                        _dbContext.APPurchaseItems.Add(newItem);
                    }
                    await _dbContext.SaveChangesAsync();
                }
            }
            return true;
        }

        public async Task<bool> Delete(int id)
        {
            var purchaseDelete = _dbContext.APPurchases.Find(id);
            purchaseDelete.IsDeleted = true;
            var entry = _dbContext.APPurchases.Update(purchaseDelete);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Approve(int id,string userId)
        {
            try
            {
                var approve = _dbContext.APPurchases.Find(id);
                approve.Status = "Approved";
                approve.ApprovedBy = userId;
                approve.ApprovedDate = DateTime.Now;
                var entry = _dbContext.Update(approve);
                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                string message = ex.Message.ToString();
                return false;
            }
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

        public APPurchaseItemViewModel GetPurchaseItems(int id, int itemId)
        {
            var item = _dbContext.APPurchaseItems.Include(i => i.Purchase).Include(i => i.Item).Where(i => i.Id == id && i.IsDeleted != true).FirstOrDefault();
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
            return viewModel;
        }

        public int GetPurchaseNo(int companyId)
        {
            int maxPurchaseNo = 1;
            var purchases = _dbContext.APPurchases.Where(c => c.CompanyId == companyId).ToList();
            if (purchases.Count > 0)
            {
                maxPurchaseNo = purchases.Max(v => v.PurchaseNo);
                return maxPurchaseNo + 1;
            }
            else
            {
                return maxPurchaseNo;
            }

        }
    }
}
