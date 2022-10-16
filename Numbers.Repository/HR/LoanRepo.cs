using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Numbers.Entity.Models;
using Numbers.Repository.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Repository.HR
{
    public class LoanRepo : SessionBase
    {
        public static IEnumerable<HRLoan> GetAll()
        {
            IEnumerable<HRLoan> list = _dbContext.HRLoans.Where(e =>e.IsDeleted==false && e.CompanyId==_companyId).ToList();
            return list;
        }
        public static HRLoan GetById(int id)
        {
            HRLoan loan= _dbContext.HRLoans.Find(id);
            return loan;
        }
       
        [HttpPost]
        public static async Task<bool> Create(HRLoan model, IFormCollection collection)
        {
            try
            {
                HRLoan loan = new HRLoan();
                model.BankCashAccountId = loan.BankCashAccountId;
                model.CreatedBy = _userId;
                model.CreatedDate = DateTime.Now;
                model.Status = "Created";
                model.CompanyId = _companyId;
                model.IsDeleted = false;
                _dbContext.HRLoans.Add(model);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                string msg = ex.InnerException.Message.ToString();
                return false;
            }
        }
        [HttpPost]
        public static async Task<bool> Update(HRLoan model, IFormCollection collection)
        {
            HRLoan loan = _dbContext.HRLoans.Find(model.Id);
            loan.LoanNo = model.LoanNo;
            loan.LoanDate = model.LoanDate;
            loan.EmployeeId = model.EmployeeId;
            loan.ExemptionAccount = model.ExemptionAccount;
            loan.ExemptionAmount = model.ExemptionAmount;
            loan.ExemptionRemarks = model.ExemptionRemarks;
            loan.Remarks = model.Remarks;
            loan.MarkUpPercentage = model.MarkUpPercentage;
            loan.NoOfInstallment = model.NoOfInstallment;
            loan.PerMonthInstalment = model.PerMonthInstalment;
            loan.LoanAmount = model.LoanAmount;
            loan.AccountingDate = model.AccountingDate;
            loan.StartDate = model.StartDate;
            loan.DeductionType = model.DeductionType;
            loan.ChequeNo = model.ChequeNo;
            loan.BankCashAccountId = model.BankCashAccountId;
            loan.UpdatedBy = _userId;
            loan.UpdatedDate = DateTime.Now;
            var entry=_dbContext.HRLoans.Update(loan);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            //HREmployeeFamily[] familyMembers = JsonConvert.DeserializeObject<HREmployeeFamily[]>(collection["familyDetail"]);
            //foreach (var familymember in familyMembers)
            //{
            //    var id = familymember.Id;
            //    if (id == 0)
            //    {
            //        HREmployeeFamily member = new HREmployeeFamily();
            //        member = familymember;
            //        member.EmployeeId = loan.Id;
            //        member.CreatedBy = _userId;
            //        _dbContext.HREmployeeFamilies.Add(member);
            //        _dbContext.SaveChanges();
            //    }
            //    else
            //    {
            //        HREmployeeFamily member = _dbContext.HREmployeeFamilies.Where(f => f.Id == id).FirstOrDefault();
            //        member = familymember;
            //        member.UpdatedBy = _userId;
            //        ((DbContext)_dbContext).ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            //        _dbContext.HREmployeeFamilies.Update(member);
            //        var tracker = _dbContext.HREmployeeFamilies.Update(member);
            //        tracker.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            //        _dbContext.SaveChanges();
            //    }
            //}

            return true;

        }

        public static async Task<bool> Delete(int id)
        {
            var loan = _dbContext.HRLoans.Find(id);
            loan.IsDeleted = true;
            var entry = _dbContext.HRLoans.Update(loan);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public static int GetLoanNo(int id)
        {
            int maxLoanNo = 1;
            var loans = _dbContext.HRLoans.Where(r => r.CompanyId == _companyId).ToList();
            if (loans.Count > 0)
            {
                maxLoanNo = loans.Max(r => r.LoanNo);
                return maxLoanNo + 1;
            }
            else
            {
                return maxLoanNo;
            }

        }
        public static SelectList GetAccounts()
        {
            var accounts = new SelectList(_dbContext.GLBankCashAccounts.Where(b => b.CompanyId == _companyId && b.IsActive == true).ToList(), "Id", "AccountName");
            return accounts;
        }
        
    }
}
