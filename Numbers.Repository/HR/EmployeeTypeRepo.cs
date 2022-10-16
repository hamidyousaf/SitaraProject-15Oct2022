using Microsoft.AspNetCore.Mvc;
using Numbers.Entity.Models;
using Numbers.Repository.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numbers.Repository.HR
{
    public class EmployeeTypeRepo : SessionBase
    {
        public static IEnumerable<HREmployeeType> GetAll()
        {
            IEnumerable<HREmployeeType> listRepo = _dbContext.HREmployeeTypes.Where(t => t.IsDeleted == false && t.CompanyId == _companyId).ToList();
            return listRepo;
        }
        public static HREmployeeType GetById(int id)
        {
            HREmployeeType type = _dbContext.HREmployeeTypes.Find(id);
            return type;
        }

        [HttpPost]
        public static async Task<bool> Create(HREmployeeType model)
        {
            try
            {
                var Type = new HREmployeeType();
                Type.AdvanceSalaryAccountId = model.AdvanceSalaryAccountId;
                Type.Name = model.Name;
                Type.ShortDescription = model.ShortDescription;
                Type.PayrollProcess = model.PayrollProcess;
                Type.Description = model.Description;
                Type.IsActive = model.IsActive;
                Type.CompanyId = _companyId;
                Type.CreatedBy = _userId;
                Type.CreatedDate = DateTime.Now;
                Type.IsDeleted = false;
                _dbContext.HREmployeeTypes.Add(Type);
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

        public static async Task<bool> Update(HREmployeeType model)
        {
            var obj = _dbContext.HREmployeeTypes.Find(model.Id);
            obj.AdvanceSalaryAccountId = model.AdvanceSalaryAccountId;
            obj.Name = model.Name;
            obj.ShortDescription = model.ShortDescription;
            obj.PayrollProcess = model.PayrollProcess;
            obj.Description = model.Description;
            obj.IsActive = model.IsActive;
            obj.CompanyId = _companyId;
            obj.UpdatedBy = _userId;
            obj.UpdatedDate = DateTime.Now;
            var entry = _dbContext.HREmployeeTypes.Update(obj);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public static async Task<bool> Delete(int id)
        {
            var typeDelete = _dbContext.HREmployeeTypes.Find(id);
            typeDelete.IsDeleted = true;
            var entry = _dbContext.HREmployeeTypes.Update(typeDelete);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
