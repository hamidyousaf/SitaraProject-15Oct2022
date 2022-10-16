using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Numbers.Repository.Helpers
{
    public class ConfigValues
    {
        private readonly NumbersDbContext _dbContext;
        public ConfigValues(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public SelectList Supplier(int companyId)
        {
            var suppliers = new SelectList(_dbContext.APSuppliers.ToList().Where(s => s.IsActive /*&& s.CompanyId == companyId*/), "Id", "Name");
            return suppliers;
        }
        public SelectList CustomerType(int companyId)
        {
            var Customer = new SelectList(_dbContext.ARCustomers.ToList().Where(c => c.IsActive /*&& c.CompanyId == companyId*/), "Id", "Name");
            return Customer;
        }
        public SelectList GetConfigValues(string module, string configName, int companyId)
        {
            SelectList list = new SelectList((from b in _dbContext.AppCompanyConfigBases
                                              join c in _dbContext.AppCompanyConfigs on b.Id equals c.BaseId
                                              where /*c.CompanyId == companyId && b.CompanyId==companyId &&*/ c.IsActive && c.IsDeleted == false && b.Name == configName && b.Module == module
                                              select c
                                  ).ToList(), "Id", "ConfigValue");
           /* SelectList list = new SelectList((from b in _dbContext.AppCompanyConfigBases where b.Name == configName && b.Module == module select b).ToList(),"Id");
                                              *//*join c in _dbContext.AppCompanyConfigs on b.Id equals c.BaseId
                                              where c.CompanyId == companyId && c.IsActive && c.IsDeleted == false && b.Name == configName && b.Module == module
                                              select c
                                  ).ToList(), "Id", "ConfigValue");*/

            return list;
        }
        public SelectList GetConfigValues(string module, string configName,string ConfigValue1, int companyId)
        {
            SelectList list = new SelectList((from b in _dbContext.AppCompanyConfigBases
                                              join c in _dbContext.AppCompanyConfigs on b.Id equals c.BaseId
                                              where /*c.CompanyId == companyId && b.CompanyId==companyId &&*/ c.IsActive && c.IsDeleted == false && b.Name == configName && c.UserValue1==ConfigValue1 && b.Module == module
                                              select c
                                  ).ToList(), "Id", "ConfigValue");
           /* SelectList list = new SelectList((from b in _dbContext.AppCompanyConfigBases where b.Name == configName && b.Module == module select b).ToList(),"Id");
                                              *//*join c in _dbContext.AppCompanyConfigs on b.Id equals c.BaseId
                                              where c.CompanyId == companyId && c.IsActive && c.IsDeleted == false && b.Name == configName && b.Module == module
                                              select c
                                  ).ToList(), "Id", "ConfigValue");*/

            return list;
        }
        public SelectList GetOrgValues(int resp_Id, string configName, int companyId)
        {
            SelectList list = new SelectList("","");
            try
            {


                string respvalues = _dbContext.SysProfileValues.FirstOrDefault(x =>  x.ProfileOption == configName && x.IsDeleted != true).RespValues;
                list = new SelectList((from b in _dbContext.Sys_ORG_Profile_Details.Where(x => x.SysORGProfile_ID == Convert.ToInt32(respvalues))
                                       join c in _dbContext.SysOrganization on b.Organization_ID equals c.Organization_Id
                                       select c
                                    ).ToList(), "Organization_Id", "OrgName");
                return list;
            }
            catch (Exception e)
            {

            }
            return list;
        }
        public SelectList GetConfigBase(int companyId)
        {

            var company = AppSession.GetCompanyId;
            SelectList list = new SelectList((from b in _dbContext.AppCompanyConfigBases
                                              where  b.IsActive && b.IsDeleted == false
                                              select b
                                  ).ToList(), "Id", "Name");

            return list;
        }

        public SelectList GetDeductions(int companyId)
        {
            var deduction = new SelectList(_dbContext.HRDeductions.ToList().Where(s => s.IsDeleted == false && s.CompanyId == companyId), "Id", "Type");
            return deduction;
        }
        public SelectList GetAllowances(int companyId)
        {
            var allowances = new SelectList(_dbContext.HREmployeeAllowances.ToList().Where(s => s.IsDeleted == false && s.CompanyId == companyId), "Id", "Name");
            return allowances;
        }

        public string GetUom(int id)
        {
            return _dbContext.AppCompanyConfigs.Where(c => c.ConfigName == "UOM" && c.Id == id).Select(c => c.ConfigValue).FirstOrDefault();
        }

        public string GetReportPath(string module, string configName)
        {
            string configValues = _dbContext.AppCompanyConfigs
                                            .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                                            .Select(c => c.ConfigValue)
                                            .FirstOrDefault();
            return configValues;
        }

        public string GetCompanyConfig( int id)
        {
            var value = (from s in _dbContext.AppCompanyConfigs
                         where s.Id == id
                         select s.ConfigDescription).FirstOrDefault();
            return value;
        }
        public string GetItemNameById(int itemId)
        {
            var item = _dbContext.InvItems.Where(x => x.Id == itemId)
                                             .Select(a => new
                                             {
                                                 id = a.Id,
                                                 code = a.Code,
                                                 Name = a.Name,
                                             })
                                             .FirstOrDefault();
            return item.Name;
        }
        public string GetOperatingUnit( int id)
        {
            var value = (from s in _dbContext.SysOrganization
                         where s.Organization_Id == id
                         select s.OrgName).FirstOrDefault();
            return value;
        }
        public string GetMenubyId(int id)
        {
            var value = (from s in _dbContext.AppMenus
                         where s.Id == id
                         select s.Name).FirstOrDefault();
            return value;
        }
        public string GetModulebyId(int id)
        {
            var value = (from s in _dbContext.AppModules
                         where s.Id == Convert.ToString(id)
                         select s.Description).FirstOrDefault();
            return value;
        }
        public string GetForm(int id)
        {
            var value = (from s in _dbContext.SYS_FORMS
                         where s.FORMID == id
                         select s.FORM_FMX_NAME).FirstOrDefault();
            return value;
        }
        public string GetCompanySetupValue(string moduleId, string name, int companyId)
        {
            var value = (from s in _dbContext.AppCompanySetups
                         where s.ModuleId == moduleId && s.Name == name && s.CompanyId == companyId
                         select s.Value).FirstOrDefault();
            return value;
        }
        public int CreateGLAccountByParentCode(string type, string name, int companyId, string userId)
        {
            /*
            * type 
            * 1. Customer
            * 2. Supplier 
            * 3. Bank
             */
            string controlAccount = "";
            if (type == "Customer"){
                controlAccount = GetCompanySetupValue("AR", "Customer GL Control Account", companyId); 
            }
            else if (type == "Supplier")
            {
                controlAccount = GetCompanySetupValue("AP", "Supplier GL Control Account", companyId);
            }     
            else if (type == "Bank")
            {
                controlAccount = GetCompanySetupValue("GL", "Bank GL Control Account", companyId);
            }
            else if (type == "Cash")
            {
                controlAccount = GetCompanySetupValue("GL", "Cash GL Control Account", companyId);
            }
            GLAccount parentAccount = _dbContext.GLAccounts.Where(a => !a.IsDeleted && a.Code == controlAccount && a.CompanyId == companyId).FirstOrDefault();

            var account = _dbContext.GLAccounts
                        .Where(a => a.ParentId == parentAccount.Id && a.CompanyId == companyId && a.IsDeleted == false)
                        .OrderByDescending(a => a.Code)
                        .FirstOrDefault();
            string newCode;
            int newCodeLength = 4;
            int newAccountLevel = 4;
            string splitter = ".";
            if (account == null) //means no child row, start with 1
            {
                newCode = "1";
                newCode = newCode.PadLeft(newCodeLength, '0');
            }
            else  //increment 1 into last code
            {
                newCode = account.Code.Substring(account.Code.Length - newCodeLength, newCodeLength);
                int c = Convert.ToInt16(newCode) + 1;
                newCode = c.ToString();
                newCode = newCode.PadLeft(newCodeLength, '0');

            }
            newCode = string.Concat(parentAccount.Code, splitter, newCode);
            GLAccount glAccount = new GLAccount();
            glAccount.ParentId = parentAccount.Id;
            glAccount.Code = newCode;
            glAccount.Name = name;
            glAccount.AccountLevel = Convert.ToInt16(newAccountLevel);
            glAccount.CompanyId = companyId;
            glAccount.IsDeleted = false;
            glAccount.IsActive = true;
            glAccount.CreatedBy = userId;
            glAccount.CreatedDate = DateTime.Now;
            _dbContext.GLAccounts.Add(glAccount);
            _dbContext.SaveChanges();
            return glAccount.Id;
        }
        public SelectList SupplierById(int companyId, int supplierId)
        {
            var suppliers = new SelectList(_dbContext.APSuppliers.ToList().Where(s => s.Id == supplierId && s.IsActive && s.CompanyId == companyId), "Id", "Name");
            return suppliers;
        }
        public SelectList GreigeVendor(int companyId)
        {
            var Customer = new SelectList(_dbContext.APSuppliers.Include(x=>x.Account).ToList().Where(c => c.IsActive /*&& c.CompanyId == companyId*/ && c.Account.Code == "2.02.04.0005"), "Id", "Name");
            return Customer;
        }
        public SelectList GreigeVendorByCode(int companyId)
        {
            var Customer = new SelectList(_dbContext.APSuppliers.Include(x => x.Account).ToList().Where(c => c.IsActive /*&& c.CompanyId == companyId*/ && c.Account.Code == "2.02.04.0006"), "Id", "Name");
            return Customer;
        }
        public string GetOrgName(int resp_Id, string configName, int companyId)
        {
            var OrgName = "";
            try
            {


                string respvalues = _dbContext.SysProfileValues.SingleOrDefault(x => x.RespId == resp_Id && x.ProfileOption == configName && x.IsDeleted != true).RespValues;
                OrgName = (from b in _dbContext.Sys_ORG_Profile_Details.Where(x => x.SysORGProfile_ID == Convert.ToInt32(respvalues))
                                       join c in _dbContext.SysOrganization on b.Organization_ID equals c.Organization_Id
                                       select c
                                    ).FirstOrDefault().OrgName;
                return OrgName;
            }
            catch (Exception e)
            {

            }
            return OrgName;
        }
        [HttpGet]
        public SelectList GetSecondCategoryByResp(int resp_Id)
        {
            var items = _dbContext.AppCompanySetups.Where(x => x.Name == "Local Finish ItemCode").FirstOrDefault().Value;

            var x = _dbContext.Sys_ResponsibilityItemCategory.Include(x => x.ItemCategory).Where(x => x.ResponsibilityId == resp_Id).ToList();
            var b = new SelectList(from ac in x.Where(x => x.ItemCategory.IsDeleted == false && x.ItemCategory.CategoryLevel == 2 && x.ItemCategory.Code.StartsWith(items)).OrderBy(x => x.ItemCategory.Code).ToList()
                                   select new
                                   {
                                       Id = ac.ItemCategory.Id,
                                       Name = ac.ItemCategory.Code + " - " + ac.ItemCategory.Name
                                   }, "Id", "Name");
            return b;
        }
    }
}
