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
    public class OutwardGatePassController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public OutwardGatePassController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.NavbarHeading = "List of OGP";
            return View();
        }
        public IActionResult Create(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var configValues = new ConfigValues(_dbContext);
            var SIGPRepository = new OutwardGatePassRepo(_dbContext);
            OutwardGatePassViewModel SIGPViewModel = new OutwardGatePassViewModel();
            if (id == 0)
            {
               
                SIGPViewModel.CustomerLOV = new SelectList(_dbContext.ARCustomers.Where(x => /*x.CompanyId == companyId &&*/ x.IsDeleted != true  && x.IsActive != false).ToList(), "Id", "Name");
                ViewBag.WareHouseLOV = configValues.GetConfigValues("Inventory", "Ware House", companyId).OrderByDescending(x => x.Text.Contains("GD Sale Local"));
                ViewBag.EntityState = "Create";
                ViewBag.NavbarHeading = "Create OGP";
                return View(SIGPViewModel);
            }
            else
            {
                SIGPViewModel = SIGPRepository.GetById(id);
                SIGPViewModel.CustomerLOV = new SelectList(_dbContext.ARCustomers.Where(x => x.Id == SIGPViewModel.CustomerId /*&& x.CompanyId == companyId*/ && x.IsDeleted != true && x.IsActive != false).ToList(), "Id", "Name");
                SIGPViewModel.Address = _dbContext.ARCustomers.FirstOrDefault(x => x.Id == SIGPViewModel.CustomerId).Address;
                //Handle Balance In Edit
                var TotalOGPQty = _dbContext.AROutwardGatePass.Where(x=>x.SIGPId == SIGPViewModel.SIGPId && x.IsActive == true && x.IsDeleted == false).Sum(x=>x.OGPQty);
                var TotalBales = _dbContext.ARSaleReturnInwardGatePass.FirstOrDefault(x=>x.Id == SIGPViewModel.SIGPId && x.IsActive == true && x.IsDeleted == false).Bails;
                SIGPViewModel.BalanceBails = TotalBales - Convert.ToInt32(TotalOGPQty);
                //----------
                ViewBag.WareHouseLOV = new SelectList(_dbContext.AppCompanyConfigs.Where(x => x.Id == SIGPViewModel.WarehouseId).ToList(), "Id", "ConfigValue");
                ViewBag.SIGPNo = new SelectList(_dbContext.ARSaleReturnInwardGatePass.Where(x=> x.Id == SIGPViewModel.SIGPId && x.IsApproved == true).ToList(),"Id", "SIGPNo");
                ViewBag.EntityState = "Update";
                ViewBag.NavbarHeading = "Update OGP";
                ViewBag.TitleStatus = "Created";
                return View(SIGPViewModel);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Create(OutwardGatePassViewModel model)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            var SIGPRepository = new OutwardGatePassRepo(_dbContext);
            if (model.Id == 0)
            {
                model.CreatedBy = userId;
                model.CompanyId = companyId;
                model.Resp_ID = resp_Id;
                model.OGPNo = SIGPRepository.SIGPMaxNo(companyId);
                bool isSuccess = await SIGPRepository.Create(model);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = string.Format("OGP No. {0} has been created successfully.", model.OGPNo);
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
                    TempData["message"] = string.Format("OGP No. {0} has been updated successfully.", model.OGPNo);
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
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                var address = _dbContext.ARCustomers.FirstOrDefault(x=>x.Id == Id && x.CompanyId == companyId).Address;
                if (address != null)
                {
                    return Ok(address);
                }
                return Ok(null);
            }
            return Ok(null);
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
                var searchSOGPNo = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchSOGPDate = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchWarehouse = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchCustomer = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchBuiltyNo = Request.Form["columns[4][search][value]"].FirstOrDefault();
                var searchBails = Request.Form["columns[5][search][value]"].FirstOrDefault();
                var searchStatus = Request.Form["columns[6][search][value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var Data = (from SIGP in _dbContext.AROutwardGatePass.Include(x=>x.Warehouse).Include(x=>x.Customer).Where(x => x.IsDeleted == false && x.CompanyId == companyId) select SIGP);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    Data = Data.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                Data = !string.IsNullOrEmpty(searchSOGPNo) ? Data.Where(m => m.OGPNo.ToString().ToLower().Contains(searchSOGPNo.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchSOGPDate) ? Data.Where(m => m.OGPDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchSOGPDate.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchWarehouse) ? Data.Where(m => m.Warehouse.ConfigValue.ToUpper().Contains(searchWarehouse.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchCustomer) ? Data.Where(m => m.Customer.Name.ToString().ToLower().Contains(searchCustomer.ToLower())) : Data;
                Data = !string.IsNullOrEmpty(searchBuiltyNo) ? Data.Where(m => m.BuiltyNo.ToString().Contains(searchBuiltyNo.ToLower())) : Data;
                Data = !string.IsNullOrEmpty(searchBails) ? Data.Where(m => m.OGPQty.ToString().ToUpper().Contains(searchBails.ToUpper())) : Data;
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
                List<OutwardGatePassViewModel> Details = new List<OutwardGatePassViewModel>();
                foreach (var grp in data)
                {
                    OutwardGatePassViewModel aRSaleOrderViewModel = new OutwardGatePassViewModel();
                    aRSaleOrderViewModel.AROutwardGatePass = grp;
                    aRSaleOrderViewModel.AROutwardGatePass.Approve = approve;
                    aRSaleOrderViewModel.AROutwardGatePass.Unapprove = unApprove;
                    aRSaleOrderViewModel.Date = grp.OGPDate.ToString(Helpers.CommonHelper.DateFormat);
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
            var outwardGatePassRepo = new OutwardGatePassRepo(_dbContext);
            bool isSuccess = await outwardGatePassRepo.Approve(id, userId);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Sale Return IGP has been approved successfully";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong.";
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult GetSaleReturnIGPNo(int customerId, int warehouseId)
        {
            if (customerId != 0 && warehouseId != 0)
            {
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                var saleReturnIGPNo = _dbContext.ARSaleReturnInwardGatePass.Where(x => x.CompanyId == companyId && x.CustomerId == customerId && x.WarehouseId == warehouseId && x.IsApproved == true && x.BailsBalance > 0).ToList();
                if (saleReturnIGPNo != null)
                {
                    return Ok(saleReturnIGPNo);
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
                var igp = _dbContext.ARSaleReturnInwardGatePass.FirstOrDefault(x => x.Id == Id);
                if (igp != null)
                {
                    return Ok(igp);
                }
                return Ok(null);
            }
            return Ok(null);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var outwardGatePassRepo = new OutwardGatePassRepo(_dbContext);
            bool isSuccess = await outwardGatePassRepo.Delete(id);
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
            var sROutwardGatePass = _dbContext.AROutwardGatePass
                .Include(i => i.Customer)
                .Include(i => i.Warehouse)
                .Include(i => i.SIGP)
                .Where(i => i.Id == id).FirstOrDefault();
            ViewBag.NavbarHeading = "OGP";
            ViewBag.TitleStatus = "Approved";

            return View(sROutwardGatePass);
        }
        [HttpGet]
        public IActionResult GetCustomer(int warehouseId)
        {
            if (warehouseId != 0)
            {
                var customerIds = _dbContext.ARSaleReturnInwardGatePass.Where(x => x.WarehouseId == warehouseId && x.IsApproved == true && x.BailsBalance > 0).Select(x=>x.CustomerId).ToList();
                var customers = _dbContext.ARCustomers.Where(x=> customerIds.Contains(x.Id) && x.IsActive == true && x.IsDeleted == false).ToList();
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
