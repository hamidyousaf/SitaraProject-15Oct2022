using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using Numbers.Repository.Helpers;

namespace Numbers.Areas.Planning.Controllers
{
    [Area("Planning")]
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
       
  
        [HttpGet]
        public IActionResult GetWeftWarp(string q = "")
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var YarnItems = _dbContext.AppCompanySetups.Where(x => x.Name == "Yarn Item Code").FirstOrDefault().Value;
            var items = _dbContext.InvItems.Where(
                                                (a => (a.Code.Contains(q) || a.Name.Contains(q)) && a.CompanyId == companyId && a.Code.StartsWith(YarnItems)
                                               ))
                                               .Include(a => a.Category)
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   code = a.Code,
                                                   text = a.Code + " - " + a.Name,
                                               })
                                               .OrderBy(a => a.code)
                                               .Take(25)
                                               .ToList();
            return Ok(items);
        }


        [HttpGet]
        public IActionResult GetGreigeConstruction(int qualityId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var items = _dbContext.GRQuality.Include(x => x.GRConstruction).ThenInclude(x=>x.Warp).Include(x=>x.GRConstruction).ThenInclude(x=>x.Weft).Where(x => x.Id == qualityId).FirstOrDefault();
            return Ok(items);
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

       
        public IActionResult GetItemsCategoryByParentId(int ParentId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;

            var items = _dbContext.InvItemCategories.Where(a => a.IsDeleted == false && a.CompanyId == companyId && a.ParentId == ParentId)
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = a.Name
                                               })
                                               .OrderBy(a => a.text)
                                               .ToList();
            return Ok(items);
        }
        [HttpGet]
        public IActionResult GetCustomerAddress(int Id)
        {
            if (Id != 0)
            {
                var address = _dbContext.ARCustomers.FirstOrDefault(x => x.Id == Id).Address;
                if (address != null)
                {
                    return Ok(address);
                }
                return Ok(null);
            }
            return Ok(null);
        }
        [HttpGet]
        public SelectList GetWeavingContract(int companyId)
        {
            var WeavingContract = new SelectList(_dbContext.GRWeavingContracts.Include(x=>x.Vendor).ToList().Where(s => s.IsDeleted != true && s.CompanyId == companyId)
                .Select(x => new ListOfValue{ 
                    Id = x.Id,
                    Name = x.TransactionNo + " - " + x.Vendor.Name 
                }), "Id" , "Name");
            return WeavingContract;
        }

        [HttpGet]
        public SelectList GetFolding(int companyId)
        {
            var WeavingContract = new SelectList(_dbContext.GRFolding.ToList().Where(s => s.IsDeleted != true && s.CompanyId == companyId)
                .Select(x => new ListOfValue
                {
                    Id = x.Id,
                    Name = x.FoldingNo.ToString()
                }), "Id", "Name"); ;
            return WeavingContract;
        }
        [HttpGet]
        public SelectList GetGRN(int companyId)
        {
            var WeavingContract = new SelectList(_dbContext.GRGRNS.ToList().Where(s => s.IsDeleted != true && s.CompanyId == companyId)
                .Select(x => new ListOfValue
                {
                    Id = x.Id,
                    Name = x.TransactionNo.ToString()
                }), "Id", "Name"); ;
            return WeavingContract;
        }
        [HttpGet]
        public SelectList PurchaseContractLOV(int companyId)
        {
            var PurchaseContract = new SelectList(_dbContext.GRPurchaseContract.Include(x => x.Vendor).ToList().Where(s => s.IsDeleted != true && s.CompanyId == companyId)
                .Select(x => new ListOfValue
                {
                    Id = x.Id,
                    Name = x.ContractNo + " - " + x.Vendor.Name
                }), "Id", "Name");
            return PurchaseContract;
        }
         
    }
}
