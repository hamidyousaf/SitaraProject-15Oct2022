using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Repository.AR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using Numbers.Repository.Helpers;

namespace Numbers.Areas.AR.Controllers
{
    [Area("AR")]
    [Authorize]
    public class PackingController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public PackingController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {

            ViewBag.NavbarHeading = "List of Packing";

            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string configs = _dbContext.AppCompanyConfigs
                     .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                     .Select(c => c.ConfigValue)
                     .FirstOrDefault();
            ViewBag.ReportUrl = configs;
            ViewBag.ReportPath = string.Concat(configs, "Viewer", "?Report=PackingBasePrint&cId=", companyId, "&id=");
            

            return View();
        }
        public IActionResult Create(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var PackingsRepo = new PackingRepo(_dbContext);
            var configValues = new ConfigValues(_dbContext);
            var PackingRepo = new PackingRepo(_dbContext);
            PackingViewModel packingViewModel = new PackingViewModel();
            packingViewModel.ReasonType = configValues.GetConfigValues("AR", "Reason Type", companyId);
            packingViewModel.ReturnType = configValues.GetConfigValues("AR", "Return Type", companyId);
            packingViewModel.Season = configValues.GetConfigValues("Inventory", "Season", companyId);
            var Categories = _dbContext.InvItemCategories.AsQueryable();
            ViewBag.Counter = 0;
            ViewBag.WareHouseLOV = configValues.GetConfigValues("Inventory", "Ware House", companyId).OrderByDescending(x => x.Text.Contains("GD Sale Local"));
            if (id == 0)
            {
                //packingViewModel.SIGPNo = SIGPRepository.SIGPMaxNo(companyId);
                //var customersIds = _dbContext.SaleReturn.Where(x=>x.IsDeleted == false && x.IsApproved == true && x.BalanceQuantity > 0).Select(x=>x.CustomerId).ToList();
                var customersIds = _dbContext.SaleReturnItems.Include(x=>x.SaleReturn).Where(x => x.SaleReturn.IsDeleted == false && x.SaleReturn.IsApproved == true && x.MetersBalance > 0).Select(x => x.SaleReturn.CustomerId).ToList();
                packingViewModel.CustomerLOV = new SelectList(_dbContext.ARCustomers.Where(x => customersIds.Contains(x.Id) /*&& x.CompanyId == companyId*/ && x.IsDeleted != true).ToList(), "Id", "Name");
                //ViewBag.FourthLevelCategoryLOV = new SelectList(from c in Categories

                //                                                where c.IsDeleted != true && c.CategoryLevel == 4 && c.Code.Contains("07.01")
                //                                                select new
                //                                                {
                //                                                    Id = c.Id,
                //                                                    Name = c.Code + " - " + c.Name
                //                                                }, "Id", "Name");
                ViewBag.EntityState = "Create";
                ViewBag.NavbarHeading = "Create Packing";
                return View(packingViewModel);
            }
            else
            {
                packingViewModel = PackingsRepo.GetById(id);
                packingViewModel.CustomerLOV = new SelectList(_dbContext.ARCustomers.Where(x => x.Id == packingViewModel.CustomerId/* && x.CompanyId == companyId*/ && x.IsDeleted != true).ToList(), "Id", "Name");
                packingViewModel.Address = _dbContext.ARCustomers.FirstOrDefault(x => x.Id == packingViewModel.CustomerId).Address;
                packingViewModel.PackingItemsList = _dbContext.ARPackingItems.Include(x=>x.FourthLevel).Include(x=>x.Item).Include(x => x.ReasonType).Include(x => x.ReturnType).Include(x => x.Season).Where(x => x.PackingId == packingViewModel.Id).ToArray();
                packingViewModel.ReasonType = configValues.GetConfigValues("AR", "Reason Type", companyId);
                packingViewModel.ReturnType = configValues.GetConfigValues("AR", "Return Type", companyId);
                packingViewModel.Season = configValues.GetConfigValues("Inventory", "Season", companyId);
                ViewBag.SRINo = new SelectList(_dbContext.SaleReturn.Where(x => x.IsApproved == true && x.Id == packingViewModel.SRIId).ToList(), "Id", "TransactionNo");
                int[] FouthCategoryIds = _dbContext.SaleReturnItems.Where(x=> x.SaleReturnId == packingViewModel.SRIId).Select(x=>x.FourthItemCategory).ToArray();
                ViewBag.FourthLevelCategoryLOV = new SelectList(from c in Categories

                                                                where FouthCategoryIds.Contains(c.Id) && c.IsDeleted != true && c.CategoryLevel == 4 && c.Code.Contains("07.01")
                                                                select new
                                                                {
                                                                    Id = c.Id,
                                                                    Name = c.Code + " - " + c.Name
                                                                }, "Id", "Name");
                ViewBag.EntityState = "Update";
                ViewBag.NavbarHeading = "Update Packing";
                ViewBag.TitleStatus = "Created";
                return View(packingViewModel);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Create(PackingViewModel model, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var PackingsRepo = new PackingRepo(_dbContext);
          
            if (model.Id == 0)
            {
                model.CreatedBy = userId;
                model.CompanyId = companyId;
                model.Resp_ID = resp_Id;
                model.PackingNo = PackingsRepo.MaxNo(companyId);
                
                bool isSuccess = await PackingsRepo.Create(model,collection);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = string.Format("Packing. {0} has been created successfully.", model.PackingNo);
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Something went wrong.";
                }
                return RedirectToAction("Create", "Packing");
            }
            else
            {
                model.UpdatedBy = userId;
                model.CompanyId = companyId;
                model.Resp_ID = resp_Id;
                
                bool isSuccess = await PackingsRepo.Update(model,collection);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = string.Format("Packing. {0} has been updated successfully.", model.PackingNo);
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
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            try
            {
                int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
                string userId = HttpContext.Session.GetString("UserId");
                var approve = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == resp_Id).FirstOrDefault().Approve;
                var unApprove = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == resp_Id).FirstOrDefault().UnApprove;

                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchTransNo = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchSRDate = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchCustomer = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchIGPNo = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchBuiltyNo = Request.Form["columns[4][search][value]"].FirstOrDefault();
                var searchBails = Request.Form["columns[5][search][value]"].FirstOrDefault();
                var searchStatus = Request.Form["columns[6][search][value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var Data = (from packing in _dbContext.ARPacking.Include(x => x.SRI).Include(x=>x.Customer).Where(x => x.IsDeleted == false && x.CompanyId == companyId) select packing);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    Data = Data.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                Data = !string.IsNullOrEmpty(searchTransNo) ? Data.Where(m => m.PackingNo.ToString().ToLower().Contains(searchTransNo.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchSRDate) ? Data.Where(m => m.PackingDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchSRDate.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchCustomer) ? Data.Where(m => m.Customer.Name.ToString().ToLower().Contains(searchCustomer.ToLower())) : Data;
                Data = !string.IsNullOrEmpty(searchIGPNo) ? Data.Where(m => m.SRI.TransactionNo.ToString().Contains(searchIGPNo.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchStatus) ? Data.Where(m => m.Status.ToString().ToUpper().Contains(searchStatus.ToUpper())) : Data;

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
                List<PackingViewModel> Details = new List<PackingViewModel>();
                foreach (var grp in data)
                {
                    PackingViewModel packingViewModel = new PackingViewModel();
                    packingViewModel.ARPacking = grp;
                    packingViewModel.ARPacking.Approve = approve;
                    packingViewModel.ARPacking.Unapprove = unApprove;
                    packingViewModel.Date = grp.PackingDate.ToString(Helpers.CommonHelper.DateFormat);
                    Details.Add(packingViewModel);
                }
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = Details };
                return Ok(jsonData);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<IActionResult> Approve(int id)
        {
            string userId = HttpContext.Session.GetString("UserId");
            var PackingsRepo = new PackingRepo(_dbContext);
            bool isSuccess = await PackingsRepo.Approve(id, userId);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Packing has been approved successfully";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong.";
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult GetBuiltyAndBale(int Id)
        {
            if (Id != 0)
            {
                var igp = _dbContext.ARInwardGatePass.FirstOrDefault(x => x.Id == Id);
                if (igp != null)
                {
                    return Ok(igp);
                }
                return Ok(null);
            }
            return Ok(null);
        }
        [HttpGet]
        public IActionResult GetSaleReturnNo(int Id)
        {
            if (Id != 0)
            {
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                var saleReturnNo = _dbContext.SaleReturn.Where(x => x.CompanyId == companyId && x.CustomerId == Id && x.IsApproved == true).ToList();
                if (saleReturnNo != null)
                {
                    return Ok(saleReturnNo);
                }
                return Ok(null);
            }
            return Ok(null);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var packingRepo = new PackingRepo(_dbContext);
            bool isSuccess = await packingRepo.Delete(id);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "OGP has been deleted successfully.";
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
            var aRPacking = _dbContext.ARPacking
                .Include(i => i.Customer)
                .Include(i => i.SRI)
            .Where(i => i.Id == id).FirstOrDefault();
            var aRPackingItems = _dbContext.ARPackingItems
                                .Include(i => i.FourthLevel)
                                .Include(i => i.Item)
                                .Include(i => i.ReturnType)
                                .Include(i => i.ReasonType)
                                .Include(i => i.Season)
                                .Where(i => i.PackingId == id)
                                .ToList();
            ViewBag.TotalQty = aRPackingItems.Sum(x => x.Qty);

            ViewBag.NavbarHeading = "Packing/Segregation";
            ViewBag.TitleStatus = "Approved";

            TempData["Detail"] = aRPackingItems;
            return View(aRPacking);
        }

        public IActionResult GetFourthCategoryId(int saleReturnId)
        {
            var items = _dbContext.SaleReturnItems.Include(x=>x.FourthLevel).Where(x => x.SaleReturnId == saleReturnId).ToList();
            var item = (from sr in _dbContext.SaleReturnItems.Where(x => x.SaleReturnId == saleReturnId).ToList()
            join c in _dbContext.InvItemCategories on sr.FourthItemCategory equals c.Id
                        select c).ToList();
            return Ok(item);
        }
        [HttpGet]
        public IActionResult GetBaleQuantity(int FourthCatId, int SaleReturnId)
        {
            if (FourthCatId != 0 && SaleReturnId != 0)
            {
                var baleQty = _dbContext.SaleReturnItems.FirstOrDefault(x => x.SaleReturnId == SaleReturnId && x.FourthItemCategory == FourthCatId);
                if (baleQty != null)
                {
                    ViewBag.Counter = 0;
                    return Ok(baleQty);
                }
                return Ok(null);
            }
            return Ok(null);
        }
    }
}
