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
    public class LeaveTypeRepo: SessionBase
    {

        public static IEnumerable<HRLeaveType> GetAll()
        {
            IEnumerable<HRLeaveType> leaveType = _dbContext.HRLeaveTypes.Where(a => a.IsDeleted == false && a.CompanyId==_companyId).ToList();
            return leaveType;
        }

        public static HRLeaveType GetById(int id)
        {
            HRLeaveType leaveById = _dbContext.HRLeaveTypes.Find(id);
            return leaveById;
        }

        public static async Task<bool> Create(HRLeaveType model)
        {
            try
            {
                model.CreatedBy = _userId;
                model.CreatedDate = DateTime.Now;
                model.IsDeleted = false;
                model.CompanyId = _companyId;
                _dbContext.HRLeaveTypes.Add(model);
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
        public static async Task<bool> Update(HRLeaveType model)
        {
            var leave = _dbContext.HRLeaveTypes.Find(model.Id);
            leave.UpdatedBy = _userId;
            leave.UpdatedDate = DateTime.Now;
            leave.Description = model.Description;
            leave.ShortDescription = model.ShortDescription;
            leave.Unit = model.Unit;
            leave.IsActive = model.IsActive;
            var entry=_dbContext.HRLeaveTypes.Update(leave);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public static async Task<bool> Delete(int id)
        {
            var leaveTypeDelete = _dbContext.HRLeaveTypes.Find(id);
            leaveTypeDelete.IsDeleted = true;
            var entry = _dbContext.HRLeaveTypes.Update(leaveTypeDelete);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
