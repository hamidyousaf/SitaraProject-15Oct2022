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
//using Numbers.Areas.Inventory.Controllers.ApiController;

namespace Numbers.Areas.Greige.Controllers
{
    [Area("Greige")]
    [Authorize]
    public class GRRequisitionController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public GRRequisitionController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet]
        public IActionResult Index()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var ContractRepo = new ContractRepo(_dbContext);
            IEnumerable<GRWeavingContract> items = ContractRepo.GetAll(companyId);
             
                ViewBag.NavbarHeading = "List of Greige Requisition";
                return View(items);
          
          
        }

        [HttpGet]
        public IActionResult Create(int id)
        {
            try
            {
                VMGriegeRequisition viewModel = new VMGriegeRequisition();
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                
                

                ViewBag.Department = new SelectList(_dbContext.GLDivision.Where(x => x.Name == "GODOWN").ToList(), "Id", "Name");
                ViewBag.SubDepartment = new SelectList(_dbContext.GLSubDivision.Where(x => x.Name == "GREIGE (LOCAL)").ToList(), "Id", "Name");

                if (id == 0)
                {
                    ViewBag.EntityState = "Create";
                    ViewBag.NavbarHeading = "Create Griege Requisition";
                 

                    return View(viewModel);
                }
                else
                {
                    ViewBag.EntityState = "Update";
                    ViewBag.NavbarHeading = "Update Griege Requisition";

                    VMGriegeRequisition model = new VMGriegeRequisition();

                    VMGriegeRequisition item = new VMGriegeRequisition(); // 
                    item.GriegeRequisition = _dbContext.GRGriegeRequisition.Where(x => x.Id == id && x.TransferToCompany==companyId).FirstOrDefault();
                    

                    model.GriegeRequisition.Id = item.GriegeRequisition.Id;
                    model.GriegeRequisition.TransactionNo = item.GriegeRequisition.TransactionNo;
                    model.GriegeRequisition.TransactionDate = item.GriegeRequisition.TransactionDate;
                    model.GriegeRequisition.DepartmentId = item.GriegeRequisition.DepartmentId;
                    model.GriegeRequisition.SubDepartmentId = item.GriegeRequisition.SubDepartmentId;
                    model.GriegeRequisition.IsCreatePR = item.GriegeRequisition.IsCreatePR;
                    model.GriegeRequisition.SPID = item.GriegeRequisition.SPID;
                   
                    //model.ContractQualityDesc = QualityVals.GreigeQualityDesc;
                    //model.LoomQualityDesc = QualityVals.LoomQualityDesc;
                   
                    var det = _dbContext.GRGriegeRequisitionDetails.Include(x=>x.GriegeQuality)
                        .Where(x => x.GRRequisitionId == id).ToList();
                    List<VMGRGriegeRequisitionDetails> Details = new List<VMGRGriegeRequisitionDetails>();

                    foreach (var grp in det)
                    {
                        VMGRGriegeRequisitionDetails modeld = new VMGRGriegeRequisitionDetails();
                        modeld.Id = grp.Id;
                        modeld.GRRequisitionId = grp.GRRequisitionId;
                        modeld.SPNo = grp.SPNo;
                        modeld.GriegeQualityId = grp.GriegeQualityId;
                        modeld.UOMId = grp.UOMId;
                        modeld.AvailableStock = Convert.ToInt32(_dbContext.InvItems.FirstOrDefault(x => x.Id == grp.GriegeQuality.ItemId).StockQty);
                        modeld.ReserveAvailableQty = grp.ReserveAvailableQty;
                        modeld.Qty = grp.Qty;
                        modeld.WarpBag = grp.WarpBag;
                        modeld.WeftBag = grp.WeftBag;
                        modeld.WarpBagWOQ = grp.WarpBagWOQ;
                        modeld.WeftBagWOQ = grp.WeftBagWOQ;
                        modeld.UOMName = (_dbContext.AppCompanyConfigs.Where(x => x.Id == grp.UOMId).Select(x => x.ConfigValue)).FirstOrDefault();
                        modeld.GRQuality = (_dbContext.GRQuality.Where(x => x.Id == grp.GriegeQualityId).Select(x => x.Description)).FirstOrDefault();
                        modeld.seasonalFabricCons = (_dbContext.SeasonalPlaningDetail.Where(x => x.GreigeQualityId == grp.GriegeQualityId ).Select(x => x.FabricConsumption)).Sum();
                        
                        Details.Add(modeld);
                    }

                    model.GRGriegeRequisitionDetailsList = Details.OrderBy(x => x.SPNo).ToList();
                   

                    return View(model);
                }
            }catch(Exception e)
            {
                return View(new VMGriegeRequisition());
            }
           
        }

        [HttpGet]
        public IActionResult View(int id)
        {
            try
            {
                VMGriegeRequisition viewModel = new VMGriegeRequisition();
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;



                ViewBag.Department = new SelectList(_dbContext.GLDivision.Where(x => x.Id == 8).ToList(), "Id", "Name");
                ViewBag.SubDepartment = new SelectList(_dbContext.GLSubDivision.Where(x => x.Id == 25).ToList(), "Id", "Name");

                if (id == 0)
                {
                    ViewBag.EntityState = "Create";
                    ViewBag.NavbarHeading = "Create Griege Requisition";


                    return View(viewModel);
                }
                else
                {
                    ViewBag.EntityState = "View";
                    ViewBag.NavbarHeading = "Griege Requisition";

                    VMGriegeRequisition model = new VMGriegeRequisition();

                    VMGriegeRequisition item = new VMGriegeRequisition(); // 
                    item.GriegeRequisition = _dbContext.GRGriegeRequisition.Where(x => x.Id == id && x.TransferToCompany==companyId).FirstOrDefault();


                    model.GriegeRequisition.Id = item.GriegeRequisition.Id;
                    model.GriegeRequisition.TransactionNo = item.GriegeRequisition.TransactionNo;
                    model.GriegeRequisition.TransactionDate = item.GriegeRequisition.TransactionDate;
                    model.GriegeRequisition.DepartmentId = item.GriegeRequisition.DepartmentId;
                    model.GriegeRequisition.SubDepartmentId = item.GriegeRequisition.SubDepartmentId;
                    model.GriegeRequisition.IsCreatePR = item.GriegeRequisition.IsCreatePR;

                    //model.ContractQualityDesc = QualityVals.GreigeQualityDesc;
                    //model.LoomQualityDesc = QualityVals.LoomQualityDesc;

                    var det = _dbContext.GRGriegeRequisitionDetails.Where(x => x.GRRequisitionId == id).ToList();
                    List<VMGRGriegeRequisitionDetails> Details = new List<VMGRGriegeRequisitionDetails>();

                    foreach (var grp in det)
                    {
                        VMGRGriegeRequisitionDetails modeld = new VMGRGriegeRequisitionDetails();
                        modeld.Id = grp.Id;
                        modeld.GRRequisitionId = grp.GRRequisitionId;
                        modeld.SPNo = grp.SPNo;
                        modeld.GriegeQualityId = grp.GriegeQualityId;
                        modeld.UOMId = grp.UOMId;
                        modeld.AvailableStock = grp.AvailableStock;
                        modeld.ReserveAvailableQty = grp.ReserveAvailableQty;
                        modeld.Qty = grp.Qty;
                        modeld.WarpBag = grp.WarpBag;
                        modeld.WeftBag = grp.WeftBag;
                        modeld.WarpBagWOQ = grp.WarpBagWOQ;
                        modeld.WeftBagWOQ = grp.WeftBagWOQ;
                        modeld.UOMName = (_dbContext.AppCompanyConfigs.Where(x => x.Id == grp.UOMId).Select(x => x.ConfigValue)).FirstOrDefault();
                        modeld.GRQuality = (_dbContext.GRQuality.Where(x => x.Id == grp.GriegeQualityId).Select(x => x.Description)).FirstOrDefault();
                        modeld.seasonalFabricCons = (_dbContext.SeasonalPlaningDetail.Where(x => x.GreigeQualityId == grp.GriegeQualityId).Select(x => x.FabricConsumption)).Sum();

                        Details.Add(modeld);
                    }

                    model.GRGriegeRequisitionDetailsList = Details.OrderBy(x => x.SPNo).ToList();


                    return View(model);
                }
            }
            catch (Exception e)
            {
                return View(new VMGriegeRequisition());
            }

        }

        [HttpGet]
        public IActionResult Detail(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            VMGriegeRequisition model = new VMGriegeRequisition();

            VMGriegeRequisition item = new VMGriegeRequisition(); // 
            item.GriegeRequisition = _dbContext.GRGriegeRequisition.Where(x => x.Id == id && x.CompanyId == companyId).FirstOrDefault();


            model.GriegeRequisition.Id = item.GriegeRequisition.Id;
            model.GriegeRequisition.TransactionNo = item.GriegeRequisition.TransactionNo;
            model.GriegeRequisition.TransactionDate = item.GriegeRequisition.TransactionDate;
            model.GriegeRequisition.DepartmentId = item.GriegeRequisition.DepartmentId;
            model.GriegeRequisition.SubDepartmentId = item.GriegeRequisition.SubDepartmentId;
            model.DepartmentName= (_dbContext.GLDivision.Where(x => x.Id == model.GriegeRequisition.DepartmentId).Select(x => x.Name)).FirstOrDefault();
            model.SubDepartmentName= (_dbContext.GLSubDivision.Where(x => x.Id == model.GriegeRequisition.SubDepartmentId).Select(x => x.Name)).FirstOrDefault();
            //model.ContractQualityDesc = QualityVals.GreigeQualityDesc;
            //model.LoomQualityDesc = QualityVals.LoomQualityDesc;

            var det = _dbContext.GRGriegeRequisitionDetails.Where(x => x.GRRequisitionId == id).ToList();
            List<VMGRGriegeRequisitionDetails> Details = new List<VMGRGriegeRequisitionDetails>();

            foreach (var grp in det)
            {
                VMGRGriegeRequisitionDetails modeld = new VMGRGriegeRequisitionDetails();
                modeld.Id = grp.Id;
                modeld.GRRequisitionId = grp.GRRequisitionId;
                modeld.SPNo = grp.SPNo;
                modeld.GriegeQualityId = grp.GriegeQualityId;
                modeld.UOMId = grp.UOMId;
                modeld.AvailableStock = grp.AvailableStock;
                modeld.ReserveAvailableQty = grp.ReserveAvailableQty;
                modeld.Qty = grp.Qty;
                modeld.WarpBag = grp.WarpBag;
                modeld.WeftBag = grp.WeftBag;
                
                modeld.UOMName = (_dbContext.AppCompanyConfigs.Where(x => x.Id == grp.UOMId).Select(x => x.ConfigValue)).FirstOrDefault();
                modeld.GRQuality = (_dbContext.GRQuality.Where(x => x.Id == grp.GriegeQualityId).Select(x => x.Description)).FirstOrDefault();

                Details.Add(modeld);
            }

            model.GRGriegeRequisitionDetailsList = Details.OrderBy(x => x.SPNo).ToList();


            return View(model);

        }



        [HttpPost]
        public async Task<IActionResult> Create(VMGriegeRequisition mod , IFormCollection collection)
        {
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
            if (mod.GriegeRequisition.TransactionNo == 0)
            {
                
                model.CreatedBy = userId;
                model.CompanyId = companyId;
                model.Resp_ID = resp_Id;
                model.CreatedBy = userId;
                model.CreatedDate = Convert.ToDateTime(DateTime.Now);
                model.IsDeleted = false;
                model.TransactionDate= Convert.ToDateTime(mod.GriegeRequisition.TransactionDate);
                model.TransactionNo = TransactionNo;
                model.ApprovedDate = mod.GriegeRequisition.ApprovedDate;
                model.UpdatedDate = mod.GriegeRequisition.UpdatedDate;
                model.UnApprovedDate = mod.GriegeRequisition.UnApprovedDate;
                model.DepartmentId = mod.GriegeRequisition.DepartmentId;
                model.SubDepartmentId = mod.GriegeRequisition.SubDepartmentId;
                model.IsCreatePR = mod.GriegeRequisition.IsCreatePR;
                model.IsApproved = false;
                model.Status = "Created";
                model.UnApprovedBy = mod.GriegeRequisition.UnApprovedBy;
                model.Deletedby = mod.GriegeRequisition.Deletedby;
                model.DeletedDate = mod.GriegeRequisition.DeletedDate;
                model.UpdatedBy = model.UpdatedBy;
                
                _dbContext.GRGriegeRequisition.Add(model);
                await _dbContext.SaveChangesAsync();
                List<GRGriegeRequisitionDetails> customersDetails = new List<GRGriegeRequisitionDetails>();

                for (int i = 0; i < collection["spNo"].Count; i++)
                {
                    GRGriegeRequisitionDetails modeld = new GRGriegeRequisitionDetails();
                    modeld.SPNo = Convert.ToInt32(collection["spNo"][i]); ;
                    modeld.GriegeQualityId = Convert.ToInt32(collection["specificationId"][i]);
                    modeld.UOMId= Convert.ToInt32(collection["unitId"][i]);
                    modeld.AvailableStock= Convert.ToInt32(collection["availableStock"][i]);
                    modeld.ReserveAvailableQty= Convert.ToInt32(collection["reserveQty"][i]);
                    modeld.Qty = Convert.ToInt32(collection["qty"][i]);
                    modeld.WarpBag = Convert.ToInt32(collection["warpbag"][i]);
                    modeld.WeftBag = Convert.ToInt32(collection["weftbag"][i]);
                    modeld.GRRequisitionId = model.Id;
                    customersDetails.Add(modeld);
                }
                _dbContext.GRGriegeRequisitionDetails.AddRange(customersDetails);
                await _dbContext.SaveChangesAsync();
                TempData["error"] = "false";
                TempData["message"] = "Greige Requisition # "+ TransactionNo + " has been saved successfully.";
                return RedirectToAction(nameof(Index));
 
            }
            else
            {
                model = _dbContext.GRGriegeRequisition.FirstOrDefault(x => x.Id == mod.GriegeRequisition.Id);
                model.TransactionNo = mod.GriegeRequisition.TransactionNo;
                model.TransactionDate = Convert.ToDateTime(mod.GriegeRequisition.TransactionDate);
                model.DepartmentId = mod.GriegeRequisition.DepartmentId;
                model.SubDepartmentId = mod.GriegeRequisition.SubDepartmentId;
                model.IsCreatePR = mod.GriegeRequisition.IsCreatePR;
                model.UpdatedDate = DateTime.Now;
                model.UpdatedBy = userId;
                _dbContext.GRGriegeRequisition.Update(model);

                var existingDetail = _dbContext.GRGriegeRequisitionDetails.Where(x => x.GRRequisitionId == mod.GriegeRequisition.Id).ToList();
                //Deleting monthly target limit
                List<int> ids = new List<int>();
                for (int i = 0; i < collection["id"].Count; i++)
                {
                    ids.Add(Convert.ToInt32(collection["id"][i]));
                }
                foreach (var detail in existingDetail)
                {
                    bool isExist = ids.Any(x => x == detail.Id);
                    if (!isExist)
                    {
                        _dbContext.GRGriegeRequisitionDetails.Remove(detail);
                        await _dbContext.SaveChangesAsync();
                    }
                }
                //Inserting/Updating monthly limit
                for (int i = 0; i < collection["id"].Count; i++)
                {
                    int detailId = Convert.ToInt32(collection["id"][i]);
                    if (detailId != 0) //Inserting New Records
                    {
                        GRGriegeRequisitionDetails Items = _dbContext.GRGriegeRequisitionDetails.FirstOrDefault(x => x.Id == detailId);
                        Items.SPNo = Convert.ToInt32(collection["spNo"][i]);
                        Items.GriegeQualityId = Convert.ToInt32(collection["specificationId"][i]);
                        Items.UOMId = Convert.ToInt32(collection["unitId"][i]);
                        Items.AvailableStock = Convert.ToInt32(collection["availableStock"][i]);
                        Items.ReserveAvailableQty = Convert.ToInt32(collection["reserveQty"][i]);
                        Items.Qty = Convert.ToInt32(collection["qty"][i]);
                        Items.BalanceQty = Convert.ToInt32(collection["qty"][i]);
                        Items.WarpBag = Convert.ToDecimal(collection["warpbag"][i]);
                        Items.WeftBag = Convert.ToDecimal(collection["weftbag"][i]);
                        Items.WarpBagWOQ = collection["WarpBagWOQ"][i];
                        Items.WeftBagWOQ = collection["WeftBagWOQ"][i];
                        Items.GRRequisitionId = model.Id;
                        Items.IsUsed = false;
                    }
                    await _dbContext.SaveChangesAsync();
                }
                await _dbContext.SaveChangesAsync();
                TempData["error"] = "false";
                TempData["message"] = "Greige Requisition  # " + model.TransactionNo + " has been updated successfully.";
                return RedirectToAction(nameof(Index));

            }
        }
 
        public async Task<IActionResult> Delete(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;

            bool isSuccess ;

            var deleteItem = _dbContext.GRGriegeRequisition.Where(x => x.Id == id && x.CompanyId==companyId).FirstOrDefault();
            if (deleteItem == null)
            {
                isSuccess = false; 
            }
            else
            {
                deleteItem.IsDeleted = true;
                deleteItem.Deletedby = userId;
                deleteItem.DeletedDate= Convert.ToDateTime(DateTime.Now);
                var entry = _dbContext.GRGriegeRequisition.Update(deleteItem);
                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                await _dbContext.SaveChangesAsync();
                isSuccess = true;
            }

            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Greige Requisition has been deleted successfully";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Greige Requisition not found";
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

                var GRWeavingContracts = (from m in _dbContext.GRMending.Include(x=>x.GRIGP )
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
                var PurchaseContract = api.PurchaseContractById(Convert.ToInt32( GRWeavingContracts.PurchaseContractNo));
                var WeavingContract = api.GetWeavingContractById(Convert.ToInt32(GRWeavingContracts.WeavingContractNo));
                if (GRWeavingContracts != null)
                {
                    return Ok(new { GRWeavingContracts = GRWeavingContracts,PurchaseContract = PurchaseContract, WeavingContract= WeavingContract });
                }
                return Ok(null);
            }
            return Ok(null);
        }

        [HttpGet]
        public IActionResult GetSPData()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;

            

            var SeasonalPlanData = (from a in _dbContext.SeasonalPlaning.Where(x => x.IsDeleted == false)
                                        join b in _dbContext.SeasonalPlaningDetail on a.Id equals b.SeasonalPlaningId
                                        join q in _dbContext.GRQuality on b.GreigeQualityId equals q.Id
                                        join i in _dbContext.InvItems on q.ItemId equals i.Id
                                        join app in _dbContext.AppCompanyConfigs on i.Unit equals app.Id
                                       
                                        where a.IsDeleted == false && a.IsApproved==true && a.CompanyId==companyId
                                        select new
                                        {   q.ItemId,
                                            SpNo = a.TransactionNo,
                                            SpecificationId= b.GreigeQualityId,
                                            Specification = (_dbContext.GRQuality.Where(x => x.Id == b.GreigeQualityId ).Select(x => x.Description)).FirstOrDefault(),
                                            AvailableStock = (_dbContext.VwInvLedgers.Where(x => x.ItemId == q.ItemId ).Select(x => x.Qty)).Sum(),
                                            Uom = app.ConfigValue,
                                            UnitId= i.Unit,
                                            ReserveQty = 0,
                                            Qty=0,
                                            Warpbag = 0,
                                            Weftbag=0,
                                            SeasonalFabricCons = b.FabricConsumption
                                        }).Distinct().ToList();
               

                if (SeasonalPlanData != null)
                {
                    return Ok(SeasonalPlanData);
                }
                return Ok(null);
           
          
        }


        [HttpGet]
        public IActionResult GetGreigeReqData(int Id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;



            var SeasonalPlanData = (from a in _dbContext.GRGriegeRequisitionDetails.Where(x=>x.Id==Id)
                                    select new
                                    {
                                        AvlblQty = a.AvailableStock,
                                        Warpbag = a.WarpBagWOQ,
                                        Weftbag = a.WeftBagWOQ,
                                        SeasonalFabricCons = a.Qty
                                    }).Distinct().ToList();


            if (SeasonalPlanData != null)
            {
                return Ok(SeasonalPlanData);
            }
            return Ok(null);


        }

        public IActionResult GetStockByItemWarehouse(int itemId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            try
            {
                var Stock = _dbContext.VwInvLedgers
               .Where(s => s.ItemId == itemId )
               .Select(s => s.Qty).DefaultIfEmpty(0).ToArray();
                var totalStock = Convert.ToInt32(Stock.Sum());
                return Ok(totalStock);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
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

                var searchTransNo = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchSPNo = Request.Form["columns[1][search][value]"].FirstOrDefault();   
                var searchDate = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchDepartment = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchSubDepartment = Request.Form["columns[4][search][value]"].FirstOrDefault();

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
                var InvData = _dbContext.GRGriegeRequisition.Include(x =>x.SeasonalPlaning).Include(x=>x.Department).Include(x=>x.SubDepartment).Where(x => x.IsDeleted != true && x.TransferToCompany==companyId);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    InvData = InvData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                InvData = !string.IsNullOrEmpty(searchTransNo) ? InvData.Where(m => m.TransactionNo.ToString().ToUpper().Contains(searchTransNo.ToUpper())) : InvData;
                InvData = !string.IsNullOrEmpty(searchSPNo) ? InvData.Where(m => m.SeasonalPlaning.TransactionNo.ToString().ToUpper().Contains(searchSPNo.ToUpper())): InvData;
                InvData = !string.IsNullOrEmpty(searchDate) ? InvData.Where(m => m.TransactionDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchDate.ToUpper())) : InvData;
                InvData = !string.IsNullOrEmpty(searchDepartment) ? InvData.Where(m => m.Department.Name.ToString().ToUpper().Contains(searchDepartment.ToUpper())) : InvData;
                InvData = !string.IsNullOrEmpty(searchSubDepartment) ? InvData.Where(m => m.SubDepartment.Name.ToString().ToUpper().Contains(searchSubDepartment.ToUpper())) : InvData;
 
                ////recordsTotal = InvData.Count();

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

                List<VMGriegeRequisition> viewModel = new List<VMGriegeRequisition>();
                foreach (var item in data)
                {
                    VMGriegeRequisition model = new VMGriegeRequisition();
                    model.GriegeRequisition = item;
                    model.GriegeRequisition.Approve = approve;
                    model.GriegeRequisition.Unapprove = unApprove;
                    model.ListDate = item.TransactionDate.ToString(Helpers.CommonHelper.DateFormat);
                    model.DepartmentName = (_dbContext.GLDivision.Where(x => x.Id == item.DepartmentId).Select(x => x.Name)).FirstOrDefault();
                    model.SubDepartmentName = (_dbContext.GLSubDivision.Where(x => x.Id == item.SubDepartmentId).Select(x => x.Name)).FirstOrDefault();
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
            GRGriegeRequisition model = _dbContext.GRGriegeRequisition.Where(x=> x.Id==id && x.TransferToCompany == _companyId).FirstOrDefault();
            
            model.ApprovedDate = DateTime.UtcNow;
            model.IsApproved = true;
            model.Status = "Approved";
            _dbContext.GRGriegeRequisition.Update(model);
            _dbContext.SaveChanges();
            TempData["error"] = "false";
            TempData["message"] = "Greige Requisition has been approved successfully.";
            return RedirectToAction("Index", "GRRequisition");
        }
        public IActionResult UnApprove(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            GRGriegeRequisition model = _dbContext.GRGriegeRequisition.Where(x => x.Id == id && x.TransferToCompany == _companyId).FirstOrDefault();


           
                
                model.ApprovedDate = DateTime.UtcNow;
                model.IsApproved = false;
                model.Status = "Created";
                _dbContext.GRGriegeRequisition.Update(model);
                _dbContext.SaveChanges();
                TempData["error"] = "false";
                TempData["message"] = "Greige Requisition has been UnApproved successfully.";

          


            return RedirectToAction("Index", "GRRequisition");

            
        }
    }
}