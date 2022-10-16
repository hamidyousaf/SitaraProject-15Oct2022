using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Numbers.Entity.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Security.Claims;
using Numbers.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Numbers.Repository.Inventory;
using System.Linq.Dynamic.Core;
using Numbers.Repository.Helpers;
using Numbers.Entity.ViewModels;

namespace Numbers.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    [Authorize]
    public class ItemController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public ItemController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet]
        public IActionResult Index()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var itemRepo = new ItemRepo(_dbContext);
            IEnumerable<InvItem> items = itemRepo.GetAll(companyId);
            if (items != null)
            {
                ViewBag.NavbarHeading = "List of Items";
                return View(items);
            }
            TempData["error"] = "true";
            TempData["message"] = "There are no products in stock, You need to add products in order to view them.";
            ViewBag.NavbarHeading = "List of Items";
            return RedirectToAction(nameof(Create));
        }

        [HttpGet]
        public IActionResult Create(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var configValues = new ConfigValues(_dbContext);
            var itemRepo = new ItemRepo(_dbContext);
            InventoryHelper helper = new InventoryHelper(_dbContext,HttpContext.Session.GetInt32("CompanyId").Value);
          ////  ViewBag.listOfItemCategories = helper.GetCategoriesSelectLists();
          ViewBag.listOfItemCategories = new SelectList(from ac in _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.CategoryLevel==4 && x.CompanyId==companyId).OrderBy(x=>x.Code).ToList()
                                                        select new
                                                        {
                                                            Id = ac.Id,
                                                            Name = ac.Code+ " - " + ac.Name
                                                        }, "Id", "Name");
            ViewBag.UOMs = configValues.GetConfigValues("Inventory", "UOM", companyId);
            ViewBag.Season = configValues.GetConfigValues("Inventory", "Season", companyId);
            ViewBag.FabricConstruction = configValues.GetConfigValues("Inventory", "Fabric Construction", companyId);
            ViewBag.listOfItemAccounts = new SelectList(_dbContext.InvItemAccounts.Where(i=>i.CompanyId== HttpContext.Session.GetInt32("CompanyId").Value && i.IsDeleted==false).ToList(), "Id", "Name");
            ViewBag.listOfManufactures = configValues.GetConfigValues("Inventory", "Manufacture Company", companyId);
            ViewBag.listOfWareHouse = configValues.GetConfigValues("Inventory", "Ware House", companyId);
            ViewBag.listOfLocation = configValues.GetConfigValues("Inventory", "Location", companyId);
            //ViewBag.listOfManufactures = CommonHelper.getAppCompanyConfigLists(_dbContext, "Inventory", "Manufacture Company", HttpContext.Session.GetInt32("CompanyId").Value); 
            if (id == 0)
            {
                try
                {
                    var result = _dbContext.InvItems.Where(x => x.IsActive && x.CompanyId == companyId).ToList();
                    if (result.Count > 0)
                    {
                        string Code = _dbContext.InvItems.Select(x => x.Code).LastOrDefault();
                        string a = Code.Split('-')[1];
                        // string b = Code.Substring(a, 0);
                        int c = Convert.ToInt16(a) + 1;
                        string BarCode = _dbContext.InvItems.Select(x => x.Barcode).LastOrDefault().Split('-')[1];
                        //string e = Code.Substring(0, d);
                        int f = Convert.ToInt16(BarCode) + 1;
                        ViewBag.Code = Convert.ToDouble(c).ToString("000000");
                        ViewBag.Barcode = Convert.ToDouble(f).ToString("00000000000");

                    }
                    else
                    {
                        ViewBag.Code = "000001";
                        ViewBag.Barcode = "00000000001";
                    }
                    ViewBag.EntityState = "Create";
                    ViewBag.NavbarHeading = "Create Item";
                    //ViewBag.Barcode = itemRepo.GetBarCode(companyId);
                    //ViewBag.LastItemCode = itemRepo.GetLastItemCode(companyId);
                }
                catch (Exception e)
                {
                    
                }
                return View(new InvItem());
            }
            else
            {
                ViewBag.EntityState = "Update";
                ViewBag.NavbarHeading = "Update Item";
                InvItem item = itemRepo.GetById(id);
                //int a = item.Code.LastIndexOf('-');
                //string b = item.Code.Substring(0, a);
                //int c = Convert.ToInt16(b) + 1;
                //string CategoryName = _dbContext.InvItemCategories.FirstOrDefault(x => x.Id == item.CategoryId).Code;
                ////int d = CategoryName.LastIndexOf('-');
                ////string e = CategoryName.Substring(0, d);
                ////   item.IsPurchaseable = item.IsPurchaseable == true ? 1 :0;
                ViewBag.Code = item.Code;
                ViewBag.Barcode = item.Barcode;
                return View(item);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(InvItem invItem, IFormFile img)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var itemRepo = new ItemRepo(_dbContext);

            if (invItem.Id == 0)
            {
                if (_dbContext.InvItems.Any(c => c.Code == invItem.Code && c.IsDeleted == false && c.CompanyId == invItem.CompanyId))
                {
                    TempData["error"] = "true";
                    TempData["message"] = string.Format("Item Code.{0}-{1} already exists", invItem.Name, invItem.Code);
                    return View(invItem);
                }
                else
                {
                    invItem.CompanyId = companyId;
                    invItem.CreatedBy = userId;
                    bool isSuccess = await itemRepo.Create(invItem, img);
                    if (isSuccess == true)
                    {
                        TempData["error"] = "false";
                        TempData["message"] = string.Format("Item Name. {0}-{1} has been added successfully", invItem.Name, invItem.Code);
                    }
                    else
                    {
                        TempData["error"] = "true";
                        TempData["message"] = "Something went wrong.";
                    }
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                invItem.CompanyId = companyId;
                invItem.UpdatedBy = userId;
                bool isSuccess = await itemRepo.Update(invItem, img);
              //  IsDuplicateCode("invItem.Code", invItem.Id);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = string.Format("Item Name. {0}-{1} has been updated successfully", invItem.Name, invItem.Code);
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Something went wrong.";
                }
                return RedirectToAction(nameof(Index));
            }
        }

        //private bool IsDuplicateCode(string code, int id)
        //{
        //    // bool isSuccess =ItemRepo.IsDuplicateCode(code,id);
        //    int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
        //    var items = _dbContext.InvItems.Any(c => c.Code == code && c.Id != id && c.IsDeleted == false && c.CompanyId == companyId);
        //    if (items == true)
        //        return true;
        //    else
        //        return false;
        //}
        public async Task<IActionResult> Delete(int id)
        {
            var itemRepo = new ItemRepo(_dbContext);
            bool isSuccess = await itemRepo.Delete(id);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Item has been deleted successfully";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Item not found";
            }
            return RedirectToAction(nameof(Index)); 
        }

        public JsonResult checkProductCodeAlreadyExists(string code)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            System.Threading.Thread.Sleep(200);
            if (code == "0")
                return Json(0);
            var chkCode = _dbContext.InvItems.Where(a => a.IsActive == true && a.IsDeleted == false && a.Code == code && a.CompanyId == companyId).FirstOrDefault();
            return Json(chkCode == null ? 0 : 1);
        }
        [HttpGet]
        public IActionResult GetItems(string q = "")
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var items = _dbContext.InvItems.Where(a => a.CompanyId == companyId && a.IsDeleted == false && a.IsActive == true)
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   name = a.Name
                                               });
            return Ok(items);
        }
        public IActionResult GetList()
        {
            try
            {
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                var configValues = new ConfigValues(_dbContext);
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                //var searchValue = Request.Form["search[value]"].FirstOrDefault();

                var searchItemCode = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchItemName = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchBarcode = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchUOM = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchMake = Request.Form["columns[4][search][value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var InvData = (from Inv in _dbContext.InvItems.Include(x=>x.UOM).Where(x => x.IsDeleted == false) select Inv);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    InvData = InvData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                InvData = !string.IsNullOrEmpty(searchItemCode) ? InvData.Where(m => m.Code.ToString().ToUpper().Contains(searchItemCode.ToUpper())) : InvData;
                InvData = !string.IsNullOrEmpty(searchItemName) ? InvData.Where(m => m.Name.ToString().ToUpper().Contains(searchItemName.ToUpper())) : InvData;
                InvData = !string.IsNullOrEmpty(searchBarcode) ? InvData.Where(m => m.Barcode.ToString().ToUpper().Contains(searchBarcode.ToUpper())) : InvData;
                InvData = !string.IsNullOrEmpty(searchUOM) ? InvData.Where(m => m.UOM.ConfigValue.ToString().ToUpper().Contains(searchUOM.ToUpper())) : InvData;
                InvData = !string.IsNullOrEmpty(searchMake) ? InvData.Where(m => m.Make.ToString().ToUpper().Contains(searchMake.ToUpper())) : InvData;

                recordsTotal = InvData.Count();
                var data = InvData.ToList();
                if (pageSize == -1)
                {
                    data = InvData.Skip(skip).Take(recordsTotal == 0 ? 1 : pageSize == -1 ? recordsTotal : 1).ToList();
                }
                else
                {
                    data = InvData.Skip(skip).Take(pageSize).ToList();
                }

                List<ItemListViewModel> Details = new List<ItemListViewModel>();
                foreach (var grp in data)
                {
                    ItemListViewModel itemListViewModel = new ItemListViewModel();
                    itemListViewModel.InvItems = grp;
                    Details.Add(itemListViewModel);
                }

                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = Details };
                return Ok(jsonData);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IActionResult GetCode(int ItemCategoryId, int InvItemId)
        {
            var fourthCategoryCode = _dbContext.InvItemCategories.FirstOrDefault(x => x.Id == ItemCategoryId && !x.IsDeleted && x.CategoryLevel == 4).Code;
            var secondCategoryCode = fourthCategoryCode.Substring(0, 5);
            if (InvItemId == 0)
            {
                string codeMax = "000001";
                string barcodeMax = "00000000001";

                var InvItems = _dbContext.InvItems.Where(x => !x.IsDeleted).Select(x => new { x.Code, x.Barcode }).ToList();

                List<int> codeList = new List<int>();
                List<int> barCode = new List<int>();
            
                if (InvItems.Count > 0)
                {
                    foreach (var item in InvItems)
                    {
                        var x = Convert.ToInt32(item.Code.Split("-")[1]);
                        var y = Convert.ToInt32(item.Barcode.Split("-")[1]);
                        codeList.Add(x);
                        barCode.Add(y);
                    }
                    codeMax = (codeList.Max() + 1).ToString("000000");
                    barcodeMax = (barCode.Max() + 1).ToString("00000000000");
                }

                var a = $"{fourthCategoryCode} -{codeMax}";
                var b = $"{secondCategoryCode}-{barcodeMax}";

                ListOfValue value = new ListOfValue() {
                    Code = a,
                    Barcode = b
                };
                return Ok(value);
            }
            else
            {
                var InvItems = _dbContext.InvItems.Where(x => !x.IsDeleted && x.Id == InvItemId).Select(x => new { x.Code, x.Barcode }).FirstOrDefault();

                var x = (InvItems.Code.Split("-")[1]);
                var y = (InvItems.Barcode.Split("-")[1]);

                var a = $"{fourthCategoryCode} -{x}";
                var b = $"{secondCategoryCode}-{y}";

                ListOfValue value = new ListOfValue()
                {
                    Code = a,
                    Barcode = b
                };
                return Ok(value);
            }
        }
    }
}