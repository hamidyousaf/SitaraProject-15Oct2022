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
    public class PlnMonthlyRepo
    {
        private HttpContext HttpContext { get; }
        private readonly NumbersDbContext _dbContext;
        public PlnMonthlyRepo(NumbersDbContext context)
        {
            _dbContext = context;
        }
        public PlnMonthlyRepo(NumbersDbContext context, HttpContext httpContext)
        {
            _dbContext = context;
            HttpContext = httpContext;
        }

        public IEnumerable<PlnMonthly> GetAll(int companyId)
        {
            IEnumerable<PlnMonthly> list = _dbContext.PlnMonthlies.Include(s => s.Season).Include(s => s.MonthId)
                .Where(s => s.IsDeleted == false && s.CompanyId == companyId)
                .ToList();
            return list;
        }

        public PlnMonthlyItem[] GetPlnMonthlyItems(int id)
        {
            PlnMonthlyItem[] storeIssueItems = _dbContext.PlnMonthlyItems.Where(i => i.PlnMonthlyId == id && i.IsDeleted == false).ToArray();
            return storeIssueItems;
        }

        public PlnMonthlyViewModel GetById(int id)
        {
            PlnMonthly storeIssue = _dbContext.PlnMonthlies.Find(id);
            var viewModel = new PlnMonthlyViewModel();
            viewModel.IssueNo = storeIssue.IssueNo;
            viewModel.IssueDate = storeIssue.IssueDate;
            viewModel.SeasonId = storeIssue.SeasonId;
            viewModel.MonthId = storeIssue.MonthId;
            viewModel.Remarks = storeIssue.Remarks;
            viewModel.Status = storeIssue.Status;
            viewModel.VoucherId = storeIssue.VoucherId;
            return viewModel;
        }

        //[HttpPost]
        //public async Task<bool> Create(PlnMonthlyViewModel model, IFormCollection collection)
        //{
        //    try
        //    {
        //        //for master table
        //        var storeIssue = new PlnMonthly();
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
        //        _dbContext.PlnMonthlys.Add(storeIssue);
        //        await _dbContext.SaveChangesAsync();
        //        //for detail table
        //        PlnMonthlyItem[] InvAdjustmentItems = JsonConvert.DeserializeObject<PlnMonthlyItem[]>(collection["details"]);
        //        foreach (var item in InvAdjustmentItems)
        //        {
        //            var storeIssueItem = new PlnMonthlyItem();
        //            storeIssueItem.StoreIssueId = storeIssue.Id;
        //            storeIssueItem.ItemId = item.ItemId;
        //            storeIssueItem.Qty = -Math.Abs(item.Qty);
        //            storeIssueItem.Rate = Convert.ToDecimal(item.Rate);
        //            storeIssueItem.LineTotal = -Math.Abs(item.LineTotal);
        //            storeIssueItem.IsDeleted = false;
        //            storeIssueItem.CreatedBy = model.CreatedBy;
        //            storeIssueItem.CreatedDate = DateTime.Now;
        //            _dbContext.PlnMonthlyItems.Add(storeIssueItem);
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
        public async Task<bool> Create(PlnMonthlyViewModel model, IFormCollection collection)
        {
            try
            {
                //for master table
                var storeIssue = new PlnMonthly();
                storeIssue.IssueNo = model.IssueNo;
                storeIssue.IssueDate = model.IssueDate;
                storeIssue.SeasonId = model.SeasonId;
                storeIssue.MonthId = model.MonthId;
                storeIssue.Remarks = (collection["Remarks"][0]);
                storeIssue.CompanyId = model.CompanyId;
                storeIssue.IsDeleted = false;
                storeIssue.Status = "Created";
                storeIssue.CreatedBy = model.CreatedBy;
                storeIssue.CreatedDate = DateTime.Now;
                _dbContext.PlnMonthlies.Add(storeIssue);
                await _dbContext.SaveChangesAsync();
                //for detail table
                PlnMonthlyItem[] plnMonthlyItems = JsonConvert.DeserializeObject<PlnMonthlyItem[]>(collection["details"]);
                if (plnMonthlyItems.Count() > 0)
                {
                    foreach (var items in plnMonthlyItems)
                    {
                        if (items.Id != 0)
                        {
                            PlnMonthlyItem data = _dbContext.PlnMonthlyItems.Where(i => i.PlnMonthlyId == storeIssue.Id && i.IsDeleted == false && i.Id == items.Id).FirstOrDefault();
                            //foreach (var i in data)
                            //{

                            PlnMonthlyItem obj = new PlnMonthlyItem();
                            obj = data;
                            obj.PlnMonthlyId = storeIssue.Id;
                            obj.ItemCategory4Id = items.ItemCategory4Id;
                            obj.SpecificationId = items.SpecificationId;
                            obj.MonthId = items.MonthId;
                            obj.SeasonalDetailId = items.SeasonalDetailId;
                            data.IsDeleted = false;
                            data.UpdatedDate = DateTime.Now;
                            data.UpdatedBy = model.UpdatedBy;
                            _dbContext.PlnMonthlyItems.Update(obj);
                            _dbContext.SaveChanges();
                            //}
                        }
                        else
                        {
                            PlnMonthlyItem data = new PlnMonthlyItem();
                            data = items;
                            data.ItemCategory4Id = items.ItemCategory4Id;
                            data.SpecificationId = items.SpecificationId;
                            data.MonthId = items.MonthId;
                            data.SeasonalDesignCount = items.SeasonalDesignCount;

                            data.SeasonalRunQty = items.SeasonalRunQty;
                            data.SeasonalFabricQty = items.SeasonalFabricQty;
                            data.MonthlyDesignCount = items.MonthlyDesignCount;
                            data.MonthlyRunQty = items.MonthlyRunQty;
                            data.MonthlyFabicCons = items.SeasonalFabricQty;
                            data.PlnMonthlyId = storeIssue.Id;
                            data.IsDeleted = false;
                            data.CreatedDate = DateTime.Now;
                            data.CreatedBy = model.CreatedBy;
                            _dbContext.PlnMonthlyItems.Add(data);
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
        public async Task<bool> Update(PlnMonthlyViewModel model, IFormCollection collection)
        {
            //for partial-items removal
            string[] idsDeleted = Convert.ToString(collection["IdsDeleted"]).Split(",");
            if (!idsDeleted.Contains(""))
            {
                for (int j = 0; j < idsDeleted.Length; j++)
                {
                    var itemToRemove = _dbContext.PlnMonthlyItems.Find(Convert.ToInt32(idsDeleted[j]));
                    itemToRemove.IsDeleted = true;
                    var tracker = _dbContext.PlnMonthlyItems.Update(itemToRemove);
                    tracker.OriginalValues.SetValues(await tracker.GetDatabaseValuesAsync());
                    await _dbContext.SaveChangesAsync();
                }
            }
            //updating existing data
            var obj = _dbContext.PlnMonthlies.Find(model.Id);
            obj.IssueNo = model.IssueNo;
            obj.IssueDate = model.IssueDate;
            obj.SeasonId = model.SeasonId;
            obj.MonthId = model.MonthId;
            obj.Remarks = collection["Remarks"][0];
            obj.Status = "Created";
            obj.UpdatedBy = model.UpdatedBy;
            obj.CompanyId = model.CompanyId;
            obj.UpdatedDate = DateTime.Now;
            obj.IsDeleted = false;
            var entry = _dbContext.PlnMonthlies.Update(obj);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            PlnMonthlyItem[] InvAdjustmentItems = JsonConvert.DeserializeObject<PlnMonthlyItem[]>(collection["details"]);
            var list = _dbContext.PlnMonthlyItems.Where(l => l.PlnMonthlyId == model.Id).ToList();
            if (InvAdjustmentItems != null)
            {
                foreach (var issue in InvAdjustmentItems)
                {
                    if (issue.Id != 0)
                    {
                        var issueItem = _dbContext.PlnMonthlyItems
                      .Where(j => j.PlnMonthlyId == model.Id && j.Id == issue.Id).FirstOrDefault();
                        //below phenomenon prevents Id from being marked as modified
                        //var entityEntry = _dbContext.Entry(issueItem);
                        //entityEntry.State = EntityState.Modified;
                        //entityEntry.Property(p => p.Id).IsModified = false;
                        issueItem.ItemCategory4Id = issue.ItemCategory4Id;
                        issueItem.SpecificationId = issue.SpecificationId;
                        issueItem.MonthId = issue.MonthId;
                        issueItem.SeasonalRunQty = issue.SeasonalRunQty;
                        issueItem.SeasonalFabricQty = issue.SeasonalFabricQty;
                        issueItem.MonthlyDesignCount = issue.MonthlyDesignCount;
                        issueItem.MonthlyRunQty = issue.MonthlyRunQty;
                        issueItem.MonthlyFabicCons = issue.MonthlyFabicCons;
                        issueItem.PlnMonthlyId = obj.Id;

                        issueItem.UpdatedBy = model.UpdatedBy;
                        issueItem.UpdatedDate = DateTime.Now;
                        var dbEntry = _dbContext.PlnMonthlyItems.Update(issueItem);
                        //dbEntry.OriginalValues.SetValues(await entityEntry.GetDatabaseValuesAsync());
                    }
                    else
                    {
                        var newItem = new PlnMonthlyItem();
                        newItem.ItemCategory4Id = issue.ItemCategory4Id;
                        newItem.SpecificationId = issue.SpecificationId;
                        newItem.MonthId = issue.MonthId;
                        newItem.SeasonalDesignCount = issue.SeasonalDesignCount;
                        newItem.SeasonalRunQty = issue.SeasonalRunQty;
                        newItem.SeasonalFabricQty = issue.SeasonalFabricQty;
                        newItem.MonthlyDesignCount = issue.MonthlyDesignCount;
                        newItem.MonthlyRunQty = issue.MonthlyRunQty;
                        newItem.MonthlyFabicCons = issue.MonthlyFabicCons;
                        newItem.PlnMonthlyId = obj.Id;

                        newItem.CreatedDate = DateTime.Now;
                        newItem.CreatedBy = model.UpdatedBy;
                        newItem.UpdatedBy = model.UpdatedBy;
                        newItem.UpdatedDate = DateTime.Now;
                        _dbContext.PlnMonthlyItems.Add(newItem);
                    }
                    await _dbContext.SaveChangesAsync();
                }
            }
            return true;
        }
        public dynamic GetStoreIssueItems(int id)
        {
            var item = _dbContext.PlnMonthlyItems.Include(i => i.ItemCategory4).Where(i => i.Id == id && i.IsDeleted == false).FirstOrDefault();
            PlnMonthlyItem viewModel = new PlnMonthlyItem();
            viewModel.SpecificationId = item.SpecificationId;
            viewModel.MonthId = item.MonthId;
            viewModel.SeasonalDesignCount = item.SeasonalDesignCount;
            viewModel.SeasonalRunQty = item.SeasonalRunQty;
            viewModel.SeasonalFabricQty = item.SeasonalFabricQty;
            viewModel.MonthlyDesignCount = item.MonthlyDesignCount;
            viewModel.MonthlyRunQty = item.MonthlyRunQty;
            viewModel.MonthlyRunQty = item.MonthlyRunQty;

            viewModel.ItemCategory4Id = item.Id;
            return viewModel;
        }

       

        public IEnumerable<PlnMonthly> GetApproved()
        {
            var model = _dbContext.PlnMonthlies.Include(s => s.Season).Where(i => i.Status == "Approved" && !i.IsDeleted && i.CompanyId == HttpContext.Session.GetInt32("CompanyId").Value).AsEnumerable();
            return model;
        }

        //public async Task<Dictionary<string, string>> UnApprove(int id)
        //{
        //    using (var transaction = _dbContext.Database.BeginTransaction())
        //    {
        //        Dictionary<string, string> response = new Dictionary<string, string>();
        //        try
        //        {
        //            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
        //            string userId = HttpContext.Session.GetString("UserId");
        //            var storeIssue = _dbContext.PlnMonthlies
        //                .Where(a => a.Id == id && a.CompanyId == companyId && a.Status == "Approved" && !a.IsDeleted).FirstOrDefault();
        //            storeIssue.Status = "Created";
        //            storeIssue.ApprovedBy = null;
        //            storeIssue.ApprovedDate = null;
        //            var entry = _dbContext.PlnMonthlies.Update(storeIssue);
        //            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
        //            await _dbContext.SaveChangesAsync();
        //            var storeIssueItems = _dbContext.PlnMonthlyItems.Where(i => i.StoreIssueId == id && i.IsDeleted == false).ToList();
        //            foreach (var storeIssueItem in storeIssueItems)
        //            {
        //                var item = _dbContext.InvItems.Find(storeIssueItem.ItemId);
        //                item.StockQty = item.StockQty + storeIssueItem.Qty;
        //                item.StockValue = item.StockValue + (item.AvgRate * storeIssueItem.Qty);
        //                var dbEntry = _dbContext.InvItems.Update(item);
        //                dbEntry.OriginalValues.SetValues(await dbEntry.GetDatabaseValuesAsync());
        //                await _dbContext.SaveChangesAsync();
        //            }
        //            var storeVoucher = _dbContext.GLVoucherDetails.Where(x => x.VoucherId == storeIssue.VoucherId).ToList();
        //            foreach (var item in storeVoucher)
        //            {
        //                item.IsDeleted = true;
        //                var dbEntry = _dbContext.GLVoucherDetails.Update(item);
        //                dbEntry.OriginalValues.SetValues(await dbEntry.GetDatabaseValuesAsync());
        //                await _dbContext.SaveChangesAsync();
        //            }
        //            transaction.Commit();
        //            response.Add("error", "false");
        //            response.Add("message", "Store Issue Return has been Un-Approved Successfully");
        //            return response;
        //        }
        //        catch (Exception exc)//Error
        //        {
        //            transaction.Rollback();
        //            response.Add("error", "true");
        //            response.Add("message", exc.Message == null ? exc.InnerException.Message.ToString() : exc.Message.ToString());
        //            return response;
        //        }
        //    }
        //}

        public async Task<bool> Delete(int id)
        {
            var itemDelete = _dbContext.PlnMonthlies.Find(id);
            itemDelete.IsDeleted = true;
            var entry = _dbContext.PlnMonthlies.Update(itemDelete);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public int StoreIssueCountNo(int companyId)
        {
            int maxStoreIssueNo = 1;
            var storeIssue = _dbContext.PlnMonthlies.Where(s => s.CompanyId == companyId).ToList();
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
            var invoiceItems = _dbContext.PlnMonthlyItems.Include(i => i.PlnMonthly).Include(i => i.ItemCategory4)
                .Where(i =>   i.PlnMonthly.CompanyId == companyId && i.IsDeleted == false)
                .Where(i => !skipIds.Contains(i.Id)).ToList();
            return invoiceItems;
        }

        public PlnMonthlyViewModel GetSaleInvoiceItems(int id)
        {
            var item = new PlnMonthlyItem();
            item = _dbContext.PlnMonthlyItems.Include(i => i.ItemCategory4).Include(i => i.PlnMonthly).Where(i => i.Id == id && i.IsDeleted == false).FirstOrDefault();
            PlnMonthlyViewModel viewModel = new PlnMonthlyViewModel();
            viewModel.StoreIssueId = item.PlnMonthly.Id;
            viewModel.IssueNo = item.PlnMonthly.IssueNo;
            viewModel.StoreIssueItemId = item.Id;
            viewModel.ItemName = item.ItemCategory4.Name;
            viewModel.ItemId = item.ItemCategory4Id;
            viewModel.UOM = "SQM";
            // viewModel.Stock = item.;

            return viewModel;
        }

    }
}

