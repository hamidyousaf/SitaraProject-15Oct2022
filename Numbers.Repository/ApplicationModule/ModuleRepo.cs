using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numbers.Repository.AppModule
{
    public class ModuleRepo
    {
        private readonly NumbersDbContext _dbContext;

        public ModuleRepo(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        [HttpPost]
        public async Task<bool> Create(AppCompanyModule appCompanyModule, IFormCollection collection)
        {
            try
            {
                AppCompanyModule Appmodule = new AppCompanyModule();
              //  Appmodule.ModId = appCompanyModule.ModId;
                Appmodule.Module_Name = appCompanyModule.Module_Name;
                Appmodule.Module_Description = appCompanyModule.Module_Description;
                Appmodule.Short_Name = appCompanyModule.Short_Name;

                Appmodule.CreatedBy = appCompanyModule.CreatedBy;
                Appmodule.CreatedDate = appCompanyModule.CreatedDate;
                Appmodule.CompanyId = appCompanyModule.CompanyId;
                Appmodule.Is_Active = appCompanyModule.Is_Active;
                _dbContext.AppCompanyModules.Add(Appmodule);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                string message = ex.Message.ToString();
                return true;
            }         
        }


        [HttpPost]
        public async Task<bool> Update(AppCompanyModule appCompanyModule, IFormCollection collection)
        {

            var data = _dbContext.AppCompanyModules.Find(appCompanyModule.Id);
            
            //data.ModId = appCompanyModule.ModId;
            data.Module_Name = appCompanyModule.Module_Name;
            data.Module_Description = appCompanyModule.Module_Description;
            data.Short_Name = appCompanyModule.Short_Name;

            data.UpdatedBy = appCompanyModule.UpdatedBy;
            data.UpdatedDate = DateTime.Now;
            data.CompanyId = appCompanyModule.CompanyId;
            data.Is_Active = appCompanyModule.Is_Active;

            var entry = _dbContext.AppCompanyModules.Update(data);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());

            //_dbContext.AppCompanyModules.Update(data);

            await _dbContext.SaveChangesAsync();

                return true;
        }



        public async Task<bool> Delete(int Id)
        {
            var module = await _dbContext.AppCompanyModules.Where(n => n.Id == Id).FirstAsync();
            if (module != null)
            {
                module.Is_Active = false;
                _dbContext.AppCompanyModules.Update(module);
                await _dbContext.SaveChangesAsync();
            }
            return true;
        }
    }
}
