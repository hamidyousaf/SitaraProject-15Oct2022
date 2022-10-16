using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numbers.Repository.Inventory
{
    public class StockTransferRepo
    {
        private HttpContext HttpContext;
        private readonly NumbersDbContext _dbContext;
        public StockTransferRepo(NumbersDbContext context)
        {
            _dbContext = context;
        }
        public StockTransferRepo(NumbersDbContext context,HttpContext httpContext)
        {
            _dbContext = context;
            HttpContext=httpContext;
        }

        public IEnumerable<InvStockTransfer> GetAll(int companyId)
        {
            IEnumerable<InvStockTransfer> list = _dbContext.InvStockTransfers.Include(s => s.WareHouseTo).Include(s=>s.WareHouseFrom).Where(s => s.IsDeleted == false && s.CompanyId == companyId)
                .ToList();
            return list;
        }

        public InvStockTransferItem[] GetStockTransferItems(int id)
        {
            InvStockTransferItem[] storeIssueItems = _dbContext.InvStockTransferItems.Where(i => i.StockTransferId == id && i.IsDeleted == false).ToArray();
            return storeIssueItems;
        }

        public InvStockTransferViewModel GetById(int id)
        {
            InvStockTransfer storeIssue = _dbContext.InvStockTransfers.Find(id);
            var viewModel = new InvStockTransferViewModel();
            viewModel.TransferNo = storeIssue.TransferNo;
            viewModel.TransferDate = storeIssue.TransferDate;
            viewModel.WareHouseToId = storeIssue.WareHouseToId;
            viewModel.WareHouseFromId = storeIssue.WareHouseFromId;
            viewModel.Remarks = storeIssue.Remarks;
            viewModel.Status = storeIssue.Status;
            return viewModel;
        }

        [HttpPost]
        public async Task<bool> Create(InvStockTransferViewModel model, IFormCollection collection)
        {
            try
            {
                //for master table
                var stockTransfer = new InvStockTransfer();
                stockTransfer.TransferNo = model.TransferNo;
                stockTransfer.TransferDate = model.TransferDate;
                stockTransfer.WareHouseToId = model.WareHouseToId;
                stockTransfer.WareHouseFromId = model.WareHouseFromId;
                stockTransfer.Remarks = (collection["Remarks"][0]);
                stockTransfer.CompanyId = model.CompanyId;
                stockTransfer.IsDeleted = false;
                stockTransfer.Status = "Created";
                stockTransfer.CreatedBy = model.CreatedBy;
                stockTransfer.CreatedDate = DateTime.Now;
                _dbContext.InvStockTransfers.Add(stockTransfer);
                await _dbContext.SaveChangesAsync();
                //for detail table
                for (int i = 0; i < collection["ItemId"].Count; i++)
                {
                    var stockTransferItem = new InvStockTransferItem();
                    stockTransferItem.StockTransferId = stockTransfer.Id;
                    stockTransferItem.ItemId = Convert.ToInt32(collection["ItemId"][i]);
                    stockTransferItem.Qty = Convert.ToInt32(collection["Qty"][i]);
                    stockTransferItem.IsDeleted = false;
                    stockTransferItem.CreatedBy = model.CreatedBy;
                    stockTransferItem.CreatedDate = DateTime.Now;
                    _dbContext.InvStockTransferItems.Add(stockTransferItem);
                    await _dbContext.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                return false;
            }
        }
        [HttpPost]
        public async Task<bool> Update(InvStockTransferViewModel model, IFormCollection collection)
        {
            //for partial-items removal
            string[] idsDeleted = Convert.ToString(collection["IdsDeleted"]).Split(",");
            if (!idsDeleted.Contains(""))
            {
                for (int j = 0; j < idsDeleted.Length; j++)
                {
                    var itemToRemove = _dbContext.InvStockTransferItems.Find(Convert.ToInt32(idsDeleted[j]));
                    itemToRemove.IsDeleted = true;
                    var tracker = _dbContext.InvStockTransferItems.Update(itemToRemove);
                    tracker.OriginalValues.SetValues(await tracker.GetDatabaseValuesAsync());
                    await _dbContext.SaveChangesAsync();
                }
            }
            //updating existing data
            var obj = _dbContext.InvStockTransfers.Find(model.Id);
            obj.TransferNo = model.TransferNo;
            obj.TransferDate = model.TransferDate;
            obj.WareHouseToId = model.WareHouseToId;
            obj.WareHouseFromId = model.WareHouseFromId;
            obj.Remarks = collection["Remarks"][0];
            //obj.Status = "Created";
            obj.UpdatedBy = model.UpdatedBy;
            obj.CompanyId = model.CompanyId;
            obj.UpdatedDate = DateTime.Now;
            obj.IsDeleted = false;
            var entry = _dbContext.InvStockTransfers.Update(obj);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();

            var list = _dbContext.InvStockTransferItems.Where(l => l.StockTransferId == model.Id).ToList();
            if (list != null)
            {
                for (int i = 0; i < collection["ItemId"].Count; i++)
                {
                    var transferItem = _dbContext.InvStockTransferItems
                       .Where(j => j.StockTransferId == model.Id && j.Id == Convert.ToInt32(collection["StockTransferItemId"][i] == "" ? 0 : Convert.ToInt32(collection["StockTransferItemId"][i]))).FirstOrDefault();
                    // Extract coresponding values from form-collection
                    var itemId = Convert.ToInt32(collection["ItemId"][i]);
                    var qty = Convert.ToDecimal(collection["Qty"][i]);
                    var remarks = collection["Remarks"][i + 1];
                    if (transferItem != null && itemId != 0)
                    {
                        //below phenomenon prevents Id from being marked as modified
                        var entityEntry = _dbContext.Entry(transferItem);
                        entityEntry.State = EntityState.Modified;
                        entityEntry.Property(p => p.Id).IsModified = false;
                        transferItem.ItemId = itemId;
                        transferItem.StockTransferId = obj.Id;
                        transferItem.Qty = qty;
                        transferItem.Remarks = remarks;
                        transferItem.UpdatedBy = model.UpdatedBy;
                        transferItem.UpdatedDate = model.UpdatedDate;
                        var dbEntry = _dbContext.InvStockTransferItems.Update(transferItem);
                        dbEntry.OriginalValues.SetValues(await entityEntry.GetDatabaseValuesAsync());
                    }
                    else if (itemId != 0 && transferItem == null)
                    {
                        var newItem = new InvStockTransferItem();
                        newItem.ItemId = itemId;
                        newItem.StockTransferId = obj.Id;
                        newItem.Qty = qty;
                        newItem.Remarks = remarks;
                        newItem.UpdatedBy = model.UpdatedBy;
                        newItem.UpdatedDate = model.UpdatedDate;
                        _dbContext.InvStockTransferItems.Add(newItem);
                    }
                    await _dbContext.SaveChangesAsync();
                }
            }
            return true;
        }

        public dynamic GetStockTransferItems(int id, int itemId)
        {
            var item = _dbContext.InvStockTransferItems.Include(i => i.Item).Where(i => i.Id == id && i.IsDeleted == false).FirstOrDefault();
            InvStockTransferViewModel viewModel = new InvStockTransferViewModel();
            viewModel.ItemId = item.ItemId;
            viewModel.Qty = item.Qty;
            viewModel.StockTransferItemId = item.Id;
            viewModel.Remarks = item.Remarks;
            return viewModel;
        }

        public async Task<bool> Approve(int id, string userId)
        {
            try
            {
                var itemApprove = _dbContext.InvStockTransfers.Find(id);
                itemApprove.Status = "Approved";
                itemApprove.ApprovedBy = userId;
                itemApprove.ApprovedDate = DateTime.Now;
                var entry = _dbContext.InvStockTransfers.Update(itemApprove);
                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                return false;
            }
        }
        public IEnumerable<InvStockTransfer> GetApproved()
        {
            var list = _dbContext.InvStockTransfers.Include(s=>s.WareHouseFrom).Include(s=>s.WareHouseTo).Where(s => s.IsDeleted == false && s.Status == "Approved" && s.CompanyId == HttpContext.Session.GetInt32("CompanyId").Value).AsEnumerable();
            return list;
        }
        public async Task<Dictionary<string,string>>UnApprove(int id)
        {
            Dictionary<string, string> response = new Dictionary<string, string>();
            try
            {
                var itemApprove = _dbContext.InvStockTransfers.Find(id);
                itemApprove.Status = "Created";
                itemApprove.ApprovedBy = null;
                itemApprove.ApprovedDate = null;
                var entry = _dbContext.InvStockTransfers.Update(itemApprove);
                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                await _dbContext.SaveChangesAsync();
                response.Add("error", "false");
                response.Add("message", "Stock Transfer has been Un-Approved Successfully");
                return response;
            }
            catch (Exception ex)
            {
                response.Add("error", "true");
                response.Add("message", ex.Message == null ? ex.InnerException.Message.ToString() : ex.Message.ToString());
                return response;
            }
        }
        public async Task<bool> Delete(int id)
        {
            var itemDelete = _dbContext.InvStockTransfers.Find(id);
            itemDelete.IsDeleted = true;
            var entry = _dbContext.InvStockTransfers.Update(itemDelete);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return true;
        }

        //public int StockTransferCountNo(int companyId)
        //{
        //    int maxStockTransferNo = 1;
        //    var stockTransfer = _dbContext.InvStockTransfers.Where(s => s.CompanyId == companyId).ToList();
        //    if (stockTransfer.Count > 0)
        //    {
        //        maxStockTransferNo = stockTransfer.Max(s => s.TransferNo);
        //        return maxStockTransferNo + 1;
        //    }
        //    else
        //    {
        //        return maxStockTransferNo;
        //    }
        //}

    }
}
