using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
    public class PurchaseContractController : Controller
    {
        private readonly NumbersDbContext _dbContext;

        public PurchaseContractController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Create(int? Id)
        {
            GRPurchaseContractViewModel viewModel = new GRPurchaseContractViewModel();
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var configValues = new ConfigValues(_dbContext);
            viewModel.VendorLOV = configValues.GreigeVendor(companyId);
            //viewModel.GRWeavingContractsLOV = new SelectList(_dbContext.GRWeavingContracts.Where(x => x.IsDeleted != true).ToList(), "Id", "TransactionNo");
            //viewModel.GRWeavingContractsLOV = new SelectList(_dbContext.GRQuality.Where(x => x.IsDeleted != true).ToList(), "Id", "Description");
            viewModel.SalesTaxLOV = new SelectList(_dbContext.AppTaxes.Include(t => t.SalesTaxAccount).Include(t => t.ExciseTaxAccount).Include(t => t.IncomeTaxAccount).Where(t => t.CompanyId == companyId && t.IsDeleted == false).OrderByDescending(x=>x.Name.Contains("GST 17%")).ToList(), "Id", "Name");
            ViewBag.NavbarHeading = "Greige Purchase Contract";
           
            if (Id != 0 && Id != null)
            {
                viewModel.GRPurchaseContract = _dbContext.GRPurchaseContract.Where(x => x.Id == Id).FirstOrDefault();
                //viewModel.GRQualityLOV = new SelectList(_dbContext.GRQuality.Where(x => x.IsDeleted != true).ToList(), "Id", "Description");

                var GRGriegeRequisition = new SelectList(_dbContext.GRGriegeRequisition.Where(x => x.IsDeleted == false && x.CompanyId == companyId && x.Id == viewModel.GRPurchaseContract.GRRequisitionId).OrderByDescending(x => x.TransactionNo).ToList()
                                                    , "Id", "TransactionNo");
                ViewBag.GRRequisitionNo = GRGriegeRequisition;
                var x = _dbContext.GRGriegeRequisitionDetails.FirstOrDefault(x => x.GRRequisitionId == viewModel.GRPurchaseContract.GRRequisitionId).BalanceQty;
                ViewBag.Quantity = x;
                ViewBag.hQuantity = x + viewModel.GRPurchaseContract.ContractQuantity;
                viewModel.GRQualityLOV = new SelectList(from det in _dbContext.GRGriegeRequisitionDetails
                                                        join
                                                          qual in _dbContext.GRQuality.Where(x => x.IsDeleted == false).OrderBy(x => x.TransactionNo).ToList()
                                                          on det.GriegeQualityId equals qual.Id
                                                        where (qual.Id == viewModel.GRPurchaseContract.PurchaseGRQualityId && det.GRRequisitionId.ToString() == GRGriegeRequisition.Select(x => x.Value).FirstOrDefault()) || (!det.IsUsed && det.GRRequisitionId.ToString() == GRGriegeRequisition.Select(x => x.Value).FirstOrDefault())
                                                        select qual
                                                        , "Id", "Description");
                viewModel.ContractQualityLOV = new SelectList(from det in _dbContext.GRGriegeRequisitionDetails
                                                        join
                                                          qual in _dbContext.GRQuality.Where(x => x.IsDeleted == false).OrderBy(x => x.TransactionNo).ToList()
                                                          on det.GriegeQualityId equals qual.Id
                                                        where (qual.Id == viewModel.GRPurchaseContract.ContractGRQualityId && det.GRRequisitionId.ToString() == GRGriegeRequisition.Select(x => x.Value).FirstOrDefault()) || (!det.IsUsed && det.GRRequisitionId.ToString() == GRGriegeRequisition.Select(x => x.Value).FirstOrDefault())
                                                        select qual
                                                       , "Id", "Description");
                return View(viewModel);
            }
            var data = from master in _dbContext.GRGriegeRequisition.Where(x => x.IsApproved && x.IsDeleted == false && x.TransferToCompany == companyId).OrderByDescending(x => x.TransactionNo).ToList()
                       join
                        detail in _dbContext.GRGriegeRequisitionDetails.Where(x => x.BalanceQty > 0) on
                        master.Id equals detail.GRRequisitionId
                       select master;
            var transactionNo = data.GroupBy(x => x.Id).Select(x => new ListOfValue
            {
                Id = x.Select(a => a.Id).FirstOrDefault(),
                TransactionNo = x.Select(a => a.TransactionNo).FirstOrDefault()
            }).ToList();
            ViewBag.GRRequisitionNo = new SelectList(transactionNo, "Id", "TransactionNo");
            return View(viewModel);
        }

        public IActionResult Details(int? Id)
        {
            GRPurchaseContractViewModel viewModel = new GRPurchaseContractViewModel();
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var configValues = new ConfigValues(_dbContext);
            viewModel.VendorLOV = configValues.GreigeVendor(companyId);
            viewModel.GRQualityLOV = new SelectList(_dbContext.GRQuality.Where(x => x.IsDeleted != true ).ToList(), "Id", "Description");
            //viewModel.GRWeavingContractsLOV = new SelectList(_dbContext.GRWeavingContracts.Where(x => x.IsDeleted != true).ToList(), "Id", "TransactionNo");
            viewModel.GRWeavingContractsLOV = new SelectList(_dbContext.GRQuality.Where(x => x.IsDeleted != true ).ToList(), "Id", "Description");
            viewModel.SalesTaxLOV = new SelectList(_dbContext.AppTaxes.Include(t => t.SalesTaxAccount).Include(t => t.ExciseTaxAccount).Include(t => t.IncomeTaxAccount).Where(t => t.CompanyId == companyId && t.IsDeleted == false).OrderByDescending(x => x.Name.Contains("GST 17%")).ToList(), "Id", "Name");
            ViewBag.NavbarHeading = "Greige Purchase Contract";
            if (Id != 0 && Id != null)
            {
                viewModel.GRPurchaseContract = _dbContext.GRPurchaseContract.Where(x => x.Id == Id  && x.CompanyId == companyId).FirstOrDefault();
                var x = _dbContext.GRGriegeRequisitionDetails.FirstOrDefault(x => x.GRRequisitionId == viewModel.GRPurchaseContract.GRRequisitionId).BalanceQty;
                ViewBag.Quantity = x;
            }
            return View(viewModel);
        }


        [HttpGet]
        public IActionResult GetContractQuality(int Id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            if (Id != 0)
            {
                var GRWeavingContracts = _dbContext.GRWeavingContracts.FirstOrDefault(x => x.Id == Id && x.CompanyId==companyId);
                if (GRWeavingContracts != null)
                {
                    return Ok(GRWeavingContracts);
                }
                return Ok(null);
            }
            return Ok(null);
        }
        [HttpPost]
        public async Task<IActionResult> Create(GRPurchaseContractViewModel viewModel, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var ContractRepo = new PurchaseContractRepo(_dbContext);

            if (viewModel.GRPurchaseContract.Id == 0)
            {
                int TransactionNo = 1;
                var list = _dbContext.GRPurchaseContract.ToList();
                if (list.Count != 0)
                {
                    TransactionNo = list.Select(x => x.ContractNo).Max() + 1;
                }
                viewModel.GRPurchaseContract.ContractNo = TransactionNo;
                viewModel.GRPurchaseContract.CompanyId = companyId;
                viewModel.GRPurchaseContract.CreatedBy = userId;
                
                viewModel.GRPurchaseContract.SalesTaxId = Convert.ToInt32(collection["SalesTaxId"]);
                bool isSuccess = await ContractRepo.Create(viewModel.GRPurchaseContract);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = string.Format("Purchase Contract has been saved successfully");
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Something went wrong.";
                }
                return RedirectToAction(nameof(Index));

            }
            else
            {
                viewModel.GRPurchaseContract.CompanyId = companyId;
                viewModel.GRPurchaseContract.UpdatedBy = userId;
                bool isSuccess = await ContractRepo.Update(viewModel.GRPurchaseContract);

                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = string.Format("Purchase Contract has been saved successfully");
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
            ViewBag.NavbarHeading = "List of Purchase Contract";
            return View();
        }
        public IActionResult GetList()
        {
            try
            {
                int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
                string userId = HttpContext.Session.GetString("UserId");
                var approve = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == resp_Id ).FirstOrDefault().Approve;
                var unApprove = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == resp_Id).FirstOrDefault().UnApprove;

                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                var configValues = new ConfigValues(_dbContext);
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                //var searchValue = Request.Form["search[value]"].FirstOrDefault();

                var searchContractNo = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchContractDate = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchVendor = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchQuality = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchRate = Request.Form["columns[4][search][value]"].FirstOrDefault();
                var searchAmount = Request.Form["columns[5][search][value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var Data = (from Contract in _dbContext.GRPurchaseContract.Include(x => x.Vendor).Include(x => x.PurchaseGRQuality).Where(x => x.IsDeleted == false && x.CompanyId == companyId) select Contract);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    Data = Data.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                Data = !string.IsNullOrEmpty(searchContractNo) ? Data.Where(m => m.ContractNo.ToString().ToUpper().Contains(searchContractNo.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchContractDate) ? Data.Where(m => m.ContractDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchContractDate.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchVendor) ? Data.Where(m => m.Vendor.Name.ToString().ToUpper().Contains(searchVendor.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchQuality) ? Data.Where(m => m.PurchaseGRQuality.Description.ToString().ToUpper().Contains(searchQuality.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchRate) ? Data.Where(m => m.RatePerMeter.ToString().ToUpper().Contains(searchRate.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchAmount) ? Data.Where(m => m.InSalesTaxAmount.ToString().ToUpper().Contains(searchAmount.ToUpper())) : Data;

               // recordsTotal = Data.Count();

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
                List<GRPurchaseContract> details = new List<GRPurchaseContract>();
                foreach (var item in data)
                {
                    var purchaseContract = new GRPurchaseContract();
                    purchaseContract.UpdatedBy = item.ContractDate.ToString(Helpers.CommonHelper.DateFormat);
                    purchaseContract.purchaseContract = item;
                    purchaseContract.purchaseContract.Approve = approve;
                    purchaseContract.purchaseContract.Unapprove = unApprove;
                    details.Add(purchaseContract);
                    //item.UpdatedBy = item.ContractDate.ToString(Helpers.CommonHelper.DateFormat);
                }
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = details };
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
            GRPurchaseContract model = _dbContext.GRPurchaseContract.Find(id);
            model.ApprovedBy = _userId;
            model.ApprovedDate = DateTime.UtcNow;
            model.IsApproved = true;
            model.Status = "Approved";
            _dbContext.GRPurchaseContract.Update(model);
            _dbContext.SaveChanges();
            TempData["error"] = "false";
            TempData["message"] = "Purchase Contract has been approved successfully.";
            return RedirectToAction("Index", "PurchaseContract");
        }
        public IActionResult UnApprove(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            GRPurchaseContract model = _dbContext.GRPurchaseContract.Find(id);


            var checkCatgryRfrnc = _dbContext.GRInwardGatePass.Where(x => x.WeavingContractId == id).ToList();
            //var checkCatgryRfrnc2 = _dbContext.GRPurchaseContract.Where(x => x.PurchaseGRQualityId == id || x.ContractGRQualityId == id).ToList();
            if (checkCatgryRfrnc.Count == 0)
            {
                model.ApprovedBy = _userId;
                model.ApprovedDate = DateTime.UtcNow;
                model.IsApproved = false;
                model.Status = "Created";
                _dbContext.GRPurchaseContract.Update(model);
                _dbContext.SaveChanges();
                TempData["error"] = "false";
                TempData["message"] = "Purchase Contract has been UnApproved successfully.";

            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Transaction No is Used in IGP..!";
            }


            return RedirectToAction("Index", "Contract");

            //model.ApprovedBy = _userId;
            //model.ApprovedDate = DateTime.UtcNow;
            //model.IsApproved = false;
            //model.Status = "Created";
            //_dbContext.GRPurchaseContract.Update(model);
            //_dbContext.SaveChanges();
            //TempData["error"] = "false";
            //TempData["message"] = "Purchase Contract has been UnApproved successfully.";
            //return RedirectToAction("Index", "PurchaseContract");
        }



        public async Task<IActionResult> Delete(int id)
        {
            var ContractRepo = new PurchaseContractRepo(_dbContext);
            bool isSuccess = await ContractRepo.Delete(id);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Purchase Contract has been deleted successfully";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Purchase Contract not found";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
