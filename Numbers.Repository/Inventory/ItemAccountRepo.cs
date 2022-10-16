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
    public class ItemAccountRepo
    {
        private readonly NumbersDbContext _dbContext;
        public ItemAccountRepo(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<InvItemAccount> GetAll(int companyId)
        {
            IEnumerable<InvItemAccount> listRepo = _dbContext.InvItemAccounts.Include(v=>v.GLAssetAccount)
                .Include(v=>v.GLSaleAccount).Include(v=>v.GLCostofSaleAccount).Where(v => v.IsDeleted == false && v.CompanyId == companyId)
                .ToList();
            return listRepo;
        }

        public InvItemAccount GetById(int id)
        {
            var itemAccount = _dbContext.InvItemAccounts.Find(id);
            return itemAccount;
        }

        [HttpPost]
        public async Task<bool> Create(InvItemAccount model)
        {
            try
            {
                var itemAccount = new InvItemAccount();
                itemAccount.Name = model.Name;
                itemAccount.GLAssetAccountId = model.GLAssetAccountId;
                itemAccount.GLSaleAccountId = model.GLSaleAccountId;
                itemAccount.GLCostofSaleAccountId = model.GLCostofSaleAccountId;
                itemAccount.GLWIPAccountId = model.GLWIPAccountId;
                itemAccount.IsActive = model.IsActive;
                itemAccount.IsDeleted = false;
                itemAccount.CompanyId = model.CompanyId;
                itemAccount.CreatedBy = model.CreatedBy;
                itemAccount.CreatedDate = DateTime.Now;
                _dbContext.InvItemAccounts.Add(itemAccount);
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
        public async Task<bool> Update(InvItemAccount model)
        {
            var obj = _dbContext.InvItemAccounts.Find(model.Id);
            obj.Name = model.Name;
            obj.GLAssetAccountId = model.GLAssetAccountId;
            obj.GLSaleAccountId = model.GLSaleAccountId;
            obj.GLCostofSaleAccountId = model.GLCostofSaleAccountId;
            obj.GLWIPAccountId = model.GLWIPAccountId;
            obj.IsActive = model.IsActive;
            obj.CompanyId = model.CompanyId;
            obj.UpdatedBy = model.UpdatedBy;
            obj.UpdatedDate = DateTime.Now;
            var entry=_dbContext.InvItemAccounts.Update(obj);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(int id)
        {
            var deleteAccount= _dbContext.InvItemAccounts.Find(id);
            deleteAccount.IsDeleted = true;
            var entry = _dbContext.InvItemAccounts.Update(deleteAccount);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
