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
using Numbers.Repository.Greige;
using System.Linq.Dynamic.Core;
using Numbers.Repository.Helpers;
using Numbers.Entity.ViewModels;

namespace Numbers.Areas.Planning.Controllers
{
    [Area("Planning")]
    public class MonthlyPlanningController : Controller
    {
        
        private readonly NumbersDbContext _dbContext;
        public MonthlyPlanningController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            //int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            ViewBag.NavbarHeading = "List of Monthly Planning";
            return View();

        }

        public IActionResult Create(int Id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            try
            {
                
                if (Id == 0)
                {
                    var data = from detail in _dbContext.SeasonalPlaningDetail join
                            master in _dbContext.SeasonalPlaning.Where(a => a.IsDeleted != true && a.IsApproved == true && a.CompanyId==companyId)
                            on detail.SeasonalPlaningId equals master.Id
                               where detail.BalanceFabricConsumption > 0
                            select master;
                    var lov = data.GroupBy(x => x.Id).Select(x=> new  ListOfValue{ Id = x.Select(a=>a.Id).FirstOrDefault(), TransactionNo = x.Select(a=>a.TransactionNo).FirstOrDefault()});
                    ViewBag.SPNo = new SelectList(lov.ToList().OrderByDescending(x=>x.Id), "Id", "TransactionNo");
                    var model = new VMMonthllyPlanning();
                    ViewBag.EntityState = "Create";
                    ViewBag.NavbarHeading = "Create Monthly Planning";
                    return View(model);
                }
                else
                {
                    ViewBag.EntityState = "Update";
                    ViewBag.NavbarHeading = "Update Monthly Planning";
                    VMMonthllyPlanning model = new VMMonthllyPlanning();
                    VMMonthllyPlanning item = new VMMonthllyPlanning(); // 
                    item.PlanMonthly = _dbContext.PlanMonthlyPlanning.Where(x => x.Id == Id).FirstOrDefault();
                    ViewBag.SPNo = new SelectList(_dbContext.SeasonalPlaning.Where(a => a.Id == item.PlanMonthly.SPId).OrderByDescending(x=>x.Id).ToList(), "Id", "TransactionNo");
                    ViewBag.Planofmonth = new SelectList(_dbContext.PlanMonthlyPlanning.Where(a => a.Id == item.PlanMonthly.Id), "Planofmonth", "Planofmonth");


                    model.PlanMonthly.Id = item.PlanMonthly.Id;
                    model.PlanMonthly.TransactionNo = item.PlanMonthly.TransactionNo;
                    TempData["Code"]= item.PlanMonthly.TransactionNo;
                    model.PlanMonthly.TransactionDate = item.PlanMonthly.TransactionDate;
                    model.PlanMonthly.SPId = item.PlanMonthly.SPId;
                    model.PlanMonthly.Planofmonth = item.PlanMonthly.Planofmonth;
                    model.PlanMonthly.Remarks = item.PlanMonthly.Remarks;
                    

                    var det = _dbContext.PlanMonthlyPlanningItems.Where(x => x.PlannigId == Id).ToList();
                    List<PlanMonthlyPlanningItems> Details = new List<PlanMonthlyPlanningItems>();

                    foreach (var grp in det)
                    {
                        PlanMonthlyPlanningItems modeld = new PlanMonthlyPlanningItems();
                        modeld.Id = grp.Id;
                        modeld.PlannigId = grp.PlannigId;
                        modeld.ForthLevelCategoryId = grp.ForthLevelCategoryId;
                        modeld.CategoryName = grp.CategoryName;
                        modeld.PlanSpecificationId = grp.PlanSpecificationId;
                        modeld.spec_Id = grp.spec_Id;
                        modeld.MonthId = grp.MonthId;
                        modeld.SeasonalDesignCount = grp.SeasonalDesignCount;
                        modeld.SeasonalDetailId = grp.SeasonalDetailId;
                        modeld.SeasonalRunQty = grp.SeasonalRunQty;
                        modeld.SeasonalFabricCons = grp.SeasonalFabricCons;
                        modeld.MonthlyDesignCount = grp.MonthlyDesignCount;
                        modeld.MonthlyFabricConsBalance = grp.MonthlyFabricConsBalance;
                        modeld.MonthlyRunQty = grp.MonthlyRunQty;
                        modeld.MonthlyFabricCons = grp.MonthlyFabricCons;
                        Details.Add(modeld);
                    }

                    model.PlanMonthlyPlanningDetail = Details.OrderBy(x => x.Id).ToList();
                    return View(model);
                }
                
            }
            catch (Exception)
            {

                return View(new VMMonthllyPlanning());
            }
            
        }


        public IActionResult Details(int Id)
        {
            try
            {

              //  ViewBag.SPNo = new SelectList(_dbContext.SeasonalPlaning.Where(a => a.IsDelete != true).ToList(), "Id", "TransactionNo");
               
                    ViewBag.EntityState = "Detials";
                    ViewBag.NavbarHeading = "Detail Monthly Planning";
                    VMMonthllyPlanning model = new VMMonthllyPlanning();
                    VMMonthllyPlanning item = new VMMonthllyPlanning(); // 
                    item.PlanMonthly = _dbContext.PlanMonthlyPlanning.Where(x => x.Id == Id).FirstOrDefault();


                    model.PlanMonthly.Id = item.PlanMonthly.Id;
                    model.PlanMonthly.TransactionNo = item.PlanMonthly.TransactionNo;
                    TempData["Code"] = item.PlanMonthly.TransactionNo;
                    model.PlanMonthly.TransactionDate = item.PlanMonthly.TransactionDate;
                    model.PlanMonthly.SPId = item.PlanMonthly.SPId;
                    model.PlanMonthly.Planofmonth = item.PlanMonthly.Planofmonth;
                    model.PlanMonthly.Remarks = item.PlanMonthly.Remarks;


                    var det = _dbContext.PlanMonthlyPlanningItems.Where(x => x.PlannigId == Id).ToList();
                    List<PlanMonthlyPlanningItems> Details = new List<PlanMonthlyPlanningItems>();

                    foreach (var grp in det)
                    {
                        PlanMonthlyPlanningItems modeld = new PlanMonthlyPlanningItems();
                        modeld.Id = grp.Id;
                        modeld.PlannigId = grp.PlannigId;
                        modeld.ForthLevelCategoryId = grp.ForthLevelCategoryId;
                        modeld.CategoryName = grp.CategoryName;
                        modeld.PlanSpecificationId = grp.PlanSpecificationId;
                        modeld.MonthId = grp.MonthId;
                        modeld.SeasonalDesignCount = grp.SeasonalDesignCount;
                        modeld.SeasonalRunQty = grp.SeasonalRunQty;
                        modeld.SeasonalFabricCons = grp.SeasonalFabricCons;
                        modeld.MonthlyDesignCount = grp.MonthlyDesignCount;
                        modeld.MonthlyRunQty = grp.MonthlyRunQty;
                        modeld.MonthlyFabricCons = grp.MonthlyFabricCons;
                        Details.Add(modeld);
                    }

                    model.PlanMonthlyPlanningDetail = Details.OrderBy(x => x.Id).ToList();
                    return View(model);
                

            }
            catch (Exception)
            {

                return View(new VMMonthllyPlanning());
            }

        }


        [HttpPost]
        public async Task<IActionResult> Create(VMMonthllyPlanning mod, IFormCollection collection)
        {
            try
            {
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                string userId = HttpContext.Session.GetString("UserId");
                int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
                PlanMonthlyPlanning model = new PlanMonthlyPlanning();
                List<PlanMonthlyPlanningItems> MnthlyPlanDetailss = new List<PlanMonthlyPlanningItems>();


                if (mod.PlanMonthly.Id == 0)
                {
                    model.CompanyId = companyId;
                    model.Resp_Id = resp_Id;
                    model.CreatedBy = userId;
                    model.CreatedDate = Convert.ToDateTime(DateTime.Now);
                    model.IsDeleted = false;
                    model.TransactionDate = Convert.ToDateTime(mod.PlanMonthly.TransactionDate);
                    model.TransactionNo = GetMaxTransNo(companyId);
                    model.ApprovedDate = Convert.ToDateTime(DateTime.Now);
                    model.UpdatedDate = Convert.ToDateTime(DateTime.Now);
                    model.UnApprovedDate = Convert.ToDateTime(DateTime.Now);
                    model.SPId = mod.PlanMonthly.SPId;
                    model.Planofmonth = mod.PlanMonthly.Planofmonth;
                    model.Remarks = mod.PlanMonthly.Remarks;
                    model.IsApproved = false;
                    model.IsDeleted = false;


                    _dbContext.PlanMonthlyPlanning.Add(model);
                    await _dbContext.SaveChangesAsync();
                    List<PlanMonthlyPlanningItems> pLanDetails = new List<PlanMonthlyPlanningItems>();

                    for (int i = 0; i < collection["FourhcategoryId"].Count; i++)
                    {
                        PlanMonthlyPlanningItems modeld = new PlanMonthlyPlanningItems();
                        modeld.PlannigId = model.Id;
                        modeld.SeasonalDetailId = Convert.ToInt32(collection["SeasonalDetailId"][i]);
                        modeld.ForthLevelCategoryId = Convert.ToInt32(collection["FourhcategoryId"][i]);
                        modeld.CategoryName = collection["CategoryNamename"][i];
                        modeld.spec_Id = Convert.ToInt32(collection["spec_Id"][i]);
                        modeld.PlanSpecificationId = collection["specification"][i];
                        modeld.MonthId = collection["MonthId"][i];
                        modeld.SeasonalDesignCount = Convert.ToInt32(collection["seasonalDesignCount"][i]);
                        modeld.SeasonalRunQty = Convert.ToInt32(collection["seasonalRun"][i]);
                        modeld.SeasonalFabricCons = Convert.ToInt32(collection["seasonalFabricCons"][i]);
                        modeld.MonthlyDesignCount = Convert.ToInt32(collection["monthlyDesignCount"][i]);
                        modeld.MonthlyRunQty = Convert.ToInt32(collection["monthlyRun"][i]);
                        modeld.MonthlyFabricCons = Convert.ToInt32(collection["monthlyFabricCons"][i]);
                        modeld.MonthlyFabricConsBalance = Convert.ToInt32(collection["monthlyFabricCons"][i]);
                        
                        // Handle Balance Quantity
                        SeasonalPlaningDetail seasonalPlaningDetail = _dbContext.SeasonalPlaningDetail.FirstOrDefault(x => x.Id == modeld.SeasonalDetailId);
                        seasonalPlaningDetail.BalanceDesignCount = seasonalPlaningDetail.BalanceDesignCount - modeld.MonthlyDesignCount;
                        //seasonalPlaningDetail.BalanceDesignRun = seasonalPlaningDetail.BalanceDesignRun - modeld.MonthlyRunQty;
                        seasonalPlaningDetail.BalanceFabricConsumption = seasonalPlaningDetail.BalanceFabricConsumption - modeld.MonthlyFabricCons;
                        //-----

                        pLanDetails.Add(modeld);
                        await _dbContext.SaveChangesAsync();
                    }
                    _dbContext.PlanMonthlyPlanningItems.AddRange(pLanDetails);
                    await _dbContext.SaveChangesAsync();
                    TempData["error"] = "false";
                    TempData["message"] = "Monthly Planning # " + model.TransactionNo + " has been saved successfully.";
                    return RedirectToAction(nameof(Index));

                }
                else
                {
                    //PlanMonthlyPlanning modelu = new PlanMonthlyPlanning();
                    PlanMonthlyPlanning modelu = _dbContext.PlanMonthlyPlanning.Find(mod.PlanMonthly.Id);
                    model.CompanyId = companyId;
                    modelu.Resp_Id = resp_Id;
                    modelu.CreatedBy = userId;
                    modelu.CreatedDate = Convert.ToDateTime(DateTime.Now);
                    modelu.IsDeleted = false;
                    modelu.TransactionDate = Convert.ToDateTime(mod.PlanMonthly.TransactionDate);
                    modelu.TransactionNo = mod.PlanMonthly.TransactionNo;
                    modelu.ApprovedDate = Convert.ToDateTime(DateTime.Now);
                    modelu.UpdatedDate = Convert.ToDateTime(DateTime.Now);
                    modelu.UnApprovedDate = Convert.ToDateTime(DateTime.Now);
                    modelu.SPId = mod.PlanMonthly.SPId;
                    modelu.UpdatedBy = userId;
                    modelu.Planofmonth = mod.PlanMonthly.Planofmonth;
                    modelu.Remarks = mod.PlanMonthly.Remarks;
                    modelu.IsApproved = false;
                    modelu.IsDeleted = false;

                    _dbContext.PlanMonthlyPlanning.Update(modelu);
                    await _dbContext.SaveChangesAsync();
                    var existingDetail = _dbContext.PlanMonthlyPlanningItems.Where(x => x.PlannigId == mod.PlanMonthly.Id).ToList();
                    //Deleting detail
                    var j = 0;
                    foreach (var detail in existingDetail)
                    {
                        var detailId = Convert.ToInt32(collection["Id"][j]);
                        bool isExist = detailId == detail.Id ? true : false;
                        if (!isExist)
                        {
                            _dbContext.PlanMonthlyPlanningItems.Remove(detail);
                            await _dbContext.SaveChangesAsync();
                        }
                        j++;
                    }
                    //Inserting/Updating monthly limit
                    List<PlanMonthlyPlanningItems> MPDetails = new List<PlanMonthlyPlanningItems>();
                    for (int i = 0; i < collection["FourhcategoryId"].Count; i++)
                    {
                        var detailId = Convert.ToInt32(collection["Id"][i]);
                        PlanMonthlyPlanningItems modeld = new PlanMonthlyPlanningItems();

                        if (detailId == 0)
                        {
                            modeld.PlannigId = modelu.Id;
                            modeld.SeasonalDetailId = Convert.ToInt32(collection["SeasonalDetailId"][i]);
                            modeld.ForthLevelCategoryId = Convert.ToInt32(collection["FourhcategoryId"][i]);
                            modeld.CategoryName = collection["CategoryNamename"][i];
                            modeld.spec_Id = Convert.ToInt32(collection["spec_Id"][i]);
                            modeld.PlanSpecificationId = collection["specification"][i];
                            modeld.MonthId = collection["MonthId"][i];
                            modeld.SeasonalDesignCount = Convert.ToInt32(collection["seasonalDesignCount"][i]);
                            modeld.SeasonalRunQty = Convert.ToInt32(collection["seasonalRun"][i]);
                            modeld.SeasonalFabricCons = Convert.ToInt32(collection["seasonalFabricCons"][i]);
                            modeld.MonthlyDesignCount = Convert.ToInt32(collection["monthlyDesignCount"][i]);
                            modeld.MonthlyRunQty = Convert.ToInt32(collection["monthlyRun"][i]);
                            modeld.MonthlyFabricCons = Convert.ToInt32(collection["monthlyFabricCons"][i]);
                            modeld.MonthlyFabricConsBalance = Convert.ToInt32(collection["monthlyFabricCons"][i]);

                            // Handle Balance Quantity
                            SeasonalPlaningDetail seasonalPlaningDetail = _dbContext.SeasonalPlaningDetail.FirstOrDefault(x => x.Id == modeld.SeasonalDetailId);
                            seasonalPlaningDetail.BalanceDesignCount = seasonalPlaningDetail.BalanceDesignCount - modeld.MonthlyDesignCount;
                            //seasonalPlaningDetail.BalanceDesignRun = seasonalPlaningDetail.BalanceDesignRun - modeld.MonthlyRunQty;
                            seasonalPlaningDetail.BalanceFabricConsumption = seasonalPlaningDetail.BalanceFabricConsumption - modeld.MonthlyFabricCons;
                            //-----

                            await _dbContext.PlanMonthlyPlanningItems.AddAsync(modeld);
                        }
                        else
                        {
                            PlanMonthlyPlanningItems Items = _dbContext.PlanMonthlyPlanningItems.FirstOrDefault(x => x.Id == detailId);
                            
                            // Handle Balance Quantity
                            if (Items.MonthlyDesignCount < Convert.ToInt32(collection["monthlyDesignCount"][i]))
                            {
                                SeasonalPlaningDetail seasonalPlaningDetail = _dbContext.SeasonalPlaningDetail.FirstOrDefault(x => x.Id == Items.SeasonalDetailId);
                                seasonalPlaningDetail.BalanceDesignCount = seasonalPlaningDetail.BalanceDesignCount - (Convert.ToInt32(collection["monthlyDesignCount"][i]) - Items.MonthlyDesignCount);
                                //seasonalPlaningDetail.BalanceDesignRun = seasonalPlaningDetail.BalanceDesignRun - modeld.MonthlyRunQty;
                                seasonalPlaningDetail.BalanceFabricConsumption = seasonalPlaningDetail.BalanceFabricConsumption - (Convert.ToInt32(collection["monthlyFabricCons"][i]) - Items.MonthlyFabricCons);
                            }
                            if (Items.MonthlyDesignCount > Convert.ToInt32(collection["monthlyDesignCount"][i]))
                            {
                                SeasonalPlaningDetail seasonalPlaningDetail = _dbContext.SeasonalPlaningDetail.FirstOrDefault(x => x.Id == Items.SeasonalDetailId);
                                seasonalPlaningDetail.BalanceDesignCount = seasonalPlaningDetail.BalanceDesignCount - (Convert.ToInt32(collection["monthlyDesignCount"][i]) - Items.MonthlyDesignCount);
                                //seasonalPlaningDetail.BalanceDesignRun = seasonalPlaningDetail.BalanceDesignRun - modeld.MonthlyRunQty;
                                seasonalPlaningDetail.BalanceFabricConsumption = seasonalPlaningDetail.BalanceFabricConsumption + (Items.MonthlyFabricCons - Convert.ToInt32(collection["monthlyFabricCons"][i]));
                            }
                            ////-----
                            
                            Items.SeasonalDetailId = Convert.ToInt32(collection["SeasonalDetailId"][i]);
                            Items.ForthLevelCategoryId = Convert.ToInt32(collection["FourhcategoryId"][i]);
                            Items.CategoryName = collection["CategoryNamename"][i];
                            Items.spec_Id = Convert.ToInt32(collection["spec_Id"][i]);
                            Items.PlanSpecificationId = collection["specification"][i];
                            Items.MonthId = collection["MonthId"][i];
                            Items.SeasonalDesignCount = Convert.ToInt32(collection["seasonalDesignCount"][i]);
                            Items.SeasonalRunQty = Convert.ToInt32(collection["seasonalRun"][i]);
                            Items.SeasonalFabricCons = Convert.ToInt32(collection["seasonalFabricCons"][i]);
                            Items.MonthlyDesignCount = Convert.ToInt32(collection["monthlyDesignCount"][i]);
                            Items.MonthlyRunQty = Convert.ToInt32(collection["monthlyRun"][i]);
                            Items.MonthlyFabricCons = Convert.ToInt32(collection["monthlyFabricCons"][i]);
                            Items.MonthlyFabricConsBalance = Convert.ToInt32(collection["monthlyFabricCons"][i]);

                         


                            _dbContext.PlanMonthlyPlanningItems.Update(Items);
                        }
                        await _dbContext.SaveChangesAsync();


                    }

                    TempData["error"] = "false";
                    TempData["message"] = "Monthly Planning #  " + modelu.TransactionNo + " has been updated successfully.";
                    return RedirectToAction(nameof(Index));

                }
            }
            catch (Exception ex)
            {

                TempData["error"] = "true";
                TempData["message"] = "Monthly Planning #  " + ex + ".";
                return RedirectToAction(nameof(Create));
            }                      
        }

        //
        public async Task<IActionResult> Delete(int id)
        {
            //var ContractRepo = new GRFolding(_dbContext);
            string userId = HttpContext.Session.GetString("UserId");
            bool isSuccess;

            var deleteItem = _dbContext.PlanMonthlyPlanning.Where(x => x.Id == id).FirstOrDefault();
            if (deleteItem == null)
            {
                isSuccess = false;
            }
            else
            {
                deleteItem.IsDeleted = true;
                deleteItem.DeletedBy = userId;
                deleteItem.DeletedDate = Convert.ToDateTime(DateTime.Now);
                var entry = _dbContext.PlanMonthlyPlanning.Update(deleteItem);

                var details = _dbContext.PlanMonthlyPlanningItems.Where(x=> x.PlannigId == deleteItem.Id);
                foreach (var item in details)
                {
                    // Handle Balance Quantity
                    SeasonalPlaningDetail seasonalPlaningDetail = _dbContext.SeasonalPlaningDetail.FirstOrDefault(x => x.Id == item.SeasonalDetailId);
                    seasonalPlaningDetail.BalanceDesignCount = seasonalPlaningDetail.BalanceDesignCount + item.MonthlyDesignCount;
                    //seasonalPlaningDetail.BalanceDesignRun = seasonalPlaningDetail.BalanceDesignRun - modeld.MonthlyRunQty;
                    seasonalPlaningDetail.BalanceFabricConsumption = seasonalPlaningDetail.BalanceFabricConsumption + item.MonthlyFabricCons;
                    //-----
                    await _dbContext.SaveChangesAsync();

                }

                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                await _dbContext.SaveChangesAsync();
                isSuccess = true;
            }

            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Monthly Planning has been deleted successfully";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Monthly Planning not found";
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Approve(int id)
        {
            //var ContractRepo = new GRFolding(_dbContext);
            string userId = HttpContext.Session.GetString("UserId");
            bool isSuccess;

            var deleteItem = _dbContext.PlanMonthlyPlanning.Where(x => x.Id == id).FirstOrDefault();
            if (deleteItem == null)
            {
                isSuccess = false;
            }
            else
            {
                deleteItem.IsApproved = true;
                deleteItem.ApprovedBy = userId;
                deleteItem.ApprovedDate = Convert.ToDateTime(DateTime.Now);
                var entry = _dbContext.PlanMonthlyPlanning.Update(deleteItem);
                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                await _dbContext.SaveChangesAsync();
                isSuccess = true;
            }

            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Monthly Planning has been Approved successfully";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Monthly Planning not found";
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> UnApprove(int id)
        {
            //var ContractRepo = new GRFolding(_dbContext);
            string userId = HttpContext.Session.GetString("UserId");
            bool isSuccess;

            var deleteItem = _dbContext.PlanMonthlyPlanning.Where(x => x.Id == id).FirstOrDefault();
            if (deleteItem == null)
            {
                isSuccess = false;
            }
            else
            {
                deleteItem.IsApproved = false;
                deleteItem.UnApprovedBy = userId;
                deleteItem.UnApprovedDate = Convert.ToDateTime(DateTime.Now);
                var entry = _dbContext.PlanMonthlyPlanning.Update(deleteItem);
                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                await _dbContext.SaveChangesAsync();
                isSuccess = true;
            }

            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Monthly Planning has been UnApproved successfully";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Monthly Planning not found";
            }
            return RedirectToAction(nameof(Index));
        }

        public int GetMaxTransNo(int companyId)
        {
            int maxReceiptNo = 1;
            var receipts = _dbContext.PlanMonthlyPlanning.ToList();
            if (receipts.Count > 0)
            {
                maxReceiptNo = receipts.Max(r => r.TransactionNo);
                return maxReceiptNo + 1;
            }
            else
            {
                return maxReceiptNo;
            }
        }
        
        public IActionResult FindSeasonalPlanning(int id, int[] skipIds, string month)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;

            var check = (from master in _dbContext.PlanMonthlyPlanning.ToList() join
            detail in _dbContext.PlanMonthlyPlanningItems.ToList() on master.Id equals detail.PlannigId
            where master.SPId == id && master.Planofmonth == month && master.IsDeleted != true
            select new
            {
                master,
                detail
            }).ToList();
            if (check.Count() == 0)
            {
                var SeasonalPlanData = (from a in _dbContext.SeasonalPlaning.Where(x => x.IsDeleted == false && x.CompanyId==companyId)
                                        join b in _dbContext.SeasonalPlaningDetail.Include(x=>x.Season).Where(i => !skipIds.Contains(i.Id) && i.BalanceDesignCount > 0).ToList() on a.Id equals b.SeasonalPlaningId
                                        join c in _dbContext.InvItemCategories on b.FourthItemCategoryId equals c.Id

                                        where a.Id == id && a.IsDeleted == false
                                        select new
                                        {
                                            a.Id,

                                            DetailId = b.Id,

                                            CategoryId = c.Id,
                                            ItemCategory = c.Name,

                                            Specification = (_dbContext.GRQuality.Where(x => x.Id == b.GreigeQualityId).Select(x => x.Description)).FirstOrDefault(),
                                            Season = b.Season.ConfigValue,
                                            b.StartDate,
                                            b.EndDate,
                                            SeasonalDesignCount = b.BalanceDesignCount,
                                            SeasonalRun = b.DesignRun,
                                            SeasonalFabricCons = b.BalanceFabricConsumption,
                                            Period = String.Concat(b.StartDate.ToString("MMM-yyyy"), "-", b.EndDate.ToString("MMM-yyyy"))

                                        }).ToList();
                return Ok(SeasonalPlanData);
            }
            // int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;


            //var SeasonalPlanData1 = (from a in _dbContext.SeasonalPlaning.Where(x => x.IsDelete == false)
            //                        join b in _dbContext.SeasonalPlaningDetail on a.Id equals b.SeasonalPlaningId
            //                        join c in _dbContext.InvItemCategories on Convert.ToInt32(b.FourthItemCategory) equals c.Id

            //                        where a.Id == id
            //                        && a.IsDelete == false
            //                        select new
            //                        {
            //                            a.Id,
            //                            DetailId = b.Id,
            //                            CategoryId = c.Id,
            //                            ItemCategory = c.Name,
            //                            Specification = "All is Good",
            //                            Season = b.Season,
            //                            b.StartDate,
            //                            b.EndDate,
            //                            SeasonalDesignCount = b.DesignCount,
            //                            SeasonalRun = b.DesignRunQty,
            //                            SeasonalFabricCons = b.FabricConsumption,
            //                            Period = String.Concat(b.StartDate.ToString(Helpers.CommonHelper.DateFormat), "-", b.EndDate.ToString(Helpers.CommonHelper.DateFormat))

            //                        }).ToList();



            return Ok(null);
        }

        public IActionResult FindSeasonalMonths(int id)
        {
            string SDate = _dbContext.SeasonalPlaningDetail.Where(x => x.SeasonalPlaningId == id).Select(x => x.StartDate).FirstOrDefault().ToString();
            string EDate = _dbContext.SeasonalPlaningDetail.Where(x => x.SeasonalPlaningId == id).Select(x => x.EndDate).FirstOrDefault().ToString();
            DateTime StartDate = Convert.ToDateTime(SDate);
            DateTime EndDate = Convert.ToDateTime(EDate);
            
            string mons = String.Concat(StartDate.ToString("MM"), StartDate.ToString("yy"));
            string mone = String.Concat(EndDate.ToString("MM"), EndDate.ToString("yy"));
            int SSData = Convert.ToInt32(mons);
            int EEData = Convert.ToInt32(mone);
            List<string> NewList = new List<string>();
            int a = 0;
            int b = 0;
            if (EEData < SSData)
            {
                for (int i = EEData; i <= SSData; i++)
                {
                    string Monthdate = "";

                    DateTime FirstDay = new DateTime(StartDate.Year + b, StartDate.Month + a, 1);

                    // FirstDay = StartDate;

                    Monthdate = FirstDay.ToString("MMM-yyyy");
                    //DateTime LastDay = new DateTime(StartDate.Year, StartDate.Month + i, DateTime.DaysInMonth(StartDate.Year, FirstDay.Month));
                    //if (i == EndDate.Month - StartDate.Month)
                    //{
                    //    LastDay = EndDate;
                    //}
                    if(NewList.Count() < 6) {
                        NewList.Add(Monthdate);
                        i = i + 100 - 1;
                        a = a == 2 ? -9 : a + 1;
                        if (a == -9)
                        {
                            b = 1;
                        }
                    }
                    else
                    {
                        i = i + 100 - 1;
                    }
                    Console.WriteLine();
                }

            }
            else
            {
                for (int i = SSData; i <= EEData; i++)
                {
                    string Monthdate = "";

                    DateTime FirstDay = new DateTime(StartDate.Year, StartDate.Month + a, 1);

                    // FirstDay = StartDate;

                    Monthdate = FirstDay.ToString("MMM-yyyy");
                    //DateTime LastDay = new DateTime(StartDate.Year, StartDate.Month + i, DateTime.DaysInMonth(StartDate.Year, FirstDay.Month));
                    //if (i == EndDate.Month - StartDate.Month)
                    //{
                    //    LastDay = EndDate;
                    //}
                    NewList.Add(Monthdate);
                    i = i + 100 - 1;
                    a = a + 1;
                    Console.WriteLine();
                }
            }


            var moonths = NewList.Select(c => new {
                id = c
            });


            return Ok(moonths);
        }

            public IActionResult FindSeasonalDetailData(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            // int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            var SeasonalPlanDetailData = (
                                    from b in _dbContext.SeasonalPlaningDetail.Where(x => x.Id == id)
                                    join a in _dbContext.SeasonalPlaning.Where(x=>x.CompanyId==companyId) on b.SeasonalPlaningId equals a.Id
                                    join c in _dbContext.InvItemCategories on b.FourthItemCategoryId equals c.Id
                                    select new
                                    {
                                        
                                        DetailId = b.Id,
                                        CategoryId = c.Id,
                                        ItemCategory = c.Name,
                                        spec_Id= (_dbContext.GRQuality.Where(x => x.Id == b.GreigeQualityId).Select(x => x.Id)).FirstOrDefault(),
                                        Specification = (_dbContext.GRQuality.Where(x => x.Id == b.GreigeQualityId).Select(x => x.Description)).FirstOrDefault(),
                                        Season = b.SeasonId,
                                        b.StartDate,
                                        b.EndDate,
                                        SeasonalDesignCount = b.BalanceDesignCount,
                                        SeasonalRun = b.DesignRun,
                                        SeasonalFabricCons = b.BalanceFabricConsumption,
                                        Period = String.Concat(b.StartDate.ToString("MMM-yyyy"), "-")

                                    }).ToList();

            return Ok(SeasonalPlanDetailData);

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
                var InvData = (from m in _dbContext.PlanMonthlyPlanning.Where(x=>x.CompanyId == companyId).Include(x=>x.SP)
                                   // join d in _dbContext.GRFoldingItems on m.FoldingNo equals d.FoldingId
                               where m.IsDeleted != true
                              select m);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    InvData = InvData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                InvData = !string.IsNullOrEmpty(searchItemCode) ? InvData.Where(m => m.TransactionNo.ToString().ToUpper().Contains(searchItemCode.ToUpper())) : InvData;
                InvData = !string.IsNullOrEmpty(searchItemName) ? InvData.Where(m => m.TransactionDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchItemName.ToUpper())) : InvData;
                InvData = !string.IsNullOrEmpty(searchBarcode) ? InvData.Where(m => m.SP.TransactionNo.ToString().ToUpper().Contains(searchBarcode.ToUpper())) : InvData;
                InvData = !string.IsNullOrEmpty(searchUOM) ? InvData.Where(m => m.Planofmonth.ToString().ToUpper().Contains(searchUOM.ToUpper())) : InvData;

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

                List<VMMonthllyPlanning> viewModel = new List<VMMonthllyPlanning>();
                foreach (var item in data)
                {
                    VMMonthllyPlanning Modelv = new VMMonthllyPlanning();
                    Modelv.PlanMonthly = item;
                    Modelv.PlanMonthly.Approve = approve;
                    Modelv.PlanMonthly.Unapprove = unApprove;
                    Modelv.SeasonalId = (_dbContext.SeasonalPlaning.Where(x => x.Id == item.SPId && x.CompanyId==companyId).Select(x => x.TransactionNo).FirstOrDefault());
                    Modelv.ApprovedBy = item.TransactionDate.ToString(Helpers.CommonHelper.DateFormat);
                    viewModel.Add(Modelv);
                }

                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = viewModel };
                return Ok(jsonData);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
