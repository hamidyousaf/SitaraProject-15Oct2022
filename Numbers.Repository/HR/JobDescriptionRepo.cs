using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Numbers.Entity.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Repository.HR
{
    public class JobDescriptionRepo
    {
        private readonly NumbersDbContext _dbContext;
        public JobDescriptionRepo(NumbersDbContext context)
        {
            _dbContext = context;
        }

        public IEnumerable<HRJobDescription> GetAll()
        {
            IEnumerable<HRJobDescription> jobDescription = _dbContext.HRJobDescriptions.Where(a => a.IsDeleted == false).ToList();
            return jobDescription;
        }

        public HRJobDescription GetById(int id)
        {
            HRJobDescription description = _dbContext.HRJobDescriptions.Find(id);
            return description;
        }

        public async Task<bool> Create(HRJobDescription modelRepo)
        {
            try
            {
                _dbContext.HRJobDescriptions.Add(modelRepo);
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
        public async Task<bool> Update(HRJobDescription modelRepo)
        {
            var obj = _dbContext.HRJobDescriptions.Find(modelRepo.Id);
            obj.UpdatedBy = modelRepo.UpdatedBy;
            obj.UpdatedDate = DateTime.Now;
            obj.Description = modelRepo.Description;
            obj.IsActive = modelRepo.IsActive;
            _dbContext.HRJobDescriptions.Update(obj);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(int id)
        {
            var descriptionDelete = _dbContext.HRJobDescriptions.Find(id);
            descriptionDelete.IsDeleted = true;
            var entry = _dbContext.HRJobDescriptions.Update(descriptionDelete);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
