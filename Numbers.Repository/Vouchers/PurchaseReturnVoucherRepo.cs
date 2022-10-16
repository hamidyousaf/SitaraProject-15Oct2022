using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using Numbers.Repository.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numbers.Repository.Vouchers
{
    public class PurchaseReturnVoucherRepo
    {
        private HttpContext HttpContext { get; }
        private readonly NumbersDbContext _dbContext;
        public PurchaseReturnVoucherRepo(NumbersDbContext dbContext, HttpContext httpContext)
        {
            HttpContext = httpContext;
            _dbContext = dbContext;
        }

        public async Task<bool> Approve(int id, string userId, int companyId)
        {
            APPurchase purchase = _dbContext.APPurchases
             .Include(p => p.Supplier)
             .Where(p => p.Status == "Created" && p.TransactionType == "Purchase Return" && p.CompanyId == companyId && p.Id == id && p.IsDeleted == false)
             .FirstOrDefault();
            try
            {
                int voucherId;
                GLVoucher voucherMaster = new GLVoucher();
                List<GLVoucherDetail> voucherDetails = new List<GLVoucherDetail>();
                string voucherDescription = string.Format(
                "Purchase # : {0} of  " +
                "{1} {2}",
                purchase.PurchaseNo,
                purchase.Supplier.Name, purchase.Remarks);
                voucherMaster.VoucherType = "PR";
                voucherMaster.VoucherDate = purchase.PurchaseDate;
                voucherMaster.Reference = purchase.ReferenceNo;
                voucherMaster.Currency = purchase.Currency;
                voucherMaster.CurrencyExchangeRate = purchase.CurrencyExchangeRate;
                voucherMaster.Description = purchase.Remarks;
                voucherMaster.Status = "Approved";
                voucherMaster.ApprovedBy = userId;
                voucherMaster.ApprovedDate = DateTime.Now;
                voucherMaster.IsSystem = true;
                voucherMaster.ModuleName = "AP/PurchaseReturn";
                voucherMaster.ModuleId = id;
                voucherMaster.ReferenceId = purchase.SupplierId;
                //Voucher Details
                var invoiceItems = _dbContext.APPurchaseItems.Where(i => i.PurchaseId == purchase.Id && !i.IsDeleted).ToList();
                var amount = invoiceItems.Sum(s => s.LineTotal);
                //Debit Entry
                GLVoucherDetail voucherDetail = new GLVoucherDetail();
                voucherDetail.AccountId = purchase.Supplier.AccountId;
                voucherDetail.Sequence = 10;
                voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                voucherDetail.Debit = Math.Abs(amount);
                voucherDetail.Credit = 0;
                voucherDetail.IsDeleted = false;
                voucherDetail.CreatedBy = userId;
                voucherDetail.CreatedDate = DateTime.Now;
                voucherDetails.Add(voucherDetail);

                #region Sale Account
                //Credit Entry
                var itemAccounts = (from li in _dbContext.APPurchaseItems
                                    join i in _dbContext.InvItems.Include(a => a.InvItemAccount) on li.ItemId equals i.Id
                                    where li.PurchaseId == id
                                    select new
                                    {
                                        li.Total,
                                        i.InvItemAccount.GLAssetAccountId
                                    }).GroupBy(l => l.GLAssetAccountId)
                                    .Select(li => new APPurchaseItem
                                    {
                                        Total = li.Sum(c => c.Total),
                                        PurchaseId = li.FirstOrDefault().GLAssetAccountId //PurchaseId is temporarily containing GLSaleAccountId
                                    }).ToList();

                foreach (var item in itemAccounts)
                {
                    voucherDetail = new GLVoucherDetail();
                    voucherDetail.AccountId = item.PurchaseId;
                    voucherDetail.Sequence = 20;
                    voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                    voucherDetail.Debit = 0;
                    voucherDetail.Credit = Math.Abs(item.Total);
                    voucherDetail.IsDeleted = false;
                    voucherDetail.CreatedBy = userId;
                    voucherDetail.CreatedDate = DateTime.Now;
                    voucherDetails.Add(voucherDetail);
                }
                #endregion Sale Account
                #region Sales Tax 
                var saleTaxAccounts = (from li in _dbContext.APPurchaseItems
                                       join i in _dbContext.InvItems on li.ItemId equals i.Id
                                       join t in _dbContext.AppTaxes on li.TaxId equals t.Id
                                       where li.PurchaseId == id
                                       select new
                                       {
                                           li.SalesTaxAmount,
                                           t.SalesTaxAccountId
                                       }).GroupBy(l => l.SalesTaxAccountId)
                                        .Select(li => new APPurchaseItem
                                        {
                                            SalesTaxAmount = li.Sum(c => c.SalesTaxAmount),
                                            PurchaseId = li.FirstOrDefault().SalesTaxAccountId //PurchaseId is temporarily containing SalesTaxAccountId
                                        }).ToList();
                foreach (var item in saleTaxAccounts)
                {
                    if(item.SalesTaxAmount!=0)
                    { 
                    voucherDetail = new GLVoucherDetail();
                    voucherDetail.AccountId = item.PurchaseId;
                    voucherDetail.Sequence = 20;
                    voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                    voucherDetail.Debit = 0;
                    voucherDetail.Credit = Math.Abs(item.SalesTaxAmount);
                    voucherDetail.IsDeleted = false;
                    voucherDetail.CreatedBy = userId;
                    voucherDetail.CreatedDate = DateTime.Now;
                    voucherDetails.Add(voucherDetail);
                    }
                }
                #endregion Sales Tax
                //Create Voucher 
                VoucherHelper helper = new VoucherHelper(_dbContext, HttpContext);
                voucherId = helper.CreateVoucher(voucherMaster, voucherDetails);
                if (voucherId != 0)
                {
                    purchase.VoucherId = voucherId;
                    purchase.Status = "Approved";
                    purchase.ApprovedBy = userId;
                    purchase.ApprovedDate = DateTime.Now;
                    using (var transaction = _dbContext.Database.BeginTransaction())
                    {
                        try
                        {
                            foreach (var purchaseItem in invoiceItems)
                            {
                                var item = _dbContext.InvItems.Find(purchaseItem.ItemId);
                                item.StockQty = item.StockQty + purchaseItem.Qty;
                                item.StockValue = item.StockValue + (purchaseItem.Rate * purchaseItem.Qty);
                                if (item.StockQty != 0)
                                {
                                    item.AvgRate = Math.Round(item.StockValue / item.StockQty, 6);
                                }
                                var dbEntry = _dbContext.InvItems.Update(item);
                                dbEntry.OriginalValues.SetValues(await dbEntry.GetDatabaseValuesAsync());
                                await _dbContext.SaveChangesAsync();
                            }
                            var entry = _dbContext.Update(purchase);
                            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                            await _dbContext.SaveChangesAsync();
                            transaction.Commit();
                            return true;
                        }
                        catch (Exception exc)
                        {
                            Console.WriteLine(exc.Message);
                            transaction.Rollback();
                            return false;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                var message = ex.Message.ToString();
                return false;
            }
        }

    }
}
