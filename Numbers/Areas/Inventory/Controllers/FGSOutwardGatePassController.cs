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
    public class FGSOutwardGatePassController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public FGSOutwardGatePassController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Create(int? Id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var configValues = new ConfigValues(_dbContext);
            var items = _dbContext.AppCompanySetups.Where(x => x.Name == "Local Finish ItemCode").FirstOrDefault().Value;

            FGSOutwardGatePassViewModel viewModel = new FGSOutwardGatePassViewModel();
            ViewBag.WareHouseLOV = configValues.GetConfigValues("Inventory", "Ware House", companyId).OrderByDescending(x => x.Text.Contains("GD Sale Local"));
            ViewBag.Customer = new SelectList((_dbContext.ARCustomers.Where(x => x.CompanyId == companyId)).ToList(), "Id", "Name");
            ViewBag.SecondLevelCategoryLOV = new SelectList(from ac in _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.CategoryLevel == 2 && x.Code.StartsWith(items)).OrderBy(x => x.Code).ToList()
                                                            select new
                                                            {
                                                                Id = ac.Id,
                                                                Name = ac.Code + " - " + ac.Name
                                                            }, "Id", "Name");

            if (Id != 0 && Id != null)
            {
                viewModel.FGSOutwardGatePass = _dbContext.FGSOutwardGatePasses.Where(x => x.Id == Id && x.CompanyId == companyId).FirstOrDefault();
                viewModel.FGSOutwardGatePassDetails = _dbContext.FGSOutwardGatePassDetails.Include(x => x.Item).Where(x => x.FGSOutwardGatePassId == viewModel.FGSOutwardGatePass.Id).ToArray();
                ViewBag.FourthCategoryLOV = new SelectList(from ac in _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.CategoryLevel == 4 && x.Id == viewModel.FGSOutwardGatePass.FourthItemCategoryId && x.Code.StartsWith(items)).OrderBy(x => x.Code).ToList()
                                                                select new
                                                                {
                                                                    Id = ac.Id,
                                                                    Name = ac.Code + " - " + ac.Name
                                                                }, "Id", "Name");
                ViewBag.EntityState = "Update";
                ViewBag.NavbarHeading = "Update FGS Outward Gate Pass";
                return View(viewModel);
            }


            ViewBag.EntityState = "Create";
            ViewBag.NavbarHeading = "Create FGS Outward Gate Pass";
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(FGSOutwardGatePassViewModel model, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var repo = new FGSOutwardGatePassRepo(_dbContext);
            FGSOutwardGatePassDetails[] detail = JsonConvert.DeserializeObject<FGSOutwardGatePassDetails[]>(collection["ItemDetail"]);
            if (model.FGSOutwardGatePass.Id == 0)
            {
                model.FGSOutwardGatePass.OGPNo = repo.Max(companyId);
                model.FGSOutwardGatePass.CreatedBy = userId;
                model.FGSOutwardGatePass.CompanyId = companyId;
                model.FGSOutwardGatePass.Resp_ID = resp_Id;
                model.FGSOutwardGatePassDetails = detail;
                bool isSuccess = await repo.Create(model);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = string.Format("OGP #. {0} has been created successfully.", model.FGSOutwardGatePass.OGPNo);
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
                model.FGSOutwardGatePass.UpdatedBy = userId;
                model.FGSOutwardGatePass.CompanyId = companyId;
                model.FGSOutwardGatePass.Resp_ID = resp_Id;
                model.FGSOutwardGatePassDetails = detail;
                bool isSuccess = await repo.Update(model);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = string.Format("OGP #. {0} has been updated successfully.", model.FGSOutwardGatePass.OGPNo);
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
            ViewBag.NavbarHeading = "List of FGS Outward Gate Pass";
            return View();
        }
        //[HttpPost]
        public IActionResult GetBales(int[] skipIds, int fourthCategoryId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            List<BaleInformation> bales = _dbContext.BaleInformation.Include(x => x.Item)
                .Where(x => !skipIds.Contains(x.Id)  && x.ItemCategory4 == fourthCategoryId).OrderByDescending(x => x.Id).ToList();
            return Ok(bales);
        }
        [HttpGet]
        public async Task<IActionResult> GetBalesById(int baleId, int fouthLevelId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            BaleInformation bale = await _dbContext.BaleInformation.Include(x => x.Item)
                .FirstOrDefaultAsync(x =>  x.Id == baleId && x.ItemCategory4 == fouthLevelId);
            return Ok(bale);
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

                var searchOGPNo = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchOGPDate = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchWarehouse = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchCustomer = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchSecondItemCategory = Request.Form["columns[4][search][value]"].FirstOrDefault();
                var searchFourthItemCategory = Request.Form["columns[5][search][value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var Data = (from records in _dbContext.FGSOutwardGatePasses
                            .Include(x => x.Warehouse)
                            .Include(x => x.Customer)
                            .Include(x => x.SecondItemCategory)
                            .Include(x => x.FourthItemCategory)
                            .Where(x => x.IsDeleted != true && x.CompanyId == companyId) select records);

                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    Data = Data.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                Data = !string.IsNullOrEmpty(searchOGPNo) ? Data.Where(m => m.OGPNo.ToString().ToUpper().Contains(searchOGPNo.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchOGPDate) ? Data.Where(m => m.OGPDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchOGPDate.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchWarehouse) ? Data.Where(m => m.Warehouse.ConfigValue.ToString().ToUpper().Contains(searchWarehouse.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchCustomer) ? Data.Where(m => m.Customer.Name.ToString().ToUpper().Contains(searchCustomer.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchSecondItemCategory) ? Data.Where(m => m.SecondItemCategory.Name.ToString().ToUpper().Contains(searchSecondItemCategory.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchFourthItemCategory) ? Data.Where(m => m.FourthItemCategory.Name.ToString().ToUpper().Contains(searchFourthItemCategory.ToUpper())) : Data;

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

                List<FGSOutwardGatePassViewModel> viewModel = new List<FGSOutwardGatePassViewModel>();
                foreach (var item in data)
                {
                    FGSOutwardGatePassViewModel model = new FGSOutwardGatePassViewModel();
                    model.FGSOutwardGatePass = item;
                    model.Date = item.OGPDate.ToString(Helpers.CommonHelper.DateFormat);
                    model.FGSOutwardGatePass.Approve = approve;
                    model.FGSOutwardGatePass.Unapprove = unApprove;
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
            var Repo = new FGSOutwardGatePassRepo(_dbContext);
            bool isSuccess = await Repo.Delete(id, _companyId);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "OGP #. has been deleted successfully";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "OGP #. not found";
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Approve(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            FGSOutwardGatePass model = _dbContext.FGSOutwardGatePasses.Where(x => x.Id == id && x.CompanyId == _companyId).FirstOrDefault();
            model.ApprovedBy = _userId;
            model.ApprovedDate = DateTime.UtcNow;
            model.IsApproved = true;
            model.Status = "Approved";
            _dbContext.FGSOutwardGatePasses.Update(model);
            _dbContext.SaveChanges();
            TempData["error"] = "false";
            TempData["message"] = "OGP #. has been approved successfully.";
            return RedirectToAction(nameof(Index));
        }
        public IActionResult UnApprove(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            FGSOutwardGatePass model = _dbContext.FGSOutwardGatePasses.Where(x => x.Id == id && x.CompanyId == _companyId).FirstOrDefault();
            try
            {


                var checkCatgryRfrnc = _dbContext.FGSInwardGatePasses.Where(x => x.OGPId == id && x.IsDeleted != true && x.CompanyId == _companyId).ToList();
                if (checkCatgryRfrnc.Count == 0)
                {
                    model.ApprovedBy = _userId;
                    model.ApprovedDate = DateTime.UtcNow;
                    model.IsApproved = false;
                    model.Status = "Created";
                    _dbContext.FGSOutwardGatePasses.Update(model);
                    _dbContext.SaveChanges();
                    TempData["error"] = "false";
                    TempData["message"] = "OGP #. has been UnApproved successfully.";

                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Transaction is Used in IGP..!";
                }
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(Index));
            }


            return RedirectToAction(nameof(Index));


        }
        public IActionResult Details(int? Id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var configValues = new ConfigValues(_dbContext);
            var items = _dbContext.AppCompanySetups.Where(x => x.Name == "Local Finish ItemCode").FirstOrDefault().Value;

            FGSOutwardGatePassViewModel viewModel = new FGSOutwardGatePassViewModel();
            ViewBag.WareHouseLOV = configValues.GetConfigValues("Inventory", "Ware House", companyId).OrderByDescending(x => x.Text.Contains("GD Sale Local"));
            ViewBag.Customer = new SelectList((_dbContext.ARCustomers.Where(x => x.CompanyId == companyId)).ToList(), "Id", "Name");
            ViewBag.SecondLevelCategoryLOV = new SelectList(from ac in _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.CategoryLevel == 2 && x.Code.StartsWith(items)).OrderBy(x => x.Code).ToList()
                                                            select new
                                                            {
                                                                Id = ac.Id,
                                                                Name = ac.Code + " - " + ac.Name
                                                            }, "Id", "Name");

            if (Id != 0 && Id != null)
            {
                viewModel.FGSOutwardGatePass = _dbContext.FGSOutwardGatePasses.Where(x => x.Id == Id && x.CompanyId == companyId).FirstOrDefault();
                viewModel.FGSOutwardGatePassDetails = _dbContext.FGSOutwardGatePassDetails.Include(x => x.Item).Where(x => x.FGSOutwardGatePassId == viewModel.FGSOutwardGatePass.Id).ToArray();
                ViewBag.FourthCategoryLOV = new SelectList(from ac in _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.CategoryLevel == 4 && x.Id == viewModel.FGSOutwardGatePass.FourthItemCategoryId && x.Code.StartsWith(items)).OrderBy(x => x.Code).ToList()
                                                           select new
                                                           {
                                                               Id = ac.Id,
                                                               Name = ac.Code + " - " + ac.Name
                                                           }, "Id", "Name");
                ViewBag.EntityState = "Update";
                ViewBag.NavbarHeading = "Update FGS Outward Gate Pass";
                return View(viewModel);
            }


            ViewBag.EntityState = "Create";
            ViewBag.NavbarHeading = "Create FGS Outward Gate Pass";
            return View(viewModel);
        }
    }
}
