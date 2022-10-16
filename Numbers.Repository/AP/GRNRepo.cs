using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Repository.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numbers.Repository.AP
{
    public class GRNRepo
    {
        private readonly NumbersDbContext _dbContext;
        public GRNRepo(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public string UploadFile(IFormFile img)
        {
            string filesList = "";
            if (img!= null)
            {
                if (img.Length > 0)
                {
                    var fileName = Path.GetFileName(img.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads\\purchaseOrder-images", fileName);
                    using (var Fstream = new FileStream(filePath, FileMode.Create))
                    {
                        img.CopyTo(Fstream);
                        var fullPath = "/uploads/purchaseOrder-images/" + fileName;
                        filesList += fullPath;
                    }
                }
            }
            return filesList;
        }
      
        public IEnumerable<APPurchaseOrder> GetAll(int companyId)
        {
            IEnumerable<APPurchaseOrder> listRepo = _dbContext.APPurchaseOrders.Include(c=>c.Supplier).Where(c => c.CompanyId == companyId && c.IsDeleted==false).ToList();
            return listRepo;
        }

        public APPurchaseOrderItem[] GetPurchaseOrderItems(int id)
        {
            APPurchaseOrderItem[] items = _dbContext.APPurchaseOrderItems.Where(i => i.POId == id && i.IsDeleted == false).ToArray();
            return items;
        }

        public APPurchaseOrderViewModel GetById(int id)
        {
            APPurchaseOrder listOrder = _dbContext.APPurchaseOrders.Find(id);
            var viewModel = new APPurchaseOrderViewModel();
            viewModel.Id = listOrder.Id;
            viewModel.PONo = listOrder.PONo;
            viewModel.PODate = listOrder.PODate;
            viewModel.Currency = listOrder.Currency;
            viewModel.CurrencyExchangeRate = listOrder.CurrencyExchangeRate;
            viewModel.POTypeId = listOrder.POTypeId;
            viewModel.SupplierId = listOrder.SupplierId;
            viewModel.ReferenceNo = listOrder.ReferenceNo;
            viewModel.DeliveryTermId = listOrder.DeliveryTermId;
            viewModel.PaymentTermId = listOrder.PaymentTermId;
            viewModel.Remarks = listOrder.Remarks;
            viewModel.Status = listOrder.Status;
            viewModel.Total = listOrder.Total;
            viewModel.TotalTaxAmount = listOrder.TotalTaxAmount;
            viewModel.Freight = listOrder.Freight;
            viewModel.GrandTotal = listOrder.GrandTotal;
            viewModel.PaymentMode = listOrder.PaymentMode;
            viewModel.CostTerms = listOrder.CostTerms;
            viewModel.ShippingMode = listOrder.ShippingMode;
            viewModel.ShippingPort = listOrder.ShippingPort;
            viewModel.DischargePort = listOrder.DischargePort;
            viewModel.Origin = listOrder.Origin;
            viewModel.ImportType = listOrder.ImportType;

            //viewModel.ImportType = listOrder.ImportType;

            //viewModel.ImportType = listOrder.ImportType;

            return viewModel;
        }

        [HttpPost]
        public async Task<bool> Create(APPurchaseOrderViewModel modelRepo, IFormCollection collection, IFormFile Attachment)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    APPurchaseOrder purchaseOrder = new APPurchaseOrder();
                    purchaseOrder.PONo = modelRepo.PONo;
                    purchaseOrder.PODate = modelRepo.PODate;
                    purchaseOrder.PeriodId = modelRepo.PeriodId;
                    purchaseOrder.Currency = modelRepo.Currency;
                    purchaseOrder.CurrencyExchangeRate = modelRepo.CurrencyExchangeRate;
                    purchaseOrder.POTypeId = modelRepo.POTypeId;
                    purchaseOrder.SupplierId = modelRepo.SupplierId;
                    purchaseOrder.ReferenceNo = modelRepo.ReferenceNo;
                    purchaseOrder.DeliveryTermId = modelRepo.DeliveryTermId;
                    purchaseOrder.PaymentTermId = modelRepo.PaymentTermId;
                    purchaseOrder.Attachment = UploadFile(Attachment);
                    purchaseOrder.Remarks = modelRepo.Remarks;
                    purchaseOrder.Total = modelRepo.Total;
                    purchaseOrder.TotalTaxAmount = modelRepo.TotalTaxAmount;
                    purchaseOrder.Freight = modelRepo.Freight;
                    purchaseOrder.GrandTotal = modelRepo.GrandTotal;
                    //more
                    purchaseOrder.PaymentMode = modelRepo.PaymentMode;
                    purchaseOrder.CostTerms = modelRepo.CostTerms;
                    purchaseOrder.ShippingMode = modelRepo.ShippingMode;
                    purchaseOrder.ShippingPort = modelRepo.ShippingPort;
                    purchaseOrder.DischargePort = modelRepo.DischargePort;
                    purchaseOrder.Origin = modelRepo.Origin;
                    purchaseOrder.ImportType = modelRepo.ImportType;


                    purchaseOrder.CreatedBy = modelRepo.CreatedBy;
                    purchaseOrder.CompanyId = modelRepo.CompanyId;
                    purchaseOrder.CreatedDate = DateTime.Now;
                    purchaseOrder.IsDeleted = false;
                    purchaseOrder.Status = "Created";
                    _dbContext.APPurchaseOrders.Add(purchaseOrder);
                    await _dbContext.SaveChangesAsync();

                    //partialView's data saving in dbContext
                    for (int i = 0; i < collection["ItemId"].Count; i++)
                    {
                        var orderItem = new APPurchaseOrderItem();
                        orderItem.POId = purchaseOrder.Id;
                        orderItem.ItemId = Convert.ToInt32(collection["ItemId"][i]);
                        orderItem.DeliveryDate = Convert.ToDateTime(collection["DeliveryDate"][i]);
                        orderItem.Qty = Convert.ToDecimal(collection["Qty"][i]);
                        orderItem.Rate = Convert.ToDecimal(collection["Rate"][i]);
                        orderItem.Total = Convert.ToDecimal(collection["Total_"][i]);
                        orderItem.TaxId = Convert.ToInt32(collection["TaxId"][i]);
                        orderItem.TaxAmount = Convert.ToDecimal(collection["TaxAmount"][i]);

                      //  orderItem.PKRValue = Convert.ToDecimal(collection["PKRValue"][i]);

                        orderItem.HSCode = Convert.ToInt32(collection["HSCode"][i]);
                        orderItem.NetTotal = Convert.ToDecimal(collection["NetTotal"][i]);
                      //  orderItem.FCValue = Convert.ToInt32(collection["FCValue"][i]);


                        _dbContext.APPurchaseOrderItems.Add(orderItem);
                    }
                    await _dbContext.SaveChangesAsync();
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.InnerException.Message);
                    transaction.Rollback();
                    string message = ex.Message.ToString();
                    return false;
                }
            }
        }
        [HttpPost]
        public async Task<bool> Update(APPurchaseOrderViewModel modelRepo, IFormCollection collection, IFormFile Attachment)
        {
            //for partial-items removal
            string[] idsDeleted = Convert.ToString(collection["IdsDeleted"]).Split(",");
            if (!idsDeleted.Contains(""))
            {
                for (int j = 0; j < idsDeleted.Length; j++)
                {
                    if (idsDeleted[j] != "0")
                    {
                        var itemToRemove = _dbContext.APPurchaseOrderItems.Find(Convert.ToInt32(idsDeleted[j]));
                        itemToRemove.IsDeleted = true;
                        var tracker = _dbContext.APPurchaseOrderItems.Update(itemToRemove);
                        tracker.OriginalValues.SetValues(await tracker.GetDatabaseValuesAsync());
                        await _dbContext.SaveChangesAsync();
                    }
                }
            }
            //updating existing data
            var obj = _dbContext.APPurchaseOrders.Find(modelRepo.Id);
            obj.PONo = modelRepo.PONo;
            obj.PODate = modelRepo.PODate;
            obj.PeriodId = modelRepo.PeriodId;
            obj.Currency = modelRepo.Currency;
            obj.CurrencyExchangeRate = modelRepo.CurrencyExchangeRate;
            obj.POTypeId = modelRepo.POTypeId;
            obj.SupplierId = modelRepo.SupplierId;
            obj.ReferenceNo = modelRepo.ReferenceNo;
            obj.DeliveryTermId = modelRepo.DeliveryTermId;
            obj.PaymentTermId = modelRepo.PaymentTermId;
            obj.Status = "Created";
            obj.Attachment = UploadFile(Attachment);
            obj.Remarks = modelRepo.Remarks;
            obj.Total = modelRepo.Total;
            obj.TotalTaxAmount = modelRepo.TotalTaxAmount;
            obj.Freight = modelRepo.Freight;
            obj.GrandTotal = modelRepo.GrandTotal;

            obj.PaymentMode = modelRepo.PaymentMode;
            obj.CostTerms = modelRepo.CostTerms;
            obj.ShippingMode = modelRepo.ShippingMode;
            obj.ShippingPort = modelRepo.ShippingPort;
            obj.DischargePort = modelRepo.DischargePort;
            obj.Origin = modelRepo.Origin;
            obj.ImportType = modelRepo.ImportType;

            obj.UpdatedBy = modelRepo.UpdatedBy;
            obj.CompanyId = modelRepo.CompanyId;
            obj.UpdatedDate = DateTime.Now;
            obj.IsDeleted = modelRepo.IsDeleted;
            var entry = _dbContext.APPurchaseOrders.Update(obj);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();

            var list = _dbContext.APPurchaseOrderItems.Where(l => l.POId == Convert.ToInt32(collection["Id"])).ToList();
            if (list != null)
            {
                for (int i = 0; i < collection["ItemId"].Count; i++)
                {
                    var orderItem = _dbContext.APPurchaseOrderItems
                        .Where(j => j.POId == modelRepo.Id && j.Id == Convert.ToInt32(collection["PRItemId"][i] == "" ? 0 : Convert.ToInt32(collection["PRItemId"][i]))).FirstOrDefault();
                    // Extract coresponding values from form collection
                    var itemId = Convert.ToInt32(collection["ItemId"][i]);
                    var deliveryDate = Convert.ToDateTime(collection["DeliveryDate"][i]);
                    var qty = Convert.ToDecimal(collection["Qty"][i]);
                    var rate = Convert.ToDecimal(collection["Rate"][i]);
                    var total = Convert.ToDecimal(collection["Total_"][i]);
                    //var lineTotal = Convert.ToDecimal(collection["LineTotal"][i]);
                    var taxId = Convert.ToInt32(collection["TaxId"][i]);
                    var taxAmount = Convert.ToDecimal(collection["TaxAmount"][i]);

                    var PKRValue = Convert.ToDecimal(collection["PKRValue"][i]);

                    var hscode = Convert.ToDecimal(collection["HSCode"][i]);
                    var nettotal = Convert.ToDecimal(collection["NetTotal"][i]);
                    var fcvalue = Convert.ToDecimal(collection["FCValue"][i]);


                    if (orderItem != null && itemId != 0)
                    {
                        var entityEntry = _dbContext.Entry(orderItem);
                        entityEntry.State = EntityState.Modified;
                        entityEntry.Property(p => p.Id).IsModified = false;
                        orderItem.ItemId = itemId;
                        orderItem.POId = modelRepo.Id;
                        orderItem.DeliveryDate = deliveryDate;
                        orderItem.Qty = qty;
                        orderItem.Rate = rate;
                        orderItem.Total = total;
                       // orderItem.PKRValue = PKRValue;
                        orderItem.TaxId = taxId;
                        orderItem.TaxAmount = taxAmount;
                        
                        orderItem.HSCode = Convert.ToInt32(hscode);
                        orderItem.NetTotal = nettotal;
                    //    orderItem.FCValue = Convert.ToInt32(fcvalue);

                        var dbEntry = _dbContext.APPurchaseOrderItems.Update(orderItem);
                        dbEntry.OriginalValues.SetValues(await entityEntry.GetDatabaseValuesAsync());
                    }
                    else if(orderItem == null && itemId != 0)
                    {
                        APPurchaseOrderItem newItem = new APPurchaseOrderItem();
                        newItem.ItemId = itemId;
                        newItem.POId = modelRepo.Id;
                        newItem.DeliveryDate = deliveryDate;
                        newItem.Qty = qty;
                        newItem.Rate = rate;
                        newItem.Total = total;
                       // newItem.PKRValue = PKRValue;
                        newItem.TaxId = taxId;
                        newItem.TaxAmount = taxAmount;

                        newItem.HSCode = Convert.ToInt32(hscode);
                        newItem.NetTotal = nettotal;
                       // newItem.FCValue = Convert.ToInt32(fcvalue);


                        _dbContext.APPurchaseOrderItems.Add(newItem);
                    }
                    await _dbContext.SaveChangesAsync();
                }
            }
                return true;
        }

        public async Task<bool> Delete(int id)
        {
            var purchaseOrderDelete = _dbContext.APPurchaseOrders.Find(id);
            purchaseOrderDelete.IsDeleted = true;
            var entry = _dbContext.APPurchaseOrders.Update(purchaseOrderDelete);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
           await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Approve(int id,string userId)
        {
            try
            {
                var orderApprove = _dbContext.APPurchaseOrders.Find(id);
                orderApprove.Status = "Approved";
                orderApprove.ApprovedBy = userId;
                orderApprove.ApprovedDate = DateTime.Now;
                var entry = _dbContext.Update(orderApprove);
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
        public async Task<bool> UnApproveVoucher(int id)
        {
            try
            {
                var orderApprove = _dbContext.APPurchaseOrders.Find(id);
                orderApprove.Status = "Created";
                orderApprove.ApprovedBy = null;
                orderApprove.ApprovedDate = DateTime.Now;
                var entry = _dbContext.Update(orderApprove);
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

        public APPurchaseOrderViewModel GetOrderItems(int id, int itemId)
        {
            var item = _dbContext.APPurchaseOrderItems.Include(i => i.PO).Include(i => i.Item).Where(i => i.Id == id && i.IsDeleted != true).FirstOrDefault();

            APPurchaseOrderViewModel viewModel = new APPurchaseOrderViewModel();
            viewModel.PRItemId = item.Id;
            viewModel.ItemId = item.ItemId;
            viewModel.Qty = item.Qty;
            viewModel.Rate = item.Rate;
            viewModel.TaxId = item.TaxId;
            viewModel.TaxAmount = item.TaxAmount;
            viewModel.LineTotal = item.LineTotal;
            viewModel.Total_ = item.Total;
            return viewModel;
        }

        public int PurchaseOrderNo(int companyId)
        {
            int maxPoNo = 1;
            var orders = _dbContext.APPurchaseOrders.Where(c => c.CompanyId == companyId).ToList();
            if (orders.Count > 0)
            {
                maxPoNo = orders.Max(o => o.PONo);
                return maxPoNo + 1;
            }
            else
            {
                return maxPoNo;
            }
        }
        
    }
}
