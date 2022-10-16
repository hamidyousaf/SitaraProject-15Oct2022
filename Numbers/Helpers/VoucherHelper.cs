using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Helpers
{
    public class VoucherHelper
    {
        private HttpContext HttpContext { get; }
        private int _companyId;
        private string _userId;
        private readonly NumbersDbContext _dbContext;
        private readonly Controller _controller;
        public VoucherHelper(NumbersDbContext dbContext, HttpContext httpContext,Controller controller)
        {
            _dbContext = dbContext;
            HttpContext = httpContext;
            _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            _userId = httpContext.Session.GetString("UserId");
            _controller = controller;
        }
        public VoucherHelper(NumbersDbContext dbContext, HttpContext httpContext)
        {
            _dbContext = dbContext;
            HttpContext = httpContext;
            _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            _userId = httpContext.Session.GetString("UserId");

        }
        public VoucherHelper(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;

        }
        public int CreateVoucher(GLVoucher voucherMaster, IList<GLVoucherDetail> voucherDetail)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                int voucherId = 0;
                try
                {
                    //var voucher = _dbContext.GLVouchers.Where(v => v.ModuleName == voucherMaster.ModuleName && v.ModuleId == voucherMaster.ModuleId && v.VoucherType==voucherMaster.VoucherType && v.IsSystem).FirstOrDefault();
                    //if (voucher == null)//If Previously not created voucher master
                    //{
                    voucherMaster.CompanyId = _companyId;
                    voucherMaster.CreatedBy = _userId;
                    voucherMaster.CreatedDate = DateTime.Now;
                    voucherMaster.PeriodId = GetPeriodIdAutoVoucher(voucherMaster.VoucherDate, _companyId);
                    voucherMaster.VoucherNo = GetNewVoucherNoAutoVoucher(voucherMaster.VoucherType, voucherMaster.PeriodId, _companyId);

                    _dbContext.GLVouchers.Add(voucherMaster);
                    _dbContext.SaveChanges();

                    voucherId = voucherMaster.Id;
                    //}
                    //else
                    //{
                    //    voucherId = voucher.Id;
                    //}
                    decimal debit = 0;
                    decimal credit = 0;
                    foreach (var item in voucherDetail)
                    {
                        item.Id = 0;
                        debit = debit + item.Debit;
                        credit = credit + item.Credit;
                        item.VoucherId = voucherId;
                        if (item.AccountId != 0)
                            _dbContext.GLVoucherDetails.Add(item);
                    }
                    if (debit == credit && debit > 0)
                    {
                        _dbContext.SaveChanges();
                        transaction.Commit();
                        return voucherId;
                    }
                    else
                    {
                        transaction.Rollback();
                        return voucherId = 0;
                    }
                }
                catch (Exception Exp)
                {
                    _controller.TempData["error"] = "true";
                    _controller.TempData["message"] = Exp.InnerException == null ? Exp.Message : Exp.InnerException.Message;
                    transaction.Rollback();
                    return voucherId = 0;
                }
            }
        }
        public int CreateVoucher(GLVoucher voucherMaster, IList<GLVoucherDetail> voucherDetail,int _companyId,string _userId)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                int voucherId = 0;
                try
                {
                    //var voucher = _dbContext.GLVouchers.Where(v => v.ModuleName == voucherMaster.ModuleName && v.ModuleId == voucherMaster.ModuleId && v.VoucherType==voucherMaster.VoucherType && v.IsSystem).FirstOrDefault();
                    //if (voucher == null)//If Previously not created voucher master
                    //{
                        voucherMaster.CompanyId = _companyId;
                        voucherMaster.CreatedBy = _userId;
                        voucherMaster.CreatedDate = DateTime.Now;
                        voucherMaster.PeriodId = GetPeriodIdAutoVoucher(voucherMaster.VoucherDate,_companyId);
                        voucherMaster.VoucherNo = GetNewVoucherNoAutoVoucher(voucherMaster.VoucherType, voucherMaster.PeriodId, _companyId);

                        _dbContext.GLVouchers.Add(voucherMaster);
                        _dbContext.SaveChanges();

                        voucherId = voucherMaster.Id;
                    //}
                    //else
                    //{
                    //    voucherId = voucher.Id;
                    //}
                    decimal debit = 0;
                    decimal credit = 0;
                    foreach (var item in voucherDetail)
                    {
                        item.Id = 0;
                        debit = debit+item.Debit;
                        credit = credit+item.Credit;
                        item.VoucherId = voucherId;
                        if(item.AccountId!=0)
                            _dbContext.GLVoucherDetails.Add(item);
                    }
                    if (debit == credit && debit > 0)
                    {
                        _dbContext.SaveChanges();
                        transaction.Commit();
                        return voucherId;
                    }
                    else
                    {
                        transaction.Rollback();
                        return voucherId=0;                        
                    }                    
                }
                catch (Exception Exp)
                {
                    _controller.TempData["error"] = "true";
                    _controller.TempData["message"] = Exp.InnerException == null ? Exp.Message : Exp.InnerException.Message;
                    transaction.Rollback();
                    return voucherId=0;
                }
            }           
        }
        public int GetPeriodId(DateTime date)
        {
            int? returnVal = _dbContext.AppPeriods
                            .First(p => p.CompanyId == _companyId && (date >= p.StartDate && date <= p.EndDate)).Id;
            if (returnVal == null)
                returnVal = 0;
            return returnVal.Value;
        }

        public int GetPeriodIdAutoVoucher(DateTime date,int _companyId)
        {
            int? returnVal = _dbContext.AppPeriods
                            .First(p => p.CompanyId == _companyId && (date >= p.StartDate && date <= p.EndDate)).Id;
            if (returnVal == null)
                returnVal = 0;
            return returnVal.Value;
        }
        public int GetNewVoucherNoAutoVoucher(string voucherType, int periodId,int _companyId)
        {
            int returnVal = 1;
            var voucher = _dbContext.GLVouchers
                            .Where(v => v.CompanyId == _companyId &&
                            v.IsDeleted == false &&
                            v.VoucherType == voucherType &&
                            v.PeriodId == periodId).Max(v => (int?)v.VoucherNo);
            if (voucher != null)
                returnVal = voucher.Value + 1;

            return returnVal;
        } 
        public int GetNewVoucherNo(string voucherType, int periodId)
        {
            int returnVal = 1;
            var voucher = _dbContext.GLVouchers
                            .Where(v => v.CompanyId == _companyId &&
                            v.IsDeleted == false &&
                            v.VoucherType == voucherType &&
                            v.PeriodId == periodId).Max(v => (int?)v.VoucherNo);
            if (voucher != null)
                returnVal = voucher.Value + 1;

            return returnVal;
        }
        //Cash Purchase sale payment voucher
        public int CreateCashPurchaseSalePaymentVoucher(BkgCashPurchaseSaleViewModel cashPayment)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");

            #region  "Create Voucher"
            VoucherHelper voucher = new VoucherHelper(_dbContext, HttpContext);
            GLVoucher voucherMaster = new GLVoucher();
            List<GLVoucherDetail> voucherDetails = new List<GLVoucherDetail>();

            //CashPurchaseSale Payment voucher
            voucherMaster.VoucherType = "CP-PY";
            voucherMaster.VoucherDate = cashPayment.PaymentDate;
            voucherMaster.Reference = cashPayment.PaymentReference;
            voucherMaster.Currency = "PKR";
            voucherMaster.CurrencyExchangeRate = 1;
            voucherMaster.Description = cashPayment.PaymentRemarksD;
            voucherMaster.Status = "Created";
            //voucherMaster.IsSystem = true;
            voucherMaster.ModuleName = "Booking Cash Purchase Sale";
            voucherMaster.ModuleId = cashPayment.Id;

            //Voucher Details

            //Debit Entry
            GLVoucherDetail voucherDetail = new GLVoucherDetail();
            voucherDetail.AccountId = cashPayment.PaymentGLAccountId;
            voucherDetail.Sequence = 10;
            voucherDetail.Description = cashPayment.PaymentRemarksD;
            voucherDetail.Debit = cashPayment.PaymentAmountD;
            voucherDetail.Credit = 0;
            voucherDetail.IsDeleted = false;
            voucherDetail.CreatedBy = userId;
            voucherDetail.CreatedDate = DateTime.Now;
            voucherDetails.Add(voucherDetail);

            //Credit Entry
            voucherDetail = new GLVoucherDetail();
            voucherDetail.AccountId = 24;
            voucherDetail.Sequence = 20;
            voucherDetail.Description = cashPayment.PaymentRemarksD;
            voucherDetail.Debit = 0;
            voucherDetail.Credit = cashPayment.PaymentAmountD;
            voucherDetail.IsDeleted = false;
            voucherDetail.CreatedBy = userId;
            voucherDetail.CreatedDate = DateTime.Now;
            voucherDetails.Add(voucherDetail);

            //Create Voucher
            int voucherId = 0;
            voucherId =voucher.CreateVoucher(voucherMaster, voucherDetails);
            return voucherId;
            #endregion
        }

        //Cash purchase sale Receipt Voucher
        public int CreateCashPurchaseSaleReceiptVoucher(BkgCashPurchaseSaleViewModel cashReceipt)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");

            #region  "Create Voucher"
            VoucherHelper voucher = new VoucherHelper(_dbContext, HttpContext);
            GLVoucher voucherMaster = new GLVoucher();
            List<GLVoucherDetail> voucherDetails = new List<GLVoucherDetail>();

            //CashPurchaseSale Payment voucher
            voucherMaster.VoucherType = "CP-R";
            voucherMaster.VoucherDate = cashReceipt.ReceiptDate;
            voucherMaster.Reference = cashReceipt.ReceiptsReference;
            voucherMaster.Currency = "PKR";
            voucherMaster.CurrencyExchangeRate = 1;
            voucherMaster.Description = cashReceipt.ReceiptsRemarks;
            voucherMaster.Status = "Created";
            //voucherMaster.IsSystem = true;
            voucherMaster.ModuleName = "Booking Cash Purchase Sale";
            voucherMaster.ModuleId = cashReceipt.Id;

            //Voucher Details

            //Debit Entry
            GLVoucherDetail voucherDetail = new GLVoucherDetail();
            voucherDetail.AccountId = cashReceipt.ReceiptsGLAccountId;
            voucherDetail.Sequence = 10;
            voucherDetail.Description = cashReceipt.ReceiptsRemarks;
            voucherDetail.Debit = cashReceipt.ReceiptAmount;
            voucherDetail.Credit = 0;
            voucherDetail.IsDeleted = false;
            voucherDetail.CreatedBy = userId;
            voucherDetail.CreatedDate = DateTime.Now;
            voucherDetails.Add(voucherDetail);

            //Credit Entry
            voucherDetail = new GLVoucherDetail();
            voucherDetail.AccountId = 24;
            voucherDetail.Sequence = 20;
            voucherDetail.Description = cashReceipt.ReceiptsRemarks;
            voucherDetail.Debit = 0;
            voucherDetail.Credit = cashReceipt.ReceiptAmount;
            voucherDetail.IsDeleted = false;
            voucherDetail.CreatedBy = userId;
            voucherDetail.CreatedDate = DateTime.Now;
            voucherDetails.Add(voucherDetail);

            //Create Voucher
            int voucherId = 0;
            voucherId = voucher.CreateVoucher(voucherMaster, voucherDetails);
            return voucherId;
            #endregion
        }
    }
}