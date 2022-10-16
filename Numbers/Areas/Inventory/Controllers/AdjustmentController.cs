using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Helpers;
using Numbers.Repository.Helpers;
using System.Linq.Dynamic.Core;

namespace Numbers.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    [Authorize]
    public class AdjustmentController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public AdjustmentController(NumbersDbContext context)
        {
            _dbContext = context;
        }
        public IActionResult Index()
        {
            string userid = HttpContext.Session.GetString("UserId");
           // var BranchIds = _dbContext.AppUserBranches.Where(x => x.UserId == userid).Select(x => x.BranchId).ToList();
            ViewBag.NavbarHeading = "List of Adjusted Items";
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            return View(_dbContext.InvAdjustments
                                                 .Include(w => w.WareHouse)
                                                 .Where(a => !a.IsDeleted&& a.CompanyId == companyId).ToList());
        }
        [HttpGet]
        public IActionResult Create(int id)
        {
            ViewBag.Counter = 0;
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
          //  TempData["ShowRate"] = (from a in _dbContext.Users.Where(x => x.Id == userId) select a.ShowRate).FirstOrDefault();
            var configValues = new ConfigValues(_dbContext);
            ViewBag.WareHouse = configValues.GetConfigValues("Inventory", "Ware House", companyId);
            //ViewBag.Items = new SelectList(from ac in _dbContext.InvItems.Where(x => x.IsDeleted == false).ToList()
            //                               select new
            //                               {
            //                                   Id = ac.Id,
            //                                   Name = ac.Code + " - " + ac.Name
            //                               }, "Id", "Name");
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            List<InvItemCategories> ItemCategory = _dbContext.Sys_ResponsibilityItemCategory.Include(x => x.ItemCategory).Where(x => x.ResponsibilityId == resp_Id).Select(x => x.ItemCategory).ToList();
            List<InvItem> ItemByResp = new List<InvItem>();
            foreach (InvItemCategories item in ItemCategory)
            {
                var itemList = _dbContext.InvItems.Include(x => x.Category).Where(x => /*x.CompanyId == companyId && */x.IsDeleted == false && x.Category.Code.StartsWith(item.Code)).ToList();

                ItemByResp.AddRange(itemList.ToList());
            }

            ViewBag.Items = new SelectList(
                ItemByResp.ToList().Select(a => new
                {
                    id = a.Id,
                    text = string.Concat(a.Code, " - ", a.Name)
                }).ToList(), "id", "text");



            if (id == 0)
            {
                TempData["Mode"] = false;
                ViewBag.EntityState = "Create";
                ViewBag.NavbarHeading = "Create Store Adjustment";
                int maxAdjustmentNo = 1;
                var adjustment = _dbContext.InvAdjustments.Where(c => c.CompanyId == companyId).ToList();
                if (adjustment.Count > 0)
                {
                    maxAdjustmentNo = adjustment.Max(v => v.AdjustmentNo);
                    ViewBag.Adjustment = maxAdjustmentNo + 1;
                }
                else
                {
                    ViewBag.Adjustment = maxAdjustmentNo;
                }
                return View(new InvAdjustmentViewModel());
            }
            else
            {
                TempData["Mode"] = true;
                var adjustment = _dbContext.InvAdjustments.Find(id);
                var viewModel = new InvAdjustmentViewModel();
                viewModel.Id = adjustment.Id;
                ViewBag.Adjustment = adjustment.AdjustmentNo;
                viewModel.AdjustmentDate = adjustment.AdjustmentDate;
                viewModel.WareHouseId = adjustment.WareHouseId;
                viewModel.VoucherId = adjustment.VoucherId;
                viewModel.Remarks = adjustment.Remarks;
                viewModel.Status = adjustment.Status;
                var adjustmentItems = _dbContext.InvAdjustmentItems
                                        .Include(i => i.Item)
                                        .Include(i => i.Adjustment)
                                          .Where(i => i.AdjustmentId == id && i.IsDeleted == false)
                                          .ToList();

                foreach (var item in adjustmentItems)
                {
                    item.ItemName = item.Item.Code+"-"+item.Item.Name;
                    //var obj = _dbContext.InvItems.Where(x => x.Id == id && x.IsDeleted == false).FirstOrDefault();
                    item.UnitName = (from a in _dbContext.AppCompanyConfigs.Where(x => x.Id == item.Item.Unit) select a.ConfigValue).FirstOrDefault();
                }
                ViewBag.AdjustmentItems = adjustmentItems;
                //var maxInvoiceNo = _dbContext.ARInvoices.Where(c => c.CompanyId == companyId).Max(v => v.InvoiceNo);
                viewModel.InvAdjustmentItems = adjustmentItems;
                TempData["AdjustmentNo"] = viewModel.AdjustmentNo;
                if (viewModel.Status != "Approved")
                {
                    ViewBag.EntityState = "Update";
                    ViewBag.NavbarHeading = "Store Adjustment";
                    ViewBag.TitleStatus = "Created";
                }
                return View(viewModel);
            }
        }
        public IActionResult GetItem(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var item = _dbContext.InvItems.Where(a => a.Id == id && a.CompanyId == companyId && a.IsDeleted == false)
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   text = a.Code+"-"+a.Name
                                               })
                                               .FirstOrDefault();
            return Ok(item);
        }
        public IActionResult GetItemById(int id)
        {
            var obj = _dbContext.InvItems.Where(x => x.Id == id && x.IsDeleted == false).FirstOrDefault();
            if (obj != null)
            {
                obj.Code = (from a in _dbContext.AppCompanyConfigs.Where(x => x.Id == obj.Unit) select a.ConfigValue).FirstOrDefault();
            }
                return Json(obj);
        }
        public JsonResult checkProductCodeAlreadyExists(int code)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            System.Threading.Thread.Sleep(200);
            if (code == 0)
                return Json(0);
            var chkCode = _dbContext.InvAdjustments.Where(a => a.IsDeleted == false && a.AdjustmentNo == code && a.CompanyId == companyId).FirstOrDefault();
            return Json(chkCode == null ? 0 : 1);
        }

        [HttpPost]
        public async Task<IActionResult> Create(InvAdjustmentViewModel viewModel, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var adjustment = new InvAdjustment();
            if (!ModelState.IsValid)
            {
                return View(adjustment);
            }
            else
            {
                if (viewModel.Id == 0)
                {
                    InvAdjustmentItem[] InvAdjustmentItems = JsonConvert.DeserializeObject<InvAdjustmentItem[]>(collection["details"]);
                    if (InvAdjustmentItems.Count() > 0)
                    {
                        adjustment.Status = "Created";
                        adjustment.AdjustmentType = "ADJ";
                        adjustment.CreatedBy = userId;
                        adjustment.CreatedDate = DateTime.Now;
                        adjustment.CompanyId = companyId;
                        adjustment.AdjustmentNo = Convert.ToInt16(viewModel.AdjustmentNo);
                        adjustment.AdjustmentDate = viewModel.AdjustmentDate;
                        adjustment.WareHouseId = viewModel.WareHouseId;
                        adjustment.Remarks = (collection["Remarks"][0]);

                        _dbContext.InvAdjustments.Add(adjustment);
                        await _dbContext.SaveChangesAsync();


                        foreach (var items in InvAdjustmentItems)
                        {
                            if (items.Id != 0)
                            {
                                InvAdjustmentItem data = _dbContext.InvAdjustmentItems.Where(i => i.AdjustmentId == adjustment.Id && i.IsDeleted == false && i.Id == items.Id).FirstOrDefault();
                                //foreach (var i in data)
                                //{

                                InvAdjustmentItem obj = new InvAdjustmentItem();
                                obj = data;
                                obj.ItemId = items.ItemId;
                                obj.LineTotal = items.LineTotal;
                                obj.PhysicalStock = items.PhysicalStock;
                                obj.Rate = items.Rate;
                              

                               
                                _dbContext.InvAdjustmentItems.Update(obj);
                                _dbContext.SaveChanges();
                                //}
                            }
                            else
                            {
                                InvAdjustmentItem data = new InvAdjustmentItem();
                                data = items;
                                data.PhysicalStock = items.PhysicalStock;
                                data.AdjustmentId = adjustment.Id;


                                _dbContext.InvAdjustmentItems.Add(data);
                                //val.OriginalValues.SetValues(await val.GetDatabaseValuesAsync());
                                _dbContext.SaveChanges();
                            }
                        }
                        TempData["error"] = "false";
                        TempData["message"] = "Adjustment has been created successfully";
                        return RedirectToAction(nameof(Index));
                        //return RedirectToAction("Create", new { id = adjustment.Id });
                    }
                    else
                    {
                        TempData["error"] = "true";
                        TempData["message"] = "No any Adjustment has been Created. It must contain atleast one item";
                        return RedirectToAction(nameof(Index));
                    }

                }
                else
                {

                }
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public IActionResult GetAdjustmentItems(int id, int itemId)
        {
            var item = _dbContext.InvAdjustmentItems.Include(i => i.Item).Where(i => i.Id == id && i.IsDeleted == false).FirstOrDefault();
            ViewBag.Counter = id;
            InvAdjustmentViewModel viewModel = new InvAdjustmentViewModel();
            viewModel.ItemId = item.ItemId;
            viewModel.Rate = item.Rate;
            viewModel.LineTotal = item.LineTotal;
            viewModel.Stock = item.Stock;
            viewModel.PhysicalStock = item.PhysicalStock;
            viewModel.Balance = +Math.Abs(item.Balance);
            viewModel.AdjustmentItemId = item.Id;
            viewModel.Remarks = item.Remarks;
            ViewBag.ItemId = itemId;
            return PartialView("_partialAdjustmentItem", viewModel);
        }

        public async Task<IActionResult> Update(InvAdjustmentViewModel viewModel, IFormCollection collection)
        {

            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");

            string[] idsDeleted = Convert.ToString(collection["IdsDeleted"]).Split(",");
            foreach (var item in idsDeleted)
            {
                if (item != "")
                {
                    var itemToRemove = _dbContext.InvAdjustmentItems.Find(Convert.ToInt32(item));
                    itemToRemove.IsDeleted = true;
                    _dbContext.InvAdjustmentItems.Update(itemToRemove);
                    await _dbContext.SaveChangesAsync();

                }
            }
            var adjustment = _dbContext.InvAdjustments.Find(viewModel.Id);
            if (viewModel.Id != 0)
            {
                InvAdjustmentItem[] InvAdjustmentItems = JsonConvert.DeserializeObject<InvAdjustmentItem[]>(collection["details"]);
                if (InvAdjustmentItems.Count() > 0)
                {
                    // var adjustment =
                    adjustment.Status = "Created";
                    adjustment.AdjustmentType = "ADJ";
                    adjustment.CreatedBy = userId;
                    adjustment.CreatedDate = DateTime.Now;
                    adjustment.CompanyId = companyId;
                    adjustment.AdjustmentNo =Convert.ToInt16(viewModel.AdjustmentNo);
                    adjustment.AdjustmentDate = viewModel.AdjustmentDate;
                    adjustment.WareHouseId = viewModel.WareHouseId;
                    adjustment.Remarks = (collection["Remarks"][0]);

                    _dbContext.InvAdjustments.Update(adjustment);
                    await _dbContext.SaveChangesAsync();


                    foreach (var items in InvAdjustmentItems)
                    {
                        if (items.Id != 0)
                        {
                            InvAdjustmentItem data = _dbContext.InvAdjustmentItems.Where(i => i.AdjustmentId == adjustment.Id && i.IsDeleted == false && i.Id == items.Id).FirstOrDefault();
                            //foreach (var i in data)
                            //{

                            InvAdjustmentItem obj = new InvAdjustmentItem();
                           // data = items;
                            data.AdjustmentId = adjustment.Id;
                            data.ItemId = items.ItemId;
                            data.LineTotal = items.LineTotal;
                            data.PhysicalStock = items.PhysicalStock;
                            data.Rate = items.Rate;
                            _dbContext.InvAdjustmentItems.Update(data);
                            _dbContext.SaveChanges();
                            TempData["error"] = "false";
                            TempData["message"] = "Adjustment has been Updated successfully";
                            //}
                        }
                        else
                        {
                            InvAdjustmentItem data = new InvAdjustmentItem();
                            data = items;
                            data.PhysicalStock = items.PhysicalStock;
                            data.AdjustmentId = adjustment.Id;


                            _dbContext.InvAdjustmentItems.Add(data);
                            //val.OriginalValues.SetValues(await val.GetDatabaseValuesAsync());
                            _dbContext.SaveChanges();
                        }
                    }
                    TempData["error"] = "false";
                    TempData["message"] = "Adjustment has been created successfully";
                    return RedirectToAction(nameof(Index));
                    //return RedirectToAction("Create", new { id = adjustment.Id });
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "No any Adjustment has been Created. It must contain atleast one item";
                    return RedirectToAction(nameof(Index));
                }
            }
            return RedirectToAction(nameof(Index)); ;
            //}
        }

        public async Task<IActionResult> Approve(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var adjustment = _dbContext.InvAdjustments
                       .Where(a => a.Id == id && a.CompanyId == companyId && a.Status == "Created" && !a.IsDeleted).FirstOrDefault();

            try
            {
                //Create Voucher  
                GLVoucher voucherMaster = new GLVoucher();
                List<GLVoucherDetail> voucherDetails = new List<GLVoucherDetail>();
                string voucherDescription = string.Format(
                "Store Adjustment # : {0} ",
                adjustment.AdjustmentNo);

                int voucherId = 0;
                voucherMaster.VoucherType = "ADJ";
                voucherMaster.VoucherDate = adjustment.AdjustmentDate;
                voucherMaster.Currency = "MYR";
                voucherMaster.CurrencyExchangeRate = 1;
                voucherMaster.Description = voucherDescription.Substring(0, voucherDescription.Length);
                voucherMaster.Status = "Approved";
                voucherMaster.ApprovedBy = userId;
                voucherMaster.ApprovedDate = DateTime.Now;
                voucherMaster.IsSystem = true;
                voucherMaster.ModuleName = "Inventory/Adjustment";
                voucherMaster.ModuleId = id;

                GLVoucherDetail voucherDetail = new GLVoucherDetail();
                //debit entry
                #region Debit
                //var stockAccounts = (from li in _dbContext.InvAdjustmentItems
                //                     from a in _dbContext.GLAccounts
                //                     where li.AdjustmentId == id && li.IsDeleted == false && a.CompanyId == companyId &&
                //                     a.Id == li.Item.CostofSaleAccountId && a.CompanyId == companyId
                //                     select new
                //                     {
                //                         li.LineTotal,
                //                         a.Id
                //                     }).GroupBy(a => a.Id)
                //                .Select(li => new InvAdjustmentItem
                //                {
                //                    LineTotal = li.Sum(c => c.LineTotal),
                //                    AdjustmentId = li.FirstOrDefault().Id //AdjustmentId is temporarily containing EarningAccountId
                //                }).ToList();
                var stockAccounts = (from li in _dbContext.InvAdjustmentItems
                                     join i in _dbContext.InvItems.Include(a => a.InvItemAccount) on li.ItemId equals i.Id
                                     where li.AdjustmentId == id && li.IsDeleted == false
                                     select new
                                     {
                                         li.LineTotal,
                                         i.InvItemAccount.GLAssetAccountId
                                     }).GroupBy(l => l.GLAssetAccountId)
                            .Select(li => new InvAdjustmentItem
                            {
                                LineTotal = li.Sum(c => c.LineTotal),
                                AdjustmentId = li.FirstOrDefault().GLAssetAccountId //StoreIssueId is temporarily containing GLCostofSaleAccountId
                            }).ToList();
                foreach (var item in stockAccounts)
                {
                    voucherDetail = new GLVoucherDetail();
                    voucherDetail.AccountId = item.AdjustmentId;
                    voucherDetail.Sequence = 20;
                    voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                    voucherDetail.Debit = item.LineTotal;
                    voucherDetail.Credit = 0;
                    voucherDetail.IsDeleted = false;
                    voucherDetail.CreatedBy = userId;
                    voucherDetail.CreatedDate = DateTime.Now;
                    voucherDetails.Add(voucherDetail);
                }
                #endregion Debit
                //credit entry
                #region Credit
                //var earningAccounts = (from li in _dbContext.InvAdjustmentItems
                //                       from a in _dbContext.GLAccounts
                //                       where li.AdjustmentId == id && li.IsDeleted == false && a.CompanyId == companyId &&
                //                       a.Name == "TELEPHONE BILLS PAYABLE" && a.CompanyId == companyId
                //                       select new
                //                       {
                //                           li.LineTotal,
                //                           a.Id
                //                       }).GroupBy(a => a.Id)
                //                .Select(li => new InvAdjustmentItem
                //                {
                //                    LineTotal = li.Sum(c => c.LineTotal),
                //                    AdjustmentId = li.FirstOrDefault().Id //AdjustmentId is temporarily containing EarningAccountId
                //                }).ToList();

                var earningAccounts = (from li in _dbContext.InvAdjustmentItems
                                       join i in _dbContext.InvItems.Include(a => a.InvItemAccount) on li.ItemId equals i.Id
                                       where li.AdjustmentId == id && li.IsDeleted == false
                                       select new
                                       {
                                           li.LineTotal,
                                           i.InvItemAccount.GLCostofSaleAccountId
                                       }).GroupBy(l => l.GLCostofSaleAccountId)
                            .Select(li => new InvAdjustmentItem
                            {
                                LineTotal = li.Sum(c => c.LineTotal),
                                AdjustmentId = li.FirstOrDefault().GLCostofSaleAccountId //StoreIssueId is temporarily containing GLCostofSaleAccountId
                            }).ToList(); 

                foreach (var item in earningAccounts)
                {
                    voucherDetail = new GLVoucherDetail();
                    voucherDetail.AccountId = item.AdjustmentId;
                    voucherDetail.Sequence = 20;
                    voucherDetail.Description = voucherDescription.Substring(0, voucherDescription.Length);
                    voucherDetail.Debit = 0;
                    voucherDetail.Credit = item.LineTotal;
                    voucherDetail.IsDeleted = false;
                    voucherDetail.CreatedBy = userId;
                    voucherDetail.CreatedDate = DateTime.Now;
                    voucherDetails.Add(voucherDetail);
                }
                #endregion Credit
                //Create Voucher 
                var helper = new Numbers.Repository.Helpers.VoucherHelper(_dbContext, HttpContext);
                voucherId = helper.CreateVoucher(voucherMaster, voucherDetails);
                if (voucherId != 0)
                {
                    using (var transaction = _dbContext.Database.BeginTransaction())
                    {
                        adjustment.VoucherId = voucherId;
                        adjustment.Status = "Approved";
                        adjustment.ApprovedBy = userId;
                        adjustment.ApprovedDate = DateTime.Now;
                        var entry = _dbContext.InvAdjustments.Update(adjustment);
                        entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                        await _dbContext.SaveChangesAsync();

                        var adjustments = _dbContext.InvAdjustmentItems.Where(i => i.AdjustmentId == id && i.IsDeleted == false).ToList();
                        foreach (var adjustmentItem in adjustments)
                        {
                            var item = _dbContext.InvItems.Find(adjustmentItem.ItemId);
                            item.StockQty = item.StockQty + adjustmentItem.PhysicalStock;
                            item.AvgRate = adjustmentItem.Rate / item.StockQty;
                            item.StockValue = item.StockValue + (adjustmentItem.Rate * adjustmentItem.PhysicalStock);
                            if (item.StockQty != 0)
                            {
                                item.AvgRate = Math.Round(item.StockValue / item.StockQty, 6);
                            }
                            var dbEntry = _dbContext.InvItems.Update(item);
                            dbEntry.OriginalValues.SetValues(await dbEntry.GetDatabaseValuesAsync());
                            await _dbContext.SaveChangesAsync();
                        }
                        transaction.Commit();

                        TempData["error"] = "false";
                        TempData["message"] = "Store Adjustment has been approved.";
                        return RedirectToAction(nameof(Index));
                    }
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Something went wrong.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception exc)
            {
                //transaction.Rollback();
                TempData["error"] = "true";
                TempData["message"] = exc.Message == null ? exc.InnerException.Message.ToString() : exc.Message.ToString();
                return RedirectToAction(nameof(Index));
            }
        }
        public IActionResult UnApprove()
        {
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            string userId = HttpContext.Session.GetString("UserId");
            ViewBag.UnApprove = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == resp_Id).FirstOrDefault().UnApprove;

            var model = _dbContext.InvAdjustments.Where(i => i.Status == "Approved" && !i.IsDeleted && i.CompanyId == HttpContext.Session.GetInt32("CompanyId").Value).ToList();
            ViewBag.NavbarHeading = "Un-Approve Store Adjustment";
            return View(model);
        }
        public async Task<IActionResult> UnApproveAdjustment(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var adjustment = _dbContext.InvAdjustments
                                                .Where(a => a.Id == id && a.CompanyId == companyId && a.Status == "Approved" && !a.IsDeleted).FirstOrDefault();
                    adjustment.Status = "Created";
                    adjustment.ApprovedBy = null;
                    adjustment.ApprovedDate = null;
                    var entry = _dbContext.InvAdjustments.Update(adjustment);
                    entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                    await _dbContext.SaveChangesAsync();
                    var adjustments = _dbContext.InvAdjustmentItems.Where(i => i.AdjustmentId == id && i.IsDeleted == false).ToList();
                    foreach (var adjustmentItem in adjustments)
                    {
                        var item = _dbContext.InvItems.Find(adjustmentItem.ItemId);
                        item.StockQty = item.StockQty - adjustmentItem.PhysicalStock;
                        item.AvgRate = adjustmentItem.Rate / item.StockQty;
                        item.StockValue = item.StockValue - (adjustmentItem.Rate * adjustmentItem.PhysicalStock);
                        var dbEntry = _dbContext.InvItems.Update(item);
                        dbEntry.OriginalValues.SetValues(await dbEntry.GetDatabaseValuesAsync());
                        await _dbContext.SaveChangesAsync();
                    }
                    var adjustmentVoucher = _dbContext.GLVoucherDetails.Where(x => x.VoucherId == adjustment.VoucherId).ToList();
                    foreach(var item in adjustmentVoucher)
                    {
                        item.IsDeleted = true;
                        var dbEntry = _dbContext.GLVoucherDetails.Update(item);
                        dbEntry.OriginalValues.SetValues(await dbEntry.GetDatabaseValuesAsync());
                        await _dbContext.SaveChangesAsync();
                    }
                    transaction.Commit();
                    TempData["error"] = "false";
                    TempData["message"] = "Store Adjustment has been Un-Approved.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exc)
                {
                    transaction.Rollback();
                    TempData["error"] = "true";
                    TempData["message"] = exc.Message == null ? exc.InnerException.Message.ToString() : exc.Message.ToString();
                    return RedirectToAction(nameof(Index));
                }
            }
        }
        [HttpPost]
        public IActionResult PartialAdjustmentItem(int? counter)
        {
            ViewBag.Counter = counter;
            return PartialView("_partialAdjustmentItem");
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            TempData["RoleName"] = (from a in _dbContext.Roles
                                    join b in _dbContext.UserRoles.Where(x => x.UserId == userId) on a.Id equals b.RoleId
                                    select a.Name).FirstOrDefault();
         //   TempData["ShowRate"] = (from a in _dbContext.Users.Where(x => x.Id == userId) select a.ShowRate).FirstOrDefault();
            string configValues = _dbContext.AppCompanyConfigs
                                .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                                .Select(c => c.ConfigValue)
                                .FirstOrDefault();
            ViewBag.ReportPath = string.Concat(configValues, "Viewer", "?Report=Voucher&cId=", companyId, "&id={0}");
            ViewBag.ReportPath2 = string.Concat(configValues, "Viewer", "?Report=StoreAdjustment&cId=", companyId, "&showRate=" + TempData["ShowRate"], "&id={0}");
            var adjustment = _dbContext.InvAdjustments.Include(i => i.WareHouse)
            .Where(i => i.Id == id && i.CompanyId == companyId).FirstOrDefault();

            var adjustmentItems = _dbContext.InvAdjustmentItems
                                       .Include(i => i.Item)
                                       .Include(i => i.Adjustment)
                                       .Where(i => i.AdjustmentId == id && i.IsDeleted == false)
                                         .ToList();

            ViewBag.NavbarHeading = "Store Adjustment";
            ViewBag.TitleStatus = "Approved";

            TempData["Detail"] = adjustmentItems;
            return View(adjustment);
        }

        public IActionResult GetAdjustment()
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

                var searchAdjNo = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchAdjDate = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchWarehouse = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchRemarks = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchStatus = Request.Form["columns[4][search][value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var AdjData = (from Adj in _dbContext.InvAdjustments.Include(x=>x.WareHouse).Where(x => x.IsDeleted == false && x.CompanyId == companyId) select Adj);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    AdjData = AdjData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                
                AdjData = !string.IsNullOrEmpty(searchAdjNo) ? AdjData.Where(m => m.AdjustmentNo.ToString().ToUpper().Contains(searchAdjNo.ToUpper())) : AdjData;
                AdjData = !string.IsNullOrEmpty(searchAdjDate) ? AdjData.Where(m => m.AdjustmentDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchAdjDate.ToUpper())) : AdjData;
                AdjData = !string.IsNullOrEmpty(searchWarehouse) ? AdjData.Where(m => m.WareHouse.ConfigValue.ToString().ToUpper().Contains(searchWarehouse.ToUpper())) : AdjData;
                AdjData = !string.IsNullOrEmpty(searchRemarks) ? AdjData.Where(m => (m.Remarks != null ? m.Remarks.ToString().ToUpper().Contains(searchRemarks.ToUpper()) : false)) : AdjData;
                AdjData = !string.IsNullOrEmpty(searchStatus) ? AdjData.Where(m => m.Status.ToString().ToUpper().Contains(searchStatus.ToUpper())) : AdjData;

                recordsTotal = AdjData.Count();
                var data = AdjData.ToList();
                if (pageSize == -1)
                {
                    data = AdjData.Skip(skip).Take(recordsTotal == 0 ? 1 : pageSize == -1 ? recordsTotal : 1).ToList();
                }
                else
                {
                    data = AdjData.Skip(skip).Take(pageSize).ToList();
                }
                List<InvAdjustmentViewModel> Details = new List<InvAdjustmentViewModel>();
                foreach (var grp in data)
                {
                    InvAdjustmentViewModel invAdjustmentViewModel = new InvAdjustmentViewModel();
                    invAdjustmentViewModel.AdjDate = grp.AdjustmentDate.ToString(Helpers.CommonHelper.DateFormat);
                    invAdjustmentViewModel.InvAdjustments = grp;
                    invAdjustmentViewModel.InvAdjustments.Approve = approve;
                    invAdjustmentViewModel.InvAdjustments.Unapprove = unApprove;
                    Details.Add(invAdjustmentViewModel);

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