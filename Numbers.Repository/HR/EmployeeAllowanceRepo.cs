using Microsoft.AspNetCore.Mvc;
using Numbers.Entity.Models;
using Numbers.Repository.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Numbers.Repository.HR
{
    public class EmployeeAllowanceRepo: SessionBase
    {
        public static IEnumerable<HREmployeeAllowance> GetAll()
        {
            IEnumerable<HREmployeeAllowance> listRepo = _dbContext.HREmployeeAllowances.Where(l => l.IsDeleted == false && l.CompanyId == _companyId).ToList();
            return listRepo;
        }
        public static HREmployeeAllowance GetById(int id)
        {
            HREmployeeAllowance allowance = _dbContext.HREmployeeAllowances.Find(id);
            return allowance;
        }

        [HttpPost]
        public static async Task<bool> Create(HREmployeeAllowance model)
        {
            try
            {
                var Allowance = new HREmployeeAllowance();
                Allowance.Name = model.Name;
                Allowance.IncrementPercentAge = model.IncrementPercentAge;
                Allowance.AccountDescription = model.AccountDescription;
                Allowance.IsActive = model.IsActive;
                Allowance.CompanyId = _companyId;
                Allowance.CreatedBy = _userId;
                Allowance.CreatedDate = DateTime.Now;
                Allowance.IsDeleted = false;
                _dbContext.HREmployeeAllowances.Add(Allowance);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                string message = ex.Message.ToString();
                return false;
            }
        }

        public static async Task<bool> Update(HREmployeeAllowance model)
        {
            var obj = _dbContext.HREmployeeAllowances.Find(model.Id);
            obj.Name = model.Name;
            obj.IncrementPercentAge = model.IncrementPercentAge;
            obj.AccountDescription = model.AccountDescription;
            obj.IsActive = model.IsActive;
            obj.CompanyId = _companyId;
            obj.UpdatedBy = _userId;
            obj.UpdatedDate = DateTime.Now;
            var entry = _dbContext.HREmployeeAllowances.Update(obj);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public static async Task<bool> Delete(int id)
        {
            var allowanceDelete = _dbContext.HREmployeeAllowances.Find(id);
            allowanceDelete.IsDeleted = true;
            var entry = _dbContext.HREmployeeAllowances.Update(allowanceDelete);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
