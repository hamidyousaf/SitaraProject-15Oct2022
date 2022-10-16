using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Numbers.Entity.Models;
using System.Threading.Tasks;
using Numbers.Repository.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Numbers.Repository.HR
{
    public class LeaveGroupRepo:SessionBase
    {
        public static IEnumerable<HRLeaveTypeGroup> GetAll()
        {
            IEnumerable<HRLeaveTypeGroup> leaveGroup = _dbContext.HRLeaveTypeGroups.Include(a=>a.LeaveType).Where(a => a.IsDeleted == false && a.CompanyId ==_companyId).ToList();
            return leaveGroup;
        }

        public static HRLeaveTypeGroup GetById(int id)
        {
            HRLeaveTypeGroup listLeaveGroup = _dbContext.HRLeaveTypeGroups.Find(id);
            return listLeaveGroup;
        }

        [HttpPost]
        public static async Task<bool> Create(HRLeaveTypeGroup modelRepo)
        {
            try
            {
                var typeGroup = new HRLeaveTypeGroup();
                typeGroup.EmployeeTypeId = modelRepo.EmployeeTypeId;
                typeGroup.LeaveTypeId = modelRepo.LeaveTypeId;
                typeGroup.NoOfDays = modelRepo.NoOfDays;
                typeGroup.Forwardable = modelRepo.Forwardable;
                typeGroup.IsActive = modelRepo.IsActive;
                typeGroup.CompanyId = _companyId;
                typeGroup.CreatedBy = _userId;
                typeGroup.CreatedDate = DateTime.Now;
                typeGroup.IsDeleted = false;
                _dbContext.HRLeaveTypeGroups.Add(typeGroup);
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

        [HttpPost]
        public static async Task<bool> Update(HRLeaveTypeGroup modelRepo)
        {
            var obj = _dbContext.HRLeaveTypeGroups.Find(modelRepo.Id);
            obj.EmployeeTypeId = modelRepo.EmployeeTypeId;
            obj.LeaveTypeId = modelRepo.LeaveTypeId;
            obj.NoOfDays = modelRepo.NoOfDays;
            obj.Forwardable = modelRepo.Forwardable;
            obj.IsActive = modelRepo.IsActive;
            obj.CompanyId = _companyId;
            obj.UpdatedBy = _userId;
            obj.UpdatedDate = DateTime.Now;
            var entry=_dbContext.HRLeaveTypeGroups.Update(obj);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public static async Task<bool> Delete(int id)
        {
            var typeGroupDelete = _dbContext.HRLeaveTypeGroups.Find(id);
            typeGroupDelete.IsDeleted = true;
            var entry = _dbContext.HRLeaveTypeGroups.Update(typeGroupDelete);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
