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
    public class PaymentVoucherRepo
    {
        private HttpContext HttpContext { get; }
        private readonly NumbersDbContext _dbContext;
        public PaymentVoucherRepo(NumbersDbContext dbContext, HttpContext httpContext)
        {
            _dbContext = dbContext;
            HttpContext = httpContext;
        }

        public async Task<bool> Approve(int id, string userId, int companyId)
        {
            APPayment payment = _dbContext.APPayments
             .Include(p => p.Supplier)
             .Include(p => p.BankCashAccount)
             .Where(p => p.Status == "Created" && p.CompanyId == companyId && p.Id == id && p.IsDeleted == false)
             .FirstOrDefault();
            try
            {
                //Create Voucher
                VoucherHelper voucher = new VoucherHelper(_dbContext, HttpContext);
                GLVoucher voucherMaster = new GLVoucher();
                List<GLVoucherDetail> voucherDetails = new List<GLVoucherDetail>();
                string voucherDescription = string.Format(
                "Payment # : {0} of  " +
                "{1} {2}",
                payment.PaymentNo,
                payment.Supplier.Name, payment.Remarks);

                int voucherId;
                voucherMaster.VoucherType = "PYMT"; 
                voucherMaster.VoucherDate = payment.PaymentDate;
                //voucherMaster.Reference = receipt.reference;
                voucherMaster.Currency = payment.Currency;
                voucherMaster.CurrencyExchangeRate = payment.CurrencyExchangeRate;
                voucherMaster.Description = payment.Remarks;
                voucherMaster.Status = "Approved";
                voucherMaster.ApprovedBy = userId;
                voucherMaster.ApprovedDate = DateTime.Now;
                voucherMaster.IsSystem = true;
                voucherMaster.ModuleName = "AP/Payment";
                voucherMaster.ModuleId = id;

                //Voucher Details
                var amount = payment.TotalPaidAmount;
                //DEbit Entry
                GLVoucherDetail voucherDetail = new GLVoucherDetail();
                voucherDetail.AccountId = payment.Supplier.AccountId;
                voucherDetail.Sequence = 10;
                voucherDetail.Description = payment.Remarks;
                voucherDetail.Debit = amount;
                voucherDetail.Credit = 0;
                voucherDetail.IsDeleted = false;
                voucherDetail.CreatedBy = userId;
                voucherDetail.CreatedDate = DateTime.Now;
                voucherDetails.Add(voucherDetail);

                //Credit Entry
                #region Sales Tax 
                var incomTaxAccounts = (from li in _dbContext.APPaymentInvoices
                                        join t in _dbContext.AppTaxes on li.TaxId equals t.Id
                                        where li.PaymentId == id
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
                        voucherDetail.Description = string.Format("Tax Amount Against Payment # {0}", payment.PaymentNo);
                        voucherDetail.Credit = income.TaxAmount;
                        voucherDetail.Debit = 0;
                        voucherDetail.IsDeleted = false;
                        voucherDetail.CreatedBy = userId;
                        voucherDetail.CreatedDate = DateTime.Now;
                        voucherDetails.Add(voucherDetail);
                    }
                }
                #endregion Sales Tax

                #region Line Items
                //Payment Voucher Debit Entry
                voucherDetail = new GLVoucherDetail();
                voucherDetail.AccountId = payment.BankCashAccount.AccountId;
                voucherDetail.Sequence = 30;
                voucherDetail.Description = voucherDescription;
                voucherDetail.Debit = 0;
                voucherDetail.Credit = payment.GrandTotal;
                voucherDetail.IsDeleted = false;
                voucherDetail.CreatedBy = userId;
                voucherDetail.CreatedDate = DateTime.Now;
                voucherDetails.Add(voucherDetail);
                #endregion Line Items

                //Create Voucher 
                voucherId = voucher.CreateVoucher(voucherMaster, voucherDetails);
                if (voucherId != 0)
                {
                    using (var transaction = _dbContext.Database.BeginTransaction())
                    {
                        try
                        {
                            payment.VoucherId = voucherId;
                            payment.Status = "Approved";
                            payment.ApprovedBy = userId;
                            payment.ApprovedDate = DateTime.Now;
                            //On approval updating Payment
                            //var entry = 
                                _dbContext.Update(payment);
                            //entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                            await _dbContext.SaveChangesAsync();
                            var items = _dbContext.APPaymentInvoices.Where(p => p.IsDeleted == false && p.PaymentId == id).ToList();
                            foreach (var item in items)
                            {
                                if (item.InvoiceId != 0)
                                {
                                    var invoice = _dbContext.APPurchases.Find(item.InvoiceId);
                                    //invoice.InvoiceAmount = payment.GrandTotal - invoice.ReceiptTotal - invoice.AdjustmentTotal;
                                    invoice.PaymentTotal = invoice.PaymentTotal + item.LineTotal;
                                    var dbEntry = _dbContext.APPurchases.Update(invoice);
                                    dbEntry.OriginalValues.SetValues(await dbEntry.GetDatabaseValuesAsync());
                                    await _dbContext.SaveChangesAsync();
                                }
                            }
                            transaction.Commit();
                        }
                        catch(Exception exc)
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
    }
}
