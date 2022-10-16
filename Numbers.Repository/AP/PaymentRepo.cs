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
    public class PaymentRepo
    {
        private readonly NumbersDbContext _dbContext;
        public PaymentRepo(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
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

        public IEnumerable<APPayment> GetAll(int companyId)
        {
            IEnumerable<APPayment> listRepo = _dbContext.APPayments.Include(p => p.Supplier).Include(p => p.BankCashAccount).Where(p => p.CompanyId == companyId && p.IsDeleted == false)
            .ToList();
            return listRepo;
        }

        public APPaymentInvoice[] GetPaymentInvoices(int id)
        {
            APPaymentInvoice[] invoices = _dbContext.APPaymentInvoices.Where(i => i.PaymentId == id && i.IsDeleted == false).ToArray();
            return invoices;
        }

        public APPaymentViewModel GetById(int id)
        {
            APPayment payment = _dbContext.APPayments.Find(id);
            var viewModel = new APPaymentViewModel();
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
            viewModel.Id = payment.Id;
            return viewModel;
        }

        [HttpPost]
        public async Task<bool> Create(APPaymentViewModel model, IFormCollection collection, IFormFile Attachment)
        {
            try
            {
                APPayment payment = new APPayment();
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
                _dbContext.APPayments.Add(payment);
                await _dbContext.SaveChangesAsync();

                //partialView's data saving in dbContext
                for (int i = 0; i < collection["InvoiceId"].Count; i++)
                {
                    var paymentInvoice = new APPaymentInvoice();
                    paymentInvoice.PaymentId = payment.Id;
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
                    _dbContext.APPaymentInvoices.Add(paymentInvoice);
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
        public async Task<bool> Update(APPaymentViewModel model, IFormCollection collection, IFormFile Attachment)
        {
            //for partial-items removal
            string[] idsDeleted = Convert.ToString(collection["IdsDeleted"]).Split(",");
            if (!idsDeleted.Contains(""))
            {
                for (int j = 0; j < idsDeleted.Length; j++)
                {
                    if (idsDeleted[j] != "0")
                    {
                        var itemToRemove = _dbContext.APPaymentInvoices.Find(Convert.ToInt32(idsDeleted[j]));
                        itemToRemove.IsDeleted = true;
                        var tracker = _dbContext.APPaymentInvoices.Update(itemToRemove);
                        tracker.OriginalValues.SetValues(await tracker.GetDatabaseValuesAsync());
                        await _dbContext.SaveChangesAsync();
                    }
                }
            }
            //updating existing data
            var obj = _dbContext.APPayments.Find(model.Id);
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
            var entry =_dbContext.APPayments.Update(obj);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            //add or remove new invoice while updating existing data
            var list = _dbContext.APPaymentInvoices.Where(l => l.PaymentId == Convert.ToInt32(collection["Id"])).ToList();
            if (list != null)
            {
                for (int i = 0; i < collection["InvoiceId"].Count; i++)
                {
                    var orderItem = _dbContext.APPaymentInvoices
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
                        orderItem.InvoiceAmount = invoiceAmount;
                        orderItem.Balance = balance;
                        orderItem.PaymentAmount = paymentAmount;
                        orderItem.LineTotal = lineTotal;
                        orderItem.TaxId = taxId;
                        orderItem.TaxAmount = taxAmount;
                        var dbEntry = _dbContext.APPaymentInvoices.Update(orderItem);
                        dbEntry.OriginalValues.SetValues(await entityEntry.GetDatabaseValuesAsync());
                    }
                    else if (orderItem == null && invoiceId != 0)
                    {
                        APPaymentInvoice newItem = new APPaymentInvoice();
                        newItem.InvoiceId = invoiceId;
                        newItem.InvoiceNo = invoiceNo;
                        newItem.PaymentId = model.Id;
                        newItem.InvoiceAmount = invoiceAmount;
                        newItem.Balance = balance;
                        newItem.PaymentAmount = paymentAmount;
                        newItem.LineTotal = lineTotal;
                        newItem.TaxId = taxId;
                        newItem.TaxAmount = taxAmount;
                        _dbContext.APPaymentInvoices.Add(newItem);
                    }
                    await _dbContext.SaveChangesAsync();
                }
            }
            return true;
        }

        public APPaymentViewModel GetPaymentInvoices(int id, int invoiceId)
        {
            var item = _dbContext.APPaymentInvoices.Include(i => i.Payment).Where(i => i.Id == id && i.IsDeleted != true).FirstOrDefault();
            APPaymentViewModel viewModel = new APPaymentViewModel();
            viewModel.PaymentItemId = item.Id;
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
            APPayment delete = _dbContext.APPayments.Find(id);
            delete.IsDeleted = true;
            var entry = _dbContext.APPayments.Update(delete);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public int GetPaymentNo(int companyId)
        {
            int maxPaymentNo = 1;
            var payments = _dbContext.APPayments.Where(p => p.CompanyId == companyId).ToList();
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
        public APPaymentViewModel GetPurchaseInvoice(int id)
        {
            var item = _dbContext.APPurchases.Include(i => i.Supplier).Where(i => i.Id == id  && i.IsDeleted == false).FirstOrDefault();
            APPaymentViewModel viewModel = new APPaymentViewModel();
            if (item != null)
            {
                viewModel.InvoiceNo = item.PurchaseNo;
                viewModel.InvoiceId = item.Id;
                viewModel.InvoiceDate = item.PurchaseDate;
                viewModel.InvoiceAmount = item.GrandTotal;
                viewModel.Balance = item.GrandTotal-item.PaymentTotal;
            }
            return viewModel;
        }
        [HttpGet]
        public dynamic GetUnpaidInvoicesBySupplierId(int id, int[] skipIds,int companyId)
        {
            var invoice = _dbContext.APPurchases.Where(i => i.SupplierId == id && i.CompanyId == companyId && i.IsDeleted == false && i.Status == "Approved")
                                            .Where(i => !skipIds.Contains(i.Id)).ToList();
            return invoice;
        }
        public IEnumerable<APPayment> GetApprovedPayments(int companyId)
        {
            var list = _dbContext.APPayments.Where(p => p.Status == "Approved" && p.CompanyId == companyId && p.IsDeleted == false).AsEnumerable();
            return list;
        }

        public async Task<bool> UnApproveVoucher(int id,int companyId)
        {
           
            var payment = _dbContext.APPayments
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
                        var items = _dbContext.APPaymentInvoices.Where(r => r.IsDeleted == false && r.PaymentId == id).ToList();
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
                        var entry = _dbContext.APPayments.Update(payment);
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
