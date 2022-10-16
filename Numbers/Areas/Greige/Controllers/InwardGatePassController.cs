using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Repository.Greige;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
 
namespace Numbers.Areas.Greige.Controllers
{
    [Area("Greige")]
    [Authorize]
    public class InwardGatePassController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public IActionResult Index()
        {
            List<GRInwardGatePass> list = new List<GRInwardGatePass>();
            ViewBag.NavbarHeading = "List of Inward Gate Pass";
            return View(list);
        }
        public InwardGatePassController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Create(int? Id)
        {
            GRInwardGatePassViewModel viewModel = new GRInwardGatePassViewModel();
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var api = new ApiController(_dbContext);
           
            if (Id != 0 && Id != null)
            {
                viewModel.GRInwardGatePass = _dbContext.GRInwardGatePass.Where(x => x.Id == Id && x.CompanyId == companyId).FirstOrDefault();
               
                viewModel.PurchaseContract = viewModel.GRInwardGatePass.PurchaseContractId != 0 ? _dbContext.GRPurchaseContract.Include(x=>x.Vendor).Include(x=>x.PurchaseGRQuality).Include(x=>x.ContractGRQuality).FirstOrDefault(x => x.Id == viewModel.GRInwardGatePass.PurchaseContractId && x.CompanyId==companyId) : null;
                viewModel.WeavingContract = viewModel.GRInwardGatePass.WeavingContractId != 0 ? _dbContext.GRWeavingContracts.Include(x => x.Vendor).Include(x=>x.GreigeQuality).Include(x=>x.GreigeQualityLoom).FirstOrDefault(x => x.Id == viewModel.GRInwardGatePass.WeavingContractId && x.CompanyId == companyId) : null;

                viewModel.GRInwardGatePassDetail = _dbContext.GRInwardGatePassDetails.Where(x => x.GRIGPId == Id).ToArray();

                viewModel.WeavingContractLOV = api.GetWeavingContractById(viewModel.GRInwardGatePass.WeavingContractId, companyId);
                viewModel.PurchaseContractLOV = api.PurchaseContractById(viewModel.GRInwardGatePass.PurchaseContractId, companyId);
                return View(viewModel);
            }
            var repo = new InwardGatePassRepo(_dbContext);
            viewModel.GRInwardGatePass.LotNo = repo.GenerateLotNo(companyId);
            viewModel.WeavingContractLOV = api.GetWeavingContract(companyId);
            viewModel.PurchaseContractLOV = api.PurchaseContractLOV(companyId);
            ViewBag.NavbarHeading = "Greige Inward Gate Pass";
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string configs = _dbContext.AppCompanyConfigs
                   .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                   .Select(c => c.ConfigValue)
                   .FirstOrDefault();
            //ViewBag.ReportPath = string.Concat(configs, "Viewer", "?Report=Voucher&cId=", companyId, "&id={0}");
            //ViewBag.ReportPath2 = string.Concat(configs, "Viewer", "?Report=PurchaseOrder&cId=", companyId, "&id={0}");
            var model = _dbContext.GRInwardGatePass.Where(x => x.Id == id).FirstOrDefault();
            if (model.WeavingContractId != 0 && model.WeavingContractId != null)
            {
                model = _dbContext.GRInwardGatePass.Include(i => i.WeavingContract).ThenInclude(x=>x.Vendor)
                .Where(i => i.Id == id && i.CompanyId == companyId).FirstOrDefault();
            }
            else{

                model = _dbContext.GRInwardGatePass.Include(i => i.PurchaseContract).ThenInclude(x => x.Vendor).
                Where(i => i.Id == id && i.CompanyId == companyId).FirstOrDefault();
            }
            var InwardGatePassdetails = _dbContext.GRInwardGatePassDetails
                                
                                .Where(i => i.GRIGPId == id)
                                .ToList();
            //var tax = purchaseOrderItems.Sum(x => x.TaxAmount);
            //var SubTotal = purchaseOrderItems.Sum(x => x.Total);
            //ViewBag.TaxAmount = tax;
            //ViewBag.SubTotal = SubTotal;
            ViewBag.NavbarHeading = "Details of Inward Gate Pass";
            ViewBag.TitleStatus = "Approved";

            TempData["Detail"] = InwardGatePassdetails;
            return View(model);
        }

        

        //public IActionResult Details(int? Id)
        //{
        //    GRInwardGatePassViewModel viewModel = new GRInwardGatePassViewModel();
        //    int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
        //    var api = new ApiController(_dbContext);

        //    if (Id != 0 && Id != null)
        //    {
        //        viewModel.GRInwardGatePass = _dbContext.GRInwardGatePass.Where(x => x.Id == Id).FirstOrDefault();

        //        viewModel.PurchaseContract = viewModel.GRInwardGatePass.PurchaseContractId != 0 ? _dbContext.GRPurchaseContract.Include(x => x.Vendor).Include(x => x.PurchaseGRQuality).Include(x => x.ContractGRQuality).FirstOrDefault(x => x.Id == viewModel.GRInwardGatePass.PurchaseContractId) : null;
        //        viewModel.WeavingContract = viewModel.GRInwardGatePass.WeavingContractId != 0 ? _dbContext.GRWeavingContracts.Include(x => x.Vendor).Include(x => x.GreigeQuality).Include(x => x.GreigeQualityLoom).FirstOrDefault(x => x.Id == viewModel.GRInwardGatePass.WeavingContractId) : null;

        //        viewModel.GRInwardGatePassDetail = _dbContext.GRInwardGatePassDetails.Where(x => x.GRIGPId == Id).ToArray();

        //        viewModel.WeavingContractLOV = api.GetWeavingContractById(viewModel.GRInwardGatePass.WeavingContractId);
        //        viewModel.PurchaseContractLOV = api.PurchaseContractById(viewModel.GRInwardGatePass.PurchaseContractId);
        //        return View(viewModel);
        //    }

        //    viewModel.WeavingContractLOV = api.GetWeavingContract(companyId);
        //    viewModel.PurchaseContractLOV = api.PurchaseContractLOV(companyId);
        //    ViewBag.NavbarHeading = "Greige Inward Gate Pass";

        //    //TempData["Detail"] = GRInwardGatePassDetail;

        //    return View(viewModel);
        //}
        [HttpGet]
        public IActionResult GetWeavingContract(int Id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            if (Id != 0)
            {
                var address = _dbContext.GRWeavingContracts.Include(x=>x.Vendor).Include(x=>x.GreigeQuality).Include(x=>x.GreigeQualityLoom).FirstOrDefault(x => x.Id == Id && x.CompanyId==companyId);
                if (address != null)
                {
                    return Ok(address);
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
                var address = _dbContext.GRPurchaseContract.Include(x => x.Vendor).Include(x=>x.ContractGRQuality).Include(x=>x.PurchaseGRQuality).FirstOrDefault(x => x.Id == Id && x.CompanyId == companyId);
                if (address != null)
                {
                    return Ok(address);
                }
                return Ok(null);
            }
            return Ok(null);
        }
        [HttpPost]
        public async Task<IActionResult> Create(GRInwardGatePassViewModel model, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var repo = new InwardGatePassRepo(_dbContext);
            GRInwardGatePassDetail[] inwardGatePassDetail = JsonConvert.DeserializeObject<GRInwardGatePassDetail[]>(collection["ItemDetail"]);
            if (model.GRInwardGatePass.Id == 0)
            {
                model.GRInwardGatePass.TransactionNo = repo.Max(companyId);
                model.GRInwardGatePass.CreatedBy = userId;
                model.GRInwardGatePass.CompanyId = companyId;
                model.GRInwardGatePass.Resp_Id = resp_Id;
                model.GRInwardGatePassDetail = inwardGatePassDetail;
                bool isSuccess = await repo.Create(model, collection);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = string.Format("IGP. {0} has been created successfully.", model.GRInwardGatePass.TransactionNo);
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Something went wrong.";
                }
                return RedirectToAction("Create", "InwardGatePass");
            }
            else
            {
                model.GRInwardGatePass.UpdatedBy = userId;
                model.GRInwardGatePass.CompanyId = companyId;
                model.GRInwardGatePass.Resp_Id = resp_Id;
                model.GRInwardGatePassDetail = inwardGatePassDetail;
                bool isSuccess = await repo.Update(model, collection);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = string.Format("IGP. {0} has been updated successfully.", model.GRInwardGatePass.TransactionNo);
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
                var searchWeavingContract = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchLotNo = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchReceived = Request.Form["columns[4][search][value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var Data = (from records in _dbContext.GRInwardGatePass.Where(x=>x.IsDeleted != true && x.CompanyId == companyId) select records);

                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    Data = Data.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                Data = !string.IsNullOrEmpty(searchTransNo) ? Data.Where(m => m.TransactionNo.ToString().ToUpper().Contains(searchTransNo.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchTransDate) ? Data.Where(m => m.TransactionDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchTransDate.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchWeavingContract) ? Data.Where(m => m.WeavingContractId != 0 ? _dbContext.GRWeavingContracts.Include(x=>x.Vendor).Where(x => x.Id == m.WeavingContractId).Select(x=> new ListOfValue { Name = x.TransactionNo + " - " + x.Vendor.Name}).Select(x => x.Name).FirstOrDefault().ToString().ToUpper().Contains(searchWeavingContract.ToUpper()):
                                _dbContext.GRPurchaseContract.Include(x => x.Vendor).Where(x => x.Id == m.PurchaseContractId).Select(x => new ListOfValue { Name = x.ContractNo + " - " + x.Vendor.Name }).Select(x => x.Name).FirstOrDefault().ToString().ToUpper().Contains(searchWeavingContract.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchLotNo) ? Data.Where(m => m.LotNo!=null? m.LotNo.ToString().ToUpper().Contains(searchLotNo.ToUpper()): false) : Data;
                Data = !string.IsNullOrEmpty(searchReceived) ? Data.Where(m => m.TotalRecievedQuantity.ToString().ToUpper().Contains(searchReceived.ToUpper())) : Data;
                
                //recordsTotal = Data.Count();

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

                //var data = Data.Skip(skip).Take(pageSize).ToList();

                List<GRInwardGatePassViewModel> viewModel = new List<GRInwardGatePassViewModel>();
                foreach(var item in data)
                {
                    GRInwardGatePassViewModel model = new GRInwardGatePassViewModel();
                    model.GRInwardGatePass = item;
                    model.GRInwardGatePass.Approve = approve;
                    model.GRInwardGatePass.Unapprove = unApprove;
                    model.Weaver = item.WeavingContractId != 0 ? _dbContext.GRWeavingContracts.Where(x=>x.Id == item.WeavingContractId).Include(x=>x.Vendor).Select(x=> new ListOfValue { Name = x.TransactionNo + " - " + x.Vendor.Name }).FirstOrDefault().Name :
                         _dbContext.GRPurchaseContract.Where(x => x.Id == item.PurchaseContractId).Include(x => x.Vendor).Select(x => new ListOfValue { Name = x.ContractNo + " - " + x.Vendor.Name }).FirstOrDefault().Name;
                    model.Date = item.TransactionDate.ToString(Helpers.CommonHelper.DateFormat);
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
            GRInwardGatePass model = _dbContext.GRInwardGatePass.Find(id);
            model.ApprovedBy = _userId;
            model.ApprovedDate = DateTime.UtcNow;
            model.IsApproved = true;
            model.Status = "Approved";
            _dbContext.GRInwardGatePass.Update(model);
            _dbContext.SaveChanges();
            TempData["error"] = "false";
            TempData["message"] = "IGP has been approved successfully.";
            return RedirectToAction("Index", "InwardGatePass");
        }
        public IActionResult UnApprove(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            GRInwardGatePass model = _dbContext.GRInwardGatePass.Find(id);

            var checkCatgryRfrnc = _dbContext.GRMending.Where(x => x.GRIGPId == id).ToList();
            if (checkCatgryRfrnc.Count == 0)
            {
                model.ApprovedBy = _userId;
                model.ApprovedDate = DateTime.UtcNow;
                model.IsApproved = false;
                model.Status = "Created";
                _dbContext.GRInwardGatePass.Update(model);
                _dbContext.SaveChanges();
                TempData["error"] = "false";
                TempData["message"] = "Weaving Contract has been UnApproved successfully.";

            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Transaction No is Used in Mending..!";
            }
            return RedirectToAction("Index", "InwardGatePass");

        }
        public async Task<IActionResult> Delete(int id)
        {
            var Repo = new InwardGatePassRepo(_dbContext);
            bool isSuccess = await Repo.Delete(id);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Mending has been deleted successfully";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Mending not found";
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
