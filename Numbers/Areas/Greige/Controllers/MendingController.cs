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
using Microsoft.AspNetCore.Mvc.Rendering;
using Numbers.Repository.Helpers;

namespace Numbers.Areas.Greige.Controllers
{
    [Area("Greige")]
    [Authorize]
    public class MendingController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public MendingController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Create(int? Id)
        {
            GRMendingViewModel viewModel = new GRMendingViewModel();
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var api = new ApiController(_dbContext);
            int[] igpIds = _dbContext.GRMending.Where(x => !x.IsDeleted && x.CompanyId==companyId).Select(x => x.GRIGPId).ToArray();
            int igpId = 0;
            ViewBag.NavbarHeading = "Greige Mending";
            if (Id != 0 && Id != null)
            {
                viewModel.GRMending = _dbContext.GRMending.Include(x=>x.GRIGP).Where(x => x.Id == Id && x.CompanyId == companyId).FirstOrDefault();
                viewModel.PurchaseContract = viewModel.GRMending.GRIGP.PurchaseContractId != 0 ? _dbContext.GRPurchaseContract.Include(x=>x.PurchaseGRQuality).Include(x=>x.ContractGRQuality).FirstOrDefault(x => x.Id == viewModel.GRMending.GRIGP.PurchaseContractId) : null;
                viewModel.WeavingContract = viewModel.GRMending.GRIGP.WeavingContractId != 0 ? _dbContext.GRWeavingContracts.Include(x=>x.GreigeQuality).Include(x=>x.GreigeQualityLoom).FirstOrDefault(x => x.Id == viewModel.GRMending.GRIGP.WeavingContractId) : null;
                viewModel.GRMendingDetail = _dbContext.GRMendingDetail.Where(x => x.GRMendingId == viewModel.GRMending.Id).ToArray();
                viewModel.DamageTypeLOV = this.DamageType(companyId);
                igpId = viewModel.GRMending.GRIGP.PurchaseContractId != 0 ?
                     viewModel.GRMending.GRIGP.Id : viewModel.GRMending.GRIGP.Id;

                viewModel.InwardGatePassLOV = api.InwardGatePassById(igpId);
                return View(viewModel);
            }
            viewModel.InwardGatePassLOV = new SelectList(_dbContext.GRInwardGatePass.Where(x=> !igpIds.Contains(x.Id) && x.IsDeleted != true && x.IsApproved && x.CompanyId == companyId).ToList().OrderByDescending(x => x.Id), "Id", "TransactionNo");
            
            return View(viewModel);
        }

        public IActionResult Details(int? Id)
        {
            GRMendingViewModel viewModel = new GRMendingViewModel();
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var api = new ApiController(_dbContext);
            int[] igpIds = _dbContext.GRMending.Where(x => !x.IsDeleted && x.CompanyId == companyId).Select(x => x.GRIGPId).ToArray();
            int igpId = 0;
            ViewBag.NavbarHeading = "Greige Mending";
            if (Id != 0 && Id != null)
            {
                viewModel.GRMending = _dbContext.GRMending.Include(x => x.GRIGP).Where(x => x.Id == Id && x.CompanyId == companyId).FirstOrDefault();
                viewModel.PurchaseContract = viewModel.GRMending.GRIGP.PurchaseContractId != 0 ? _dbContext.GRPurchaseContract.Include(x => x.PurchaseGRQuality).Include(x => x.ContractGRQuality).FirstOrDefault(x => x.Id == viewModel.GRMending.GRIGP.PurchaseContractId && x.CompanyId == companyId) : null;
                viewModel.WeavingContract = viewModel.GRMending.GRIGP.WeavingContractId != 0 ? _dbContext.GRWeavingContracts.Include(x => x.GreigeQuality).Include(x => x.GreigeQualityLoom).FirstOrDefault(x => x.Id == viewModel.GRMending.GRIGP.WeavingContractId && x.CompanyId == companyId) : null;
                viewModel.GRMendingDetail = _dbContext.GRMendingDetail.Where(x => x.GRMendingId == viewModel.GRMending.Id).ToArray();
                viewModel.DamageTypeLOV = this.DamageType(companyId);
                igpId = viewModel.GRMending.GRIGP.PurchaseContractId != 0 ?
                     viewModel.GRMending.GRIGP.Id : viewModel.GRMending.GRIGP.Id;

                viewModel.InwardGatePassLOV = api.InwardGatePassById(igpId);
                return View(viewModel);
            }
            viewModel.InwardGatePassLOV = new SelectList(_dbContext.GRInwardGatePass.Where(x => !igpIds.Contains(x.Id) && x.IsDeleted != true && x.CompanyId == companyId).ToList(), "Id", "TransactionNo");

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult GetInwardGatePass(int Id)
        {
            if (Id != 0)
            {
                int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                var configValues = new ConfigValues(_dbContext);

                GRInwardGatePassViewModel viewModel = new GRInwardGatePassViewModel();
                viewModel.GRInwardGatePass = _dbContext.GRInwardGatePass.FirstOrDefault(x => x.Id == Id && x.CompanyId == companyId);
                if (viewModel.GRInwardGatePass.WeavingContractId != 0)
                {
                    GRWeavingContract Contract = _dbContext.GRWeavingContracts.Include(x=>x.GreigeQuality).Include(x=>x.GreigeQualityLoom).FirstOrDefault(x => x.Id == viewModel.GRInwardGatePass.WeavingContractId && x.CompanyId == companyId);
                    viewModel.ContractNo = Contract.TransactionNo;
                    viewModel.ContractQuality = Contract.GreigeQuality.Description;
                    viewModel.LoomQuality = Contract.GreigeQualityLoom.Description;
                } else if (viewModel.GRInwardGatePass.PurchaseContractId != 0)
                {
                    GRPurchaseContract Contract = _dbContext.GRPurchaseContract.Include(x => x.PurchaseGRQuality).Include(x => x.ContractGRQuality).FirstOrDefault(x => x.Id == viewModel.GRInwardGatePass.PurchaseContractId && x.CompanyId == companyId);
                    viewModel.ContractNo = Contract.ContractNo;
                    viewModel.ContractQuality = Contract.ContractGRQuality.Description;
                    viewModel.LoomQuality = Contract.PurchaseGRQuality.Description;
                }
                viewModel.GRInwardGatePassDetail = _dbContext.GRInwardGatePassDetails.Where(x => x.GRIGPId == viewModel.GRInwardGatePass.Id).OrderBy(x=>x.SrNo).ToArray();
                //viewModel.DamageTypeLOV = configValues.GetOrgValues(resp_Id, "Damage Type", companyId);
                viewModel.DamageTypeLOV = this.DamageType(companyId);
                return Ok(viewModel);
            }
            return Ok(null);
        }
        [HttpPost]
        public async Task<IActionResult> Create(GRMendingViewModel model, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var repo = new MendingRepo(_dbContext);
            GRMendingDetail[] mendingDetail = JsonConvert.DeserializeObject<GRMendingDetail[]>(collection["ItemDetail"]);
            if (model.GRMending.Id == 0)
            {
                model.GRMending.TransactionNo = repo.Max(companyId);
                model.GRMending.CreatedBy = userId;
                model.GRMending.CompanyId = companyId;
                model.GRMending.Resp_Id = resp_Id;
                model.GRMending.TotalRecievedQuantity = Convert.ToDecimal((collection["TotalRecievedQuantity"]));
                model.GRMending.TotalRejectedQuantity = Convert.ToDecimal((collection["TotalRejectedQuantity"]));
                model.GRMending.TotalFreshQuantity = Convert.ToDecimal((collection["TotalFreshQuantity"]));
                model.GRMending.TotalGradedQuantity = Convert.ToDecimal((collection["TotalGradedQuantity"]));
                model.GRMendingDetail = mendingDetail;
                bool isSuccess = await repo.Create(model);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = string.Format("Mending. {0} has been created successfully.", model.GRMending.TransactionNo);
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Something went wrong.";
                }
                return RedirectToAction("Create", "Mending");
            }
            else
            {
                model.GRMending.UpdatedBy = userId;
                model.GRMending.CompanyId = companyId;
                model.GRMending.Resp_Id = resp_Id;
                model.GRMending.TotalRecievedQuantity = Convert.ToDecimal((collection["TotalRecievedQuantity"]));
                model.GRMending.TotalRejectedQuantity = Convert.ToDecimal((collection["TotalRejectedQuantity"]));
                model.GRMending.TotalFreshQuantity = Convert.ToDecimal((collection["TotalFreshQuantity"]));
                model.GRMending.TotalGradedQuantity = Convert.ToDecimal((collection["TotalGradedQuantity"]));
                model.GRMendingDetail = mendingDetail;
                bool isSuccess = await repo.Update(model);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = string.Format("Mending. {0} has been updated successfully.", model.GRMending.TransactionNo);
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
            var configValues = new ConfigValues(_dbContext);
            string configValue = _dbContext.AppCompanyConfigs
                                          .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                                          .Select(c => c.ConfigValue)
                                          .FirstOrDefault();
            ViewBag.ReportPath = string.Concat(configValue, "Viewer", "?Report=OGPMendingBP&cId=", companyId, "&id=");
            ViewBag.NavbarHeading = "List of Mendings";
            return View();
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
                var searchLotNo = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchContractNo = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchReceivedQuantity = Request.Form["columns[4][search][value]"].FirstOrDefault();
                var LotNo = Request.Form["columns[5][search][value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var Data = (from records in _dbContext.GRMending.Include(x=>x.GRIGP).Where(x => x.IsDeleted != true && x.CompanyId == companyId) select records);

                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    Data = Data.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                Data = !string.IsNullOrEmpty(searchTransNo) ? Data.Where(m => m.TransactionNo.ToString().ToUpper().Contains(searchTransNo.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchTransDate) ? Data.Where(m => m.TransactionDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchTransDate.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchLotNo) ? Data.Where(m => m.GRIGP.TransactionNo.ToString().ToUpper().Contains(searchLotNo.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchContractNo) ? Data.Where(m => m.GRIGP.PurchaseContractId != 0 ? 
                                _dbContext.GRPurchaseContract.FirstOrDefault(x=>x.Id == m.GRIGP.PurchaseContractId).ContractNo.ToString().ToUpper().Contains(searchContractNo.ToUpper()) :
                                _dbContext.GRWeavingContracts.FirstOrDefault(x=>x.Id == m.GRIGP.WeavingContractId).TransactionNo.ToString().ToUpper().Contains(searchContractNo.ToUpper())) : Data;
                
                Data = !string.IsNullOrEmpty(searchReceivedQuantity) ? Data.Where(m => m.GRIGP.TotalActualQuantity.ToString().ToUpper().Contains(searchReceivedQuantity.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(LotNo) ? Data.Where(m => m.GRIGP.LotNo != null ? m.GRIGP.LotNo.ToString().ToUpper().Contains(LotNo.ToUpper()): false) : Data;

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

                List<GRMendingViewModel> viewModel = new List<GRMendingViewModel>();
                foreach (var item in data)
                {
                    GRMendingViewModel model = new GRMendingViewModel();
                    model.GRMending = item;
                    model.ContractNo = item.GRIGP.WeavingContractId != 0 ? _dbContext.GRWeavingContracts.FirstOrDefault(x => x.Id == item.GRIGP.WeavingContractId).TransactionNo.ToString() :
                                _dbContext.GRPurchaseContract.FirstOrDefault(x => x.Id == item.GRIGP.PurchaseContractId).ContractNo.ToString();
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
            GRMending model = _dbContext.GRMending.Where(x=>x.Id==id && x.CompanyId == _companyId).FirstOrDefault();
            model.ApprovedBy = _userId;
            model.ApprovedDate = DateTime.UtcNow;
            model.IsApproved = true;
            model.Status = "Approved";
            _dbContext.GRMending.Update(model);
            _dbContext.SaveChanges();
            TempData["error"] = "false";
            TempData["message"] = "Mending has been approved successfully.";
            return RedirectToAction("Index", "Mending");
        }
        public IActionResult UnApprove(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            GRMending model = _dbContext.GRMending.Where(x => x.Id == id && x.CompanyId == _companyId).FirstOrDefault();
            try
            {

            
            var checkCatgryRfrnc = _dbContext.GRFolding.Where(x => x.MendingId == id && x.IsDeleted != true && x.CompanyId==_companyId).ToList();
                if (checkCatgryRfrnc.Count == 0)
                {
                model.ApprovedBy = _userId;
                model.ApprovedDate = DateTime.UtcNow;
                model.IsApproved = false;
                model.Status = "Created";
                _dbContext.GRMending.Update(model);
                _dbContext.SaveChanges();
                TempData["error"] = "false";
                TempData["message"] = "Mending has been UnApproved successfully.";

            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Transaction No is Used in Folding..!";
            }
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "Mending");
            }


            return RedirectToAction("Index", "Mending");

           
        }


        public async Task<IActionResult> Delete(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var Repo = new MendingRepo(_dbContext);
            bool isSuccess = await Repo.Delete(id,_companyId);
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
        public SelectList DamageType(int companyId)
        {
            var DamageType = new SelectList((from b in _dbContext.AppCompanyConfigBases.Where(x => x.Name == "Damage Type")
                            join
            c in _dbContext.AppCompanyConfigs on b.Id equals c.BaseId
                            where  b.IsActive && b.IsDeleted == false
                            select c
                                  ).ToList(), "Id", "ConfigValue");
            return DamageType;
        }
    }
}
