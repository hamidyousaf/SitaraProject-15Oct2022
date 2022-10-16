﻿using Microsoft.AspNetCore.Authorization;
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
using Numbers.Repository.Helpers;

namespace Numbers.Areas.Planning.Controllers
{
    [Area("Planning")]
    [Authorize]
    public class GreigeIssuanceController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public GreigeIssuanceController(NumbersDbContext context)
        {
            _dbContext = context;
        }
        public IActionResult Create(int? id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            GreigeIssuanceViewModel GreigeIssuanceViewModel = new GreigeIssuanceViewModel();

            var BalRequireQty = (from  c in _dbContext.ProductionOrders.Where(x=>x.CompanyId==companyId) 
                                 join d in _dbContext.ProductionOrderItems on c.Id equals d.ProductionOrderId
                                 where (_dbContext.ProductionOrderItems.Where(x=>x.ProductionOrderId == c.Id ).Select(x=>x.VersionQuantity).Sum() - (_dbContext.GreigeIssuanceDetail.Where(x => x.ProductionOrderDetailId == c.Id )).Select(x => x.IssuanceQty).Sum()) <=0
                                 select new
                                 {
                                     c.Id
                                   
                                    // Qty = d.VersionQuantity - (_dbContext.GreigeIssuanceDetail.Where(x => x.ProductionOrderDetailId == c.Id && x.GreigeQualityId ==  d.GreigeQualityId)).Select(x => x.IssuanceQty).Sum()
                                 }).Distinct()
                                 .ToList();



            int[] terms = new int[BalRequireQty.Count()];

           
            for (int runs = 0; runs < BalRequireQty.Count(); runs++)
            {
               // if (BalRequireQty[runs].Qty <= 0)
               // {
                    terms[runs] = BalRequireQty[runs].Id;
               // }

            }
            GreigeIssuanceViewModel.SpecificationLOV = new SelectList(_dbContext.ProductionOrders.Where(x => x.IsDeleted != true && x.CompanyId==companyId && x.Status=="Approved" && !terms.Contains(x.Id)).ToList().OrderByDescending(x =>x.Id), "Id", "TransactionNo");
           // ViewBag.spe = new SelectList(_dbContext.PlanSpecifications.Where(x => x.IsDeleted != true).ToList(), "Id", "Description");
            var configValues = new ConfigValues(_dbContext);
            GreigeIssuanceViewModel.VendorLOV = configValues.GreigeVendorByCode(companyId);
            GreigeIssuanceViewModel.IssueTypeLOV =  configValues.GetConfigValues("Greige", "Issue Type", companyId);
            GreigeIssuanceViewModel.WareHouseLOV =  configValues.GetConfigValues("Inventory", "Ware House", "GD Greige", companyId);

            if (id == null)
            {
                ViewBag.State = "Create";
                return View(GreigeIssuanceViewModel);
            }
            else
            {

              



                GreigeIssuanceViewModel.SpecificationLOV = new SelectList(_dbContext.ProductionOrders.Where(x => x.IsDeleted != true && x.CompanyId == companyId && x.Status == "Approved" ).ToList().OrderByDescending(x => x.Id), "Id", "TransactionNo");

                GreigeIssuanceViewModel.GreigeIssuance = _dbContext.GreigeIssuance.Where(x=>x.Id==id &&  x.CompanyId == companyId).FirstOrDefault();
                var AlreadIssuedQty = (from a in _dbContext.GreigeIssuance.Where(x => x.SpecificationId == GreigeIssuanceViewModel.GreigeIssuance.SpecificationId && x.CompanyId == companyId)
                                       join b in _dbContext.GreigeIssuanceDetail on a.Id equals b.GreigeIssuanceId
                                       where a.IsDeleted != true
                                       select new
                                       {
                                           b.IssuanceQty,
                                           ProductionId = a.SpecificationId,
                                           QualityId = b.GreigeQualityId
                                       }).ToList();

                // GreigeIssuanceViewModel.GreigeIssuanceDetails
                var GRIssuanceDetail= _dbContext.GreigeIssuanceDetail
                    .Include(x => x.GreigeQuality)
                    .Where(x => x.GreigeIssuanceId == id).ToList();

                List<GreigeIssuanceDetail> detail = new List<GreigeIssuanceDetail>();
                foreach (var item in GRIssuanceDetail)
                {
                    var d = new GreigeIssuanceDetail();
                    d.Id = item.Id;
                    d.GreigeIssuanceId = item.GreigeIssuanceId;
                    d.ProductionOrderDetailId = item.ProductionOrderDetailId;
                    d.GreigeQualityId = item.GreigeQualityId;
                    d.GreigeQuality = item.GreigeQuality;
                    d.RequiredQty = item.RequiredQty;
                    d.AvailableQty = item.AvailableQty;
                    d.IssuanceQty = item.IssuanceQty;
                    d.AlreadyIssuedQty = (AlreadIssuedQty.Where(x => x.QualityId == item.GreigeQualityId && x.ProductionId == GreigeIssuanceViewModel.GreigeIssuance.SpecificationId).Select(x => x.IssuanceQty).Sum());

                    detail.Add(d);

                }
                GreigeIssuanceViewModel.GreigeIssuanceDetails = detail.ToArray();
                ViewBag.State = "Update";
                return View(GreigeIssuanceViewModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(GreigeIssuanceViewModel model, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var repo = new GreigeIssuanceRepo(_dbContext);
            GreigeIssuanceDetail[] detail = JsonConvert.DeserializeObject<GreigeIssuanceDetail[]>(collection["itemDetail"]);
            if (model.GreigeIssuance.Id == 0)
            {
                model.GreigeIssuance.TransactionNo = repo.Max(companyId);
                model.GreigeIssuance.CreatedBy = userId;
                model.GreigeIssuance.CompanyId = companyId;
                model.GreigeIssuance.Resp_Id = resp_Id;
                model.GreigeIssuance.VendorId = Convert.ToInt32(collection["VendorId"]); 
                model.GreigeIssuanceDetails = detail;
                bool isSuccess = await repo.Create(model);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = string.Format("Greige Issuance. {0} has been created successfully.", model.GreigeIssuance.TransactionNo);
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Something went wrong.";
                }
                return RedirectToAction("Create", "GreigeIssuance");
            }
            else
            {
                model.GreigeIssuance.UpdatedBy = userId;
                model.GreigeIssuance.CompanyId = companyId;
                model.GreigeIssuance.Resp_Id = resp_Id;
                model.GreigeIssuanceDetails = detail;
                bool isSuccess = await repo.Update(model);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = string.Format("Greige Issuance. {0} has been updated successfully.", model.GreigeIssuance.TransactionNo);
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
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string configValues = _dbContext.AppCompanyConfigs
                             .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                             .Select(c => c.ConfigValue)
                             .FirstOrDefault();
            ViewBag.ReportPath = string.Concat(configValues, "Viewer", "?Report=Voucher&cId=", companyId, "&id=");
            ViewBag.Navbarheading = "List of Greige Issuance";
            return View();
        }

        public async Task<IActionResult> Delete(int id)
        {
            var Repo = new GreigeIssuanceRepo(_dbContext);
                bool isSuccess = await Repo.Delete(id);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Greige Issuance has been deleted successfully";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Greige Issuance not found";
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public IActionResult GetSpecificationForPopUp(int[] skipIds, int wareHouseId, int seasonId)
        {

            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;

            var existGreigeData = (from master in _dbContext.GreigeIssuance.Where(x => x.SpecificationId == seasonId).ToList() join
            detail in _dbContext.GreigeIssuanceDetail.ToList() on 
            master.Id equals detail.GreigeIssuanceId
            where !master.IsDeleted && master.CompanyId == companyId
            select new
            {
                master,
                detail
            }).GroupBy(x =>x.detail.GreigeQualityId).Select(x => new { 
                GreigeQualityId = x.Select(a => a.detail.GreigeQualityId).FirstOrDefault(),
                IssuanceQty = x.Select(a => a.detail.IssuanceQty).Sum()
            });





            var BalRequireQty = (from a in _dbContext.GreigeIssuance.Where(x => x.SpecificationId == seasonId)
                                 join b in _dbContext.GreigeIssuanceDetail on a.Id equals b.GreigeIssuanceId
                                 where a.IsDeleted != true && a.CompanyId == companyId
                                 select new
                                 {
                                     b.IssuanceQty,
                                     ProductionId = a.SpecificationId,
                                     QualityId = b.GreigeQualityId
                                 }).ToList();


            ProductionOrder order = _dbContext.ProductionOrders
                .Include(x=>x.ProductionOrderItems)
                    .ThenInclude(x=>x.GreigeQuality)
                .FirstOrDefault(x => x.Id == seasonId && x.CompanyId == companyId);

            var data = order.ProductionOrderItems.Where(x => !skipIds.Contains(x.GreigeQualityId)).GroupBy(x => x.GroupId).Select(x => new
            {
                ProductionOrderItems = x.Select(x => x).ToList(),
            });
            var d = data
                .GroupBy(x => x.ProductionOrderItems.Select(x => x.GreigeQualityId)).Select(x => new
                 {
                     ItemId = x.Select(a => a.ProductionOrderItems.Select(b => b.GreigeQuality.ItemId).FirstOrDefault()).FirstOrDefault(),
                     ProductionOrderId = x.Select(a => a.ProductionOrderItems.Select(b => b.ProductionOrderId).FirstOrDefault()).FirstOrDefault(),
                     GreigeQualityId = x.Select(a => a.ProductionOrderItems.Select(b => b.GreigeQualityId).FirstOrDefault()).FirstOrDefault(),
                     GreigeQualityDesc = x.Select(a => a.ProductionOrderItems.Select(b => b.GreigeQuality.Description).FirstOrDefault()).FirstOrDefault(),
                     RequiredQty = x.Select(a => a.ProductionOrderItems.Select(b => b.SuitMeters).FirstOrDefault()).Sum(),
                 });
            var x = d.GroupBy(x=>x.GreigeQualityId).Select(x=> new {
                ItemId = x.Select(a=>a.ItemId).FirstOrDefault(),
                Id = x.Select(a=>a.ProductionOrderId).FirstOrDefault(),
                QualityId = x.Select(a=>a.GreigeQualityId).FirstOrDefault(),
                description = x.Select(a=>a.GreigeQualityDesc).FirstOrDefault(),
                RequiredQty = x.Select(a=>a.RequiredQty).Sum(),
                availableStock = (_dbContext.VwInvLedgers.Where(a => a.ItemId == x.Select(a => a.ItemId).FirstOrDefault() && a.WareHouseId == wareHouseId && a.CompanyId == companyId).Select(x => x.Qty)).Sum() - (BalRequireQty.Where(a => a.QualityId == x.Select(a => a.GreigeQualityId).FirstOrDefault() && a.ProductionId == x.Select(a => a.ProductionOrderId).FirstOrDefault()).Select(x => x.IssuanceQty).Sum()),

            });







            List<GreigeIssuanceBalance> GreigeIssuance = new List<GreigeIssuanceBalance>();
            foreach (var item1 in x)
            {
                foreach (var item2 in existGreigeData)
                {
                    if (item1.QualityId == item2.GreigeQualityId)
                    {
                        GreigeIssuanceBalance model = new GreigeIssuanceBalance();
                        if (item1.RequiredQty > 0)
                        {
                            model.Id = item1.Id;
                            model.QualityId = item1.QualityId;
                            model.description = item1.description;
                            model.availableStock = item1.availableStock;
                            model.RequiredQty = item1.RequiredQty - item2.IssuanceQty;
                            GreigeIssuance.Add(model);
                        }
                    }
                }

            }







            if (BalRequireQty.Count() != 0)
            {
                var specification = (from a in _dbContext.ProductionOrderItems.Where(x => !skipIds.Contains(x.GreigeQualityId)).ToList()
                                     join b in _dbContext.ProductionOrders.Where(x=>x.CompanyId==companyId) on a.ProductionOrderId equals b.Id
                                     join q in _dbContext.GRQuality.ToList() on a.GreigeQualityId equals q.Id
                                     where a.ProductionOrderId == seasonId

                                     select new
                                     {
                                         Id = a.ProductionOrderId,
                                         QualityId = q.Id,
                                         description = q.Description,
                                         availableStock = (_dbContext.VwInvLedgers.Where(x => x.ItemId == q.ItemId && x.WareHouseId == wareHouseId && x.CompanyId == companyId).Select(x => x.Qty)).Sum() - (BalRequireQty.Where(x => x.QualityId == q.Id && x.ProductionId == a.ProductionOrderId).Select(x => x.IssuanceQty).Sum()),
                                         RequiredQty = (_dbContext.ProductionOrderItems.Where(x=>x.ProductionOrderId == a.ProductionOrderId && x.GreigeQualityId== a.GreigeQualityId ).Select(x=>x.SuitMeters).Max()) - (BalRequireQty.Where(x => x.QualityId == q.Id && x.ProductionId == a.ProductionOrderId).Select(x =>x.IssuanceQty).Sum())
                                     }).Distinct().GroupBy(x => x.QualityId).Select(x => new {
                                         Id = x.Max(x => x.Id),
                                         QualityId = x.Max(x => x.QualityId),
                                         description = x.Max(x => x.description),
                                         availableStock = x.Sum(x => x.availableStock),
                                         RequiredQty = x.Sum(x => x.RequiredQty)
                                     }).OrderByDescending(x => x.Id).ToList();
                List<GreigeIssuanceBalance> GreigeIssuanceBalance = new List<GreigeIssuanceBalance>();
                foreach (var item in GreigeIssuance)
                {
                    GreigeIssuanceBalance model = new GreigeIssuanceBalance();
                    if (item.RequiredQty >0 )
                    {
                        model.Id = item.Id;
                        model.QualityId = item.QualityId;
                        model.description = item.description;
                        model.availableStock = item.availableStock;
                        model.RequiredQty = item.RequiredQty;
                        GreigeIssuanceBalance.Add(model);
                    }
                }
                return Ok(GreigeIssuanceBalance);
            }
            else
            {
                var specification = (from a in _dbContext.ProductionOrderItems.Where(x => !skipIds.Contains(x.GreigeQualityId)).ToList()
                                     join b in _dbContext.ProductionOrders.Where(x => x.CompanyId == companyId) on a.ProductionOrderId equals b.Id
                                     join q in _dbContext.GRQuality.ToList() on a.GreigeQualityId equals q.Id
                                     where a.ProductionOrderId == seasonId
                                     select new
                                     {
                                         Id = a.ProductionOrderId,
                                         QualityId = q.Id,
                                         description = q.Description,
                                         availableStock = (_dbContext.VwInvLedgers.Where(x => x.ItemId == q.ItemId && x.WareHouseId == wareHouseId && x.CompanyId==companyId).Select(x => x.Qty)).Sum(),
                                         RequiredQty = a.VersionQuantity
                                     }).Distinct().GroupBy(x => x.QualityId).Select(x => new {
                                         Id = x.Max(x => x.Id),
                                         QualityId = x.Max(x => x.QualityId),
                                         description = x.Max(x => x.description),
                                         availableStock = x.Sum(x => x.availableStock),
                                         RequiredQty = x.Sum(x => x.RequiredQty)
                                     }).OrderByDescending(x => x.Id).ToList();
                return Ok(x);
                
            }

          
        }


        [HttpGet]
        public IActionResult GetProcess(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;

            var subdivision = _dbContext.ProductionOrders.Include(x => x.Vendor).Where(x => x.Id == id && x.IsDeleted == false && x.CompanyId == companyId).FirstOrDefault();
            var vendor = subdivision != null ? subdivision : null;

            return Ok(vendor);
        }


        [HttpPost]
        public IActionResult GetSpecificationById(int[] specificationId, int[] qualityId, int wareHouseId)
        {
            if (specificationId.Count() != 0)
            {
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                var BalRequireQty = (from a in _dbContext.GreigeIssuance.Where(x => specificationId.Contains(x.SpecificationId) && x.CompanyId==companyId)
                                     join b in _dbContext.GreigeIssuanceDetail on a.Id equals b.GreigeIssuanceId
                                     where a.IsDeleted != true
                                     select new
                                     {
                                         b.IssuanceQty,
                                         ProductionId = a.SpecificationId,
                                         QualityId = b.GreigeQualityId
                                     }).ToList();

                if (BalRequireQty.Count() != 0)
                {
                    var specification = (from a in _dbContext.ProductionOrderItems.Where(x => specificationId.Contains(x.ProductionOrderId)).ToList()
                                         join b in _dbContext.ProductionOrders.Where(x => x.CompanyId == companyId) on a.ProductionOrderId equals b.Id
                                         join q in _dbContext.GRQuality.Where(x => qualityId.Contains(x.Id)).ToList() on a.GreigeQualityId equals q.Id
                                         // where a.ProductionOrderId
                                         select new
                                         {
                                             Id = a.Id,
                                             QualityId = q.Id,
                                             description = q.Description,
                                             availableStock = (_dbContext.VwInvLedgers.Where(x => x.ItemId == q.ItemId && x.WareHouseId == wareHouseId && x.CompanyId==companyId).Select(x => x.Qty)).Sum()- (BalRequireQty.Where(x => x.QualityId == q.Id && x.ProductionId == a.ProductionOrderId).Select(x => x.IssuanceQty).Sum()),
                                             RequiredQty = ((_dbContext.ProductionOrderItems.Where(x => x.ProductionOrderId == a.ProductionOrderId && x.GreigeQualityId == a.GreigeQualityId).Select(x => x.SuitMeters).Max())) - (BalRequireQty.Where(x => x.QualityId == q.Id && x.ProductionId == a.ProductionOrderId).Select(x => x.IssuanceQty).Sum()),
                                             AlreadyIssuedQty = (BalRequireQty.Where(x => x.QualityId == q.Id && x.ProductionId == a.ProductionOrderId).Select(x => x.IssuanceQty).Sum())
                                         }).Distinct().GroupBy(x => x.QualityId).Select(x => new
                                         {
                                             Idd = x.Max(x => x.Id),
                                             QualityIdd = x.Max(x => x.QualityId),
                                             descriptiond = x.Max(x => x.description),
                                             availableStockd = x.Max(x => x.availableStock),
                                             RequiredQtyd = x.Max(x => x.RequiredQty),
                                             AlreadyIssuedQtyd = x.Max(x => x.AlreadyIssuedQty)
                                         }).OrderByDescending(x => x.Idd).ToList();
                    List<GreigeIssuanceBal> GreigeIssuanceBalance = new List<GreigeIssuanceBal>();
                    foreach (var item in specification)
                    {
                        GreigeIssuanceBal model = new GreigeIssuanceBal();
                        if (item.RequiredQtyd > 0)
                        {
                            model.Idd = item.Idd;
                            model.QualityIdd = item.QualityIdd;
                            model.descriptiond = item.descriptiond;
                            model.availableStockd = item.availableStockd;
                            model.RequiredQtyd = item.RequiredQtyd;
                            GreigeIssuanceBalance.Add(model);
                        }
                    }

                    return Ok(GreigeIssuanceBalance);
                }
                else
                {

                    //var specification = (from a in _dbContext.ProductionOrderItems.Where(x => specificationId.Contains(x.ProductionOrderId)).ToList()
                    //                     join b in _dbContext.ProductionOrders.Where(x => x.CompanyId == companyId) on a.ProductionOrderId equals b.Id
                    //                     join q in _dbContext.GRQuality.Where(x => qualityId.Contains(x.Id)).ToList() on a.GreigeQualityId equals q.Id
                    //                     // where a.ProductionOrderId
                    //                     select new
                    //                     {
                    //                         Id = a.Id,
                    //                         QualityId = q.Id,
                    //                         description = q.Description,
                    //                         availableStock = (_dbContext.VwInvLedgers.Where(x => x.ItemId == q.ItemId && x.WareHouseId == wareHouseId && x.CompanyId==companyId).Select(x => x.Qty)).Sum(),
                    //                         RequiredQty = a.VersionQuantity,
                    //                         AlreadyIssuedQty = (BalRequireQty.Where(x => x.QualityId == q.Id && x.ProductionId == a.ProductionOrderId).Select(x => x.IssuanceQty).Sum())
                    //                     }).Distinct().GroupBy(x => x.QualityId).Select(x => new
                    //                     {
                    //                         Idd = x.Max(x => x.Id),
                    //                         QualityIdd = x.Max(x => x.QualityId),
                    //                         descriptiond = x.Max(x => x.description),
                    //                         availableStockd = x.Max(x => x.availableStock),
                    //                         RequiredQtyd = x.Max(x => x.RequiredQty)
                    //                     }).OrderByDescending(x => x.Idd).ToList();
                    var specification = (from a in _dbContext.ProductionOrderItems.Where(x => specificationId.Contains(x.ProductionOrderId)).ToList()
                                         join b in _dbContext.ProductionOrders.Where(x => x.CompanyId == companyId) on a.ProductionOrderId equals b.Id
                                         join q in _dbContext.GRQuality.Where(x => qualityId.Contains(x.Id)).ToList() on a.GreigeQualityId equals q.Id
                                        // where a.ProductionOrderId == seasonId
                                         select new
                                         {
                                             Id = a.ProductionOrderId,
                                             QualityId = q.Id,
                                             description = q.Description,
                                             availableStock = (_dbContext.VwInvLedgers.Where(x => x.ItemId == q.ItemId && x.WareHouseId == wareHouseId && x.CompanyId == companyId).Select(x => x.Qty)).Sum(),
                                             RequiredQty = a.VersionQuantity,
                                             AlreadyIssuedQty = 0
                                         }).Distinct().GroupBy(x => x.QualityId).Select(x => new {
                                             Idd = x.Max(x => x.Id),
                                             QualityIdd = x.Max(x => x.QualityId),
                                             descriptiond = x.Max(x => x.description),
                                             availableStockd = x.Sum(x => x.availableStock),
                                             RequiredQtyd = x.Sum(x => x.RequiredQty),
                                             AlreadyIssuedQtyd = x.Sum(x => x.AlreadyIssuedQty)
                                         }).OrderByDescending(x => x.Idd).ToList();
                    return Ok(specification);
                }
                
            }
            return Ok(null);
        }
        public SelectList Seasons(int companyId)
        {
            var Seasons = new SelectList((from b in _dbContext.AppCompanyConfigBases.Where(x => x.Name == "Seasons")
                                             join c in _dbContext.AppCompanyConfigs on b.Id equals c.BaseId
                                             where b.CompanyId == companyId && b.IsActive && b.IsDeleted == false
                                             select c).ToList(), "Id", "ConfigValue");
            return Seasons;
        }
        public IActionResult GetList()
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
                var searchTransDate = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var Specification = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var Vendor = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var issuetype = Request.Form["columns[4][search][value]"].FirstOrDefault();
                var process = Request.Form["columns[5][search][value]"].FirstOrDefault();
                var warehouse = Request.Form["columns[6][search][value]"].FirstOrDefault();
           
               

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var Data = (from m in _dbContext.GreigeIssuance.Where(x => x.IsDeleted != true && x.CompanyId == companyId)
                               .Include(x => x.Specification)
                                .Include(x => x.WareHouse)
                                .Include(x => x.Vendor)
                               // .Include(x => x.ProductionOrder)
                              
                                //.Include(x => x.IssueType)
                                 select m);

                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    Data = Data.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                Data = !string.IsNullOrEmpty(searchTransNo) ? Data.Where(m => m.TransactionNo.ToString().ToUpper().Contains(searchTransNo.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchTransDate) ? Data.Where(m => m.TransactionDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchTransDate.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(Specification) ? Data.Where(m => m.Specification.ToString().ToUpper().Contains(Specification.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(Vendor) ? Data.Where(m => m.Vendor.ToString().ToUpper().Contains(Vendor.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(issuetype) ? Data.Where(m => m.IssueTypeId.ToString().ToUpper().Contains(issuetype.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(process) ? Data.Where(m => m.process.ToString().ToUpper().Contains(process.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(warehouse) ? Data.Where(m => m.WareHouse.ToString().ToUpper().Contains(warehouse.ToUpper())) : Data;


                //recordsTotal = data.Count();
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
                //var result = data.Skip(skip).Take(pageSize).ToList();

                List<GreigeIssuanceViewModel> viewModel = new List<GreigeIssuanceViewModel>();
                foreach (var item in data)
                {
                    GreigeIssuanceViewModel model = new GreigeIssuanceViewModel();
                    model.Date = item.TransactionDate.ToString(Helpers.CommonHelper.DateFormat);
                    model.GreigeIssuance = item;
                    model.GreigeIssuance.ApprovedBy = item.TransactionDate.ToString(Helpers.CommonHelper.DateFormat);
                    model.Productionorder = _dbContext.ProductionOrders.Where(x => x.Id==item.SpecificationId).Select(x => x.TransactionNo).FirstOrDefault();
                    model.IssuanceQty = _dbContext.GreigeIssuanceDetail.Where(x => x.GreigeIssuanceId==item.Id).Select(x => x.IssuanceQty).Sum();
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
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var Repo = new GreigeIssuanceRepo(_dbContext, HttpContext);
            bool isSuccess = await Repo.Approve(id, userId, companyId);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Greige Issuance has been approved successfully";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong.";
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> UnApprove(int id)
        {
            string userId = HttpContext.Session.GetString("UserId");
            bool isSuccess;

            var deleteItem = _dbContext.GreigeIssuance.Where(x => x.Id == id).FirstOrDefault();
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
                    var entry = _dbContext.GreigeIssuance.Update(deleteItem);

                    var storeVoucher = _dbContext.GLVoucherDetails.Where(x => x.VoucherId == deleteItem.VoucherId).ToList();
                    foreach (var item in storeVoucher)
                    {
                        item.IsDeleted = true;
                        var dbEntry = _dbContext.GLVoucherDetails.Update(item);
                        dbEntry.OriginalValues.SetValues(await dbEntry.GetDatabaseValuesAsync());
                        await _dbContext.SaveChangesAsync();
                    }


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
                TempData["message"] = "Greige Issuance has been Unapproved successfully";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Greige Issuance in Monthly planning";
            }


            //int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            //string _userId = HttpContext.Session.GetString("UserId");
            //ProductionOrder model = _dbContext.ProductionOrder.Find(id);
            //model.UnApprovedBy = _userId;
            //model.UnApprovedDate = DateTime.UtcNow;
            //model.IsApproved = false;
            //_dbContext.ProductionOrder.Update(model);
            //_dbContext.SaveChanges();
            //TempData["error"] = "false";
            //TempData["message"] = "Seasonal Planing has been UnApproved successfully.";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;

            var model = _dbContext.GreigeIssuance.Where(x=>x.CompanyId==companyId)
                .Include(p => p.GreigeIssuanceDetail)
                  
                .Include(tiers => tiers.GreigeIssuanceDetail)
                    .ThenInclude(contact => contact.GreigeQuality)
                .Include(tiers => tiers.GreigeIssuanceDetail)
                 .Include(x => x.Vendor)
               
                .Where(p => p.Id == id).FirstOrDefault();
           var ProductionOrder = (from a in _dbContext.ProductionOrders.Where(x => x.Id == model.SpecificationId && x.CompanyId==companyId)
                                       select new { 
                                           a.TransactionNo
                                       }).Distinct().FirstOrDefault();
            ViewBag.ProductionOrder = ProductionOrder.TransactionNo;
            return View(model);
        }
    }
}