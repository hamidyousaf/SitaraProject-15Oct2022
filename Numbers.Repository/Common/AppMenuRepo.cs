using Numbers.Repository.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using Numbers.Entity.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Numbers.Repository.Common
{
    public class AppMenuRepo
    {
        readonly NumbersDbContext _dbContext;
        List<int> returnVal= new List<int>();
        public AppMenuRepo()
        {
            _dbContext = new NumbersDbContext(AppSession.GetHttpContextAccessor);
        }
       
        public List<int> GetParentMenuId(int menuId,bool clear)
        {
            if (menuId == 0)
                return returnVal;
            int parentId = _dbContext.AppMenus.Find(menuId).ParentId;
            if (parentId == 0)
                return returnVal;
            returnVal.Add(parentId);
            GetParentMenuId(parentId, false);
            return returnVal;
        } 
        public List<int> GetParentMenu(int menuId)
        {
            if (menuId == 0)
                return returnVal;
            int parentId = _dbContext.SYS_MENU_M.Find(menuId).MENU_ID;
            if (parentId == 0)
                return returnVal;

            returnVal.Add(parentId);
            GetParentMenuId(parentId, false);
            return returnVal;
        }
    }
}
