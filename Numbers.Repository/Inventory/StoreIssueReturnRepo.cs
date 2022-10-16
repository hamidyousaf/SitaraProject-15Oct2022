using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numbers.Repository.Inventory
{
    public class StoreIssueReturnRepo
    {
        private HttpContext HttpContext { get; }
        private readonly NumbersDbContext _dbContext;
        public StoreIssueReturnRepo(NumbersDbContext context)
        {
            _dbContext = context;
        }
        public StoreIssueReturnRepo(NumbersDbContext context, HttpContext httpContext)
        {
            _dbContext = context;
            HttpContext = httpContext;
        }

        public IEnumerable<InvStoreIssue> GetAll(int companyId)
        {
            IEnumerable<InvStoreIssue> list = _dbContext.InvStoreIssues.Include(s => s.WareHouse).Include(s => s.CostCenter)
                .Where(s => s.IsDeleted == false && s.TransactionType == "Issue Return" && s.CompanyId == companyId)
                .ToList();
            return list;
        }

        public InvStoreIssueItem[] GetStoreIssueItems(int id)
        {
            InvStoreIssueItem[] storeIssueItems = _dbContext.InvStoreIssueItems.Where(i => i.StoreIssueId == id && i.IsDeleted == false).ToArray();
            return storeIssueItems;
        }

        public InvStoreIssueViewModel GetById(int id)
        {
            InvStoreIssue storeIssue = _dbContext.InvStoreIssues.Find(id);
            var viewModel = new InvStoreIssueViewModel();
            viewModel.IssueNo = storeIssue.IssueNo;
            viewModel.IssueDate = storeIssue.IssueDate;
            viewModel.WareHouseId = storeIssue.WareHouseId;
            viewModel.CostCenterId = storeIssue.CostCenterId;
            viewModel.Remarks = storeIssue.Remarks;
            viewModel.Status = storeIssue.Status;
            viewModel.VoucherId = storeIssue.VoucherId;
            return viewModel;
        }

        //[HttpPost]
        //public async Task<bool> Create(InvStoreIssueViewModel model, IFormCollection collection)
        //{
        //    try
        //    {
        //        //for master table
        //        var storeIssue = new InvStoreIssue();
        //        storeIssue.IssueNo = model.IssueNo;
        //        storeIssue.IssueDate = model.IssueDate;
        //        storeIssue.WareHouseId = model.WareHouseId;
        //        storeIssue.CostCenterId = model.CostCenterId;
        //        storeIssue.Remarks = (collection["Remarks"][0]);
        //        storeIssue.TransactionType = "Issue Return";
        //        storeIssue.CompanyId = model.CompanyId;
        //        storeIssue.IsDeleted = false;
        //        storeIssue.Status = "Created";
        //        storeIssue.CreatedBy = model.CreatedBy;
        //        storeIssue.CreatedDate = DateTime.Now;
        //        _dbContext.InvStoreIssues.Add(storeIssue);
        //        await _dbContext.SaveChangesAsync();
        //        //for detail table
        //        InvStoreIssueItem[] InvAdjustmentItems = JsonConvert.DeserializeObject<InvStoreIssueItem[]>(collection["details"]);
        //        foreach (var item in InvAdjustmentItems)
        //        {
        //            var storeIssueItem = new InvStoreIssueItem();
        //            storeIssueItem.StoreIssueId = storeIssue.Id;
        //            storeIssueItem.ItemId = item.ItemId;
        //            storeIssueItem.Qty = -Math.Abs(item.Qty);
        //            storeIssueItem.Rate = Convert.ToDecimal(item.Rate);
        //            storeIssueItem.LineTotal = -Math.Abs(item.LineTotal);
        //            storeIssueItem.IsDeleted = false;
        //            storeIssueItem.CreatedBy = model.CreatedBy;
        //            storeIssueItem.CreatedDate = DateTime.Now;
        //            _dbContext.InvStoreIssueItems.Add(storeIssueItem);
        //            await _dbContext.SaveChangesAsync();
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.InnerException.Message);
        //        return false;
        //    }
        //}

        [HttpPost]
        public async Task<bool> Create(InvStoreIssueViewModel model, IFormCollection collection)
        {
            try
            {
                //for master table
                var storeIssue = new InvStoreIssue();
                storeIssue.IssueNo = model.IssueNo;
                storeIssue.IssueDate = model.IssueDate;
                storeIssue.WareHouseId = model.WareHouseId;
                storeIssue.CostCenterId = model.CostCenterId;
                storeIssue.Remarks = (collection["Remarks"][0]);
                storeIssue.TransactionType = "Issue Return";
                storeIssue.CompanyId = model.CompanyId;
                storeIssue.IsDeleted = false;
                storeIssue.Status = "Created";
                storeIssue.CreatedBy = model.CreatedBy;
                storeIssue.CreatedDate = DateTime.Now;
                _dbContext.InvStoreIssues.Add(storeIssue);
                await _dbContext.SaveChangesAsync();
                //for detail table
                InvStoreIssueItem[] InvStoreIssueItems = JsonConvert.DeserializeObject<InvStoreIssueItem[]>(collection["details"]);
                if (InvStoreIssueItems.Count() > 0)
                {
                    foreach (var items in InvStoreIssueItems)
                    {
                        if (items.Id != 0)
                        {
                            InvStoreIssueItem data = _dbContext.InvStoreIssueItems.Where(i => i.StoreIssueId == storeIssue.Id && i.IsDeleted == false && i.Id == items.Id).FirstOrDefault();
                            //foreach (var i in data)
                            //{

                            InvStoreIssueItem obj = new InvStoreIssueItem();
                            obj = data;
                            obj.StoreIssueId = storeIssue.Id;
                            obj.ItemId = items.ItemId;
                            obj.LineTotal = items.LineTotal;
                            obj.Qty = items.Qty;
                            obj.Rate = items.Rate;
                            data.IsDeleted = false;
                            data.UpdatedDate = DateTime.Now;
                            data.UpdatedBy = model.UpdatedBy;
                            _dbContext.InvStoreIssueItems.Update(obj);
                            _dbContext.SaveChanges();
                            //}
                        }
                        else
                        {
                            InvStoreIssueItem data = new InvStoreIssueItem();
                            data = items;
                            data.Rate = items.Rate;
                            data.Qty = items.Qty;
                            data.StoreIssueId = storeIssue.Id;
                            data.IsDeleted = false;
                            data.CreatedDate = DateTime.Now;
                            data.CreatedBy = model.CreatedBy;
                            _dbContext.InvStoreIssueItems.Add(data);
                            _dbContext.SaveChanges();
                        }

                        if (items.StoreIssueItemId != 0)
                        {
                            var orderItem = _dbContext.InvStoreIssueItems.Find(items.StoreIssueItemId);
                            orderItem.ReturnQty = orderItem.ReturnQty + items.Qty;
                            _dbContext.InvStoreIssueItems.Update(orderItem);
                            _dbContext.SaveChanges();
                        }
                    }
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
        public async Task<bool> Update(InvStoreIssueViewModel model, IFormCollection collection)
        {
            //for partial-items removal
            string[] idsDeleted = Convert.ToString(collection["IdsDeleted"]).Split(",");
            if (!idsDeleted.Contains(""))
            {
                for (int j = 0; j < idsDeleted.Length; j++)
                {
                    var itemToRemove = _dbContext.InvStoreIssueItems.Find(Convert.ToInt32(idsDeleted[j]));
                    itemToRemove.IsDeleted = true;
                    var tracker = _dbContext.InvStoreIssueItems.Update(itemToRemove);
                    tracker.OriginalValues.SetValues(await tracker.GetDatabaseValuesAsync());
                    await _dbContext.SaveChangesAsync();
                }
            }
            //updating existing data
            var obj = _dbContext.InvStoreIssues.Find(model.Id);
            obj.IssueNo = model.IssueNo;
            obj.IssueDate = model.IssueDate;
            obj.WareHouseId = model.WareHouseId;
            obj.CostCenterId = model.CostCenterId;
            obj.Remarks = collection["Remarks"][0];
            obj.TransactionType = "Issue Return";
            obj.Status = "Created";
            obj.UpdatedBy = model.UpdatedBy;
            obj.CompanyId = model.CompanyId;
            obj.UpdatedDate = DateTime.Now;
            obj.IsDeleted = false;
            var entry = _dbContext.InvStoreIssues.Update(obj);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            InvStoreIssueItem[] InvAdjustmentItems = JsonConvert.DeserializeObject<InvStoreIssueItem[]>(collection["details"]);
            var list = _dbContext.InvStoreIssueItems.Where(l => l.StoreIssueId == model.Id).ToList();
            if (InvAdjustmentItems != null)
            {
                foreach (var issue in InvAdjustmentItems)
                {
                    if (issue.Id != 0)
                    {
                        var issueItem = _dbContext.InvStoreIssueItems
                      .Where(j => j.StoreIssueId == model.Id && j.Id == issue.Id).FirstOrDefault();
                        //below phenomenon prevents Id from being marked as modified
                        //var entityEntry = _dbContext.Entry(issueItem);
                        //entityEntry.State = EntityState.Modified;
                        //entityEntry.Property(p => p.Id).IsModified = false;
                        issueItem.ItemId = issue.ItemId;
                        issueItem.StoreIssueId = obj.Id;
                        issueItem.Qty = -Math.Abs(issue.Qty);
                        issueItem.Rate = Convert.ToDecimal(issue.Rate);
                        issueItem.LineTotal = -Math.Abs(issue.LineTotal);
                        issueItem.UpdatedBy = model.UpdatedBy;
                        issueItem.UpdatedDate = DateTime.Now;
                        var dbEntry = _dbContext.InvStoreIssueItems.Update(issueItem);
                        //dbEntry.OriginalValues.SetValues(await entityEntry.GetDatabaseValuesAsync());
                    }
                    else
                    {
                        var newItem = new InvStoreIssueItem();
                        newItem.ItemId = issue.ItemId;
                        newItem.StoreIssueId = obj.Id;
                        newItem.Qty = -Math.Abs(issue.Qty); ;
                        newItem.Rate = Convert.ToDecimal(issue.Rate); ;
                        newItem.LineTotal = -Math.Abs(issue.LineTotal);
                        newItem.CreatedDate = DateTime.Now;
                        newItem.CreatedBy = model.UpdatedBy;
                        newItem.UpdatedBy = model.UpdatedBy;
                        newItem.UpdatedDate = DateTime.Now;
                        _dbContext.InvStoreIssueItems.Add(newItem);
                    }
                    await _dbContext.SaveChangesAsync();
                }
            }
            return true;
        }
        public dynamic GetStoreIssueItems(int id, int itemId)
        {
            var item = _dbContext.InvStoreIssueItems.Include(i => i.Item).Where(i => i.Id == id && i.IsDeleted == false).FirstOrDefault();
            InvStoreIssueViewModel viewModel = new InvStoreIssueViewModel();
            viewModel.ItemId = item.ItemId;
            viewModel.Qty = item.Qty;
            viewModel.Rate = item.Rate;
            viewModel.LineTotal = item.LineTotal;
            viewModel.StoreIssueItemId = item.Id;
            viewModel.Remarks = item.Remarks;
            return viewModel;
        }

        public async Task<bool> Approve(int id, string userId, int companyId)
        {
            InvStoreIssue storeIssue = _dbContext.InvStoreIssues
           .Where(a => a.Status == "Created" && a.TransactionType == "Issue Return" && a.CompanyId == companyId && a.Id == id && a.IsDeleted == false)
           .FirstOrDefault();
            try
            {
                //Create Voucher  
                GLVoucher voucherMaster = new GLVoucher();
                List<GLVoucherDetail> voucherDetails = new List<GLVoucherDetail>();
                string voucherDescription = string.Format(
                "Store Issue # : {0} ",
                storeIssue.IssueNo);

                int voucherId = 0;
                voucherMaster.VoucherType = "ISS-RTN";
                voucherMaster.VoucherDate = storeIssue.IssueDate;
                voucherMaster.Currency = "MYR";
                voucherMaster.CurrencyExchangeRate = 1;
                voucherMaster.Description = voucherDescription.Substring(0, voucherDescription.Length);
                voucherMaster.Status = "Approved";
                voucherMaster.ApprovedBy = userId;
                voucherMaster.ApprovedDate = DateTime.Now;
                voucherMaster.IsSystem = true;
                voucherMaster.ModuleName = "Inventory/StoreIssueReturn";
                voucherMaster.ModuleId = id;

                GLVoucherDetail voucherDetail = new GLVoucherDetail();
                //debit entry
                #region Debit
                var costAccounts = (from li in _dbContext.InvStoreIssueItems
                                    join i in _dbContext.InvItems.Include(a => a.InvItemAccount) on li.ItemId equals i.Id
                                    where li.StoreIssueId == id && li.IsDeleted == false
                                    select new
                                    {
                                        li.LineTotal,
                                        i.InvItemAccount.GLCostofSaleAccountId
                                    }).GroupBy(l => l.GLCostofSaleAccountId)
                               .Select(li => new InvStoreIssueItem
                               {
                                   LineTotal = li.Sum(c => c.LineTotal),
                                   StoreIssueId = li.FirstOrDefault().GLCostofSaleAccountId //StoreIssueId is temporarily containing GLCostofSaleAccountId
                               }).ToList();
                foreach (var item in costAccounts)
                {
                    voucherDetail = new GLVoucherDetail();
                    voucherDetail.AccountId = item.StoreIssueId;
                    voucherDetail.Sequence = 20;
                    voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                    voucherDetail.Debit = 0;
                    voucherDetail.Credit = item.LineTotal;
                    voucherDetail.IsDeleted = false;
                    voucherDetail.CreatedBy = userId;
                    voucherDetail.CreatedDate = DateTime.Now;
                    voucherDetails.Add(voucherDetail);
                }
                #endregion Debit
                //credit entry
                #region Credit
                var assetAccounts = (from li in _dbContext.InvStoreIssueItems
                                     join i in _dbContext.InvItems.Include(a => a.InvItemAccount) on li.ItemId equals i.Id
                                     where li.StoreIssueId == id && li.IsDeleted == false
                                     select new
                                     {
                                         li.LineTotal,
                                         i.InvItemAccount.GLAssetAccountId
                                     }).GroupBy(l => l.GLAssetAccountId)
                              .Select(li => new InvStoreIssueItem
                              {
                                  LineTotal = li.Sum(c => c.LineTotal),
                                  StoreIssueId = li.FirstOrDefault().GLAssetAccountId //StoreIssueId is temporarily containing GLCostofSaleAccountId
                              }).ToList();
                foreach (var item in assetAccounts)
                {
                    voucherDetail = new GLVoucherDetail();
                    voucherDetail.AccountId = item.StoreIssueId;
                    voucherDetail.Sequence = 20;
                    voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                    voucherDetail.Debit = item.LineTotal;
                    voucherDetail.Credit = 0;
                    voucherDetail.IsDeleted = false;
                    voucherDetail.CreatedBy = userId;
                    voucherDetail.CreatedDate = DateTime.Now;
                    voucherDetails.Add(voucherDetail);
                }
                #endregion Credit
                //Create Voucher 
                var helper = new Numbers.Repository.Helpers.VoucherHelper(_dbContext, HttpContext);
                voucherId = helper.CreateVoucher(voucherMaster, voucherDetails);
                if (voucherId != 0)
                {
                    using (var transaction = _dbContext.Database.BeginTransaction())
                    {
                        //var storeIssue = _dbContext.InvStoreIssues.Find(id);
                        storeIssue.VoucherId = voucherId;
                        storeIssue.Status = "Approved";
                        storeIssue.ApprovedBy = userId;
                        storeIssue.ApprovedDate = DateTime.Now;
                        var entry = _dbContext.InvStoreIssues.Update(storeIssue);
                        entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                        await _dbContext.SaveChangesAsync();
                        var storeIssueItems = _dbContext.InvStoreIssueItems.Where(i => i.StoreIssueId == id && i.IsDeleted == false).ToList();
                        foreach (var storeIssueItem in storeIssueItems)
                        {
                            var item = _dbContext.InvItems.Find(storeIssueItem.ItemId);
                            item.StockQty = item.StockQty - storeIssueItem.Qty;
                            item.StockValue = item.StockValue - (item.AvgRate * storeIssueItem.Qty);
                            var dbEntry = _dbContext.InvItems.Update(item);
                            dbEntry.OriginalValues.SetValues(await dbEntry.GetDatabaseValuesAsync());
                            await _dbContext.SaveChangesAsync();
                        }
                        transaction.Commit();
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                //transaction.Rollback();
                return false;
            }
        }

        public IEnumerable<InvStoreIssue> GetApproved()
        {
            var model = _dbContext.InvStoreIssues.Include(s => s.WareHouse).Where(i => i.Status == "Approved" && i.TransactionType == "Issue Return" && !i.IsDeleted && i.CompanyId == HttpContext.Session.GetInt32("CompanyId").Value).AsEnumerable();
            return model;
        }

        public async Task<Dictionary<string, string>> UnApprove(int id)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                Dictionary<string, string> response = new Dictionary<string, string>();
                try
                {
                    int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                    string userId = HttpContext.Session.GetString("UserId");
                    var storeIssue = _dbContext.InvStoreIssues
                        .Where(a => a.Id == id && a.CompanyId == companyId && a.Status == "Approved" && !a.IsDeleted).FirstOrDefault();
                    storeIssue.Status = "Created";
                    storeIssue.ApprovedBy = null;
                    storeIssue.ApprovedDate = null;
                    var entry = _dbContext.InvStoreIssues.Update(storeIssue);
                    entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                    await _dbContext.SaveChangesAsync();
                    var storeIssueItems = _dbContext.InvStoreIssueItems.Where(i => i.StoreIssueId == id && i.IsDeleted == false).ToList();
                    foreach (var storeIssueItem in storeIssueItems)
                    {
                        var item = _dbContext.InvItems.Find(storeIssueItem.ItemId);
                        item.StockQty = item.StockQty + storeIssueItem.Qty;
                        item.StockValue = item.StockValue + (item.AvgRate * storeIssueItem.Qty);
                        var dbEntry = _dbContext.InvItems.Update(item);
                        dbEntry.OriginalValues.SetValues(await dbEntry.GetDatabaseValuesAsync());
                        await _dbContext.SaveChangesAsync();
                    }
                    var storeVoucher = _dbContext.GLVoucherDetails.Where(x => x.VoucherId == storeIssue.VoucherId).ToList();
                    foreach (var item in storeVoucher)
                    {
                        item.IsDeleted = true;
                        var dbEntry = _dbContext.GLVoucherDetails.Update(item);
                        dbEntry.OriginalValues.SetValues(await dbEntry.GetDatabaseValuesAsync());
                        await _dbContext.SaveChangesAsync();
                    }
                    transaction.Commit();
                    response.Add("error", "false");
                    response.Add("message", "Store Issue Return has been Un-Approved Successfully");
                    return response;
                }
                catch (Exception exc)//Error
                {
                    transaction.Rollback();
                    response.Add("error", "true");
                    response.Add("message", exc.Message == null ? exc.InnerException.Message.ToString() : exc.Message.ToString());
                    return response;
                }
            }
        }

        public async Task<bool> Delete(int id)
        {
            var itemDelete = _dbContext.InvStoreIssues.Find(id);
            itemDelete.IsDeleted = true;
            var entry = _dbContext.InvStoreIssues.Update(itemDelete);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public int StoreIssueCountNo(int companyId)
        {
            int maxStoreIssueNo = 1;
            var storeIssue = _dbContext.InvStoreIssues.Where(s => s.CompanyId == companyId && s.TransactionType == "Issue Return").ToList();
            if (storeIssue.Count > 0)
            {
                maxStoreIssueNo = storeIssue.Max(s => s.IssueNo);
                return maxStoreIssueNo + 1;
            }
            else
            {
                return maxStoreIssueNo;
            }
        }

        [HttpGet]
        public dynamic GetStoreIssueById(int id, int[] skipIds, int companyId)
        {
            var invoiceItems = _dbContext.InvStoreIssueItems.Include(i => i.StoreIssue).Include(i => i.Item)
                .Where(i => i.StoreIssue.TransactionType == "Issue" && i.StoreIssue.CompanyId == companyId && i.IsDeleted == false && i.StoreIssue.WareHouseId == id)
                .Where(i => !skipIds.Contains(i.Id)).ToList();
            return invoiceItems;
        }

        public InvStoreIssueViewModel GetSaleInvoiceItems(int id)
        {
            var item = new InvStoreIssueItem();
            item = _dbContext.InvStoreIssueItems.Include(i => i.Item).Include(i => i.StoreIssue).Where(i => i.Id == id && i.IsDeleted == false).FirstOrDefault();
            InvStoreIssueViewModel viewModel = new InvStoreIssueViewModel();
            viewModel.StoreIssueId = item.StoreIssue.Id;
            viewModel.IssueNo = item.StoreIssue.IssueNo;
            viewModel.StoreIssueItemId = item.Id;
            viewModel.ItemName = item.Item.Name;
            viewModel.ItemId = item.ItemId;
            viewModel.Qty = item.Qty - item.ReturnQty;
            viewModel.UOM = "SQM";
            // viewModel.Stock = item.;
            viewModel.Rate = item.Rate;
            viewModel.LineTotal = item.LineTotal;

            return viewModel;
        }

    }
}
