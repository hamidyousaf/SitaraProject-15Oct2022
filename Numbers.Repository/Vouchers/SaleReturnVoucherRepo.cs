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
    public class SaleReturnVoucherRepo
    {
        private HttpContext HttpContext { get; }
        private readonly NumbersDbContext _dbContext;
        public SaleReturnVoucherRepo(NumbersDbContext dbContext, HttpContext httpContext)
        {
            HttpContext = httpContext;
            _dbContext = dbContext;
        }

        public async Task<bool> Approve(int id,int companyId,string userId)
        {
            ARInvoice invoice = _dbContext.ARInvoices
             .Include(c => c.Customer)
             .Where(a => a.Status == "Created" && a.TransactionType=="Sale Return" && a.CompanyId == companyId && a.Id == id && a.IsDeleted == false)
             .FirstOrDefault();
            try
            {
                int voucherId;
                GLVoucher voucherMaster = new GLVoucher();
                List<GLVoucherDetail> voucherDetails = new List<GLVoucherDetail>();
                string voucherDescription = string.Format(
                "Invoice # : {0} of  " +
                "{1} {2}",
                invoice.InvoiceNo,
                invoice.Customer.Name, invoice.Remarks);

                voucherMaster.VoucherType = "SR";
                voucherMaster.ReferenceId = invoice.CustomerId;
                voucherMaster.VoucherDate = invoice.InvoiceDate;
                voucherMaster.Reference = invoice.ReferenceNo;
                voucherMaster.Currency = invoice.Currency;
                voucherMaster.CurrencyExchangeRate = invoice.CurrencyExchangeRate;
                voucherMaster.Description = invoice.Remarks;
                voucherMaster.Status = "Approved";
                voucherMaster.ApprovedBy = userId;
                voucherMaster.ApprovedDate = DateTime.Now;
                voucherMaster.IsSystem = true;
                voucherMaster.ModuleName = "AR/Sales Return";
                voucherMaster.ModuleId = id;
                voucherMaster.Amount = invoice.GrandTotal;
                //Voucher Details
                GLVoucherDetail voucherDetail = new GLVoucherDetail();
                //Credit Entry
                #region Customer Entry
                var invoiceItems = _dbContext.ARInvoiceItems.Where(i => i.InvoiceId == invoice.Id && !i.IsDeleted).ToList();
                var amount = invoiceItems.Sum(s => s.LineTotal);
                //Credit Entry
                voucherDetail = new GLVoucherDetail();
                voucherDetail.AccountId = invoice.Customer.AccountId;
                voucherDetail.Sequence = 30;
                voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                voucherDetail.Debit = 0;
                voucherDetail.Credit = Math.Abs(amount);
                voucherDetail.IsDeleted = false;
                voucherDetail.CreatedBy = userId;
                voucherDetail.CreatedDate = DateTime.Now;
                voucherDetails.Add(voucherDetail);
                #endregion Customer Entry
                //Debit Entry
                #region Sales Account
                var itemAccounts = (from li in _dbContext.ARInvoiceItems
                                    join i in _dbContext.InvItems.Include(a => a.InvItemAccount) on li.ItemId equals i.Id
                                    where li.InvoiceId == id
                                    select new
                                    {
                                        li.Total,
                                        i.InvItemAccount.GLSaleAccountId
                                    }).GroupBy(l => l.GLSaleAccountId)
                                    .Select(li => new ARInvoiceItem
                                    {
                                        Total = li.Sum(c => c.Total),
                                        InvoiceId = li.FirstOrDefault().GLSaleAccountId //invoice id is temporarily containing GLSaleAccountId
                                    }).ToList();

                foreach (var item in itemAccounts)
                {
                    voucherDetail = new GLVoucherDetail();
                    voucherDetail.AccountId = item.InvoiceId;
                    voucherDetail.Sequence = 10;
                    voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                    voucherDetail.Debit = Math.Abs(item.Total); 
                    voucherDetail.Credit = 0;
                    voucherDetail.IsDeleted = false;
                    voucherDetail.CreatedBy = userId;
                    voucherDetail.CreatedDate = DateTime.Now;
                    voucherDetails.Add(voucherDetail);
                }
                #endregion Sales Account
                #region Sales Tax 
                var saleTaxAccounts = (from li in _dbContext.ARInvoiceItems
                                       join i in _dbContext.InvItems on li.ItemId equals i.Id
                                       join t in _dbContext.AppTaxes on li.TaxSlab equals t.Id
                                       where li.InvoiceId == id
                                       select new
                                       {
                                           li.SalesTaxAmount,
                                           t.SalesTaxAccountId
                                       }).GroupBy(l => l.SalesTaxAccountId)
                                        .Select(li => new ARInvoiceItem
                                        {
                                            SalesTaxAmount = li.Sum(c => c.SalesTaxAmount),
                                            InvoiceId = li.FirstOrDefault().SalesTaxAccountId //invoice id is temporarily containing SalesTaxAccountId
                                        }).ToList();
                foreach (var item in saleTaxAccounts)
                {
                    if (item.SalesTaxAmount < 0)
                    {
                        voucherDetail = new GLVoucherDetail();
                        voucherDetail.AccountId = item.InvoiceId;
                        voucherDetail.Sequence = 20;
                        voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                       // voucherDetail.Debit = Math.Abs(item.SalesTaxAmount);
                        voucherDetail.Debit = item.SalesTaxAmount;
                        voucherDetail.Credit = 0;
                        voucherDetail.IsDeleted = false;
                        voucherDetail.CreatedBy = userId;
                        voucherDetail.CreatedDate = DateTime.Now;
                        voucherDetails.Add(voucherDetail);
                    }
                    
                }
                #endregion Sales Tax
                #region Cost of Sale Account
                //Credit Entry
                var costOfSaleAccount = (from li in _dbContext.ARInvoiceItems
                                         join i in _dbContext.InvItems.Include(a => a.InvItemAccount) on li.ItemId equals i.Id
                                         where li.InvoiceId == id && li.IsDeleted == false
                                         select new
                                         {
                                             li.CostofSales,
                                             i.InvItemAccount.GLCostofSaleAccountId
                                         }).GroupBy(l => l.GLCostofSaleAccountId)
                                    .Select(li => new ARInvoiceItem
                                    {
                                        CostofSales = li.Sum(c => c.CostofSales),
                                        InvoiceId = li.FirstOrDefault().GLCostofSaleAccountId //invoice id is temporarily containing GLCostofSaleAccountId
                                    }).ToList();

                foreach (var item in costOfSaleAccount)
                {
                    voucherDetail = new GLVoucherDetail();
                    voucherDetail.AccountId = item.InvoiceId;
                    voucherDetail.Sequence = 40;
                    voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                    voucherDetail.Debit = 0;
                    voucherDetail.Credit = Math.Abs(item.CostofSales);
                    voucherDetail.IsDeleted = false;
                    voucherDetail.CreatedBy = userId;
                    voucherDetail.CreatedDate = DateTime.Now;
                    voucherDetails.Add(voucherDetail);
                }
                #endregion Cost of Sale Account
                #region Asset/Stock Account
                //Debit Entry
                var assetAcount = (from li in _dbContext.ARInvoiceItems
                                   join i in _dbContext.InvItems.Include(a => a.InvItemAccount) on li.ItemId equals i.Id
                                   where li.InvoiceId == id && li.IsDeleted == false
                                   select new
                                   {
                                       li.CostofSales,
                                       i.InvItemAccount.GLAssetAccountId
                                   }).GroupBy(l => l.GLAssetAccountId)
                                    .Select(li => new ARInvoiceItem
                                    {
                                        CostofSales = li.Sum(c => c.CostofSales),
                                        InvoiceId = li.FirstOrDefault().GLAssetAccountId //invoice id is temporarily containing GLAssetAccountId
                                    }).ToList();

                foreach (var item in assetAcount)
                {
                    voucherDetail = new GLVoucherDetail();
                    voucherDetail.AccountId = item.InvoiceId;
                    voucherDetail.Sequence = 50;
                    voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                    voucherDetail.Debit = Math.Abs(item.CostofSales);
                    voucherDetail.Credit = 0;
                    voucherDetail.IsDeleted = false;
                    voucherDetail.CreatedBy = userId;
                    voucherDetail.CreatedDate = DateTime.Now;
                    voucherDetails.Add(voucherDetail);
                }
                #endregion Asset/Stock Account
                

                //Create Voucher 
                VoucherHelper helper = new VoucherHelper(_dbContext, HttpContext);
                voucherId =  helper.CreateVoucher(voucherMaster, voucherDetails);
                if (voucherId != 0)
                {
                    invoice.VoucherId = voucherId;
                    invoice.Status = "Approved";
                    invoice.ApprovedBy = userId;
                    invoice.ApprovedDate = DateTime.Now;
                    using (var transaction = _dbContext.Database.BeginTransaction())
                    {
                        try
                        {
                            foreach (var invoiceItem in invoiceItems)
                            {
                                var item = _dbContext.InvItems.Find(invoiceItem.ItemId);
                                item.StockQty = item.StockQty + Math.Abs(invoiceItem.Qty);
                                item.StockValue = item.StockValue + (item.AvgRate * Math.Abs(invoiceItem.Qty));
                                //if (item.StockQty != 0)
                                //{
                                //    item.AvgRate = Math.Round(item.StockValue / item.StockQty, 6);
                                //}
                                var dbEntry = _dbContext.InvItems.Update(item);
                                dbEntry.OriginalValues.SetValues(await dbEntry.GetDatabaseValuesAsync());
                                await _dbContext.SaveChangesAsync();
                            }
                            var entry = _dbContext.Update(invoice);
                            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                            await _dbContext.SaveChangesAsync();
                            transaction.Commit();
                            return true;
                        }
                        catch(Exception exc)
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
        public IEnumerable<ARInvoice> GetApprovedVouchers()
        {
            return _dbContext.ARInvoices
               .Where(v => v.Status == "Approved" && v.IsDeleted == false && v.TransactionType == "Sale Return" && v.CompanyId == HttpContext.Session.GetInt32("CompanyId")).ToList();
        }
        public async Task<bool> UnApproveVoucher(int id)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                    var voucher = _dbContext.ARInvoices
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

                        var invoiceItems = _dbContext.ARInvoiceItems.Where(i => i.InvoiceId == id && !i.IsDeleted).ToList();
                        foreach (var invoiceItem in invoiceItems)
                        {
                            var item = _dbContext.InvItems.Find(invoiceItem.ItemId);
                            item.StockQty = item.StockQty - Math.Abs(invoiceItem.Qty);
                            item.StockValue = item.StockValue - (item.AvgRate * Math.Abs(invoiceItem.Qty));
                            var dbEntry = _dbContext.InvItems.Update(item);
                            dbEntry.OriginalValues.SetValues(await dbEntry.GetDatabaseValuesAsync());
                            await _dbContext.SaveChangesAsync();
                        }
                        var entry = _dbContext.ARInvoices.Update(voucher);
                        entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                        await _dbContext.SaveChangesAsync();
                        transaction.Commit();
                    }
                    return true;
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc);
                    return false;
                }
            }      
        }
    }
}
