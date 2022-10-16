using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Repository.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numbers.Repository.AP
{
    public class GRPaymentRepo
    {
        private HttpContext HttpContext { get; }
        private readonly NumbersDbContext _dbContext;
        public GRPaymentRepo(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public GRPaymentRepo(NumbersDbContext dbContext, HttpContext httpContext)
        {
            _dbContext = dbContext;
            HttpContext = httpContext;
        }
        public string UploadFile(IFormFile img)
        {
            string filesList = "";
            if (img != null)
            {
                if (img.Length > 0)
                {
                    var fileName = Path.GetFileName(img.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads\\payment-images", fileName);
                    using (var Fstream = new FileStream(filePath, FileMode.Create))
                    {
                        img.CopyTo(Fstream);
                        var fullPath = "/uploads/payment-images/" + fileName;
                        filesList += fullPath;
                    }
                }
            }
            return filesList;
        }

        public IEnumerable<GRPayment> GetAll(int companyId)
        {
            IEnumerable<GRPayment> listRepo = _dbContext.GRPayments.Include(p => p.PaymentMode).Include(p => p.Supplier).Include(p => p.BankCashAccount).Where(p => p.CompanyId == companyId && p.IsDeleted == false)
            .ToList();
            return listRepo;
        }

        public GRPaymentInvoice[] GetPaymentInvoices(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            GRPaymentInvoice[] invoices = _dbContext.GRPaymentInvoices.Where(i => i.PaymentId == id && i.IsDeleted == false && i.CompanyId==companyId).ToArray();
            return invoices;
        }

        public GRPaymentViewModel GetById(int id)
        {
            GRPayment payment = _dbContext.GRPayments.Find(id);
            var viewModel = new GRPaymentViewModel();
            viewModel.PaymentNo = payment.PaymentNo;
            viewModel.PaymentDate = payment.PaymentDate;
            viewModel.PaymentModeId = payment.PaymentModeId;
            viewModel.OperationId = payment.OperationId;
            viewModel.DepartmentId = payment.DepartmentId;
            viewModel.DocumentNo = payment.DocumentNo;
            viewModel.DocumentDate = payment.DocumentDate;
            viewModel.Attachment = payment.Attachment;
            viewModel.TotalTaxAmount = payment.TotalTaxAmount;
            viewModel.TotalPaidAmount = payment.TotalPaidAmount;
            viewModel.InvoiceAdjusted = payment.InvoiceAdjusted;
            viewModel.GrandTotal = payment.GrandTotal;
            viewModel.SupplierId = payment.SupplierId;
            viewModel.VoucherId = payment.VoucherId;
            viewModel.BankCashAccountId = payment.BankCashAccountId;
            viewModel.Currency = payment.Currency;
            viewModel.CurrencyExchangeRate = payment.CurrencyExchangeRate;
            viewModel.Remarks = payment.Remarks;
            viewModel.Status = payment.Status;
            return viewModel;
        }

        [HttpPost]
        public async Task<bool> Create(GRPaymentViewModel model, IFormCollection collection, IFormFile Attachment)
        {
            try
            {
                GRPayment payment = new GRPayment();
                payment.PaymentNo = model.PaymentNo;
                payment.PaymentDate = model.PaymentDate;
                payment.PaymentModeId= model.PaymentModeId;
                payment.DocumentNo = model.DocumentNo;
                payment.DocumentDate = model.DocumentDate;
                payment.Attachment = UploadFile(Attachment);
                payment.TotalTaxAmount = model.TotalTaxAmount;
                payment.TotalPaidAmount = model.TotalPaidAmount;
                payment.InvoiceAdjusted = model.InvoiceAdjusted;
                payment.GrandTotal = model.GrandTotal;
                payment.SupplierId = model.SupplierId;
                payment.DepartmentId = model.DepartmentId;
                payment.OperationId = model.OperationId;
                payment.BankCashAccountId = model.BankCashAccountId;
                payment.Currency = model.Currency;
                payment.CurrencyExchangeRate = model.CurrencyExchangeRate;
                payment.Remarks = model.Remarks;
                payment.Status  = "Created";
                payment.CompanyId = model.CompanyId;
                payment.CreatedBy = model.CreatedBy;
                payment.CreatedDate = DateTime.Now;
                payment.IsDeleted = false;
                _dbContext.GRPayments.Add(payment);
                await _dbContext.SaveChangesAsync();

                //partialView's data saving in dbContext
                for (int i = 0; i < collection["InvoiceId"].Count; i++)
                {
                    var paymentInvoice = new GRPaymentInvoice();
                    paymentInvoice.PaymentId = payment.Id;
                    paymentInvoice.CompanyId = payment.CompanyId;
                    
                    paymentInvoice.InvoiceId = Convert.ToInt32(collection["InvoiceId"][i]);
                    paymentInvoice.InvoiceNo = Convert.ToInt32(collection["InvoiceNo"][i]);
                    paymentInvoice.InvoiceDate = Convert.ToDateTime(collection["InvoiceDate"][i]);
                    paymentInvoice.InvoiceAmount = Convert.ToDecimal(collection["InvoiceAmount"][i]);
                    paymentInvoice.Balance = Convert.ToDecimal(collection["Balance"][i]);
                    paymentInvoice.PaymentAmount = Convert.ToDecimal(collection["PaymentAmount"][i]);
                    paymentInvoice.TaxId = Convert.ToInt32(collection["TaxId"][i]);
                    paymentInvoice.TaxAmount = Convert.ToDecimal(collection["TaxAmount"][i]);
                    paymentInvoice.LineTotal = Convert.ToDecimal(collection["LineTotal"][i]);
                    paymentInvoice.IsDeleted = false;
                    _dbContext.GRPaymentInvoices.Add(paymentInvoice);
                    await _dbContext.SaveChangesAsync();
                }
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                string message = ex.Message.ToString();
                return false;
            }
        }

        [HttpPost]
        public async Task<bool> Update(GRPaymentViewModel model, IFormCollection collection, IFormFile Attachment)
        {
            //for partial-items removal
            string[] idsDeleted = Convert.ToString(collection["IdsDeleted"]).Split(",");
            if (!idsDeleted.Contains(""))
            {
                for (int j = 0; j < idsDeleted.Length; j++)
                {
                    if (idsDeleted[j] != "0")
                    {
                        var itemToRemove = _dbContext.GRPaymentInvoices.Find(Convert.ToInt32(idsDeleted[j]));
                        itemToRemove.IsDeleted = true;
                        var tracker = _dbContext.GRPaymentInvoices.Update(itemToRemove);
                        tracker.OriginalValues.SetValues(await tracker.GetDatabaseValuesAsync());
                        await _dbContext.SaveChangesAsync();
                    }
                }
            }
            //updating existing data
            var obj = _dbContext.GRPayments.Find(model.Id);
            obj.PaymentNo = model.PaymentNo;
            obj.PaymentDate = model.PaymentDate;
            obj.PaymentModeId = model.PaymentModeId;
            obj.DocumentNo = model.DocumentNo;
            obj.DocumentDate = model.DocumentDate;
            if (Attachment != null)
            {
                obj.Attachment = UploadFile(Attachment);
            }
            else
            {
                _dbContext.Entry(obj).State = EntityState.Modified;
                _dbContext.Entry(obj).Property(x => x.Attachment).IsModified = false;
            }
            obj.TotalTaxAmount = model.TotalTaxAmount;
            obj.TotalPaidAmount = model.TotalPaidAmount;
            obj.InvoiceAdjusted = model.InvoiceAdjusted;
            obj.GrandTotal = model.GrandTotal;
            obj.SupplierId = model.SupplierId;
            obj.DepartmentId = model.DepartmentId;
            obj.OperationId = model.OperationId;
            obj.BankCashAccountId = model.BankCashAccountId;
            obj.Currency = model.Currency;
            obj.CurrencyExchangeRate = model.CurrencyExchangeRate;
            obj.Remarks = model.Remarks;
            obj.CompanyId = model.CompanyId;
            obj.UpdatedBy = model.UpdatedBy;
            obj.UpdatedDate = DateTime.Now;
            var entry =_dbContext.GRPayments.Update(obj);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            //add or remove new invoice while updating existing data
            var list = _dbContext.GRPaymentInvoices.Where(l => l.PaymentId == Convert.ToInt32(collection["Id"])).ToList();
            if (list != null)
            {
                for (int i = 0; i < collection["InvoiceId"].Count; i++)
                {
                    var orderItem = _dbContext.GRPaymentInvoices
                        .Where(j => j.PaymentId == model.Id && j.Id == Convert.ToInt32(collection["PaymentItemId"][i] == "" ? 0 : Convert.ToInt32(collection["PaymentItemId"][i]))).FirstOrDefault();
                    // Extract coresponding values from form collection
                    var invoiceId = Convert.ToInt32(collection["InvoiceId"][i]);
                    var invoiceNo = Convert.ToInt32(collection["InvoiceNo"][i]);
                    var invoiceDate = Convert.ToDateTime(collection["InvoiceDate"][i]);
                    var invoiceAmount = Convert.ToDecimal(collection["InvoiceAmount"][i]);
                    var balance = Convert.ToDecimal(collection["Balance"][i]);
                    var paymentAmount = Convert.ToDecimal(collection["PaymentAmount"][i]);
                    var lineTotal = Convert.ToDecimal(collection["LineTotal"][i]);
                    var taxId = Convert.ToInt32(collection["TaxId"][i]);
                    var taxAmount = Convert.ToDecimal(collection["TaxAmount"][i]);
                    if (orderItem != null && invoiceId != 0)
                    {
                        var entityEntry = _dbContext.Entry(orderItem);
                        entityEntry.State = EntityState.Modified;
                        entityEntry.Property(p => p.Id).IsModified = false;
                        orderItem.InvoiceId = invoiceId;
                        orderItem.InvoiceNo = invoiceNo;
                        orderItem.PaymentId = model.Id;
                        orderItem.CompanyId = model.CompanyId;
                        orderItem.InvoiceAmount = invoiceAmount;
                        orderItem.Balance = balance;
                        orderItem.PaymentAmount = paymentAmount;
                        orderItem.LineTotal = lineTotal;
                        orderItem.TaxId = taxId;
                        orderItem.TaxAmount = taxAmount;
                        var dbEntry = _dbContext.GRPaymentInvoices.Update(orderItem);
                        dbEntry.OriginalValues.SetValues(await entityEntry.GetDatabaseValuesAsync());
                    }
                    //else if (orderItem == null && invoiceId != 0)
                    //{
                    //    GRPaymentInvoice newItem = new GRPaymentInvoice();
                    //    newItem.InvoiceId = invoiceId;
                    //    newItem.InvoiceNo = invoiceNo;
                    //    newItem.PaymentId = model.Id;
                    //    newItem.InvoiceAmount = invoiceAmount;
                    //    newItem.Balance = balance;
                    //    newItem.PaymentAmount = paymentAmount;
                    //    newItem.LineTotal = lineTotal;
                    //    newItem.TaxId = taxId;
                    //    newItem.TaxAmount = taxAmount;
                    //    _dbContext.GRPaymentInvoices.Add(newItem);
                    //}
                    await _dbContext.SaveChangesAsync();
                }
            }
            return true;
        }

        public GRPaymentViewModel GetPaymentInvoices(int id, int invoiceId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var item = _dbContext.GRPaymentInvoices.Include(i => i.Payment).Where(i => i.Id == id && i.IsDeleted != true && i.CompanyId==companyId).FirstOrDefault();
            GRPaymentViewModel viewModel = new GRPaymentViewModel();
            //viewModel.PaymentItemId = item.Id;
            viewModel.InvoiceId = item.InvoiceId;
            viewModel.InvoiceNo = item.InvoiceNo;
            viewModel.InvoiceDate = item.InvoiceDate;
            viewModel.InvoiceAmount = item.InvoiceAmount;
            viewModel.Balance = item.Balance;
            viewModel.PaymentAmount = item.PaymentAmount;
            viewModel.TaxId = item.TaxId;
            viewModel.TaxAmount = item.TaxAmount;
            viewModel.LineTotal = item.LineTotal;
            return viewModel;
        }

        //public async Task<bool> Approve(int id, string userId, int companyId)
        //{
        //    APPayment receipt = _dbContext.APPayments
        //     .Include(p => p.Supplier)
        //     .Include(p => p.BankCashAccount)
        //     .Where(p => p.Status == "Created" && p.CompanyId == companyId && p.Id == id && p.IsDeleted == false)
        //     .FirstOrDefault();
        //    try
        //    {
        //        var payment = _dbContext.APPayments.Find(id);
        //        payment.Status = "Approved";
        //        payment.ApprovedBy = userId;
        //        payment.ApprovedDate = DateTime.Now;
        //        var entry = _dbContext.APPayments.Update(payment);
        //        entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
        //        await _dbContext.SaveChangesAsync();
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.InnerException.Message);
        //        string message = ex.Message.ToString();
        //        return false;
        //    }
        //}

        public async Task<bool> Delete(int id)
        {
            GRPayment delete = _dbContext.GRPayments.Find(id);
            delete.IsDeleted = true;
            var entry = _dbContext.GRPayments.Update(delete);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public int GetPaymentNo(int companyId)
        {
            int maxPaymentNo = 1;
            var payments = _dbContext.GRPayments.Where(p => p.CompanyId == companyId).ToList();
            if (payments.Count > 0)
            {
                maxPaymentNo = payments.Max(p => p.PaymentNo);
                return maxPaymentNo + 1;
            }
            else
            {
                return maxPaymentNo;
            }
        }
        public GRPaymentViewModel GetPurchaseInvoice(int id, int companyId)
        {
            //int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            //var item = _dbContext.GRGRNInvoices.Where(i => i.Id == id  && i.IsDeleted == false).FirstOrDefault();
            var item = (from a in _dbContext.GRGRNInvoices.Where(x => x.Id==id && x.IsDeleted!=true && x.CompanyId==companyId).ToList()
                           join c in _dbContext.GRGRNInvoiceDetails on a.Id equals c.GRNId
                           join b in _dbContext.GRGRNS.Where(x =>x.CompanyId==companyId) on a.GRNId equals b.Id
                           select new
                           {
                               a.Id,
                               a.PurchaseNo,
                               PurchaseDate = a.ApprovedDate,
                               Total = c.NetPayableAmount,
                               TotalSalesTaxAmount = c.NetPayableAmount * 17/100,
                               TotalExciseTaxAmount = 0,
                               GrandTotal = c.NetPayableAmount,
                               PaymentTotal = _dbContext.GRPaymentInvoices.Where(x => x.InvoiceId == a.Id && a.CompanyId==companyId).Select(x => x.PaymentAmount).Sum()


                           }).FirstOrDefault();

            GRPaymentViewModel viewModel = new GRPaymentViewModel();
            if (item != null)
            {
                viewModel.InvoiceNo = item.PurchaseNo;
                viewModel.InvoiceId = item.Id;
                viewModel.InvoiceDate = Convert.ToDateTime(item.PurchaseDate);
                viewModel.InvoiceAmount = item.Total + item.TotalSalesTaxAmount + item.TotalExciseTaxAmount;
                viewModel.Balance = Convert.ToDecimal(item.Total + item.TotalSalesTaxAmount + item.TotalExciseTaxAmount) - Convert.ToDecimal(item.PaymentTotal);
            }
            return viewModel;
        }
        [HttpGet]
        public dynamic GetUnpaidInvoicesBySupplierId(int id, int[] skipIds,int companyId)
        {
            //int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var invoice = (from a in _dbContext.GRGRNInvoices.Where(i => !skipIds.Contains(i.Id) && i.CompanyId==companyId).ToList()
                           join c in _dbContext.GRGRNInvoiceDetails on a.GRNId equals c.GRNId
                           join b in _dbContext.GRGRNS.Where(i => i.VendorId == id && i.CompanyId == companyId) on a.GRNId equals b.Id
                           select new
                           {
                               a.Id,
                               a.PurchaseNo,
                               PurchaseDate = a.CreatedDate,
                               Total=c.Amount,
                               TotalSalesTaxAmount=c.TotalPenaltyAmount,
                               TotalExciseTaxAmount = 0,
                               GrandTotal = c.Amount,
                               PaymentTotal=c.Amount,


                           }) ;
                //_dbContext.GRGRNInvoice.Where(i => i.SupplierId == id && i.CompanyId == companyId && i.IsDeleted == false && i.Status == "Approved")
                //                            .Where(i => !skipIds.Contains(i.Id)).ToList();
            return invoice;
        }
        public IEnumerable<GRPayment> GetApprovedPayments(int companyId)
        {
            var list = _dbContext.GRPayments.Where(p => p.Status == "Approved" && p.CompanyId == companyId && p.IsDeleted == false).AsEnumerable();
            return list;
        }

        public async Task<bool> UnApproveVoucher(int id,int companyId)
        {
           
            var payment = _dbContext.GRPayments
                            .Where(v => v.IsDeleted == false && v.Id == id && v.Status == "Approved" && v.CompanyId == companyId).FirstOrDefault();
            if (payment == null)
            {
                return false;
            }
            else
            {
                using (var transaction = _dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        var voucherDetail = _dbContext.GLVoucherDetails.Where(v => v.VoucherId == payment.VoucherId).ToList();
                        var items = _dbContext.GRPaymentInvoices.Where(r => r.IsDeleted == false && r.PaymentId == id).ToList();
                        foreach (var item in items)
                        {
                            var invoice = _dbContext.APPurchases.Find(item.InvoiceId);
                            invoice.PaymentTotal = invoice.PaymentTotal - item.LineTotal;
                            var dbEntry = _dbContext.APPurchases.Update(invoice);
                            dbEntry.OriginalValues.SetValues(await dbEntry.GetDatabaseValuesAsync());
                            await _dbContext.SaveChangesAsync();
                        }
                        foreach (var item in voucherDetail)
                        {
                            var tracker = _dbContext.GLVoucherDetails.Remove(item);
                            tracker.OriginalValues.SetValues(await tracker.GetDatabaseValuesAsync());
                            await _dbContext.SaveChangesAsync();
                        }
                        payment.Status = "Created";
                        payment.ApprovedBy = null;
                        payment.ApprovedDate = DateTime.Now;
                        var entry = _dbContext.GRPayments.Update(payment);
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

        public async Task<bool> Approve(int id, string userId, int companyId)
        {
            GRPayment payment = _dbContext.GRPayments
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
                "GR Payment # : {0} of  " +
                "{1} {2}",
                payment.PaymentNo,
                payment.Supplier.Name, payment.Remarks);

                int voucherId;
                voucherMaster.VoucherType = "GR-PYMT";
                voucherMaster.VoucherDate = payment.PaymentDate;
                //voucherMaster.Reference = receipt.reference;
                voucherMaster.Currency = payment.Currency;
                voucherMaster.CurrencyExchangeRate = payment.CurrencyExchangeRate;
                voucherMaster.Description = payment.Remarks;
                voucherMaster.Status = "Approved";
                voucherMaster.ApprovedBy = userId;
                voucherMaster.ApprovedDate = DateTime.Now;
                voucherMaster.IsSystem = true;
                voucherMaster.ModuleName = "GR/Payment";
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
                var incomTaxAccounts = (from li in _dbContext.GRPaymentInvoices
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

    }
}
