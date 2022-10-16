using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Repository.AR
{
    public class SaleOrderRepo
    {
        private readonly NumbersDbContext _dbContext;
        public SaleOrderRepo(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<ARSaleOrder> GetAll(int companyId)
        {
            //IEnumerable<ARSaleOrder> listRepo = _dbContext.ARSaleOrders.Include(c => c.Customer).Where(c => c.CompanyId == companyId && c.IsDeleted == false).ToList();
            IEnumerable<ARSaleOrder> listRepo = _dbContext.ARSaleOrders.Include(s => s.Customer).Where(c => c.CompanyId == companyId && c.IsDeleted == false).ToList();
            return listRepo;
        }

        public ARSaleOrderItem[] GetSaleOrderItems(int id)
        {
            var ite = _dbContext.ARSaleOrderItems.Where(i => i.SaleOrderId == id && i.IsDeleted == false).ToList();
            ARSaleOrderItem[] items = _dbContext.ARSaleOrderItems.Where(i => i.SaleOrderId == id && i.IsDeleted == false).ToArray();
            return items;
        }

        public ARSaleOrderViewModel GetById(int id)
        {
            ARSaleOrder listOrder = _dbContext.ARSaleOrders.Find(id);
            var viewModel = new ARSaleOrderViewModel();
            viewModel.Id = listOrder.Id;
            viewModel.SaleOrderNo = listOrder.SaleOrderNo;
            viewModel.SaleOrderDate = listOrder.SaleOrderDate;
            viewModel.Currency = listOrder.Currency;
            viewModel.CurrencyExchangeRate = listOrder.CurrencyExchangeRate;
            viewModel.WareHouseId = listOrder.WareHouseId;
            viewModel.CustomerId = listOrder.CustomerId;
            viewModel.ReferenceNo = listOrder.ReferenceNo;
            viewModel.DeliveryTermId = listOrder.DeliveryTermId;
            viewModel.PaymentTermId = listOrder.PaymentTermId;
            viewModel.Status = listOrder.Status;
            viewModel.Remarks = listOrder.Remarks;
            viewModel.Total = listOrder.Total;
            viewModel.TotalTaxAmount = listOrder.TotalTaxAmount;
            viewModel.Freight = listOrder.Freight;
            viewModel.CostCenter = listOrder.CostCenter;
            viewModel.GrandTotal = listOrder.GrandTotal;
            viewModel.ProductTypeId = listOrder.ProductTypeId;
            viewModel.ItemCategoryId = listOrder.ItemCategoryId;
            viewModel.FourthCategoryId = listOrder.FourthCategoryId;
            return viewModel;
        }

        [HttpPost]
        public async Task<bool> Create(ARSaleOrderViewModel modelRepo, IFormCollection collection)
        {
            try
            {
                int length = _dbContext.ARCreditLimit.Where(x => x.ARCustomerId == modelRepo.CustomerId && x.IsActive && x.IsClosed == false && x.IsExpired == false).Count();
                ARSaleOrder saleOrder = new ARSaleOrder();
                saleOrder.SaleOrderNo = this.SaleOrderCountNo(modelRepo.CompanyId);
                saleOrder.SaleOrderDate = modelRepo.SaleOrderDate;
                saleOrder.Validity = modelRepo.Validity;
                saleOrder.Currency = modelRepo.Currency;
                saleOrder.CurrencyExchangeRate = modelRepo.CurrencyExchangeRate;
                saleOrder.WareHouseId = modelRepo.WareHouseId;
                saleOrder.CustomerId = modelRepo.CustomerId;
                saleOrder.ReferenceNo = modelRepo.ReferenceNo;
                //saleOrder.DeliveryTermId = modelRepo.DeliveryTermId;
                //saleOrder.PaymentTermId = modelRepo.PaymentTermId;
                saleOrder.Remarks = modelRepo.Remarks;
                //saleOrder.Total = modelRepo.Total;
                saleOrder.Total = Convert.ToDecimal(collection["formTotal"]);
                saleOrder.TotalTaxAmount = modelRepo.TotalTaxAmount;
                saleOrder.Freight = modelRepo.Freight;
                saleOrder.GrandTotal = modelRepo.GrandTotal;
                saleOrder.CreatedBy = modelRepo.CreatedBy;
                saleOrder.CompanyId = modelRepo.CompanyId;
                saleOrder.ResponsibilityId = modelRepo.ResponsibilityId;
                saleOrder.CostCenter = modelRepo.CostCenter;
                saleOrder.ItemCategoryId = modelRepo.ItemCategoryId;
                saleOrder.FourthCategoryId = modelRepo.FourthCategoryId;
                saleOrder.ProductTypeId = modelRepo.ProductTypeId;
                saleOrder.CreatedDate = DateTime.Now;
                saleOrder.IsDeleted = false;
                saleOrder.Status = "Created";
                _dbContext.ARSaleOrders.Add(saleOrder);
                await _dbContext.SaveChangesAsync();
                List<int> z = new List<int>();
                z.Add(0);
                //partialView's data saving in dbContext
                for (int i = 0; i < collection["ItemId"].Count; i++)
                {
                    var orderItem = new ARSaleOrderItem();
                    orderItem.SaleOrderId = saleOrder.Id;
                    orderItem.ItemId = Convert.ToInt32(collection["ItemId"][i]);
                    orderItem.BaleType = Convert.ToString(collection["BaleType"][i]);
                    orderItem.AvailableStock = Convert.ToInt32(collection["AvailableStock"][i]);
                    orderItem.BookedStock = Convert.ToInt32(collection["BookedStock"][i]);
                    orderItem.BalanceStock = Convert.ToInt32(collection["BalanceStock"][i]);
                    orderItem.Meters = Convert.ToDecimal(collection["Meters"][i]);
                    orderItem.Qty = Convert.ToInt32(collection["SaleQty"][i]);
                    orderItem.Rate = Convert.ToDecimal(collection["Rate"][i]);
                    orderItem.PricingRate = Convert.ToDecimal(collection["PricingRate"][i]);
                    orderItem.Total = Convert.ToDecimal(collection["TotalAmount"][i]);
                    orderItem.BaleId = Convert.ToInt32(collection["BaleId"][i]);
                    orderItem.Conversion = Convert.ToDecimal(collection["Conversion"][i]);
                    orderItem.Price = Convert.ToDecimal(collection["Price"][i]);
                    orderItem.UOMQty = Convert.ToDecimal(collection["UOMQty"][i]);
                    orderItem.TotalMeter = Convert.ToDecimal(collection["TotalMeter"][i]);
                    orderItem.TotalMeterAmount = Convert.ToDecimal(collection["TotalMeterAmount"][i]);

                    //orderItem.TaxId = Convert.ToInt32(collection["TaxId"][i]);
                    orderItem.TaxId = _dbContext.AppTaxes.Where(x => x.Name == "NO TAX").Select(x => x.Id).FirstOrDefault();
                    //orderItem.TaxAmount = Convert.ToDecimal(collection["TaxAmount"][i]);
                    //orderItem.LineTotal = Convert.ToDecimal(collection["LineTotal"][i]);
                    //orderItem.DetailCostCenter = Convert.ToInt32(collection["DetailCostCenter"][i]);
                    List<BaleInformation> baleInformations = _dbContext.BaleInformation.Where(x => x.ItemId == orderItem.ItemId && x.Meters == orderItem.Meters && x.TempBales != 0 && x.UsedFNumber != true).ToList();
                    int index = 0;
                    foreach (var item in baleInformations)
                    {
                        if (index < orderItem.Qty)
                        {
                            item.TempBales = item.TempBales - 1;
                            _dbContext.BaleInformation.Update(item);
                        }
                        else
                        {
                            break;
                        }
                        index++;
                    }

                    if (orderItem.DetailCostCenter == 0)
                    {
                        orderItem.DetailCostCenter = saleOrder.CostCenter;
                        _dbContext.ARSaleOrderItems.Add(orderItem);
                    }

                    else
                        _dbContext.ARSaleOrderItems.Add(orderItem);

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
        public async Task<bool> Update(ARSaleOrderViewModel modelRepo, IFormCollection collection)
        {
            //for partial-items removal
            //string[] idsDeleted = Convert.ToString(collection["IdsDeleted"]).Split(",");
            //if (!idsDeleted.Contains(""))
            //{
            //    for (int j = 0; j < idsDeleted.Length; j++)
            //    {
            //        if (idsDeleted[j] != "0")
            //        {
            //            var itemToRemove = _dbContext.ARSaleOrderItems.Find(Convert.ToInt32(idsDeleted[j]));
            //            itemToRemove.IsDeleted = true;
            //            var tracker = _dbContext.ARSaleOrderItems.Update(itemToRemove);
            //            tracker.OriginalValues.SetValues(await tracker.GetDatabaseValuesAsync());
            //            await _dbContext.SaveChangesAsync();
            //        }
            //    }

            //}
            //updating existing data
            var obj = _dbContext.ARSaleOrders.Find(modelRepo.Id);
            obj.SaleOrderNo = modelRepo.SaleOrderNo;
            obj.SaleOrderDate = modelRepo.SaleOrderDate;
            obj.Validity = modelRepo.Validity;
            obj.Currency = modelRepo.Currency;
            obj.CurrencyExchangeRate = modelRepo.CurrencyExchangeRate;
            obj.WareHouseId = modelRepo.WareHouseId;
            obj.CustomerId = modelRepo.CustomerId;
            obj.ReferenceNo = modelRepo.ReferenceNo;
            obj.DeliveryTermId = modelRepo.DeliveryTermId;
            obj.PaymentTermId = modelRepo.PaymentTermId;
            obj.Remarks = modelRepo.Remarks;
            //  obj.Total = modelRepo.Total;
            obj.Total = Convert.ToDecimal(collection["formTotal"]);
            obj.TotalTaxAmount = modelRepo.TotalTaxAmount;
            obj.Freight = modelRepo.Freight;
            obj.GrandTotal = modelRepo.GrandTotal;
            obj.UpdatedBy = modelRepo.UpdatedBy;
            obj.CompanyId = modelRepo.CompanyId;
            obj.ResponsibilityId = modelRepo.ResponsibilityId;
            obj.UpdatedDate = DateTime.Now;
            obj.IsDeleted = modelRepo.IsDeleted;
            obj.CostCenter = modelRepo.CostCenter;
            obj.ItemCategoryId = modelRepo.ItemCategoryId;
            obj.FourthCategoryId = modelRepo.FourthCategoryId;
            obj.ProductTypeId = modelRepo.ProductTypeId;
            var entry = _dbContext.ARSaleOrders.Update(obj);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            var EditList = new List<int>();
            for (int i = 0; i < collection["ItemId"].Count; i++)
            {
                try
                {
                    int id = Convert.ToInt32(collection["SOItemId"][i]);
                    EditList.Add(id);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

            }
            await _dbContext.SaveChangesAsync();
            var list = _dbContext.ARSaleOrderItems.Where(l => l.SaleOrderId == Convert.ToInt32(collection["Id"])).ToList();
            foreach (var detail in list)
            {
                bool result = EditList.Exists(x => x == detail.Id);
                if (!result)
                {
                    var item = _dbContext.ARSaleOrderItems.Where(x => x.Id == detail.Id).FirstOrDefault();
                    _dbContext.ARSaleOrderItems.Remove(item);
                }
            }
            if (list != null)
            {
                for (int i = 0; i < collection["ItemId"].Count; i++)
                {
                    var orderItem = _dbContext.ARSaleOrderItems
                            .Where(j => j.SaleOrderId == modelRepo.Id && j.Id == Convert.ToInt32(collection["SOItemId"][i] == "" ? 0 : Convert.ToInt32(collection["SOItemId"][i]))).FirstOrDefault();
                    // Extract coresponding values from form collection

                    var itemId = Convert.ToInt32(collection["ItemId"][i]);
                    var baleType = Convert.ToString(collection["BaleType"][i]);
                    var availableStock = Convert.ToInt32(collection["AvailableStock"][i]);
                    var bookedStock = Convert.ToInt32(collection["BookedStock"][i]);
                    var balanceStock = Convert.ToInt32(collection["BalanceStock"][i]);
                    var meters = Convert.ToDecimal(collection["Meters"][i]);
                    var qty = Convert.ToInt32(collection["SaleQty"][i]);
                    var rate = Convert.ToDecimal(collection["Rate"][i]);
                    var pricingRate = Convert.ToDecimal(collection["PricingRate"][i]);
                    var total = Convert.ToDecimal(collection["TotalAmount"][i]);
                    var baleId = Convert.ToInt32(collection["BaleId"][i]);
                    var totalMeter = Convert.ToDecimal(collection["TotalMeter"][i]);
                    var totalMeterAmount = Convert.ToDecimal(collection["TotalMeterAmount"][i]);
                    var conversion = Convert.ToDecimal(collection["Conversion"][i]);
                    var price = Convert.ToDecimal(collection["Price"][i]);
                    var uOMQty = Convert.ToDecimal(collection["UOMQty"][i]);

                    var baleQty = orderItem.Qty - qty;
                    //BaleInformation baleInformation = _dbContext.BaleInformation.Where(x => x.Id == baleId).FirstOrDefault();
                    //if  (orderItem != null) { 
                    //    baleInformation.TempBales = baleInformation.TempBales +  orderItem.Qty - qty;
                    //}
                    //else
                    //{
                    //    baleInformation.TempBales = baleInformation.TempBales - qty;

                    //}
                    //_dbContext.BaleInformation.Update(baleInformation);


                    for (int a = 0; a < Math.Abs( baleQty); a++)
                    {

                        List<BaleInformation> baleInformationsZero = _dbContext.BaleInformation.Where(x => x.ItemId == itemId && x.Meters == meters && x.TempBales == 0).ToList();
                        List<BaleInformation> baleInformationsOne = _dbContext.BaleInformation.Where(x => x.ItemId == itemId && x.Meters == meters && x.TempBales != 0).ToList();
                        if (qty > orderItem.Qty)
                        {
                            baleInformationsOne[0].TempBales = (baleInformationsOne[0].TempBales - 1);
                            _dbContext.BaleInformation.Update(baleInformationsOne[0]);
                            await _dbContext.SaveChangesAsync();
                        }
                        else if (qty < orderItem.Qty)
                        {
                            baleInformationsZero[0].TempBales = (baleInformationsZero[0].TempBales + 1);
                            _dbContext.BaleInformation.Update(baleInformationsZero[0]);
                            await _dbContext.SaveChangesAsync();
                        }
                        else
                        {
                            
                        }
                    }

                    if (orderItem != null && itemId != 0)
                    {
                        var entityEntry = _dbContext.Entry(orderItem);
                        entityEntry.State = EntityState.Modified;
                        entityEntry.Property(p => p.Id).IsModified = false;
                        orderItem.ItemId = itemId;
                        orderItem.SaleOrderId = modelRepo.Id;
                        orderItem.BaleType = baleType;
                        orderItem.AvailableStock = availableStock;
                        orderItem.BookedStock = bookedStock;
                        orderItem.BalanceStock = balanceStock;
                        orderItem.Meters = meters;
                        orderItem.Qty = qty;
                        orderItem.Rate = rate; 
                        orderItem.PricingRate = pricingRate;
                        orderItem.Total = total;
                        orderItem.BaleId = baleId;
                        orderItem.Conversion = conversion;
                        orderItem.Price = price;
                        orderItem.UOMQty = uOMQty;
                        orderItem.TotalMeter = totalMeter;
                        orderItem.TotalMeterAmount = totalMeterAmount;
                        orderItem.TaxId = _dbContext.AppTaxes.Where(x => x.Name == "NO TAX").Select(x => x.Id).FirstOrDefault();

                        if (orderItem.DetailCostCenter == 0)
                        {
                            orderItem.DetailCostCenter = obj.CostCenter;
                        }
                        var dbEntry = _dbContext.ARSaleOrderItems.Update(orderItem);
                        dbEntry.OriginalValues.SetValues(await entityEntry.GetDatabaseValuesAsync());
                    }
                    else if (orderItem == null && itemId != 0)
                    {
                        ARSaleOrderItem newItem = new ARSaleOrderItem();
                        newItem.ItemId = itemId;
                        newItem.SaleOrderId = modelRepo.Id;
                        newItem.BaleType = baleType;
                        newItem.AvailableStock = availableStock;
                        newItem.BookedStock = bookedStock;
                        newItem.BalanceStock = balanceStock;
                        newItem.Meters = meters;
                        newItem.Qty = qty;
                        newItem.Rate = rate;
                        newItem.PricingRate = pricingRate;
                        newItem.Total = total;
                        newItem.BaleId = baleId;
                        newItem.Conversion = conversion;
                        newItem.Price = price;
                        newItem.UOMQty = uOMQty;
                        newItem.TotalMeter = totalMeter;
                        newItem.TotalMeterAmount = totalMeterAmount;
                        newItem.TaxId = _dbContext.AppTaxes.Where(x => x.Name == "NO TAX").Select(x => x.Id).FirstOrDefault();

                        if (newItem.DetailCostCenter == 0)
                        {
                            newItem.DetailCostCenter = obj.CostCenter;
                        }
                        _dbContext.ARSaleOrderItems.Add(newItem);
                    }

                }
                await _dbContext.SaveChangesAsync();

            }
            return true;
        }

        public async Task<bool> Delete(int id)
        {
            var saleOrderDelete = _dbContext.ARSaleOrders.Find(id);
            saleOrderDelete.IsDeleted = true;
            var entry = _dbContext.ARSaleOrders.Update(saleOrderDelete);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Approve(int id, string userId)
        {
            try
            {
                var orderApprove = _dbContext.ARSaleOrders.Find(id);
                orderApprove.Status = "Approved";
                orderApprove.ApprovedBy = userId;
                orderApprove.ApprovedDate = DateTime.Now;
                _dbContext.Update(orderApprove);
                // entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                _dbContext.SaveChanges();
                decimal Total = orderApprove.GrandTotal;
                List<int> z = new List<int>();
                z.Add(0);
                int length = _dbContext.ARCreditLimit.Where(x => x.ARCustomerId == orderApprove.CustomerId && x.IsActive && x.IsClosed == false && x.IsExpired == false).Count();
                if (length > 0)
                {
                    for (int j = 0; j < length; j++)
                    {

                        ARCreditLimit aRCreditLimit = _dbContext.ARCreditLimit.LastOrDefault(x => x.ARCustomerId == orderApprove.CustomerId && x.IsActive && x.IsClosed == false && x.IsExpired == false && !z.Contains(x.Id));
                        if (Total > Convert.ToDecimal(0))
                        {
                            if (aRCreditLimit.RemainingBalance > Total)
                            {
                                Total = Convert.ToDecimal(aRCreditLimit.RemainingBalance) - Total;
                                aRCreditLimit.RemainingBalance = Total;
                                Total = 0;
                            }
                            else
                            {
                                Total = Total - Convert.ToDecimal(aRCreditLimit.RemainingBalance);
                                aRCreditLimit.RemainingBalance = 0;
                            }
                            aRCreditLimit.UpdatedBy = userId;
                            aRCreditLimit.UpdatedDate = DateTime.Now;
                            _dbContext.ARCreditLimit.Update(aRCreditLimit);
                            await _dbContext.SaveChangesAsync();
                        }
                        z.Add(aRCreditLimit.Id);
                    }
                }


                //for (int a = 0; a < Math.Abs(orderApprove baleQty); a++)
                //{

                //    List<BaleInformation> baleInformationsZero = _dbContext.BaleInformation.Where(x => x.ItemId == itemId && x.Meters == meters && x.TempBales == 0).ToList();
                //    List<BaleInformation> baleInformationsOne = _dbContext.BaleInformation.Where(x => x.ItemId == itemId && x.Meters == meters && x.TempBales != 0).ToList();
                //    if (qty > orderItem.Qty)
                //    {
                //        baleInformationsOne[0].TempBales = (baleInformationsOne[0].TempBales - 1);
                //        _dbContext.BaleInformation.Update(baleInformationsOne[0]);
                //        await _dbContext.SaveChangesAsync();
                //    }
                //    else if (qty < orderItem.Qty)
                //    {
                //        baleInformationsZero[0].TempBales = (baleInformationsZero[0].TempBales + 1);
                //        _dbContext.BaleInformation.Update(baleInformationsZero[0]);
                //        await _dbContext.SaveChangesAsync();
                //    }
                //    else
                //    {

                //    }
                //}
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                string message = ex.Message.ToString();
                return false;
            }
        }
        public async Task<bool> UnApproveVoucher(int id, string userId)
        {
            try
            {
                var orderApprove = _dbContext.ARSaleOrders.Find(id);
                orderApprove.Status = "Created";
                orderApprove.ApprovedBy = null;
                orderApprove.ApprovedDate = DateTime.Now;
                var entry = _dbContext.Update(orderApprove);
                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                await _dbContext.SaveChangesAsync();
                decimal Total = orderApprove.GrandTotal;
                List<int> z = new List<int>();
                z.Add(0);
                int length = _dbContext.ARCreditLimit.Where(x => x.ARCustomerId == orderApprove.CustomerId && x.IsActive && x.IsClosed == false && x.IsExpired == false).Count();
                if (length > 0)
                {
                    for (int j = 0; j < length; j++)
                    {

                        ARCreditLimit aRCreditLimit = _dbContext.ARCreditLimit.FirstOrDefault(x => x.ARCustomerId == orderApprove.CustomerId && x.IsActive && x.IsClosed == false && x.IsExpired == false && !z.Contains(x.Id));
                        if (Total > Convert.ToDecimal(0))
                        {
                            if (Convert.ToDecimal(aRCreditLimit.CreditLimit) >= Total)
                            {
                                aRCreditLimit.RemainingBalance = Convert.ToDecimal(aRCreditLimit.RemainingBalance) + Total;
                                //aRCreditLimit.RemainingBalance = orderApprove.GrandTotal;
                            }
                            else
                            {
                                if (Convert.ToDecimal(aRCreditLimit.CreditLimit) != aRCreditLimit.RemainingBalance)
                                {
                                    Total = Total - Convert.ToDecimal(aRCreditLimit.CreditLimit);
                                    // orderApprove.GrandTotal = orderApprove.GrandTotal + Convert.ToDecimal(aRCreditLimit.RemainingBalance);
                                    aRCreditLimit.RemainingBalance = Convert.ToDecimal(aRCreditLimit.CreditLimit);
                                }
                            }
                            aRCreditLimit.UpdatedBy = userId;
                            aRCreditLimit.UpdatedDate = DateTime.Now;
                            _dbContext.ARCreditLimit.Update(aRCreditLimit);
                            _dbContext.SaveChanges();
                        }
                        z.Add(aRCreditLimit.Id);
                    }
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

        public dynamic GetOrderItems(int id, int itemId)
        {
            var item = _dbContext.ARSaleOrderItems.Include(i => i.SaleOrder).Include(i => i.Item).Where(i => i.Id == id && i.IsDeleted != true).FirstOrDefault();
            ARSaleOrderViewModel viewModel = new ARSaleOrderViewModel();
            viewModel.PRItemId = item.Id;
            viewModel.ItemId = item.ItemId;
            viewModel.BaleNumber = item.BaleNumber;
            viewModel.Qty = item.Qty;
            viewModel.Rate = item.Rate;
            viewModel.TaxId = item.TaxId;
            viewModel.TaxAmount = item.TaxAmount;
            viewModel.LineTotal = item.LineTotal;
            viewModel.Total_ = item.Total;
            viewModel.DetailCostCenter = item.DetailCostCenter;
            return viewModel;
        }

        public int SaleOrderCountNo(int companyId)
        {
            int maxSaleOrderNo = 1;
            var orders = _dbContext.ARSaleOrders.Where(c => c.CompanyId == companyId).ToList();
            if (orders.Count > 0)
            {
                maxSaleOrderNo = orders.Max(o => o.SaleOrderNo);
                return maxSaleOrderNo + 1;
            }
            else
            {
                return maxSaleOrderNo;
            }
        }
        public int MaxSaleOrder(int companyId)
        {
            int max = _dbContext.ARSaleOrders.Where(x => x.CompanyId == companyId).Max(x => x.SaleOrderNo);
            return max;
        }
    }
}
