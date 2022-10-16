using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using static Numbers.Areas.ApplicationModule.Controllers.ApiController;

namespace Numbers.Controllers
{
    public class ApiController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        private readonly IEmailService emailService;
        private readonly IList<SelectListItem> _options = new List<SelectListItem>();

        public ApiController(NumbersDbContext dbContext, IEmailService emailService)
        {
            _dbContext = dbContext;
            this.emailService = emailService;

        }
        [HttpGet]
        public IActionResult GetCities(int id)
        {
            var cities = _dbContext.AppCities.Where(c => c.CountryId == id).OrderBy(c=>c.Name).ToList();
            return Ok(cities);
        }
        [HttpGet]
        public IActionResult GetConfigValuesById(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var configs = _dbContext.AppCompanyConfigs.Where(c => c.BaseId == id && c.CompanyId == companyId && c.IsDeleted == false).ToList();
            return Ok(configs);
        }

        [HttpGet]
        public IActionResult GetCitiesByCountryId(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var cities = _dbContext.AppCities.Where(c => c.CountryId == id).OrderBy(c=>c.Name).ToList();
            return Ok(cities);
        }

        
        [HttpGet]
        public IEnumerable<AccountTree> GetItemCategory()
        {
            int companyId = 1;
            var categories = _dbContext.InvItemCategories.Include(c => c.Parent).Where(a => a.IsDeleted == false && a.CompanyId == companyId).OrderBy(a => a.CategoryLevel);
            List<AccountTree> trees = new List<AccountTree>();
            foreach (var category in categories)
            {
                AccountTree tree = new AccountTree();
                tree.Id = category.Id;
                tree.Parent = (category.ParentId == null ? "#" : category.ParentId.ToString());
                tree.Text = category.Name;
                if (!category.IsActive)
                    tree.Icon = "far fa-file-minus";
                else
                    tree.Icon = "far fa-folder";
                trees.Add(tree);
            }
            return trees;
        }

        public IEnumerable<SelectListItem> GetMenuSelectLists(int id)
        {
            GetRootMenus(id);
            return _options.Distinct();
        }
        //==== Recrusive method.
        public void GetRootMenus(int id)
        {
            int companyId =1;
           // _options.Add(new SelectListItem() { Value = "0", Text = "Root" });
            var menus = _dbContext.AppMenus.Where(x => x.ParentId == id ||x.Id==id && x.CompanyId == companyId).ToList(); //Root level categories
            foreach (var menu in menus)
            {
                SelectListItem listItem = new SelectListItem();
                listItem.Value = menu.Id.ToString();
                listItem.Text = menu.Name; //string.Format("{0} --> {1}", category.Name, category.Name);
                _options.Add(listItem);//Add current options
                GetParentMenus(menu.Id, menu.Name);
                //_options.Remove(listItem);
            }
        }


        string lastParentName = "";
        public void GetParentMenus(int parentId, string parentName)
        {
            int companyId = 1;
            var chiled = _dbContext.AppMenus.FromSql("Select * from AppMenus where Id not in ( select Id from AppMenus where Id  in ( select ParentId from AppMenus))  and IsActive =0");
            var ctx = new NumbersDbContext();
            var parents = ctx.AppMenus.Where(p => p.Id == p.ParentId && p.CompanyId == companyId).ToList();
            var children = from r in ctx.AppMenus
                           where r.ParentId == parentId
                           select new { r.Name, r.Id, r.ParentId };
            foreach (var child in children)
            {
                SelectListItem listItem = new SelectListItem();
                listItem.Value = child.Id.ToString();
                lastParentName = string.Format("{0} --> {1}", parentName, child.Name);
                listItem.Text = lastParentName;
                foreach (var lst in chiled)
                    if (child.Id == lst.Id)
                    {
                        _options.Add(listItem);
                    }
                GetParentMenus(child.Id, lastParentName);
            }
        }

        public string oldParent = null;
        public string GetParent(int? parent, string name)
        {
            int companyId = 1;
            var ctx = new NumbersDbContext();

            var parents = ctx.InvItemCategories.Where(p => p.Id == parent && p.CompanyId == companyId).FirstOrDefault();
            var children = from r in ctx.InvItemCategories
                           where r.ParentId == parent && r.Name == name
                           select new { r.Name, r.Id, r.ParentId };
            string grand = "";
            string prev = "";
            var grandParent = ctx.InvItemCategories.Where(p => p.Id == parents.ParentId && p.CompanyId == companyId).FirstOrDefault();
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
                        GetParent(child.Id, grandParent.Name);
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


        [HttpPost]
        public IActionResult SendEmaiL(string subject, string senderEmail, string message)
        {
            var so = _dbContext.ARSaleOrderItems.Include(x=>x.SaleOrder).Where(x=>x.IsDeleted==false).ToList();
            var soTotal = so.Sum(x=>x.Total);
            var totalSo = so.Count();
            var sototalQty = so.Sum(x=>x.Qty);
            message = DateTime.Now.Date +"Sale Order";
                emailService.Send("Sajidtalib74@gmail.com", "saleemi@visionplus.com.pk", "Sajid Maher", message);
 
            return View();
        }


    }
}