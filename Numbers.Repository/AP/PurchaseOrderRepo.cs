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
    public class PurchaseOrderRepo
    {
        private readonly NumbersDbContext _dbContext;
        public PurchaseOrderRepo(NumbersDbContext dbContext)
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
            IEnumerable<APPurchaseOrder> listRepo = _dbContext.APPurchaseOrders.Include(p=>p.APPurchaseOrderItems).Include(c=>c.Supplier).Where(c => c.CompanyId == companyId && c.IsDeleted==false).ToList();
            return listRepo;
        }

        public APPurchaseOrderItem[] GetPurchaseOrderItems(int id)
        {
            APPurchaseOrderItem[] items = _dbContext.APPurchaseOrderItems.Include(x=>x.UOMName).Where(i => i.POId == id && i.IsDeleted == false).ToArray();
            return items;
        }

        public APPurchaseOrderViewModel GetById(int id)
        {
            APPurchaseOrder listOrder = _dbContext.APPurchaseOrders.Include(x=>x.Operation).FirstOrDefault(x=>x.Id == id);
            var viewModel = new APPurchaseOrderViewModel();
            viewModel.Id = listOrder.Id;
            viewModel.PONo = listOrder.PONo;
            viewModel.PODate = listOrder.PODate;
            viewModel.Currency = listOrder.Currency;
            viewModel.CurrencyExchangeRate = listOrder.CurrencyExchangeRate;
            viewModel.POTypeId = listOrder.POTypeId;
            viewModel.SupplierId = listOrder.SupplierId;
            viewModel.OrganizationId = listOrder.OrganizationId;
            viewModel.OperationId = listOrder.OperationId;
            viewModel.DepartmentId = listOrder.DepartmentId;
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
                    purchaseOrder.CurrencyExchangeRate = modelRepo.CurrencyExchangeRate;
                    purchaseOrder.PerformaNo = Convert.ToInt32(collection["PerformaNo"]);
                    purchaseOrder.PaymentMode = modelRepo.PaymentMode;
                    purchaseOrder.CostTerms = modelRepo.CostTerms;
                    purchaseOrder.ShippingMode = modelRepo.ShippingMode;
                    purchaseOrder.ShippingPort = modelRepo.ShippingPort;
                    purchaseOrder.DischargePort = modelRepo.DischargePort;
                    purchaseOrder.Origin = modelRepo.Origin;
                    purchaseOrder.ImportType = modelRepo.ImportType;
                    purchaseOrder.CostCenter = modelRepo.CostCenter;
                    purchaseOrder.OrganizationId = modelRepo.OrganizationId;
                    purchaseOrder.DepartmentId = modelRepo.DepartmentId;
                    purchaseOrder.OperationId = modelRepo.OperationId;
                    purchaseOrder.CreatedBy = modelRepo.CreatedBy;
                    purchaseOrder.CompanyId = modelRepo.CompanyId;
                    purchaseOrder.CreatedDate = DateTime.Now;
                    purchaseOrder.IsDeleted = false;
                    purchaseOrder.Status = "Created";
                    _dbContext.APPurchaseOrders.Add(purchaseOrder);
                    await _dbContext.SaveChangesAsync();
                    //partialView's data saving in dbContext
                    for (int i = 0; i < collection["ItemCode"].Count; i++)
                    {
                        var orderItem = new APPurchaseOrderItem();
                        var date = Convert.ToString(collection["DeliveryDate"][i]);
                        orderItem.POId = purchaseOrder.Id;
                        orderItem.PrNo = Convert.ToInt32(collection["PrNo"][i]);
                        orderItem.ItemId = Convert.ToInt32(collection["ItemId"][i]);
                        orderItem.PrDetailId = Convert.ToInt32(collection["PrDetailId"][i]);
                        orderItem.ItemCode = Convert.ToString(collection["ItemCode"][i]);
                        orderItem.ItemDescription = Convert.ToString(collection["ItemDescription"][i]);
                        orderItem.UOM = Convert.ToInt32(collection["UOM"][i]);
                        //orderItem.IndentNo = Convert.ToInt32(collection["IndentNo"][i]);
                        orderItem.Qty = Convert.ToDecimal(collection["IndentQty"][i]);
                        orderItem.Rate = Convert.ToDecimal(collection["Rate"][i]);
                        orderItem.Value = Convert.ToDecimal(collection["Value"][i]);
                        orderItem.Total = Convert.ToDecimal(collection["Value"][i]);
                        orderItem.TaxId = Convert.ToInt32(collection["TaxId"][i]);
                        orderItem.TaxAmount = Convert.ToDecimal(collection["TaxAmount"][i]);
                        orderItem.PRReferenceNo = Convert.ToString(collection["PRRefrenceNo"][i]);
                   
                        orderItem.FedPercentage = Convert.ToDecimal(collection["FedPercentage"][i]);
                        orderItem.FedAmount = Convert.ToDecimal(collection["AmountWFed"][i]);

                        //orderItem.DeliveryDate = Convert.ToDateTime(collection["DeliveryDate"][i]);
                        orderItem.Remarks = Convert.ToString(collection["Remark"][i]);
                        orderItem.LineTotal = Convert.ToDecimal(collection["Total"][i]);
                        orderItem.NetTotal = Convert.ToDecimal(collection["Total"][i]);
                        orderItem.IGPBalc = Convert.ToDecimal(collection["IndentQty"][i]);
                        if (Convert.ToInt32(modelRepo.PerformaNo) > 0) 
                        {
                            var a = Convert.ToString(collection["OriginID"][i]);
                            orderItem.Category = Convert.ToInt32(collection["CategoryId"][i]);
                            orderItem.HSCode = Convert.ToInt32(collection["HSCode"][i]);
                            orderItem.FCValue = Convert.ToDecimal(collection["FCValue"][i]);
                            orderItem.Origin = Convert.ToInt32(collection["OriginID"][i]);
                            orderItem.IsImport = true;
                        }
                      //  orderItem.FCValue = Convert.ToDecimal(collection["FCValue"][i]);
                      // orderItem.DetailCostCenter = Convert.ToInt32(collection["DetailCostCenter"][i]);
                        orderItem.DetailCostCenter = 0;
                        if (orderItem.DetailCostCenter == 0)
                        {
                            orderItem.DetailCostCenter = purchaseOrder.CostCenter;
                        }
                        //var item = _APPurchaseRequisitionDetailsRepository.Get(x => x.Id == Convert.ToInt32(collection["PrDetailId"][i])).ToList();
                        var item = (from e in _dbContext.APPurchaseRequisitionDetails where e.Id == Convert.ToInt32(collection["PrDetailId"][i]) select e).FirstOrDefault();
                        item.IsPOCreated = true;
                        _dbContext.APPurchaseRequisitionDetails.Update(item);
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
            string module = (from c in _dbContext.Sys_Responsibilities where c.Resp_Id == modelRepo.Resp_ID select c.Resp_Name).FirstOrDefault();

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
            obj.CostCenter = modelRepo.CostCenter;
            obj.OrganizationId = modelRepo.OrganizationId;
            obj.DepartmentId = modelRepo.DepartmentId;
            obj.OperationId = modelRepo.OperationId;
            obj.PaymentMode = modelRepo.PaymentMode;
            obj.CostTerms = modelRepo.CostTerms;
            obj.ShippingMode = modelRepo.ShippingMode;
            obj.ShippingPort = modelRepo.ShippingPort;
            obj.DischargePort = modelRepo.DischargePort;
            obj.Origin = modelRepo.Origin;
            obj.ImportType = modelRepo.ImportType;
            obj.PerformaNo= Convert.ToInt32(collection["PerformaNo"]);
            obj.UpdatedBy = modelRepo.UpdatedBy;
            obj.CompanyId = modelRepo.CompanyId;
            obj.UpdatedDate = DateTime.Now;
            obj.IsDeleted = modelRepo.IsDeleted;
            obj.POTypeId = modelRepo.POTypeId;
            var entry = _dbContext.APPurchaseOrders.Update(obj);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();

            var list = _dbContext.APPurchaseOrderItems.Where(l => l.POId == modelRepo.Id).ToList();
            if (list != null)
            {
                for (int i = 0; i < collection["ItemCode"].Count; i++)
                {
                    var orderItem = _dbContext.APPurchaseOrderItems
                        .Where(j => j.POId == modelRepo.Id && j.Id == Convert.ToInt32(collection["POItemId"][i] == "" ? 0 : Convert.ToInt32(collection["POItemId"][i]))).FirstOrDefault();
                    // Extract coresponding values from form collection
                    var itemId = Convert.ToString(collection["ItemCode"][i]);

                    if (orderItem != null && itemId != null)
                    {
                        var entityEntry = _dbContext.Entry(orderItem);
                        entityEntry.State = EntityState.Modified;
                        entityEntry.Property(p => p.Id).IsModified = false;
                        var date = Convert.ToString(collection["DeliveryDate"][i]);
                        orderItem.POId = obj.Id;
                        orderItem.PrNo = Convert.ToInt32(collection["PrNo"][i]);
                        orderItem.ItemId = Convert.ToInt32(collection["ItemId"][i]);
                        orderItem.BrandId = module == "Yarn Purchase" ? Convert.ToInt32(collection["BrandId"][i]) : 0;
                        orderItem.ItemCode = Convert.ToString(collection["ItemCode"][i]);
                        orderItem.ItemDescription = Convert.ToString(collection["ItemDescription"][i]);
                        orderItem.UOM = Convert.ToInt32(collection["UOM"][i]);
                       // orderItem.IndentNo = Convert.ToInt32(collection["IndentNo"][i]);
                        orderItem.Qty = Convert.ToDecimal(collection["IndentQty"][i]);
                        orderItem.IGPBalc = Convert.ToDecimal(collection["IndentQty"][i]);

                        orderItem.Rate = Convert.ToDecimal(collection["Rate"][i]);
                        orderItem.Value = Convert.ToDecimal(collection["Value"][i]);
                        orderItem.Total = Convert.ToDecimal(collection["Total"][i]);
                        // orderItem.TaxId = Convert.ToInt32(collection["TaxId"][i]);
                        var tax = collection["TaxId"][i];
                        int taxId = 0;
                        if (tax != "Select...")
                        {
                            taxId = Convert.ToInt32(tax);
                        }
                        orderItem.TaxId = taxId;
                        orderItem.TaxAmount = Convert.ToDecimal(collection["TaxAmount"][i]);

                        orderItem.FedPercentage = Convert.ToDecimal(collection["FedPercentage"][i]);
                        orderItem.FedAmount = Convert.ToDecimal(collection["AmountWFed"][i]);

                        //orderItem.DeliveryDate = Convert.ToDateTime(collection["DeliveryDate"][i]);
                        orderItem.Remarks = Convert.ToString(collection["Remark"][i]);
                        orderItem.NetTotal = Convert.ToDecimal(collection["Total"][i]);
                        orderItem.LineTotal = Convert.ToDecimal(collection["Total"][i]);
                        //  orderItem.FCValue = Convert.ToDecimal(collection["FCValue"][i]);
                        // orderItem.DetailCostCenter = Convert.ToInt32(collection["DetailCostCenter"][i]);
                        orderItem.DetailCostCenter = 0;
                        if (orderItem.DetailCostCenter == 0)
                        {
                            orderItem.DetailCostCenter = obj.CostCenter;
                        }
                        if (Convert.ToInt32(modelRepo.PerformaNo) > 0)
                        {
                            orderItem.Category = Convert.ToInt32(collection["Categoryid"][i]);
                            orderItem.HSCode = Convert.ToInt32(collection["Hscode"][i]);
                            orderItem.FCValue = Convert.ToDecimal(collection["FCvalue"][i]);
                            orderItem.Origin = Convert.ToInt32(collection["Originid"][i]);
                            orderItem.IsImport = true;
                        }

                        var dbEntry = _dbContext.APPurchaseOrderItems.Update(orderItem);
                        dbEntry.OriginalValues.SetValues(await entityEntry.GetDatabaseValuesAsync());
                    }
                    else if(orderItem == null && itemId != null)
                    {
                        APPurchaseOrderItem newItem = new APPurchaseOrderItem();
                        var date = Convert.ToString(collection["DeliveryDate"][i]);
                        newItem.POId = modelRepo.Id;
                        newItem.PrNo = Convert.ToInt32(collection["PrNo"][i]);
                        newItem.ItemId = Convert.ToInt32(collection["ItemId"][i]);
                        newItem.ItemCode = Convert.ToString(collection["ItemCode"][i]);
                        newItem.ItemDescription = Convert.ToString(collection["ItemDescription"][i]);
                        newItem.UOM = Convert.ToInt32(collection["UOM"][i]);
                        newItem.IndentNo = Convert.ToInt32(collection["IndentNo"][i]);
                        newItem.Qty = Convert.ToDecimal(collection["IndentQty"][i]);
                        newItem.IGPBalc = Convert.ToDecimal(collection["IndentQty"][i]);
                        newItem.Rate = Convert.ToDecimal(collection["Rate"][i]);
                        newItem.Value = Convert.ToDecimal(collection["Value"][i]);
                        newItem.Total = Convert.ToDecimal(collection["Total"][i]);
                        var tax = collection["TaxId"][i];
                        int taxId = 0;
                        if (tax != "Select...")
                        {
                            taxId = Convert.ToInt32(tax);
                        }
                        newItem.TaxId = taxId;
                        newItem.TaxAmount = Convert.ToDecimal(collection["TaxAmount"][i]);

                        newItem.FedPercentage = Convert.ToDecimal(collection["FedPercentage"][i]);
                        newItem.FedAmount = Convert.ToDecimal(collection["AmountWFed"][i]);

                        //orderItem.DeliveryDate = Convert.ToDateTime(collection["DeliveryDate"][i]);
                        newItem.Remarks = Convert.ToString(collection["Remark"][i]);
                        newItem.NetTotal = Convert.ToDecimal(collection["Total"][i]);
                        //  orderItem.FCValue = Convert.ToDecimal(collection["FCValue"][i]);
                        // orderItem.DetailCostCenter = Convert.ToInt32(collection["DetailCostCenter"][i]);
                        newItem.DetailCostCenter = 0;
                        if (newItem.DetailCostCenter == 0)
                        {
                            newItem.DetailCostCenter = modelRepo.CostCenter;
                        }
                        if (Convert.ToInt32(modelRepo.PerformaNo) > 0)
                        {
                            newItem.Category = Convert.ToInt32(collection["CategoryId"][i]);
                            newItem.HSCode = Convert.ToInt32(collection["HSCode"][i]);
                            newItem.FCValue = Convert.ToDecimal(collection["FCValue"][i]);
                            newItem.Origin = Convert.ToInt32(collection["OriginID"][i]);
                            newItem.IsImport = true;
                        }
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
            /*var CSNo = purchaseOrderDelete.CSNo;
            if (CSNo != 0)
            {
                var 
            }*/
            purchaseOrderDelete.IsDeleted = true;
            var entry = _dbContext.APPurchaseOrders.Update(purchaseOrderDelete);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
           await _dbContext.SaveChangesAsync();
            return true;
        } 
        public async Task<bool> ClosePO(int id)
        {
            var purchaseOrderDelete = _dbContext.APPurchaseOrders.Find(id);
            purchaseOrderDelete.Status = "Closed";
            var entry = _dbContext.APPurchaseOrders.Update(purchaseOrderDelete);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
           await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> ClosePOItems(int id)
        {
            
            var purchaseOrderItem = _dbContext.APPurchaseOrderItems.Find(id);

            purchaseOrderItem.IsClosed = true;
            var entry = _dbContext.APPurchaseOrderItems.Update(purchaseOrderItem);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
           await _dbContext.SaveChangesAsync();

            var checkPOItemClosed = _dbContext.APPurchaseOrderItems.Where(x=>x.POId==purchaseOrderItem.POId && x.IsClosed!=true && x.IsDeleted==false).ToList();
            if(checkPOItemClosed.Count==0)
            {
                await ClosePO(purchaseOrderItem.POId);
            }
            return true;
        }

        public async Task<string> Approve(int id,string userId)
        {
            try
            {
                var orderApprove = _dbContext.APPurchaseOrders.Find(id);
                var isImport = _dbContext.APPurchaseOrderItems.Where(x => x.POId == orderApprove.Id).FirstOrDefault();
                if (isImport.IsImport)
                {
                    if (isImport.Origin == 0)
                        return "Origin";
                    if(isImport.HSCode == 0)
                        return "HSCode";
                    else
                    {
                        orderApprove.Status = "Approved";
                        orderApprove.ApprovedBy = userId;
                        orderApprove.ApprovedDate = DateTime.Now;
                        var entry = _dbContext.Update(orderApprove);
                        entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                        await _dbContext.SaveChangesAsync();
                        return "Saved";
                    }
                }
                else
                {
                    orderApprove.Status = "Approved";
                    orderApprove.ApprovedBy = userId;
                    orderApprove.ApprovedDate = DateTime.Now;
                    var entry = _dbContext.Update(orderApprove);
                    entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                    await _dbContext.SaveChangesAsync();
                    return "Saved";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                string message = ex.Message.ToString();
                return "Error";
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
            viewModel.HSCode = item.HSCode;
            viewModel.NetTotal = item.NetTotal;
          //  viewModel.FCValue = item.FCValue;
          // viewModel.PKRValue = item.PKRValue;
            viewModel.Total_ = item.Total;
            viewModel.DetailCostCenter = item.DetailCostCenter;
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
