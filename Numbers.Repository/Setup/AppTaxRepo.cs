using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Repository.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numbers.Repository.Setup
{
    public class AppTaxRepo
    {
        private readonly NumbersDbContext _dbContext;
        public AppTaxRepo(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<AppTax> GetAll(int companyId)
        {
            IEnumerable<AppTax> listRepo = _dbContext.AppTaxes.Include(t=>t.SalesTaxAccount)
                .Include(t => t.ExciseTaxAccount).Include(t => t.IncomeTaxAccount).Where(t => /*t.CompanyId == companyId && */t.IsDeleted == false).ToList();
            return listRepo;
        }

        public AppTax GetById(int id)
        {
            var tax = _dbContext.AppTaxes.Find(id);
            return tax;
        }

        [HttpPost]
        public async Task<bool> Create(AppTax model)
        {
            try
            {
                AppTax tax = new AppTax();
                tax.Name = model.Name;
                tax.SalesTaxPercentage = model.SalesTaxPercentage;
                tax.SalesTaxAccountId = model.SalesTaxAccountId;
                tax.ExciseTaxPercentage = model.ExciseTaxPercentage;
                tax.ExciseTaxAccountId = model.ExciseTaxAccountId;
                tax.IncomeTaxPercentage = model.IncomeTaxPercentage;
                tax.IncomeTaxAccountId = model.IncomeTaxAccountId;
                tax.Description = model.Description;
                tax.IsActive = model.IsActive;
                tax.IsDeleted = false;
                tax.CompanyId = model.CompanyId;
                tax.CreatedBy = model.CreatedBy;
                tax.CreatedDate = DateTime.Now;
                _dbContext.AppTaxes.Add(tax);
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
        public async Task<bool> Update(AppTax model)
        {
            var obj = _dbContext.AppTaxes.Find(model.Id);
            obj.Name = model.Name;
            obj.SalesTaxPercentage = model.SalesTaxPercentage;
            obj.SalesTaxAccountId = model.SalesTaxAccountId;
            obj.ExciseTaxPercentage = model.ExciseTaxPercentage;
            obj.ExciseTaxAccountId = model.ExciseTaxAccountId;
            obj.IncomeTaxPercentage = model.IncomeTaxPercentage;
            obj.IncomeTaxAccountId = model.IncomeTaxAccountId;
            obj.Description = model.Description;
            obj.IsActive = model.IsActive;
            obj.IsDeleted = model.IsDeleted;
            obj.CompanyId = model.CompanyId;
            obj.UpdatedBy = model.UpdatedBy;
            obj.UpdatedDate = DateTime.Now;
            var entry = _dbContext.AppTaxes.Update(obj);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(int id)
        {
            var deleteTax = _dbContext.AppTaxes.Find(id);
            deleteTax.IsDeleted = true;
            var entry = _dbContext.AppTaxes.Update(deleteTax);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return true;
        }
        public List <AppTax> GetTaxes(int companyId)
        {
            List<AppTax> list =new List<AppTax>();
            list = _dbContext.AppTaxes.Include(t => t.SalesTaxAccount)
                .Include(t => t.ExciseTaxAccount).Include(t => t.IncomeTaxAccount).Where(t => t.CompanyId == companyId && t.IsDeleted == false).OrderByDescending(x => x.Name.Contains("NO TAX")).ToList();
            //list = _dbContext.AppTaxes.Where(t=>t.IsActive && !t.IsDeleted && t.CompanyId==companyId).ToList();
            return list;
        }
        public List<AppTax> GetTaxesById(int companyId, int id)
        {
            List<AppTax> list = new List<AppTax>();
            list = _dbContext.AppTaxes.Where(x=>x.Id == id).Include(t => t.SalesTaxAccount)
                .Include(t => t.ExciseTaxAccount).Include(t => t.IncomeTaxAccount).Where(t => t.CompanyId == companyId && t.IsDeleted == false).OrderByDescending(x => x.Name.Contains("NO TAX")).ToList();
            //list = _dbContext.AppTaxes.Where(t=>t.IsActive && !t.IsDeleted && t.CompanyId==companyId).ToList();
            return list;
        }
    }
}
