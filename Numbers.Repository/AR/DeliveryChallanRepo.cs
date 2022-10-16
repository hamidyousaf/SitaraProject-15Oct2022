using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Repository.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numbers.Repository.AR
{
    public class DeliveryChallanRepo
    {
        private readonly NumbersDbContext _dbContext;
        public DeliveryChallanRepo(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public string UploadFile(IFormFile img)
        {
            string filesList = "";
            if (img != null)
            {
                if (img.Length > 0)
                {
                    var fileName = Path.GetFileName(img.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads\\deliverychallan-images", fileName);
                    using (var Fstream = new FileStream(filePath, FileMode.Create))
                    {
                        img.CopyTo(Fstream);
                        var fullPath = "/uploads/deliverychallan-images/" + fileName;
                        filesList += fullPath;
                    }
                }
            }
            return filesList;
        }

        public IEnumerable<ARDeliveryChallan> GetAll(int companyId)
        {
            IEnumerable<ARDeliveryChallan> listRepo = _dbContext.ARDeliveryChallans.Include(c => c.Customer).Where(c => c.CompanyId == companyId && c.IsDeleted == false)
                                             .ToList();
            return listRepo;
        }

        public SelectList SaleOrderNo()
        {
            var saleOrderNo = new SelectList(_dbContext.ARSaleOrders.Where(s => s.IsDeleted == false), "Id", "SaleOrderNo");
            return saleOrderNo;
        }

        public ARDeliveryChallanViewModel GetById(int id)
        {
            ARDeliveryChallan deliveryChallan = _dbContext.ARDeliveryChallans.Find(id);
            var viewModel = new ARDeliveryChallanViewModel();
            viewModel.Id = deliveryChallan.Id;
            viewModel.DCNo = deliveryChallan.DCNo;
            viewModel.DCDate = deliveryChallan.DCDate;
            viewModel.ManualDCNo = deliveryChallan.ManualDCNo;
            viewModel.CustomerId = deliveryChallan.CustomerId;
            viewModel.ShipToId = deliveryChallan.ShipToId;
            viewModel.Storemaster = deliveryChallan.Store;
            viewModel.ItemGroupId = deliveryChallan.ItemGroupId;
            viewModel.SalesCategoryId = deliveryChallan.SalesCategoryId;
            viewModel.VehicleNo = deliveryChallan.VehicleNo;
            viewModel.VehicleType = deliveryChallan.VehicleType;
            viewModel.DriverContactNo = deliveryChallan.DriverContactNo;
            viewModel.DriverName = deliveryChallan.DriverName;
            viewModel.Status = deliveryChallan.Status;
            viewModel.TransportCompany = deliveryChallan.TransportCompany;
            viewModel.BuiltyNo = deliveryChallan.BuiltyNo;
            viewModel.Remarks = deliveryChallan.Remarks;
            viewModel.Attachment = deliveryChallan.Attachment;
            viewModel.BranchId = deliveryChallan.BranchId;
            viewModel.CompanyId = deliveryChallan.CompanyId;
            return viewModel;
        }

        [HttpPost]
        public async Task<bool> Create(ARDeliveryChallanViewModel modelRepo, IFormCollection collection, IFormFile Attachment, int companyId, string userId)
        {

            try
            {
                ARDeliveryChallan challan = new ARDeliveryChallan();
                challan.DCNo = this.DeliveryChallanCountNo(companyId);
                challan.DCDate = modelRepo.DCDate;
                challan.ManualDCNo = modelRepo.ManualDCNo;
                challan.CustomerId = modelRepo.CustomerId;
                challan.ShipToId = modelRepo.ShipToId;
                challan.Store = modelRepo.Storemaster;
                challan.ItemGroupId = modelRepo.ItemGroupId;
                challan.SalesCategoryId = modelRepo.SalesCategoryId;
                //challan.DriverContactNo = Convert.ToInt32(modelRepo.DriverContactNo);
                challan.DriverContactNo = modelRepo.DriverContactNo;
                challan.VehicleNo = modelRepo.VehicleNo;
                challan.VehicleType = modelRepo.VehicleType;
                challan.DriverName = modelRepo.DriverName;
                challan.TransportCompany = modelRepo.TransportCompany;
                challan.BuiltyNo = modelRepo.BuiltyNo;
                challan.Remarks = modelRepo.Remarks;
                challan.Attachment = UploadFile(Attachment);
                challan.BranchId = modelRepo.BranchId;
                challan.CreatedBy = modelRepo.CreatedBy;
                challan.CompanyId = modelRepo.CompanyId;
                challan.ResponsibilityId = modelRepo.ResponsibilityId;
                challan.CreatedDate = DateTime.Now;
                challan.IsDeleted = false;
                challan.Status = "Created";
                _dbContext.ARDeliveryChallans.Add(challan);
                await _dbContext.SaveChangesAsync();

                //partialView's data saving in dbContext
                ARDeliveryChallanItem[] delivery = JsonConvert.DeserializeObject<ARDeliveryChallanItem[]>(collection["DeliveryDetails"]);

                foreach (var item in delivery)
                {
                    ARDeliveryChallanItem model = new ARDeliveryChallanItem();
                    model.DeliveryChallanId = challan.Id;
                    model.SaleOrderId = item.SaleOrderId;
                    model.ItemId = item.ItemId;
                    model.BaleId = item.BaleId;
                    model.ItemDescription = item.ItemDescription;
                    model.UnitName = item.UnitName;
                    model.UOM = item.UOM;
                    model.AvailableStock = item.AvailableStock;
                    model.SalesOrderQty = item.SalesOrderQty;
                    model.SaleOrderBalance = item.SaleOrderBalance - Convert.ToDecimal(item.Qty);
                    model.BaleNo = item.BaleNo;
                    model.Qty = Convert.ToInt32(item.Qty);


                    ARSaleOrderItem saleOrderItem = _dbContext.ARSaleOrderItems.FirstOrDefault(x=>x.Id == model.SaleOrderId);
                    saleOrderItem.DCQty = saleOrderItem.DCQty + Convert.ToInt32(model.Qty);
                    _dbContext.ARSaleOrderItems.Update(saleOrderItem);
                    
                    BaleInformation baleInformation = _dbContext.BaleInformation.FirstOrDefault(x => x.BaleNumber == model.BaleNo && x.UsedFNumber == false);
                    baleInformation.UsedFNumber = true;
                    _dbContext.BaleInformation.Update(baleInformation);
                    //model.ItemId = item.ItemId;
                    //model.ItemDescription = item.ItemDescription;
                    //model.Qty = item.Qty;
                    //model.SaleOrderId = item.SaleOrderId;
                    //model.StockInStore = item.StockInStore;
                    //model.SalesOrderQty = item.SalesOrderQty;
                    //model.DCBalance = item.DCBalance;
                    //model.UnitName = item.UnitName;
                    //model.UOM = item.UnitName;
                    //model.IsDeleted = false;
                    //model.CreatedBy = userId;
                    //model.CompanyId = companyId;
                    //model.CreatedDate = DateTime.Now;
                    _dbContext.ARDeliveryChallanItems.Add(model);
                    _dbContext.SaveChanges();

                    InvItem inv = _dbContext.InvItems.Find(model.ItemId);
                    var a = inv.StockQty;
                    inv.StockQty = a - model.Qty;
                    _dbContext.InvItems.Update(inv);
                    _dbContext.SaveChanges();

                    var orderItems = _dbContext.ARSaleOrderItems.Where(x => x.Id == model.SaleOrderId).FirstOrDefault();
                    orderItems.SaleQty = orderItems.SaleQty + model.Qty;
                    _dbContext.ARSaleOrderItems.Update(orderItems);
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
        public async Task<bool> Update(ARDeliveryChallanViewModel modelRepo, IFormCollection collection, IFormFile Attachment, int companyId, string userId)
        {
            var obj = _dbContext.ARDeliveryChallans.Find(modelRepo.Id);
            obj.DCNo = modelRepo.DCNo;
            obj.DCDate = modelRepo.DCDate;
            obj.ManualDCNo = modelRepo.ManualDCNo;
            obj.CustomerId = modelRepo.CustomerId;
            obj.ShipToId = modelRepo.ShipToId;
            obj.Store = modelRepo.Storemaster;
            obj.ItemGroupId = modelRepo.ItemGroupId;
            obj.SalesCategoryId = modelRepo.SalesCategoryId;
            obj.DriverContactNo = modelRepo.DriverContactNo;
            obj.VehicleNo = modelRepo.VehicleNo;
            obj.VehicleType = modelRepo.VehicleType;
            obj.DriverName = modelRepo.DriverName;
            obj.TransportCompany = modelRepo.TransportCompany;
            obj.BuiltyNo = modelRepo.BuiltyNo;
            obj.Remarks = modelRepo.Remarks;
            obj.Attachment = UploadFile(Attachment);
            obj.BranchId = modelRepo.BranchId;
            obj.UpdatedBy = modelRepo.UpdatedBy;
            obj.CompanyId = modelRepo.CompanyId;
            obj.ResponsibilityId = modelRepo.ResponsibilityId;
            obj.UpdatedDate = DateTime.Now;
            obj.IsDeleted = modelRepo.IsDeleted;
            obj.Status = modelRepo.Status;
            var entry = _dbContext.ARDeliveryChallans.Update(obj);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();

            var EditList = new List<int>();
            for (int i = 0; i < collection["DetailId"].Count; i++)
            {
                try
                {
                    int id = Convert.ToInt32(collection["DetailId"][i]);
                    EditList.Add(id);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

            }
            var list = _dbContext.ARDeliveryChallanItems.Where(l => l.DeliveryChallanId == obj.Id).ToList();
            foreach (var detail in list)
            {
                bool result = EditList.Exists(x => x == detail.Id);
                if (!result)
                {
                    var item = _dbContext.ARDeliveryChallanItems.Where(x => x.Id == detail.Id).FirstOrDefault();
                    if (item != null)
                    {

                        BaleInformation baleInformation = _dbContext.BaleInformation.FirstOrDefault(x => x.BaleNumber == item.BaleNo && x.UsedFNumber != false);
                        if (baleInformation != null)
                        {
                            baleInformation.UsedFNumber = false;
                            _dbContext.BaleInformation.Update(baleInformation);
                        }
                    }
                    _dbContext.ARDeliveryChallanItems.Remove(item);

                }
            }
            if (list != null)
            {
                for (int i = 0; i < collection["DetailId"].Count; i++)
                {
                    
                    var orderItem = _dbContext.ARDeliveryChallanItems
                            .Where(j => j.DeliveryChallanId == modelRepo.Id && j.Id == Convert.ToInt32(collection["DetailId"][i] == "" ? 0 : Convert.ToInt32(collection["DetailId"][i]))).FirstOrDefault();
                    // Extract coresponding values from form collection
                    var detailId = Convert.ToInt32(collection["DetailId"][i]);
                    var SaleOrderItemId = Convert.ToInt32(collection["SaleOrderItemId"][i]);
                    var ItemId = Convert.ToInt32(collection["ItemId"][i]);
                    var BaleId = Convert.ToInt32(collection["BaleId"][i]);
                    var ItemDesc = Convert.ToString(collection["ItemDesc"][i]);
                    var UOM = Convert.ToString(collection["UOM"][i]);
                    var AvailableStock = Convert.ToInt32(collection["AvailableStock"][i]);
                    //var SaleOrderQty = Convert.ToInt32(collection["SaleOrderQty"][i]);
                    //var BalanceStock = Convert.ToDecimal(collection["BalanceStock"][i]);
                    var BaleNo = Convert.ToString(collection["BaleNo"][i]);
                    var DCQuantity = Convert.ToDecimal(collection["DCQuantity"][i]);

                    BaleInformation baleInformation = _dbContext.BaleInformation.FirstOrDefault(x=>x.BaleNumber == BaleNo && x.UsedFNumber != true);
                    if (baleInformation != null)
                    {
                        baleInformation.UsedFNumber = true;
                        _dbContext.BaleInformation.Update(baleInformation);
                    }

                    if (orderItem != null && detailId != 0)
                    {
                        orderItem.DeliveryChallanId = obj.Id;
                        orderItem.SaleOrderId = SaleOrderItemId;
                        orderItem.ItemId = ItemId;
                        orderItem.BaleId = BaleId;
                        orderItem.ItemDescription = ItemDesc;
                        orderItem.UnitName = UOM;
                        orderItem.UOM = UOM;
                        orderItem.AvailableStock = AvailableStock;
                        //orderItem.SalesOrderQty = SaleOrderQty;
                        //orderItem.SaleOrderBalance = BalanceStock;
                        orderItem.BaleNo = BaleNo;
                        orderItem.Qty = Convert.ToInt32(DCQuantity);
                        var dbEntry = _dbContext.ARDeliveryChallanItems.Update(orderItem);
                    }
                    else if (orderItem == null && detailId == 0)
                    {
                        ARDeliveryChallanItem newItem = new ARDeliveryChallanItem();
                        newItem.DeliveryChallanId = obj.Id;
                        newItem.SaleOrderId = SaleOrderItemId;
                        newItem.ItemId = ItemId;
                        newItem.BaleId = BaleId;
                        newItem.ItemDescription = ItemDesc;
                        newItem.UnitName = UOM;
                        newItem.UOM = UOM;
                        newItem.AvailableStock = AvailableStock;
                        //newItem.SalesOrderQty = SaleOrderQty;
                        //newItem.SaleOrderBalance = BalanceStock;
                        newItem.BaleNo = BaleNo;
                        newItem.Qty = Convert.ToInt32(DCQuantity);
                        _dbContext.ARDeliveryChallanItems.Add(newItem);
                    }
                    await _dbContext.SaveChangesAsync();
                }
            }

           return true;
        }

        public async Task<bool> Delete(int id)
        {
            var challanDelete = _dbContext.ARDeliveryChallans.Find(id);
            var challandelete = _dbContext.ARContactPerson.Find(id);
            challanDelete.IsDeleted = true;
            var entry = _dbContext.ARDeliveryChallans.Update(challanDelete);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Approve(int id, string userId)
        {
            try
            {
                var orderApprove = _dbContext.ARDeliveryChallans.Find(id);
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

        public int DeliveryChallanCountNo(int companyId)
        {
            int maxDeliveryChallanNo = 1;
            var orders = _dbContext.ARDeliveryChallans.Where(c => c.CompanyId == companyId).ToList();
            if (orders.Count > 0)
            {
                maxDeliveryChallanNo = orders.Max(o => o.DCNo);
                return maxDeliveryChallanNo + 1;
            }
            else
            {
                return maxDeliveryChallanNo;
            }
        }
        public IEnumerable<ARSaleOrderItem> GetSOItems()
        {
            IEnumerable<ARSaleOrderItem> list = _dbContext.ARSaleOrderItems.Include(i => i.Item).Include(i => i.SaleOrder).Where(i => i.IsDeleted == false).ToList();
            return list;
        }
        public IEnumerable<ARDeliveryChallanItem> GetDCItems(int id)
        {
            IEnumerable<ARDeliveryChallanItem> list = _dbContext.ARDeliveryChallanItems.Include(i => i.Item).Include(i => i.ARSaleOrderItem).ThenInclude(p=>p.SaleOrder).Where(i => i.IsDeleted == false && i.DeliveryChallanId == id).ToList();
            return list;
        }
        public ARDeliveryChallanViewModel GetDCItem(int id)
        {
            ARDeliveryChallanItem item = _dbContext.ARDeliveryChallanItems.Include(i => i.Item).Include(i => i.ARSaleOrderItem).ThenInclude(p=>p.SaleOrder).Where(i => i.IsDeleted == false && i.Id == id).FirstOrDefault();
            ARDeliveryChallanViewModel viewModel = new ARDeliveryChallanViewModel();
            viewModel.SaleOrderId = item.ARSaleOrderItem.SaleOrder.SaleOrderNo;
            viewModel.ExpiryDate = item.ARSaleOrderItem.SaleOrder.SaleOrderDate;
            viewModel.Qty = item.Qty;
            viewModel.SaleOrderBalance = item.SaleOrderBalance;
            viewModel.Bonus = item.Bonus;
            viewModel.PRItemId = item.Id;
            viewModel.ItemId = item.ItemId;
            viewModel.DCBalance = item.DCBalance;
            viewModel.StockInStore = item.StockInStore;
            return viewModel;
        }
        public ARDeliveryChallanViewModel GetSOItem(int id)
        {
            var item = _dbContext.ARSaleOrderItems.Include(i => i.Item).Include(i => i.SaleOrder).Where(i => i.Id == id && i.IsDeleted == false).FirstOrDefault();

            ARDeliveryChallanViewModel viewModel = new ARDeliveryChallanViewModel();
            viewModel.SaleOrderId = item.SaleOrder.SaleOrderNo;
            viewModel.ExpiryDate = item.SaleOrder.SaleOrderDate;
            viewModel.PRItemId = item.Id;
            viewModel.Qty = item.Qty;
            viewModel.ItemId = item.ItemId;
            return viewModel;
        }
        public int MaxDeliveryChallan(int companyId)
        {
            int max = _dbContext.ARDeliveryChallans.Where(x => x.CompanyId == companyId).Max(x => x.DCNo);
            return max;
        }
    }
}