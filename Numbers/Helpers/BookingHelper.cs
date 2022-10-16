using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Helpers;
namespace Numbers.Helpers
{
    public static class BookingHelper
    {
        public static BkgCashPurchaseSaleViewModel getBkgCashPurchaseSaleViewModel
            (NumbersDbContext numbersDbContext, int bkgCashPurchaseSaleId)
        {
            BkgCashPurchaseSaleViewModel bkgCashPurchaseSaleViewModel = new BkgCashPurchaseSaleViewModel();
            BkgCashPurchaseSale bkgCashPurchaseSale = numbersDbContext.BkgCashPurchaseSales.Find(bkgCashPurchaseSaleId);
            //        List<BkgCashPurchaseSalePayment> bkgCashPurchaseSalePayment = numbersDbContext.
            //            BkgCashPurchaseSalePayments.Where(w => w.BkgCashPurchaseSaleId == bkgCashPurchaseSaleId).ToList();
            //        List<BkgCashPurchaseSaleReceipt> bkgCashPurchaseSaleReceipts = numbersDbContext.
            //BkgCashPurchaseSaleReceipts.Where(w => w.BkgCashPurchaseSaleId == bkgCashPurchaseSaleId).ToList();


            List<BkgCashPurchaseSalePayment> bkgCashPurchaseSalePayment = numbersDbContext.
            BkgCashPurchaseSalePayments.Where(w => w.CashPurchaseSaleId == bkgCashPurchaseSaleId).ToList();

            List<BkgCashPurchaseSaleReceipt> bkgCashPurchaseSaleReceipt = numbersDbContext.
            BkgCashPurchaseSaleReceipts.Where(w => w.CashPurchaseSaleId == bkgCashPurchaseSaleId).ToList();



            bkgCashPurchaseSaleViewModel.BookingDate = bkgCashPurchaseSale.BookingDate;
            bkgCashPurchaseSaleViewModel.BookingNo = bkgCashPurchaseSale.BookingNo;
            bkgCashPurchaseSaleViewModel.BookingRemarks = bkgCashPurchaseSale.BookingRemarks;
            bkgCashPurchaseSaleViewModel.BookingStatus = bkgCashPurchaseSale.BookingStatus;
            bkgCashPurchaseSaleViewModel.ChassisNo = bkgCashPurchaseSale.ChassisNo;
            bkgCashPurchaseSaleViewModel.CompanyId = bkgCashPurchaseSale.CompanyId;
            bkgCashPurchaseSaleViewModel.CustomerId = bkgCashPurchaseSale.CustomerId;
            bkgCashPurchaseSaleViewModel.EngineNo = bkgCashPurchaseSale.EngineNo;
            bkgCashPurchaseSaleViewModel.EstimatedPurchase = bkgCashPurchaseSale.EstimatedPurchase;
            bkgCashPurchaseSaleViewModel.Id = bkgCashPurchaseSale.Id;
            bkgCashPurchaseSaleViewModel.IGPDate = bkgCashPurchaseSale.IGPDate;
            bkgCashPurchaseSaleViewModel.IGPNo = bkgCashPurchaseSale.IGPNo;
            bkgCashPurchaseSaleViewModel.IGPStatus = bkgCashPurchaseSale.IGPStatus;
            bkgCashPurchaseSaleViewModel.IssuanceRemarks = bkgCashPurchaseSale.IssuanceRemarks;
            bkgCashPurchaseSaleViewModel.ItemId = bkgCashPurchaseSale.ItemId;
            bkgCashPurchaseSaleViewModel.OGPDate = bkgCashPurchaseSale.OGPDate;
            bkgCashPurchaseSaleViewModel.OGPNo = bkgCashPurchaseSale.OGPNo;
            bkgCashPurchaseSaleViewModel.OGPStatus = bkgCashPurchaseSale.OGPStatus;
            bkgCashPurchaseSaleViewModel.PaymentAmount = bkgCashPurchaseSale.PaymentTotalAmount;
            bkgCashPurchaseSaleViewModel.ReceiptAmount = bkgCashPurchaseSale.ReceiptTotalAmount;
            bkgCashPurchaseSaleViewModel.SalesPartyAccount = bkgCashPurchaseSale.SalesPartyAccount;
            bkgCashPurchaseSaleViewModel.VoucherId = bkgCashPurchaseSale.VoucherId;
            bkgCashPurchaseSaleViewModel.PurchaseVoucherId = bkgCashPurchaseSale.PurchaseVoucherId;
            bkgCashPurchaseSaleViewModel.SalesVoucherId = bkgCashPurchaseSale.SalesVoucherId;
            bkgCashPurchaseSaleViewModel.InvoiceNo = bkgCashPurchaseSale.InvoiceNo;
            

            bkgCashPurchaseSaleViewModel.PurchaseDate = bkgCashPurchaseSale.PurchaseDate;
            bkgCashPurchaseSaleViewModel.PurchaseParty = bkgCashPurchaseSale.PurchaseParty;
            bkgCashPurchaseSaleViewModel.PurchaseRate = bkgCashPurchaseSale.PurchaseRate;
            bkgCashPurchaseSaleViewModel.PurchaseRemarks = bkgCashPurchaseSale.PurchaseRemarks;
            bkgCashPurchaseSaleViewModel.ReceivingRemarks = bkgCashPurchaseSale.ReceivingRemarks;
            bkgCashPurchaseSaleViewModel.SalesCustomerCNIC = bkgCashPurchaseSale.SalesCustomerCNIC;
            bkgCashPurchaseSaleViewModel.SalesDate = bkgCashPurchaseSale.SalesDate;
            bkgCashPurchaseSaleViewModel.SalesParty = bkgCashPurchaseSale.SalesParty;
            bkgCashPurchaseSaleViewModel.SalesPartyAccount = bkgCashPurchaseSale.SalesPartyAccount;
            bkgCashPurchaseSaleViewModel.SalesRate = bkgCashPurchaseSale.SalesRate;
            bkgCashPurchaseSaleViewModel.SalesRemarks = bkgCashPurchaseSale.SalesRemarks;
            bkgCashPurchaseSaleViewModel.SalesStatus = bkgCashPurchaseSale.SalesStatus;
            bkgCashPurchaseSaleViewModel.Status = bkgCashPurchaseSale.Status;
            bkgCashPurchaseSaleViewModel.TransDate = bkgCashPurchaseSale.TransDate;
            //bkgCashPurchaseSaleViewModel.AdvancePurchase = bkgCashPurchaseSale.AdvancePurchase;
            //bkgCashPurchaseSaleViewModel.IsDeleted = bkgCashPurchaseSale.IsDeleted;

            if (bkgCashPurchaseSalePayment != null)
            {
                bkgCashPurchaseSaleViewModel.PurchasePayments = bkgCashPurchaseSalePayment;
                ////Payments
                //bkgCashPurchaseSaleViewModel.PaymentReference = bkgCashPurchaseSalePayment.Reference;
                //bkgCashPurchaseSaleViewModel.PaymentRemarksD = bkgCashPurchaseSalePayment.Remarks;
                //bkgCashPurchaseSaleViewModel.PaymentDate = bkgCashPurchaseSalePayment.PaymentDate;
                //bkgCashPurchaseSaleViewModel.PaymentAmountD = bkgCashPurchaseSalePayment.PaymentAmount;
                //bkgCashPurchaseSaleViewModel.PaymentGLAccountId = bkgCashPurchaseSalePayment.BankCashAccountId;
            }
            else
                bkgCashPurchaseSaleViewModel.PaymentDate = DateTime.Now;
            if (bkgCashPurchaseSaleReceipt != null)
            {
                bkgCashPurchaseSaleViewModel.SaleReceipts = bkgCashPurchaseSaleReceipt;

                //Receipts
                //bkgCashPurchaseSaleViewModel.ReceiptAmount = bkgCashPurchaseSaleReceipt.ReceiptAmount;
                //bkgCashPurchaseSaleViewModel.ReceiptDate = bkgCashPurchaseSaleReceipt.ReceiptDate;
                //bkgCashPurchaseSaleViewModel.ReceiptsGLAccountId = bkgCashPurchaseSaleReceipt.BankCashAccountId;
                //bkgCashPurchaseSaleViewModel.ReceiptsReference = bkgCashPurchaseSaleReceipt.Reference;
                //bkgCashPurchaseSaleViewModel.ReceiptsRemarks = bkgCashPurchaseSaleReceipt.Remarks;
            }
            else
                bkgCashPurchaseSaleViewModel.ReceiptDate = DateTime.Now;

            bkgCashPurchaseSaleViewModel.UpdatedBy = bkgCashPurchaseSale.UpdatedBy;
            bkgCashPurchaseSaleViewModel.UpdatedDate = bkgCashPurchaseSale.UpdatedDate;
            bkgCashPurchaseSaleViewModel.CreatedBy = bkgCashPurchaseSale.CreatedBy;
            bkgCashPurchaseSaleViewModel.CreatedDate = bkgCashPurchaseSale.CreatedDate;

            return bkgCashPurchaseSaleViewModel;
        }

        public static int saveBkgCashPurchaseSale(NumbersDbContext numbersDbContext, BkgCashPurchaseSaleViewModel
            bkgCashPurchaseSaleViewModel, int bkgCashPurchaseSaleId, string activeUserID, int activeCompanyID)
        {

            BkgCashPurchaseSale bkgCashPurchaseSale = new BkgCashPurchaseSale();
            BkgCashPurchaseSalePayment bkgCashPurchaseSalePayment = new BkgCashPurchaseSalePayment();
            BkgCashPurchaseSaleReceipt bkgCashPurchaseSaleReceipt = new BkgCashPurchaseSaleReceipt();
            if (bkgCashPurchaseSaleId != 0)
            {
                bkgCashPurchaseSale = numbersDbContext.BkgCashPurchaseSales.Find(bkgCashPurchaseSaleId);

                bkgCashPurchaseSalePayment = numbersDbContext.
         BkgCashPurchaseSalePayments.Where(w => w.CashPurchaseSaleId == bkgCashPurchaseSaleId).FirstOrDefault();
                bkgCashPurchaseSaleReceipt = numbersDbContext.
       BkgCashPurchaseSaleReceipts.Where(w => w.CashPurchaseSaleId == bkgCashPurchaseSaleId).FirstOrDefault();

            }

            bkgCashPurchaseSale.BookingDate = bkgCashPurchaseSaleViewModel.BookingDate;
            bkgCashPurchaseSale.BookingNo = bkgCashPurchaseSaleViewModel.BookingNo;
            bkgCashPurchaseSale.BookingRemarks = bkgCashPurchaseSaleViewModel.BookingRemarks;
            bkgCashPurchaseSale.BookingStatus = bkgCashPurchaseSaleViewModel.BookingStatus;
            bkgCashPurchaseSale.ChassisNo = bkgCashPurchaseSaleViewModel.ChassisNo;
            bkgCashPurchaseSale.CompanyId = bkgCashPurchaseSaleViewModel.CompanyId;
            bkgCashPurchaseSale.CustomerId = bkgCashPurchaseSaleViewModel.CustomerId;
            bkgCashPurchaseSale.EngineNo = bkgCashPurchaseSaleViewModel.EngineNo;
            bkgCashPurchaseSale.EstimatedPurchase = bkgCashPurchaseSaleViewModel.EstimatedPurchase;
            bkgCashPurchaseSale.Id = bkgCashPurchaseSaleViewModel.Id;
            bkgCashPurchaseSale.IGPDate = bkgCashPurchaseSaleViewModel.IGPDate;
            bkgCashPurchaseSale.IGPNo = bkgCashPurchaseSaleViewModel.IGPNo;
            bkgCashPurchaseSale.IGPStatus = bkgCashPurchaseSaleViewModel.IGPStatus;
            bkgCashPurchaseSale.IssuanceRemarks = bkgCashPurchaseSaleViewModel.IssuanceRemarks;
            bkgCashPurchaseSale.ItemId = bkgCashPurchaseSaleViewModel.ItemId;
            bkgCashPurchaseSale.OGPDate = bkgCashPurchaseSaleViewModel.OGPDate;
            bkgCashPurchaseSale.OGPNo = bkgCashPurchaseSaleViewModel.OGPNo;
            bkgCashPurchaseSale.OGPStatus = bkgCashPurchaseSaleViewModel.OGPStatus;
            bkgCashPurchaseSale.PaymentTotalAmount = bkgCashPurchaseSaleViewModel.PaymentAmount;
            bkgCashPurchaseSale.SalesCustomerId = bkgCashPurchaseSaleViewModel.CustomerId;
            bkgCashPurchaseSale.PurchaseDate = bkgCashPurchaseSaleViewModel.PurchaseDate;
            bkgCashPurchaseSale.PurchaseParty = bkgCashPurchaseSaleViewModel.PurchaseParty;
            bkgCashPurchaseSale.PurchaseRate = bkgCashPurchaseSaleViewModel.PurchaseRate;
            bkgCashPurchaseSale.PurchaseRemarks = bkgCashPurchaseSaleViewModel.PurchaseRemarks;
            bkgCashPurchaseSale.ReceivingRemarks = bkgCashPurchaseSaleViewModel.ReceivingRemarks;
            bkgCashPurchaseSale.SalesCustomerCNIC = bkgCashPurchaseSaleViewModel.SalesCustomerCNIC;
            bkgCashPurchaseSale.SalesDate = bkgCashPurchaseSaleViewModel.SalesDate;
            bkgCashPurchaseSale.SalesParty = bkgCashPurchaseSaleViewModel.SalesParty;
            bkgCashPurchaseSale.SalesPartyAccount = bkgCashPurchaseSaleViewModel.SalesPartyAccount;
            bkgCashPurchaseSale.SalesRate = bkgCashPurchaseSaleViewModel.SalesRate;
            bkgCashPurchaseSale.SalesRemarks = bkgCashPurchaseSaleViewModel.SalesRemarks;
            bkgCashPurchaseSale.SalesStatus = bkgCashPurchaseSaleViewModel.SalesStatus;
            bkgCashPurchaseSale.Status = bkgCashPurchaseSaleViewModel.Status;
            bkgCashPurchaseSale.TransDate = bkgCashPurchaseSaleViewModel.TransDate;
            /*
            //Payments
            bkgCashPurchaseSalePayment.Reference = bkgCashPurchaseSalePayment.Reference;
            bkgCashPurchaseSalePayment.Remarks = bkgCashPurchaseSalePayment.Remarks;
            bkgCashPurchaseSalePayment.PaymentDate = bkgCashPurchaseSalePayment.PaymentDate;
            bkgCashPurchaseSalePayment.PaymentAmount = bkgCashPurchaseSalePayment.PaymentAmount;
            bkgCashPurchaseSalePayment.GLAccountId = bkgCashPurchaseSalePayment.GLAccountId;

            //Receipts
            bkgCashPurchaseSaleReceipt.ReceiptAmount = bkgCashPurchaseSaleReceipt.ReceiptAmount;
            bkgCashPurchaseSaleReceipt.ReceiptDate = bkgCashPurchaseSaleReceipt.ReceiptDate;
            bkgCashPurchaseSaleReceipt.GLAccountId = bkgCashPurchaseSaleReceipt.GLAccountId;
            bkgCashPurchaseSaleReceipt.Reference = bkgCashPurchaseSaleReceipt.Reference;
            bkgCashPurchaseSaleReceipt.Remarks = bkgCashPurchaseSaleReceipt.Remarks;



            bkgCashPurchaseSale.UpdatedBy = bkgCashPurchaseSaleViewModel.UpdatedBy;
            bkgCashPurchaseSale.UpdatedDate = bkgCashPurchaseSaleViewModel.UpdatedDate;
            bkgCashPurchaseSale.CreatedBy = bkgCashPurchaseSaleViewModel.CreatedBy;
            bkgCashPurchaseSale.CreatedDate = bkgCashPurchaseSaleViewModel.CreatedDate;

            bkgCashPurchaseSalePayment.CreatedBy = bkgCashPurchaseSaleViewModel.CreatedBy;
            bkgCashPurchaseSalePayment.CreatedDate = bkgCashPurchaseSaleViewModel.CreatedDate;

            bkgCashPurchaseSaleReceipt.CreatedBy = bkgCashPurchaseSaleViewModel.CreatedBy;
            bkgCashPurchaseSaleReceipt.CreatedDate = bkgCashPurchaseSaleViewModel.CreatedDate;
            */
            if (bkgCashPurchaseSaleId == 0)
            {
                bkgCashPurchaseSale.CompanyId = activeCompanyID;
                bkgCashPurchaseSale.CreatedBy = activeUserID;
                bkgCashPurchaseSale.CreatedDate = DateTime.Now;
                numbersDbContext.BkgCashPurchaseSales.Add(bkgCashPurchaseSale);
            }
            else
            {
                numbersDbContext.Entry(bkgCashPurchaseSale).State = EntityState.Modified;
                numbersDbContext.Entry(bkgCashPurchaseSale).Property("CompanyId").IsModified = false;
                numbersDbContext.Entry(bkgCashPurchaseSale).Property("CreatedBy").IsModified = false;
                numbersDbContext.Entry(bkgCashPurchaseSale).Property("CreatedDate").IsModified = false;
                numbersDbContext.Entry(bkgCashPurchaseSale).Property("Status").IsModified = false;

                bkgCashPurchaseSale.UpdatedBy = activeUserID;
                bkgCashPurchaseSale.UpdatedDate = DateTime.Now;
            }
            numbersDbContext.SaveChanges();
            return bkgCashPurchaseSale.Id;
        }

        /*
        public static bool saveBkgCashPurchaseSalePayment(NumbersDbContext numbersDbContext,
            BkgCashPurchaseSaleViewModel bkgCashPurchaseSaleViewModel, int bkgCashPurchaseSaleId, int activeUserID, int activeCompanyID)
        {

            BkgCashPurchaseSalePayment bkgCashPurchaseSalePayment = new BkgCashPurchaseSalePayment();
            if (bkgCashPurchaseSaleId != 0)
            {
                bkgCashPurchaseSalePayment = numbersDbContext.
         BkgCashPurchaseSalePayments.Where(w => w.BkgCashPurchaseSaleId == bkgCashPurchaseSaleId).FirstOrDefault();

            }


            //Payments
            bkgCashPurchaseSalePayment.Reference = bkgCashPurchaseSalePayment.Reference;
            bkgCashPurchaseSalePayment.Remarks = bkgCashPurchaseSalePayment.Remarks;
            bkgCashPurchaseSalePayment.PaymentDate = bkgCashPurchaseSalePayment.PaymentDate;
            bkgCashPurchaseSalePayment.PaymentAmount = bkgCashPurchaseSalePayment.PaymentAmount;
            bkgCashPurchaseSalePayment.GLAccountId = bkgCashPurchaseSalePayment.GLAccountId;




            if (bkgCashPurchaseSaleId == 0)
            {
                bkgCashPurchaseSalePayment.CreatedBy = activeUserID;
                bkgCashPurchaseSalePayment.CreatedDate = DateTime.Now;

                numbersDbContext.BkgCashPurchaseSalePayments.Add(bkgCashPurchaseSalePayment);
            }
            else
            {
                numbersDbContext.Entry(bkgCashPurchaseSalePayment).State = EntityState.Modified;
                numbersDbContext.Entry(bkgCashPurchaseSalePayment).Property("CompanyId").IsModified = false;
                numbersDbContext.Entry(bkgCashPurchaseSalePayment).Property("CreatedBy").IsModified = false;
                numbersDbContext.Entry(bkgCashPurchaseSalePayment).Property("CreatedDate").IsModified = false;
                numbersDbContext.Entry(bkgCashPurchaseSalePayment).Property("Status").IsModified = false;

                bkgCashPurchaseSalePayment.UpdatedBy = activeUserID;
                bkgCashPurchaseSalePayment.UpdatedDate = DateTime.Now;
            }
            numbersDbContext.SaveChanges();
            return true;
        }

        public static bool saveBkgCashPurchaseSaleReceipt(NumbersDbContext numbersDbContext,
            BkgCashPurchaseSaleViewModel bkgCashPurchaseSaleViewModel, int bkgCashPurchaseSaleId, int activeUserID, int activeCompanyID)
        {

            BkgCashPurchaseSaleReceipt bkgCashPurchaseSaleReceipt = new BkgCashPurchaseSaleReceipt();
            if (bkgCashPurchaseSaleId != 0)
            {
                bkgCashPurchaseSaleReceipt = numbersDbContext.
                     BkgCashPurchaseSaleReceipts.Where(w => w.BkgCashPurchaseSaleId == bkgCashPurchaseSaleId).FirstOrDefault();

            }


            //Receipts
            bkgCashPurchaseSaleReceipt.Reference = bkgCashPurchaseSaleReceipt.Reference;
            bkgCashPurchaseSaleReceipt.Remarks = bkgCashPurchaseSaleReceipt.Remarks;
            bkgCashPurchaseSaleReceipt.ReceiptDate = bkgCashPurchaseSaleReceipt.ReceiptDate;
            bkgCashPurchaseSaleReceipt.ReceiptAmount = bkgCashPurchaseSaleReceipt.ReceiptAmount;
            bkgCashPurchaseSaleReceipt.GLAccountId = bkgCashPurchaseSaleReceipt.GLAccountId;

            if (bkgCashPurchaseSaleId == 0)
            {
                bkgCashPurchaseSaleReceipt.CreatedBy = activeUserID;
                bkgCashPurchaseSaleReceipt.CreatedDate = DateTime.Now;

                numbersDbContext.BkgCashPurchaseSaleReceipts.Add(bkgCashPurchaseSaleReceipt);
            }
            else
            {
                numbersDbContext.Entry(bkgCashPurchaseSaleReceipt).State = EntityState.Modified;
                numbersDbContext.Entry(bkgCashPurchaseSaleReceipt).Property("CompanyId").IsModified = false;
                numbersDbContext.Entry(bkgCashPurchaseSaleReceipt).Property("CreatedBy").IsModified = false;
                numbersDbContext.Entry(bkgCashPurchaseSaleReceipt).Property("CreatedDate").IsModified = false;
                numbersDbContext.Entry(bkgCashPurchaseSaleReceipt).Property("Status").IsModified = false;

                bkgCashPurchaseSaleReceipt.UpdatedBy = activeUserID;
                bkgCashPurchaseSaleReceipt.UpdatedDate = DateTime.Now;
            }
            numbersDbContext.SaveChanges();
            return true;
        }
        */
        public static bool SaveBkgCashPurchasePurchaseInformation(NumbersDbContext numbersDbContext,
            BkgCashPurchaseSaleViewModel cashPurchaseSaleViewModel, string activeUserID, int activeCompanyID)
        {

            BkgCashPurchaseSale cashPurchaseSale = numbersDbContext.BkgCashPurchaseSales.Find(cashPurchaseSaleViewModel.Id);
            cashPurchaseSale.PurchaseDate = cashPurchaseSaleViewModel.PurchaseDate;
            cashPurchaseSale.PurchaseRate = cashPurchaseSaleViewModel.PurchaseRate;
            cashPurchaseSale.PurchaseParty = cashPurchaseSaleViewModel.PurchaseParty;
            cashPurchaseSale.PurchaseRemarks = cashPurchaseSaleViewModel.PurchaseRemarks;
            numbersDbContext.BkgCashPurchaseSales.Update(cashPurchaseSale);

            numbersDbContext.SaveChanges();
            return true;
        }

        public static bool saveBkgCashPurchaseSaleInformation(NumbersDbContext numbersDbContext,
           BkgCashPurchaseSaleViewModel cashPurchaseSaleViewModel, string activeUserID, int activeCompanyID)
        {

            BkgCashPurchaseSale cashPurchaseSale = numbersDbContext.BkgCashPurchaseSales.Find(cashPurchaseSaleViewModel.Id);

            cashPurchaseSale.SalesDate = cashPurchaseSaleViewModel.SalesDate;
            cashPurchaseSale.SalesRate = cashPurchaseSaleViewModel.SalesRate;
            cashPurchaseSale.SalesParty = cashPurchaseSaleViewModel.SalesParty;
            cashPurchaseSale.SalesRemarks = cashPurchaseSaleViewModel.SalesRemarks;
            cashPurchaseSale.SalesPartyAccount = cashPurchaseSaleViewModel.SalesPartyAccount;
            numbersDbContext.BkgCashPurchaseSales.Update(cashPurchaseSale);


            numbersDbContext.SaveChanges();
            return true;
        }

        public static bool saveCashPurchaseSalePayment(NumbersDbContext numbersDbContext,HttpContext httpContext, 
           BkgCashPurchaseSaleViewModel cashPurchaseSaleViewModel, string activeUserID, int activeCompanyID)
        {
            VoucherHelper voucherHelper = new VoucherHelper(numbersDbContext,httpContext);
            BkgCashPurchaseSale cashPurchaseSale = numbersDbContext.BkgCashPurchaseSales.Find(cashPurchaseSaleViewModel.Id);
            cashPurchaseSale.PaymentTotalAmount = cashPurchaseSale.PaymentTotalAmount + cashPurchaseSaleViewModel.PaymentAmountD;
            BkgCashPurchaseSalePayment cashPurchaseSalePayment = new BkgCashPurchaseSalePayment();
            cashPurchaseSalePayment.CashPurchaseSaleId = cashPurchaseSaleViewModel.Id;
            cashPurchaseSalePayment.CreatedBy = activeUserID;
            cashPurchaseSalePayment.CreatedDate = DateTime.Now;
            cashPurchaseSalePayment.BankCashAccountId = cashPurchaseSaleViewModel.PaymentGLAccountId;
            cashPurchaseSalePayment.PaymentAmount = cashPurchaseSaleViewModel.PaymentAmountD;
            cashPurchaseSalePayment.PaymentDate = cashPurchaseSaleViewModel.PaymentDate;
            cashPurchaseSalePayment.Reference = cashPurchaseSaleViewModel.PaymentReference;
            cashPurchaseSalePayment.Remarks = cashPurchaseSaleViewModel.PaymentRemarksD;
            cashPurchaseSalePayment.VoucherId = voucherHelper.CreateCashPurchaseSalePaymentVoucher(cashPurchaseSaleViewModel);

            numbersDbContext.BkgCashPurchaseSales.Update(cashPurchaseSale);
            numbersDbContext.BkgCashPurchaseSalePayments.Add(cashPurchaseSalePayment);

            numbersDbContext.SaveChanges();
            return true;
        }
        


        public static bool saveCashPurchaseSaleReceipt(NumbersDbContext numbersDbContext,HttpContext httpContext,
           BkgCashPurchaseSaleViewModel cashPurchaseSaleViewModel, string activeUserID, int activeCompanyID)
        {
            VoucherHelper voucherHelper = new VoucherHelper(numbersDbContext, httpContext);
            BkgCashPurchaseSaleReceipt cashPurchaseSaleReceipt = new BkgCashPurchaseSaleReceipt();
            BkgCashPurchaseSale cashPurchaseSale = numbersDbContext.BkgCashPurchaseSales.Find(cashPurchaseSaleViewModel.Id);
            cashPurchaseSale.ReceiptTotalAmount = cashPurchaseSale.ReceiptTotalAmount + cashPurchaseSaleViewModel.ReceiptAmount;
            cashPurchaseSaleReceipt.CashPurchaseSaleId = cashPurchaseSaleViewModel.Id;
            cashPurchaseSaleReceipt.CreatedBy = activeUserID;
            cashPurchaseSaleReceipt.CreatedDate = DateTime.Now;
            cashPurchaseSaleReceipt.BankCashAccountId = cashPurchaseSaleViewModel.ReceiptsGLAccountId;
            cashPurchaseSaleReceipt.ReceiptAmount = cashPurchaseSaleReceipt.ReceiptAmount + cashPurchaseSaleViewModel.ReceiptAmount;
            cashPurchaseSaleReceipt.ReceiptDate = cashPurchaseSaleViewModel.ReceiptDate;
            cashPurchaseSaleReceipt.Reference = cashPurchaseSaleViewModel.ReceiptsReference;
            cashPurchaseSaleReceipt.Remarks = cashPurchaseSaleViewModel.ReceiptsRemarks;
            cashPurchaseSaleReceipt.VoucherId = voucherHelper.CreateCashPurchaseSaleReceiptVoucher(cashPurchaseSaleViewModel);
            numbersDbContext.BkgCashPurchaseSaleReceipts.Add(cashPurchaseSaleReceipt);

            numbersDbContext.SaveChanges();
            return true;
        }
        //public static BkgCashPurchaseSaleViewModel updateBkgCashPurchaseSaleViewModel(NumbersDbContext numbersDbContext, int id)
        //{
        //    BkgCashPurchaseSaleViewModel cashPurchaseSaleViewModel = new BkgCashPurchaseSaleViewModel();

        //    BkgCashPurchaseSale cashPurchaseSale = numbersDbContext.BkgCashPurchaseSales.Find(id);
        //    BkgCashPurchaseSalePayment cashPurchaseSalePayment = numbersDbContext.BkgCashPurchaseSalePayments
        //        .Where(w => w.BkgCashPurchaseSaleId == id).FirstOrDefault();
        //    BkgCashPurchaseSaleReceipt cashPurchaseSaleReceipt = numbersDbContext.BkgCashPurchaseSaleReceipts
        //        .Where(w => w.BkgCashPurchaseSaleId == id).FirstOrDefault();

        //    cashPurchaseSaleViewModel.AdvancePurchase=
        //    return cashPurchaseSaleViewModel;
        //}


        public static int addVoucher(NumbersDbContext numbersDbContext, GLVoucher voucher, List<GLVoucherDetail> voucherDetail,
            string activeUserID, int activeCompanyID)
        {
            int? periodId = numbersDbContext.AppPeriods
                .First(p => p.CompanyId == activeCompanyID && (voucher.VoucherDate >= p.StartDate && voucher.VoucherDate <= p.EndDate)).Id;



            voucher = new GLVoucher();
            var voucherNo = numbersDbContext.GLVouchers.Where(v => v.CompanyId == activeCompanyID
            && v.VoucherType == voucher.VoucherType && v.IsDeleted == false)
                            .Max(v => (int?)v.VoucherNo);
            if (voucherNo == null)
                voucherNo = 1;
            else
                voucherNo++;
            voucher.VoucherNo = voucherNo.Value;
            voucher.PeriodId = periodId.Value;
            voucher.IsDeleted = false;
            voucher.CompanyId = activeCompanyID;
            voucher.CreatedBy = activeUserID;
            voucher.CreatedDate = DateTime.Now;
            voucher.Status = "Created";

            numbersDbContext.GLVouchers.Add(voucher);

            numbersDbContext.SaveChanges();
            int voucherId = voucher.Id;

            //Create Voucher Detail
            foreach (var item in voucherDetail)
            {
                GLVoucherDetail detail;
                if (item.Id != 0) // New Voucher Detail Line
                    detail = numbersDbContext.GLVoucherDetails.Find(item.Id);
                else
                    detail = new GLVoucherDetail();

                detail.AccountId = item.AccountId;
                detail.VoucherId = voucherId;
                detail.Sequence = item.Sequence;
                detail.SubAccountId = item.SubAccountId;
                detail.Description = item.Description;
                detail.Debit = item.Debit;
                detail.Credit = item.Credit;
                detail.CreatedBy = activeUserID;
                detail.CreatedDate = DateTime.Now;
                detail.IsDeleted = false;
                numbersDbContext.GLVoucherDetails.Add(detail);

            }


            numbersDbContext.SaveChanges();
            numbersDbContext.GLVouchers.Add(voucher);
            return voucherId;
        }


        public static bool checkPurchaseInformationSaved(NumbersDbContext numbersDbContext, int bkgCashPurchaseSaleId)
        {
            bool PurchaseInformationSaved = false;

            BkgCashPurchaseSale bkgCashPurchaseSale = numbersDbContext.BkgCashPurchaseSales.Find(bkgCashPurchaseSaleId);
            if (bkgCashPurchaseSale.PurchaseRate > 0)
                PurchaseInformationSaved = true;

            return PurchaseInformationSaved;
        }
        public static bool checkSalesInformationSaved(NumbersDbContext numbersDbContext, int bkgCashPurchaseSaleId)
        {
            bool SalesInformationSaved = false;

            BkgCashPurchaseSale bkgCashPurchaseSale = numbersDbContext.BkgCashPurchaseSales.Find(bkgCashPurchaseSaleId);
            if (bkgCashPurchaseSale.SalesRate > 0)
                SalesInformationSaved = true;

            return SalesInformationSaved;
        }

    }
}