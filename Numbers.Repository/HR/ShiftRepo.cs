using Microsoft.AspNetCore.Http;
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
    public class ShiftRepo: SessionBase
    {
        public static IEnumerable<HRShift> GetAll()
        {
            IEnumerable<HRShift> shifts = _dbContext.HRShifts.Where(a=>a.IsDeleted==false && a.CompanyId==_companyId).ToList();
            return shifts;
        }
        public static HRShift GetById(int id)
        {
            HRShift employee = _dbContext.HRShifts.Find(id);
            return employee;
        }
        
        public static async Task<bool> Create(HRShift model)
        {
            try
            {
                var shift = new HRShift();
                shift.Name = model.Name;
                shift.ShortName = model.ShortName;
                shift.Type = model.Type;
                shift.WorkingHours = model.WorkingHours;
                shift.MarkAbsentBefore = model.MarkAbsentBefore;
                shift.MarkShortLeave = model.MarkShortLeave;
                shift.MarkHalfDayShort = model.MarkHalfDayShort;
                shift.StartTime = model.StartTime;
                shift.EndTime = model.EndTime;
                shift.MarkLateAfter = model.MarkLateAfter;
                shift.MarkHalfDayBefore = model.MarkHalfDayBefore;
                shift.BreakStartTime = model.BreakStartTime;
                shift.BreakEndTime = model.BreakEndTime;
                shift.IsActive = model.IsActive;
                shift.CompanyId = _companyId;
                shift.IsDeleted = false;
                shift.CreatedBy = _userId;
                shift.CreatedDate = DateTime.Now;
                _dbContext.HRShifts.Add(shift);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                string msg = ex.Message.ToString();
                return false;
            }
        }
       
        [HttpPost]
        public static async Task<bool> Update(HRShift shift)
        {
            var obj = _dbContext.HRShifts.Find(shift.Id);
            obj.Name = shift.Name;
            obj.ShortName = shift.ShortName;
            obj.Type = shift.Type;
            obj.WorkingHours = shift.WorkingHours;
            obj.MarkAbsentBefore = shift.MarkAbsentBefore;
            obj.MarkShortLeave = shift.MarkShortLeave;
            obj.MarkHalfDayShort = shift.MarkHalfDayShort;
            obj.StartTime = shift.StartTime;
            obj.EndTime = shift.EndTime;
            obj.MarkLateAfter = shift.MarkLateAfter;
            obj.MarkHalfDayBefore = shift.MarkHalfDayBefore;
            obj.BreakStartTime = shift.BreakStartTime;
            obj.BreakEndTime = shift.BreakEndTime;
            obj.IsActive = shift.IsActive;
            obj.CompanyId = _companyId;
            obj.UpdatedBy = _userId;
            obj.UpdatedDate = DateTime.Now;
            var entry=_dbContext.HRShifts.Update(obj);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public static async Task<bool> Delete(int id)
        {
            var shiftDelete = _dbContext.HRShifts.Find(id);
            shiftDelete.IsDeleted = true;
            var entry = _dbContext.HRShifts.Update(shiftDelete);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
