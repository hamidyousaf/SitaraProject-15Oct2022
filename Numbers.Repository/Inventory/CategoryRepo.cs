using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using Numbers.Repository.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numbers.Repository.Inventory
{
    public class CategoryRepo
    {
        private readonly NumbersDbContext _dbContext;
        public CategoryRepo(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<InvItemCategories> GetAll(int companyId)
        {
            IEnumerable<InvItemCategories> listRepo= _dbContext.InvItemCategories.Include(c => c.Parent)
           .Where(v => v.IsDeleted == false && v.CompanyId == companyId).ToList();
            return listRepo;
        }

        public InvItemCategories GetById(int id)
        {
            var category = _dbContext.InvItemCategories.Find(id);
            return category;
        }

        [HttpPost]
        public async Task<bool> Create(InvItemCategories model)
        {
            try
            {
                var category = new InvItemCategories();
                if (model.ParentId !=0 && model.ParentId !=null)
                {
                    category.ParentId = model.ParentId;
                    category.Status = "Child";
                }
                else
                {
                    category.ParentId = null;
                    category.Status = "Parent";
                }
                category.Name = model.Name;
                category.Code = model.ParentCode+model.Code;
                category.CategoryLevel = model.CategoryLevel;
                category.IsActive = true;
                category.IsDeleted = false;
                category.CompanyId = model.CompanyId;
                category.CreatedBy = model.CreatedBy;
                category.CreatedDate = DateTime.Now;
                _dbContext.InvItemCategories.Add(category);
                await _dbContext.SaveChangesAsync();
                return true;
            }catch(Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                string message = ex.Message.ToString();
                return false;
            }
        }
        [HttpPost]
        public async Task<bool> Update(InvItemCategories model)
        {
            try
            {
                var obj = _dbContext.InvItemCategories.Find(model.Id);
                obj.Name = model.Name;
                if (model.ParentId != 0) { obj.ParentId = model.ParentId; }
                else { obj.ParentId = null; }
                obj.CategoryLevel = model.CategoryLevel;
                obj.Code = model.ParentCode + model.Code;
                obj.IsActive = true;
                obj.UpdatedBy = model.UpdatedBy;
                obj.CompanyId = model.CompanyId;
                obj.UpdatedDate = DateTime.Now;
                var entry = _dbContext.InvItemCategories.Update(obj);
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

        public async Task<bool> Delete(int id)
        {
            var deleteCategory = _dbContext.InvItemCategories.Find(id);
            deleteCategory.IsDeleted = true;
            var entry = _dbContext.InvItemCategories.Update(deleteCategory);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
