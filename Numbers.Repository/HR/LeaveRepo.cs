using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Numbers.Entity.Models;
using System.Linq;
using System.Threading.Tasks;
using Numbers.Repository.Helpers;

namespace Numbers.Repository.HR
{
    public class LeaveRepo:SessionBase
    {
        public static IEnumerable<HRLeave> GetAll()
        {
            IEnumerable<HRLeave> leaveList = _dbContext.HRLeaves.Where(l => l.IsDeleted == false && l.CompanyId==_companyId).ToList();
            return leaveList;
        }

        public static HRLeave GetById(int id)
        {
            HRLeave leave = _dbContext.HRLeaves.Find(id);
            return leave;
        }

        [HttpPost]
        public static async Task<bool> Create(HRLeave model)
        {
            try
            {
                var leave = new HRLeave();
                leave.Description = model.Description;
                leave.ShortDescription = model.ShortDescription;
                leave.Unit = model.Unit;
                leave.Flag = model.Flag;
                leave.Weight = model.Weight;
                leave.LeaveTypeId = model.LeaveTypeId;
                leave.IsActive = model.IsActive;
                leave.CompanyId = _companyId;
                leave.CreatedBy = _userId;
                leave.CreatedDate = DateTime.Now;
                leave.IsDeleted = false;
                _dbContext.HRLeaves.Add(leave);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                string message= ex.Message.ToString();
                return false;
            }
        }

        [HttpPost]
        public static async Task<bool> Update(HRLeave model)
        {
            var obj = _dbContext.HRLeaves.Find(model.Id);
            obj.Description = model.Description;
            obj.ShortDescription = model.ShortDescription;
            obj.Unit = model.Unit;
            obj.Flag = model.Flag;
            obj.Weight = model.Weight;
            obj.LeaveTypeId = model.LeaveTypeId;
            obj.IsActive = model.IsActive;
            obj.CompanyId = _companyId;
            obj.UpdatedBy = _userId;
            obj.UpdatedDate = DateTime.Now;
            var entry=_dbContext.HRLeaves.Update(obj);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public static async Task<bool> Delete(int id)
        {
            var leaveDelete = _dbContext.HRLeaves.Find(id);
            leaveDelete.IsDeleted = true;
            var entry = _dbContext.HRLeaves.Update(leaveDelete);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
