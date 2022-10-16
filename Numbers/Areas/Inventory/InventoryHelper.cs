using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Areas.Inventory
{
    public class InventoryHelper
    {
        private readonly IList<SelectListItem> _options = new List<SelectListItem>();
        private readonly NumbersDbContext _dbContext;
        private readonly int _companyId;
        public InventoryHelper(NumbersDbContext context,int companyId)
        {
            _dbContext = context;
            _companyId = companyId;
        }

        public IEnumerable<SelectListItem> GetCategoriesSelectLists()
        {
            GetRootCategories();
            return _options;
        }
        //==== Recrusive method.
        public void GetRootCategories()
        {
            _options.Add(new SelectListItem() { Value = "1", Text = "Root" });
            var categories = _dbContext.InvItemCategories.Where(x => x.ParentId == null && x.CompanyId==_companyId).ToList(); //Root level categories
            foreach (var category in categories)
            {
                SelectListItem listItem = new SelectListItem();
                listItem.Value = category.Id.ToString();
                listItem.Text =category.Code+"-"+ category.Name; //string.Format("{0} --> {1}", category.Name, category.Name);
                _options.Add(listItem);//Add current options
                GetParentCategories(category.Id, category.Name);
            }
        }
        string lastParentName = "";
        public void GetParentCategories(int parentId, string parentName)
        {
            var ctx = new NumbersDbContext();
            var parents = ctx.InvItemCategories.Where(p => p.Id == p.ParentId && p.CompanyId==_companyId).ToList();
            var children = from r in ctx.InvItemCategories
                           where r.ParentId == parentId
                           select new { r.Name, r.Id, r.ParentId };
            foreach (var child in children)
            {
                SelectListItem listItem = new SelectListItem();
                listItem.Value = child.Id.ToString();
                lastParentName = string.Format("{0} --> {1}", parentName, child.Name);
                listItem.Text = lastParentName;
                _options.Add(listItem);
                GetParentCategories(child.Id, lastParentName);
            }
        }
        public string oldParent = null;
        public string GetParent(int? parent,string name)
        {
            var ctx = new NumbersDbContext();

            var parents = ctx.InvItemCategories.Where(p => p.Id == parent && p.CompanyId == _companyId).FirstOrDefault();
            var children = from r in ctx.InvItemCategories
                           where r.ParentId == parent && r.Name==name
                           select new { r.Name, r.Id, r.ParentId };
            string grand = "";
            string prev = "";
            var grandParent = ctx.InvItemCategories.Where(p => p.Id == parents.ParentId && p.CompanyId==_companyId).FirstOrDefault();
            foreach (var child in children)
            {
                if (grandParent != null)
                {
                    prev = parents.Name;
                    grand = grandParent.Name;
                    prev = string.Format("{0} --> {1}", grand, prev);
                    if (grandParent.ParentId != null)
                    {
                        oldParent = prev;
                        GetParent(child.Id,grandParent.Name);
                    }

                }
                else if (parents.ParentId == null)
                {
                    prev = parents.Name;
                }
                else if (oldParent != null)
                {
                    prev = string.Format("{0} --> {1}", parents.Name, oldParent);
                }                
                else
                {
                    prev = parents.Name;
                }
            }
            return prev;
        }
    }
}
