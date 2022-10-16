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
    public class StackingController : Controller
    {
        private readonly NumbersDbContext _dbContext;

        public StackingController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var ContractRepo = new StackingRepo(_dbContext);
            IEnumerable<GRStacking> items = ContractRepo.GetAll(companyId);
            ViewBag.NavbarHeading = "List of Stacking";
            return View(items);

        }

        public IActionResult Create(int? Id)
        {
            GRStackingViewModel viewModel = new GRStackingViewModel();
            try
            {
              
                var configValues = new ConfigValues(_dbContext);
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                var api = new ApiController(_dbContext);
                viewModel.GRNLov = api.GetGRN(companyId);
                viewModel.VendorLov = api.GetFolding(companyId);
                viewModel.WeavingContractLOV = api.GetWeavingContract(companyId);
                viewModel.PurchaseContractLOV = api.PurchaseContractLOV(companyId);
                viewModel.WareHouse = configValues.GetConfigValues("Inventory", "Ware House", companyId);
                viewModel.Location = configValues.GetConfigValues("Greige", "Location", companyId);
                if (Id != 0 && Id != null)
                {
                    viewModel.GRStacking = _dbContext.GRStackings.Where(x => x.Id == Id && x.CompanyId==companyId).FirstOrDefault();
                    viewModel.GRStackingItem = _dbContext.GRStackingItems.Include(x => x.WareHouseId).Where(x => x.GRGRNId == viewModel.GRStacking.Id).ToArray();
                }
                ViewBag.NavbarHeading = "Greige Stacking";
            }
            catch(Exception e)
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
                var address = _dbContext.GRFolding.Include(x=>x.Mending).FirstOrDefault(x => x.Id == Id && x.CompanyId==companyId);
                var foldingdata = (from f in _dbContext.GRFolding.Where(x => x.Id == Id && x.CompanyId == companyId)
                                   join fd in _dbContext.GRFoldingItems.ToList() on f.Id equals fd.FoldingId
                                   select fd).Distinct().GroupBy(x => x.FoldingId).Select(x => x.Sum(x=>x.FoldQty)
                                   );
                var data = (from f in _dbContext.GRFolding.Where(x => x.Id == Id && x.CompanyId == companyId)
                                   join m in _dbContext.GRMending.Where(x=>x.CompanyId==companyId) on f.MendingId equals m.Id
                                   join md in _dbContext.GRMendingDetail.ToList() on m.Id equals md.GRMendingId
                                   select new {
                                   md.GRMendingId,
                                   md.MendingQuantity,
                                   md.ReceivedQuantity,
                                   md.FreshQuantity,
                                   md.RejectedQuantity,
                                   }).Distinct().GroupBy(x => x.GRMendingId).Select(x => new
                                   {
                                       MendingQuantity = x.Sum(x => x.MendingQuantity),
                                       ReceivedQuantity = x.Sum(x => x.ReceivedQuantity),
                                       FreshQuantity = x.Sum(x => x.FreshQuantity),
                                       RejectedQuantity = x.Sum(x => x.RejectedQuantity),
                                       FoldQty =foldingdata.FirstOrDefault(),

                                   }).FirstOrDefault();

                 

                if (data != null)
                {
                    return Ok(data);
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
        public async Task<IActionResult> Create(GRStackingViewModel model, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var repo = new StackingRepo(_dbContext);
            GRStackingItem[] items = JsonConvert.DeserializeObject<GRStackingItem[]>(collection["ItemDetail"]);
            if (model.GRStacking.Id == 0)
            {
                model.GRStacking.TransactionNo = repo.Max(companyId);
                model.GRStacking.CreatedBy = userId;
                model.GRStacking.CompanyId = companyId;
                model.GRStacking.Resp_Id = resp_Id;
                model.GRStackingItem = items;
                bool isSuccess = await repo.Create(model);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = string.Format(" Greige Stacking. {0} has been created successfully.", model.GRStacking.TransactionNo);
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Something went wrong.";
                }
                return RedirectToAction("Create", "Stacking");
            }
            else
            {
                model.GRStacking.UpdatedBy = userId;
                model.GRStacking.CompanyId = companyId;
                model.GRStacking.Resp_Id = resp_Id;
                model.GRStackingItem = items;
                bool isSuccess = await repo.Update(model);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = string.Format("Greige Stacking. {0} has been updated successfully.", model.GRStacking.TransactionNo);
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
                var weavingContact = _dbContext.GRWeavingContracts.ToList();
                var purchaseContact = _dbContext.GRPurchaseContract.ToList();
                var InvData = (from m in _dbContext.GRStackings where m.IsDeleted != true && m.CompanyId==companyId
                               select m);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    InvData = InvData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                InvData = !string.IsNullOrEmpty(searchItemCode) ? InvData.Where(m => m.TransactionNo.ToString().ToUpper().Contains(searchItemCode.ToUpper())) : InvData;
                InvData = !string.IsNullOrEmpty(searchItemName) ? InvData.Where(m => m.TransactionDate.ToString().ToUpper().Contains(searchItemName.ToUpper())) : InvData;
                InvData = !string.IsNullOrEmpty(searchBarcode) ? InvData.Where(m => m.WeavingContractId.ToString().ToUpper().Contains(searchBarcode.ToUpper())) : InvData;
                InvData = !string.IsNullOrEmpty(searchUOM) ? InvData.Where(m => m.PurchaseContractId.ToString().ToUpper().Contains(searchUOM.ToUpper())) : InvData;
                InvData = !string.IsNullOrEmpty(searchUOM) ? InvData.Where(m => m.VendorName.ToString().ToUpper().Contains(searchUOM.ToUpper())) : InvData;
                InvData = !string.IsNullOrEmpty(searchUOM) ? InvData.Where(m => m.GRNId.ToString().ToUpper().Contains(searchUOM.ToUpper())) : InvData;

                recordsTotal = InvData.Count();
                var data = InvData.Skip(skip).Take(pageSize).ToList();
                var listData = new List<GRStackingViewModel>();
                foreach(var item in data)
                {

                    var model = new GRStackingViewModel();
                    model.Id = item.Id;
                    model.TransactionNo = item.TransactionNo;
                    model.TransactionDate = item.TransactionDate.ToString("dd-MMM-yyyy");
                    model.WeavingContactNo =_dbContext.GRWeavingContracts.Where(x=>x.Id==item.WeavingContractId).Select(x=>x.TransactionNo).FirstOrDefault();
                    model.PurcahseContracNo = _dbContext.GRPurchaseContract.Where(x => x.Id == item.PurchaseContractId).Select(x => x.ContractNo).FirstOrDefault();
                    model.VendorName = _dbContext.APSuppliers.Where(x => x.Id == item.VendorId).Select(x => x.Name).FirstOrDefault();
                    model.GRNNo = _dbContext.GRFolding.Where(x => x.Id == item.GRNId).Select(x => x.FoldingNo).FirstOrDefault(); 

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

    }
}
