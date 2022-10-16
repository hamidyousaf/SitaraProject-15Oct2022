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
    public class ContractController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public ContractController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public IActionResult Index()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var ContractRepo = new ContractRepo(_dbContext);

           
            string configValue = _dbContext.AppCompanyConfigs
                                          .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                                          .Select(c => c.ConfigValue)
                                          .FirstOrDefault();
            ViewBag.ReportPath = string.Concat(configValue, "Viewer", "?Report=WeavingContractBP&cId=", companyId, "&id=");

            IEnumerable<GRWeavingContract> items = ContractRepo.GetAll(companyId);
             
                ViewBag.NavbarHeading = "List of Weaving Contract";
                return View(items);
          
        }
        
        public IActionResult Details(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            ViewBag.SalesTaxLOV = new SelectList(_dbContext.AppTaxes.Include(t => t.SalesTaxAccount).Include(t => t.ExciseTaxAccount).Include(t => t.IncomeTaxAccount).Where(t => t.CompanyId == companyId && t.IsDeleted == false).OrderByDescending(x => x.Name.Contains("GST 17%")).ToList(), "Id", "Name");
            ViewBag.GreigeQuality = new SelectList(_dbContext.GRQuality.Where(x => x.IsDeleted == false ).OrderBy(x => x.TransactionNo).ToList()
                                                          , "Id", "Description");

            ViewBag.Vendors = new SelectList(_dbContext.APSuppliers.OrderBy(x => x.Id).ToList()
                                                        , "Id", "Name");
            if (id == 0)
            {

                ViewBag.EntityState = "Create";
                ViewBag.NavbarHeading = "Create Weaving Contract";


                return View(new GRWeavingContract());
            }
            else
            {
                ViewBag.EntityState = "Details";
                ViewBag.NavbarHeading = "Details Weaving Contract";
                GRWeavingContract item = _dbContext.GRWeavingContracts.Include(x => x.GreigeQuality).Include(x => x.GreigeQualityLoom).Where(x => x.Id == id && x.CompanyId==companyId).FirstOrDefault();
                return View(item);
            }

        }

        [HttpGet]
        public IActionResult Create(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            ViewBag.SalesTaxLOV = new SelectList(_dbContext.AppTaxes.Include(t => t.SalesTaxAccount).Include(t => t.ExciseTaxAccount).Include(t => t.IncomeTaxAccount).Where(t => t.CompanyId == companyId && t.IsDeleted == false).OrderByDescending(x => x.Name.Contains("GST 17%")).ToList(), "Id", "Name");
            ViewBag.Vendors = new SelectList(_dbContext.APSuppliers.OrderBy(x => x.Id).ToList()
                                                      , "Id", "Name");
            ViewBag.SalesTaxLOV = new SelectList(_dbContext.AppTaxes.Include(t => t.SalesTaxAccount).Include(t => t.ExciseTaxAccount).Include(t => t.IncomeTaxAccount).Where(t => t.CompanyId == companyId && t.IsDeleted == false).OrderByDescending(x => x.Name.Contains("GST 17%")).ToList(), "Id", "Name");

            if (id == 0)
            {
                var data = from master in _dbContext.GRGriegeRequisition.Where(x => x.IsDeleted == false && x.TransferToCompany == companyId && x.IsApproved).OrderByDescending(x => x.TransactionNo).ToList()
                           join
                            detail in _dbContext.GRGriegeRequisitionDetails.Where(x => x.BalanceQty > 0) on
                            master.Id equals detail.GRRequisitionId
                           select master;
                var transactionNo = data.GroupBy(x => x.Id).Select(x => new ListOfValue {
                    Id = x.Select(a => a.Id).FirstOrDefault(),
                    TransactionNo = x.Select(a => a.TransactionNo).FirstOrDefault()
                }).ToList();
                ViewBag.GRRequisitionNo = new SelectList(transactionNo, "Id", "TransactionNo");
                ViewBag.GreigeQualityOnLoom = new SelectList(_dbContext.GRQuality.Where(x => x.IsDeleted == false /*&& x.CompanyId == companyId*/).OrderBy(x => x.TransactionNo).ToList()
                                                          , "Id", "Description");
                ViewBag.EntityState = "Create";
                    ViewBag.NavbarHeading = "Create Weaving Contract";
 
               
                return View(new GRWeavingContract());
            }
            else
            {
                ViewBag.EntityState = "Update";
                ViewBag.NavbarHeading = "Update Weaving Contract";
                GRWeavingContract item = _dbContext.GRWeavingContracts.Include(x=>x.GreigeQuality).Include(x=>x.GreigeQualityLoom).Where(x => x.Id == id && x.CompanyId==companyId).FirstOrDefault();
                var GRGriegeRequisition = new SelectList(_dbContext.GRGriegeRequisition.Where(x => x.IsDeleted == false && x.TransferToCompany == companyId && x.Id == item.GRRequisitionId).OrderByDescending(x => x.TransactionNo).ToList()
                                                    , "Id", "TransactionNo");
                ViewBag.GRRequisitionNo = GRGriegeRequisition;
                ViewBag.GreigeQualityOnLoom = new SelectList(_dbContext.GRQuality.Where(x => x.IsDeleted == false).OrderBy(x => x.TransactionNo).ToList()
                                                        , "Id", "Description");
                ViewBag.GreigeQuality = new SelectList( from det in _dbContext.GRGriegeRequisitionDetails join
                                                        qual in _dbContext.GRQuality.Where(x => x.IsDeleted == false ).OrderBy(x => x.TransactionNo).ToList()
                                                        on det.GriegeQualityId equals qual.Id 
                                                        where (qual.Id == item.GreigeQualityId && det.GRRequisitionId.ToString() == GRGriegeRequisition.Select(x => x.Value).FirstOrDefault()) || (!det.IsUsed && det.GRRequisitionId.ToString() == GRGriegeRequisition.Select(x=>x.Value).FirstOrDefault())
                                                        select qual
                                                        , "Id", "Description");
                var x = _dbContext.GRGriegeRequisitionDetails.FirstOrDefault(x => x.GRRequisitionId == item.GRRequisitionId).BalanceQty;
                ViewBag.Quantity = x;
                ViewBag.hQuantity = x + item.ContractQty;
                return View(item);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(GRWeavingContract model, IFormFile img)
        {

            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var ContractRepo = new ContractRepo(_dbContext);
            //if (ModelState.IsValid)
            //{
                int TransactionNo = 1;
                var list = _dbContext.GRWeavingContracts.ToList();
                if (list.Count != 0)
                {
                    TransactionNo = list.Select(x => x.TransactionNo).Max() + 1;
                }
                model.TransactionNo = TransactionNo;
                if (model.Id == 0)
                {
                    model.CompanyId = companyId;
                    model.CreatedBy = userId;
                    bool isSuccess = await ContractRepo.Create(model, img);
                    if (isSuccess == true)
                    {
                        TempData["error"] = "false";
                        TempData["message"] = string.Format("Weaving Contract has been saved successfully");
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
                    model.CompanyId = companyId;
                    model.UpdatedBy = userId;
                    bool isSuccess = await ContractRepo.Update(model, img);

                    if (isSuccess == true)
                    {
                        TempData["error"] = "false";
                        TempData["message"] = string.Format("Weaving Contract has been saved successfully");
                    }
                    else
                    {
                        TempData["error"] = "true";
                        TempData["message"] = "Something went wrong.";
                    }
                    return RedirectToAction(nameof(Index));
                }
            //}
            //return View();
        }
 
        public async Task<IActionResult> Delete(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var ContractRepo = new ContractRepo(_dbContext);
            bool isSuccess = await ContractRepo.Delete(id,companyId);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Weaving Contract has been deleted successfully";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Weaving Contract not found";
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
                var InvData = (from Inv in _dbContext.GRWeavingContracts.Include(x=>x.GreigeQuality).Include(x=>x.GreigeQualityLoom).Where(x => x.IsDeleted == false && x.CompanyId == companyId) select Inv);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    InvData = InvData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                InvData = !string.IsNullOrEmpty(searchItemCode) ? InvData.Where(m => m.TransactionNo.ToString().ToUpper().Contains(searchItemCode.ToUpper())) : InvData;
                InvData = !string.IsNullOrEmpty(searchItemName) ? InvData.Where(m => m.TransactionDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchItemName.ToUpper())) : InvData;
                InvData = !string.IsNullOrEmpty(searchBarcode) ? InvData.Where(m => m.GreigeQuality.Description.ToString().ToUpper().Contains(searchBarcode.ToUpper())) : InvData;
                InvData = !string.IsNullOrEmpty(searchUOM) ? InvData.Where(m => m.GreigeQualityLoom.Description.ToString().ToUpper().Contains(searchUOM.ToUpper())) : InvData;

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
                List<GRWeavingContract> details = new List<GRWeavingContract>();
                foreach (var item in data)
                {
                    var WeavingContract = new GRWeavingContract();
                    WeavingContract.UpdatedBy = item.TransactionDate.ToString(Helpers.CommonHelper.DateFormat);
                    WeavingContract.WeavingContract = item;
                    WeavingContract.WeavingContract.Approve = approve;
                    WeavingContract.WeavingContract.Unapprove = unApprove;
                    details.Add(WeavingContract);
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
            var model = _dbContext.GRWeavingContracts.Where(x=>x.Id==id && x.CompanyId==_companyId).FirstOrDefault();
            model.ApprovedBy = _userId;
            model.ApprovedDate = DateTime.UtcNow;
            model.IsApproved = true;
            model.Status = "Approved";
            _dbContext.GRWeavingContracts.Update(model);
            _dbContext.SaveChanges();
            TempData["error"] = "false";
            TempData["message"] = "Weaving Contract has been approved successfully.";
            return RedirectToAction("Index", "Contract");
        }

        [HttpGet]
        public IActionResult GetQuality(int Id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;

            var item = (from b in _dbContext.GRGriegeRequisitionDetails
                        join c in _dbContext.GRQuality on b.GriegeQualityId equals c.Id
                        where b.GRRequisitionId == Id  && b.BalanceQty > 0
                        select new
                        {
                            id = c.Id,
                            text = c.Description
                        }).ToList();
            return Ok(item);

        }
        [HttpGet]
        public IActionResult GetQty(int QId ,int ReqId)
        {
            

            var item = (from b in _dbContext.GRGriegeRequisitionDetails
                       
                        where b.GRRequisitionId == ReqId  && b.GriegeQualityId==QId
                        select new
                        {
                            qty=b.BalanceQty
                        }).FirstOrDefault();
            return Ok(item);

        }
        public IActionResult UnApprove(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            var model = _dbContext.GRWeavingContracts.Where(x=>x.Id==id && x.CompanyId==_companyId).FirstOrDefault();

            
            var checkCatgryRfrnc = _dbContext.GRInwardGatePass.Where(x => x.WeavingContractId == id && x.CompanyId==_companyId).ToList();
            if (checkCatgryRfrnc.Count == 0 )
            {
                model.ApprovedBy = _userId;
                model.ApprovedDate = DateTime.UtcNow;
                model.IsApproved = false;
                model.Status = "Created";
                _dbContext.GRWeavingContracts.Update(model);
                _dbContext.SaveChanges();
                TempData["error"] = "false";
                TempData["message"] = "Weaving Contract has been UnApproved successfully.";

            }
            else 
            {
                TempData["error"] = "true";
                TempData["message"] = "Transaction No is Used in IGP..!";
            }
            
 
            return RedirectToAction("Index", "Contract");

           
        }

    }
}