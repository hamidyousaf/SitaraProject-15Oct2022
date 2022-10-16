using Microsoft.AspNetCore.Http;
using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numbers.Repository.Helpers
{

    public class VoucherHelper
    {
        private HttpContext HttpContext { get; }
        private int _companyId;
        private string _userId;
        private readonly NumbersDbContext _dbContext;
        public VoucherHelper(NumbersDbContext dbContext, HttpContext httpContext)
        {
            _dbContext = dbContext;
            HttpContext = httpContext;
            _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            _userId = httpContext.Session.GetString("UserId");

        }
        //public int CreateVoucher(GLVoucher voucherMaster, IList<GLVoucherDetail> voucherDetail)
        //{
        //    try
        //    {
        //        voucherMaster.CreatedDate = DateTime.Now;
        //        voucherMaster.CreatedBy = _userId;
        //        voucherMaster.PeriodId = GetPeriodId(voucherMaster.VoucherDate);
        //        voucherMaster.VoucherNo = GetNewVoucherNo(voucherMaster.VoucherType, voucherMaster.PeriodId);
        //        _dbContext.GLVouchers.Add(voucherMaster);
        //        _dbContext.SaveChanges();

        //        int voucherId = voucherMaster.Id;
        //        foreach (var item in voucherDetail)
        //        {
        //            item.VoucherId = voucherId;
        //            _dbContext.GLVoucherDetails.Add(item);
        //            _dbContext.SaveChanges();
        //        }
        //        return voucherId;
        //    }
        //    catch (Exception Exp)
        //    {
        //        var message = Exp.Message.ToString();
        //        throw new InvalidOperationException();
        //    }
        //}
        public int CreateVoucher(GLVoucher voucherMaster, IList<GLVoucherDetail> voucherDetail)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                int voucherId = 0;
                try
                {
                    var voucher = _dbContext.GLVouchers.Where(v => v.ModuleName == voucherMaster.ModuleName && v.ModuleId == voucherMaster.ModuleId && v.VoucherType == voucherMaster.VoucherType && v.IsSystem).FirstOrDefault();
                    if (voucher == null)//If Previously not created voucher master
                    {
                        voucherMaster.CompanyId = _companyId;
                        voucherMaster.CreatedBy = _userId;
                        voucherMaster.CreatedDate = DateTime.Now;
                        voucherMaster.Currency = "PKR";
                        voucherMaster.PeriodId = GetPeriodId(voucherMaster.VoucherDate);
                        voucherMaster.VoucherNo = GetNewVoucherNo(voucherMaster.VoucherType, voucherMaster.PeriodId);

                        _dbContext.GLVouchers.Add(voucherMaster);
                        _dbContext.SaveChanges();

                        voucherId = voucherMaster.Id;
                    }
                    else
                    {
                        voucherId = voucher.Id;
                    }
                    decimal debit = 0;
                    decimal credit = 0;
                    foreach (var item in voucherDetail)
                    {
                        debit = debit + item.Debit;
                        credit = credit + item.Credit;
                        item.VoucherId = voucherId;
                        if(item.AccountId!=0)
                            _dbContext.GLVoucherDetails.Add(item);
                    }
                    
                   // if (debit == credit && debit > 0)
                    if (debit == credit && debit != 0)
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
                    Console.WriteLine(Exp);
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
        public int GetNewVoucherNo(string voucherType, int periodId)
        {
            int returnVal = 1;
            var voucher = _dbContext.GLVouchers
                            .Where(v => v.CompanyId == _companyId &&
                            v.IsDeleted == false &&
                            v.VoucherType == voucherType &&
                            v.PeriodId == periodId).Max(v => (int?)v.VoucherNo);
            if (voucher != null)
            {
                returnVal = voucher.Value + 1;
            }

            return returnVal;
        }



    }
}

