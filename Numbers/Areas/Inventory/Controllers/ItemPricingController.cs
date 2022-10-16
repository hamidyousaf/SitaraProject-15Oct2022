using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Repository.Helpers;
using Numbers.Repository.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Numbers.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Numbers.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    [Authorize]
    public class ItemPricingController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        private readonly ItemPricingDetailsRepository _itemPricingDetailsRepository;
        private readonly ItemPricingsRepository _itemPricingsRepository;
        public ItemPricingController(NumbersDbContext context, ItemPricingsRepository itemPricingsRepository, ItemPricingDetailsRepository itemPricingDetailsRepository)
        {
            _dbContext = context;
            _itemPricingDetailsRepository = itemPricingDetailsRepository;
            _itemPricingsRepository = itemPricingsRepository;

        }
        public IActionResult Index()
        {
            ViewBag.NavbarHeading = "List of Pricings";
            return View();
        }
        public IActionResult Create(int? id)
        {
            ViewBag.EntityState = "Create";
            ViewBag.NavbarHeading = "Create Item Pricing";
            var configValues = new ConfigValues(_dbContext);
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            ItemPricingVM itemPricingVM = new ItemPricingVM();


            ViewBag.Season = configValues.GetConfigValues("Inventory", "Season", companyId);
            if (id.HasValue)
            {
                
                itemPricingVM.itemPricing = _itemPricingsRepository.Get(x => x.ID == id).FirstOrDefault();
                itemPricingVM.itemPricingDetails = _itemPricingDetailsRepository.Get(x => x.ItemPrice_Id == id && x.IsDelete == false).ToList();
                itemPricingVM.itemPricingList = new List<PricingDetailVM>();
                ViewBag.ProductType = new SelectList(_dbContext.AppCompanyConfigs.Where(x => x.Id == itemPricingVM.itemPricing.ProductType_Id), "Id", "ConfigValue");
                ViewBag.Itemfouth = _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.CategoryLevel == 4 && x.CompanyId == companyId).OrderBy(x => x.Code).ToList();
                ViewBag.Category = _dbContext.InvItems.Where(x => x.IsActive == true && x.IsDeleted == false).ToList();
                ViewBag.UOM = _dbContext.AppCompanyConfigs.Where(x => x.IsActive == true && x.IsDeleted == false && x.Module == "Inventory" && x.ConfigName == "UOM" && x.CompanyId == companyId).ToList();
                var items = _dbContext.AppCompanySetups.Where(x => x.Name == "Local Finish ItemCode").FirstOrDefault().Value;


                ViewBag.listOfItemCategories3 = new SelectList(from ac in _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.Id == itemPricingVM.itemPricing.ItemCategory_Id3  && x.Code.StartsWith(items)).OrderBy(x => x.Code).ToList()
                                                               select new
                                                               {
                                                                   Id = ac.Id,
                                                                   Name = ac.Code + " - " + ac.Name
                                                               }, "Id", "Name");
                foreach (var list in itemPricingVM.itemPricingDetails)
                {
                    PricingDetailVM item = new PricingDetailVM();
                    item.ID = list.ID;
                    item.Item_ThirdLevel = list.ItemID_ThirdLevel.ToString();
                    var Item = _dbContext.InvItemCategories.FirstOrDefault(x => x.Id == list.ItemID_FourthLevel && x.CategoryLevel == 4);
                    item.Item = string.Concat(Item.Code, " - ", Item.Name);
                    item.ItemId = list.ItemID_FourthLevel;
                    item.Season = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.IsActive == true && x.IsDeleted == false && x.Id == list.Season).ConfigValue;
                    item.SeasonId = list.Season;
                    item.Price = list.ItemPrice;
                    item.Price_StartDate = list.Price_StartDate.ToString(Helpers.CommonHelper.DateFormat);
                    item.Price_EndDate = list.Price_EndDate.ToString(Helpers.CommonHelper.DateFormat);
                    item.Discount = list.ItemDiscount;
                    item.DiscountAmount = list.DiscountAmount;
                    item.Dis_StartDate = list.Dis_StartDate.ToString(Helpers.CommonHelper.DateFormat);
                    item.Dis_EndDate = list.Dis_EndDate.ToString(Helpers.CommonHelper.DateFormat);
                    try
                    {
                        var UOM1 = (from c in _dbContext.InvItems.Where(x => x.CategoryId == list.ItemID_FourthLevel)
                                    join b in _dbContext.AppCompanyConfigs on c.Unit equals b.Id
                                    where b.IsDeleted == false && b.IsActive == true
                                    select b.ConfigValue).FirstOrDefault();
                        item.UOM1 = UOM1;
                    }
                    catch
                    {
                        item.UOM1 = " ";
                    }
                    try
                    {
                        var UOM2 = (from c in _dbContext.InvItems.Where(x => x.CategoryId == list.ItemID_FourthLevel)
                                    join b in _dbContext.AppCompanyConfigs on c.PackUnit equals b.Id
                                    where b.IsDeleted == false && b.IsActive == true
                                    select b.ConfigValue).FirstOrDefault();
                        item.UOM2 = UOM2;
                    }
                    catch
                    {
                        item.UOM2 = " ";
                    }
                    itemPricingVM.itemPricingList.Add(item);
                }
            }
            else
            {
                //ViewBag.Tran = MaxNumber();
                ViewBag.ProductType = configValues.GetConfigValues("AR", "Product Type", companyId);
                var items = _dbContext.AppCompanySetups.Where(x => x.Name == "Local Finish ItemCode").FirstOrDefault().Value;

                ViewBag.listOfItemCategories = new SelectList(from ac in _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.CategoryLevel == 2 /*&& x.CompanyId == companyId */&& x.Code.StartsWith(items)).OrderBy(x => x.Code).ToList()
                                                              select new
                                                              {
                                                                  Id = ac.Id,
                                                                  Name = ac.Code + " - " + ac.Name
                                                              }, "Id", "Name");
                ViewBag.listOfItemCategories3 = new SelectList(from ac in _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.CategoryLevel == 3 /*&& x.CompanyId == companyId*/ && x.Code.StartsWith(items)).OrderBy(x => x.Code).ToList()
                                                               select new
                                                               {
                                                                   Id = ac.Id,
                                                                   Name = ac.Code + " - " + ac.Name
                                                               }, "Id", "Name");
            }

            return View(itemPricingVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ItemPricingVM itemPricingVM, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            ItemPricingDetails[] itemDetail = JsonConvert.DeserializeObject<ItemPricingDetails[]>(collection["detail"]);

            if (itemPricingVM.itemPricing.ID == 0)
            {
                itemPricingVM.itemPricing.CompanyId = companyId;
                itemPricingVM.itemPricing.CreatedBy = userId;
                itemPricingVM.itemPricing.CreatedDate = DateTime.Now;
                itemPricingVM.itemPricing.Resp_ID = resp_Id;
                itemPricingVM.itemPricing.IsActive = true;
                itemPricingVM.itemPricing.IsDelete = false;
                itemPricingVM.itemPricing.Status = "Created";
                itemPricingVM.itemPricing.TrancationNo = MaxNumber();
                await _itemPricingsRepository.CreateAsync(itemPricingVM.itemPricing);
                //Monthly Target
                foreach (var item in itemDetail)
                {
                    ItemPricingDetails itemPricingDetails = new ItemPricingDetails();
                    itemPricingDetails = item;
                    itemPricingDetails.ItemPrice_Id = itemPricingVM.itemPricing.ID;
                    itemPricingDetails.IsDelete = false;
                    await _itemPricingDetailsRepository.CreateAsync(itemPricingDetails);
                }
            }
            else
            {
                ItemPricings itemPricings = _itemPricingsRepository.Get(x => x.ID == itemPricingVM.itemPricing.ID).FirstOrDefault();
                itemPricings.ItemCategory_Id = itemPricings.ItemCategory_Id;
                itemPricings.ProductType_Id = itemPricings.ProductType_Id;
                itemPricings.IsActive = true;
                itemPricings.IsDelete = false;
                itemPricings.UpdatedBy = userId;
                itemPricings.UpdatedDate = DateTime.Now;
                _dbContext.ItemPricings.Update(itemPricings);
                var existingItemCate = _itemPricingDetailsRepository.Get(x => x.ItemPrice_Id == itemPricingVM.itemPricing.ID).ToList();
                //Deleting monthly target limit
                foreach (var item in existingItemCate)
                {
                    bool isExist = itemDetail.Any(x => x.ID == item.ID);
                    if (!isExist)
                    {
                        await _itemPricingDetailsRepository.DeleteAsync(item);
                        _dbContext.SaveChanges();
                    }
                }
                //Inserting/Updating annual limit
                foreach (var item in itemDetail)
                {
                    if (item.ID == 0) //Inserting New Records
                    {
                        ItemPricingDetails itemPricingDetails = new ItemPricingDetails();
                        itemPricingDetails = item;
                        itemPricingDetails.ItemPrice_Id = itemPricingVM.itemPricing.ID;
                        itemPricingDetails.IsDelete = false;
                        await _itemPricingDetailsRepository.CreateAsync(itemPricingDetails);
                    }
                    else   //Updating Records
                    {
                        var itemdetail = _itemPricingDetailsRepository.Get(x => x.ID == item.ID).FirstOrDefault();
                        itemdetail.ItemID_FourthLevel = item.ItemID_FourthLevel;
                        itemdetail.ItemID_ThirdLevel = item.ItemID_ThirdLevel;
                        itemdetail.ItemPrice = item.ItemPrice;
                        itemdetail.Price_StartDate = item.Price_StartDate;
                        itemdetail.Price_EndDate = item.Price_EndDate;
                        itemdetail.ItemDiscount = item.ItemDiscount;
                        itemdetail.Dis_StartDate = item.Dis_StartDate;
                        itemdetail.Dis_EndDate = item.Dis_EndDate;
                        itemdetail.IsDelete = false;
                        _dbContext.ItemPricingDetails.Update(itemdetail);
                    }
                }
            }
            _dbContext.SaveChanges();
            return RedirectToAction("Create");
        }

        [HttpPost]
        public IActionResult GetItemDetails()
        {
            try
            {
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                //var searchValue = Request.Form["search[value]"].FirstOrDefault();
                var searchTransNo = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchItem = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchSeason = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchPrice = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchStart = Request.Form["columns[4][search][value]"].FirstOrDefault();
                var searchEnd = Request.Form["columns[5][search][value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                sortColumn = "ID";
                var itemdetail = _dbContext.ItemPricingDetails.Include(p => p.ItemPricing).Where(p => p.ItemPricing.CompanyId == companyId && p.ItemPricing.IsDelete == false && p.IsDelete == false);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    itemdetail = itemdetail.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                //if (!string.IsNullOrEmpty(searchValue))
                //{
                //    /*itemdetail = itemdetail.Where(m => m..Contains(searchValue)
                //                                    || m.EmployeeCode.Contains(searchValue)
                //                                    || m.Designation.Contains(searchValue)
                //                                    || m.Department.Contains(searchValue)
                //                                    || m.Comission.ToString().Contains(searchValue)
                //                                  );*/

                //}
                itemdetail = !string.IsNullOrEmpty(searchTransNo) ? itemdetail.Where(m => _dbContext.ItemPricings.FirstOrDefault(x => x.ID == m.ItemPrice_Id).TrancationNo.ToString().Contains(searchTransNo)) : itemdetail;
                itemdetail = !string.IsNullOrEmpty(searchItem) ? itemdetail.Where(m => _dbContext.InvItemCategories.FirstOrDefault(x => x.Id == Convert.ToInt32(m.ItemID_FourthLevel) && x.CategoryLevel == 4).Name.ToUpper().Contains(searchItem.ToUpper()) || _dbContext.InvItemCategories.FirstOrDefault(x => x.Id == Convert.ToInt32(m.ItemID_FourthLevel) && x.CategoryLevel == 4).Code.ToUpper().Contains(searchItem.ToUpper())) : itemdetail;
                itemdetail = !string.IsNullOrEmpty(searchSeason) ? itemdetail.Where(m => _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.IsActive == true && x.IsDeleted == false && x.Id == m.Season).ConfigValue.ToUpper().Contains(searchSeason.ToUpper())) : itemdetail;
                itemdetail = !string.IsNullOrEmpty(searchPrice) ? itemdetail.Where(m => m.ItemPrice.ToString().ToUpper().Contains(searchPrice.ToUpper())) : itemdetail;
                itemdetail = !string.IsNullOrEmpty(searchStart) ? itemdetail.Where(m => m.Price_StartDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchStart.ToUpper())) : itemdetail;
                itemdetail = !string.IsNullOrEmpty(searchEnd) ? itemdetail.Where(m => m.Price_EndDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchEnd.ToUpper())) : itemdetail;

                recordsTotal = itemdetail.Count();
                var data = itemdetail.ToList();
                if (pageSize == -1)
                {
                    data = itemdetail.Skip(skip).Take(recordsTotal == 0 ? 1 : pageSize == -1 ? recordsTotal : 1).ToList();
                }
                else
                {
                    data = itemdetail.Skip(skip).Take(pageSize).ToList();
                }
                List<PricingDetailVM> Details = new List<PricingDetailVM>();
                foreach (var grp in data)
                {
                    PricingDetailVM itemPricingDetails = new PricingDetailVM();
                    itemPricingDetails.ItemPrice_Id = grp.ItemPrice_Id;
                    //itemPricingDetails.ItemID_ThirdLevel = _dbContext.InvItemCategories.FirstOrDefault(x => x.Id == grp.ItemID_ThirdLevel&&x.CategoryLevel==3).Code;
                    var Item = _dbContext.InvItemCategories.FirstOrDefault(x => x.Id == grp.ItemID_FourthLevel && x.CategoryLevel == 4);
                    itemPricingDetails.Item = string.Concat(Item.Code, " - ", Item.Name);
                    itemPricingDetails.Season = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.IsActive == true && x.IsDeleted == false && x.Id == grp.Season).ConfigValue;
                    itemPricingDetails.Price = grp.ItemPrice;
                    itemPricingDetails.Price_StartDate = grp.Price_StartDate.ToString(Helpers.CommonHelper.DateFormat);
                    itemPricingDetails.Price_EndDate = grp.Price_EndDate.ToString(Helpers.CommonHelper.DateFormat);
                    itemPricingDetails.Discount = grp.DiscountAmount;
                    itemPricingDetails.Dis_StartDate = grp.Dis_StartDate.ToString(Helpers.CommonHelper.DateFormat);
                    itemPricingDetails.Dis_EndDate = grp.Dis_EndDate.ToString(Helpers.CommonHelper.DateFormat);
                    itemPricingDetails.ItemPricings = _dbContext.ItemPricings.FirstOrDefault(x => x.ID == grp.ItemPrice_Id);

                    Details.Add(itemPricingDetails);

                }
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = Details };
                return Ok(jsonData);
            }
            catch (Exception)
            {
                throw;
            }

        }


        [HttpGet]
        public IActionResult GetItem(int Id, int level)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            if (level == 3)
            {
                var item = _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.CategoryLevel == 3 && x.ParentId == Id /*&& x.CompanyId == companyId*/).OrderBy(x => x.Code).ToList().
                           Select(a => new
                           {
                               id = a.Id,
                               text = string.Concat(a.Code, " - ", a.Name)
                           });
                return Ok(item);
            }
            else
            {
                var item = _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.CategoryLevel == 4 && x.ParentId == Id /*&& x.CompanyId == companyId*/).OrderBy(x => x.Code).ToList().
                            Select(a => new
                            {
                                id = a.Id,
                                text = string.Concat(a.Code, " - ", a.Name)
                            });
                return Ok(item);
            }
        }

        [HttpGet]
        public IActionResult GetItems(int Id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
           
                var item = _dbContext.InvItems.Where(x => x.IsDeleted == false && x.CompanyId == companyId && x.CategoryId==Id).OrderBy(x => x.Code).ToList().
                           Select(a => new
                           {
                               id = a.Id,
                               text = string.Concat(a.Code, " - ", a.Name)
                           });
                return Ok(item);
           
        }

        [HttpGet]
        public IActionResult GetUOM(int Id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var ItemPricingDetails = _dbContext.ItemPricingDetails.Include(p => p.ItemPricing).Where(p => p.ItemID_FourthLevel == Id && p.ItemPricing.CompanyId == companyId);

            if (ItemPricingDetails.Any(p => p.ItemPricing.IsClosed != true))
            {
                if (ItemPricingDetails.Any(p => p.Price_EndDate.Date < DateTime.Now.Date && p.ItemPricing.IsClosed != true))
                {
                    var ItemPrice = ItemPricingDetails.Where(p => p.ItemID_FourthLevel == Id && p.ItemPricing.IsClosed != true).FirstOrDefault().ItemPricing;
                    ItemPrice.IsClosed = true;
                    ItemPrice.Status = "Closed";
                    _dbContext.ItemPricings.Update(ItemPrice);
                    _dbContext.SaveChanges();
                }
                else
                {
                    return Json(true);
                }
            }

            var Item = _dbContext.InvItems.FirstOrDefault(x => x.CategoryId == Id);
            UOM uOM = new UOM();
            if (Item != null)
            {
                var UOM1 = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.IsActive == true && x.IsDeleted == false && x.Id == Item.Unit);
                uOM.UOM1 = UOM1.ConfigValue;
                uOM.UOM2 = "";
                if (Item.PackUnit != 0)
                {
                    var UOM2 = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.IsActive == true && x.IsDeleted == false && x.Id == Item.PackUnit);
                    uOM.UOM2 = UOM2.ConfigValue;
                }
            }
            return Ok(uOM);
        }

        private int MaxNumber()
        {
            var result = _itemPricingsRepository.Get(x => x.IsActive).ToList();
            if (result.Count > 0)
            {
                return _dbContext.ItemPricings.Select(x => x.TrancationNo).Max() + 1;
            }
            else
            {
                return 1;
            }
        }

        public IActionResult Delete(int id)
        {
            var itemPricing = _dbContext.ItemPricings.Where(x => x.ID == id).FirstOrDefault();
            itemPricing.IsDelete = true;
            _dbContext.ItemPricings.Update(itemPricing);
            _dbContext.SaveChanges();
            var itemOricingItems = _dbContext.ItemPricingDetails.Where(p => p.ItemPrice_Id == id && p.IsDelete == false).ToList();
            foreach (var item in itemOricingItems)
            {
                item.IsDelete = true;
                _dbContext.ItemPricingDetails.Update(item);
                _dbContext.SaveChanges();
            }
            TempData["error"] = "false";
            TempData["message"] = "Record Deleted Successfully";
            return RedirectToAction("Index");
        }

        public IActionResult Close(int id)
        {
            var itemPricing = _dbContext.ItemPricings.Where(x => x.ID == id).FirstOrDefault();
            itemPricing.IsClosed = true;
            itemPricing.Status = "Closed";
            _dbContext.ItemPricings.Update(itemPricing);
            _dbContext.SaveChanges();
            TempData["error"] = "false";
            TempData["message"] = "Record Closed Successfully";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            var itemPricings = _dbContext.ItemPricings
                .Include(i => i.ProductType)
                .Include(i => i.InvItemSecond)
                .Where(i => i.ID == id && i.CompanyId == companyId && i.IsActive != false && i.IsDelete != true && i.Resp_ID == resp_Id).FirstOrDefault();
            var itemPricingDetails = _dbContext.ItemPricingDetails
                                .Include(i => i.InvItemThird)
                                .Include(i => i.InvItemFourth)
                                .Include(i => i.SeasonName)
                                .Where(i => i.ItemPrice_Id == id && i.IsDelete == false)
                                .ToList();
            ViewBag.NavbarHeading = "Item Pricings";
            ViewBag.TitleStatus = "Detail";

            TempData["Detail"] = itemPricingDetails;
            return View(itemPricings);
        }
    }
}

