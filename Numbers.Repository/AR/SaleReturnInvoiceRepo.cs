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

namespace Numbers.Repository.AR
{
    public class SaleReturnInvoiceRepo
    {
        private readonly NumbersDbContext _dbContext;
        private HttpContext HttpContext { get; }
        public SaleReturnInvoiceRepo(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public SaleReturnInvoiceRepo(NumbersDbContext dbContext, HttpContext httpContext)
        {
            _dbContext = dbContext;
            HttpContext = httpContext;
        }
        public SaleReturnInvoiceViewModel GetById(int id)
        {
            ARSaleReturnInvoice ARSaleReturnInvoice = _dbContext.ARSaleReturnInvoice.Find(id);
            var viewModel = new SaleReturnInvoiceViewModel();
            viewModel.Id = ARSaleReturnInvoice.Id;
            viewModel.InvoiceNo = ARSaleReturnInvoice.InvoiceNo;
            viewModel.InvoiceDate = ARSaleReturnInvoice.InvoiceDate;
            viewModel.CustomerId = ARSaleReturnInvoice.CustomerId;
            viewModel.PackingId = ARSaleReturnInvoice.PackingId;
            return viewModel;
        }
        public int MaxNo(int companyId)
        {
            int maxNo = 1;
            var orders = _dbContext.ARSaleReturnInvoice.Where(c => c.CompanyId == companyId).ToList();
            if (orders.Count > 0)
            {
                maxNo = orders.Max(o => o.InvoiceNo);
                return maxNo + 1;
            }
            else
            {
                return maxNo;
            }
        }
        [HttpPost]
        public async Task<bool> Create(SaleReturnInvoiceViewModel modelRepo,IFormCollection collection)
        {
            try
            {
                ARSaleReturnInvoiceItems[] ARSaleReturnInvoiceItems = JsonConvert.DeserializeObject<ARSaleReturnInvoiceItems[]>(collection["PackingDetail"]);

                ARSaleReturnInvoice ARSaleReturnInvoice = new ARSaleReturnInvoice();
                ARSaleReturnInvoice.InvoiceNo = modelRepo.InvoiceNo;
                ARSaleReturnInvoice.InvoiceDate = modelRepo.InvoiceDate;
                ARSaleReturnInvoice.CustomerId = modelRepo.CustomerId;
                ARSaleReturnInvoice.PackingId = modelRepo.PackingId;
                ARSaleReturnInvoice.TotalQty = modelRepo.TotalQty;
                ARSaleReturnInvoice.TotalDiscount = modelRepo.TotalDiscount;
                ARSaleReturnInvoice.TotalAmount = modelRepo.TotalAmount;
 
                ARSaleReturnInvoice.Status = "Created";
                ARSaleReturnInvoice.IsActive = true;
                ARSaleReturnInvoice.IsDeleted = false;
                ARSaleReturnInvoice.CompanyId = modelRepo.CompanyId;
                ARSaleReturnInvoice.Resp_ID = modelRepo.Resp_ID;
                ARSaleReturnInvoice.CreatedBy = modelRepo.CreatedBy;
                ARSaleReturnInvoice.CreatedDate = DateTime.Now.Date;
                ARSaleReturnInvoice.Resp_ID = modelRepo.Resp_ID;
                _dbContext.ARSaleReturnInvoice.Add(ARSaleReturnInvoice);
                _dbContext.SaveChanges();
              
                foreach (var item in ARSaleReturnInvoiceItems)
                {
                    ARSaleReturnInvoiceItems packingItems = new ARSaleReturnInvoiceItems();
                    packingItems.SRInvoiceId = ARSaleReturnInvoice.Id;
                    packingItems.FourthItemCategoryId = item.FourthItemCategoryId;
                    packingItems.ItemId = item.ItemId;
                    packingItems.SaleInvoiceId = item.SaleInvoiceId;
                    packingItems.SaleInvoiceItemId = item.SaleInvoiceItemId;
                    packingItems.Qty = item.Qty;
                    packingItems.SeasonId = item.SeasonId;
                    packingItems.ReasonTypeId = item.ReasonTypeId;
                    packingItems.ReturnTypeId = item.ReturnTypeId;
                    packingItems.Discount = item.Discount;
                    packingItems.Rate = item.Rate;
                    packingItems.Amount = item.Amount;
                    _dbContext.ARSaleReturnInvoiceItems.Add(packingItems);
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

        public async Task<bool> Approve(int id, int companyId, string userId)
        {
            ARInvoice invoice = _dbContext.ARInvoices
             .Include(c => c.Customer)
             .Where(a => a.Status == "Created" && a.TransactionType == "Sale Return" && a.CompanyId == companyId && a.Id == id && a.IsDeleted == false)
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
                voucherId = helper.CreateVoucher(voucherMaster, voucherDetails);
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

        [HttpPost]
        public async Task<bool> Update(SaleReturnInvoiceViewModel modelRepo,IFormCollection collection)
        {
            try
            {
                ARSaleReturnInvoiceItems[] ARSaleReturnInvoiceItems = JsonConvert.DeserializeObject<ARSaleReturnInvoiceItems[]>(collection["PackingDetail"]);
                //var existingDetail = _dbContext.ARSaleReturnInvoiceItems.Where(x => x.PackingId == modelRepo.Id).ToList();

                //foreach (var detail in existingDetail)
                //{
                //    bool isExist = modelRepo.PackingItemsList.Any(x => x.Id == detail.Id);
                //    if (!isExist)
                //    {
                //        _dbContext.ARSaleReturnInvoiceItems.Remove(detail);
                //        await _dbContext.SaveChangesAsync();
                //    }
                //}
                ARSaleReturnInvoice ARSaleReturnInvoice = _dbContext.ARSaleReturnInvoice.FirstOrDefault(x => x.Id == modelRepo.Id);
                ARSaleReturnInvoice.InvoiceNo = modelRepo.InvoiceNo;
                ARSaleReturnInvoice.InvoiceDate = modelRepo.InvoiceDate;
                ARSaleReturnInvoice.CustomerId = modelRepo.CustomerId;
                ARSaleReturnInvoice.PackingId = modelRepo.PackingId;
                ARSaleReturnInvoice.TotalQty = modelRepo.TotalQty;
                ARSaleReturnInvoice.TotalDiscount = modelRepo.TotalDiscount;
                ARSaleReturnInvoice.TotalAmount = modelRepo.TotalAmount;
                ARSaleReturnInvoice.IsActive = true;
                ARSaleReturnInvoice.IsDeleted = false;
                ARSaleReturnInvoice.UpdatedBy = modelRepo.UpdatedBy;
                ARSaleReturnInvoice.UpdatedDate = DateTime.Now;
                _dbContext.ARSaleReturnInvoice.Update(ARSaleReturnInvoice);
              
                
                foreach (var detail in ARSaleReturnInvoiceItems)
                {
                    if (detail.Id == 0)  
                    {
                        ARSaleReturnInvoiceItems packingItems = new ARSaleReturnInvoiceItems();
                        packingItems.SRInvoiceId = modelRepo.Id;
                        packingItems.FourthItemCategoryId = detail.FourthItemCategoryId;
                        packingItems.Qty = detail.Qty;
                        packingItems.ItemId = detail.ItemId;
                        packingItems.SaleInvoiceId = detail.SaleInvoiceId;
                        packingItems.SaleInvoiceItemId = detail.SaleInvoiceItemId;
                        packingItems.ReasonTypeId = detail.ReasonTypeId;
                        packingItems.ReturnTypeId = detail.ReturnTypeId;
                        packingItems.SeasonId = detail.SeasonId;
                        packingItems.Discount = detail.Discount;
                        packingItems.Rate = detail.Rate;
                        packingItems.Amount = detail.Amount;
                        await _dbContext.ARSaleReturnInvoiceItems.AddAsync(packingItems);

                    }
                    else    
                    {
                        var ARSaleReturnInvoiceItemsData = _dbContext.ARSaleReturnInvoiceItems.FirstOrDefault(x => x.Id == detail.Id);
                        ARSaleReturnInvoiceItemsData.SRInvoiceId = modelRepo.Id;
                        ARSaleReturnInvoiceItemsData.FourthItemCategoryId = detail.FourthItemCategoryId;
                        ARSaleReturnInvoiceItemsData.ItemId = detail.ItemId;
                        ARSaleReturnInvoiceItemsData.Qty = detail.Qty;
                        ARSaleReturnInvoiceItemsData.SaleInvoiceId = detail.SaleInvoiceId;
                        ARSaleReturnInvoiceItemsData.SaleInvoiceItemId = detail.SaleInvoiceItemId;
                        ARSaleReturnInvoiceItemsData.ReasonTypeId = detail.ReasonTypeId;
                        ARSaleReturnInvoiceItemsData.ReturnTypeId = detail.ReturnTypeId;
                        ARSaleReturnInvoiceItemsData.SeasonId = detail.SeasonId;
                        ARSaleReturnInvoiceItemsData.Discount = detail.Discount;
                        ARSaleReturnInvoiceItemsData.Rate = detail.Rate;
                        ARSaleReturnInvoiceItemsData.Amount = detail.Amount;
                        _dbContext.ARSaleReturnInvoiceItems.Update(ARSaleReturnInvoiceItemsData);
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
    }
}
