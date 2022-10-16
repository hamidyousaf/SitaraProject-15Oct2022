using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Repository.Helpers;
using System.Linq.Dynamic.Core;
using Numbers.Repository.Inventory;
using Numbers.Helpers;

namespace Numbers.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    [Authorize]
    public class StockTransferController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public StockTransferController(NumbersDbContext context)
        {
            _dbContext = context;
        }
        public IActionResult Index()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userid = HttpContext.Session.GetString("UserId");
            var stockTransferRepo = new StockTransferRepo(_dbContext);
            IEnumerable<InvStockTransfer> list = stockTransferRepo.GetAll(companyId);
            ViewBag.NavbarHeading = "List of Stock Transfers";
            return View(list);
        }

        public IActionResult Create(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var stockTransferRepo = new StockTransferRepo(_dbContext);
            var configValues = new ConfigValues(_dbContext);
            ViewBag.Counter = 0;
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            List<InvItemCategories> ItemCategory = _dbContext.Sys_ResponsibilityItemCategory.Include(x => x.ItemCategory).Where(x => x.ResponsibilityId == resp_Id).Select(x => x.ItemCategory).ToList();
            List<InvItem> ItemByResp = new List<InvItem>();
            foreach (InvItemCategories item in ItemCategory)
            {
                var itemList = _dbContext.InvItems.Include(x => x.Category).Where(x => /*x.CompanyId == companyId &&*/ x.IsDeleted == false && x.Category.Code.StartsWith(item.Code)).ToList();

                ItemByResp.AddRange(itemList.ToList());
            }
            ViewBag.Items = new SelectList(
                ItemByResp.ToList().Select(a => new
                {
                    id = a.Id,
                    text = string.Concat(a.Code, " - ", a.Name)
                }).ToList(), "id", "text");

            string userId = HttpContext.Session.GetString("UserId");
            //TempData["ShowRate"] = (from a in _dbContext.Users.Where(x => x.Id == userId) select a.ShowRate).FirstOrDefault();
            //ViewBag.Branch = new CommonDDL(_dbContext).GetBranchesbyId(userId);
            ViewBag.WareHouseTo = configValues.GetConfigValues("Inventory", "Ware House", companyId);
            ViewBag.WareHouseFrom = configValues.GetConfigValues("Inventory", "Ware House", companyId);
            if (id == 0)
            {
                var result = _dbContext.InvStockTransfers.Where(x => x.IsDeleted == false).ToList();
                if (result.Count > 0)
                {
                    ViewBag.Id = _dbContext.InvStockTransfers.Select(x => x.TransferNo).Max() + 1;
                }
                else
                {
                    ViewBag.Id = 1;
                }
                TempData["Mode"] = false;
                ViewBag.EntityState = "Create";
                ViewBag.NavbarHeading = "Create Stock Transfer";
               // TempData["TransferNo"] = stockTransferRepo.StockTransferCountNo(companyId);
                var model = new InvStockTransferViewModel();
                return View(model);
            }
            else
            {
                TempData["Mode"] = true;
                ViewBag.Id = id;
                InvStockTransferViewModel modelEdit = stockTransferRepo.GetById(id);
                InvStockTransferItem[] stockTransferItems = stockTransferRepo.GetStockTransferItems(id);
                modelEdit.InvStockTransferItems = _dbContext.InvStockTransferItems
                                       .Include(i => i.Item)
                                       .Include(i => i.StockTransfer)
                                         .Where(i => i.StockTransferId == id && i.IsDeleted == false)
                                         .ToList();
                foreach (var item in modelEdit.InvStockTransferItems)
                {
                    item.ItemName = item.Item.Code + " - " + item.Item.Name;
                    item.ItemRate = item.Item.AvgRate;
                    item.ItemAmount = item.Item.AvgRate * item.Qty;
                    //var obj = _dbContext.InvItems.Where(x => x.Id == id && x.IsDeleted == false).FirstOrDefault();
                    item.UnitName = (from a in _dbContext.AppCompanyConfigs.Where(x => x.Id == item.Item.Unit) select a.ConfigValue).FirstOrDefault();
                }
                //ViewBag.Items = stockTransferItems;
                //  TempData["TransferNo"] = modelEdit.TransferNo;

                if (modelEdit.Status != "Approved")
                {
                    ViewBag.EntityState = "Update";
                    ViewBag.NavbarHeading = "Stock Transfer";
                    ViewBag.TitleStatus = "Created";
                }
                return View(modelEdit);
            }
        }
        public JsonResult checkProductCodeAlreadyExists(int code)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            System.Threading.Thread.Sleep(200);
            if (code == 0)
                return Json(0);
            var chkCode = _dbContext.InvStockTransfers.Where(a => a.IsDeleted == false && a.TransferNo == code && a.CompanyId == companyId).FirstOrDefault();
            return Json(chkCode == null ? 0 : 1);
        }

        [HttpPost]
        public async Task<IActionResult> Create(InvStockTransferViewModel model, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var stockTransferRepo = new StockTransferRepo(_dbContext);
            if (model.Id == 0)
            {
                //for master table
                var stockTransfer = new InvStockTransfer();
                stockTransfer.TransferNo = model.TransferNo;
                stockTransfer.TransferDate = model.TransferDate;
                stockTransfer.WareHouseToId = model.WareHouseToId;
                stockTransfer.WareHouseFromId = model.WareHouseFromId;
                stockTransfer.Remarks = (collection["Remarks"][0]);
                stockTransfer.CompanyId = companyId;
                stockTransfer.IsDeleted = false;
                stockTransfer.Status = "Created";
                stockTransfer.CreatedBy = model.CreatedBy;
                stockTransfer.CreatedDate = DateTime.Now;
                _dbContext.InvStockTransfers.Add(stockTransfer);
                await _dbContext.SaveChangesAsync();
                //for detail table
                for (int i = 0; i < collection["ItemId"].Count; i++)
                {
                    var stockTransferItem = new InvStockTransferItem();
                    stockTransferItem.StockTransferId = stockTransfer.Id;
                    stockTransferItem.ItemId = Convert.ToInt32(collection["ItemId"][i]);
                    stockTransferItem.Qty = Convert.ToDecimal(collection["Qty"][i]);
                    stockTransferItem.IsDeleted = false;
                    stockTransferItem.CreatedBy = model.CreatedBy;
                    stockTransferItem.CreatedDate = DateTime.Now;
                    _dbContext.InvStockTransferItems.Add(stockTransferItem);
                    await _dbContext.SaveChangesAsync();
                }
                
                return RedirectToAction(nameof(Index));
            }
            else
            {
                model.UpdatedBy = userId;
                model.CompanyId = companyId;
                //for partial-items removal
                //string[] idsDeleted = Convert.ToString(collection["IdsDeleted"]).Split(",");
                //if (!idsDeleted.Contains(""))
                //{
                //    for (int j = 0; j < idsDeleted.Length; j++)
                //    {
                //        var itemToRemove = _dbContext.InvStockTransferItems.Find(Convert.ToInt32(idsDeleted[j]));
                //        itemToRemove.IsDeleted = true;
                //        var tracker = _dbContext.InvStockTransferItems.Update(itemToRemove);
                //        tracker.OriginalValues.SetValues(await tracker.GetDatabaseValuesAsync());
                //        await _dbContext.SaveChangesAsync();
                //    }
                //}
                //updating existing data
                var obj = _dbContext.InvStockTransfers.Find(model.Id);
                obj.TransferNo = model.TransferNo;
                obj.TransferDate = model.TransferDate;
                obj.WareHouseToId = model.WareHouseToId;
                obj.WareHouseFromId = model.WareHouseFromId;
                obj.Remarks = collection["Remarks"][0];
                //obj.Status = "Created";
                obj.UpdatedBy = model.UpdatedBy;
                obj.CompanyId = model.CompanyId;
                obj.UpdatedDate = DateTime.Now;
                obj.IsDeleted = false;
                var entry = _dbContext.InvStockTransfers.Update(obj);
                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                await _dbContext.SaveChangesAsync();


                var removeItems = _dbContext.InvStockTransferItems.Where(u => u.StockTransferId == model.Id).ToList();
                if (removeItems != null)
                {
                    foreach (var item in removeItems)
                    {
                        _dbContext.InvStockTransferItems.Remove(item);
                        _dbContext.SaveChanges();
                    }
                }


                for (int i = 0; i < collection["ItemId"].Count; i++)
                {
                    var stockTransferItem = new InvStockTransferItem();
                    stockTransferItem.StockTransferId = model.Id;
                    stockTransferItem.ItemId = Convert.ToInt32(collection["ItemId"][i]);
                    stockTransferItem.Qty = Convert.ToDecimal(collection["Qty"][i]);
                    stockTransferItem.IsDeleted = false;
                    stockTransferItem.CreatedBy = model.CreatedBy;
                    stockTransferItem.CreatedDate = DateTime.Now;
                    _dbContext.InvStockTransferItems.Add(stockTransferItem);
                    await _dbContext.SaveChangesAsync();
                }

                
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var stockTransferRepo = new StockTransferRepo(_dbContext);
            bool isSuccess = await stockTransferRepo.Delete(id);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Stock Transfer has been deleted successfully.";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong.";
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ApproveStockTransfer(int id)
        {
            string userId = HttpContext.Session.GetString("UserId");
            var stockTransferRepo = new StockTransferRepo(_dbContext);
            bool isSuccess = await stockTransferRepo.Approve(id, userId);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Stock Transfer has been approved successfully";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong.";
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult UnApprove()
        {
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            string userId = HttpContext.Session.GetString("UserId");
            ViewBag.UnApprove = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == resp_Id).FirstOrDefault().UnApprove;

            var model = new StockTransferRepo(_dbContext,HttpContext).GetApproved();
            ViewBag.NavbarHeading = "Un-Approve Stock Transfer";
            return View(model);
        }
        public async Task<IActionResult> UnApproveStockTransfer(int id)
        {
            var result = await new StockTransferRepo(_dbContext, HttpContext).UnApprove(id);
            if (result["error"] == "true")
            {
                TempData["error"] = "true";
                TempData["message"] = result["message"];
            }
            else
            {
                TempData["error"] = "false";
                TempData["message"] = result["message"];
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public IActionResult PartialStockTransferItem(int? counter)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            ViewBag.Counter = counter;
            var model = new InvStockTransferViewModel();
            return PartialView("_partialStockTransferItem", model);
        }

        public IActionResult GetStockTransferItems(int id, int itemId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var stockTransferRepo = new StockTransferRepo(_dbContext);
            var viewModel = stockTransferRepo.GetStockTransferItems(id, itemId);
            ViewBag.Counter = id;
            ViewBag.ItemId = viewModel.ItemId;
            return PartialView("_partialStockTransferItem", viewModel);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            TempData["RoleName"] = (from a in _dbContext.Roles
                                    join b in _dbContext.UserRoles.Where(x => x.UserId == userId) on a.Id equals b.RoleId
                                    select a.Name).FirstOrDefault();
            //TempData["ShowRate"] = (from a in _dbContext.Users.Where(x => x.Id == userId) select a.ShowRate).FirstOrDefault();
            string configValues = _dbContext.AppCompanyConfigs
                               .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                               .Select(c => c.ConfigValue)
                               .FirstOrDefault();
            var stockTransfer = _dbContext.InvStockTransfers.Include(i => i.WareHouseFrom).Include(i => i.WareHouseTo)
            .Where(i => i.Id == id && i.CompanyId == companyId).FirstOrDefault();
            var stockTransferItems = _dbContext.InvStockTransferItems
                                       .Include(i => i.Item)
                                       .Include(i => i.StockTransfer)
                                       .Where(i => i.StockTransferId == id && i.IsDeleted == false)
                                         .ToList();
            ViewBag.ReportPath2 = string.Concat(configValues, "Viewer", "?Report=StockTransfer&cId=", companyId, "&showRate=" + TempData["ShowRate"], "&id={0}");
            ViewBag.NavbarHeading = "Stock Transfer";
            ViewBag.TitleStatus = "Approved";

            TempData["Detail"] = stockTransferItems;
            return View(stockTransfer);
        }
        public IActionResult GetUOM(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var items = from item in _dbContext.InvItems
                        join config in _dbContext.AppCompanyConfigs on item.Id equals id
                        where item.Id == id && config.Id == item.Unit && config.CompanyId == companyId
                        select new
                        {
                            uom = config.ConfigValue,
                            id = config.Id,
                            rate = item.PurchaseRate,
                            avgRate = item.AvgRate,
                            stock = item.StockAccountId
                        };
            return Ok(items);
        }
        //public IActionResult GetItem(int id)
        //{
        //    int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
        //    var item = _dbContext.InvItems.Where(a => a.Id == id && a.CompanyId == companyId && a.IsDeleted == false)
        //                                       .Select(a => new
        //                                       {
        //                                           id = a.Id,
        //                                           text = a.Code + " - " + a.Name
        //                                       })
        //                                       .FirstOrDefault();
        //    ViewBag.Items = new SelectList(from ac in _dbContext.InvItems.Where(x => x.IsDeleted == false).ToList()
        //                                   select new
        //                                   {
        //                                       Id = ac.Id,
        //                                       Code = ac.Code,
        //                                       Name = ac.Code + " - " + ac.Name
        //                                   }, "Id", "Name");
        //    return Ok(item);
        //}

        public IActionResult GetItems()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var item = new SelectList(from ac in _dbContext.InvItems.Where(x => x.IsDeleted == false && x.CompanyId == companyId).ToList()
                                           select new
                                           {
                                               Id = ac.Id,
                                               Name = ac.Code + " - " + ac.Name
                                           }, "Id", "Name");
            return Ok(item);
        }
        public IActionResult GetStockTransfer()
        {
            try
            {
                int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
                string userId = HttpContext.Session.GetString("UserId");
                var approve = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == resp_Id).FirstOrDefault().Approve;
                var unApprove = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == resp_Id).FirstOrDefault().UnApprove;

                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                //var searchValue = Request.Form["search[value]"].FirstOrDefault();

                var searchTransNo = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchTransDate = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchFrom = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchTo = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchStatus = Request.Form["columns[4][search][value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var StockTransfersData = (from StockTransfers in _dbContext.InvStockTransfers.Include(x=>x.WareHouseFrom).Include(x=>x.WareHouseTo).Where(x => x.IsDeleted == false && x.CompanyId == companyId) select StockTransfers);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    StockTransfersData = StockTransfersData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                StockTransfersData = !string.IsNullOrEmpty(searchTransNo) ? StockTransfersData.Where(m => m.TransferNo.ToString().ToUpper().Contains(searchTransNo.ToUpper())) : StockTransfersData;
                StockTransfersData = !string.IsNullOrEmpty(searchTransDate) ? StockTransfersData.Where(m => m.TransferDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchTransDate.ToUpper())) : StockTransfersData;
                StockTransfersData = !string.IsNullOrEmpty(searchFrom) ? StockTransfersData.Where(m => m.WareHouseFrom.ConfigValue.ToString().ToUpper().Contains(searchFrom.ToUpper())) : StockTransfersData;
                StockTransfersData = !string.IsNullOrEmpty(searchTo) ? StockTransfersData.Where(m => m.WareHouseTo.ConfigValue.ToString().ToUpper().Contains(searchTo.ToUpper())) : StockTransfersData;
                StockTransfersData = !string.IsNullOrEmpty(searchStatus) ? StockTransfersData.Where(m => m.Status.ToString().ToUpper().Contains(searchStatus.ToUpper())) : StockTransfersData;

                recordsTotal = StockTransfersData.Count();
                var data = StockTransfersData.ToList();
                if (pageSize == -1)
                {
                    data = StockTransfersData.Skip(skip).Take(recordsTotal == 0 ? 1 : pageSize == -1 ? recordsTotal : 1).ToList();
                }
                else
                {
                    data = StockTransfersData.Skip(skip).Take(pageSize).ToList();
                }
                List<InvStockTransferViewModel> Details = new List<InvStockTransferViewModel>();
                foreach (var grp in data)
                {
                    InvStockTransferViewModel invStockTransferViewModel = new InvStockTransferViewModel();
                    invStockTransferViewModel.StockTransferDate = grp.TransferDate.ToString(Helpers.CommonHelper.DateFormat);
                    invStockTransferViewModel.StockTransfer = grp;
                    invStockTransferViewModel.StockTransfer.Approve = approve;
                    invStockTransferViewModel.StockTransfer.Unapprove = unApprove;
                    Details.Add(invStockTransferViewModel);

                }
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = Details };
                return Ok(jsonData);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}