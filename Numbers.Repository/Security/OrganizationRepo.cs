using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Repository.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numbers.Repository.Inventory
{
    public class OrganizationRepo
    {
        private HttpContext HttpContext { get; }
        private readonly NumbersDbContext _dbContext;
        public OrganizationRepo(NumbersDbContext context)
        {
            _dbContext = context;
        }
        public OrganizationRepo(NumbersDbContext context,HttpContext httpContext)
        {
            _dbContext = context;
            HttpContext = httpContext;
        }

        public IEnumerable<SysOrganization> GetAll(int companyId ,string userid)
        {
             
            IEnumerable<SysOrganization> list = _dbContext.SysOrganization.ToList();
            return list;
        }

        //public SYS_MENU_D[] GetStoreIssueItems(int id)
        //{
        //    InvStoreIssueItem[] storeIssueItems = _dbContext.InvStoreIssueItems.Where(i => i.StoreIssueId == id && i.IsDeleted == false).ToArray();
        //    return storeIssueItems;
        //}

        public OrganizationViewModel GetById(int id)
        {
            SysOrganization storeIssue = _dbContext.SysOrganization.Find(id);
            var viewModel = new OrganizationViewModel();
            viewModel.Organization_Id = storeIssue.Organization_Id;
            viewModel.OrgName = storeIssue.OrgName;
            viewModel.OrgType = storeIssue.OrgType;
            viewModel.EffectiveFromDate = storeIssue.EffectiveFromDate;
            viewModel.EffectiveToDate = storeIssue.EffectiveToDate;
            viewModel.OrgLocation = storeIssue.OrgLocation;
            viewModel.OrgInternalExternal = storeIssue.OrgInternalExternal;
            viewModel.OrgAddress = storeIssue.OrgAddress;
            viewModel.OrgIntAddress = storeIssue.OrgIntAddress;
            viewModel.ShortName = storeIssue.ShortName;

            return viewModel;
        }

        [HttpPost]
        public async Task<bool> Create(OrganizationViewModel model, IFormCollection collection)
        {
            try
            {
                //for master table
                var newModel = new SysOrganization();
              //  newModel.Organization_Id = model.Organization_Id;
                newModel.OrgName = model.OrgName;
                newModel.OrgType = model.OrgType;
                newModel.EffectiveFromDate = model.EffectiveFromDate;
                newModel.EffectiveToDate = model.EffectiveToDate;
                newModel.OrgLocation = model.OrgLocation;
                newModel.OrgInternalExternal = model.OrgInternalExternal;
                newModel.OrgAddress = model.OrgAddress;
                newModel.OrgIntAddress = model.OrgIntAddress;
                newModel.ShortName = model.ShortName;
                _dbContext.SysOrganization.Add(newModel);
                await _dbContext.SaveChangesAsync();
                //for detail table
                SysOrgClassification[] InvStoreIssueItems = JsonConvert.DeserializeObject<SysOrgClassification[]>(collection["details"]);
                if (InvStoreIssueItems.Count() > 0)
                {
                    foreach (var items in InvStoreIssueItems)
                    {
                        if (items.ClassificationDetailId != 0)
                        {
                            SysOrgClassification data = _dbContext.SysOrgClassification.Where(i => i.OrganizationId == newModel.Organization_Id  && i.ClassificationDetailId == items.ClassificationDetailId).FirstOrDefault();
                            //foreach (var i in data)
                            //{

                            SysOrgClassification obj = new SysOrgClassification();
                            obj = data;
                            obj.OrganizationId = newModel.Organization_Id;
                            obj.ClassificationId = items.ClassificationId;
                            obj.Ledger_Id = items.Ledger_Id;
                            obj.OperationUnitId = items.OperationUnitId;
                            obj.BusinessUnitId = items.BusinessUnitId;
                            _dbContext.SysOrgClassification.Update(obj);
                            _dbContext.SaveChanges();
                            //}
                        }
                        else
                        {
                            SysOrgClassification data = new SysOrgClassification();
                            ///data = items;
                            data.OrganizationId = newModel.Organization_Id;
                            data.ClassificationId = items.ClassificationId;
                            data.Ledger_Id = items.Ledger_Id;
                            data.OperationUnitId = items.OperationUnitId;
                            data.BusinessUnitId = items.BusinessUnitId;
                            _dbContext.SysOrgClassification.Add(data);
                            _dbContext.SaveChanges();
                        }
                    }
                }
                    
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                return false;
            }
        }
        [HttpPost]
        public async Task<bool> Update(OrganizationViewModel model, IFormCollection collection)
        {
            //updating existing data
            var obj = _dbContext.SysOrganization.Find(model.Organization_Id);
            obj.OrgName = model.OrgName;
            obj.OrgType = model.OrgType;
            obj.EffectiveFromDate = model.EffectiveFromDate;
            obj.EffectiveToDate = model.EffectiveToDate;
            obj.OrgLocation = model.OrgLocation;
            obj.OrgInternalExternal = model.OrgInternalExternal;
            obj.OrgAddress = model.OrgAddress;
            obj.OrgIntAddress = model.OrgIntAddress;
            obj.ShortName = model.ShortName;
            obj.LastUpdatedDate = DateTime.Now;
            var entry = _dbContext.SysOrganization.Update(obj);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();

            var list = _dbContext.SysOrgClassification.Where(l => l.OrganizationId == model.Organization_Id).ToList();
            SysOrgClassification[] InvStoreIssueItems = JsonConvert.DeserializeObject<SysOrgClassification[]>(collection["details"]);
            if (InvStoreIssueItems.Count() > 0)
            {
                foreach (var items in InvStoreIssueItems)
                {
                    if (items.ClassificationDetailId != 0)
                    {
                        SysOrgClassification data = _dbContext.SysOrgClassification.Where(i => i.OrganizationId == obj.Organization_Id && i.ClassificationDetailId == items.ClassificationDetailId).FirstOrDefault();
                        //foreach (var i in data)
                        //{

                        SysOrgClassification obj2 = new SysOrgClassification();
                        obj2 = data;
                        obj2.OrganizationId = obj.Organization_Id;
                        obj2.ClassificationId = items.ClassificationId;
                        obj2.Ledger_Id = items.Ledger_Id;
                        obj2.OperationUnitId = items.OperationUnitId;
                        obj2.BusinessUnitId = items.BusinessUnitId;
                        _dbContext.SysOrgClassification.Update(obj2);
                        _dbContext.SaveChanges();
                        //}
                    }
                    else
                    {
                        SysOrgClassification data = new SysOrgClassification();
                        ///data = items;
                        data.OrganizationId = obj.Organization_Id;
                        data.ClassificationId = items.ClassificationId;
                        data.Ledger_Id = items.Ledger_Id;
                        data.OperationUnitId = items.OperationUnitId;
                        data.BusinessUnitId = items.BusinessUnitId;
                        _dbContext.SysOrgClassification.Add(data);
                        _dbContext.SaveChanges();
                    }
                }
            }
            return true;
        }

        //public dynamic GetStoreIssueItems(int id)
        //{
        //    var item = _dbContext.SYS_MENU_D.Include(i => i.SYS_FORMS).Where(i => i.MENU_D_ID == id ).FirstOrDefault();
        //    SYS_MENU_D viewModel = new SYS_MENU_D();
        //    viewModel.PROMPTS = item.PROMPTS;
        //    viewModel.SUBMENU_ID = item.SUBMENU_ID;
        //    viewModel.FUNCTION_ID = item.FUNCTION_ID;
        //    viewModel.DESCRIPTION = item.DESCRIPTION;
        //    viewModel.SEQUENCE_ID = item.SEQUENCE_ID;
        //    return viewModel;
        //}

        public dynamic GetStoreIssueItems(int id)
        {
            var item = _dbContext.SysOrgClassification.Where(i => i.OrganizationId == id).FirstOrDefault();
            SysOrgClassification viewModel = new SysOrgClassification();
            viewModel.ClassificationId = item.ClassificationId;
            viewModel.Ledger_Id = item.Ledger_Id;
            viewModel.OperationUnitId = item.OperationUnitId;
            viewModel.BusinessUnitId = item.BusinessUnitId;
            return viewModel;
        }
    }
}
