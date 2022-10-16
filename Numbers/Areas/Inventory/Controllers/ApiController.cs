using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Repository.Helpers;

namespace Numbers.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    public class ApiController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public ApiController(NumbersDbContext context)
        {
            _dbContext = context;
        }

        [HttpGet]
        public IActionResult GetItem(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var items = _dbContext.InvItems.Where(a => a.Id == id && a.CompanyId == companyId && a.IsDeleted == false)
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = string.Concat(a.Name)
                                               })
                                               .FirstOrDefault();
            return Ok(items);
        }

        public IActionResult GetCategories()
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
                    id = m.Id,
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
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult GetStockByItemBrandWise(int itemId, int warehouseId, DateTime stockDate,int brandId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            try
            {
                var Stock = _dbContext.VwInvLedgers
               .Where(s => s.ItemId == itemId && s.CompanyId == companyId && s.WareHouseId == warehouseId && s.TransDate <= stockDate && s.BrandId==brandId)
               .Select(s => s.Qty).DefaultIfEmpty(0).ToArray();
                var totalStock = Stock.Sum();
                return Ok(totalStock);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }


        [HttpPost]
        public IActionResult GetItemStockByBrandWarehouse(int itemId, int warehouseId, DateTime stockDate,int brandId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var configValues = new ConfigValues(_dbContext);

            try
            {
               // var Stock = _dbContext.VwInvLedgers
               //.Where(s => s.ItemId == itemId && s.CompanyId == companyId && s.WareHouseId == warehouseId && s.TransDate <= stockDate).Include(x=>x.Item).Select(p => new
               //{

               //    ItemId = p.ItemId,
               //    ItemName = p.Item.Name,
               //    ItemCode = p.Item.Code,
               //    Brand = p.Brand,
               //    UOM = p.Item.Unit,
               //    Qty =p.Qty,

               //}).ToList()
               //.ToList();
                var Stock = _dbContext.VwInvLedgers
               .Where(s => s.ItemId == itemId && s.CompanyId == companyId && s.WareHouseId == warehouseId && s.TransDate <= stockDate).Include(x=>x.Item).Where(p => p.Brand != "").GroupBy(p => p.Brand).Select(p => new
               {

                   ItemId = p.FirstOrDefault().ItemId,
                   ItemName = p.FirstOrDefault().Item.Name,
                   ItemCode = p.FirstOrDefault().Item.Code,
                   BrandId = p.FirstOrDefault().BrandId,
                   Brand = p.FirstOrDefault().Brand,
                   UOM =p.FirstOrDefault().Item.Unit,
                   Qty = p.Sum(p => p.Qty)

               }).ToList();

                //List<YarnIssueViewModel> list = new List<YarnIssueViewModel>();
                //foreach (var item in Stock)
                //{

                //    var model = new YarnIssueViewModel();
                //    model.ItemId = item.ItemId;
                //    model.ItemName = item.ItemName;
                //    model.ItemCode = item.Code;
                //    model.Brand = item.Brand;
                //    model.UOM = configValues.GetUom(item.Unit);
                //    model.Qty = item.Qty;
                //    list.Add(model);

                //}
                Stock.Where(p => p.Brand != "").GroupBy(p => p.Brand).Select(p => new
                {

                    ItemId = p.FirstOrDefault().ItemId,
                    ItemName = p.FirstOrDefault().ItemName,
                    ItemCode = p.FirstOrDefault().ItemCode,
                    BrandId = p.FirstOrDefault().BrandId,
                    Brand = p.FirstOrDefault().Brand,
                    UOM = configValues.GetUom(p.FirstOrDefault().UOM),
                    Qty = p.Sum(p => p.Qty)
                    
                }).ToList();

                return Ok(Stock);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }


        [HttpGet]
        public IActionResult GetItems()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;

            var items = _dbContext.InvItems.Where(a => a.IsDeleted == false && a.CompanyId == companyId)
                                             .Select(a => new
                                             {
                                                 id = a.Id,
                                                 text = string.Concat(a.Code, " - ", a.Name),
                                                 code = a.Code
                                             })
                                            .OrderBy(a => a.text).Take(100)
                                            .ToList();
            return Ok(items);

        }

        public IActionResult GetSuppliers()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var suppliers = _dbContext.APSuppliers.Where((c => c.CompanyId == companyId))
                                               .Select(c => new
                                               {
                                                   value = c.Id,
                                                   text = c.Name
                                               })
                                               .OrderBy(a => a.text)
                                               .ToList();
            return Ok(suppliers);
        }
        public IActionResult GetStores()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var configValues = new ConfigValues(_dbContext);
            var stores = configValues.GetConfigValues("Inventory", "Ware House", companyId).Select(c => new
            {
                value = c.Value,
                text = c.Text
            })
            .OrderBy(a => a.text)
            .ToList();
            return Ok(stores);
        }
        public IActionResult GetDepartments()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var departments = _dbContext.GLDivision.Where((c => c.CompanyId == companyId))
                                               .Select(c => new
                                               {
                                                   value = c.Name,
                                                   text = c.Name
                                               })
                                               .Take(25)
                                               .OrderBy(p=>p.text)
                                               .ToList();
            return Ok(departments);
        }
        [HttpGet]
        public IActionResult GetItems(int id, bool isFirst, bool isLast)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            if (isFirst)
            {
                var items = _dbContext.InvItems.Where(a => a.IsDeleted == false && a.CompanyId == companyId)
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

        [HttpGet]
        public IActionResult GetItemsForReport(int id, bool isFirst, bool isLast)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;

            var items = _dbContext.InvItems.Where(a => a.IsDeleted == false && a.CompanyId == companyId && a.IsActive == true)
                                             .Select(a => new
                                             {
                                                 id = a.Id,
                                                 text = string.Concat(a.Code, " - ", a.Name),
                                                 code = a.Code
                                             })
                                            .OrderBy(a => a.text)
                                            .ToList();
            return Ok(items);

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

        public IActionResult GetItemsCategoryByParentId(int ParentId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;

            var items = _dbContext.InvItemCategories.Where(a => a.IsDeleted == false && a.ParentId == ParentId)
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = a.Name
                                               })
                                               .OrderBy(a => a.text)
                                               .ToList();
            return Ok(items);
        }


        #region Category Tree
        [HttpGet]
        public IEnumerable<AccountTree> GetItemCategory()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var categories = _dbContext.InvItemCategories.Include(c => c.Parent).Where(a => a.IsDeleted == false /*&& a.CompanyId == companyId*/).OrderBy(a => a.CategoryLevel);
            List<AccountTree> trees = new List<AccountTree>();
            foreach (var category in categories)
            {
                AccountTree tree = new AccountTree();
                tree.Id = category.Id;
                tree.Parent = (category.ParentId == null ? "#" : category.ParentId.ToString());
                tree.Text = category.Code + "-" + category.Name;
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