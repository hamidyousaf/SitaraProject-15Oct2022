using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Numbers.Entity;
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
    public class GRNInvoiceController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        private object collection;

        public GRNInvoiceController(NumbersDbContext context)
        {
            _dbContext = context;
        }
        public IActionResult Index()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            ViewBag.NavbarHeading = "List of Invoices";
            string configs = _dbContext.AppCompanyConfigs
                .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                .Select(c => c.ConfigValue)
                .FirstOrDefault();
            ViewBag.ReportPath = string.Concat(configs, "Viewer", "?Report=Voucher&cId=", companyId, "&id=");
            return View();
        }
        public IActionResult Create(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            GRNInvoiceViewModel viewModel = new GRNInvoiceViewModel();
            var repo = new GRNInvoiceRepo(_dbContext);
            if (id != 0 && id != null)
            {
                ViewBag.NavbarHeading = "Update Invoice";
                viewModel = repo.GetById(id);
                ViewBag.GRNNo = new SelectList(_dbContext.GRGRNS.Where(x => x.Id == viewModel.GRNId && x.CompanyId==companyId).ToList(), "Id", "TransactionNo");
                return View(viewModel);
            }
            ViewBag.NavbarHeading = "Create Invoice";
            var GRNId = _dbContext.GRGRNInvoices.Where(x => !x.IsDeleted).Select(x => x.GRNId).ToList();
            ViewBag.GRNNo = new SelectList(_dbContext.GRGRNS.Where(x => !GRNId.Contains(x.Id) && !x.IsDeleted && x.IsApproved && x.CompanyId == companyId).ToList().OrderByDescending(x=>x.Id), "Id", "TransactionNo");
            return View(viewModel);
        }
        public IActionResult GetDataFromGRN(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            GRGRNViewModel viewModel = new GRGRNViewModel();

            var grgrn = _dbContext.GRGRNS.FirstOrDefault(x => x.Id == id && x.CompanyId==companyId);
            viewModel.GRGRN = _dbContext.GRGRNS.Include(x=>x.Vendor).Where(x => x.Id == id && x.CompanyId == companyId).FirstOrDefault();
            viewModel.GRGRNItem = _dbContext.GRGRNItems.Include(x => x.Penalty).Where(x => x.GRGRNId == viewModel.GRGRN.Id).ToArray();
            viewModel.GRStackingItem = _dbContext.GRStackingItems.Include(x => x.WareHouse).Include(x => x.Location).Where(x => x.GRGRNId == id).ToArray();
            viewModel.Quantity = _dbContext.GRStackingItems.Where(x => x.GRGRNId == id).Select(x => x.Quantity).Sum();
            viewModel.RateOfConversionIncTax = viewModel.GRGRN.WeavingContractId != 0 ? _dbContext.GRWeavingContracts.Where(x => x.Id == viewModel.GRGRN.WeavingContractId && x.CompanyId==companyId).Select(x => x.RateOfConversionIncTax).FirstOrDefault() / 40 :
                _dbContext.GRPurchaseContract.Where(x => x.Id == viewModel.GRGRN.PurchaseContractId && x.CompanyId==companyId).Select(x => x.RatePerMeter).FirstOrDefault();
            viewModel.GRWeavingContract = _dbContext.GRWeavingContracts.Include(x => x.GreigeQualityLoom).Where(x => x.Id == viewModel.GRGRN.WeavingContractId && x.CompanyId == companyId).FirstOrDefault();
            viewModel.GRPurchaseContract = _dbContext.GRPurchaseContract.Include(x => x.ContractGRQuality).Where(x => x.Id == viewModel.GRGRN.PurchaseContractId && x.CompanyId == companyId).FirstOrDefault();

            decimal Amount1 = 0;
            decimal warpWeight = 0;
            decimal weftWeight = 0;
            decimal totalWarpBag = 0;
            decimal totalWeftBag = 0;
            decimal warpBagPerPound = 0;
            decimal weftBagPerPound = 0;
            decimal warpPricePerPund = 0;
            decimal weftPricePerPound = 0;
            decimal totalRequiredPriceOfYarnPerPund = 0;
            decimal totalActualPriceOfYarnPerPund = 0;
            decimal lessYarnBag = totalRequiredPriceOfYarnPerPund - totalActualPriceOfYarnPerPund;

            if (viewModel.GRWeavingContract != null)
            {
                viewModel.Item = _dbContext.InvItems.FirstOrDefault(x => x.Name == viewModel.GRWeavingContract.GreigeQualityLoom.Description);
                //Required Price Of Yarn Per Pound
                totalWarpBag = Convert.ToDecimal(viewModel.GRWeavingContract.TotalWarpBags);
                totalWeftBag = Convert.ToDecimal(viewModel.GRWeavingContract.TotalWeftBags);
                warpBagPerPound = totalWarpBag * 100;
                weftBagPerPound = totalWeftBag * 100;
                warpPricePerPund = warpBagPerPound * Convert.ToDecimal(viewModel.GRWeavingContract.WarpRatePound);
                weftPricePerPound = weftBagPerPound * Convert.ToDecimal(viewModel.GRWeavingContract.WeftRatePound);
                totalRequiredPriceOfYarnPerPund = Math.Round(warpPricePerPund + weftPricePerPound);

                //Actual Price Of Yarn Per Pound

                #region ActualPriceOfYarnPerPound

                //warp Weight
                var reed = viewModel.GRWeavingContract.Reed;
                var width = Convert.ToDecimal( viewModel.GRGRN.Width);
                var pick = Convert.ToDecimal( viewModel.GRGRN.Picks);
                var widthOld = viewModel.GRGRNItem.Where(x => x.Penalty.ConfigValue == "Short Width").FirstOrDefault();
                if (widthOld != null)
                {
                      width = viewModel.GRGRNItem.Where(x => x.Penalty.ConfigValue == "Short Width").FirstOrDefault().ActualWidth;
                }
                
                    var WarpCount = viewModel.GRWeavingContract.WarpCount;
                    var totalWarpWeight = ((reed * width * Convert.ToDecimal( 1.0936)) / WarpCount / 20 / 40);
                     warpWeight = totalWarpWeight;
                 

                // Weft Weight
                
                    var pickModel = viewModel.GRGRNItem.Where(x => x.Penalty.ConfigValue == "Short Pick").FirstOrDefault();

                if (pickModel != null)
                {
                    pick = viewModel.GRGRNItem.Where(x => x.Penalty.ConfigValue == "Short Pick").FirstOrDefault().ActualPick;
                }
                 
                       // width = viewModel.GRGRNItem.Where(x => x.Penalty.ConfigValue == "Short Width").FirstOrDefault().ActualWidth;
                    var WeftCount = viewModel.GRWeavingContract.WeftCount;
                    var totalWeftWeight = ((pick * width * Convert.ToDecimal(1.0936)) / WeftCount / 20 / 40);
                        weftWeight = totalWeftWeight;

                // Total Warp Bag
                
                    var contractQty = viewModel.GRGRN.FoldedQty;
                    var warpWeightPerMeter =warpWeight;
                    var amount =  ((contractQty * warpWeightPerMeter) / 100);
                        totalWarpBag = amount;

                // Total Weft Bag
                
                      contractQty = viewModel.GRGRN.FoldedQty;
                  var weftWeightPerMeter = weftWeight;  
                      amount =  ((contractQty * weftWeightPerMeter) / 100);
                      totalWeftBag = amount;
                 
                totalWarpBag = Convert.ToDecimal(totalWarpBag);
                totalWeftBag = Convert.ToDecimal(totalWeftBag);
                warpBagPerPound = totalWarpBag * 100;
                weftBagPerPound = totalWeftBag * 100;
                warpPricePerPund = warpBagPerPound * Convert.ToDecimal(viewModel.GRWeavingContract.WarpRatePound);
                weftPricePerPound = weftBagPerPound * Convert.ToDecimal(viewModel.GRWeavingContract.WeftRatePound);
                totalActualPriceOfYarnPerPund = warpPricePerPund + weftPricePerPound;

                #endregion ActualPriceOfYarnPerPound

            }
            if (viewModel.GRPurchaseContract != null)
            {
                viewModel.Item = _dbContext.InvItems.FirstOrDefault(x => x.Name == viewModel.GRPurchaseContract.ContractGRQuality.Description);
            }

            viewModel.Amount = viewModel.Quantity * viewModel.RateOfConversionIncTax;
            viewModel.TotalPenaltyAmount = viewModel.GRGRNItem.Select(x=>x.Amount).Sum();
            viewModel.NetPenaltyAmount = viewModel.Amount - viewModel.TotalPenaltyAmount;
            viewModel.LessYarnPrice = totalRequiredPriceOfYarnPerPund - totalActualPriceOfYarnPerPund;
            viewModel.NetPayableAmount = viewModel.NetPenaltyAmount - viewModel.LessYarnPrice;
            return Ok(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Create(GRNInvoiceViewModel model, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var repo = new GRNInvoiceRepo(_dbContext);
            GRGRNInvoiceDetail[] details = JsonConvert.DeserializeObject<GRGRNInvoiceDetail[]>(collection["Detail"]);
            if (model.Id == 0)
            {
                model.PurchaseNo = repo.Max(companyId);
                model.CreatedBy = userId;
                model.CompanyId = companyId;
                model.GRGRNInvoiceDetails = details;
                bool isSuccess = await repo.Create(model);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = string.Format("GRN Invoice. {0} has been created successfully.", model.PurchaseNo);
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
                model.UpdatedBy = userId;
                model.CompanyId = companyId;
                model.GRGRNInvoiceDetails = details;
                bool isSuccess = await repo.Update(model);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = string.Format("Mending. {0} has been updated successfully.", model.PurchaseNo);
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

                var searchTransNo = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchTransDate = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchVendor = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchInvoiceNo = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchInvoiceDate = Request.Form["columns[4][search][value]"].FirstOrDefault();
                var searchGRN = Request.Form["columns[5][search][value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var Data = (from records in _dbContext.GRGRNInvoices.Include(x => x.GRN).ThenInclude(x=>x.Vendor).Where(x => x.IsDeleted != true && x.CompanyId == companyId) select records);

                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    Data = Data.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                Data = !string.IsNullOrEmpty(searchTransNo) ? Data.Where(m => m.PurchaseNo.ToString().ToUpper().Contains(searchTransNo.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchTransDate) ? Data.Where(m => m.CreatedDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchTransDate.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchVendor) ? Data.Where(m => m.GRN.Vendor.Name.ToString().ToUpper().Contains(searchVendor.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchInvoiceNo) ? Data.Where(m => m.SupplierInvoiceNo != null ? m.SupplierInvoiceNo.ToString().ToUpper().Contains(searchInvoiceNo.ToUpper()): false) : Data;
                Data = !string.IsNullOrEmpty(searchInvoiceDate) ? Data.Where(m => m.SupplierInvoiceDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchInvoiceDate.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchGRN) ? Data.Where(m => m.GRN.TransactionNo.ToString().ToUpper().Contains(searchGRN.ToUpper())) : Data;

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

                List<GRNInvoiceViewModel> viewModel = new List<GRNInvoiceViewModel>();
                foreach (var item in data)
                {
                    GRNInvoiceViewModel model = new GRNInvoiceViewModel();
                    model.GRGRNInvoices = item;
                    model.Date = item.CreatedDate.ToString(Helpers.CommonHelper.DateFormat);
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
            var grnRepo = new GRNInvoiceRepo(_dbContext, HttpContext);
            bool isSuccess = await grnRepo.Approve(id, userId, companyId);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Greige GRN Invoice has been approved successfully";
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
            GRGRNInvoice model = _dbContext.GRGRNInvoices.Find(id);
            model.ApprovedBy = _userId;
            model.ApprovedDate = DateTime.UtcNow;
            model.Status = "Approved";
            _dbContext.GRGRNInvoices.Update(model);
            _dbContext.SaveChanges();
            TempData["error"] = "false";
            TempData["message"] = "Invoice has been approved successfully.";
            return RedirectToAction(nameof(Index));
        }
        public IActionResult UnApprove(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            GRGRNInvoice model = _dbContext.GRGRNInvoices.Find(id);

            model.Status = "Created";
            _dbContext.GRGRNInvoices.Update(model);
            _dbContext.SaveChanges();
            TempData["error"] = "false";
            TempData["message"] = "Invoice has been UnApproved.";
            return RedirectToAction(nameof(Index));


        }
        public async Task<IActionResult> Delete(int id)
        {
            var repo = new GRNInvoiceRepo(_dbContext);
            bool isSuccess = await repo.Delete(id);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "IGP has been deleted successfully.";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong.";
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var master = _dbContext.GRGRNInvoices
                .Include(i => i.GRN).ThenInclude(x=>x.Vendor)
            .Where(i => i.Id == id && i.CompanyId==companyId).FirstOrDefault();
            var details = _dbContext.GRGRNInvoiceDetails
                                .Include(i => i.Item)
                                .Where(i => i.GRNId == id)
                                .ToList();
            //ViewBag.TotalQty = aRPackingItems.Sum(x => x.Qty);

            ViewBag.NavbarHeading = "Invoice";
            ViewBag.TitleStatus = "Approved";

            TempData["Detail"] = details;
            return View(master);
        }
    }
}