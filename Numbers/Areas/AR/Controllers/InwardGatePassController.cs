using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Repository.AR;
using Numbers.Repository.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;

namespace Numbers.Areas.AR.Controllers
{
    [Area("AR")]
    [Authorize]
    public class InwardGatePassController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public InwardGatePassController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.NavbarHeading = "List of IGP";
            return View();
        }
        public IActionResult Create(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var configValues = new ConfigValues(_dbContext);
            var SIGPRepository = new InwardGatePassRepo(_dbContext);
            InwardGatePassViewModel SIGPViewModel = new InwardGatePassViewModel();
            if (id == 0)
            {
                 
                SIGPViewModel.CustomerLOV = new SelectList(_dbContext.ARCustomers.Where(x => /*x.CompanyId == companyId &&*/ x.IsDeleted != true && x.IsActive != false).ToList(), "Id", "Name");
                ViewBag.WareHouseLOV = configValues.GetConfigValues("Inventory", "Ware House", companyId).OrderByDescending(x => x.Text.Contains("GD Sale Local"));
                ViewBag.EntityState = "Create";
                ViewBag.NavbarHeading = "Create Inward Gate Pass";
                return View(SIGPViewModel);
            }
            else
            {
                SIGPViewModel = SIGPRepository.GetById(id);
                SIGPViewModel.CustomerLOV = new SelectList(_dbContext.ARCustomers.Where(x => x.Id == SIGPViewModel.CustomerId /*&& x.CompanyId == companyId*/ && x.IsDeleted != true && x.IsActive != false).ToList(), "Id", "Name");
                SIGPViewModel.Address = _dbContext.ARCustomers.FirstOrDefault(x => x.Id == SIGPViewModel.CustomerId).Address;
                ViewBag.OGPNo = new SelectList(_dbContext.AROutwardGatePass.Where(x=>x.IsApproved == true && x.Id == SIGPViewModel.OGPId).ToList(), "Id", "OGPNo");
                ViewBag.WareHouseLOV = new SelectList(_dbContext.AppCompanyConfigs.Where(x => x.Id == SIGPViewModel.WarehouseId).ToList(), "Id", "ConfigValue");
                ViewBag.EntityState = "Update";
                ViewBag.NavbarHeading = "Update Inward Gate Pass";
                ViewBag.TitleStatus = "Created";
                return View(SIGPViewModel);

            }
        }
        [HttpPost]
        public async Task<IActionResult> Create(InwardGatePassViewModel model)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            var SIGPRepository = new InwardGatePassRepo(_dbContext);
            if (model.Id == 0)
            {
                model.CreatedBy = userId;
                model.CompanyId = companyId;
                model.Resp_ID = resp_Id;
                model.IGPNo = SIGPRepository.SIGPMaxNo(companyId);
                bool isSuccess = await SIGPRepository.Create(model);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = string.Format("IGP No. {0} has been created successfully.", model.IGPNo);
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Something went wrong.";
                }
                return RedirectToAction(nameof(Create));
            }
            else
            {
                model.UpdatedBy = userId;
                model.CompanyId = companyId;
                model.Resp_ID = resp_Id;
                bool isSuccess = await SIGPRepository.Update(model);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = string.Format("IGP No. {0} has been updated successfully.", model.IGPNo);
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Something went wrong.";
                }
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpGet]
        public IActionResult GetCustomerAddress(int Id)
        {
            if (Id != 0)
            {
                var address = _dbContext.ARCustomers.FirstOrDefault(x=>x.Id == Id).Address;
                if (address != null)
                {
                    return Ok(address);
                }
                return Ok("Not Available");
            }
            return Ok("Not Available");
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
                var searchSIGPNo = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchSIGPDate = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchWarehouse = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchCustomer = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchBuiltyNo = Request.Form["columns[4][search][value]"].FirstOrDefault();
                var searchBails = Request.Form["columns[5][search][value]"].FirstOrDefault();
                var searchStatus = Request.Form["columns[6][search][value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var Data = (from SIGP in _dbContext.ARInwardGatePass.Include(x=>x.Warehouse).Include(x=>x.Customer).Where(x => x.IsDeleted == false && x.CompanyId == companyId) select SIGP);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    Data = Data.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                Data = !string.IsNullOrEmpty(searchSIGPNo) ? Data.Where(m => m.IGPNo.ToString().ToLower().Contains(searchSIGPNo.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchSIGPDate) ? Data.Where(m => m.IGPDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchSIGPDate.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchWarehouse) ? Data.Where(m => m.Warehouse.ConfigValue.ToUpper().Contains(searchWarehouse.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchCustomer) ? Data.Where(m => m.Customer.Name.ToString().ToLower().Contains(searchCustomer.ToLower())) : Data;
                Data = !string.IsNullOrEmpty(searchBuiltyNo) ? Data.Where(m => m.BuiltyNo.ToString().Contains(searchBuiltyNo.ToLower())) : Data;
                Data = !string.IsNullOrEmpty(searchBails) ? Data.Where(m => m.Bails.ToString().ToUpper().Contains(searchBails.ToUpper())) : Data;
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
                List<InwardGatePassViewModel> Details = new List<InwardGatePassViewModel>();
                foreach (var grp in data)
                {
                    InwardGatePassViewModel aRSaleOrderViewModel = new InwardGatePassViewModel();
                    aRSaleOrderViewModel.ARInwardGatePass = grp;
                    aRSaleOrderViewModel.ARInwardGatePass.Approve = approve;
                    aRSaleOrderViewModel.ARInwardGatePass.Unapprove = unApprove;
                    aRSaleOrderViewModel.Date = grp.IGPDate.ToString(Helpers.CommonHelper.DateFormat);
                    Details.Add(aRSaleOrderViewModel);
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
            var inwardGatePassRepo = new InwardGatePassRepo(_dbContext);
            bool isSuccess = await inwardGatePassRepo.Approve(id, userId);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "IGP has been approved successfully";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong.";
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult GetOGPNo(int Id)
        {
            if (Id != 0)
            {
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                var ogpIds = _dbContext.ARInwardGatePass.Where(x => x.IsActive == true && x.IsDeleted == false).Select(x=>x.OGPId);
                var aROutwardGatePass = _dbContext.AROutwardGatePass.Where(x => !ogpIds.Contains(x.Id) && x.CompanyId == companyId && x.CustomerId == Id && x.IsApproved == true).ToList();
                if (aROutwardGatePass != null)
                {
                    return Ok(aROutwardGatePass);
                }
                return Ok(null);
            }
            return Ok(null);
        }
        [HttpGet]
        public IActionResult GetBuiltyAndBale(int Id)
        {
            if (Id != 0)
            {
                var ogp = _dbContext.AROutwardGatePass.FirstOrDefault(x => x.Id == Id);
                if (ogp != null)
                {
                    return Ok(ogp);
                }
                return Ok(null);
            }
            return Ok(null);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var inwardGatePassRepo = new InwardGatePassRepo(_dbContext);
            bool isSuccess = await inwardGatePassRepo.Delete(id);
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
            var sROutwardGatePass = _dbContext.ARInwardGatePass
                .Include(i => i.Customer)
                .Include(i => i.OGP)
                .Include(i => i.Warehouse)
                .Where(i => i.Id == id).FirstOrDefault();
            ViewBag.NavbarHeading = "IGP";
            ViewBag.TitleStatus = "Approved";

            return View(sROutwardGatePass);
        }
        [HttpGet]
        public IActionResult GetCustomer(int warehouseId)
        {
            if (warehouseId != 0)
            {
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                var ogpIds = _dbContext.ARInwardGatePass.Where(x => x.IsActive == true && x.IsDeleted == false).Select(x => x.OGPId);
                var customerIds = _dbContext.AROutwardGatePass.Where(x => !ogpIds.Contains(x.Id) && x.WarehouseId == warehouseId && x.IsApproved == true ).Select(x => x.CustomerId).ToList();
                var customers = _dbContext.ARCustomers.Where(x => x.CompanyId == companyId && customerIds.Contains(x.Id) && x.IsActive == true && x.IsDeleted == false).ToList();
                if (customers != null)
                {
                    return Ok(customers);
                }
                return Ok(null);
            }
            return Ok(null);
        }
    }
}
