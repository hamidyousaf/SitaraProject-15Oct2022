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
using Numbers.Repository.Inventory;
using System.Linq.Dynamic.Core;
using Numbers.Helpers;

namespace Numbers.Areas.Inventory.Controllers
{
    [Authorize]
    [Area("Inventory")]
    public class StoreIssueController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public StoreIssueController(NumbersDbContext context)
        {
            _dbContext = context;
        }

        public IActionResult Index()
        {
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userid = HttpContext.Session.GetString("UserId");


         
           
           
            string configValues = _dbContext.AppCompanyConfigs
                                .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                                .Select(c => c.ConfigValue)
                                .FirstOrDefault();
                                //string.Concat(configValue, "Viewer", "?Report=PRBasePrint&cId=", companyId, "&id=");
            ViewBag.ReportPath2 = string.Concat(configValues, "Viewer", "?Report=StoreIssue&cId=", companyId, "&id=");


            var storeIssueRepo = new StoreIssueRepo(_dbContext);
            //var configValues = new ConfigValues(_dbContext);
            // ViewBag.OperatingUnit = configValues.GetOrgValues(resp_Id, "Operating Unit", companyId);
            IEnumerable<InvStoreIssue> list = storeIssueRepo.GetAll(companyId);
            ViewBag.NavbarHeading = "List of Store Issues";
            return View(list);
        }
        public JsonResult checkProductCodeAlreadyExists(int code)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            System.Threading.Thread.Sleep(200);
            if (code == 0)
                return Json(0);
            var chkCode = _dbContext.InvStoreIssues.Where(a => a.IsDeleted == false && a.IssueNo == code && a.TransactionType== "Issue" && a.CompanyId == companyId).FirstOrDefault();
            return Json(chkCode == null ? 0 : 1);
        }
        public IActionResult Create(int id)
        {
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var storeIssueRepo = new StoreIssueRepo(_dbContext);
            var configValues = new ConfigValues(_dbContext);
            ViewBag.Counter = 0;
           // TempData["ShowRate"] = (from a in _dbContext.Users.Where(x => x.Id == userId) select a.ShowRate).FirstOrDefault();
           // ViewBag.Branch = new CommonDDL(_dbContext).GetBranchesbyId(userId);
            ViewBag.WareHouse = configValues.GetConfigValues("Inventory", "Ware House", companyId);
            // ViewBag.CostCenter = new SelectList(_dbContext.CostCenter, "Id", "Description").ToList();
            //ViewBag.CostCenter = new SelectList(_dbContext.AppCompanyConfigs.Where(x=>x.BaseId==11), "Id", "ConfigValue").ToList();
           
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

            ViewBag.OperatingUnit = configValues.GetOrgValues(resp_Id, "Operating Unit", companyId);
            ViewBag.Department = new SelectList(_dbContext.GLDivision.Where(x => x.CompanyId == companyId && x.IsDeleted != true).Select(x=> new ListOfValue { Id = x.Id, Name = x.Id + " - " + x.Name }).ToList(), "Id", "Name");

            if (id == 0)
            {
                var result = _dbContext.InvStoreIssues.Where(x => x.IsDeleted == false).ToList();
                if (result.Count > 0)
                {
                    ViewBag.IssueNo = _dbContext.InvStoreIssues.Select(x => x.IssueNo).Max() + 1;
                }
                else
                {
                    ViewBag.IssueNo = 1;
                }
                TempData["Mode"] = false;
                ViewBag.EntityState = "Create";
                ViewBag.NavbarHeading = "Create Store Issue";
                int maxissueNo = 1;
                var issue = _dbContext.InvStoreIssues.Where(c => c.CompanyId == companyId && c.IsDeleted==false).ToList();
                if (issue.Count > 0)
                {
                    maxissueNo = issue.Max(v => v.IssueNo);
                    ViewBag.Issue = maxissueNo + 1;
                }
                else
                {
                    ViewBag.Issue = maxissueNo;
                }
                // TempData["IssueNo"] = storeIssueRepo.StoreIssueCountNo(companyId);
                var model = new InvStoreIssueViewModel();
                return View(model);
            }
            else
            {
                TempData["Mode"] = true;
                ViewBag.Id = id;
                InvStoreIssueViewModel modelEdit = storeIssueRepo.GetById(id);
                ViewBag.Issue = modelEdit.IssueNo;
                InvStoreIssueItem[] storeIssueItems = storeIssueRepo.GetStoreIssueItems(id);
                modelEdit.InvStoreIssueItems = _dbContext.InvStoreIssueItems
                                        .Include(i => i.Item)
                                        .Include(i => i.StoreIssue)
                                          .Where(i => i.StoreIssueId == id && i.IsDeleted == false)
                                          .ToList();
                foreach (var item in modelEdit.InvStoreIssueItems)
                {
                    item.ItemName = item.Item.Code+" - "+item.Item.Name;
                    item.UnitName = (from a in _dbContext.AppCompanyConfigs.Where(x => x.Id == item.Item.Unit) select a.ConfigValue).FirstOrDefault();
                }
                ViewBag.SubDivision = new SelectList(_dbContext.GLSubDivision.Where(a => a.IsDeleted == false && a.Id == modelEdit.SubDepartmentId).ToList(), "Id", "Name");
                ViewBag.CostCenter = new SelectList(_dbContext.CostCenter.Where(a => a.IsDeleted == false && a.Id == modelEdit.CostCenterId).ToList(), "Id", "Description");

                ViewBag.Items = storeIssueItems;
               // TempData["IssueNo"] = modelEdit.IssueNo;
                if (modelEdit.Status != "Approved")
                {
                    ViewBag.EntityState = "Update";
                    ViewBag.NavbarHeading = "Store Issue";
                    ViewBag.TitleStatus = "Created";
                }
                return View(modelEdit);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Create(InvStoreIssueViewModel model,IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var storeIssueRepo = new StoreIssueRepo(_dbContext);
            if (model.Id == 0)
            {
                //for master table
                var storeIssue = new InvStoreIssue();
                storeIssue.IssueNo = model.IssueNo;
                storeIssue.IssueDate = model.IssueDate;
                storeIssue.WareHouseId = model.WareHouseId;
                storeIssue.CostCenterId = model.CostCenterId;
                storeIssue.DepartmentId = model.DepartmentId;
                storeIssue.SubDepartmentId = model.SubDepartmentId;
                storeIssue.Remarks = (collection["Remarks"][0]);
                storeIssue.TransactionType = "Issue";
                storeIssue.CompanyId = companyId;
                storeIssue.IsDeleted = false;
                storeIssue.Status = "Created";
                storeIssue.CreatedBy = userId;
                storeIssue.CreatedDate = DateTime.Now;
                _dbContext.InvStoreIssues.Add(storeIssue);
                await _dbContext.SaveChangesAsync();
                //for detail table
                for (int i = 0; i < collection["ItemId"].Count; i++)
                {
                    var invStoreIssueItem = new InvStoreIssueItem();
                    invStoreIssueItem.StoreIssueId = storeIssue.Id;
                    invStoreIssueItem.ItemId = Convert.ToInt32(collection["ItemId"][i]);
                    invStoreIssueItem.Qty = Math.Abs(Convert.ToDecimal(collection["Qty"][i]));
                    //invStoreIssueItem.Qty = -Math.Abs(Convert.ToDecimal(collection["Qty"][i]));
                    invStoreIssueItem.Rate = Convert.ToDecimal(collection["Rate"][i]);
                    //invStoreIssueItem.LineTotal = -Math.Abs(Convert.ToDecimal(collection["LineTotal"][i]));
                    invStoreIssueItem.LineTotal = Math.Abs(Convert.ToDecimal(collection["LineTotal"][i]));
                    invStoreIssueItem.IsDeleted = false;
                    invStoreIssueItem.CreatedBy = userId;
                    invStoreIssueItem.CreatedDate = DateTime.Now;
                    _dbContext.InvStoreIssueItems.Add(invStoreIssueItem);
                    await _dbContext.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                model.UpdatedBy = userId;
                model.CompanyId = companyId;
                //updating existing data
                var obj = _dbContext.InvStoreIssues.Find(model.Id);
                obj.IssueNo = model.IssueNo;
                obj.IssueDate = model.IssueDate;
                obj.WareHouseId = model.WareHouseId;
                obj.DepartmentId = model.DepartmentId;
                obj.SubDepartmentId = model.SubDepartmentId;
                obj.CostCenterId = model.CostCenterId;
                obj.Remarks = collection["Remarks"][0];
                obj.TransactionType = "Issue";
                obj.Status = "Created";
                obj.UpdatedBy = model.UpdatedBy;
                obj.CompanyId = model.CompanyId;
                obj.UpdatedDate = DateTime.Now;
                obj.IsDeleted = false;
                var entry = _dbContext.InvStoreIssues.Update(obj);
                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                await _dbContext.SaveChangesAsync();

                var removeItems = _dbContext.InvStoreIssueItems.Where(u => u.StoreIssueId == model.Id).ToList();
                if (removeItems != null)
                {
                    foreach (var item in removeItems)
                    {
                        _dbContext.InvStoreIssueItems.Remove(item);
                        _dbContext.SaveChanges();
                    }
                }

                //for detail table
                for (int i = 0; i < collection["ItemId"].Count; i++)
                {
                    var invStoreIssueItem = new InvStoreIssueItem();
                    invStoreIssueItem.StoreIssueId = model.Id;
                    invStoreIssueItem.ItemId = Convert.ToInt32(collection["ItemId"][i]);
                   // invStoreIssueItem.Qty = -Math.Abs(Convert.ToDecimal(collection["Qty"][i]));
                    invStoreIssueItem.Qty = Math.Abs(Convert.ToDecimal(collection["Qty"][i]));
                    invStoreIssueItem.Rate = Convert.ToDecimal(collection["Rate"][i]);
                   // invStoreIssueItem.LineTotal = -Math.Abs(Convert.ToDecimal(collection["LineTotal"][i]));
                    invStoreIssueItem.LineTotal = Math.Abs(Convert.ToDecimal(collection["LineTotal"][i]));
                    invStoreIssueItem.IsDeleted = false;
                    invStoreIssueItem.CreatedBy = userId;
                    invStoreIssueItem.CreatedDate = DateTime.Now;
                    _dbContext.InvStoreIssueItems.Add(invStoreIssueItem);
                    await _dbContext.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var storeIssueRepo = new StoreIssueRepo(_dbContext);
            bool isSuccess = await storeIssueRepo.Delete(id);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Store Issue has been deleted successfully.";
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
            var storeIssueRepo = new StoreIssueRepo(_dbContext,HttpContext);
            var isSuccess = await storeIssueRepo.Approve(id, userId, companyId);
            if (isSuccess == "true")
            {
                TempData["error"] = "false";
                TempData["message"] = "Store Issue has been approved successfully";
            }
            else if (isSuccess == "false")
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong.";
            }
            else  
            {
                TempData["error"] = "true";
                TempData["message"] = "Avg.Rate Must be Greater than Zero";
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult UnApprove()
        {
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            string userId = HttpContext.Session.GetString("UserId");
            ViewBag.UnApprove = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == resp_Id).FirstOrDefault().UnApprove;

            var model = new StoreIssueRepo(_dbContext,HttpContext).GetApproved();
            ViewBag.NavbarHeading = "Un-Approve Store Issue";
            return View(model);
        }
        [HttpGet]
        public async Task <IActionResult> UnApproveStoreIssue(int id)
        {
            try
            {
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                string userId = HttpContext.Session.GetString("UserId");
                 var result = await new StoreIssueRepo(_dbContext, HttpContext).UnApprove(id);
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
            return PartialView("_partialStoreIssueItem", model);
        }

        public IActionResult GetStoreIssueItems(int id, int itemId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var storeIssueRepo = new StoreIssueRepo(_dbContext);
            var viewModel = storeIssueRepo.GetStoreIssueItems(id, itemId);
            ViewBag.Counter = id;
            ViewBag.ItemId = viewModel.ItemId;
            return PartialView("_partialStoreIssueItem", viewModel);
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
            ViewBag.ReportPath2 = string.Concat(configValues, "Viewer", "?Report=StoreIssue&cId=", companyId, "&showRate=" + TempData["ShowRate"], "&id={0}");

            var storeIssue = _dbContext.InvStoreIssues.Include(i => i.WareHouse).Include(i => i.CostCenter)
            .Where(i => i.Id == id && i.CompanyId == companyId).FirstOrDefault();
            var storeIssueItems = _dbContext.InvStoreIssueItems
                                       .Include(i => i.Item)
                                       .Include(i => i.StoreIssue)
                                       .Where(i => i.StoreIssueId == id && i.IsDeleted == false)
                                         .ToList();
            ViewBag.NavbarHeading = "Store Issue";
            ViewBag.TitleStatus = "Approved";

            TempData["Detail"] = storeIssueItems;
            return View(storeIssue);
        }

        public IActionResult GetStoreIssue()
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

                var searchIssueNo = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchIssueDate = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchWarehouse = Request.Form["columns[2][search][value]"].FirstOrDefault();

                var searchDepartment = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchSubDepartment = Request.Form["columns[4][search][value]"].FirstOrDefault();

                var searchCostCenter = Request.Form["columns[5][search][value]"].FirstOrDefault();
                var searchCreatedBy = Request.Form["columns[6][search][value]"].FirstOrDefault();
                var searchStatus = Request.Form["columns[7][search][value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var StoreIssuesData = (from StoreIssues in _dbContext.InvStoreIssues.Include(x => x.Department).Include(x => x.SubDepartment).Include(x=>x.User).Include(x=>x.WareHouse).Include(x=>x.CostCenter).Where(x => x.IsDeleted == false && x.TransactionType == "Issue" && x.CompanyId == companyId) select StoreIssues);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    StoreIssuesData = StoreIssuesData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                StoreIssuesData = !string.IsNullOrEmpty(searchIssueNo) ? StoreIssuesData.Where(m => m.IssueNo.ToString().ToUpper().Contains(searchIssueNo.ToUpper())) : StoreIssuesData;
                StoreIssuesData = !string.IsNullOrEmpty(searchIssueDate) ? StoreIssuesData.Where(m => m.IssueDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchIssueDate.ToUpper())) : StoreIssuesData;
                StoreIssuesData = !string.IsNullOrEmpty(searchWarehouse) ? StoreIssuesData.Where(m => m.WareHouse.ConfigValue.ToString().ToUpper().Contains(searchWarehouse.ToUpper())) : StoreIssuesData;
                StoreIssuesData = !string.IsNullOrEmpty(searchCostCenter) ? StoreIssuesData.Where(m => m.CostCenter.Description.ToString().ToUpper().Contains(searchCostCenter.ToUpper())) : StoreIssuesData;
                StoreIssuesData = !string.IsNullOrEmpty(searchCreatedBy) ? StoreIssuesData.Where(m => m.User.FullName.ToString().ToUpper().Contains(searchCreatedBy.ToUpper())) : StoreIssuesData;
                StoreIssuesData = !string.IsNullOrEmpty(searchStatus) ? StoreIssuesData.Where(m => m.Status.ToString().ToUpper().Contains(searchStatus.ToUpper())) : StoreIssuesData;

                StoreIssuesData = !string.IsNullOrEmpty(searchDepartment) ? StoreIssuesData.Where(m => m.Department.Name.ToString().ToUpper().Contains(searchDepartment.ToUpper())) : StoreIssuesData;
                StoreIssuesData = !string.IsNullOrEmpty(searchSubDepartment) ? StoreIssuesData.Where(m => m.SubDepartment.Name.ToString().ToUpper().Contains(searchSubDepartment.ToUpper())) : StoreIssuesData;

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
    }
}