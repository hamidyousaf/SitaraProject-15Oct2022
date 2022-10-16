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
using Numbers.Repository.AR;
using Numbers.Repository.Helpers;
using Numbers.Repository.AP;
using Numbers.Repository.Setup;
using System.Linq.Dynamic.Core;
using Numbers.Helpers;
using Newtonsoft.Json;

namespace Numbers.Areas.Inventory.Controllers
{
    [Authorize]
    [Area("AP")]
    public class YarnIssuanceController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public YarnIssuanceController(NumbersDbContext context)
        {
            _dbContext = context;
        }

        public IActionResult Index()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userid = HttpContext.Session.GetString("UserId");
            string configValues = _dbContext.AppCompanyConfigs
                             .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                             .Select(c => c.ConfigValue)
                             .FirstOrDefault();
            ViewBag.ReportPath = string.Concat(configValues, "Viewer", "?Report=Voucher&cId=", companyId, "&id=");
            ViewBag.ReportPath2 = string.Concat(configValues, "Viewer", "?Report=YarnIssuance&cId=", companyId, "&id=");
            var issueReturnRepo = new YarnIssueRepo(_dbContext);
            IEnumerable<YarnIssuance> list = issueReturnRepo.GetAll(companyId);
            ViewBag.NavbarHeading = "List of Yarn Issuance";
            return View(list);
        }
        public JsonResult checkProductCodeAlreadyExists(int code)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            System.Threading.Thread.Sleep(200);
            if (code == 0)
                return Json(0);
            var chkCode = _dbContext.YarnIssuances.Where(a => a.IsDeleted == false && a.IssueNo == code && a.TransactionType == "Issue Return" && a.CompanyId == companyId).FirstOrDefault();
            return Json(chkCode == null ? 0 : 1);
        }
        public IActionResult Create(int id)
        {
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var issueReturnRepo = new YarnIssueRepo(_dbContext);
            var configValues = new ConfigValues(_dbContext);
            ViewBag.Counter = 0;
            var model = new YarnIssueViewModel();
            //model.BrandLOV = this.GetConfig(0, companyId, "Brands");

            string userId = HttpContext.Session.GetString("UserId");
            
            ViewBag.WareHouse = configValues.GetConfigValues("Inventory", "Ware House", companyId);
            ViewBag.WeavingContracts = new SelectList(_dbContext.GRWeavingContracts.Where(x=> !x.IsDeleted && x.IsApproved).ToList().OrderByDescending(x=>x.Id),"Id", "TransactionNo");
            ViewBag.Vendors = new SelectList(_dbContext.APSuppliers.Where(x => x.IsActive == true && x.CompanyId == companyId).ToList(), "Id", "Name");
            var YarnItems = _dbContext.AppCompanySetups.Where(x => x.Name == "Yarn Item Code").FirstOrDefault().Value;

            var resposibility = _dbContext.Sys_Responsibilities.FirstOrDefault(x => x.Resp_Id == resp_Id).Resp_Name;
            ViewBag.Responsibility = resposibility;
            List<InvItemCategories> ItemCategory = _dbContext.Sys_ResponsibilityItemCategory.Include(x => x.ItemCategory).Where(x => x.ResponsibilityId == resp_Id).Select(x => x.ItemCategory).ToList();
            List<InvItem> ItemByResp = new List<InvItem>();
            foreach (InvItemCategories item in ItemCategory)
            {
                var itemList = _dbContext.InvItems.Include(x => x.Category).Where(x => x.CompanyId == companyId && x.IsDeleted == false && x.Category.Code.StartsWith(item.Code)).ToList();

                ItemByResp.AddRange(itemList.ToList());
            }
            ViewBag.Itms =
                ItemByResp.ToList().Select(a => new
                {
                    id = a.Id,
                    text = string.Concat(a.Code, " - ", a.Name)
                });

            //ViewBag.Itms = new SelectList(from ac in _dbContext.InvItems.Where(x => x.IsDeleted == false && x.Code.StartsWith(YarnItems)).ToList()
            //                              select new
            //                              {
            //                                  Id = ac.Id,
            //                                  Name = ac.Code + " - " + ac.Name
            //                              }, "Id", "Name");
            ViewBag.OperatingUnit = configValues.GetOrgValues(resp_Id, "Operating Unit", companyId); 
            if (id == 0)
            {
                TempData["Mode"] = false;
                ViewBag.EntityState = "Create";
                ViewBag.NavbarHeading = "Create Yarn Issuance";
                int maxissueNo = 1;
                var issue = _dbContext.YarnIssuances.Where(c => c.CompanyId == companyId && c.IsDeleted == false).ToList();
                if (issue.Count > 0)
                {
                    maxissueNo = issue.Max(v => v.IssueNo);
                    ViewBag.Issue = maxissueNo + 1;
                }
                else
                {
                    ViewBag.Issue = maxissueNo;
                }
              
                return View(model);
            }
            else
            {
                ViewBag.EntityState = "Update";
                TempData["Mode"] = true;
                ViewBag.Id = id;
                YarnIssueViewModel viewModel = new YarnIssueViewModel();
                viewModel.YarnIssuance= _dbContext.YarnIssuances
                    .Include(x=>x.WeavingContract)
                        .ThenInclude(x => x.GreigeQualityLoom)
                            .ThenInclude(x => x.GRConstruction)
                                .ThenInclude(x => x.Warp)
                                    .ThenInclude(x => x.UOM)
                    .Include(x => x.WeavingContract)
                        .ThenInclude(x => x.GreigeQualityLoom)
                            .ThenInclude(x => x.GRConstruction)
                                .ThenInclude(x => x.Weft)
                                    .ThenInclude(x => x.UOM)
                    .Include(x => x.Vendor)
                    .Include(x=>x.WarpIssuances)
                        .ThenInclude(x=>x.Item)
                    .Include(x => x.WarpIssuances)
                        .ThenInclude(x => x.Brand)
                    .Include(x => x.WarpIssuances)
                        .ThenInclude(x => x.UOM)
                    .Include(x=>x.WeftIssuances)
                        .ThenInclude(x=>x.Item)
                    .Include(x => x.WeftIssuances)
                        .ThenInclude(x => x.Brand)
                    .Include(x => x.WeftIssuances)
                        .ThenInclude(x => x.UOM)
                    .FirstOrDefault(x=>x.Id == id);
                return View(viewModel);
            }
        }
        public SelectList GetConfig(int companyId, string value, int itemId)
        {
            //b.CompanyId == companyId &&
            var data = (from b in _dbContext.AppCompanyConfigBases.Where(x => x.Name == value)
                        join
                            c in _dbContext.AppCompanyConfigs on b.Id equals c.BaseId
                        join
                            grn in _dbContext.VwInvLedgers.Include(x => x.Item) on c.Id equals grn.BrandId
                        where /*b.CompanyId == companyId &&*/ grn.ItemId == itemId
                        select new
                        {
                            c,
                            grn
                        }
                        ).ToList();

            var Seasons = new SelectList(data.GroupBy(x => x.c.Id).Select(x => new ListOfValue
            {
                Id = x.Select(a => a.c.Id).FirstOrDefault(),
                Quantity = x.Select(a => a.grn.Qty).Sum(),
                Name = $"{x.Select(a => a.c.ConfigValue).FirstOrDefault()}: {x.Select(a => a.grn.Qty).Sum()}"
            }).ToList(), "Id", "Name");
            return Seasons;

            //var data = (from b in _dbContext.AppCompanyConfigBases.Where(x => x.Name == value)
            //            join
            //                c in _dbContext.AppCompanyConfigs on b.Id equals c.BaseId
            //            join
            //                grn in _dbContext.APGRNItem.Include(x => x.Item).Where(x => !x.IsDeleted) on c.Id equals grn.BrandId
            //            where b.CompanyId == companyId && b.IsActive && grn.ItemId == itemId
            //            select new
            //            {
            //                c,
            //                grn
            //            }
            //                      ).ToList();

            //var Seasons = new SelectList(data.GroupBy(x=>x.c.Id).Select(x=> new ListOfValue 
            //{ 
            //    Id = x.Select(a=>a.c.Id).FirstOrDefault(),
            //    Quantity = x.Select(a=>a.grn.Item.StockQty).Sum(),
            //    Name = $"{x.Select(a=>a.c.ConfigValue).FirstOrDefault()}: {x.Select(a=>a.grn.Item.StockQty).Sum()}" 
            //}).ToList(), "Id", "Name");
            //return Seasons;
        }
        public YarnIssueViewModel GetById(int id)
        {
            YarnIssuance YarnIssuance = _dbContext.YarnIssuances.Find(id);
            var viewModel = new YarnIssueViewModel();
            viewModel.IssueNo = YarnIssuance.IssueNo;
            viewModel.IssueDate = YarnIssuance.IssueDate;
            viewModel.WareHouseId = YarnIssuance.WareHouseId;
            viewModel.VendorId = YarnIssuance.VendorId;
            viewModel.Remarks = YarnIssuance.Remarks;
            viewModel.Status = YarnIssuance.Status;
            viewModel.VoucherId = YarnIssuance.VoucherId;
            return viewModel;
        }

        [HttpPost]
        public async Task<IActionResult> Create(YarnIssueViewModel model, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            WarpIssuance[] warpdetail = JsonConvert.DeserializeObject<WarpIssuance[]>(collection["WarpDetails"]);
            WeftIssuance[] weftdetail = JsonConvert.DeserializeObject<WeftIssuance[]>(collection["WeftDetails"]);
            var issueReturnRepo = new YarnIssueRepo(_dbContext);
            if (model.Id == 0)
            {
                model.CreatedBy = userId;
                model.CompanyId = companyId;
                model.VendorId = Convert.ToInt32(collection["VendorId"]);
                model.IssueNo = issueReturnRepo.Max(companyId);
                model.WarpDetails = warpdetail;
                model.WeftDetails = weftdetail;
                bool isSuccess = await issueReturnRepo.Create(model);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = string.Format("Yarn Issuance. {0} has been created successfully.", model.IssueNo);
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Something went wrong.";
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                model.VendorId = Convert.ToInt32(collection["VendorId"]);
                model.UpdatedBy = userId;
                model.CompanyId = companyId;
                model.WarpDetails = warpdetail;
                model.WeftDetails = weftdetail;
                bool isSuccess = await issueReturnRepo.Update(model);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = string.Format("Yarn Issuance. {0} has been updated successfully.", model.IssueNo);
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Something went wrong.";
                }
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var issueReturnRepo = new YarnIssueRepo(_dbContext);
            bool isSuccess = await issueReturnRepo.Delete(id);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Yarn Issuance has been deleted successfully.";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong.";
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Approve(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var issueReturnRepo = new YarnIssueRepo(_dbContext, HttpContext);
            bool isSuccess = await issueReturnRepo.Approve(id, userId, companyId);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Yarn Issuance has been approved successfully";
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
            var model = new YarnIssueRepo(_dbContext, HttpContext).GetApproved();
            ViewBag.NavbarHeading = "Un-Approve Yarn Issuance";
            return View(model);
        }

        public IActionResult UnApprove(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            YarnIssuance model = _dbContext.YarnIssuances.Find(id);
                model.ApprovedBy = _userId;
                model.ApprovedDate = DateTime.UtcNow;
                model.Status = "Created";
                _dbContext.YarnIssuances.Update(model);
                _dbContext.SaveChanges();
                TempData["error"] = "false";
                TempData["message"] = "Yarn Issuance Approved Succesfully..!";

 
            return RedirectToAction("Index", "YarnIssuance");
        }

        [HttpGet]
        public async Task<IActionResult> UnApproveYarnIssuance(int id)
        {
            try
            {
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                string userId = HttpContext.Session.GetString("UserId");
                var result = await new YarnIssueRepo(_dbContext, HttpContext).UnApprove(id);
                if (result["error"] == "false")
                {
                    TempData["error"] = "false";
                    TempData["message"] = result["message"];
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = result["message"];
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception exc)
            {
                TempData["error"] = "true";
                TempData["message"] = exc.Message == null ? exc.InnerException.Message.ToString() : exc.Message.ToString();
                return RedirectToAction(nameof(Index));
            }
        }
 
        [HttpGet]
        public IActionResult Details(int id)
        {
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var issueReturnRepo = new YarnIssueRepo(_dbContext);
            var configValues = new ConfigValues(_dbContext);
            ViewBag.Counter = 0;
            var model = new YarnIssueViewModel();
            //model.BrandLOV = this.GetConfig(0, companyId, "Brands");

            string userId = HttpContext.Session.GetString("UserId");

            ViewBag.WareHouse = configValues.GetConfigValues("Inventory", "Ware House", companyId);
            ViewBag.WeavingContracts = new SelectList(_dbContext.GRWeavingContracts.Where(x => !x.IsDeleted && x.IsApproved).ToList().OrderByDescending(x => x.Id), "Id", "TransactionNo");
            ViewBag.Vendors = new SelectList(_dbContext.APSuppliers.Where(x => x.IsActive == true && x.CompanyId == companyId).ToList(), "Id", "Name");
            var YarnItems = _dbContext.AppCompanySetups.Where(x => x.Name == "Yarn Item Code").FirstOrDefault().Value;

            var resposibility = _dbContext.Sys_Responsibilities.FirstOrDefault(x => x.Resp_Id == resp_Id).Resp_Name;
            ViewBag.Responsibility = resposibility;
            List<InvItemCategories> ItemCategory = _dbContext.Sys_ResponsibilityItemCategory.Include(x => x.ItemCategory).Where(x => x.ResponsibilityId == resp_Id).Select(x => x.ItemCategory).ToList();
            List<InvItem> ItemByResp = new List<InvItem>();
            foreach (InvItemCategories item in ItemCategory)
            {
                var itemList = _dbContext.InvItems.Include(x => x.Category).Where(x => x.CompanyId == companyId && x.IsDeleted == false && x.Category.Code.StartsWith(item.Code)).ToList();

                ItemByResp.AddRange(itemList.ToList());
            }
            ViewBag.Itms =
                ItemByResp.ToList().Select(a => new
                {
                    id = a.Id,
                    text = string.Concat(a.Code, " - ", a.Name)
                });

            //ViewBag.Itms = new SelectList(from ac in _dbContext.InvItems.Where(x => x.IsDeleted == false && x.Code.StartsWith(YarnItems)).ToList()
            //                              select new
            //                              {
            //                                  Id = ac.Id,
            //                                  Name = ac.Code + " - " + ac.Name
            //                              }, "Id", "Name");
            ViewBag.OperatingUnit = configValues.GetOrgValues(resp_Id, "Operating Unit", companyId);
            if (id == 0)
            {
                TempData["Mode"] = false;
                ViewBag.EntityState = "Create";
                ViewBag.NavbarHeading = "Create Yarn Issuance";
                int maxissueNo = 1;
                var issue = _dbContext.YarnIssuances.Where(c => c.CompanyId == companyId && c.IsDeleted == false).ToList();
                if (issue.Count > 0)
                {
                    maxissueNo = issue.Max(v => v.IssueNo);
                    ViewBag.Issue = maxissueNo + 1;
                }
                else
                {
                    ViewBag.Issue = maxissueNo;
                }

                return View(model);
            }
            else
            {
                ViewBag.EntityState = "Update";
                TempData["Mode"] = true;
                ViewBag.Id = id;
                YarnIssueViewModel viewModel = new YarnIssueViewModel();
                viewModel.YarnIssuance = _dbContext.YarnIssuances
                    .Include(x => x.WeavingContract)
                        .ThenInclude(x => x.GreigeQualityLoom)
                            .ThenInclude(x => x.GRConstruction)
                                .ThenInclude(x => x.Warp)
                                    .ThenInclude(x => x.UOM)
                    .Include(x => x.WeavingContract)
                        .ThenInclude(x => x.GreigeQualityLoom)
                            .ThenInclude(x => x.GRConstruction)
                                .ThenInclude(x => x.Weft)
                                    .ThenInclude(x => x.UOM)
                    .Include(x => x.Vendor)
                    .Include(x => x.WarpIssuances)
                        .ThenInclude(x => x.Item)
                    .Include(x => x.WarpIssuances)
                        .ThenInclude(x => x.Brand)
                    .Include(x => x.WarpIssuances)
                        .ThenInclude(x => x.UOM)
                    .Include(x => x.WeftIssuances)
                        .ThenInclude(x => x.Item)
                    .Include(x => x.WeftIssuances)
                        .ThenInclude(x => x.Brand)
                    .Include(x => x.WeftIssuances)
                        .ThenInclude(x => x.UOM)
                    .FirstOrDefault(x => x.Id == id);
                return View(viewModel);
            }
        }
        
        [HttpGet]
        public IActionResult GetSaleInvoiceItems(int saleInvoiceItemId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var saleReturnRepo = new YarnIssueRepo(_dbContext);
            var item = saleReturnRepo.GetSaleInvoiceItems(saleInvoiceItemId);
            ViewBag.Counter = saleInvoiceItemId;
            ViewBag.ItemId = item.ItemId;
            return Ok(item);
        } 
   

        public IActionResult GetItemById(int id)
        {
            var obj = _dbContext.InvItems.Where(x => x.Id == id && x.IsDeleted == false).FirstOrDefault();
            obj.Code = (from a in _dbContext.AppCompanyConfigs.Where(x => x.Id == obj.Unit) select a.ConfigValue).FirstOrDefault();
            return Json(obj);
        }


        public IActionResult GetItemBrandWiseId(int id, int[] skipIds)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var saleReturnRepo = new YarnIssueRepo(_dbContext);
            var configValues = new ConfigValues(_dbContext);
            var api = new ApiController(_dbContext);
            var invoice = saleReturnRepo.GetYarnIssuanceById(id, skipIds, companyId);
            List<YarnIssueViewModel> list = new List<YarnIssueViewModel>();
            foreach (var item in invoice)
            {
                //if (item.ReturnQty != item.Qty)
                //{
                    var model = new YarnIssueViewModel();
                    model.YarnIssuanceId = item.YarnIssuanceId;
                    model.YarnIssuanceItemId = item.Id;
                    model.IssueNo = item.YarnIssuance.IssueNo;
                    model.IssueDate = item.YarnIssuance.IssueDate;
                    model.ItemId = item.ItemId;
                    model.ItemName = item.Item.Name;
                    model.ItemCode = item.Item.Code;
                    model.UOM = configValues.GetUom(item.Item.Unit);
                    model.Qty = item.Qty - item.ReturnQty;
                    model.Rate = item.Rate;
                    model.LineTotal = item.LineTotal;
                    list.Add(model);
                //}
            }
            return PartialView("_SaleReturnPopUp", list.ToList());
        }

        public IActionResult GetItemStockByBrandWarehouse(int itemId, int warehouseId, DateTime stockDate)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            try
            {
                var Stock = _dbContext.VwInvLedgers
               .Where(s => s.ItemId == itemId && s.CompanyId == companyId && s.WareHouseId == warehouseId && s.TransDate <= stockDate).Include(x => x.Item)
               .ToArray();
                var configValues = new ConfigValues(_dbContext);

                List<YarnIssueViewModel> list = new List<YarnIssueViewModel>();
                foreach (var item in Stock)
                {
                    var model = new YarnIssueViewModel();
                    model.ItemId = item.ItemId;
                    model.ItemName = item.Item.Name;
                    model.ItemCode = item.Item.Code;
                    model.Brand = item.Brand;
                    model.UOM = configValues.GetUom(item.Item.Unit);
                    model.Qty = item.Qty;
                    list.Add(model);
                }
                return PartialView("_YarnIssuancePopUp", list.ToList());
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public IActionResult GetStoreIssue()
        {
            try
            {
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                string configValues = _dbContext.AppCompanyConfigs
                              .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                              .Select(c => c.ConfigValue)
                              .FirstOrDefault();
                ViewBag.ReportPath = string.Concat(configValues, "Viewer", "?Report=Voucher&cId=", companyId, "&id=");
                ViewBag.ReportPath2 = string.Concat(configValues, "Viewer", "?Report=YarnIssuance&cId=", companyId, "&id=");
                //var searchValue = Request.Form["search[value]"].FirstOrDefault();

                var searchIssueNo = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchIssueDate = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchWarehouse = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchContractNo = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchVendor = Request.Form["columns[4][search][value]"].FirstOrDefault();
                var searchStatus = Request.Form["columns[5][search][value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var StoreIssuesData = (from StoreIssues in _dbContext.YarnIssuances.Include(x=>x.WeavingContract).ThenInclude(x=>x.Vendor).Include(x => x.WareHouse).Where(x => x.IsDeleted == false && x.CompanyId == companyId) select StoreIssues);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    StoreIssuesData = StoreIssuesData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                StoreIssuesData = !string.IsNullOrEmpty(searchIssueNo) ? StoreIssuesData.Where(m => m.IssueNo.ToString().ToUpper().Contains(searchIssueNo.ToUpper())) : StoreIssuesData;
                StoreIssuesData = !string.IsNullOrEmpty(searchIssueDate) ? StoreIssuesData.Where(m => m.IssueDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchIssueDate.ToUpper())) : StoreIssuesData;
                StoreIssuesData = !string.IsNullOrEmpty(searchWarehouse) ? StoreIssuesData.Where(m => m.WareHouse.ConfigValue.ToString().ToUpper().Contains(searchWarehouse.ToUpper())) : StoreIssuesData;
                StoreIssuesData = !string.IsNullOrEmpty(searchContractNo) ? StoreIssuesData.Where(m => m.WeavingContract.TransactionNo.ToString().ToUpper().Contains(searchContractNo.ToUpper())) : StoreIssuesData;
                StoreIssuesData = !string.IsNullOrEmpty(searchVendor) ? StoreIssuesData.Where(m => m.WeavingContract.Vendor.Name.ToString().ToUpper().Contains(searchVendor.ToUpper())) : StoreIssuesData;
                StoreIssuesData = !string.IsNullOrEmpty(searchStatus) ? StoreIssuesData.Where(m => m.Status.ToString().ToUpper().Contains(searchStatus.ToUpper())) : StoreIssuesData;

                recordsTotal = StoreIssuesData.Count();
                var data = StoreIssuesData.ToList();
                if (pageSize == -1)
                {
                    data = StoreIssuesData.Skip(skip).Take(recordsTotal == 0 ? 1 : pageSize == -1 ? recordsTotal : 1).ToList();
                }
                else
                {
                    data = StoreIssuesData.Skip(skip).Take(pageSize).ToList();
                }
                List<YarnIssueViewModel> Details = new List<YarnIssueViewModel>();
                foreach (var grp in data)
                {
                    YarnIssueViewModel invStoreIssueViewModel = new YarnIssueViewModel();
                    invStoreIssueViewModel.StoreIssuesDate = grp.IssueDate.ToString(Helpers.CommonHelper.DateFormat);
                    invStoreIssueViewModel.YarnIssuance = grp;
                    Details.Add(invStoreIssueViewModel);

                }
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = Details };
                return Ok(jsonData);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IActionResult GetWeavingContract(int weavingContractId)
        {
            GRWeavingContract contract = new GRWeavingContract();
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            if (weavingContractId != 0)
            {
                contract = _dbContext.GRWeavingContracts
                    //.Include(x=>x.GreigeQualityLoom)
                    .Include(x=>x.GreigeQuality)
                        .ThenInclude(x=>x.GRConstruction)
                            .ThenInclude(x=>x.Warp)
                                .ThenInclude(x=>x.UOM)
                    //.Include(x => x.GreigeQualityLoom)
                    .Include(x => x.GreigeQuality)
                        .ThenInclude(x => x.GRConstruction)
                            .ThenInclude(x => x.Weft)
                                .ThenInclude(x=>x.UOM)
                    .Include(x=>x.Vendor)
                    .FirstOrDefault(x => x.Id == weavingContractId);
                var warpBrand = this.GetConfig(companyId, "Brands", contract.GreigeQuality.GRConstruction.Warp.Id);
                var weftBrand = this.GetConfig(companyId, "Brands", contract.GreigeQuality.GRConstruction.Weft.Id);
                return Ok(new { Contract = contract, WarpBrand = warpBrand, WeftBrand = weftBrand });
            }
            return Ok(new { Contract = contract } );
        }

    }
}