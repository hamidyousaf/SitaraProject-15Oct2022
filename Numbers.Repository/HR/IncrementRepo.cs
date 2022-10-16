using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
   public class IncrementRepo:SessionBase
    {
        public static IEnumerable<HRIncrement> GetAll()
        {
            IEnumerable<HRIncrement> list = _dbContext.HRIncrements.Where(e => e.IsDeleted == false && e.CompanyId ==_companyId).ToList();
            return list;
        }

        public static HRIncrementEmployee[] GetIncrementEmployees(int id)
        {
            HRIncrementEmployee[] increments = _dbContext.HRIncrementEmployees.Where(i => i.Id == id && i.IsDeleted == false).ToArray();
            return increments;
        }

        public static HRIncrementViewModel GetById(int id)
        {
            HRIncrement increment = _dbContext.HRIncrements.Find(id);
            var viewModel = new HRIncrementViewModel();
            viewModel.IncrementNo = increment.IncrementNo;
            viewModel.IncrementDate = increment.IncrementDate;
            viewModel.IncrementType = increment.IncrementType;
            viewModel.EmployeeType = increment.EmployeeType;
            viewModel.Remarks = increment.Remarks;
            viewModel.Designation = increment.Designation;
            return viewModel;
        }

        [HttpPost]
        public static async Task<bool> Create(HRIncrementViewModel model, IFormCollection collection)
        {
            try
            {
                HRIncrement increment = new HRIncrement();
                increment.EmployeeType = model.EmployeeType;
                increment.IncrementType = model.IncrementType;
                increment.Designation = model.Designation;
                increment.CreatedBy = _userId;
                increment.CreatedDate = DateTime.Now;
                increment.Status = "Created";
                increment.CompanyId = _companyId;
                increment.IsDeleted = false;
                _dbContext.HRIncrements.Add(increment);
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
        public static async Task<bool> Update(HRIncrementViewModel model, IFormCollection collection)
        {
            HRIncrement increment = _dbContext.HRIncrements.Find(model.Id);
            increment.IncrementNo = model.IncrementNo;
            increment.IncrementDate = model.IncrementDate;
            increment.IncrementType = model.IncrementType;
            increment.Designation = model.Designation;
            increment.Department = model.Department;
            increment.EmployeeType = model.EmployeeType;
            increment.Remarks = model.Remarks;
            increment.IncrementPercentage = model.IncrementPercentage;
            increment.UpdatedBy = _userId;
            increment.UpdatedDate = DateTime.Now;
            var entry = _dbContext.HRIncrements.Update(increment);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public static async Task<bool> Delete(int id)
        {
            var increment = _dbContext.HRIncrements.Find(id);
            increment.IsDeleted = true;
            var entry = _dbContext.HRIncrements.Update(increment);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public static int GetIncrementNo(int id)
        {
            int maxIncrementNo = 1;
            var increments = _dbContext.HRIncrements.Where(r => r.CompanyId == _companyId).ToList();
            if (increments.Count > 0)
            {
                maxIncrementNo = increments.Max(r => r.IncrementNo);
                return maxIncrementNo + 1;
            }
            else
            {
                return maxIncrementNo;
            }
        }

        public static HRIncrementViewModel GetEmployeeIncrements(int id, int itemId)
        {
            var item = _dbContext.HRIncrementEmployees.Include(i => i.Increment).Where(i => i.Id == id && i.IsDeleted != true).FirstOrDefault();
            HRIncrementViewModel viewModel = new HRIncrementViewModel();
            viewModel.IncrementItemId = item.Id;
            viewModel.EmployeeId = item.EmployeeId;
            viewModel.CurrentPay = item.CurrentPay;
            viewModel.OldDesignation = item.OldDesignation;
            viewModel.NewDesignation = item.NewDesignation;
            viewModel.OldGrade = item.OldGrade;
            viewModel.NewGrade = item.NewGrade;
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
