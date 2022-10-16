using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nancy.Extensions;
using Newtonsoft.Json;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Repository.Helpers;
using Numbers.Repository.Planning;
using System.Linq.Dynamic.Core;
using Numbers.Repository.Greige;

namespace Numbers.Areas.Planning.Controllers
{
    [Area("Planning")]
    [Authorize]
    public class ProductionOrderController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public ProductionOrderController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Create(int? id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            var configValues = new ConfigValues(_dbContext);
            ProductionOrderViewModel productionOrderViewModel = new ProductionOrderViewModel();

            //var items = _dbContext.AppCompanySetups.Where(x => x.Name == "Local Finish ItemCode").FirstOrDefault().Value;
            //productionOrderViewModel.SecondLevelCategoryLOV = new SelectList(from ac in _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.CategoryLevel == 2  && x.Code.StartsWith(items)).OrderBy(x => x.Code).ToList()
            //                                                select new
            //                                                {
            //                                                    Id = ac.Id,
            //                                                    Name = ac.Code + " - " + ac.Name
            //                                                }, "Id", "Name");

            productionOrderViewModel.SecondLevelCategoryLOV = configValues.GetSecondCategoryByResp(resp_Id);
            ViewBag.Supplier = new SelectList(_dbContext.APSuppliers.Include(x => x.Account).Where(x => x.Account.Code.StartsWith("2.02.04.0006") && x.IsApproved == true && x.IsActive /*&& x.CompanyId == companyId*/).ToList(), "Id", "Name");
            productionOrderViewModel.ProcessLOV = this.GetConfig(0, companyId, "Process");
            productionOrderViewModel.TypeLOV = this.GetConfig(0, companyId, "Type");
            if (id == null)
            {
                ViewBag.State = "Create";
                ViewBag.NavbarHeading = "Create Production Order";
                //productionOrderViewModel.MonthlyPlanningLOV = new SelectList(_dbContext.PlanMonthlyPlanning.Where(x => !x.IsDeleted && x.IsApproved).OrderByDescending(x => x.Id), "Id", "TransactionNo");
                var monthlyPlanningLOV = (from master in _dbContext.PlanMonthlyPlanning.Where(x => !x.IsDeleted && x.IsApproved && x.CompanyId==companyId).OrderByDescending(x => x.Id) join
                                                                             detail in _dbContext.PlanMonthlyPlanningItems.ToList() on master.Id  equals detail.PlannigId
                                                                             where detail.MonthlyFabricConsBalance > 0
                                                                             select new ListOfValue
                                                                             {
                                                                                 Id = master.Id,
                                                                                 TransactionNo = master.TransactionNo
                                                                             }).ToList();



                var data = monthlyPlanningLOV.GroupBy(x => new { x.Id})
                 .Select(item => new ListOfValue
                 {
                     Id = item.Select(x => x.Id).FirstOrDefault(),
                     TransactionNo = item.Select(x => x.TransactionNo).FirstOrDefault(),
                 }).ToList();
                productionOrderViewModel.MonthlyPlanningLOV = new SelectList(data, "Id", "TransactionNo");


                return View(productionOrderViewModel);
            }
            else
            {
                var versions = new SelectList((from b in _dbContext.AppCompanyConfigBases.Where(x => x.Name == "Version")
                                               join
                               c in _dbContext.AppCompanyConfigs on b.Id equals c.BaseId
                                               where  b.IsActive
                                               select c
                                      ).ToList(), "Id", "ConfigValue");
                var processType = new SelectList((from b in _dbContext.AppCompanyConfigBases.Where(x => x.Name == "Process Type")
                                                  join
                                  c in _dbContext.AppCompanyConfigs on b.Id equals c.BaseId
                                                  where  b.IsActive
                                                  select c
                                      ).ToList(), "Id", "ConfigValue");
                var VersionConversion = new SelectList((from b in _dbContext.AppCompanyConfigBases.Where(x => x.Name == "Version Conversion")
                                                  join
                                  c in _dbContext.AppCompanyConfigs on b.Id equals c.BaseId
                                                  where b.IsActive
                                                  select c
                                      ).ToList(), "ConfigValue", "ConfigValue");
                productionOrderViewModel.ProductionOrder = _dbContext.ProductionOrders.Include(x=>x.ProductionOrderItems).ThenInclude(x=>x.FourthItemCategory)
                    .Include(x => x.ProductionOrderItems).ThenInclude(x => x.Type)
                    .Include(x => x.ProductionOrderItems).ThenInclude(x => x.Item)
                    .Include(x => x.ProductionOrderItems).ThenInclude(x => x.MPDetail).FirstOrDefault(x => x.Id == id && x.CompanyId==companyId);
                productionOrderViewModel.MonthlyPlanningLOV = new SelectList(_dbContext.PlanMonthlyPlanning.Where(x => x.Id == productionOrderViewModel.ProductionOrder.MonthlyPlanningId && x.CompanyId==companyId), "Id", "TransactionNo");
                productionOrderViewModel.VersionLOV = versions;
                productionOrderViewModel.ProcessTypeLOV = processType;
                productionOrderViewModel.VersionConversionLOV = VersionConversion;
                ViewBag.State = "Update";
                return View(productionOrderViewModel);
            }
        }
        public IActionResult Index()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var ContractRepo = new GRNRepo(_dbContext);
            string configs = _dbContext.AppCompanyConfigs
                .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                .Select(c => c.ConfigValue)
                .FirstOrDefault();
            ViewBag.ReportPath = string.Concat(configs, "Viewer", "?Report=ProductionOrderBP&cId=", companyId, "&id=");
            ViewBag.NavbarHeading = "List of Production Order";
            return View();
        }
        public IActionResult GetFourthCatFromMP(int monthlyPlanningId, int secondItemCategoryId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var fourthCategoryFromMP = (from master in _dbContext.PlanMonthlyPlanning.Where(x=>!x.IsDeleted && x.CompanyId==companyId) join
            detail in _dbContext.PlanMonthlyPlanningItems.Where(x=>x.MonthlyFabricConsBalance > 0) on master.Id equals detail.PlannigId
            join L4 in _dbContext.InvItemCategories on detail.ForthLevelCategoryId equals L4.Id join
            L3 in _dbContext.InvItemCategories on L4.ParentId equals L3.Id 
            join L2 in _dbContext.InvItemCategories on L3.ParentId equals L2.Id
            where master.Id == monthlyPlanningId && L2.Id == secondItemCategoryId
            select new ListOfValue
            {
                Id = L4.Id,
                Name = L4.Code + " - "+ L4.Name
            });
            var data = fourthCategoryFromMP.GroupBy(x => new { x.Id})
                 .Select(item => new ListOfValue
                 {
                     Id = item.Select(x => x.Id).Max(),
                     Name = item.Select(x => x.Name).Max(),
                 }).ToList();
            return Ok(data);
        }
        public IActionResult GetMonthlyPlanningDetail(int monthlyPlanningId,int fourthCategoryId, string greigeQuality)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var a = greigeQuality.Split(" - ")[1].Trim().ToString();
            var items = _dbContext.PlanMonthlyPlanningItems.FirstOrDefault(x => x.PlannigId == monthlyPlanningId && x.ForthLevelCategoryId == fourthCategoryId && x.PlanSpecificationId == a);
            return Ok(items);
        }
        public IActionResult GetGreige(int fourthCategoryId, int monthlyPlanningId)
        {
            var items = _dbContext.PlanMonthlyPlanningItems.Where(x => x.PlannigId == monthlyPlanningId && x.ForthLevelCategoryId == fourthCategoryId).Select(x => new ListOfValue { GreigeId = $"{x.Id}-{x.spec_Id}", Name = x.spec_Id + " - " + x.PlanSpecificationId }).ToList();
            return Ok(items);
        }
        public IActionResult GetItems(int fourthCategoryId)
        {
            var items = _dbContext.InvItems.Include(x => x.Category).Where(x => !x.IsDeleted && x.Category.Id == fourthCategoryId).Select(x => new ListOfValue { Id = x.Id, Name = x.Code + " - "+ x.Name }).ToList();
            return Ok(items);
        }
        public SelectList GetConfig(int id, int companyId, string value)
        {

            var Seasons = new SelectList((from b in _dbContext.AppCompanyConfigBases.Where(x => x.Name == value)
                                          join
                          c in _dbContext.AppCompanyConfigs on b.Id equals c.BaseId
                                          where  b.IsActive && c.IsDeleted == false
                                          select c
                                  ).ToList(), "Id", "ConfigValue"); ;
            if (id != 0)
            {
                Seasons = new SelectList((from b in _dbContext.AppCompanyConfigBases.Where(x => x.Name == value)
                                          join
                          c in _dbContext.AppCompanyConfigs on b.Id equals c.BaseId
                                          where c.Id == id && b.IsActive
                                          select c
                                  ).ToList(), "Id", "ConfigValue");
            }
            return Seasons;
        }
        public IActionResult GetVersionAndProcessType()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var versions = new SelectList((from b in _dbContext.AppCompanyConfigBases.Where(x => x.Name == "Version")
                                          join
                          c in _dbContext.AppCompanyConfigs on b.Id equals c.BaseId
                                          where  b.IsActive
                                          select c
                                  ).ToList(), "Id", "ConfigValue");
            var processType = new SelectList((from b in _dbContext.AppCompanyConfigBases.Where(x => x.Name == "Process Type")
                                           join
                           c in _dbContext.AppCompanyConfigs on b.Id equals c.BaseId
                                           where  b.IsActive
                                           select c
                                  ).ToList(), "Id", "ConfigValue");

            var versionConversion = new SelectList((from b in _dbContext.AppCompanyConfigBases.Where(x => x.Name == "Version Conversion")
                                              join
                              c in _dbContext.AppCompanyConfigs on b.Id equals c.BaseId
                                              where b.IsActive
                                              select c
                                  ).ToList(), "Id", "ConfigValue");

            return Ok(new { Versions = versions , ProcessType = processType, VersionConversion = versionConversion });
        }
        [HttpPost]
        public async Task<IActionResult> Create(ProductionOrderViewModel model, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var repo = new ProductionOrderRepo(_dbContext);
            ProductionOrderItem[] detail = JsonConvert.DeserializeObject<ProductionOrderItem[]>(collection["ItemDetail"]);
            int totalQty = Convert.ToInt32(collection["TotalQty"]);
            if (model.ProductionOrder.Id == 0)
            {
                model.ProductionOrder.TransactionNo = repo.Max(companyId);
                model.ProductionOrder.CreatedBy = userId;
                model.ProductionOrder.CompanyId = companyId;
                model.ProductionOrder.TotalQty = totalQty;
                model.ProductionOrder.Resp_Id = resp_Id;
                model.ProductionOrderItems = detail;
                bool isSuccess = await repo.Create(model);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = string.Format("Production Order. {0} has been created successfully.", model.ProductionOrder.TransactionNo);
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Something went wrong.";
                }
                return RedirectToAction("Create", "ProductionOrder");
            }
            else
            {
                model.ProductionOrder.UpdatedBy = userId;
                model.ProductionOrder.CompanyId = companyId;
                model.ProductionOrder.Resp_Id = resp_Id;
                model.ProductionOrder.TotalQty = totalQty;
                model.ProductionOrderItems = detail;
                bool isSuccess = await repo.Update(model);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = string.Format("Production Order. {0} has been updated successfully.", model.ProductionOrder.TransactionNo);
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Something went wrong.";
                }
                return RedirectToAction(nameof(Index));
            }
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
                var searchPONo = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchPODate = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchMPNo = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchPlanOf = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchQuantity = Request.Form["columns[4][search][value]"].FirstOrDefault();
                var searchStatus = Request.Form["columns[5][search][value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var Data = (from records in _dbContext.ProductionOrders.Include(x=>x.ProductionOrderItems).Include(x => x.MonthlyPlanning).Where(x => x.IsDeleted == false && x.CompanyId == companyId) select records);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    Data = Data.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                Data = !string.IsNullOrEmpty(searchPONo) ? Data.Where(m => m.TransactionNo.ToString().ToLower().Contains(searchPONo.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchPODate) ? Data.Where(m => m.TransactionDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchPODate.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchMPNo) ? Data.Where(m => m.MonthlyPlanning.TransactionNo.ToString().ToUpper().Contains(searchMPNo.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchPlanOf) ? Data.Where(m => m.PlanOf.ToString().ToLower().Contains(searchPlanOf.ToLower())) : Data;
                Data = !string.IsNullOrEmpty(searchQuantity) ? Data.Where(m => m.MonthlyQuantity.ToString().ToUpper().Contains(searchQuantity.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchStatus) ? Data.Where(m => m.Status.ToString().ToUpper().Contains(searchStatus.ToUpper())) : Data;

                recordsTotal = Data.Count();
                var data = Data.ToList();
                if (pageSize == -1)
                {
                    data = Data.Skip(skip).Take(recordsTotal == 0 ? 1 : pageSize == -1 ? recordsTotal : 1).ToList();
                }
                else
                {
                    data = Data.Skip(skip).Take(pageSize).ToList();
                }
                List<ProductionOrderViewModel> Details = new List<ProductionOrderViewModel>();
                foreach (var grp in data)
                {
                    ProductionOrderViewModel aRSaleOrderViewModel = new ProductionOrderViewModel();
                    aRSaleOrderViewModel.ProductionOrder = grp;
                    aRSaleOrderViewModel.ProductionOrder.Approve = approve;
                    aRSaleOrderViewModel.ProductionOrder.Unapprove = unApprove;
                    int count = grp.ProductionOrderItems.GroupBy(x => x.GroupId).Select(x => new  { 
                        Id = x.Select(x=>x.SuitMeters).FirstOrDefault()
                    }).Select(x=>x.Id).Sum();

                    aRSaleOrderViewModel.Date = grp.TransactionDate.ToString(Helpers.CommonHelper.DateFormat);
                    aRSaleOrderViewModel.SuiteMeters = Convert.ToInt32(count);
                    Details.Add(aRSaleOrderViewModel);
                }
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = Details };
                return Ok(jsonData);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<IActionResult> Delete(int id)
        {
            var Repo = new ProductionOrderRepo(_dbContext);
            bool isSuccess = await Repo.Delete(id);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Production Order has been deleted.";
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
            string userId = HttpContext.Session.GetString("UserId");
            var Repo = new ProductionOrderRepo(_dbContext);
            bool isSuccess = await Repo.Approve(id, userId);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Production Order has been approved successfully";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong.";
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult UnApprove(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            ProductionOrder model = _dbContext.ProductionOrders.Find(id);
            model.UnApprovedBy = _userId;
            model.UnApprovedDate = DateTime.UtcNow;
            model.IsApproved = false;
            model.Status = "Created";
            _dbContext.ProductionOrders.Update(model);
            _dbContext.SaveChanges();
            TempData["error"] = "false";
            TempData["message"] = "Production Order has been UnApproved.";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;

            ProductionOrder model = _dbContext.ProductionOrders
                .Include(p => p.MonthlyPlanning)
                .Include(p => p.Vendor)
                //.Include(p => p.Process)
                .Include(p => p.ProductionOrderItems)
                    .ThenInclude(x => x.FourthItemCategory)
                .Include(p => p.ProductionOrderItems)
                    .ThenInclude(x => x.Item)
                .Include(p => p.ProductionOrderItems)
                    .ThenInclude(x => x.Version)
                .Include(p => p.ProductionOrderItems)
                    .ThenInclude(x => x.ProcessType)
                .Where(p => p.Id == id && p.CompanyId==companyId).FirstOrDefault();
            ViewBag.NavbarHeading = "Approved Production Order";
            return model != null ? View(model) : View(nameof(Index));
        }
        public async Task<IActionResult> HandleBalanceQuantity(int MPDetailId, int Quantity)
        {
            var monthlyPlanningItems = await _dbContext.PlanMonthlyPlanningItems.FirstOrDefaultAsync(x => x.Id == MPDetailId);
            if (monthlyPlanningItems != null)
            {
                monthlyPlanningItems.MonthlyFabricConsBalance = monthlyPlanningItems.MonthlyFabricConsBalance + Quantity;
                _dbContext.PlanMonthlyPlanningItems.Update(monthlyPlanningItems);
                _dbContext.SaveChanges();
            }
            return Ok();
        }
    }
}
