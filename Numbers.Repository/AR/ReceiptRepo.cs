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

namespace Numbers.Repository.AR
{
    public class ReceiptRepo
    {
        private HttpContext HttpContext { get; }
        private readonly NumbersDbContext _dbContext;
        public ReceiptRepo(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public ReceiptRepo(NumbersDbContext dbContext,HttpContext httpContext)
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
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads\\receipt-images", fileName);
                    using (var Fstream = new FileStream(filePath, FileMode.Create))
                    {
                        img.CopyTo(Fstream);
                        var fullPath = "/uploads/receipt-images/" + fileName;
                        filesList += fullPath;
                    }
                }
            }
            return filesList;
        }

        public async Task<IEnumerable<ARReceipt>> GetAll(int companyId)
        {
            IEnumerable<ARReceipt> listRepo = await _dbContext.ARReceipts.Include(p => p.PaymentMode).Include(r => r.Customer)
                .Where(r => r.CompanyId == companyId && r.IsDeleted == false ).ToListAsync();
            return listRepo;
        }

        public ARReceiptInvoice[] GetReceiptInvoices(int id)
        {
            ARReceiptInvoice[] invoices = _dbContext.ARReceiptInvoices.Where(i => i.ReceiptId == id && i.IsDeleted == false)
                .ToArray();
            return invoices;
        }

        public ARReceiptViewModel GetById(int id)
        {
            ARReceipt receipt = _dbContext.ARReceipts.Find(id);
            ARReceiptViewModel viewModel = new ARReceiptViewModel();
            viewModel.Id = receipt.Id;
            viewModel.ReceiptNo = receipt.ReceiptNo;
            viewModel.ReceiptDate = receipt.ReceiptDate;
            viewModel.BankCashAccountId = receipt.BankCashAccountId;
            viewModel.FourthLevelAccountId = receipt.FourthLevelAccountId;
            viewModel.PaymentModeId = receipt.PaymentModeId;
            viewModel.CustomerId = receipt.CustomerId;
            viewModel.ItemCategoryId = receipt.ItemCategoryId;
            viewModel.CityId = receipt.CityId;
            viewModel.DocumentNo = receipt.DocumentNo;
            viewModel.DocumentDate = receipt.DocumentDate;
            viewModel.TotalReceivedAmount = receipt.TotalReceivedAmount;
            viewModel.InvoiceAdjusted = receipt.InvoiceAdjusted;
            viewModel.TotalTaxAmount = receipt.TotalTaxAmount;
            viewModel.GrandTotal = receipt.GrandTotal;
            viewModel.Attachment = receipt.Attachment;
            viewModel.Remarks = receipt.Remarks;
            viewModel.Status = receipt.Status;
            viewModel.VoucherId = receipt.VoucherId;
            viewModel.SalePerson = receipt.SalePerson;
            viewModel.CityId = receipt.CityId;
            viewModel.ItemCategoryId = receipt.ItemCategoryId;
          //  viewModel.ReceiptAmount = receipt.ReceiptAmount;
            return viewModel;
        }

        [HttpPost]
        public async Task<bool> Create(ARReceiptViewModel model,IFormCollection collection, IFormFile Attachment, int companyId)
        {
            try
            {

                //ARReceipt receipt = new ARReceipt();
                for (int i = 0; i < collection["id"].Count - 1; i++)
                {
                    ARReceipt receipt = new ARReceipt();
                    int CustomerId = Convert.ToInt32( collection["CustomerIds"][i]);
                    var salePerson = Convert.ToString( collection["SalePerson"][i]);
                    var ItemCategoryId = Convert.ToInt32(collection["CategoryId"][i]);
                    var CityId =Convert.ToInt32( collection["CityId"][i]);
                    var RecieptAmount =Convert.ToDecimal( collection["ReceiptAmount"][i]);
                    var remarks = collection["Remarks"][i];
                    //max receiptno
                    receipt.ReceiptNo = GetReceiptNo(companyId);
                    receipt.ReceiptDate = model.ReceiptDate;
                    receipt.BankCashAccountId = model.BankCashAccountId;
                    receipt.FourthLevelAccountId = model.FourthLevelAccountId;
                    receipt.PaymentModeId = model.PaymentModeId;
                    receipt.CustomerId = CustomerId;
                    receipt.SalePerson = salePerson;
                    receipt.ItemCategoryId = ItemCategoryId;
                    receipt.CityId = CityId;
                    receipt.DocumentNo = model.DocumentNo;
                    receipt.DocumentDate = model.DocumentDate;
                    receipt.Currency = model.Currency;
                    receipt.CurrencyExchangeRate = model.CurrencyExchangeRate;
                    receipt.TotalReceivedAmount = RecieptAmount;
                    receipt.InvoiceAdjusted = model.InvoiceAdjusted;
                    receipt.TotalTaxAmount = model.TotalTaxAmount;
                    receipt.GrandTotal = RecieptAmount;
                    receipt.Attachment = UploadFile(Attachment);
                    receipt.Remarks = remarks;
                    receipt.Status = "Created";
                    receipt.IsDeleted = false;
                    receipt.CreatedBy = model.CreatedBy;
                    receipt.CreatedDate = DateTime.Now;
                    receipt.CompanyId = model.CompanyId;
                    receipt.ResponsibilityId = model.ResponsibilityId;
                    _dbContext.ARReceipts.Add(receipt);
                    await _dbContext.SaveChangesAsync();

                    if (receipt.CustomerId != 0 && receipt.TotalReceivedAmount != 0)
                    {
                        var totalAmount = receipt.TotalReceivedAmount;
                        var customerinvoices = _dbContext.ARInvoices
                                                .Where(i => i.CustomerId == receipt.CustomerId && i.ReceiptTotal != i.GrandTotal && i.CompanyId == model.CompanyId && i.IsDeleted == false && i.Status == "Approved" && (i.TransactionType == "Service" || i.TransactionType == "Sale")).OrderBy(x => x.InvoiceNo).ToList();
                        foreach (var item in customerinvoices)
                        {
                            if (totalAmount > 0)
                            {
                                var receiptItem = new ARReceiptInvoice();
                                receiptItem.ReceiptId = receipt.Id;
                                receiptItem.InvoiceId = item.Id;
                                receiptItem.InvoiceNo = item.InvoiceNo;
                                receiptItem.InvoiceDate = item.InvoiceDate;
                                receiptItem.InvoiceAmount = item.GrandTotal;
                                receiptItem.Balance = item.GrandTotal - item.ReceiptTotal;
                                if (item.GrandTotal >= totalAmount)
                                {
                                    receiptItem.ReceiptAmount = totalAmount - item.ReceiptTotal;
                                    receiptItem.LineTotal = totalAmount;
                                    totalAmount = 0;
                                }
                                else
                                {
                                    receiptItem.ReceiptAmount = item.GrandTotal - item.ReceiptTotal;
                                    receiptItem.LineTotal = item.GrandTotal;
                                    totalAmount = totalAmount - item.GrandTotal + item.ReceiptTotal;
                                }
                                receiptItem.IsDeleted = false;
                                _dbContext.ARReceiptInvoices.Add(receiptItem);
                                await _dbContext.SaveChangesAsync();

                            }
                        }


                    }

                }

                
                //receipt.ReceiptNo = model.ReceiptNo;
                

                //get invoices by CustomerId
              
                //partialView's data saving in dbContext
                //for (int i = 0; i < collection["InvoiceId"].Count; i++)
                //{
                //    var receiptItem = new ARReceiptInvoice();
                //    receiptItem.ReceiptId = receipt.Id;
                //    receiptItem.InvoiceId= Convert.ToInt32(collection["InvoiceId"][i]);
                //    receiptItem.InvoiceNo = Convert.ToInt32(collection["InvoiceNo"][i]);
                //    receiptItem.InvoiceDate = Convert.ToDateTime(collection["InvoiceDate"][i]);
                //    receiptItem.InvoiceAmount = Convert.ToDecimal(collection["InvoiceAmount"][i]);
                //    receiptItem.Balance = Convert.ToDecimal(collection["Balance"][i]);
                //    receiptItem.ReceiptAmount = Convert.ToDecimal(collection["ReceiptAmount"][i]);
                //   /* receiptItem.TaxId = Convert.ToInt32(collection["TaxId"][i]);
                //    receiptItem.TaxAmount = Convert.ToDecimal(collection["TaxAmount"][i]);*/
                //    receiptItem.LineTotal = Convert.ToDecimal(collection["LineTotal"][i]);
                //    receiptItem.IsDeleted = false;
                //    _dbContext.ARReceiptInvoices.Add(receiptItem);
                //    await _dbContext.SaveChangesAsync();
                //}
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                string message = ex.Message.ToString();
                return false;
            }
        }
        [HttpPost]
        public async Task<bool> Update(ARReceiptViewModel model, IFormCollection collection, IFormFile Attachment)
        {
            //for partial-items removal
            string[] idsDeleted = Convert.ToString(collection["IdsDeleted"]).Split(",");
            
            //updating existing data
            ARReceipt receipt = _dbContext.ARReceipts.Find(model.Id);
            receipt.ReceiptNo = model.ReceiptNo;
            receipt.ReceiptDate = model.ReceiptDate;
            receipt.BankCashAccountId = model.BankCashAccountId;
            receipt.FourthLevelAccountId = model.FourthLevelAccountId;
            receipt.PaymentModeId = model.PaymentModeId;
            receipt.CustomerId = model.CustomerId;
            receipt.DocumentNo = model.DocumentNo;
            receipt.DocumentDate = model.DocumentDate;
            receipt.Currency = model.Currency;
            receipt.CurrencyExchangeRate = model.CurrencyExchangeRate;
            receipt.TotalReceivedAmount = model.TotalReceivedAmount;
            receipt.InvoiceAdjusted = model.InvoiceAdjusted;
            receipt.TotalTaxAmount = model.TotalTaxAmount;
            receipt.GrandTotal = model.TotalReceivedAmount;
            receipt.Remarks = model.Remarks;
            if (Attachment != null)
            {
                receipt.Attachment = UploadFile(Attachment);
            }
            else
            {
                _dbContext.Entry(receipt).State = EntityState.Modified;
                _dbContext.Entry(receipt).Property(x => x.Attachment).IsModified = false;
            }
            receipt.CompanyId = model.CompanyId;
            receipt.ResponsibilityId = model.ResponsibilityId;
            receipt.UpdatedBy = model.UpdatedBy;
            receipt.UpdatedDate = DateTime.Now;
            var entry = _dbContext.ARReceipts.Update(receipt);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            //add or remove new invoice while updating existing data
            var removereceipt = _dbContext.ARReceiptInvoices.Where(x => x.ReceiptId == receipt.Id).ToList();
            foreach (var item in removereceipt)
            {
                var itemremove = _dbContext.ARReceiptInvoices.Find(item.Id);
                _dbContext.Remove(itemremove);
               await _dbContext.SaveChangesAsync();
                
            }

            if (receipt.CustomerId != 0 && receipt.TotalReceivedAmount != 0)
            {
                var totalAmount = receipt.TotalReceivedAmount;
                var customerinvoices = _dbContext.ARInvoices
                                        .Where(i => i.CustomerId == receipt.CustomerId && i.ReceiptTotal != i.GrandTotal && i.CompanyId == model.CompanyId && i.IsDeleted == false && i.Status == "Approved" && (i.TransactionType == "Service" || i.TransactionType == "Sale")).OrderBy(x => x.InvoiceNo).ToList();
                foreach (var item in customerinvoices)
                {
                    if (totalAmount > 0)
                    {
                        var receiptItem = new ARReceiptInvoice();
                        receiptItem.ReceiptId = receipt.Id;
                        receiptItem.InvoiceId = item.Id;
                        receiptItem.InvoiceNo = item.InvoiceNo;
                        receiptItem.InvoiceDate = item.InvoiceDate;
                        receiptItem.InvoiceAmount = item.GrandTotal;
                        receiptItem.Balance = item.GrandTotal - item.ReceiptTotal;
                        if (item.GrandTotal >= totalAmount)
                        {
                            receiptItem.ReceiptAmount = totalAmount - item.ReceiptTotal;
                            receiptItem.LineTotal = totalAmount;
                            totalAmount = 0;
                        }
                        else
                        {
                            receiptItem.ReceiptAmount = item.GrandTotal - item.ReceiptTotal;
                            receiptItem.LineTotal = item.GrandTotal;
                            totalAmount = totalAmount - item.GrandTotal + item.ReceiptTotal;
                        }
                        receiptItem.IsDeleted = false;
                        _dbContext.ARReceiptInvoices.Add(receiptItem);
                        await _dbContext.SaveChangesAsync();


                    }
                }


            }
            return true;
        }
        public async Task<bool> Delete(int id)
        {
            var deleteReceipt = _dbContext.ARReceipts.Find(id);
            deleteReceipt.IsDeleted = true;
            var entry = _dbContext.ARReceipts.Update(deleteReceipt);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return true;
        }
        public ARReceiptViewModel GetReceiptInvoices(int id, int invoiceId)
        {
            var item = _dbContext.ARReceiptInvoices.Include(i => i.Receipt).Where(i => i.Id == id && i.IsDeleted != true).FirstOrDefault();
            ARReceiptViewModel viewModel = new ARReceiptViewModel();
            viewModel.ReceiptItemId = item.Id;
            viewModel.InvoiceId = item.InvoiceId;
            viewModel.InvoiceNo = item.InvoiceNo;
            viewModel.InvoiceDate = item.InvoiceDate;
            viewModel.InvoiceAmount = item.InvoiceAmount;
            viewModel.Balance = item.Balance;
            viewModel.ReceiptAmount = item.ReceiptAmount;
            viewModel.TaxId = item.TaxId;
            viewModel.TaxAmount = item.TaxAmount;
            viewModel.LineTotal = item.LineTotal;
            return viewModel;
        }
        public int GetReceiptNo(int companyId)
        {
            NumbersDbContext _dbContext = new NumbersDbContext();
            int maxReceiptNo = 1;
            var receipts = _dbContext.ARReceipts.Where(r => r.CompanyId == companyId).ToList();
            if (receipts.Count > 0)
            {
                maxReceiptNo = receipts.Max(r => r.ReceiptNo);
                return maxReceiptNo + 1;
            }
            else
            {
                return maxReceiptNo;
            }
        }
        public ARReceiptViewModel GetSaleInvoice(int id)
        {
            var item = _dbContext.ARInvoices.Include(i => i.Customer).Where(i => i.Id == id && i.IsDeleted == false ).FirstOrDefault();
            ARReceiptViewModel viewModel = new ARReceiptViewModel();
            viewModel.InvoiceNo = item.InvoiceNo;
            viewModel.InvoiceId = item.Id;
            viewModel.InvoiceDate = item.InvoiceDate;
            viewModel.InvoiceAmount = item.GrandTotal;
            viewModel.Balance = item.GrandTotal-item.ReceiptTotal;
            return viewModel;
        }
        [HttpGet]
        public dynamic GetUnpaidInvoicesByCustomerId(int id, int[] skipIds, int companyId)
        {
            var invoice = _dbContext.ARInvoices
                                            .Where(i => i.CustomerId == id && i.CompanyId == companyId && i.IsDeleted == false && i.Status == "Approved" && (i.TransactionType == "Service" || i.TransactionType == "Sale"))
                                            .Where(i => !skipIds.Contains(i.Id))
                                            .ToList();
            return invoice;
        }
        public async Task<bool> Approve(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            ARReceipt receipt = _dbContext.ARReceipts
             .Include(c => c.Customer)
             //.Include(c=>c.BankCashAccount)
             .Where(a => a.Status == "Created" && a.CompanyId == companyId && a.Id == id && a.IsDeleted == false)
             .FirstOrDefault();
            try
            {
                //Create Voucher
                VoucherHelper voucher = new VoucherHelper(_dbContext, HttpContext);
                GLVoucher voucherMaster = new GLVoucher();
                List<GLVoucherDetail> voucherDetails = new List<GLVoucherDetail>();
                string voucherDescription = string.Format(
                "Receipt # : {0} of  " +
                "{1} {2}",
                receipt.ReceiptNo,
                receipt.Customer.Name, receipt.Remarks);

                int voucherId;
                voucherMaster.VoucherType = "RCPT";
                voucherMaster.VoucherDate = receipt.ReceiptDate;
                //voucherMaster.Reference = receipt.reference;
                voucherMaster.Currency = receipt.Currency;
                voucherMaster.CurrencyExchangeRate = receipt.CurrencyExchangeRate;
                voucherMaster.Description = receipt.Remarks;
                voucherMaster.Status = "Approved";
                voucherMaster.ApprovedBy = userId;
                voucherMaster.ApprovedDate = DateTime.Now;
                voucherMaster.IsSystem = true;
                voucherMaster.ModuleName = "AR/Receipt";
                voucherMaster.ModuleId = id;
                voucherMaster.ReferenceId = receipt.CustomerId;
                //Voucher Details
                var amount = receipt.GrandTotal;
                //Credit Entry
                GLVoucherDetail voucherDetail;

                #region Cash Bank  (DEBIT)
                //Receipt Voucher Debit Entry
                voucherDetail = new GLVoucherDetail();
                voucherDetail.AccountId = receipt.BankCashAccountId != 0 ? receipt.BankCashAccountId : receipt.FourthLevelAccountId;
                voucherDetail.Sequence = 10;
                voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                voucherDetail.Debit = amount;
                voucherDetail.Credit = 0;
                voucherDetail.IsDeleted = false;
                voucherDetail.CreatedBy = userId;
                voucherDetail.CreatedDate = DateTime.Now;
                voucherDetails.Add(voucherDetail);

                #endregion Line Items

                #region W.h. Tax  (DEBIT)
                var incomTaxAccounts = (from li in _dbContext.ARReceiptInvoices
                                       join t in _dbContext.AppTaxes on li.TaxId equals t.Id
                                       where li.ReceiptId == id
                                       select new
                                       {
                                           li.TaxAmount,
                                           t.IncomeTaxAccountId
                                       }).GroupBy(l => l.IncomeTaxAccountId)
                                        .Select(li => new ARReceiptInvoice
                                        {
                                            TaxAmount = li.Sum(c => c.TaxAmount),
                                            TaxId = li.FirstOrDefault().IncomeTaxAccountId //Tax id is temporarily containing SalesTaxAccountId
                                        }).ToList();
                foreach (var income in incomTaxAccounts)
                {
                    if (income.TaxId > 0 && income.TaxAmount>0)
                    {
                        voucherDetail = new GLVoucherDetail();
                        voucherDetail.AccountId = income.TaxId;
                        voucherDetail.Sequence = 20;
                        voucherDetail.Description = string.Format("Tax Amount Against Receipt # {0}", receipt.ReceiptNo);
                        voucherDetail.Credit = 0;
                        voucherDetail.Debit = income.TaxAmount;
                        voucherDetail.IsDeleted = false;
                        voucherDetail.CreatedBy = userId;
                        voucherDetail.CreatedDate = DateTime.Now;
                        voucherDetails.Add(voucherDetail);
                    }
                }
                #endregion Sales Tax

                #region CUSTOMER (CREDIT)
                voucherDetail = new GLVoucherDetail();
                voucherDetail.AccountId = receipt.Customer.AccountId;
                voucherDetail.Sequence = 30;
                voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                voucherDetail.Debit = 0;
                voucherDetail.Credit = receipt.TotalReceivedAmount;
                voucherDetail.IsDeleted = false;
                voucherDetail.CreatedBy = userId;
                voucherDetail.CreatedDate = DateTime.Now;
                voucherDetails.Add(voucherDetail);
                #endregion 

                //Create Voucher 
                voucherId = voucher.CreateVoucher(voucherMaster, voucherDetails);
                if (voucherId != 0)
                {
                    var itemReceipt = _dbContext.ARReceiptInvoices.Where(r => r.IsDeleted == false && r.ReceiptId == id).ToList();
                    
                    receipt.VoucherId = voucherId;
                    receipt.Status = "Approved";
                    receipt.ApprovedBy = userId;
                    receipt.ApprovedDate = DateTime.Now;
                    receipt.ReceiptBalance=receipt.GrandTotal - itemReceipt.Sum(x => x.ReceiptAmount);
                    using (var transaction = _dbContext.Database.BeginTransaction())
                    {
                        //On approval updating Invoice
                        var entry = _dbContext.Update(receipt);
                        entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                        await _dbContext.SaveChangesAsync();
                        var items = _dbContext.ARReceiptInvoices.Where(r => r.IsDeleted == false && r.ReceiptId == id).ToList();
                        foreach (var item in items)
                        {
                            var invoice = _dbContext.ARInvoices.Find(item.InvoiceId);
                            //invoice.InvoiceAmount = receipt.GrandTotal - invoice.ReceiptTotal - invoice.AdjustmentTotal;
                            invoice.ReceiptTotal = invoice.ReceiptTotal + item.LineTotal;
                            var dbEntry = _dbContext.ARInvoices.Update(invoice);
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
                return false;
            }
        }
        public IEnumerable<ARReceipt> GetApprovedReceipts()
        {
            var list=_dbContext.ARReceipts.Where(r => r.Status == "Approved" && r.CompanyId == HttpContext.Session.GetInt32("CompanyId").Value && r.IsDeleted == false).AsEnumerable();
            return list;
        }
        public async Task<bool> UnApproveVoucher(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var receipt = _dbContext.ARReceipts
                            .Where(v => v.IsDeleted == false && v.Id == id && v.Status == "Approved" && v.CompanyId == companyId).FirstOrDefault();
            if (receipt == null)
            {
                return false;
            }
            else
            {
                using (var transaction = _dbContext.Database.BeginTransaction())
                {
                    try
                    {

                        var voucherDetail = _dbContext.GLVoucherDetails.Where(v => v.VoucherId == receipt.VoucherId).ToList();
                        var items = _dbContext.ARReceiptInvoices.Where(r => r.IsDeleted == false && r.ReceiptId == id).ToList();
                        foreach (var item in items)
                        {
                            var invoice = _dbContext.ARInvoices.Find(item.InvoiceId);
                            invoice.ReceiptTotal = invoice.ReceiptTotal - item.LineTotal;
                            var dbEntry = _dbContext.ARInvoices.Update(invoice);
                            dbEntry.OriginalValues.SetValues(await dbEntry.GetDatabaseValuesAsync());
                            await _dbContext.SaveChangesAsync();
                        }
                        foreach (var item in voucherDetail)
                        {
                            var tracker = _dbContext.GLVoucherDetails.Remove(item);
                            tracker.OriginalValues.SetValues(await tracker.GetDatabaseValuesAsync());
                            await _dbContext.SaveChangesAsync();
                        }
                        receipt.Status = "Created";
                        receipt.ApprovedBy = null;
                        receipt.ApprovedDate = null;
                        var entry = _dbContext.ARReceipts.Update(receipt);
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