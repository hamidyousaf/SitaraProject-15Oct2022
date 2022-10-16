using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Repository.HR
{
    public class EmployeeLeaveOpeningRepo
    {
        private readonly NumbersDbContext _dbContext;
        public EmployeeLeaveOpeningRepo(NumbersDbContext context)
        {
            _dbContext = context;
        }

        public IEnumerable<HREmployeeLeaveOpening> GetAll()
        {
            IEnumerable<HREmployeeLeaveOpening> listRepo = _dbContext.HREmployeeLeaveOpenings.Where(l=>l.IsDeleted==false).ToList();
            return listRepo;
        }

        public HREmployeeLeaveOpening GetById(int id)
        {
            HREmployeeLeaveOpening leaveById = _dbContext.HREmployeeLeaveOpenings.Find(id);
            return leaveById;
        }

        public async Task<bool> Create(HREmployeeLeaveOpening modelRepo)
        {
            try
            {
                _dbContext.HREmployeeLeaveOpenings.Add(modelRepo);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                string message = ex.Message.ToString();
                return false;
            }
        }

        [HttpPost]
        public async Task<bool> Update(HREmployeeLeaveOpening modelRepo)
        {
            var obj = _dbContext.HREmployeeLeaveOpenings.Find(modelRepo.Id);
            obj.UpdatedBy = modelRepo.UpdatedBy;
            obj.UpdatedDate = DateTime.Now;
            obj.OpeningDate = modelRepo.OpeningDate;
            obj.Status = modelRepo.Status;
            obj.OpeningBalance = modelRepo.OpeningBalance;
            obj.No = modelRepo.No;
            obj.LeaveTypeId = modelRepo.LeaveTypeId;
            obj.IsActive = modelRepo.IsActive;
            obj.EmployeeId = modelRepo.EmployeeId;
            obj.ApprovedBy = modelRepo.ApprovedBy;
            obj.ApprovedDate = modelRepo.ApprovedDate;
            _dbContext.HREmployeeLeaveOpenings.Add(obj);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(int id)
        {
            var deleteRepo=_dbContext.HREmployeeLeaveOpenings.Find(id);
            deleteRepo.IsDeleted = true;
            var entry = _dbContext.HREmployeeLeaveOpenings.Update(deleteRepo);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
