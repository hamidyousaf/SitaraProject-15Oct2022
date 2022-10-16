using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
    public class YarnIssueRepo
    {
        private HttpContext HttpContext { get; }
        private readonly NumbersDbContext _dbContext;
        public YarnIssueRepo(NumbersDbContext context)
        {
            _dbContext = context;
        }
        public YarnIssueRepo(NumbersDbContext context, HttpContext httpContext)
        {
            _dbContext = context;
            HttpContext = httpContext;
        }

        public IEnumerable<YarnIssuance> GetAll(int companyId)
        {
            IEnumerable<YarnIssuance> list = _dbContext.YarnIssuances.Include(s => s.WareHouse)
                .Where(s => s.IsDeleted == false && s.TransactionType == "Issue Return" && s.CompanyId == companyId)
                .ToList();
            return list;
        }

        public YarnIssuanceItem[] GetYarnIssuanceItems(int id)
        {
            YarnIssuanceItem[] YarnIssuanceItems = _dbContext.YarnIssuanceItems.Where(i => i.YarnIssuanceId == id && i.IsDeleted == false).ToArray();
            return YarnIssuanceItems;
        }

        public YarnIssueViewModel GetById(int id)
        {
            YarnIssuance YarnIssuance = _dbContext.YarnIssuances.Find(id);
            var viewModel = new YarnIssueViewModel();
            viewModel.IssueNo = YarnIssuance.IssueNo;
            viewModel.IssueDate = YarnIssuance.IssueDate;
            viewModel.WareHouseId = YarnIssuance.WareHouseId;
            viewModel.Remarks = YarnIssuance.Remarks;
            viewModel.Status = YarnIssuance.Status;
            viewModel.VoucherId = YarnIssuance.VoucherId;
            return viewModel;
        }
        [HttpGet]
        public int Max(int companyId)
        {
            int transactionNo = 1;
            var result = _dbContext.YarnIssuances.Where(x => x.IsDeleted != true && x.CompanyId == companyId).ToList();
            if (result.Count() > 0)
            {
                transactionNo = result.Max(x => x.IssueNo) + 1;
            }
            return transactionNo;
        }
        

        [HttpPost]
        public async Task<bool> Create(YarnIssueViewModel model)
        {
            try
            {
                //for master table
                var YarnIssuance = new YarnIssuance();
                YarnIssuance.IssueNo = model.IssueNo;
                YarnIssuance.IssueDate = model.YarnIssuance.IssueDate;
                YarnIssuance.WeavingContractId = model.YarnIssuance.WeavingContractId;
                YarnIssuance.VendorId = model.VendorId;
                YarnIssuance.WareHouseId = model.YarnIssuance.WareHouseId;
                YarnIssuance.Remarks = model.YarnIssuance.Remarks;
                YarnIssuance.TransactionType = "Yarn Issuance";
                YarnIssuance.CompanyId = model.CompanyId;
                YarnIssuance.IsDeleted = false;
                YarnIssuance.Status = "Created";
                YarnIssuance.CreatedBy = model.CreatedBy;
                YarnIssuance.CreatedDate = DateTime.Now;
                _dbContext.YarnIssuances.Add(YarnIssuance);
                await _dbContext.SaveChangesAsync();
                //For Warp Detail 
                foreach (var item in model.WarpDetails)
                {
                    WarpIssuance detail = new WarpIssuance();
                    detail.YarnIssuanceId = YarnIssuance.Id;
                    detail.ItemId = item.ItemId;
                    detail.BrandId = item.BrandId;
                    detail.UOMId = item.UOMId;
                    detail.Quantity = item.Quantity;
                    _dbContext.WarpIssuances.Add(detail);
                    await _dbContext.SaveChangesAsync();
                }
                //For Weft Detail 
                foreach (var item in model.WeftDetails)
                {
                    WeftIssuance detail = new WeftIssuance();
                    detail.YarnIssuanceId = YarnIssuance.Id;
                    detail.ItemId = item.ItemId;
                    detail.BrandId = item.BrandId;
                    detail.UOMId = item.UOMId;
                    detail.Quantity = item.Quantity;
                    _dbContext.WeftIssuances.Add(detail);
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
        public async Task<bool> Update(YarnIssueViewModel model)
        {
            try
            {
                //for master table
                YarnIssuance YarnIssuance = _dbContext.YarnIssuances.FirstOrDefault(x => x.Id == model.Id);
                YarnIssuance.IssueDate = model.YarnIssuance.IssueDate;
                YarnIssuance.VendorId = model.VendorId;
                YarnIssuance.WeavingContractId = model.YarnIssuance.WeavingContractId;
                YarnIssuance.WareHouseId = model.YarnIssuance.WareHouseId;
                YarnIssuance.Remarks = model.YarnIssuance.Remarks;
                YarnIssuance.TransactionType = "Yarn Issuance";
                YarnIssuance.CompanyId = model.CompanyId;
                YarnIssuance.IsDeleted = false;
                YarnIssuance.Status = "Created";
                YarnIssuance.UpdatedBy = model.UpdatedBy;
                YarnIssuance.UpdatedDate = DateTime.Now.Date;
                _dbContext.YarnIssuances.Update(YarnIssuance);

                var existingWarpDetail = _dbContext.WarpIssuances.Where(x => x.YarnIssuanceId == model.Id).ToList();
                //Deleting detail
                foreach (var detail in existingWarpDetail)
                {
                    bool isExist = model.WarpDetails.Any(x => x.Id == detail.Id);
                    if (!isExist)
                    {
                        _dbContext.WarpIssuances.Remove(detail);
                        await _dbContext.SaveChangesAsync();
                    }
                }
                var existingWeftDetail = _dbContext.WeftIssuances.Where(x => x.YarnIssuanceId == model.Id).ToList();
                //Deleting detail
                foreach (var detail in existingWeftDetail)
                {
                    bool isExist = model.WeftDetails.Any(x => x.Id == detail.Id);
                    if (!isExist)
                    {
                        _dbContext.WeftIssuances.Remove(detail);
                        await _dbContext.SaveChangesAsync();
                    }
                }
                //Inserting/Updating monthly limit
                foreach (var detail in model.WarpDetails)
                {
                    if (detail.Id == 0) //Inserting New Records
                    {
                        WarpIssuance Items = new WarpIssuance();
                        Items.YarnIssuanceId = model.Id;
                        Items.ItemId = detail.ItemId;
                        Items.BrandId = detail.BrandId;
                        Items.UOMId = detail.UOMId;
                        Items.Quantity = detail.Quantity;
                        await _dbContext.WarpIssuances.AddAsync(Items);
                    }
                    else   //Updating Records
                    {
                        WarpIssuance Items = _dbContext.WarpIssuances.FirstOrDefault(x => x.Id == detail.Id);
                        Items.YarnIssuanceId = model.Id;
                        Items.ItemId = detail.ItemId;
                        Items.BrandId = detail.BrandId;
                        Items.UOMId = detail.UOMId;
                        Items.Quantity = detail.Quantity;
                        _dbContext.WarpIssuances.Update(Items);
                    }
                    await _dbContext.SaveChangesAsync();
                }
                //Inserting/Updating monthly limit
                foreach (var detail in model.WeftDetails)
                {
                    if (detail.Id == 0) //Inserting New Records
                    {
                        WeftIssuance Items = new WeftIssuance();
                        Items.YarnIssuanceId = model.Id;
                        Items.ItemId = detail.ItemId;
                        Items.BrandId = detail.BrandId;
                        Items.UOMId = detail.UOMId;
                        Items.Quantity = detail.Quantity;
                        await _dbContext.WeftIssuances.AddAsync(Items);
                    }
                    else   //Updating Records
                    {
                        WeftIssuance Items = _dbContext.WeftIssuances.FirstOrDefault(x => x.Id == detail.Id);
                        Items.YarnIssuanceId = model.Id;
                        Items.ItemId = detail.ItemId;
                        Items.BrandId = detail.BrandId;
                        Items.UOMId = detail.UOMId;
                        Items.Quantity = detail.Quantity;
                        _dbContext.WeftIssuances.Update(Items);
                    }
                    await _dbContext.SaveChangesAsync();
                }
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
        public dynamic GetYarnIssuanceItems(int id, int itemId)
        {
            var item = _dbContext.YarnIssuanceItems.Include(i => i.Item).Where(i => i.Id == id && i.IsDeleted == false).FirstOrDefault();
            YarnIssueViewModel viewModel = new YarnIssueViewModel();
            viewModel.ItemId = item.ItemId;
            viewModel.Qty = item.Qty;
            viewModel.YarnIssuanceItemId = item.Id;
            viewModel.Remarks = item.Remarks;
            return viewModel;
        }

        public async Task<bool> Approve2(int id, string userId, int companyId)
        {
            try
            {
                        var YarnIssuance = _dbContext.YarnIssuances.Find(id);
                        //YarnIssuance.VoucherId = voucherId;
                        YarnIssuance.Status = "Approved";
                        YarnIssuance.ApprovedBy = userId;
                        YarnIssuance.ApprovedDate = DateTime.Now;
                        var entry = _dbContext.YarnIssuances.Update(YarnIssuance);
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

        public async Task<bool> Approve(int id, string userId, int companyId)
        {
            YarnIssuance YarnIssuance = _dbContext.YarnIssuances
           .Where(a => a.Status == "Created"  && a.CompanyId == companyId && a.Id == id && a.IsDeleted == false)
           .FirstOrDefault();
            try
            {
                //Create Voucher  
                GLVoucher voucherMaster = new GLVoucher();
                List<GLVoucherDetail> voucherDetails = new List<GLVoucherDetail>();
                string voucherDescription = string.Format(
                "Store Issue # : {0} ",
                YarnIssuance.IssueNo);

                int voucherId = 0;
                voucherMaster.VoucherType = "YIssue";
                voucherMaster.VoucherDate = YarnIssuance.IssueDate;
                voucherMaster.Currency = "MYR";
                voucherMaster.CurrencyExchangeRate = 1;
                voucherMaster.Description = voucherDescription.Substring(0, voucherDescription.Length);
                voucherMaster.Status = "Approved";
                voucherMaster.ApprovedBy = userId;
                voucherMaster.ApprovedDate = DateTime.Now;
                voucherMaster.IsSystem = true;
                voucherMaster.ModuleName = "Yarn/YarnIssuance";
                voucherMaster.ModuleId = id;

                GLVoucherDetail voucherDetail = new GLVoucherDetail();
                //debit entry
                #region Debit
                //var costAccounts = (from li in _dbContext.WarpIssuances
                //                    from lii in _dbContext.WeftIssuances
                //                    join i in _dbContext.InvItems.Include(a => a.InvItemAccount) on li.ItemId equals i.Id

                //                    where li.YarnIssuanceId == id 
                //                    select new
                //                    {
                //                        GLCostofSaleAccountId=li.YarnIssuanceId,
                //                        TotalWarp = li.Quantity * i.AvgRate,
                //                        TotalWeft = lii.Quantity * i.AvgRate
                //                    }).GroupBy(l => l.GLCostofSaleAccountId)
                //               .Select(li => new YarnIssuanceItem
                //               {
                //                   Total = li.Sum(c => c.TotalWarp )+ li.Sum(c => c.TotalWeft),
                //                   YarnIssuanceId = li.Max( x => x.GLCostofSaleAccountId) //YarnIssuanceId is temporarily containing GLCostofSaleAccountId
                //               }).ToList();
                var WarpData = (from li in _dbContext.WarpIssuances
                                    join i in _dbContext.InvItems.Include(a => a.InvItemAccount) on li.ItemId equals i.Id
                                    where li.YarnIssuanceId == id 
                                    select new
                                    {
                                        YarnIssuanceId = li.YarnIssuanceId,
                                        TotalWarp = li.Quantity * i.AvgRate,
                                    }).GroupBy(l => l.YarnIssuanceId)
                               .Select(li => new YarnIssuanceItem
                               {
                                   Total = li.Sum(c => c.TotalWarp),
                                   YarnIssuanceId = li.FirstOrDefault().YarnIssuanceId //YarnIssuanceId is temporarily containing GLCostofSaleAccountId
                               }).ToList();

                var WeftData = (from lii in _dbContext.WeftIssuances
                                    join ii in _dbContext.InvItems.Include(a => a.InvItemAccount) on lii.ItemId equals ii.Id
                                    where lii.YarnIssuanceId == id
                                    select new
                                    {
                                        YarnIssuanceId = lii.YarnIssuanceId,
                                        TotalWeft = lii.Quantity * ii.AvgRate
                                    }).GroupBy(l => l.YarnIssuanceId)
                               .Select(li => new YarnIssuanceItem
                               {
                                   Total = li.Sum(c => c.TotalWeft),
                                   YarnIssuanceId = li.FirstOrDefault().YarnIssuanceId //YarnIssuanceId is temporarily containing GLCostofSaleAccountId
                               }).ToList();



                var mergedList = WarpData.Union(WeftData).ToList();
                var mergedData = mergedList.GroupBy(x => x.YarnIssuanceId).Select(a => new YarnIssuanceItem
                {
                    YarnIssuanceId = a.Select(x => x.YarnIssuanceId).FirstOrDefault(),
                    Total = a.Select(x => x.Total).Sum()
                });



                foreach (var item in mergedData)
                {
                    voucherDetail = new GLVoucherDetail();
                    //voucherDetail.AccountId = item.YarnIssuanceId;
                    //var accountId = (from setup in _dbContext.AppCompanySetups.Where(x => x.Name == "Yarn Debit Account") join
                    //       account in _dbContext.GLAccounts.Where(x=>!x.IsDeleted) on setup.Value equals account.Code
                    //       select account.Id).FirstOrDefault();
                    var accountId = (from yarnissue in _dbContext.YarnIssuances.Where(x => x.Id == item.YarnIssuanceId)
                                     join apSupplier in _dbContext.APSuppliers on yarnissue.VendorId equals apSupplier.Id
                                     select new { apSupplier.AccountId }).FirstOrDefault();
                    voucherDetail.AccountId = accountId.AccountId;
                    voucherDetail.Sequence = 20;
                    voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                    voucherDetail.Debit = item.Total;
                    //voucherDetail.Credit = 0;
                    voucherDetail.IsDeleted = false;
                    voucherDetail.CreatedBy = userId;
                    voucherDetail.CreatedDate = DateTime.Now;
                    voucherDetails.Add(voucherDetail);
                }
                #endregion Debit
                //credit entry
                #region Credit
                var cWarpData = (from li in _dbContext.WarpIssuances
                                     join i in _dbContext.InvItems.Include(a => a.InvItemAccount) on li.ItemId equals i.Id
                                     where li.YarnIssuanceId == id
                                     select new
                                     {
                                         GLAssetAccountId = i.InvItemAccount.GLAssetAccountId,
                                         TotalWarp = li.Quantity * i.AvgRate,
                                     }).GroupBy(l => l.GLAssetAccountId)
                               .Select(li => new YarnIssuanceItem
                               {
                                   Total = li.Sum(c => c.TotalWarp),
                                   YarnIssuanceId = li.FirstOrDefault().GLAssetAccountId //YarnIssuanceId is temporarily containing GLCostofSaleAccountId
                               }).ToList();

                var cWeftData = (from lii in _dbContext.WeftIssuances
                                 join ii in _dbContext.InvItems.Include(a => a.InvItemAccount) on lii.ItemId equals ii.Id
                                 where lii.YarnIssuanceId == id
                                 select new
                                 {
                                     GLAssetAccountId = ii.InvItemAccount.GLAssetAccountId,
                                     TotalWeft = lii.Quantity * ii.AvgRate
                                 }).GroupBy(l => l.GLAssetAccountId)
                                 .Select(li => new YarnIssuanceItem
                                 {
                                    Total = li.Sum(c => c.TotalWeft),
                                    YarnIssuanceId = li.FirstOrDefault().GLAssetAccountId //YarnIssuanceId is temporarily containing GLCostofSaleAccountId
                               }).ToList();

                var cmergedList = cWarpData.Union(cWeftData).ToList();
                var cmergedData = cmergedList.GroupBy(x => x.YarnIssuanceId).Select(a => new YarnIssuanceItem
                {
                    YarnIssuanceId = a.Select(x => x.YarnIssuanceId).FirstOrDefault(),
                    Total = a.Select(x => x.Total).Sum()
                });


                foreach (var item in cmergedData)
                {
                    voucherDetail = new GLVoucherDetail();
                     var accountId = (from setup in _dbContext.AppCompanySetups.Where(x => x.Name == "Yarn Credit Account") join
                            account in _dbContext.GLAccounts.Where(x=>!x.IsDeleted) on setup.Value equals account.Code
                            select account.Id).FirstOrDefault();
                    voucherDetail.AccountId = accountId;
                    voucherDetail.Sequence = 20;
                    voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                    //voucherDetail.Debit = 0;
                    voucherDetail.Credit = item.Total;
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
                        //var YarnIssuance = _dbContext.YarnIssuances.Find(id);
                        YarnIssuance.VoucherId = voucherId;
                        YarnIssuance.Status = "Approved";
                        YarnIssuance.ApprovedBy = userId;
                        YarnIssuance.ApprovedDate = DateTime.Now;
                        var entry = _dbContext.YarnIssuances.Update(YarnIssuance);
                        entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                        await _dbContext.SaveChangesAsync();
                       // var YarnIssuanceItems = _dbContext.YarnIssuanceItems.Where(i => i.YarnIssuanceId == id && i.IsDeleted == false).ToList();
                        var YarnIssuanceItems = (from li in _dbContext.WarpIssuances
                                                 from lii in _dbContext.WeftIssuances
                                                 join i in _dbContext.InvItems.Include(a => a.InvItemAccount) on li.ItemId equals i.Id
                                                 join ii in _dbContext.InvItems.Include(a => a.InvItemAccount) on lii.ItemId equals ii.Id
                                                 where li.YarnIssuanceId == id && lii.YarnIssuanceId == id
                                                 select new
                                                 {
                                                     GLAssetAccountId = i.InvItemAccount.GLAssetAccountId,
                                                     ItemId =li.ItemId,
                                                     QtyWarp =li.Quantity,
                                                     QtyWeft =lii.Quantity,

                                                     TotalWarp = li.Quantity * i.AvgRate,
                                                     TotalWeft = lii.Quantity * i.AvgRate
                                                 }).ToList();
                        foreach (var YarnIssuanceItem in YarnIssuanceItems)
                        {
                            var Qty = YarnIssuanceItem.QtyWarp + YarnIssuanceItem.QtyWeft;
                            var item = _dbContext.InvItems.Find(YarnIssuanceItem.ItemId);
                            item.StockQty = item.StockQty - (Qty);
                            item.StockValue = item.StockValue - (item.AvgRate * Qty);
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

        public IEnumerable<YarnIssuance> GetApproved()
        {
            var model = _dbContext.YarnIssuances.Include(s => s.WareHouse).Where(i => i.Status == "Approved" && i.TransactionType == "Issue Return" && !i.IsDeleted && i.CompanyId == HttpContext.Session.GetInt32("CompanyId").Value).AsEnumerable();
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
                    var YarnIssuance = _dbContext.YarnIssuances
                        .Where(a => a.Id == id && a.CompanyId == companyId && a.Status == "Approved" && !a.IsDeleted).FirstOrDefault();
                    YarnIssuance.Status = "Created";
                    YarnIssuance.ApprovedBy = null;
                    YarnIssuance.ApprovedDate = null;
                    var entry = _dbContext.YarnIssuances.Update(YarnIssuance);
                    entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                    await _dbContext.SaveChangesAsync();
                    var YarnIssuanceItems = _dbContext.YarnIssuanceItems.Where(i => i.YarnIssuanceId == id && i.IsDeleted == false).ToList();
                    foreach (var YarnIssuanceItem in YarnIssuanceItems)
                    {
                        var item = _dbContext.InvItems.Find(YarnIssuanceItem.ItemId);
                        item.StockQty = item.StockQty + YarnIssuanceItem.Qty;
                        item.StockValue = item.StockValue + (item.AvgRate * YarnIssuanceItem.Qty);
                        var dbEntry = _dbContext.InvItems.Update(item);
                        dbEntry.OriginalValues.SetValues(await dbEntry.GetDatabaseValuesAsync());
                        await _dbContext.SaveChangesAsync();
                    }
                    var storeVoucher = _dbContext.GLVoucherDetails.Where(x => x.VoucherId == YarnIssuance.VoucherId).ToList();
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
            var itemDelete = _dbContext.YarnIssuances.Find(id);
            itemDelete.IsDeleted = true;
            var entry = _dbContext.YarnIssuances.Update(itemDelete);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public int YarnIssuanceCountNo(int companyId)
        {
            int maxYarnIssuanceNo = 1;
            var YarnIssuance = _dbContext.YarnIssuances.Where(s => s.CompanyId == companyId && s.TransactionType == "Issue Return").ToList();
            if (YarnIssuance.Count > 0)
            {
                maxYarnIssuanceNo = YarnIssuance.Max(s => s.IssueNo);
                return maxYarnIssuanceNo + 1;
            }
            else
            {
                return maxYarnIssuanceNo;
            }
        }

        [HttpGet]
        public dynamic GetYarnIssuanceById(int id, int[] skipIds, int companyId)
        {
            var invoiceItems = _dbContext.YarnIssuanceItems.Include(i => i.YarnIssuance).Include(i => i.Item)
                .Where(i => i.YarnIssuance.TransactionType == "Issue" && i.YarnIssuance.CompanyId == companyId && i.IsDeleted == false && i.YarnIssuance.WareHouseId == id)
                .Where(i => !skipIds.Contains(i.Id)).ToList();
            return invoiceItems;
        }

        public YarnIssueViewModel GetSaleInvoiceItems(int id)
        {
            var item = new Vw_InvLedger();
            var configValues = new ConfigValues(_dbContext);
            item = _dbContext.VwInvLedgers.Include(i => i.Item).Where(i => i.ItemId == id).FirstOrDefault();
            YarnIssueViewModel viewModel = new YarnIssueViewModel();
            viewModel.ItemName = item.Item.Name;
            viewModel.ItemId = item.ItemId;
            viewModel.Brand = item.Brand;
            viewModel.UOM = configValues.GetUom(item.Item.Unit);

            return viewModel;
        }

    }
}
