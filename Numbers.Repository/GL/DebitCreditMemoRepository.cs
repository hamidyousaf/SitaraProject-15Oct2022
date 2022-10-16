using Microsoft.AspNetCore.Http;
using Numbers.Entity.Models;
using Numbers.Repository.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numbers.Repository.GL
{
    public class DebitCreditMemoRepository
    {
        private HttpContext HttpContext { get; }
        private readonly NumbersDbContext _dbContext;
        public DebitCreditMemoRepository(NumbersDbContext dbContext, HttpContext httpContext)
        {
            _dbContext = dbContext;
            HttpContext = httpContext;
        }
        public DebitCreditMemoRepository(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> Approve(int id, string userId, int companyId)
        {
            APSupplier supplier = new APSupplier();
            ARCustomer customer = new ARCustomer();

            GLDebitCreditMemo memo = _dbContext.GLDebitCreditMemos.FirstOrDefault(p => p.Status == "Created" && p.CompanyId == companyId && p.Id == id && p.IsDeleted == false);
            List<GLDebitCreditMemoDetail> memoDetails = _dbContext.GLDebitCreditMemoDetails.Where(p => p.GLDebitCreditMemoId == id).ToList();
            
            if(memo.PartyType == "Customer")
            {
                customer = _dbContext.ARCustomers.FirstOrDefault(x => x.Id == memo.PartyId);
            }
            if (memo.PartyType == "Supplier")
            {
                supplier = _dbContext.APSuppliers.FirstOrDefault(x => x.Id == memo.PartyId);
            }
            try
            {
                //Create Voucher
                VoucherHelper voucher = new VoucherHelper(_dbContext, HttpContext);
                GLVoucher voucherMaster = new GLVoucher();
                List<GLVoucherDetail> voucherDetails = new List<GLVoucherDetail>();

                string type = "";
                string party = customer.Id != 0 ? customer.Name : supplier.Name;
                switch (memo.TransactionTypeId)
                {
                    case 870: //Note: 870 mean Invoice
                        type = "Invoice Memo";
                        break;
                    case 868: //Note: 868 mean Credit Note
                        type = "Credit Note";
                        break;
                    case 869: //Note: 869 mean Debit Note
                        type = "Debit Note";
                        break;
                    default: //Note: 870 mean Invoice
                        type = "";
                        break;
                }
                string voucherDescription = string.Format(
                    "{0} # : {1} of {2} {3}",
                    type,
                    memo.TransactionNo,
                    party,
                    memo.Remarks);

                int voucherId;
                voucherMaster.VoucherType = "MEMO";
                voucherMaster.VoucherDate = memo.TransactionDate;
                //voucherMaster.Reference = receipt.reference;
                voucherMaster.Currency = "PKR";
                voucherMaster.CurrencyExchangeRate = 1;
                voucherMaster.Description = memo.Remarks;
                voucherMaster.Status = "Approved";
                voucherMaster.ApprovedBy = userId;
                voucherMaster.ApprovedDate = DateTime.Now;
                voucherMaster.IsSystem = true;
                voucherMaster.ModuleName = "GL/DebitCreditMemo";
                voucherMaster.ModuleId = id;

                //Voucher Details
                var amount = memo.GrandTotal;
                //DEbit Entry
                GLVoucherDetail voucherDetail = new GLVoucherDetail();
                if (customer != null && memo.TransactionTypeId == 870) //Note: 870 mean Invoice 
                {
                    voucherDetail.AccountId = customer != null ? customer.AccountId : supplier.AccountId;
                    voucherDetail.Debit = amount;
                    voucherDetail.Credit = 0;
                }
                if (supplier != null && memo.TransactionTypeId == 869) // Note: 869 mean Debit Note
                {
                    voucherDetail.AccountId = customer != null ? customer.AccountId : supplier.AccountId;
                    voucherDetail.Debit = amount;
                    voucherDetail.Credit = 0;
                }
                if (customer != null && memo.TransactionTypeId == 868) //Note: 868 mean Credit Note
                {
                    voucherDetail.AccountId = customer != null ? customer.AccountId : supplier.AccountId;
                    voucherDetail.Debit = 0;
                    voucherDetail.Credit = amount;
                }
                voucherDetail.Sequence = 10;
                voucherDetail.Description = memo.Remarks;
                voucherDetail.IsDeleted = false;
                voucherDetail.CreatedBy = userId;
                voucherDetail.CreatedDate = DateTime.Now;
                voucherDetails.Add(voucherDetail);

                //Credit Entry
                #region Sales Tax 
                var incomTaxAccounts = (from li in _dbContext.GLDebitCreditMemoDetails
                                        join t in _dbContext.AppTaxes on li.TaxId equals t.Id
                                        where li.GLDebitCreditMemoId == id
                                        select new
                                        {
                                            li.TaxAmount,
                                            t.IncomeTaxAccountId
                                        }).GroupBy(l => l.IncomeTaxAccountId)
                                        .Select(li => new APPaymentInvoice
                                        {
                                            TaxAmount = li.Sum(c => c.TaxAmount),
                                            TaxId = li.FirstOrDefault().IncomeTaxAccountId //Tax id is temporarily containing SalesTaxAccountId
                                        }).ToList();
                foreach (var income in incomTaxAccounts)
                {
                    if (income.TaxId > 0 && income.TaxAmount > 0)
                    {
                        voucherDetail = new GLVoucherDetail();
                        voucherDetail.AccountId = income.TaxId;
                        voucherDetail.Sequence = 20;
                        voucherDetail.Description = string.Format("Tax Amount Against {0} # {1}", type , memo.TransactionNo);
                        
                        if (customer != null && memo.TransactionTypeId == 868) //Note: 868 mean Credit Note
                        {
                            voucherDetail.Credit = 0;
                            voucherDetail.Debit = income.TaxAmount;
                        }
                        else
                        {
                            voucherDetail.Credit = income.TaxAmount;
                            voucherDetail.Debit = 0;
                        }
                        voucherDetail.IsDeleted = false;
                        voucherDetail.CreatedBy = userId;
                        voucherDetail.CreatedDate = DateTime.Now;
                        voucherDetails.Add(voucherDetail);
                    }
                }
                #endregion Sales Tax                    

                #region Line Items
                foreach (var item in memoDetails)
                {
                    //Payment Voucher Debit Entry
                    voucherDetail = new GLVoucherDetail();
                    voucherDetail.AccountId = item.GLAccountId;
                    voucherDetail.Sequence = 30;
                    voucherDetail.Description = voucherDescription;
                    if (customer != null && memo.TransactionTypeId == 868) //Note: 868 mean Credit Note
                    {
                        voucherDetail.Debit = item.Total;
                        voucherDetail.Credit = 0;
                    }
                    else
                    {
                        voucherDetail.Debit = 0;
                        voucherDetail.Credit = item.Total;
                    }
                    voucherDetail.IsDeleted = false;
                    voucherDetail.CreatedBy = userId;
                    voucherDetail.CreatedDate = DateTime.Now;
                    voucherDetails.Add(voucherDetail);
                }

                #endregion Line Items

                #region Debit Discount Account
                foreach (var item in memoDetails)
                {
                    //Payment Voucher Debit Entry
                    voucherDetail = new GLVoucherDetail();
                    voucherDetail.AccountId = customer != null ? 206 : 400;  //Note: 206 mean  GLAccount Code: 04.01.04.0001
                    voucherDetail.Sequence = 30;
                    voucherDetail.Description = voucherDescription;
                    voucherDetail.Debit = item.DiscountAmount;
                    voucherDetail.Credit = 0;
                    if (customer != null && memo.TransactionTypeId == 868) //Note: 868 mean Credit Note
                    {
                        voucherDetail.Debit = 0;
                        voucherDetail.Credit = item.DiscountAmount;
                    }
                    else
                    {
                        voucherDetail.Debit = item.DiscountAmount;
                        voucherDetail.Credit = 0;
                    }
                    voucherDetail.IsDeleted = false;
                    voucherDetail.CreatedBy = userId;
                    voucherDetail.CreatedDate = DateTime.Now;
                    voucherDetails.Add(voucherDetail);
                }
                #endregion Debit Discount
                //Create Voucher 
                voucherId = voucher.CreateVoucher(voucherMaster, voucherDetails);
                if (voucherId != 0)
                {
                    using (var transaction = _dbContext.Database.BeginTransaction())
                    {
                        try
                        {
                            memo.VoucherId = voucherId;
                            memo.Status = "Approved";
                            memo.ApprovedBy = userId;
                            memo.ApprovedDate = DateTime.Now;
                            //On approval updating Payment
                            //var entry = 
                            _dbContext.Update(memo);
                            //entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                            await _dbContext.SaveChangesAsync();
                            //var items = _dbContext.APPaymentInvoices.Where(p => p.IsDeleted == false && p.PaymentId == id).ToList();
                            //foreach (var item in items)
                            //{
                            //    if (item.InvoiceId != 0)
                            //    {
                            //        var invoice = _dbContext.APPurchases.Find(item.InvoiceId);
                            //        //invoice.InvoiceAmount = payment.GrandTotal - invoice.ReceiptTotal - invoice.AdjustmentTotal;
                            //        invoice.PaymentTotal = invoice.PaymentTotal + item.LineTotal;
                            //        var dbEntry = _dbContext.APPurchases.Update(invoice);
                            //        dbEntry.OriginalValues.SetValues(await dbEntry.GetDatabaseValuesAsync());
                            //        await _dbContext.SaveChangesAsync();
                            //    }
                            //}
                            transaction.Commit();
                        }
                        catch (Exception exc)
                        {
                            Console.WriteLine(exc.Message);
                            transaction.Rollback();
                            return false;
                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                string message = ex.Message.ToString();
                return false;
            }
        }
        public async Task<bool> Delete(int id, int companyId)
        {
            var deleteItem = _dbContext.GLDebitCreditMemos.Where(v => v.IsDeleted == false && v.Id == id && v.CompanyId == companyId).FirstOrDefault();
            if (deleteItem == null)
            {
                return false;
            }
            else
            {
                deleteItem.IsDeleted = true;
                var entry = _dbContext.GLDebitCreditMemos.Update(deleteItem);
                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                await _dbContext.SaveChangesAsync();
                return true;
            }
        }
        public async Task<bool> UnApproveVoucher(int id, int companyId)
        {

            var memo = _dbContext.GLDebitCreditMemos
                            .Where(v => v.IsDeleted == false && v.Id == id && v.Status == "Approved" && v.CompanyId == companyId).FirstOrDefault();
            if (memo == null)
            {
                return false;
            }
            else
            {
                using (var transaction = _dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        var voucherDetail = _dbContext.GLVoucherDetails.Where(v => v.VoucherId == memo.VoucherId).ToList();

                        foreach (var item in voucherDetail)
                        {
                            var tracker = _dbContext.GLVoucherDetails.Remove(item);
                            tracker.OriginalValues.SetValues(await tracker.GetDatabaseValuesAsync());
                            await _dbContext.SaveChangesAsync();
                        }
                        memo.Status = "Created";
                        memo.ApprovedBy = null;
                        memo.ApprovedDate = DateTime.Now;
                        var entry = _dbContext.GLDebitCreditMemos.Update(memo);
                        entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                        await _dbContext.SaveChangesAsync();
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine(exc);
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }
    }
}
