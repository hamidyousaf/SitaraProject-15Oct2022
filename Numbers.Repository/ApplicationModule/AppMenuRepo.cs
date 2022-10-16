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
    public class AppMenuRepo
    {
        private HttpContext HttpContext { get; }
        private readonly NumbersDbContext _dbContext;
        public AppMenuRepo(NumbersDbContext context)
        {
            _dbContext = context;
        }
        public AppMenuRepo(NumbersDbContext context,HttpContext httpContext)
        {
            _dbContext = context;
            HttpContext = httpContext;
        }

        public IEnumerable<SYS_MENU_M> GetAll(int companyId ,string userid)
        {
             
            IEnumerable<SYS_MENU_M> list = _dbContext.SYS_MENU_M.Where(x=>x.IS_ACTIVE!=false).ToList();
            return list;
        }

        //public SYS_MENU_D[] GetStoreIssueItems(int id)
        //{
        //    InvStoreIssueItem[] storeIssueItems = _dbContext.InvStoreIssueItems.Where(i => i.StoreIssueId == id && i.IsDeleted == false).ToArray();
        //    return storeIssueItems;
        //}

        public AppMenuViewModel GetById(int id)
        {
            SYS_MENU_M storeIssue = _dbContext.SYS_MENU_M.Find(id);
            var viewModel = new AppMenuViewModel();
            viewModel.MENU_ID = storeIssue.MENU_ID;
            viewModel.MENU_NAME = storeIssue.MENU_NAME;
            viewModel.USER_MENU_NAME = storeIssue.USER_MENU_NAME;
            viewModel.DESCRIPTION = storeIssue.DESCRIPTION;
            viewModel.MENU_TYPE = storeIssue.MENU_TYPE;

            return viewModel;
        }

        [HttpPost]
        public async Task<bool> Create(AppMenuViewModel model, IFormCollection collection)
        {
            try
            {
                //for master table
                var menuModel = new SYS_MENU_M();
                menuModel.MENU_NAME = model.MENU_NAME;
                menuModel.DESCRIPTION = model.DESCRIPTION;
                menuModel.CREATED_BY = model.CREATED_BY;
                menuModel.CREATED_DATE = model.CREATED_DATE;
                menuModel.USER_MENU_NAME = model.USER_MENU_NAME;
                menuModel.MENU_TYPE = model.MENU_TYPE;
                menuModel.IS_ACTIVE = true;
                _dbContext.SYS_MENU_M.Add(menuModel);
                await _dbContext.SaveChangesAsync();
                //for detail table
                SYS_MENU_D[] InvStoreIssueItems = JsonConvert.DeserializeObject<SYS_MENU_D[]>(collection["details"]);
                if (InvStoreIssueItems.Count() > 0)
                {
                    foreach (var items in InvStoreIssueItems)
                    {
                        if (items.MENU_D_ID != 0)
                        {
                            SYS_MENU_D data = _dbContext.SYS_MENU_D.Where(i => i.MENU_M_ID == menuModel.MENU_ID  && i.MENU_D_ID == items.MENU_D_ID).FirstOrDefault();
                            //foreach (var i in data)
                            //{

                            SYS_MENU_D obj = new SYS_MENU_D();
                            obj = data;
                            obj.MENU_M_ID = menuModel.MENU_ID;
                            obj.PROMPTS = items.PROMPTS;
                            obj.SUBMENU_ID = items.SUBMENU_ID;
                            obj.FUNCTION_ID = items.FUNCTION_ID;
                            obj.DESCRIPTION = items.DESCRIPTION;
                            obj.SEQUENCE_ID = items.SEQUENCE_ID;
                            _dbContext.SYS_MENU_D.Update(obj);
                            _dbContext.SaveChanges();
                            //}
                        }
                        else
                        {
                            SYS_MENU_D data = new SYS_MENU_D();
                            ///data = items;
                            data.MENU_M_ID = menuModel.MENU_ID;
                            data.PROMPTS = items.PROMPTS;
                            data.SUBMENU_ID = items.SUBMENU_ID;
                            data.FUNCTION_ID = items.FUNCTION_ID;
                            data.DESCRIPTION = items.DESCRIPTION;
                            data.SEQUENCE_ID = items.SEQUENCE_ID;
                            _dbContext.SYS_MENU_D.Add(data);
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
        public async Task<bool> Update(AppMenuViewModel model, IFormCollection collection)
        {
            //for partial-items removal
            SYS_MENU_D[] InvStoreIssueItems = JsonConvert.DeserializeObject<SYS_MENU_D[]>(collection["details"]);
            var existingDetail = _dbContext.SYS_MENU_D.Where(x => x.MENU_M_ID == model.MENU_ID).ToList();

            foreach (var detail in existingDetail)
            {
                bool isExist = InvStoreIssueItems.Any(x => x.MENU_D_ID == detail.MENU_D_ID);
                if (!isExist)
                {
                    _dbContext.SYS_MENU_D.Remove(detail);
                    await _dbContext.SaveChangesAsync();
                }
            }

            //updating existing data
            var obj = _dbContext.SYS_MENU_M.Find(model.MENU_ID);
            obj.MENU_NAME = model.MENU_NAME;
            obj.DESCRIPTION = model.DESCRIPTION;
            obj.CREATED_BY = model.CREATED_BY;
            obj.CREATED_DATE = model.CREATED_DATE;
            obj.USER_MENU_NAME = model.USER_MENU_NAME;
            obj.MENU_TYPE = model.MENU_TYPE;
            obj.MENU_TYPE = model.MENU_TYPE;
            obj.LAST_UPDATED_DATE = DateTime.Now;
            var entry = _dbContext.SYS_MENU_M.Update(obj);
            entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
            await _dbContext.SaveChangesAsync();

            //var list = _dbContext.SYS_MENU_D.Where(l => l.MENU_M_ID == model.MENU_ID).ToList();
            //if (InvStoreIssueItems.Count() > 0)
            //{
                foreach (var items in InvStoreIssueItems)
                {
                    if (items.MENU_D_ID != 0)
                    {
                        SYS_MENU_D data = _dbContext.SYS_MENU_D.Where(i => i.MENU_M_ID == obj.MENU_ID   && i.MENU_D_ID == items.MENU_D_ID).FirstOrDefault();
                       if(data!=null)
                        {
                        SYS_MENU_D objItem = new SYS_MENU_D();
                        objItem = data;
                        objItem.MENU_M_ID = model.MENU_ID;
                        objItem.PROMPTS = items.PROMPTS;
                        objItem.SUBMENU_ID = items.SUBMENU_ID;
                        objItem.FUNCTION_ID = items.FUNCTION_ID;
                        objItem.DESCRIPTION = items.DESCRIPTION;
                        objItem.SEQUENCE_ID = items.SEQUENCE_ID;
                        _dbContext.SYS_MENU_D.Update(objItem);
                        _dbContext.SaveChanges();
                        }
                        //}
                    }
                    else
                    {
                        SYS_MENU_D data = new SYS_MENU_D();
                        //data = items;
                        data.MENU_M_ID = obj.MENU_ID;
                        data.PROMPTS = items.PROMPTS;
                        data.SUBMENU_ID = items.SUBMENU_ID;
                        data.FUNCTION_ID = items.FUNCTION_ID;
                        data.DESCRIPTION = items.DESCRIPTION;
                        data.SEQUENCE_ID = items.SEQUENCE_ID;
                        _dbContext.SYS_MENU_D.Add(data);
                        _dbContext.SaveChanges();
                    }
            }
            //}
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

        public async Task<bool> Delete(int Id)
        {
            var module = await _dbContext.SYS_MENU_M.Where(n => n.MENU_ID == Id).FirstAsync();
            if (module != null)
            {
                module.IS_ACTIVE = false;
                _dbContext.SYS_MENU_M.Update(module);
                await _dbContext.SaveChangesAsync();
            }
            return true;
        }
        public dynamic GetStoreIssueItems(int id)
        {
            var item = _dbContext.SYS_MENU_D.Where(i => i.MENU_M_ID == id).FirstOrDefault();
            SYS_MENU_D viewModel = new SYS_MENU_D();
            viewModel.PROMPTS = item.PROMPTS;
            viewModel.SUBMENU_ID = item.SUBMENU_ID;
            viewModel.FUNCTION_ID = item.FUNCTION_ID;
            viewModel.DESCRIPTION = item.DESCRIPTION;
            viewModel.SEQUENCE_ID = item.SEQUENCE_ID;
            return viewModel;
        }
    }
}
