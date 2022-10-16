using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Repository.Greige;
using Numbers.Repository.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;

namespace Numbers.Areas.Greige.Controllers
{
    [Area("Greige")]
    [Authorize]
    public class GRNController : Controller
    {
        private readonly NumbersDbContext _dbContext;

        public GRNController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var ContractRepo = new GRNRepo(_dbContext);
            string configs = _dbContext.AppCompanyConfigs
                .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                .Select(c => c.ConfigValue)
                .FirstOrDefault();
            ViewBag.ReportPath = string.Concat(configs, "Viewer", "?Report=Voucher&cId=", companyId, "&id=");
            ViewBag.ReportPath2 = string.Concat(configs, "Viewer", "?Report=GRNBP&cId=", companyId, "&id=");
            IEnumerable<GRGRN> items = ContractRepo.GetAll(companyId);
            ViewBag.NavbarHeading = "List of GRN";
            return View(items);

        }

        public IActionResult Create(int? Id)
        {
            GRGRNViewModel viewModel = new GRGRNViewModel();
            try
            {
              
                var configValues = new ConfigValues(_dbContext);
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                var api = new ApiController(_dbContext);
                //ViewBag.FeederLov = api.GetFolding(companyId);
              
                viewModel.WeavingContractLOV = api.GetWeavingContract(companyId);
                viewModel.PurchaseContractLOV = api.PurchaseContractLOV(companyId);
                viewModel.WareHouse = configValues.GetConfigValues("Inventory", "Ware House", "GD Greige", companyId);
                viewModel.Location = configValues.GetConfigValues("Inventory", "Location", "GD Greige", companyId);
                viewModel.Penalty = configValues.GetConfigValues("Greige", "Penalty", companyId);
                if (Id != 0 && Id != null)
                {
                    
                    viewModel.GRGRN = _dbContext.GRGRNS.Where(x => x.Id == Id && x.CompanyId==companyId).FirstOrDefault();
                    viewModel.GRGRNItem = _dbContext.GRGRNItems.Include(x => x.Penalty).Where(x => x.GRGRNId == Id).ToArray();
                    viewModel.GRStackingItem = _dbContext.GRStackingItems.Include(x => x.WareHouse).Include(x => x.Location).Where(x => x.GRGRNId == Id).ToArray();
                    viewModel.FeederLov = api.GetFoldingById(viewModel.GRGRN.FoldingId, companyId);
                }
                else if(Id==0 || Id==null)
                {
                    int[] foldings = _dbContext.GRGRNS.Where(x => !x.IsDeleted).Select(x => x.FoldingId).ToArray();
                    viewModel.FeederLov = new SelectList(_dbContext.GRFolding.Where(x => !foldings.Contains(x.Id) && x.IsApproved && x.IsDeleted != true && x.CompanyId==companyId).ToList().OrderByDescending(x => x.Id), "Id", "FoldingNo"); ;

                }
                ViewBag.NavbarHeading = "Greige Good Receipt Note";
            }
            catch(Exception e)
            {

            }
            return View(viewModel);
        }

        public IActionResult Details(int? Id)
        {
            GRGRNViewModel viewModel = new GRGRNViewModel();
            try
            {

                var configValues = new ConfigValues(_dbContext);
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                var api = new ApiController(_dbContext);
                ViewBag.FeederLov = api.GetFolding(companyId);
                viewModel.FeederLov = api.GetFolding(companyId);
                viewModel.WeavingContractLOV = api.GetWeavingContract(companyId);
                viewModel.PurchaseContractLOV = api.PurchaseContractLOV(companyId);
                viewModel.WareHouse = configValues.GetConfigValues("Inventory", "Ware House", "GD Greige", companyId);
                viewModel.Location = configValues.GetConfigValues("Inventory", "Location", "GD Greige", companyId);
                viewModel.Penalty = configValues.GetConfigValues("Greige", "Penalty", companyId);
                if (Id != 0 && Id != null)
                {
                    viewModel.GRGRN = _dbContext.GRGRNS.Where(x => x.Id == Id && x.CompanyId==companyId).FirstOrDefault();
                    viewModel.GRGRNItem = _dbContext.GRGRNItems.Include(x => x.Penalty).Where(x => x.GRGRNId == Id).ToArray();
                    viewModel.GRStackingItem = _dbContext.GRStackingItems.Include(x => x.WareHouse).Include(x => x.Location).Where(x => x.GRGRNId == Id).ToArray();
                }
                ViewBag.NavbarHeading = "Greige Good Receipt Note";
            }
            catch (Exception e)
            {

            }
            return View(viewModel);
        }


        [HttpGet]
        public IActionResult GetWeavingContract(int Id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            if (Id != 0)
            {
                var address = _dbContext.GRWeavingContracts.Include(x=>x.Vendor).Include(x=>x.GreigeQuality).Include(x=>x.GreigeQualityLoom).ThenInclude(x=>x.GRConstruction).FirstOrDefault(x => x.Id == Id && x.CompanyId==companyId);
                if (address != null)
                {
                    return Ok(address);
                }
                return Ok(null);
            }
            return Ok(null);
        } 
        
        [HttpGet]
        public IActionResult GetFolding(int Id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            if (Id != 0)
            {
                var address = _dbContext.GRFolding.Include(x=>x.Mending).FirstOrDefault(x => x.Id == Id && x.CompanyId == companyId);
                var foldingdata = (from f in _dbContext.GRFolding.Where(x => x.Id == Id && x.CompanyId == companyId)
                                   join fd in _dbContext.GRFoldingItems.ToList() on f.Id equals fd.FoldingId
                                   join me in _dbContext.GRMending.Where(x=> x.CompanyId == companyId).ToList() on f.MendingId equals me.Id
                                   join igp in _dbContext.GRInwardGatePass.Where(x => x.CompanyId == companyId).ToList() on me.GRIGPId equals igp.Id
                                   select new { f, fd , me,igp } ).Distinct().GroupBy(x => x.fd.FoldingId).Select(x => new {
                                       FoldQty = x.Sum(x => x.fd.FoldQty),
                                       FoldPiece = x.Max(x => x.fd.SrNo),
                                       PurchaseContractId = x.Max(x=>x.igp.PurchaseContractId),
                                       WeavingContractId = x.Max(x=>x.igp.WeavingContractId)
                                   }).ToList();
                                   
                var data = (from f in _dbContext.GRFolding.Where(x => x.Id == Id && x.CompanyId == companyId)
                                   join m in _dbContext.GRMending.Where(x=> x.CompanyId == companyId) on f.MendingId equals m.Id
                                   join md in _dbContext.GRMendingDetail.ToList() on m.Id equals md.GRMendingId
                                   join mIGP in _dbContext.GRInwardGatePass.Where(x=> x.CompanyId == companyId) on m.GRIGPId equals mIGP.Id
                                   join dIGP in _dbContext.GRInwardGatePassDetails.ToList() on mIGP.Id equals dIGP.GRIGPId
                                   select new {
                                  
                                  md.GRMendingId,
                                  
                                  m.TotalRejectedQuantity,
                                  m.TotalFreshQuantity,

                                       md.MendingQuantity,
                                   md.ReceivedQuantity,
                                   md.FreshQuantity,
                                   md.RejectedQuantity,
                                  Mendingpieces = md.SrNo,
                                  WovenPieces = dIGP.SrNo,
                                   WovenQty = dIGP.ReceivedQuantity
                                   }).Distinct().GroupBy(x => x.GRMendingId).Select(x => new
                                   {
                                       MendingQuantity = x.Max(x => x.TotalFreshQuantity),
                                       //MendingQuantity = x.Sum(x => x.MendingQuantity),
                                       MendingPieces = x.Max(x => x.Mendingpieces),
                                       ReceivedQuantity = x.Sum(x => x.ReceivedQuantity),
                                       FreshQuantity = x.Sum(x => x.FreshQuantity),
                                       RejectedQuantity = x.Sum(x => x.RejectedQuantity),
                                       TotalRejectedQuantity = x.Max(x => x.TotalRejectedQuantity),
                                       FoldQty =foldingdata.Sum(x => x.FoldQty),
                                       FoldPieces = foldingdata.Sum(x => x.FoldPiece),
                                       WovenPieces = x.Max(x => x.WovenPieces),
                                       WovenQty = x.Sum(x => x.WovenQty)/ x.Max(x => x.WovenPieces),
                                       PurchaseContractId = foldingdata.Max(x => x.PurchaseContractId),
                                       WeavingContractId = foldingdata.Max(x => x.WeavingContractId)

                                   }).FirstOrDefault();
                var api = new ApiController(_dbContext);
                var PurchaseContract = api.PurchaseContractById(data.PurchaseContractId, companyId);
                var WeavingContract = api.GetWeavingContractById(data.WeavingContractId, companyId);

                if (data != null)
                {
                    return Ok(new { data = data, PurchaseContract = PurchaseContract , WeavingContract = WeavingContract });
                }
                return Ok(null);
            }
            return Ok(null);
        }
        [HttpGet]
        public IActionResult GetPurchaseContract(int Id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            if (Id != 0)
            {
                var address = _dbContext.GRPurchaseContract.Include(x => x.Vendor).Include(x=>x.ContractGRQuality).ThenInclude(x=>x.GRConstruction).Include(x=>x.PurchaseGRQuality).FirstOrDefault(x => x.Id == Id && x.CompanyId==companyId);
                if (address != null)
                {
                    return Ok(address);
                }
                return Ok(null);
            }
            return Ok(null);
        }
        [HttpPost]
        public async Task<IActionResult> Create(GRGRNViewModel model, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var repo = new GRNRepo(_dbContext);
            GRGRNItem[] items = JsonConvert.DeserializeObject<GRGRNItem[]>(collection["ItemDetail"]);
            GRStackingItem[] Dataitems = JsonConvert.DeserializeObject<GRStackingItem[]>(collection["itemStack"]);
            if (model.GRGRN.Id == 0)
            {
                model.GRGRN.TransactionNo = repo.Max(companyId);
                model.GRGRN.CreatedBy = userId;
                model.GRGRN.CompanyId = companyId;
                model.GRGRN.Resp_Id = resp_Id;
                model.GRGRNItem = items;
                model.GRStackingItem = Dataitems;
                bool isSuccess = await repo.Create(model);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = string.Format(" Greige GRN. {0} has been created successfully.", model.GRGRN.TransactionNo);
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Something went wrong.";
                }
                return RedirectToAction("Create", "GRN");
            }
            else
            {
                model.GRGRN.UpdatedBy = userId;
                model.GRGRN.CompanyId = companyId;
                model.GRGRN.Resp_Id = resp_Id;
                model.GRGRNItem = items;
                model.GRStackingItem = Dataitems;
                bool isSuccess = await repo.Update(model);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = string.Format("Greige GRN. {0} has been updated successfully.", model.GRGRN.TransactionNo);
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Something went wrong.";
                }
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
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

                var searchGRNNo = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchGRNDate = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchFolding = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchWeavingContract = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchPurchasecontract = Request.Form["columns[4][search][value]"].FirstOrDefault();
                var searchVendor = Request.Form["columns[5][search][value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var weavingContact = _dbContext.GRWeavingContracts.ToList();
                var purchaseContact = _dbContext.GRPurchaseContract.ToList();
                var InvData = (from m in _dbContext.GRGRNS.Include(x=>x.Folding) where m.IsDeleted != true && m.CompanyId==companyId
                               select m);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    InvData = InvData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                InvData = !string.IsNullOrEmpty(searchGRNNo) ? InvData.Where(m => m.TransactionNo.ToString().ToUpper().Contains(searchGRNNo.ToUpper())) : InvData;
                InvData = !string.IsNullOrEmpty(searchGRNDate) ? InvData.Where(m => m.TransactionDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchGRNDate.ToUpper())) : InvData;
                InvData = !string.IsNullOrEmpty(searchFolding) ? InvData.Where(m => m.Folding.FoldingNo.ToString().ToUpper().Contains(searchFolding.ToUpper())) : InvData;
                InvData = !string.IsNullOrEmpty(searchWeavingContract) ? InvData.Where(m => m.WeavingContractId.ToString().ToUpper().Contains(searchWeavingContract.ToUpper())) : InvData;
                InvData = !string.IsNullOrEmpty(searchPurchasecontract) ? InvData.Where(m => m.PurchaseContractId.ToString().ToUpper().Contains(searchPurchasecontract.ToUpper())) : InvData;
                InvData = !string.IsNullOrEmpty(searchVendor) ? InvData.Where(m => m.VendorName.ToString().ToUpper().Contains(searchVendor.ToUpper())) : InvData;

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
                var listData = new List<GRGRNViewModel>();
                foreach(var item in data)
                {

                    var model = new GRGRNViewModel();
                    model.Id = item.Id;
                    model.GRGRN = item;
                    model.GRGRN.Approve = approve;
                    model.GRGRN.Unapprove = unApprove;
                    model.TransactionNo = item.TransactionNo;
                    model.TransactionDate = item.TransactionDate.ToString("dd-MMM-yyyy");
                    model.WeavingContactNo =_dbContext.GRWeavingContracts.Where(x=>x.Id==item.WeavingContractId).Select(x=>x.TransactionNo).FirstOrDefault();
                    model.PurcahseContracNo = _dbContext.GRPurchaseContract.Where(x => x.Id == item.PurchaseContractId).Select(x => x.ContractNo).FirstOrDefault();
                    model.VendorName = _dbContext.APSuppliers.Where(x => x.Id == item.VendorId).Select(x => x.Name).FirstOrDefault();
                    model.FoldingNo = _dbContext.GRFolding.Where(x => x.Id == item.FoldingId).Select(x => x.FoldingNo).FirstOrDefault(); 

                    listData.Add(model);
                }

                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = listData };
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
            var grnRepo = new GRNRepo(_dbContext, HttpContext);
            bool isSuccess = await grnRepo.Approve(id, userId, companyId);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Greige GRN has been approved successfully";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong.";
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Approve2(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            GRGRN model = _dbContext.GRGRNS.Find(id);
            model.ApprovedBy = _userId;
            model.ApprovedDate = DateTime.UtcNow;
            model.IsApproved = true;
            model.Status = "Approved";
            _dbContext.GRGRNS.Update(model);
            _dbContext.SaveChanges();
            TempData["error"] = "false";
            TempData["message"] = "Greige GRN has been approved successfully.";
            return RedirectToAction("Index", "GRN");
        }
        public async Task<IActionResult> UnApprove(int id)
        {
            //int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            //string _userId = HttpContext.Session.GetString("UserId");
            //GRGRN model = _dbContext.GRGRNS.Find(id);

            //model.ApprovedBy = _userId;
            //model.ApprovedDate = DateTime.UtcNow;
            //model.IsApproved = false;
            //model.Status = "Created";
            //_dbContext.GRGRNS.Update(model);
            //_dbContext.SaveChanges();
            //TempData["error"] = "false";
            //TempData["message"] = "GRN has been UnApproved successfully.";
            //return RedirectToAction("Index", "GRN");

            try
            {
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                string userId = HttpContext.Session.GetString("UserId");
                var result = await new GRNRepo(_dbContext, HttpContext).UnApprove(id);
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


    }
}
