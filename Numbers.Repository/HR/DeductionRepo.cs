using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Repository.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numbers.Repository.HR
{ 
   public class DeductionRepo:SessionBase
    {
        public static IEnumerable<HRDeduction> GetAll()
        {
            IEnumerable<HRDeduction> list = _dbContext.HRDeductions.Where(e => e.IsDeleted == false && e.CompanyId == _companyId).ToList();
            return list;
        }

        public static HRDeductionEmployee[] GetDeductionEmployees(int id)
        {
            HRDeductionEmployee[] deductions = _dbContext.HRDeductionEmployees.Where(i => i.Id == id && i.IsDeleted == false).ToArray();
            return deductions;
        }

        public static HRDeductionViewModel GetById(int id)
        {
            HRDeduction deduction = _dbContext.HRDeductions.Find(id);
            var viewModel = new HRDeductionViewModel();
            viewModel.DeductionNo = deduction.DeductionNo;
            viewModel.Date = deduction.Date;
            viewModel.Type = deduction.Type;
            viewModel.EmployeeType = deduction.EmployeeType;
            viewModel.Remarks = deduction.Remarks;
            viewModel.OrgId = deduction.OrgId;
            return viewModel;
        }

        [HttpPost]
        public static async Task<bool> Create(HRDeductionViewModel model, IFormCollection collection)
        {
            try
            {
                HRDeduction deduction = new HRDeduction();
                deduction.EmployeeType = model.EmployeeType;
                deduction.Type = model.Type;
                deduction.OrgId = model.OrgId;
                deduction.CreatedBy = _userId;
                deduction.CreatedDate = DateTime.Now;
                deduction.Status = "Created";
                deduction.CompanyId = _companyId;
                deduction.IsDeleted = false;
                _dbContext.HRDeductions.Add(deduction);
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
        public static async Task<bool> Update(HRDeductionViewModel model, IFormCollection collection)
        {
            HRDeduction deduction = _dbContext.HRDeductions.Find(model.Id);
            deduction.DeductionNo = model.DeductionNo;
            deduction.Date = model.Date;
            deduction.Type = model.Type;
            deduction.EmployeeTypeId = model.EmployeeTypeId;
            deduction.OrgId = model.OrgId;
            deduction.EmployeeType = model.EmployeeType;
            deduction.Remarks = model.Remarks;
            deduction.UpdatedBy = _userId;
            deduction.UpdatedDate = DateTime.Now;
            var entry = _dbContext.HRDeductions.Update(deduction);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public static async Task<bool> Delete(int id)
        {
            var deduction = _dbContext.HRDeductions.Find(id);
            deduction.IsDeleted =true;
            var entry = _dbContext.HRDeductions.Update(deduction);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public static int GetDeductionNo(int id)
        {
            int maxDeductionNo = 1;
            var deductions = _dbContext.HRDeductions.Where(r => r.CompanyId == _companyId).ToList();
            if (deductions.Count > 0)
            {
                maxDeductionNo = deductions.Max(r => r.DeductionNo);
                return maxDeductionNo + 1;
            }
            else
            {
                return maxDeductionNo;
            }
        }

        public static HRDeductionViewModel GetEmployeeDeduction(int id, int itemId)
        {
            var item = _dbContext.HRDeductionEmployees.Include(i => i.DeductionId).Where(i => i.Id == id && i.IsDeleted != true).FirstOrDefault();
            HRDeductionViewModel viewModel = new HRDeductionViewModel();
            viewModel.DeductionId = item.Id;
            viewModel.EmployeeId = item.EmployeeId;
            viewModel.Remarks = item.Remarks;
            viewModel.Amount = item.Amount;
            return viewModel;
        }
        public static List<HREmployeeType> GetEmployeeTypes()
        {
            List<HREmployeeType> list = new List<HREmployeeType>();
            list = _dbContext.HREmployeeTypes.Where(e => e.IsDeleted == false && e.CompanyId == _companyId).ToList();
            return list;
        }
    }
}

