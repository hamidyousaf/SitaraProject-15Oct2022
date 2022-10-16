using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Numbers.Areas.Inventory;
using Numbers.Entity.Models;

namespace Numbers.Areas.ApplicationModule.Controllers
{
    [Area("ApplicationModule")]
    public class ApiController : Controller
    {
        private readonly IList<SelectListItem> _options = new List<SelectListItem>();
        private readonly NumbersDbContext _dbContext;
        public ApiController(NumbersDbContext context)
        {
            _dbContext = context;
        }
        public IActionResult GetItems(string q = "")
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            InventoryHelper helper = new InventoryHelper(_dbContext, companyId);
            //var i = _dbContext.AppMenus.Where(
            //                                    (a => ( a.Name.Contains(q)) && a.CompanyId == companyId
            //                                   )).ToList();
            var items = _dbContext.AppMenus.Where(
                                                (a => (a.Name.Contains(q)) &&  a.CompanyId == companyId && a.IsActive==true 
                                               ))
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   code = a.Id,
                                                   text = a.Name,
                                               })
                                               .OrderBy(a => a.text)
                                               .ToList();
            return Ok(items);
        }
        [HttpGet]
        public IActionResult GetItem(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var items = _dbContext.AppMenus.Where(a => a.Id == id && a.CompanyId == companyId)
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = string.Concat(a.Name)
                                               })
                                               .FirstOrDefault();
            return Ok(items);
        }
        public IActionResult GetFunctions(string q = "")
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            InventoryHelper helper = new InventoryHelper(_dbContext, companyId);
            var i = _dbContext.SYS_FORMS.Where(
                                                (a => ( a.FORM_FMX_NAME.Contains(q)) 
                                               )).ToList();
            var items = _dbContext.SYS_FORMS.Where(
                                                (a => (a.FORM_FMX_NAME.Contains(q)) 
                                               ))
                                               .Select(a => new
                                               {
                                                   id = a.FORMID,
                                                   code = a.SHORT_CODE,
                                                   text = a.FORM_FMX_NAME,
                                               })
                                               .OrderBy(a => a.text)
                                               .Take(25)
                                               .ToList();
            return Ok(items);
        }
        [HttpGet]
        public IActionResult GetFunction(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var items = _dbContext.SYS_FORMS.Where(a => a.FORMID == id )
                                               .Select(a => new
                                               {
                                                   id = a.FORMID,
                                                   text = string.Concat(a.FORM_FMX_NAME)
                                               })
                                               .FirstOrDefault();
            return Ok(items);
        }
        public IActionResult GetMenus(string q = "")
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            InventoryHelper helper = new InventoryHelper(_dbContext, companyId);
            var i = _dbContext.AppMenus.Where(
                                                (a => (a.Name.Contains(q))
                                               )).ToList();
            //var items = _dbContext.AppMenus.Where(
            //                                    (a => (a.Name.Contains(q)) && a.CompanyId == companyId && a.IsActive == true
            //                                   ))
            //                                   .Select(a => new
            //                                   {
            //                                       id = a.Id,
            //                                       code = a.Id,
            //                                       text = a.Name,
            //                                   })
            //                                   .OrderBy(a => a.text)
            //                                   .ToList();

            var menus = (from a in _dbContext.AppMenus.Where((a => (a.Name.Contains(q)) && a.CompanyId == companyId && a.IsActive == true
                                               ))
                            join b in _dbContext.AppCompanyModules on a.ModuleId equals b.Id
                            where a.IsActive ==true
                            select new
                            {
                                id = a.Id,
                                code = a.Id,
                                text = a.Name + " - " + b.Short_Name,

                            }).OrderBy(a => a.text).ToList();;


            return Ok(menus);
        }
        
        [HttpGet]
        public IActionResult GetMenu(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var items = _dbContext.AppMenus.Where(a => a.Id == id)
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = string.Concat(a.Name)
                                               })
                                               .FirstOrDefault();
            return Ok(items);
        }


        public IActionResult GetAppMenus(string q = "")
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            InventoryHelper helper = new InventoryHelper(_dbContext, companyId);
            var i = _dbContext.SYS_MENU_M.Where(
                                                (a => (a.MENU_NAME.Contains(q))
                                               )).ToList();
            var items = _dbContext.SYS_MENU_M.Where(
                                                (a => (a.MENU_NAME.Contains(q)) && a.IS_ACTIVE == true
                                               ))
                                               .Select(a => new
                                               {
                                                   id = a.MENU_ID,
                                                   code = a.MENU_ID,
                                                   text = a.MENU_NAME,
                                               })
                                               .OrderBy(a => a.text)
                                               .ToList();

          


            return Ok(items);
        }

        [HttpGet]
        public IActionResult GetAppMenu(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var items = _dbContext.SYS_MENU_M.Where(a => a.MENU_ID == id)
                                               .Select(a => new
                                               {
                                                   id = a.MENU_ID,
                                                   text = string.Concat(a.MENU_NAME)
                                               })
                                               .FirstOrDefault();
            return Ok(items);
        }

        public IActionResult GetInvOrganizations(string q = "")
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            InventoryHelper helper = new InventoryHelper(_dbContext, companyId);
            
            var items = _dbContext.AppCompanyConfigs.Where(
                                                (a => (a.ConfigDescription.Contains(q)) && a.BaseId==1066
                                               ))
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   code = a.Id,
                                                   text = a.ConfigDescription,
                                               })
                                               .OrderBy(a => a.text)
                                               .Take(25)
                                               .ToList();
            return Ok(items);
        }
        public IActionResult GetLedgers(string q = "")
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            InventoryHelper helper = new InventoryHelper(_dbContext, companyId);

            var items = _dbContext.AppCompanyConfigs.Where(
                                                (a => (a.ConfigDescription.Contains(q)) && a.BaseId == 1067 && a.IsActive == true
                                               ))
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   code = a.Id,
                                                   text = a.ConfigDescription,
                                               })
                                               .OrderBy(a => a.text)
                                               .Take(25)
                                               .ToList();
            return Ok(items);
        }
        public IActionResult GetAllDivision()
        {
            var items = _dbContext.GLDivision.Select(a => new
            {
                id = a.Id,
                text = a.Name,
            }).ToList();
            return Ok(items);
        }
        [HttpGet]
        public IActionResult GetLedger(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var items = _dbContext.AppCompanyConfigs.Where(a => a.Id == id && a.BaseId==1067)
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = string.Concat(a.ConfigDescription)
                                               })
                                               .FirstOrDefault();
            return Ok(items);
        }

        public IActionResult GetOperatingUnits(string q = "")
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var OperUnit = (from a in _dbContext.SysOrganization.Where(x => x.CompanyId == companyId)
                            join b in _dbContext.SysOrgClassification on a.Organization_Id equals b.OrganizationId
                            where b.ClassificationId == 179
                            select new
                            {
                                id =a.Organization_Id,
                                text =a.OrgName

                            }
                           ).ToList();

            //var items = _dbContext.AppCompanyConfigs.Where(a => Convert.ToInt32(a.Id) == id).FirstOrDefault();
            return Ok(OperUnit);
        }
        [HttpGet]
        public IActionResult GetOperatingUnit(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var OperUnit = (from a in _dbContext.SysOrganization.Where(x => x.CompanyId == companyId)
                            join b in _dbContext.SysOrgClassification on a.Organization_Id equals b.OrganizationId
                            where b.ClassificationId == 179
                            select new
                            {
                                id =a.Organization_Id,
                               text = a.OrgName

                            }
                           ).ToList();
            return Ok(OperUnit.Where(x=>x.id==id));
        }
       

        [HttpGet]
        public IActionResult GetInvOrganization(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var items = _dbContext.AppCompanyConfigs.Where(a => a.Id == id && a.BaseId==1066)
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = string.Concat(a.ConfigDescription)
                                               })
                                               .FirstOrDefault();
            return Ok(items);
        }

        [HttpGet]
        public IActionResult GetAppFormbyId(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var items = _dbContext.SYS_FORMS.Where(a => a.FORMID == id).FirstOrDefault();
            return Ok(items.FORM_FMX_NAME);
        }
        [HttpGet]
        public IActionResult GetSubMenuById(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var items = _dbContext.AppMenus.Where(a => a.Id == id).FirstOrDefault();
            return Ok(items.Name);
        }
        public IActionResult GetModules(string q = "")
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            InventoryHelper helper = new InventoryHelper(_dbContext, companyId);
            var items = _dbContext.AppModules.Where(
                                                (a => (a.Description.Contains(q))
                                               ))
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = a.Description,
                                               })
                                               .OrderBy(a => a.text)
                                               .Take(25)
                                               .ToList();
            return Ok(items);
        }
        [HttpGet]
        public IActionResult GetModule(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var items = _dbContext.AppModules.Where(a => Convert.ToInt32( a.Id) == id)
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = string.Concat(a.Description)
                                               })
                                               .FirstOrDefault();
            return Ok(items);
        }
        [HttpGet]
        public IActionResult GetValueNamebyId(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var items = _dbContext.AppCompanyConfigs.Where(a => Convert.ToInt32(a.Id) == id).FirstOrDefault();
            return Ok(items);
        }
        public  IActionResult GetCategories()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            InventoryHelper helper = new InventoryHelper(_dbContext, companyId);
            //ViewBag.listOfItemCategories = helper.GetCategoriesSelectLists();
            //var categories = _dbContext.InvItems.Include(c => c.Categories)
            //    .Where(c => c.CompanyId == companyId && c.IsDeleted == false).Select(c => new
            //    {
            //        id = c.CategoriesId,
            //        text = c.Categories.Name
            //    })
            //    .OrderBy(c => c.text)
            //    .ToList();
            return Ok(helper.GetCategoriesSelectLists());
        }
        public IActionResult GetManufactures()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var manufactures = _dbContext.AppCompanyConfigs
                .Where(m => m.Module == "Inventory" && m.ConfigName == "Manufacture Company" && m.CompanyId == companyId && m.IsDeleted == false)
                .Select(m => new
                {
                    id =m.Id ,
                    text = m.ConfigValue
                })
                .OrderBy(m => m.text)
                .ToList();               
            return Ok(manufactures);
        }

        [HttpPost]
        public IActionResult GetStockByItemWarehouse(int itemId, int warehouseId, DateTime stockDate)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            try
            {
                var Stock = _dbContext.VwInvLedgers
               .Where(s => s.ItemId == itemId && s.CompanyId == companyId && s.WareHouseId == warehouseId && s.TransDate <= stockDate)
               .Select(s => s.Qty).DefaultIfEmpty(0).ToArray();
                var totalStock = Stock.Sum();
                return Ok(totalStock);
            }
            catch(Exception ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetItem(int id, bool isFirst, bool isLast)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            if (isFirst)
            {
                var items = _dbContext.InvItems.Where(a => a.IsDeleted == false && a.CompanyId == companyId )
                                                 .Select(a => new
                                                 {
                                                     id = a.Id,
                                                     text = string.Concat(a.Code, " - ", a.Name),
                                                     code = a.Code
                                                 })
                                                .OrderBy(a => a.text)
                                                .FirstOrDefault();
                return Ok(items);
            }
            else if (isLast)
            {
                var items = _dbContext.InvItems.Where(a => a.IsDeleted == false && a.CompanyId == companyId)
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = string.Concat(a.Code, " - ", a.Name),
                                                   code = a.Code
                                               })
                                               .OrderByDescending(a => a.text)
                                               .FirstOrDefault();
                return Ok(items);
            }
            else
            {
                var items = _dbContext.InvItems.Where(a => a.IsDeleted == false && a.CompanyId == companyId && a.Id == id)
                                                   .Select(a => new
                                                   {
                                                       id = a.Id,
                                                       text = string.Concat(a.Code, " - ", a.Name),
                                                       code = a.Code
                                                   })
                                                   .FirstOrDefault();
                return Ok(items);
            }
        }

        public IActionResult GetItemsForStockLedger()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            InventoryHelper helper = new InventoryHelper(_dbContext, companyId);

            var items = _dbContext.InvItems.Where(a => a.IsDeleted == false && a.CompanyId == companyId)
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = string.Concat(a.Code, " - ", a.Name),
                                                   code = a.Code,
                                                   name = a.Name
                                               })
                                               .OrderBy(a => a.text)
                                               .Take(25)
                                               .ToList();
            return Ok(items);
        }


        #region Category Tree
        [HttpGet]
        public IEnumerable<AccountTree> GetItemCategory()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var categories = _dbContext.InvItemCategories.Include(c => c.Parent).Where(a => a.IsDeleted == false && a.CompanyId == companyId).OrderBy(a=>a.CategoryLevel);
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

        public class AccountTree
        {
            public int Id { get; set; }
            public string Parent { get; set; }
            public string Text { get; set; }
            public string Icon { get; set; }
        }
        #endregion Category Tree
    }


}