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
using Numbers.Repository.Inventory;
using Numbers.Repository.Setup;
using System.Linq.Dynamic.Core;
using Numbers.Helpers;

namespace Numbers.Areas.Inventory.Controllers
{
    [Authorize]
    [Area("Inventory")]
    public class StoreIssueReturnController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public StoreIssueReturnController(NumbersDbContext context)
        {
            _dbContext = context;
        }

        public IActionResult Index()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userid = HttpContext.Session.GetString("UserId");
            var issueReturnRepo = new StoreIssueReturnRepo(_dbContext);
            IEnumerable<InvStoreIssue> list = issueReturnRepo.GetAll(companyId);
            ViewBag.NavbarHeading = "List of Store Issue Returns";
            return View(list);
        }
        public JsonResult checkProductCodeAlreadyExists(int code)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            System.Threading.Thread.Sleep(200);
            if (code == 0)
                return Json(0);
            var chkCode = _dbContext.InvStoreIssues.Where(a => a.IsDeleted == false && a.IssueNo == code && a.TransactionType == "Issue Return" && a.CompanyId == companyId).FirstOrDefault();
            return Json(chkCode == null ? 0 : 1);
        }
        public IActionResult Create(int id)
        {
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var issueReturnRepo = new StoreIssueReturnRepo(_dbContext);
            var configValues = new ConfigValues(_dbContext);
            ViewBag.Counter = 0;
            string userId = HttpContext.Session.GetString("UserId");
            //TempData["ShowRate"] = (from a in _dbContext.Users.Where(x => x.Id == userId) select a.ShowRate).FirstOrDefault();
          
            ViewBag.WareHouse = configValues.GetConfigValues("Inventory", "Ware House", companyId);
            ViewBag.CostCenter = configValues.GetConfigValues("Inventory", "Cost Center", companyId);
            
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

            ViewBag.OperatingUnit = configValues.GetOrgValues(resp_Id, "Operating Unit", companyId); 
            if (id == 0)
            {
                TempData["Mode"] = false;
                ViewBag.EntityState = "Create";
                ViewBag.NavbarHeading = "Create Store Issue Return";
                int maxissueNo = 1;
                var issue = _dbContext.InvStoreIssues.Where(c => c.CompanyId == companyId && c.IsDeleted == false).ToList();
                if (issue.Count > 0)
                {
                    maxissueNo = issue.Max(v => v.IssueNo);
                    ViewBag.Issue = maxissueNo + 1;
                }
                else
                {
                    ViewBag.Issue = maxissueNo;
                }
                //TempData["IssueNo"] = issueReturnRepo.StoreIssueCountNo(companyId);
                var model = new InvStoreIssueViewModel();
                return View(model);
            }
            else
            {
                TempData["Mode"] = true;
                ViewBag.Id = id;
                InvStoreIssueViewModel modelEdit = issueReturnRepo.GetById(id);
                InvStoreIssueItem[] storeIssueItems = issueReturnRepo.GetStoreIssueItems(id);
                ViewBag.Issue = modelEdit.IssueNo;
                ViewBag.Items = storeIssueItems;
                //TempData["IssueNo"] = modelEdit.IssueNo;
                modelEdit.InvStoreIssueItems = _dbContext.InvStoreIssueItems
                                        .Include(i => i.Item)
                                        .Include(i => i.StoreIssue)
                                          .Where(i => i.StoreIssueId == id && i.IsDeleted == false)
                                          .ToList();
                foreach (var item in modelEdit.InvStoreIssueItems)
                {
                    item.ItemName = item.Item.Code + "-" + item.Item.Name;
                    item.UnitName = (from a in _dbContext.AppCompanyConfigs.Where(x => x.Id == item.Item.Unit) select a.ConfigValue).FirstOrDefault();
                }
                if (modelEdit.Status != "Approved")
                {
                    ViewBag.EntityState = "Update";
                    ViewBag.NavbarHeading = "Store Issue Return";
                    ViewBag.TitleStatus = "Created";
                }
                return View(modelEdit);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Create(InvStoreIssueViewModel model, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var issueReturnRepo = new StoreIssueReturnRepo(_dbContext);
            if (model.Id == 0)
            {
                model.CreatedBy = userId;
                model.CompanyId = companyId;
                bool isSuccess = await issueReturnRepo.Create(model, collection);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = "Store Issue Return has been created successfully.";
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
                model.UpdatedBy = userId;
                model.CompanyId = companyId;
                bool isSuccess = await issueReturnRepo.Update(model, collection);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = "Store Issue Return has been updated successfully.";
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
            var issueReturnRepo = new StoreIssueReturnRepo(_dbContext);
            bool isSuccess = await issueReturnRepo.Delete(id);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Store Issue Return has been deleted successfully.";
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
            var issueReturnRepo = new StoreIssueReturnRepo(_dbContext, HttpContext);
            bool isSuccess = await issueReturnRepo.Approve(id, userId, companyId);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Store Issue Return has been approved successfully";
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

            var model = new StoreIssueReturnRepo(_dbContext, HttpContext).GetApproved();
            ViewBag.NavbarHeading = "Un-Approve Store Issue Return";
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> UnApproveStoreIssue(int id)
        {
            try
            {
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                string userId = HttpContext.Session.GetString("UserId");
                var result = await new StoreIssueReturnRepo(_dbContext, HttpContext).UnApprove(id);
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

        [HttpPost]
        public IActionResult PartialStoreIssueItem(int? counter)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            ViewBag.Counter = counter;
            var model = new InvStoreIssueViewModel();
            return PartialView("_partialStoreIssueReturnItem", model);
        }

        public IActionResult GetStoreIssueItems(int id, int itemId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var issueReturnRepo = new StoreIssueReturnRepo(_dbContext);
            var viewModel = issueReturnRepo.GetStoreIssueItems(id, itemId);
            ViewBag.Counter = id;
            ViewBag.ItemId = viewModel.ItemId;
            return PartialView("_partialStoreIssueReturnItem", viewModel);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            TempData["RoleName"] = (from a in _dbContext.Roles
                                    join b in _dbContext.UserRoles.Where(x => x.UserId == userId) on a.Id equals b.RoleId
                                    select a.Name).FirstOrDefault();
           // TempData["ShowRate"] = (from a in _dbContext.Users.Where(x => x.Id == userId) select a.ShowRate).FirstOrDefault();
            string configValues = _dbContext.AppCompanyConfigs
                                .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                                .Select(c => c.ConfigValue)
                                .FirstOrDefault();
            ViewBag.ReportPath = string.Concat(configValues, "Viewer", "?Report=Voucher&cId=", companyId, "&id={0}");
            ViewBag.ReportPath2 = string.Concat(configValues, "Viewer", "?Report=StoreIssueReturn&cId=", companyId, "&showRate=" + TempData["ShowRate"], "&id={0}");
            var storeIssue = _dbContext.InvStoreIssues.Include(i => i.WareHouse).Include(i => i.CostCenter)
            .Where(i => i.Id == id && i.CompanyId == companyId).FirstOrDefault();
            var storeIssueItems = _dbContext.InvStoreIssueItems
                                       .Include(i => i.Item)
                                       .Include(i => i.StoreIssue)
                                       .Where(i => i.StoreIssueId == id && i.IsDeleted == false)
                                         .ToList();
            ViewBag.NavbarHeading = "Store Issue Return";
            ViewBag.TitleStatus = "Approved";

            TempData["Detail"] = storeIssueItems;
            return View(storeIssue);
        }
        //public IActionResult GetIssueToReturnByBranchId(int id, int[] skipIds)
        //{
        //    int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
        //    var saleReturnRepo = new StoreIssueReturnRepo(_dbContext);
        //    var configValues = new ConfigValues(_dbContext);
        //    var invoice = saleReturnRepo.GetStoreReturnById(id, skipIds, companyId);
        //    List<InvStoreIssueViewModel> list = new List<InvStoreIssueViewModel>();
        //    foreach (var item in invoice)
        //    {
        //        var model = new InvStoreIssueViewModel();
        //        model.StoreIssueId = item.StoreIssueId;
        //        model.StoreIssueItemId = item.Id;
        //        model.IssueNo = item.Invoice.InvoiceNo;
        //         model.IssueDate = item.Invoice.InvoiceDate;
        //        model.ItemId = item.ItemId;
        //        model.ItemName = item.Item.Name;
        //        model.ItemCode = item.Item.Code;
        //        model.UOM = configValues.GetUom(item.Item.Unit);
        //        model.Qty = item.Qty;
        //        model.Rate = item.Rate;
        //        model.LineTotal = item.LineTotal;
        //        list.Add(model);
        //    }
        //    return PartialView("_SaleReturnPopUp", list.ToList());
        //}

        [HttpGet]
        public IActionResult GetSaleInvoiceItems(int saleInvoiceItemId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var saleReturnRepo = new StoreIssueReturnRepo(_dbContext);
            var item = saleReturnRepo.GetSaleInvoiceItems(saleInvoiceItemId);
            ViewBag.Counter = saleInvoiceItemId;
            ViewBag.ItemId = item.ItemId;
            var appTaxRepo = new AppTaxRepo(_dbContext);
            item.TaxList = appTaxRepo.GetTaxes(companyId);
            return Ok(item);
        }

        public IActionResult GetItemById(int id)
        {
            var obj = _dbContext.InvItems.Where(x => x.Id == id && x.IsDeleted == false).FirstOrDefault();
            obj.Code = (from a in _dbContext.AppCompanyConfigs.Where(x => x.Id == obj.Unit) select a.ConfigValue).FirstOrDefault();
            return Json(obj);
        }


        public IActionResult GetInvoicesToReturnByCustomerId(int id, int[] skipIds)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var saleReturnRepo = new StoreIssueReturnRepo(_dbContext);
            var configValues = new ConfigValues(_dbContext);
            var api = new ApiController(_dbContext);
            var invoice = saleReturnRepo.GetStoreIssueById(id, skipIds, companyId);
            List<InvStoreIssueViewModel> list = new List<InvStoreIssueViewModel>();
            foreach (var item in invoice)
            {
                //if (item.ReturnQty != item.Qty)
                //{
                    var model = new InvStoreIssueViewModel();
                    model.StoreIssueId = item.StoreIssueId;
                    model.StoreIssueItemId = item.Id;
                    model.IssueNo = item.StoreIssue.IssueNo;
                    model.IssueDate = item.StoreIssue.IssueDate;
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
        public IActionResult GetList()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            try
            {
                int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
                string userId = HttpContext.Session.GetString("UserId");
                var approve = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == resp_Id).FirstOrDefault().Approve;
                var unApprove = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == resp_Id).FirstOrDefault().UnApprove;

                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                //var searchValue = Request.Form["search[value]"].FirstOrDefault();

                var searchRetNo = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchRetDate = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchWarehouse = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchCostCenter = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchStatus = Request.Form["columns[4][search][value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var StoreIssuesData = (from StoreIssues in _dbContext.InvStoreIssues.Include(x=>x.WareHouse).Include(x=>x.CostCenter).Where(x => x.IsDeleted == false && x.TransactionType == "Issue Return" && x.CompanyId == companyId) select StoreIssues);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    StoreIssuesData = StoreIssuesData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                
                StoreIssuesData = !string.IsNullOrEmpty(searchRetNo) ? StoreIssuesData.Where(m => m.IssueNo.ToString().ToUpper().Contains(searchRetNo.ToUpper())) : StoreIssuesData;
                StoreIssuesData = !string.IsNullOrEmpty(searchRetDate) ? StoreIssuesData.Where(m => m.IssueDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchRetDate.ToUpper())) : StoreIssuesData;
                StoreIssuesData = !string.IsNullOrEmpty(searchWarehouse) ? StoreIssuesData.Where(m => m.WareHouse.ConfigValue.ToString().ToUpper().Contains(searchWarehouse.ToUpper())) : StoreIssuesData;
                StoreIssuesData = !string.IsNullOrEmpty(searchCostCenter) ? StoreIssuesData.Where(m => m.CostCenter.Description.ToString().ToUpper().Contains(searchCostCenter.ToUpper())) : StoreIssuesData;
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
                List<InvStoreIssueViewModel> Details = new List<InvStoreIssueViewModel>();
                foreach (var grp in data)
                {
                    InvStoreIssueViewModel invStoreIssueViewModel = new InvStoreIssueViewModel();
                    invStoreIssueViewModel.StoreIssuesDate = grp.IssueDate.ToString(Helpers.CommonHelper.DateFormat);
                    invStoreIssueViewModel.StoreIssue = grp;
                    invStoreIssueViewModel.StoreIssue.Approve = approve;
                    invStoreIssueViewModel.StoreIssue.Unapprove = unApprove;

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
    }
}