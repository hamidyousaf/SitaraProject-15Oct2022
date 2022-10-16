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

namespace Numbers.Repository.Greige
{
    public class GRNRepo
    {
        private readonly NumbersDbContext _dbContext;
        private HttpContext HttpContext { get; }
        public GRNRepo(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public GRNRepo(NumbersDbContext context, HttpContext httpContext)
        {
            _dbContext = context;
            HttpContext = httpContext;
        }


        public IEnumerable<GRGRN> GetAll(int companyId)
        {
            IEnumerable<GRGRN> listRepo = _dbContext.GRGRNS.Where(v => v.IsDeleted == false && v.CompanyId == companyId)
                .OrderByDescending(v => v.Id).ToList();
            return listRepo;
        }

        [HttpPost]
        public async Task<bool> Create(GRGRNViewModel modelRepo)
        {
            try
            {
                //Add Master
                GRGRN model = new GRGRN();
                model = modelRepo.GRGRN;
                model.TransactionNo = modelRepo.GRGRN.TransactionNo;
                model.TransactionDate = modelRepo.GRGRN.TransactionDate;
                model.FoldingId = modelRepo.GRGRN.FoldingId;
                model.WeavingContractId = modelRepo.GRGRN.WeavingContractId;
                model.PurchaseContractId = modelRepo.GRGRN.PurchaseContractId;
                model.GreigeContractQuality = modelRepo.GRGRN.GreigeContractQuality;
                model.GreigeContractQualityLoom = modelRepo.GRGRN.GreigeContractQualityLoom;
                model.Status = "Created";
                model.IsActive = true;
                model.IsDeleted = false;
                model.CompanyId = modelRepo.GRGRN.CompanyId;
                model.Resp_Id = modelRepo.GRGRN.Resp_Id;
                model.CreatedBy = modelRepo.GRGRN.CreatedBy;
                model.CreatedDate = DateTime.Now.Date;
                _dbContext.GRGRNS.Add(model);
                _dbContext.SaveChanges();

                //Add Detail
                foreach (var item in modelRepo.GRGRNItem)
                {
                    GRGRNItem detail = new GRGRNItem();
                    detail.GRGRNId = model.Id;
                    detail.PenaltyId = item.PenaltyId;
                    detail.PenaltyRate = item.PenaltyRate;
                    detail.Quantity = item.Quantity;
                    detail.Amount = item.Amount;
                    detail.ActualPick = item.ActualPick;
                    detail.ActualWidth = item.ActualWidth;
                    detail.Sample = item.Sample;
                    _dbContext.GRGRNItems.Add(detail);
                    await _dbContext.SaveChangesAsync();
                }
                // Add Stacking Detail
                foreach (var item in modelRepo.GRStackingItem)
                {
                    GRStackingItem detail = new GRStackingItem();
                    detail.GRGRNId = model.Id;
                    detail.WareHouseId = item.WareHouseId;
                    detail.LocationId = item.LocationId;
                    detail.Quantity = item.Quantity;
                    detail.BalQty = item.BalQty;
                    detail.Rate = model.ContractRate;
                    detail.Amount =detail.Quantity * detail.Rate;

                    _dbContext.GRStackingItems.Add(detail);
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
        [HttpGet]
        public int Max(int companyId)
        {
            int transactionNo = 1;
            var result = _dbContext.GRGRNS.Where(x=>x.IsDeleted != true).ToList();
            if (result.Count() > 0)
            {
                transactionNo = result.Max(x=>x.TransactionNo) + 1;
            }
            return transactionNo;
        }
        [HttpPost]
        public async Task<bool> Update(GRGRNViewModel modelRepo)
        {
            try
            {
                var model = _dbContext.GRGRNS.FirstOrDefault(x => x.Id == modelRepo.GRGRN.Id);
                //model = modelRepo.GRGRN;
                model.TransactionNo = modelRepo.GRGRN.TransactionNo;
                model.TransactionDate = modelRepo.GRGRN.TransactionDate;
                model.WeavingContractId = modelRepo.GRGRN.WeavingContractId;
                model.PurchaseContractId = modelRepo.GRGRN.PurchaseContractId;
                model.FoldingId = modelRepo.GRGRN.FoldingId;
                model.VendorId = modelRepo.GRGRN.VendorId;
                model.VendorName = modelRepo.GRGRN.VendorName;
                model.ContractRate = modelRepo.GRGRN.ContractRate;
                model.GreigeContractQuality = modelRepo.GRGRN.GreigeContractQuality;
                model.GreigeContractQualityLoom = modelRepo.GRGRN.GreigeContractQualityLoom;
                model.End = modelRepo.GRGRN.End;
                model.Picks = modelRepo.GRGRN.Picks;
                model.Width = modelRepo.GRGRN.Width;
                model.WovenPieces = modelRepo.GRGRN.WovenPieces;
                model.WovenQty = modelRepo.GRGRN.WovenQty;
                model.MendedPieces = modelRepo.GRGRN.MendedPieces;
                model.MendedQty = modelRepo.GRGRN.MendedQty;
                model.FoldedPieces = modelRepo.GRGRN.FoldedPieces;
                model.FoldedQty = modelRepo.GRGRN.FoldedQty;
                model.RejectedQty = modelRepo.GRGRN.RejectedQty;
                model.Status = "Updated";
                model.IsActive = true;
                model.IsDeleted = false;
                model.UpdatedBy = modelRepo.GRGRN.UpdatedBy;
                model.UpdatedDate = DateTime.Now;
                _dbContext.GRGRNS.Update(model);
                await _dbContext.SaveChangesAsync();
                var existingDetail = _dbContext.GRGRNItems.Where(x => x.GRGRNId == modelRepo.GRGRN.Id).ToList();
                var existingDetailStacking = _dbContext.GRStackingItems.Where(x => x.GRGRNId == modelRepo.GRGRN.Id).ToList();
                //Deleting monthly target limit
                foreach (var detail in existingDetail)
                {
                    bool isExist = modelRepo.GRGRNItem.Any(x => x.Id == detail.Id);
                    if (!isExist)
                    {
                        //Handling Balance
                        //var igp = _dbContext.ARInwardGatePass.FirstOrDefault(x => x.Id == saleReturn.IGPId);
                        //igp.BaleBalance = igp.BaleBalance + detail.BalesBalance;
                        //_dbContext.ARInwardGatePass.Update(igp);
                        //----------
                        _dbContext.GRGRNItems.Remove(detail);
                        await _dbContext.SaveChangesAsync();
                    }
                }
                //Inserting/Updating monthly limit
                foreach (var detail in modelRepo.GRGRNItem)
                {
                    if (detail.Id == 0) //Inserting New Records
                    {
                        GRGRNItem iGPItems = new GRGRNItem();
                        iGPItems.GRGRNId = modelRepo.GRGRN.Id;
                        iGPItems.PenaltyId = detail.PenaltyId;
                        iGPItems.PenaltyRate = detail.PenaltyRate;
                        iGPItems.Quantity = detail.Quantity;
                        iGPItems.Amount = detail.Amount;
                        iGPItems.ActualPick = detail.ActualPick;
                        iGPItems.ActualWidth = detail.ActualWidth;
                        iGPItems.Sample = detail.Sample;
                        await _dbContext.GRGRNItems.AddAsync(iGPItems);

                    }
                    else   //Updating Records
                    {
                        var saleReturnItemsData = _dbContext.GRGRNItems.FirstOrDefault(x => x.Id == detail.Id);
                        saleReturnItemsData.PenaltyId = detail.PenaltyId;
                        saleReturnItemsData.PenaltyRate = detail.PenaltyRate;
                        saleReturnItemsData.Quantity = detail.Quantity;
                        saleReturnItemsData.Amount = detail.Amount;
                        saleReturnItemsData.ActualPick = detail.ActualPick;
                        saleReturnItemsData.ActualWidth = detail.ActualWidth;
                        saleReturnItemsData.Sample = detail.Sample;

                        _dbContext.GRGRNItems.Update(saleReturnItemsData);
                    }
                    await _dbContext.SaveChangesAsync();
                }

                foreach (var detail in existingDetailStacking)
                {
                    bool isExist = modelRepo.GRStackingItem.Any(x => x.Id == detail.Id);
                    if (!isExist)
                    {
                        _dbContext.GRStackingItems.Remove(detail);
                        await _dbContext.SaveChangesAsync();
                    }
                }

                //Inserting/Updating monthly limit
                foreach (var item in modelRepo.GRStackingItem)
                {
                    if (item.Id == 0) //Inserting New Records
                    {
                        GRStackingItem detail = new GRStackingItem();
                        detail.GRGRNId = model.Id;
                        detail.WareHouseId = item.WareHouseId;
                        detail.LocationId = item.LocationId;
                        detail.Quantity = item.Quantity;
                        detail.BalQty = item.BalQty;
                        detail.Rate = model.ContractRate;
                        detail.Amount = detail.Quantity * detail.Rate;
                        await _dbContext.GRStackingItems.AddAsync(detail);
                       

                    }
                    else   //Updating Records
                    {
                        var detail = _dbContext.GRStackingItems.FirstOrDefault(x => x.Id == item.Id);
                        detail.GRGRNId = model.Id;
                        detail.WareHouseId = item.WareHouseId;
                        detail.LocationId = item.LocationId;
                        detail.Quantity = item.Quantity;
                        detail.BalQty = item.BalQty;
                        detail.Rate = model.ContractRate;
                        detail.Amount = detail.Quantity * detail.Rate;
                        _dbContext.GRStackingItems.Update(detail);
                    }
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


        public async Task<bool> Approve(int id, string userId, int companyId)
        {
            GRGRN GRGRN = _dbContext.GRGRNS
           .Where(a => a.Status != "Approved" && a.CompanyId == companyId && a.Id == id && a.IsDeleted == false)
           .FirstOrDefault();
            try
            {
                //Create Voucher  
                GLVoucher voucherMaster = new GLVoucher();
                List<GLVoucherDetail> voucherDetails = new List<GLVoucherDetail>();
                string voucherDescription = string.Format(
                "GRN # : {0} ",
                GRGRN.TransactionNo);

                int voucherId = 0;
                voucherMaster.VoucherType = "G-GRN";
                voucherMaster.VoucherDate = GRGRN.TransactionDate;
                voucherMaster.Currency = "PKR";
                voucherMaster.CurrencyExchangeRate = 1;
                voucherMaster.Description = voucherDescription.Substring(0, voucherDescription.Length);
                voucherMaster.Status = "Approved";
                voucherMaster.ApprovedBy = userId;
                voucherMaster.ApprovedDate = DateTime.Now;
                voucherMaster.IsSystem = true;
                voucherMaster.ModuleName = "Greige/GRN";
                voucherMaster.ModuleId = id;

                GLVoucherDetail voucherDetail = new GLVoucherDetail();
                //debit entry
                #region Debit
                var costAccounts = (from li in _dbContext.GRGRNS
                                    join grItems in _dbContext.GRStackingItems on li.Id equals grItems.GRGRNId
                                    join gq in _dbContext.GRQuality on li.GreigeContractQuality equals gq.Description
                                    join i in _dbContext.InvItems.Include(a => a.InvItemAccount) on gq.ItemId equals i.Id
                                    where li.Id == id && li.IsDeleted == false
                                    select new
                                    {
                                        GLCostofSaleAccountId = i.InvItemAccount.GLCostofSaleAccountId,
                                        Total = li.ContractRate * grItems.Quantity
                                    }).GroupBy(l => l.GLCostofSaleAccountId)
                               .Select(li => new GRStackingItem
                               {
                                   BalQty = li.Sum(c => c.Total),
                                   GRGRNId = li.FirstOrDefault().GLCostofSaleAccountId //GRGRNId is temporarily containing GLCostofSaleAccountId
                                   }).ToList();
                foreach (var item in costAccounts)
                {
                    voucherDetail = new GLVoucherDetail();
                    //voucherDetail.AccountId = item.GRGRNId;
                    voucherDetail.AccountId = _dbContext.GLAccounts.Where(x => x.Name == "RAW MATERIAL - GREY CLOTH STOCK").FirstOrDefault().Id;
                    voucherDetail.Sequence = 20;
                    voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                    voucherDetail.Debit = item.BalQty;
                    //voucherDetail.Credit = 0;
                    voucherDetail.IsDeleted = false;
                    voucherDetail.CreatedBy = userId;
                    voucherDetail.CreatedDate = DateTime.Now;
                    voucherDetails.Add(voucherDetail);
                }
                #endregion Debit
                //credit entry
                #region Credit
                var assetAccounts = (from li in _dbContext.GRGRNS
                                     join grItems in _dbContext.GRStackingItems on li.Id equals grItems.GRGRNId
                                     join gq in _dbContext.GRQuality on li.GreigeContractQuality equals gq.Description
                                     join i in _dbContext.InvItems.Include(a => a.InvItemAccount) on gq.ItemId equals i.Id
                                     where li.Id == id && li.IsDeleted == false
                                     select new
                                     {
                                         GLAssetAccountId = i.InvItemAccount.GLAssetAccountId,
                                         Total = li.ContractRate * grItems.Quantity
                                     }).GroupBy(l => l.GLAssetAccountId)
                               .Select(li => new GRStackingItem
                               {
                                   BalQty = li.Sum(c => c.Total),
                                   GRGRNId = li.FirstOrDefault().GLAssetAccountId //GRGRNId is temporarily containing GLAssetAccountId
                               }).ToList();
                foreach (var item in assetAccounts)
                {
                    voucherDetail = new GLVoucherDetail();
                    voucherDetail.AccountId = _dbContext.GLAccounts.Where(x => x.Name == "UNBILLED PURCHASES").FirstOrDefault().Id;
                    voucherDetail.Sequence = 20;
                    voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                    //voucherDetail.Debit = 0;
                    voucherDetail.Credit = item.BalQty;
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
                    //var GRGRN = _dbContext.GRGRNs.Find(id);
                     GRGRN.VoucherId = voucherId;
                    GRGRN.IsApproved = true;
                    GRGRN.Status = "Approved";
                    GRGRN.ApprovedBy = userId;
                    GRGRN.ApprovedDate = DateTime.Now;
                    var entry = _dbContext.GRGRNS.Update(GRGRN);
                    entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                    await _dbContext.SaveChangesAsync();
                    //var GRGRNItems = _dbContext.GRGRNS.Where(i => i.Id == id).ToList();
                    var GRGRNItems = _dbContext.GRStackingItems.Where(i => i.GRGRNId == id).ToList();
                    foreach (var GRGRNItem in GRGRNItems)
                    {
                        var item = (from li in _dbContext.GRGRNS
                                    join gq in _dbContext.GRQuality on li.GreigeContractQuality equals gq.Description
                                    join i in _dbContext.InvItems.Include(a => a.InvItemAccount) on gq.ItemId equals i.Id
                                    where li.Id == id && li.IsDeleted == false
                                    select new { item =i,GRN =li}).FirstOrDefault();

                        // var item = _dbContext.InvItems.Find(invoiceItem.ItemId);
                        item.item.StockQty = item.item.StockQty + GRGRNItem.Quantity;
                            // item.StockValue = item.StockValue + (GRGRNItem.Rate * GRGRNItem.FoldedQty);
                            item.item.StockValue = item.item.StockValue + (item.GRN.ContractRate * GRGRNItem.Quantity);
                        if (item.item.StockQty != 0)
                        {
                                item.item.AvgRate = Math.Round(item.item.StockValue / item.item.StockQty, 6);
                        }
                        //item.StockQty = item.StockQty - invoiceItem.AcceptedQty;
                        //item.StockValue = item.StockValue - (item.AvgRate * invoiceItem.AcceptedQty);
                        var dbEntry = _dbContext.InvItems.Update(item.item);
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
        public async Task<Dictionary<string, string>> UnApprove(int id)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                Dictionary<string, string> response = new Dictionary<string, string>();
                try
                {
                    int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                    string userId = HttpContext.Session.GetString("UserId");
                    var YarnIssuance = _dbContext.GRGRNS
                        .Where(a => a.Id == id && a.CompanyId == companyId && a.Status == "Approved" && !a.IsDeleted).FirstOrDefault();
                    YarnIssuance.Status = "Created";
                    YarnIssuance.ApprovedBy = null;
                    YarnIssuance.ApprovedDate = DateTime.Now;
                    var entry = _dbContext.GRGRNS.Update(YarnIssuance);
                    entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                    await _dbContext.SaveChangesAsync();
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
                    response.Add("message", "GRN has been Un-Approved Successfully");
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
    }
}
