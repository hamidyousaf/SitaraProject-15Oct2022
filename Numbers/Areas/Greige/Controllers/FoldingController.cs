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

namespace Numbers.Areas.Greige.Controllers
{
    [Area("Greige")]
    [Authorize]
    public class FoldingController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public FoldingController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet]
        public IActionResult Index()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var ContractRepo = new ContractRepo(_dbContext);
            IEnumerable<GRWeavingContract> items = ContractRepo.GetAll(companyId);
             
                ViewBag.NavbarHeading = "List of Folding";
                return View(items);
          
        }

        [HttpGet]
        public IActionResult Create(int id)
        {
            try
            {
                VwGRFolding viewModel = new VwGRFolding();
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                
                var api = new ApiController(_dbContext);

                ViewBag.WeavingContractLOV = api.GetWeavingContract(companyId);
                ViewBag.PurchaseContractLOV = api.PurchaseContractLOV(companyId);

                if (id == 0)
                {
                    ViewBag.EntityState = "Create";
                    ViewBag.NavbarHeading = "Create Folding";
                    int[] mendings = _dbContext.GRFolding.Where(x => !x.IsDeleted && x.CompanyId==companyId).Select(x => x.MendingId).ToArray();
                    ViewBag.Mending = new SelectList(_dbContext.GRMending.Where(x => !mendings.Contains(x.Id) && x.IsApproved && x.IsDeleted != true && x.CompanyId == companyId).ToList().OrderByDescending(x => x.Id), "Id", "TransactionNo"); ;

                    return View(viewModel);
                }
                else
                {
                    ViewBag.EntityState = "Update";
                    ViewBag.NavbarHeading = "Update Folding";
                    

                    VwGRFolding model = new VwGRFolding();
                    VwGRFolding item = new VwGRFolding(); // 
                    item.grFolding = _dbContext.GRFolding.Where(x => x.Id == id && x.CompanyId == companyId).FirstOrDefault();
                    var QualityVals= (from m in _dbContext.GRFolding
                                              join gq in _dbContext.GRQuality on m.ContractGRQualityId equals gq.Id
                                              join lq in _dbContext.GRQuality on m.PurchaseGRQualityId equals lq.Id
                                             
                                              select new
                                              {
                                                  Id=m.FoldingNo,
                                                 
                                                  LoomQualityDesc = lq.Description,
                                                  GreigeQualityDesc = gq.Description
                                              }).FirstOrDefault(x => x.Id == id);

                    model.grFolding.Id = item.grFolding.Id;
                    model.grFolding.FoldingNo = item.grFolding.FoldingNo;
                    model.grFolding.FoldingDate = item.grFolding.FoldingDate;
                    model.grFolding.WeavingContractNo = item.grFolding.WeavingContractNo;
                    model.grFolding.PurchaseContractNo = item.grFolding.PurchaseContractNo;
                    model.grFolding.ContractGRQualityId = item.grFolding.ContractGRQualityId;
                    model.grFolding.PurchaseGRQualityId = item.grFolding.PurchaseGRQualityId;
                    model.grFolding.MendingQty = item.grFolding.MendingQty;
                    model.grFolding.IGPLotNo = item.grFolding.IGPLotNo;
                    model.grFolding.MendingId = item.grFolding.MendingId;


                    model.grFolding.RecQty = item.grFolding.RecQty;
                    model.grFolding.MendQty = item.grFolding.MendQty;
                    model.grFolding.FoldQty = item.grFolding.FoldQty;
                    model.grFolding.GainLossQty = item.grFolding.GainLossQty;
                    model.grFolding.ActQty = item.grFolding.ActQty;
                    //model.ContractQualityDesc = QualityVals.GreigeQualityDesc;
                    //model.LoomQualityDesc = QualityVals.LoomQualityDesc;

                    ViewBag.Mending = new SelectList(_dbContext.GRMending.Where(x => x.Id == model.grFolding.MendingId && x.IsDeleted != true && x.CompanyId==companyId).ToList(), "Id", "TransactionNo"); ;
                    var det = _dbContext.GRFoldingItems.Where(x => x.FoldingId == id).ToList();
                    List<GRFoldingItems> Details = new List<GRFoldingItems>();

                    foreach (var grp in det)
                    {
                        GRFoldingItems modeld = new GRFoldingItems();
                        modeld.Id = grp.Id;
                        modeld.FoldingId = grp.FoldingId;
                        modeld.SrNo = grp.SrNo;
                        modeld.ReceivedQty = grp.ReceivedQty;
                        modeld.MendQty = grp.MendQty;
                        modeld.FoldQty = grp.FoldQty;
                        modeld.GainLossQty = grp.GainLossQty;
                        modeld.ActualFoldQty = grp.ActualFoldQty;
                        Details.Add(modeld);
                    }

                    model.GRFoldingDetails = Details.OrderBy(x => x.SrNo).ToList();
                    return View(model);
                }
            }catch(Exception e)
            {
                return View(new VwGRFolding());
            }
           
        }


        [HttpGet]
        public IActionResult Details(int id)
        {
            try
            {
                VwGRFolding viewModel = new VwGRFolding();
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;

                var api = new ApiController(_dbContext);

                ViewBag.WeavingContractLOV = api.GetWeavingContract(companyId);
                ViewBag.PurchaseContractLOV = api.PurchaseContractLOV(companyId);

                if (id == 0)
                {
                    ViewBag.EntityState = "Create";
                    ViewBag.NavbarHeading = "Create Folding";
                    int[] mendings = _dbContext.GRFolding.Where(x => !x.IsDeleted).Select(x => x.MendingId).ToArray();
                    ViewBag.Mending = new SelectList(_dbContext.GRMending.Where(x => !mendings.Contains(x.Id) && x.IsDeleted != true && x.CompanyId == companyId).ToList(), "Id", "TransactionNo"); ;

                    return View(viewModel);
                }
                else
                {
                    ViewBag.EntityState = "Update";
                    ViewBag.NavbarHeading = "Detail Folding";


                    VwGRFolding model = new VwGRFolding();
                    VwGRFolding item = new VwGRFolding(); // 
                    item.grFolding = _dbContext.GRFolding.Where(x => x.Id == id && x.CompanyId == companyId).FirstOrDefault();
                    var QualityVals = (from m in _dbContext.GRFolding
                                       join gq in _dbContext.GRQuality on m.ContractGRQualityId equals gq.Id
                                       join lq in _dbContext.GRQuality on m.PurchaseGRQualityId equals lq.Id
                                       select new
                                       {
                                           Id = m.FoldingNo,

                                           LoomQualityDesc = lq.Description,
                                           GreigeQualityDesc = gq.Description
                                       }).FirstOrDefault(x => x.Id == id);

                    model.grFolding.Id = item.grFolding.Id;
                    model.grFolding.FoldingNo = item.grFolding.FoldingNo;
                    model.grFolding.FoldingDate = item.grFolding.FoldingDate;
                    model.grFolding.WeavingContractNo = item.grFolding.WeavingContractNo;
                    model.grFolding.PurchaseContractNo = item.grFolding.PurchaseContractNo;
                    model.grFolding.ContractGRQualityId = item.grFolding.ContractGRQualityId;
                    model.grFolding.PurchaseGRQualityId = item.grFolding.PurchaseGRQualityId;
                    model.grFolding.MendingQty = item.grFolding.MendingQty;
                    model.grFolding.IGPLotNo = item.grFolding.IGPLotNo;
                    model.grFolding.MendingId = item.grFolding.MendingId;
                    //model.ContractQualityDesc = QualityVals.GreigeQualityDesc;
                    //model.LoomQualityDesc = QualityVals.LoomQualityDesc;
                    ViewBag.Mending = new SelectList(_dbContext.GRMending.Where(x => x.Id == model.grFolding.MendingId && x.IsDeleted != true && x.CompanyId == companyId).ToList(), "Id", "TransactionNo"); ;
                    var det = _dbContext.GRFoldingItems.Where(x => x.FoldingId == id).ToList();
                    List<GRFoldingItems> Details = new List<GRFoldingItems>();

                    foreach (var grp in det)
                    {
                        GRFoldingItems modeld = new GRFoldingItems();
                        modeld.Id = grp.Id;
                        modeld.FoldingId = grp.FoldingId;
                        modeld.SrNo = grp.SrNo;
                        modeld.ReceivedQty = grp.ReceivedQty;
                        modeld.MendQty = grp.MendQty;
                        modeld.FoldQty = grp.FoldQty;
                        modeld.GainLossQty = grp.GainLossQty;
                        modeld.ActualFoldQty = grp.ActualFoldQty;
                        Details.Add(modeld);
                    }

                    model.GRFoldingDetails = Details.OrderBy(x => x.SrNo).ToList();
                    return View(model);
                }
            }
            catch (Exception e)
            {
                return View(new VwGRFolding());
            }

        }



        [HttpPost]
        public async Task<IActionResult> Create(VwGRFolding mod , IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            GRFolding model = new GRFolding();
           
            
            List<GRFoldingItems> customersDetailss = new List<GRFoldingItems>();
            int TransactionNo = 1;
            var list = _dbContext.GRFolding.ToList();
            if (list.Count != 0)
            {
                TransactionNo = Convert.ToInt32(list.Select(x => x.FoldingNo).Max() + 1);
            }
            model.FoldingNo = TransactionNo;
            if (mod.grFolding.FoldingNo == 0)
            {
                model.CompanyId = companyId;
                model.CreatedBy = userId;
                model.CompanyId = companyId;
                model.Resp_ID = resp_Id;
                model.CreatedBy = userId;
                model.CreatedDate = Convert.ToDateTime(DateTime.Now);
                model.IsDeleted = false;
                model.FoldingDate= Convert.ToDateTime(mod.grFolding.FoldingDate);
                model.FoldingNo = TransactionNo;
                model.ApprovedDate = Convert.ToDateTime(DateTime.Now);
                model.UpdatedDate = Convert.ToDateTime(DateTime.Now);
                model.UnApprovedDate = Convert.ToDateTime(DateTime.Now);
                model.MendingId = mod.grFolding.MendingId;
                //model.WeavingContractNo = mod.grFolding.WeavingContractNo;
                //model.PurchaseContractNo = mod.grFolding.PurchaseContractNo;
                model.WeavingContractNo = Convert.ToInt32(collection["WeavingContractNo"]);
                model.PurchaseContractNo =Convert.ToInt32( collection["PurchaseContractNo"]);
                model.ContractGRQualityId = Convert.ToInt32(collection["WeavingContractNo"]);
                model.PurchaseGRQualityId = Convert.ToInt32( collection["PurchaseContractNo"]);
                model.MendingQty = mod.grFolding.MendingQty;
                model.IGPLotNo = mod.grFolding.IGPLotNo;
                model.RecQty = Convert.ToDecimal(collection["RecQty"]);
                model.MendQty = Convert.ToDecimal(collection["MendQty"]);
                model.FoldQty = Convert.ToDecimal(collection["tFoldQty"]);
                model.GainLossQty = Convert.ToDecimal(collection["tGainLossQty"]);
                model.ActQty = Convert.ToDecimal(collection["tActQty"]);
                _dbContext.GRFolding.Add(model);
                await _dbContext.SaveChangesAsync();
                List<GRFoldingItems> customersDetails = new List<GRFoldingItems>();

                for (int i = 0; i < collection["srNo"].Count; i++)
                {
                    GRFoldingItems modeld = new GRFoldingItems();
                    modeld.FoldingId = model.Id;
                    modeld.SrNo = Convert.ToInt32(collection["srNo"][i]);
                    modeld.ReceivedQty= Convert.ToDecimal(collection["receivedQuantity"][i]);
                    modeld.MendQty= Convert.ToDecimal(collection["freshQuantity"][i]);
                    modeld.FoldQty= Convert.ToDecimal(collection["foldQty"][i]);
                    modeld.GainLossQty = Convert.ToDecimal(collection["gainLossQty"][i]);
                    modeld.ActualFoldQty = Convert.ToDecimal(collection["ActualFoldQty"][i]);
                    customersDetails.Add(modeld);
                    _dbContext.GRFoldingItems.Add(modeld);
                    await _dbContext.SaveChangesAsync();
                }
                TempData["error"] = "false";
                TempData["message"] = "Folding No "+ TransactionNo + " has been saved successfully.";
                return RedirectToAction(nameof(Index));
 
            }
            else
            {
                model = _dbContext.GRFolding.FirstOrDefault(x => x.Id == mod.grFolding.Id);
                model.WeavingContractNo = Convert.ToInt32(collection["WeavingContractNo"]);
                model.PurchaseContractNo = Convert.ToInt32(collection["PurchaseContractNo"]);
                model.ContractGRQualityId = Convert.ToInt32(collection["WeavingContractNo"]);
                model.PurchaseGRQualityId = Convert.ToInt32(collection["PurchaseContractNo"]);
                model.MendingQty = mod.grFolding.MendingQty;
                model.IGPLotNo = mod.grFolding.IGPLotNo;
                model.RecQty = Convert.ToDecimal(collection["RecQty"]);
                model.MendQty = Convert.ToDecimal(collection["MendQty"]);
                model.FoldQty = Convert.ToDecimal(collection["tFoldQty"]);
                model.GainLossQty = Convert.ToDecimal(collection["tGainLossQty"]);
                model.ActQty = Convert.ToDecimal(collection["tActQty"]);
                model.IsDeleted = false;
                model.UpdatedBy = userId;
                model.UpdatedDate = DateTime.Now;

                _dbContext.GRFolding.Update(model);
                await _dbContext.SaveChangesAsync();

                //Inserting/Updating monthly limit
                for (int i = 0; i < collection["id"].Count; i++)
                {
                    var detailId = Convert.ToInt32(collection["id"][i]);
                    if (detailId == 0) //Inserting New Records
                    {
                        GRFoldingItems Items = new GRFoldingItems();
                        Items.FoldingId = model.Id;
                        Items.SrNo = Convert.ToInt32(collection["srNo"][i]);
                        Items.ReceivedQty = Convert.ToDecimal(collection["receivedQuantity"][i]);
                        Items.MendQty = Convert.ToDecimal(collection["freshQuantity"][i]);
                        Items.FoldQty = Convert.ToDecimal(collection["foldQty"][i]);
                        Items.GainLossQty = Convert.ToDecimal(collection["gainLossQty"][i]);
                        Items.ActualFoldQty = Convert.ToDecimal(collection["ActualFoldQty"][i]);
                        await _dbContext.GRFoldingItems.AddAsync(Items);
                    }
                    else
                    {
                        GRFoldingItems Items = _dbContext.GRFoldingItems.FirstOrDefault(x => x.Id == detailId);
                        Items.SrNo = Convert.ToInt32(collection["srNo"][i]);
                        Items.ReceivedQty = Convert.ToDecimal(collection["receivedQuantity"][i]);
                        Items.MendQty = Convert.ToDecimal(collection["freshQuantity"][i]);
                        Items.FoldQty = Convert.ToDecimal(collection["foldQty"][i]);
                        Items.GainLossQty = Convert.ToDecimal(collection["gainLossQty"][i]);
                        Items.ActualFoldQty = Convert.ToDecimal(collection["ActualFoldQty"][i]);
                        _dbContext.GRFoldingItems.Update(Items);
                    }
                    await _dbContext.SaveChangesAsync();
                }
                await _dbContext.SaveChangesAsync();
                TempData["error"] = "false";
                TempData["message"] = "Folding has "+ model.FoldingNo + " been updated successfully.";
                return RedirectToAction(nameof(Index));
            }
        }
 
        public async Task<IActionResult> Delete(int id)
        {
            //var ContractRepo = new GRFolding(_dbContext);

            bool isSuccess ;

            var deleteItem = _dbContext.GRFolding.Where(x => x.Id == id).FirstOrDefault();
            if (deleteItem == null)
            {
                isSuccess = false; 
            }
            else
            {
                deleteItem.IsDeleted = true;
                var entry = _dbContext.GRFolding.Update(deleteItem);
                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                await _dbContext.SaveChangesAsync();
                isSuccess = true;
            }

            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Folding has been deleted successfully";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Folding not found";
            }
            return RedirectToAction(nameof(Index)); 
        }

        public JsonResult checkProductCodeAlreadyExists(string code)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            System.Threading.Thread.Sleep(200);
            if (code == "0")
                return Json(0);
            var chkCode = _dbContext.GRWeavingContracts.Where(a =>  a.IsDeleted == false  && a.CompanyId == companyId).FirstOrDefault();
            return Json(chkCode == null ? 0 : 1);
        }
        [HttpGet]
        public IActionResult GetItems(string q = "")
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var items = _dbContext.GRWeavingContracts.Where(a => a.CompanyId == companyId && a.IsDeleted == false)
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   name = a.TransactionNo
                                               });
            return Ok(items);
        }


        [HttpGet]
        public IActionResult GetMendingData(int Id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            if (Id != 0)
            {
                //var GRWeavingContracts = _dbContext.GRMending.Where(x => x.Id == Id).ToList();
                //var GRWeavingContracts = (from m in _dbContext.GRMending
                //                          join gq in  _dbContext.GRQuality on m.ContractQualityId equals gq.Id
                //                          join lq in _dbContext.GRQuality on m.LoomQualityId equals lq.Id
                //                          select new { 
                //                              m.Id,
                //                          m.LotNo,
                //                          m.ReceivedQuantity,
                //                          LoomQualityDesc = lq.Description,
                //                          GreigeQualityDesc=gq.Description,
                //                          m.ContractQualityId,
                //                          m.LoomQualityId,
                //                          m.PurchaseContractNo,
                //                          m.WeavingContractNo

                //                          }).FirstOrDefault(x => x.Id==Id);

                var GRWeavingContracts = (from m in _dbContext.GRMending.Include(x=>x.GRIGP)
                                          where m.CompanyId == companyId
                                          select new
                                          {
                                              m.Id,
                                              m.GRIGP.LotNo,
                                              m.GRIGPId,
                                              m.ReceivedQuantity,
                                              GreigeQualityDesc  =_dbContext.GRWeavingContracts.Include(x=>x.GreigeQuality).Where(x=>x.Id==m.GRIGP.WeavingContractId).Select(x=>x.GreigeQuality.Description).FirstOrDefault() ?? _dbContext.GRPurchaseContract.Include(x => x.ContractGRQuality).Where(x => x.Id == m.GRIGP.PurchaseContractId).Select(x => x.ContractGRQuality.Description).FirstOrDefault() ?? "",
                                              LoomQualityDesc = _dbContext.GRWeavingContracts.Include(x => x.GreigeQualityLoom).Where(x => x.Id == m.GRIGP.WeavingContractId).Select(x => x.GreigeQualityLoom.Description).FirstOrDefault() ?? _dbContext.GRPurchaseContract.Include(x => x.PurchaseGRQuality).Where(x => x.Id == m.GRIGP.PurchaseContractId).Select(x => x.PurchaseGRQuality.Description).FirstOrDefault() ?? "",
                                              m.ContractQualityId,
                                              m.LoomQualityId,
                                              PurchaseContractNo =m.GRIGP.PurchaseContractId,
                                              WeavingContractNo =m.GRIGP.WeavingContractId,
                                          }).FirstOrDefault(x => x.Id == Id);
                
                var api = new ApiController(_dbContext);
                var PurchaseContract = api.PurchaseContractById(GRWeavingContracts.PurchaseContractNo, companyId);
                var WeavingContract = api.GetWeavingContractById(GRWeavingContracts.WeavingContractNo, companyId);
                if (GRWeavingContracts != null)
                {
                    return Ok(new { GRWeavingContracts = GRWeavingContracts,PurchaseContract = PurchaseContract, WeavingContract= WeavingContract });
                }
                return Ok(null);
            }
            return Ok(null);
        }

        [HttpGet]
        public IActionResult GetMendingDataDetail(int Id)
        {
            if (Id != 0)
            {
                var GRWeavingContracts = _dbContext.GRMendingDetail.Where(x => x.GRMendingId == Id).OrderBy(x => x.SrNo).ToList();

                if (GRWeavingContracts != null)
                {
                    return Ok(GRWeavingContracts);
                }
                return Ok(null);
            }
            return Ok(null);
        }


        //[HttpGet]
        //public IActionResult GetWeavingContract(int Id)
        //{
        //    if (Id != 0)
        //    {
        //        var address = _dbContext.GRWeavingContracts.Include(x => x.Vendor).Include(x => x.GreigeQuality).Include(x => x.GreigeQualityLoom).ThenInclude(x => x.GRConstruction).FirstOrDefault(x => x.Id == Id);
        //        if (address != null)
        //        {
        //            return Ok(address);
        //        }
        //        return Ok(null);
        //    }
        //    return Ok(null);
        //}

        //[HttpGet]
        //public IActionResult GetPurchaseContract(int Id)
        //{
        //    if (Id != 0)
        //    {
        //        var address = _dbContext.GRPurchaseContract.Include(x => x.Vendor).Include(x => x.ContractGRQuality).ThenInclude(x => x.GRConstruction).Include(x => x.PurchaseGRQuality).FirstOrDefault(x => x.Id == Id);
        //        if (address != null)
        //        {
        //            return Ok(address);
        //        }
        //        return Ok(null);
        //    }
        //    return Ok(null);
        //}
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
                var searchFoldQty = Request.Form["columns[4][search][value]"].FirstOrDefault();
                var searchGain = Request.Form["columns[5][search][value]"].FirstOrDefault();
                var searchActual = Request.Form["columns[6][search][value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                //var InvData = (from m in _dbContext.GRFolding
                //               join d in _dbContext.GRFoldingItems on m.FoldingNo equals d.FoldingId
                //               where m.IsDeleted != true
                //               select new
                //               {
                //                   m.FoldingNo,
                //                   FoldingDate = m.FoldingDate.ToString("dd-MMM-yyyy"),
                //                   ReceivedQty = m.RecQty,
                //                   MendQty = m.MendQty,
                //                   FoldQty = m.FoldQty,
                //                   GainLossQty = m.GainLossQty,
                //                   ActualFoldQty = m.ActQty
                //               });
                var InvData = _dbContext.GRFolding.Where(x => x.IsDeleted != true && x.CompanyId==companyId);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    InvData = InvData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                InvData = !string.IsNullOrEmpty(searchItemCode) ? InvData.Where(m => m.FoldingNo.ToString().ToUpper().Contains(searchItemCode.ToUpper())) : InvData;
                InvData = !string.IsNullOrEmpty(searchItemName) ? InvData.Where(m => m.FoldingDate.ToString().ToUpper().Contains(searchItemName.ToUpper())) : InvData;
                InvData = !string.IsNullOrEmpty(searchBarcode) ? InvData.Where(m => m.RecQty.ToString().ToUpper().Contains(searchBarcode.ToUpper())) : InvData;
                InvData = !string.IsNullOrEmpty(searchUOM) ? InvData.Where(m => m.MendQty.ToString().ToUpper().Contains(searchUOM.ToUpper())) : InvData;
                InvData = !string.IsNullOrEmpty(searchFoldQty) ? InvData.Where(m => m.FoldQty.ToString().ToUpper().Contains(searchFoldQty.ToUpper())) : InvData;
                InvData = !string.IsNullOrEmpty(searchGain) ? InvData.Where(m => m.GainLossQty.ToString().ToUpper().Contains(searchGain.ToUpper())) : InvData;
                InvData = !string.IsNullOrEmpty(searchActual) ? InvData.Where(m => m.ActQty.ToString().ToUpper().Contains(searchActual.ToUpper())) : InvData;

                //recordsTotal = InvData.Count();

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

                //var data = InvData.Skip(skip).Take(pageSize).ToList();

                List<VwGRFolding> viewModel = new List<VwGRFolding>();
                foreach (var item in data)
                {
                    VwGRFolding model = new VwGRFolding();
                    model.grFolding = item;
                    model.grFolding.Approve = approve;
                    model.grFolding.Unapprove = unApprove;
                    model.Date = item.FoldingDate.ToString(Helpers.CommonHelper.DateFormat);
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


        public IActionResult Approve(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            GRFolding model = _dbContext.GRFolding.Where(x=>x.Id==id && x.CompanyId==_companyId).FirstOrDefault();
            model.ApprovedBy = _userId;
            model.ApprovedDate = DateTime.UtcNow;
            model.IsApproved = true;
            model.Status = "Approved";
            _dbContext.GRFolding.Update(model);
            _dbContext.SaveChanges();
            TempData["error"] = "false";
            TempData["message"] = "Folding has been approved successfully.";
            return RedirectToAction("Index", "Folding");
        }
        public IActionResult UnApprove(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            GRFolding model = _dbContext.GRFolding.Where(x => x.Id == id && x.CompanyId == _companyId).FirstOrDefault();


            var checkCatgryRfrnc = _dbContext.GRGRNS.Where(x => x.FoldingId == id && x.CompanyId==_companyId).ToList();
            if (checkCatgryRfrnc.Count==0)
            {
                model.ApprovedBy = _userId;
                model.ApprovedDate = DateTime.UtcNow;
                model.IsApproved = false;
                model.Status = "Created";
                _dbContext.GRFolding.Update(model);
                _dbContext.SaveChanges();
                TempData["error"] = "false";
                TempData["message"] = "Folding has been UnApproved successfully.";

            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Transaction No is Used in GRN..!";
            }


            return RedirectToAction("Index", "Folding");

            
        }
    }
}