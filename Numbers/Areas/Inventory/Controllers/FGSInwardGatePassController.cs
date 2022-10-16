using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Repository.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;

namespace Numbers.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    [Authorize]
    public class FGSInwardGatePassController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public FGSInwardGatePassController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Create(int? Id)
        {
            FGSInwardGatePassViewModel viewModel = new FGSInwardGatePassViewModel();
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            var api = new ApiController(_dbContext);
            var configValues = new ConfigValues(_dbContext);
            //int[] igpIds = _dbContext.GRMending.Where(x => !x.IsDeleted && x.CompanyId == companyId).Select(x => x.GRIGPId).ToArray();
            int igpId = 0;
            ViewBag.NavbarHeading = "FGS Inward Gate Pass";
            viewModel.VendorLOV = new SelectList(_dbContext.APSuppliers.Include(x => x.Account).Where(x => x.Account.Code.StartsWith("2.02.04.0006")).ToList().OrderByDescending(x => x.Id), "Id", "Name");
            ViewBag.WareHouseLOV = configValues.GetConfigValues("Inventory", "Ware House", companyId).OrderByDescending(x => x.Text.Contains("GD Sale Local"));
            ViewBag.VehicleType = configValues.GetConfigValues("AR", "Vehicle Type", companyId);
            if (Id != 0 && Id != null)
            {
                viewModel.FGSInwardGatePass = _dbContext.FGSInwardGatePasses.Where(x => x.Id == Id && x.CompanyId == companyId).FirstOrDefault();
                viewModel.FGSInwardGatePassDetails = _dbContext.FGSInwardGatePassDetails.Include(x => x.Item).Where(x => x.FGSOutwardGatePassId == viewModel.FGSInwardGatePass.Id).ToArray();
                viewModel.FGSOutwardGatePassesLOV = new SelectList(_dbContext.FGSOutwardGatePasses.Where(x => x.Id == viewModel.FGSInwardGatePass.OGPId && x.IsDeleted != true && x.IsApproved && x.CompanyId == companyId).ToList().OrderByDescending(x => x.Id), "Id", "OGPNo");

                ViewBag.EntityState = "Update";
                ViewBag.NavbarHeading = "Update FGS Inward Gate Pass";
                return View(viewModel);
            }
            viewModel.FGSOutwardGatePassesLOV = new SelectList(_dbContext.FGSOutwardGatePasses.Where(x => /*!igpIds.Contains(x.Id) &&*/ x.IsDeleted != true && x.IsApproved && x.CompanyId == companyId).ToList().OrderByDescending(x => x.Id), "Id", "OGPNo");
            return View(viewModel);
        }
        public IActionResult Index()
        {
            ViewBag.NavbarHeading = "List of FGS Inward Gate Pass";
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GetOGP(int OGPId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            List<FGSOutwardGatePassDetails> details = await _dbContext.FGSOutwardGatePassDetails
                .Include(x => x.Item)
                //.Include(x => x.Bale)
                .Where(x => x.FGSOutwardGatePassId == OGPId).ToListAsync();
            return Ok(details);
        }
        [HttpPost]
        public async Task<IActionResult> Create(FGSInwardGatePassViewModel model, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var repo = new FGSInwardGatePassRepo(_dbContext);
            FGSInwardGatePassDetail[] detail = JsonConvert.DeserializeObject<FGSInwardGatePassDetail[]>(collection["ItemDetail"]);
            if (model.FGSInwardGatePass.Id == 0)
            {
                model.FGSInwardGatePass.IGPNo = repo.Max(companyId);
                model.FGSInwardGatePass.CreatedBy = userId;
                model.FGSInwardGatePass.CompanyId = companyId;
                model.FGSInwardGatePass.Resp_Id = resp_Id;
                model.FGSInwardGatePassDetails = detail;
                bool isSuccess = await repo.Create(model);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = string.Format("IGP #. {0} has been created successfully.", model.FGSInwardGatePass.IGPNo);
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
                model.FGSInwardGatePass.UpdatedBy = userId;
                model.FGSInwardGatePass.CompanyId = companyId;
                model.FGSInwardGatePass.Resp_Id = resp_Id;
                model.FGSInwardGatePassDetails = detail;
                bool isSuccess = await repo.Update(model);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = string.Format("IGP #. {0} has been updated successfully.", model.FGSInwardGatePass.IGPNo);
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

                var searchIGPNo = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchIGPDate = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchWarehouse = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchVendor = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchOGPNo = Request.Form["columns[4][search][value]"].FirstOrDefault();
                var searchVehicleType = Request.Form["columns[5][search][value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var Data = (from records in _dbContext.FGSInwardGatePasses
                            .Include(x => x.Warehouse)
                            .Include(x => x.Vendor)
                            .Include(x => x.OGP)
                            .Include(x => x.VehicleType)
                            .Where(x => x.IsDeleted != true && x.CompanyId == companyId)
                            select records);

                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    Data = Data.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                Data = !string.IsNullOrEmpty(searchIGPNo) ? Data.Where(m => m.IGPNo.ToString().ToUpper().Contains(searchIGPNo.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchIGPDate) ? Data.Where(m => m.IGPDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchIGPDate.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchWarehouse) ? Data.Where(m => m.Warehouse.ConfigValue.ToString().ToUpper().Contains(searchWarehouse.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchVendor) ? Data.Where(m => m.Vendor.Name.ToString().ToUpper().Contains(searchVendor.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchOGPNo) ? Data.Where(m => m.OGP.OGPNo.ToString().ToUpper().Contains(searchOGPNo.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchVehicleType) ? Data.Where(m => m.VehicleType.ConfigValue.ToString().ToUpper().Contains(searchVehicleType.ToUpper())) : Data;

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

                List<FGSInwardGatePassViewModel> viewModel = new List<FGSInwardGatePassViewModel>();
                foreach (var item in data)
                {
                    FGSInwardGatePassViewModel model = new FGSInwardGatePassViewModel();
                    model.FGSInwardGatePass = item;
                    model.Date = item.IGPDate.ToString(Helpers.CommonHelper.DateFormat);
                    model.FGSInwardGatePass.Approve = approve;
                    model.FGSInwardGatePass.Unapprove = unApprove;
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
        public async Task<IActionResult> Delete(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var Repo = new FGSInwardGatePassRepo(_dbContext);
            bool isSuccess = await Repo.Delete(id, _companyId);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "IGP has been deleted successfully";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "IGP not found";
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Approve(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            FGSInwardGatePass model = await _dbContext.FGSInwardGatePasses.Where(x => x.Id == id && x.CompanyId == _companyId).FirstOrDefaultAsync();
            model.ApprovedBy = _userId;
            model.ApprovedDate = DateTime.UtcNow;
            model.IsApproved = true;
            model.Status = "Approved";
            _dbContext.FGSInwardGatePasses.Update(model);
            _dbContext.SaveChanges();
            TempData["error"] = "false";
            TempData["message"] = "IGP has been approved successfully.";
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> UnApprove(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            FGSInwardGatePass model = await _dbContext.FGSInwardGatePasses.Where(x => x.Id == id && x.CompanyId == _companyId).FirstOrDefaultAsync();
            try
            {


                //var checkCatgryRfrnc = _dbContext.GRFolding.Where(x => x.MendingId == id && x.IsDeleted != true && x.CompanyId == _companyId).ToList();
                //if (checkCatgryRfrnc.Count == 0)
                //{
                model.ApprovedBy = _userId;
                model.ApprovedDate = DateTime.UtcNow;
                model.IsApproved = false;
                model.Status = "Created";
                _dbContext.FGSInwardGatePasses.Update(model);
                _dbContext.SaveChanges();
                TempData["error"] = "false";
                TempData["message"] = "IGP has been UnApproved successfully.";

                //}
                //else
                //{
                //    TempData["error"] = "true";
                //    TempData["message"] = "Transaction No is Used in Folding..!";
                //}
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Details(int? Id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var configValues = new ConfigValues(_dbContext);
            var items = _dbContext.AppCompanySetups.Where(x => x.Name == "Local Finish ItemCode").FirstOrDefault().Value;

            FGSInwardGatePassViewModel viewModel = new FGSInwardGatePassViewModel();
            viewModel.VendorLOV = new SelectList(_dbContext.APSuppliers.Include(x => x.Account).Where(x => x.Account.Code.StartsWith("2.02.04.0006")).ToList().OrderByDescending(x => x.Id), "Id", "Name");
            ViewBag.WareHouseLOV = configValues.GetConfigValues("Inventory", "Ware House", companyId).OrderByDescending(x => x.Text.Contains("GD Sale Local"));
            ViewBag.VehicleType = configValues.GetConfigValues("AR", "Vehicle Type", companyId);

            if (Id != 0 && Id != null)
            {
                viewModel.FGSInwardGatePass = await _dbContext.FGSInwardGatePasses.Where(x => x.Id == Id && x.CompanyId == companyId).FirstOrDefaultAsync();
                viewModel.FGSInwardGatePassDetails = _dbContext.FGSInwardGatePassDetails.Include(x => x.Item).Where(x => x.FGSOutwardGatePassId == viewModel.FGSInwardGatePass.Id).ToArray();
                viewModel.FGSOutwardGatePassesLOV = new SelectList(_dbContext.FGSOutwardGatePasses.Where(x => x.Id == viewModel.FGSInwardGatePass.OGPId && x.IsDeleted != true && x.IsApproved && x.CompanyId == companyId).ToList().OrderByDescending(x => x.Id), "Id", "OGPNo");
                ViewBag.EntityState = "Update";
                ViewBag.NavbarHeading = "Details of FGS Inward Gate Pass";
                return View(viewModel);
            }


            ViewBag.EntityState = "Create";
            ViewBag.NavbarHeading = "Create FGS Outward Gate Pass";
            return View(viewModel);
        }
    }
}
