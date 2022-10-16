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
    public class SaleReturnController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public SaleReturnController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var configValues = new ConfigValues(_dbContext);
            string configValue = _dbContext.AppCompanyConfigs
                                          .Where(w => w.Module == "Global" && w.ConfigName == "Report Path")
                                          .Select(c => c.ConfigValue)
                                          .FirstOrDefault();
            ViewBag.ReportPath = string.Concat(configValue, "Viewer", "?Report=SaleReturnBP&cId=", companyId, "&id=");

            ViewBag.NavbarHeading = "List of Sale Return Items";
            return View();
        }
        public IActionResult Create(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var saleReturnRepository = new SaleReturnRepository(_dbContext);
            SaleReturnViewModel saleReturnViewModel = new SaleReturnViewModel();
            var items = _dbContext.AppCompanySetups.Where(x => x.Name == "Local Finish ItemCode").FirstOrDefault().Value;

            ViewBag.SecondLevelCategory = new SelectList(from ac in _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.CategoryLevel == 2 /*&& x.CompanyId == companyId*/ && x.Code.StartsWith(items)).OrderBy(x => x.Code).ToList()
                                                         select new
                                                         {
                                                             Id = ac.Id,
                                                             Name = ac.Code + " - " + ac.Name
                                                         }, "Id", "Name");
            if (id == 0)
            {
                //SIGPViewModel.SIGPNo = SIGPRepository.SIGPMaxNo(companyId);
               // saleReturnViewModel.CustomerLOV = new SelectList(_dbContext.ARCustomers.Where(x => /*x.CompanyId == companyId &&*/ x.IsDeleted != true).ToList(), "Id", "Name");


                //var srCustomerIds = _dbContext.SaleReturn.Where(x => x.IsActive == true && x.IsDeleted == false).Select(x => x.CustomerId);
                var igpCustomerIds = _dbContext.ARInwardGatePass.Where(x => x.IsApproved == true && x.BaleBalance > 0).Select(x => x.CustomerId).ToList();
                var customers = _dbContext.ARCustomers.Where(x => igpCustomerIds.Contains(x.Id)/* && x.CompanyId == companyId*/ && x.IsActive == true && x.IsDeleted == false).ToList();
                saleReturnViewModel.CustomerLOV = new SelectList(customers.ToList(), "Id", "Name");


                ViewBag.EntityState = "Create";
                ViewBag.NavbarHeading = "Create Sale Return Items";
                return View(saleReturnViewModel);
            }
            else
            {
                saleReturnViewModel = saleReturnRepository.GetById(id);
                saleReturnViewModel.CustomerLOV = new SelectList(_dbContext.ARCustomers.Where(x => x.Id == saleReturnViewModel.CustomerId /*&& x.CompanyId == companyId*/ && x.IsDeleted != true).ToList(), "Id", "Name");
                saleReturnViewModel.Address = _dbContext.ARCustomers.FirstOrDefault(x => x.Id == saleReturnViewModel.CustomerId).Address;
                saleReturnViewModel.SaleReturnItemsList = _dbContext.SaleReturnItems.Include(x=>x.SecondLevel).Include(x=>x.ThirdLevel).Include(x=>x.FourthLevel).Where(x => x.SaleReturnId == saleReturnViewModel.Id).ToArray();
                //Handle Balance Quantity
                saleReturnViewModel.BalanceQuantity = _dbContext.ARInwardGatePass.FirstOrDefault(x=>x.Id == saleReturnViewModel.IGPId).BaleBalance;
                //----------
                ViewBag.IGPNo = new SelectList(_dbContext.ARInwardGatePass.Where(x => x.Id == saleReturnViewModel.IGPId && x.IsApproved == true).ToList(), "Id", "IGPNo");

                ViewBag.EntityState = "Update";
                ViewBag.NavbarHeading = "Update Sale Return Items";
                ViewBag.TitleStatus = "Created";
                return View(saleReturnViewModel);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Create(SaleReturnViewModel model, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var saleReturnRepository = new SaleReturnRepository(_dbContext);
            SaleReturnItems[] saleReturnItems = JsonConvert.DeserializeObject<SaleReturnItems[]>(collection["ItemDetail"]);
            if (model.Id == 0)
            {
                model.CreatedBy = userId;
                model.CompanyId = companyId;
                model.Resp_ID = resp_Id;
                model.TotalMeters = Convert.ToInt32(collection["tMeters"]);
                model.TotalBales =Math.Abs(Convert.ToDecimal(collection["tBales"]));
                model.BuiltyNo = collection["builtyNo"];
                model.Bails =Math.Abs(Convert.ToDecimal(collection["bails"]));
                model.TotalBales =Math.Abs( Convert.ToDecimal(collection["tBales"]));
                model.BalanceQuantity =Math.Abs( Convert.ToDecimal(collection["balancebails"]));
                model.TransactionNo = saleReturnRepository.MaxNo(companyId);
                model.SaleReturnItemsList = saleReturnItems;
                bool isSuccess = await saleReturnRepository.Create(model);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = string.Format("Sale Return Items. {0} has been created successfully.", model.TransactionNo);
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Something went wrong.";
                }
                return RedirectToAction("Create", "SaleReturn");
            }
            else
            {
                model.UpdatedBy = userId;
                model.CompanyId = companyId;
                model.Resp_ID = resp_Id;
                model.SaleReturnItemsList = saleReturnItems;
                bool isSuccess = await saleReturnRepository.Update(model);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = string.Format("Sale Return Items. {0} has been updated successfully.", model.TransactionNo);
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Something went wrong.";
                }
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
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
                var Data = (from SaleReturn in _dbContext.SaleReturn.Include(x => x.IGP).Include(x=>x.Customer).Where(x => x.IsDeleted == false && x.CompanyId == companyId) select SaleReturn);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    Data = Data.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                Data = !string.IsNullOrEmpty(searchTransNo) ? Data.Where(m => m.TransactionNo.ToString().ToLower().Contains(searchTransNo.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchSRDate) ? Data.Where(m => m.SRDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchSRDate.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchCustomer) ? Data.Where(m => m.Customer.Name.ToString().ToLower().Contains(searchCustomer.ToLower())) : Data;
                Data = !string.IsNullOrEmpty(searchIGPNo) ? Data.Where(m => m.IGP.IGPNo.ToString().Contains(searchIGPNo.ToUpper())) : Data;
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
                List<SaleReturnViewModel> Details = new List<SaleReturnViewModel>();
                foreach (var grp in data)
                {
                    SaleReturnViewModel saleReturnViewModel = new SaleReturnViewModel();
                    saleReturnViewModel.SaleReturn = grp;
                    saleReturnViewModel.SaleReturn.Approve = approve;
                    saleReturnViewModel.SaleReturn.Unapprove = unApprove;
                    saleReturnViewModel.Date = grp.SRDate.ToString(Helpers.CommonHelper.DateFormat);
                    Details.Add(saleReturnViewModel);
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
            var saleReturnRepository = new SaleReturnRepository(_dbContext);
            bool isSuccess = await saleReturnRepository.Approve(id, userId);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Sale Retrn Item has been approved successfully";
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
                return Ok("Not Available");
            }
            return Ok("Not Available");
        }
        [HttpGet]
        public IActionResult GetIGPNo(int Id)
        {
            if (Id != 0)
            {
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                var igpNo = _dbContext.ARInwardGatePass.Where(x => x.CompanyId == companyId && x.CustomerId == Id && x.IsApproved==true && x.BaleBalance > 0).ToList();
                if (igpNo != null)
                {
                    return Ok(igpNo);
                }
                return Ok(null);
            }
            return Ok(null);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var saleReturnRepository = new SaleReturnRepository(_dbContext);
            bool isSuccess = await saleReturnRepository.Delete(id);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Sale Return has been deleted successfully.";
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
            var saleReturn = _dbContext.SaleReturn
                .Include(i => i.Customer)
                .Include(i => i.IGP)
            .Where(i => i.Id == id).FirstOrDefault();
            var saleReturnItems = _dbContext.SaleReturnItems
                                .Include(i => i.SecondLevel)
                                .Include(i => i.ThirdLevel)
                                .Include(i => i.FourthLevel)
                                .Where(i => i.SaleReturnId == id)
                                .ToList();
            ViewBag.TotalMeter = saleReturnItems.Sum(x=>x.Meters);
            ViewBag.TotalBales = saleReturnItems.Sum(x=>x.Bales);
            ViewBag.NavbarHeading = "Sale Return Items";
            ViewBag.TitleStatus = "Approved";

            TempData["Detail"] = saleReturnItems;
            return View(saleReturn);
        }
    }
}
