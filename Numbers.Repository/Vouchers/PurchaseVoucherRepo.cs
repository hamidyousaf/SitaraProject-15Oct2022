using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class PurchaseVoucherRepo
    {
        private HttpContext HttpContext { get; }
        private readonly NumbersDbContext _dbContext;
        public PurchaseVoucherRepo(NumbersDbContext dbContext, HttpContext httpContext)
        {
            _dbContext = dbContext;
            HttpContext = httpContext;
        }

        public async Task <bool> ApprovePurchaseInvoice(int id, int companyId, string userId)
        {
            
                APPurchase purchase = _dbContext.APPurchases
                                                .Where(a => a.Status == "Created" && a.CompanyId == companyId && a.Id == id && !a.IsDeleted)
                                                    .Include(s => s.Supplier)
                                                        .FirstOrDefault();
            try
            {
                //Create Voucher
                int voucherId = 0;
                //Own Booking 
                GLVoucher voucherMaster = new GLVoucher();
                List<GLVoucherDetail> voucherDetails = new List<GLVoucherDetail>();
                string voucherDescription = string.Format(
                "Purchase # : {0} of  " +
                "{1} {2}",
                purchase.PurchaseNo,
                purchase.Supplier.Name, purchase.Remarks);

                voucherMaster.VoucherType = "PUR";
                voucherMaster.VoucherDate = purchase.PurchaseDate;
                voucherMaster.Reference = purchase.ReferenceNo;
                voucherMaster.Currency = purchase.Currency;
                voucherMaster.CurrencyExchangeRate = 1;
                voucherMaster.Description = voucherDescription.Substring(0, voucherDescription.Length);
                voucherMaster.Status = "Approved";
                voucherMaster.IsSystem = true;
                voucherMaster.CompanyId = companyId;
                voucherMaster.ModuleName = "AP/Purchase";
                voucherMaster.ModuleId = id;

                //Voucher Details
                var purchaseItems = _dbContext.APPurchaseItems.Where(p => p.PurchaseId == id && p.IsDeleted == false).ToList();
                var discount = purchaseItems.Sum(d => d.DiscountAmount);  
                //Credit Entry
                GLVoucherDetail voucherDetail = new GLVoucherDetail();
                voucherDetail.AccountId = purchase.Supplier.AccountId;
                voucherDetail.Sequence = 99;
                voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                voucherDetail.Debit = 0;
                voucherDetail.Credit = purchase.GrandTotal;
                voucherDetail.IsDeleted = false;
                voucherDetail.CreatedBy = userId;
                voucherDetail.CreatedDate = DateTime.Now;
                voucherDetails.Add(voucherDetail);
                //for discount
                //if (discount > 0)
                //{
                //    var accountCode = _dbContext.AppCompanyConfigs.Where(c => c.CompanyId == companyId && c.ConfigName == "Discount Received" && c.IsActive).FirstOrDefault().UserValue1;
                //    var discountAccount = _dbContext.GLAccounts.Where(a => a.Code == accountCode && a.CompanyId == companyId && a.IsActive).FirstOrDefault().Id;
                //    voucherDetail = new GLVoucherDetail();
                //    voucherDetail.AccountId = discountAccount;
                //    voucherDetail.Sequence = 99;
                //    voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                //    voucherDetail.Debit = 0;
                //    voucherDetail.Credit = discount;
                //    voucherDetail.IsDeleted = false;
                //    voucherDetail.CreatedBy = userId;
                //    voucherDetail.CreatedDate = DateTime.Now;
                //    voucherDetails.Add(voucherDetail);
                //}
                //Debit Entry
                #region Purchase Account
                var itemAccounts = (from li in _dbContext.APPurchaseItems
                                    join i in _dbContext.InvItems.Include(a => a.InvItemAccount) on li.ItemId equals i.Id
                                    where li.PurchaseId == id
                                    select new
                                    {
                                        li.Total,
                                        li.DiscountAmount,
                                        i.InvItemAccount.GLAssetAccountId
                                    }).GroupBy(l => l.GLAssetAccountId)

                           .Select(li => new APPurchaseItem
                           {
                               Total = li.Sum(c => c.Total) - li.Sum(d => d.DiscountAmount),
                               PurchaseId = li.FirstOrDefault().GLAssetAccountId //invoice id is temporarily containing GLSaleAccountId
                           }).ToList();

                foreach (var item in itemAccounts)
                {
                    voucherDetail = new GLVoucherDetail();
                    //voucherDetail.AccountId = item.PurchaseId;
                    var accountId = (from setup in _dbContext.AppCompanySetups.Where(x => x.Name == "Purchase Inv Debit Account") join
                            account in _dbContext.GLAccounts.Where(x=>!x.IsDeleted) on setup.Value equals account.Code
                            select account.Id).FirstOrDefault();
                    voucherDetail.AccountId = accountId; // Purchase Invoice Debit Account
                    voucherDetail.Sequence = 10;
                    voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                    voucherDetail.Debit = item.Total;
                    voucherDetail.Credit = 0;
                    voucherDetail.IsDeleted = false;
                    voucherDetail.CreatedBy = userId;
                    voucherDetail.CreatedDate = DateTime.Now;
                    voucherDetails.Add(voucherDetail);
                }
                #endregion Purchase Account
                #region Sale Tax 
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
                                            PurchaseId = li.FirstOrDefault().SalesTaxAccountId //invoice id is temporarily containing SalesTaxAccountId
                                        }).ToList();
                foreach (var item in saleTaxAccounts)
                {
                    if (item.SalesTaxAmount > 0)
                    {
                        voucherDetail = new GLVoucherDetail();
                        voucherDetail.AccountId = item.PurchaseId;
                        voucherDetail.Sequence = 10;
                        voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                        voucherDetail.Debit = item.SalesTaxAmount; //Sir waqas reversed this
                        voucherDetail.Credit = 0;
                        voucherDetail.IsDeleted = false;
                        voucherDetail.CreatedBy = userId;
                        voucherDetail.CreatedDate = DateTime.Now;
                        voucherDetails.Add(voucherDetail);
                    }
                }
                #endregion Sale Tax
                #region Excise Tax 
                var exciseTaxAccounts = (from li in _dbContext.APPurchaseItems
                                         join i in _dbContext.InvItems on li.ItemId equals i.Id
                                         join t in _dbContext.AppTaxes on li.TaxId equals t.Id
                                         where li.PurchaseId == id
                                         select new
                                         {
                                             li.ExciseTaxAmount,
                                             t.ExciseTaxAccountId
                                         }).GroupBy(l => l.ExciseTaxAccountId)
                                        .Select(li => new APPurchaseItem
                                        {
                                            ExciseTaxAmount = li.Sum(c => c.ExciseTaxAmount),
                                            PurchaseId = li.FirstOrDefault().ExciseTaxAccountId //invoice id is temporarily containing SalesTaxAccountId
                                        }).ToList();
                foreach (var item in exciseTaxAccounts)
                {
                    if (item.ExciseTaxAmount > 0)
                    {
                        voucherDetail = new GLVoucherDetail();
                        voucherDetail.AccountId = item.PurchaseId;
                        voucherDetail.Sequence = 10;
                        voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                        voucherDetail.Debit = item.ExciseTaxAmount;
                        voucherDetail.Credit = 0;
                        voucherDetail.IsDeleted = false;
                        voucherDetail.CreatedBy = userId;
                        voucherDetail.CreatedDate = DateTime.Now;
                        voucherDetails.Add(voucherDetail);
                    }
                }
                #endregion Excise Tax
                //Create Voucher 
                var helper = new VoucherHelper(_dbContext, HttpContext);
                voucherId = helper.CreateVoucher(voucherMaster, voucherDetails);
                if (voucherId != 0)
                {
                    purchase.VoucherId = voucherId;
                    purchase.Status = "Approved";
                    purchase.ApprovedBy = userId;
                    purchase.ApprovedDate = DateTime.Now;
                    //var entry =
                    _dbContext.APPurchases.Update(purchase);
                    //entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                    using (var transaction = _dbContext.Database.BeginTransaction())
                    {
                        await _dbContext.SaveChangesAsync();
                        //foreach (var purchaseItem in purchaseItems)
                        //{
                        //    var item = _dbContext.InvItems.Find(purchaseItem.ItemId);
                        //    item.StockQty = item.StockQty + purchaseItem.Qty;
                        //    item.StockValue = item.StockValue + (purchaseItem.Rate * purchaseItem.Qty);
                        //    if (item.StockQty != 0)
                        //    {
                        //        item.AvgRate = Math.Round(item.StockValue / item.StockQty, 6);
                        //    }     
                        //    var dbEntry = _dbContext.InvItems.Update(item);
                        //    dbEntry.OriginalValues.SetValues(await dbEntry.GetDatabaseValuesAsync());
                        //    await _dbContext.SaveChangesAsync();
                        //}
                        //await _dbContext.SaveChangesAsync();
                        var lineItems = _dbContext.APPurchaseItems.Where(i => i.IsDeleted == false && i.PurchaseId == id).ToList();
                        foreach (var item in lineItems)
                        {
                            if (item.PurchaseOrderItemId != 0)
                            {
                                var purchaseOrderItem = _dbContext.APPurchaseOrderItems.Find(item.PurchaseOrderItemId);
                                purchaseOrderItem.PurchaseQty = purchaseOrderItem.PurchaseQty + item.Qty;
                                var dbEntry = _dbContext.APPurchaseOrderItems.Update(purchaseOrderItem);
                                dbEntry.OriginalValues.SetValues(await dbEntry.GetDatabaseValuesAsync());
                                await _dbContext.SaveChangesAsync();
                            }
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
                var message = ex.Message.ToString();
                return false;
            }
        }
        public async Task<bool> ApproveServiceInvoice(int id, int companyId, string userId)
        {
            APPurchase purchase = _dbContext.APPurchases
             .Include(c => c.Supplier)
             .Where(a => a.Status == "Created" && a.TransactionType == "Service" && a.CompanyId == companyId && a.Id == id && a.IsDeleted == false)
             .FirstOrDefault();
            try
            {
                //Create Voucher
                VoucherHelper voucher = new VoucherHelper(_dbContext, HttpContext);
                GLVoucher voucherMaster = new GLVoucher();
                List<GLVoucherDetail> voucherDetails = new List<GLVoucherDetail>();
                string voucherDescription = string.Format(
                "Purchase # : {0} of  " +
                "{1} {2}",
                purchase.PurchaseNo,
                purchase.Supplier.Name, purchase.Remarks);

                int voucherId;
                voucherMaster.VoucherType = "P-SRV";
                voucherMaster.VoucherDate = purchase.PurchaseDate;
                voucherMaster.Reference = purchase.ReferenceNo;
                voucherMaster.Currency = purchase.Currency;
                voucherMaster.CurrencyExchangeRate = purchase.CurrencyExchangeRate;
                voucherMaster.Description = voucherDescription;
                voucherMaster.Status = "Approved";
                voucherMaster.ApprovedBy = userId;
                voucherMaster.ApprovedDate = DateTime.Now;
                voucherMaster.IsSystem = true;
                voucherMaster.ModuleName = "AP/Service Invoice";
                voucherMaster.CompanyId = companyId;
                voucherMaster.ModuleId = id;

                //Voucher Details
                var invoiceItems = _dbContext.APPurchaseItems.Where(i => i.PurchaseId == purchase.Id && !i.IsDeleted).ToList();
                var amount = invoiceItems.Sum(s => s.LineTotal);
                var discount = invoiceItems.Sum(s => s.DiscountAmount);

                GLVoucherDetail voucherDetail;
                //Credit Entry
                voucherDetail = new GLVoucherDetail();
                voucherDetail.AccountId = purchase.Supplier.AccountId;
                voucherDetail.Sequence = 99;
                voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                voucherDetail.Debit = 0;
                voucherDetail.Credit = amount;
                voucherDetail.IsDeleted = false;
                voucherDetail.CreatedBy = userId;
                voucherDetail.CreatedDate = DateTime.Now;
                voucherDetails.Add(voucherDetail);
                //for discount
                //if (discount > 0)
                //{
                //    var accountCode = _dbContext.AppCompanyConfigs.Where(c => c.CompanyId == companyId && c.ConfigName == "Discount Received" && c.IsActive).FirstOrDefault().UserValue1;
                //    var discountAccount = _dbContext.GLAccounts.Where(a => a.Code == accountCode && a.CompanyId == companyId && a.IsActive).FirstOrDefault().Id;
                //    voucherDetail = new GLVoucherDetail();
                //    voucherDetail.AccountId = discountAccount;
                //    voucherDetail.Sequence = 99;
                //    voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                //    voucherDetail.Debit = 0;
                //    voucherDetail.Credit = discount;
                //    voucherDetail.IsDeleted = false;
                //    voucherDetail.CreatedBy = userId;
                //    voucherDetail.CreatedDate = DateTime.Now;
                //    voucherDetails.Add(voucherDetail);
                //}
                #region Purchase Account
                //Debit Entry
                var lineItems = (from li in _dbContext.APPurchaseItems
                                 where li.PurchaseId == id
                                 select new
                                 {
                                     li.Total,
                                     li.DiscountAmount,
                                     li.ServiceAccountId
                                 }).GroupBy(l => l.ServiceAccountId)
                                           .Select(li => new APPurchaseItem
                                           {
                                               Total = li.Sum(c => c.Total) - li.Sum(c => c.DiscountAmount),
                                               PurchaseId = li.FirstOrDefault().ServiceAccountId //invoice id is temporarily containing SalesTaxAccountId
                                           })
                                       .ToList();
                var saleTaxAccounts = (from li in _dbContext.APPurchaseItems
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
                               PurchaseId = li.FirstOrDefault().SalesTaxAccountId //invoice id is temporarily containing SalesTaxAccountId
                           })
                       .ToList();
                foreach (var item in saleTaxAccounts)
                {
                    if (item.PurchaseId != 0)
                    {
                        voucherDetail = new GLVoucherDetail();
                        voucherDetail.AccountId = item.PurchaseId;
                        voucherDetail.Sequence = 20;
                        voucherDetail.Description = string.Format("Sales Tax Service Invoice # {0}", purchase.PurchaseNo);
                        voucherDetail.Debit = item.SalesTaxAmount;
                        voucherDetail.Credit = 0;
                        voucherDetail.IsDeleted = false;
                        voucherDetail.CreatedBy = userId;
                        voucherDetail.CreatedDate = DateTime.Now;
                        voucherDetails.Add(voucherDetail);
                    }
                }
                foreach (var item in lineItems)
                {
                    voucherDetail = new GLVoucherDetail();
                    voucherDetail.AccountId = item.PurchaseId;
                    voucherDetail.Sequence = 20;
                    voucherDetail.Description = voucherDescription;
                    if (item.Total < 0)//-ve
                    {
                        voucherDetail.Debit = 0;
                        voucherDetail.Credit = Math.Abs(item.Total);
                    }
                    else//+ve
                    {
                        voucherDetail.Debit = Math.Abs(item.Total);
                        voucherDetail.Credit = 0;

                    }

                    voucherDetail.IsDeleted = false;
                    voucherDetail.CreatedBy = userId;
                    voucherDetail.CreatedDate = DateTime.Now;
                    voucherDetails.Add(voucherDetail);
                }

                #endregion Sale Account


                //Create Voucher 
                voucherId = voucher.CreateVoucher(voucherMaster, voucherDetails);
                if (voucherId > 0)
                {
                    purchase.VoucherId = voucherId;
                    purchase.Status = "Approved";
                    purchase.ApprovedBy = userId;
                    purchase.ApprovedDate = DateTime.Now;
                    using (var transaction = _dbContext.Database.BeginTransaction())
                    {
                        //On approval updating Invoice
                        var entry = _dbContext.Update(purchase);
                        entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                        await _dbContext.SaveChangesAsync();
                        //on
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
                return false;
            }
        }
        
        public IEnumerable<APPurchase> GetApprovedPurchaseInvoices()
        {
            return _dbContext.APPurchases
               .Where(v => v.Status == "Approved" && v.IsDeleted == false && v.TransactionType == "Purchase" && v.CompanyId == HttpContext.Session.GetInt32("CompanyId")).ToList();
        }
        public IEnumerable<APPurchase> GetApprovedVouchers()
        {
            return _dbContext.APPurchases
               .Where(v => v.Status == "Approved" && v.IsDeleted == false && v.TransactionType == "Service" && v.CompanyId == HttpContext.Session.GetInt32("CompanyId")).ToList();
        }
        public async Task<bool> UnApproveServiceVoucher(int id)
        {
            try
            {
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                var voucher = _dbContext.APPurchases
                                .Where(v => v.IsDeleted == false && v.Id == id && v.Status == "Approved" && v.CompanyId == companyId).FirstOrDefault();
                if (voucher != null)
                {
                    var voucherDetail = _dbContext.GLVoucherDetails.Where(v => v.VoucherId == voucher.VoucherId).ToList();
                    foreach (var item in voucherDetail)
                    {
                        var tracker = _dbContext.GLVoucherDetails.Remove(item);
                        tracker.OriginalValues.SetValues(await tracker.GetDatabaseValuesAsync());
                        await _dbContext.SaveChangesAsync();
                    }
                    voucher.Status = "Created";
                    voucher.ApprovedBy = null;
                    voucher.ApprovedDate = null;
                    var entry = _dbContext.APPurchases.Update(voucher);
                    entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                    await _dbContext.SaveChangesAsync();
                   }
                return true;
            }
            catch(Exception exc)
            {
                Console.WriteLine(exc.Message);
                return false;
            }
        }
        public async Task<bool> UnApprovePurchaseVoucher(int id)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                    var voucher = _dbContext.APPurchases
                                    .Where(v => v.IsDeleted == false && v.Id == id && v.Status == "Approved" && v.CompanyId == companyId).FirstOrDefault();
                    if (voucher != null)
                    {
                        var voucherDetail = _dbContext.GLVoucherDetails.Where(v => v.VoucherId == voucher.VoucherId).ToList();
                        foreach (var item in voucherDetail)
                        {
                            var tracker = _dbContext.GLVoucherDetails.Remove(item);
                            tracker.OriginalValues.SetValues(await tracker.GetDatabaseValuesAsync());
                            await _dbContext.SaveChangesAsync();
                        }
                        voucher.Status = "Created";
                        voucher.ApprovedBy = null;
                        voucher.ApprovedDate = null;
                        var entry = _dbContext.APPurchases.Update(voucher);
                        entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                        await _dbContext.SaveChangesAsync();

                        var purchaseItems = _dbContext.APPurchaseItems.Where(p => p.IsDeleted == false && p.PurchaseId == id).ToList();
                        //foreach (var purchaseItem in purchaseItems)
                        //{
                        //    var item = _dbContext.InvItems.Find(purchaseItem.ItemId);
                        //    item.StockQty = item.StockQty - purchaseItem.Qty;
                        //    item.StockValue = item.StockValue - (purchaseItem.Rate * purchaseItem.Qty);
                        //    if (item.StockQty != 0)
                        //    {
                        //        item.AvgRate = Math.Round(item.StockValue / item.StockQty, 6);
                        //    }
                        //    var dbEntry = _dbContext.InvItems.Update(item);
                        //    dbEntry.OriginalValues.SetValues(await dbEntry.GetDatabaseValuesAsync());
                        //    await _dbContext.SaveChangesAsync();
                        //}
                        foreach (var item in purchaseItems)
                        {
                            if (item.PurchaseOrderItemId != 0)
                            {
                                var po = _dbContext.APPurchaseOrderItems.Find(item.PurchaseOrderItemId);
                                po.PurchaseQty = po.PurchaseQty - item.Qty;
                                var dbEntry = _dbContext.APPurchaseOrderItems.Update(po);
                                dbEntry.OriginalValues.SetValues(await dbEntry.GetDatabaseValuesAsync());
                                await _dbContext.SaveChangesAsync();
                            }
                        }
                        transaction.Commit();
                    }
                    return true;
                }
                catch (Exception exc)
                {
                    transaction.Rollback();
                    Console.WriteLine(exc);
                    return false;
                }
            }
        }
    }

}
