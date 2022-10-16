using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Repository.Planning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;

namespace Numbers.Areas.Planning.Controllers
{
    [Area("Planning")]
    [Authorize]
    public class SeasonalPlaningController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public SeasonalPlaningController(NumbersDbContext context)
        {
            _dbContext = context;
        }
        public IActionResult Create(int? id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            SeasonalPlaningViewModel seasonalPlaningViewModel = new SeasonalPlaningViewModel();
            seasonalPlaningViewModel.SeasonLOV = this.Seasons(0,companyId);
            if (id == null)
            {
                ViewBag.State = "Create";
                ViewBag.NavbarHeading = "Create Seasonal Planning";
                return View(seasonalPlaningViewModel);
            }
            else
            {
                
                seasonalPlaningViewModel.SeasonalPlaning = _dbContext.SeasonalPlaning.Find(id);
                seasonalPlaningViewModel.SeasonalPlaningDetail = _dbContext.SeasonalPlaningDetail
                    .Include(x=>x.FourthItemCategory)
                    .Include(x=>x.GreigeQuality)
                    .Include(x=>x.Season)
                    .Where(x=>x.SeasonalPlaningId == id).OrderBy(x=>x.Id).ToArray();
                ViewBag.State = "Update";
                ViewBag.NavbarHeading = "Update Seasonal Planning";
                seasonalPlaningViewModel.SeasonLOV = this.Seasons(seasonalPlaningViewModel.SeasonalPlaning.SeasonId, companyId);
                return View(seasonalPlaningViewModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(SeasonalPlaningViewModel model, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var repo = new SeasonalPlanningRepo(_dbContext);
            SeasonalPlaningDetail[] detail = JsonConvert.DeserializeObject<SeasonalPlaningDetail[]>(collection["ItemDetail"]);
            if (model.SeasonalPlaning.Id == 0)
            {
                model.SeasonalPlaning.TransactionNo = repo.Max(companyId);
                model.SeasonalPlaning.CreatedBy = userId;
                model.SeasonalPlaning.CompanyId = companyId;
                model.SeasonalPlaning.Resp_Id = resp_Id;
                model.SeasonalPlaningDetail = detail;
                bool isSuccess = await repo.Create(model);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = string.Format("Seasonal Planning. {0} has been created successfully.", model.SeasonalPlaning.TransactionNo);
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Something went wrong.";
                }
                return RedirectToAction("Create", "SeasonalPlaning");
            }
            else
            {
                model.SeasonalPlaning.UpdatedBy = userId;
                model.SeasonalPlaning.CompanyId = companyId;
                model.SeasonalPlaning.Resp_Id = resp_Id;
                model.SeasonalPlaningDetail = detail;
                bool isSuccess = await repo.Update(model);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = string.Format("Seasonal Planning. {0} has been updated successfully.", model.SeasonalPlaning.TransactionNo);
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Something went wrong.";
                }
                return RedirectToAction(nameof(Index));
            }
        }
        public IActionResult Index()
        {
            ViewBag.NavbarHeading = "List of Seasonal Planning";
            return View();
        }

        public async Task<IActionResult> Delete(int id)
        {
            var Repo = new SeasonalPlanningRepo(_dbContext);
            bool isSuccess = await Repo.Delete(id);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Seasonal Planning has been deleted successfully";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Seasonal Planning not found";
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public IActionResult GetSpecificationForPopUp(int[] skipIds)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            List<PlanSpecification> specification = _dbContext.PlanSpecifications
                .Include(x=>x.ItemCategoryLevel4)
                .Include(x=>x.GRQuality)
                .Where(x=> !skipIds.Contains(x.Id) && x.IsDeleted != true && x.CompanyId==companyId).OrderByDescending(x=>x.Id).ToList();
            return Ok(specification);
        }
        [HttpGet]
        public IActionResult GetSpecificationById(int specificationId, int seasonId)
        {
            if (specificationId != 0)
            {
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                PlanSpecification specification = _dbContext.PlanSpecifications
                    .Include(x => x.ItemCategoryLevel4)
                    .Include(x => x.GRQuality)
                    .FirstOrDefault(x => x.Id == specificationId && x.CompanyId == companyId);
                var season = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == seasonId).ConfigValue;
                var startWith = season.Substring(0, 1).ToUpper();
                var year = Convert.ToInt32(season.Split('-').Last().Trim());
                var x = new DateTime().AddYears(year - 1);
                var startDate = "";
                var endDate = "";
                switch (startWith)
                {
                    case "S":
                        startDate = x.AddMonths(3).ToString("MMM-yy");
                        endDate = x.AddMonths(8).ToString("MMM-yy");
                        break;
                    case "W":
                        startDate = x.AddMonths(9).ToString("MMM-yy");
                        endDate = x.AddMonths(14).ToString("MMM-yy");
                        break;
                }
                specification.StartDate = startDate;
                specification.EndDate = endDate;
                return Ok(specification);
            }
            return Ok(null);
        }
        public SelectList Seasons(int id,int companyId)
        {
           
            var Seasons = new SelectList((from b in _dbContext.AppCompanyConfigBases.Where(x => x.Name == "Seasons")
                                             join
                             c in _dbContext.AppCompanyConfigs on b.Id equals c.BaseId
                                             where  b.IsActive && b.IsDeleted == false
                                             select c
                                  ).ToList(), "Id", "ConfigValue");
            if(id!=0)
            {
                Seasons = new SelectList((from b in _dbContext.AppCompanyConfigBases.Where(x => x.Name == "Seasons")
                                          join
                          c in _dbContext.AppCompanyConfigs on b.Id equals c.BaseId
                                          where c.Id==id  && b.IsActive && b.IsDeleted == false
                                          select c
                                  ).ToList(), "Id", "ConfigValue");
            }
            return Seasons;
        }
        public IActionResult GetList()
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
                var searchSeason = Request.Form["columns[2][search][value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var data = (from m in _dbContext.SeasonalPlaning
                                .Include(x => x.Season)
                                .Where(x => x.IsDeleted != true && x.CompanyId == companyId) select m);

                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    data = data.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                data = !string.IsNullOrEmpty(searchTransNo) ? data.Where(m => m.TransactionNo.ToString().ToUpper().Contains(searchTransNo.ToUpper())) : data;
                data = !string.IsNullOrEmpty(searchTransDate) ? data.Where(m => m.TransactionDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchTransDate.ToUpper())) : data;
                data = !string.IsNullOrEmpty(searchSeason) ? data.Where(m => m.Season.ConfigValue.ToString().ToUpper().Contains(searchSeason.ToUpper())) : data;
                recordsTotal = data.Count();
                
                var result = data.ToList();
                if (pageSize == -1)
                {
                    result = data.Skip(skip).Take(recordsTotal == 0 ? 1 : pageSize == -1 ? recordsTotal : 1).ToList();
                }
                else
                {
                    result = data.Skip(skip).Take(pageSize).ToList();
                }

                List<SeasonalPlaningViewModel> viewModel = new List<SeasonalPlaningViewModel>();
                foreach (var item in result)
                {
                    SeasonalPlaningViewModel model = new SeasonalPlaningViewModel();
                    model.SeasonalPlaning = item;
                    model.SeasonalPlaning.Approve = approve;
                    model.SeasonalPlaning.Unapprove = unApprove;
                    model.SeasonalPlaning.ApprovedBy = item.TransactionDate.ToString(Helpers.CommonHelper.DateFormat);
                    viewModel.Add(model);
                }

                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = viewModel };
                return Ok(jsonData);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<IActionResult> Approve(int id)
        {
            string _userId = HttpContext.Session.GetString("UserId");
            SeasonalPlaning model1 = _dbContext.SeasonalPlaning.Find(id);
            model1.ApprovedBy = _userId;
            model1.ApprovedDate = DateTime.UtcNow;
            model1.IsApproved = true;
            _dbContext.SeasonalPlaning.Update(model1);
            _dbContext.SaveChanges();
           
            /////////////////////////////////////////////////////////////////// Auto Creation Greige Requisition /////////////////////////////////////

            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            GRGriegeRequisition model = new GRGriegeRequisition();


            //List<GRFoldingItems> customersDetailss = new List<GRFoldingItems>();
            int TransactionNo = 1;
            var list = _dbContext.GRGriegeRequisition.ToList();
            if (list.Count != 0)
            {
                TransactionNo = Convert.ToInt32(list.Select(x => x.TransactionNo).Max() + 1);
            }
            model.TransactionNo = TransactionNo;
            if (model1.TransactionNo != 0)
            {

                
                model.CompanyId = companyId;
                model.TransferToCompany = companyId == 15 ? 14 : companyId == 11 ? 1 : 0;
                model.Resp_ID = resp_Id;
                model.CreatedBy = userId;
                model.CreatedDate = Convert.ToDateTime(DateTime.Now);
                model.IsDeleted = false;
                model.TransactionDate = Convert.ToDateTime(DateTime.Now);
                model.TransactionNo = TransactionNo;
                model.ApprovedDate = model.ApprovedDate;
                model.UpdatedDate = model.UpdatedDate;
                model.UnApprovedDate = model.UnApprovedDate;
                model.DepartmentId = 5;
                model.SubDepartmentId = 10;
                model.IsApproved = false;
                model.Status = "Created";
                model.SPID = id;
                model.UnApprovedBy = model.UnApprovedBy;
                model.Deletedby = model.Deletedby;
                model.DeletedDate = model.DeletedDate;
                model.IsUsed = false;
                model.UpdatedBy = model.UpdatedBy;

                _dbContext.GRGriegeRequisition.Add(model);
                await _dbContext.SaveChangesAsync();
                List<GRGriegeRequisitionDetails> customersDetails = new List<GRGriegeRequisitionDetails>();
                List<SeasonalPlaningDetail> modelsd = _dbContext.SeasonalPlaningDetail.Where(x => x.SeasonalPlaningId==id).ToList();
                //  var warpCount = Convert.ToDouble(construction.Warp.Name.Substring(0, construction.Warp.Name.IndexOf("-")));
                //  var weftCount = Convert.ToDouble(construction.Weft.Name.Substring(0, construction.Weft.Name.IndexOf("-")));
                var SeasonalPlanData = (from a in _dbContext.SeasonalPlaning.Where(x => x.IsDeleted == false)
                                        join b in _dbContext.SeasonalPlaningDetail on a.Id equals b.SeasonalPlaningId
                                        join q in _dbContext.GRQuality.Include(x => x.GRConstruction) on b.GreigeQualityId equals q.Id
                                        join i in _dbContext.InvItems on q.ItemId equals i.Id
                                        join app in _dbContext.AppCompanyConfigs on i.Unit equals app.Id
                                        join gc in _dbContext.GRConstruction on q.GRConstructionId equals gc.Id
                                        where a.IsDeleted == false && a.IsApproved == true && a.Id == id
                                        select new
                                        {
                                            q.ItemId,
                                            SpNo = a.TransactionNo,
                                            SpecificationId = b.GreigeQualityId,
                                            Specification = (_dbContext.GRQuality.Where(x => x.Id == b.GreigeQualityId).Select(x => x.Description)).FirstOrDefault(),
                                            AvailableStock = (_dbContext.VwInvLedgers.Where(x => x.ItemId == q.ItemId).Select(x => x.Qty)).Sum(),
                                            Uom = app.ConfigValue,
                                            UnitId = i.Unit,
                                            ReserveQty = 0,
                                            Qty = 0,
                                            reed = gc.Reed,
                                            width = q.Width,
                                            pick = gc.Pick,
                                            warpcount = Convert.ToDecimal(q.GRConstruction.Warp.Name.Substring(0, q.GRConstruction.Warp.Name.IndexOf("-"))),
                                            weftCount = Convert.ToDecimal(q.GRConstruction.Weft.Name.Substring(0, q.GRConstruction.Weft.Name.IndexOf("-"))),
                                            WarpWeight = Math.Round(((((Convert.ToDecimal(Math.Round(gc.Reed * q.Width, 4)) * Convert.ToDecimal(1.0936)) / Convert.ToDecimal(q.GRConstruction.Warp.Name.Substring(0, q.GRConstruction.Warp.Name.IndexOf("-")))) / 20) / 40), 4),
                                            WeftWeight = Math.Round(((((Convert.ToDecimal(Math.Round(gc.Pick * q.Width, 4)) * Convert.ToDecimal(1.0936)) / Convert.ToDecimal(q.GRConstruction.Weft.Name.Substring(0, q.GRConstruction.Weft.Name.IndexOf("-")))) / 20) / 40), 4),

                                            // Warpbag = b.FabricConsumption * ( Convert.ToDecimal((((Convert.ToDecimal(gc.Reed * q.Width) * Convert.ToDecimal( 1.0936)) / Convert.ToDecimal( q.GRConstruction.Warp.Name.Substring(0, q.GRConstruction.Warp.Name.IndexOf("-")) )) / 20) / 40) ) /100,
                                            //Weftbag = b.FabricConsumption * ( Convert.ToDecimal((((Convert.ToDecimal(gc.Pick * q.Width) * Convert.ToDecimal(1.0936)) / Convert.ToDecimal(q.GRConstruction.Weft.Name.Substring(0, q.GRConstruction.Weft.Name.IndexOf("-")))) / 20) / 40) )/ 100,
                                            SeasonalFabricCons = b.FabricConsumption
                                        }).Distinct().ToList().GroupBy(x => new { x.SpecificationId })
                 .Select(x => new
                 {
                     ItemId = x.Select(x => x.ItemId).FirstOrDefault(),
                     SpNo = x.Select(x => x.SpNo).FirstOrDefault(),
                     SpecificationId = x.Select(x => x.SpecificationId).FirstOrDefault(),
                     Specification = x.Select(x => x.Specification).FirstOrDefault(),
                     AvailableStock = x.Select(x => x.AvailableStock).FirstOrDefault(),
                     Uom = x.Select(x => x.Uom).FirstOrDefault(),
                     UnitId = x.Select(x => x.UnitId).FirstOrDefault(),
                     ReserveQty = x.Select(x => x.ReserveQty).FirstOrDefault(),
                     Qty = 0,
                     reed = x.Select(x => x.reed).FirstOrDefault(),
                     width = x.Select(x => x.width).FirstOrDefault(),
                     pick = x.Select(x => x.pick).FirstOrDefault(),
                     warpcount = x.Select(x => x.warpcount).FirstOrDefault(),
                     weftCount = x.Select(x => x.weftCount).FirstOrDefault(),
                     WarpWeight = x.Select(x => x.WarpWeight).FirstOrDefault(),
                     WeftWeight = x.Select(x => x.WeftWeight).FirstOrDefault(),
                     SeasonalFabricCons = x.Sum(x => x.SeasonalFabricCons),
                 }).ToList();


                foreach (var grp in SeasonalPlanData)
                {
                    GRGriegeRequisitionDetails modeld = new GRGriegeRequisitionDetails();
                    modeld.SPNo = model1.TransactionNo ;
                    modeld.GriegeQualityId = grp.SpecificationId;
                    modeld.UOMId = grp.UnitId;
                    modeld.AvailableStock =Convert.ToInt32(grp.AvailableStock);
                    modeld.ReserveAvailableQty = 0;
                    modeld.Qty = Convert.ToInt32(grp.SeasonalFabricCons);
                    modeld.BalanceQty = Convert.ToInt32(grp.SeasonalFabricCons);
                    modeld.WarpBag = grp.SeasonalFabricCons * grp.WarpWeight / 100;
                    modeld.WeftBag = grp.SeasonalFabricCons * grp.WeftWeight / 100;
                    modeld.WarpBagWOQ = grp.WarpWeight.ToString();
                    modeld.WeftBagWOQ = grp.WeftWeight.ToString();
                    modeld.GRRequisitionId = model.Id;
                    modeld.IsUsed = false;
                    customersDetails.Add(modeld);
                }
                _dbContext.GRGriegeRequisitionDetails.AddRange(customersDetails);
                await _dbContext.SaveChangesAsync();
                //TempData["error"] = "false";
                //TempData["message"] = "Greige Requisition # " + TransactionNo + " has been saved successfully.";
                //return RedirectToAction(nameof(Index));

            }

            TempData["error"] = "false";
            TempData["message"] = "Seasonal Planing has been approved successfully.";
            return RedirectToAction(nameof(Index));


        }
        public async Task<IActionResult> UnApprove(int id)
        {
            string userId = HttpContext.Session.GetString("UserId");
            bool isSuccess;

            var deleteItem = _dbContext.SeasonalPlaning.Where(x => x.Id == id).FirstOrDefault();
            if (deleteItem == null)
            {
                isSuccess = false;
            }
            else
            {


                var findSeasonalPlan = _dbContext.PlanMonthlyPlanning.Where(x => x.SPId == id).FirstOrDefault();
                if (findSeasonalPlan == null)
                {
                    deleteItem.UnApprovedBy = userId;
                    deleteItem.UnApprovedDate= DateTime.UtcNow;
                    deleteItem.IsApproved = false;
                    var entry = _dbContext.SeasonalPlaning.Update(deleteItem);
                    entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                    await _dbContext.SaveChangesAsync();
                    isSuccess = true;

                }
                else
                {

                    isSuccess = false;

                }



            }

            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Seasonal Planning has been deleted successfully";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Seasonal Planning Consumed in Monthly planning";
            }


            //int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            //string _userId = HttpContext.Session.GetString("UserId");
            //SeasonalPlaning model = _dbContext.SeasonalPlaning.Find(id);
            //model.UnApprovedBy = _userId;
            //model.UnApprovedDate = DateTime.UtcNow;
            //model.IsApproved = false;
            //_dbContext.SeasonalPlaning.Update(model);
            //_dbContext.SaveChanges();
            //TempData["error"] = "false";
            //TempData["message"] = "Seasonal Planing has been UnApproved successfully.";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;

            var model = _dbContext.SeasonalPlaning
                .Include(p => p.SeasonalPlaningDetail)
                    .ThenInclude(p => p.FourthItemCategory)
                .Include(tiers => tiers.SeasonalPlaningDetail)
                    .ThenInclude(contact => contact.GreigeQuality)
                .Include(tiers => tiers.SeasonalPlaningDetail)
                    .ThenInclude(contact => contact.Season)
                .Where(p => p.Id == id && p.CompanyId==companyId).FirstOrDefault();
            ViewBag.NavbarHeading = "Approved Seasonal Planning";
            return View(model);
        }
    }
}
