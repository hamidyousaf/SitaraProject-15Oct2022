using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Repository.Helpers;
using Numbers.Repository.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;

namespace Numbers.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    [Authorize]
    public class BOMController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public BOMController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Create(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var BOMRepo = new BOMRepository(_dbContext);
            var configValues = new ConfigValues(_dbContext);
            InvBOMViewModel invBOMViewModel = new InvBOMViewModel();
            ViewBag.Counter = 0;
            var items = _dbContext.AppCompanySetups.Where(x => x.Name == "Local Finish ItemCode").FirstOrDefault().Value;
            
            ViewBag.Nature = configValues.GetConfigValues("Inventory", "Nature", companyId);
            //ViewBag.Item = new SelectList(from ac in _dbContext.InvItems.Where(x => x.IsDeleted == false && x.CompanyId == companyId).OrderBy(x => x.Code).ToList()
            //                               select new
            //                               {
            //                                   Id = ac.Id,
            //                                   Name = ac.Code + " - " + ac.Name
            //                               }, "Id", "Name");
            if (id == 0)
            {
                ViewBag.SecondLevelCategoryLOV = new SelectList(from ac in _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.CategoryLevel == 2 && x.CompanyId == companyId && x.Code.StartsWith(items)).OrderBy(x => x.Code).ToList()
                                                                select new
                                                                {
                                                                    Id = ac.Id,
                                                                    Name = ac.Code + " - " + ac.Name
                                                                }, "Id", "Name");
                ViewBag.FourthLevelCategoryLOV = new SelectList(from c in _dbContext.InvItemCategories
                                                                where c.IsDeleted != true && c.CategoryLevel == 4 && c.Code.Contains("07.01")
                                                                select new
                                                                {
                                                                    Id = c.Id,
                                                                    Name = c.Code + " - " + c.Name
                                                                }, "Id", "Name");
                ViewBag.EntityState = "Create";
                ViewBag.NavbarHeading = "Create BOM";
                return View(invBOMViewModel);
            }
            else
            {
                invBOMViewModel.InvBOM = _dbContext.InvBOM.Where(x => x.Id == id).FirstOrDefault();
                invBOMViewModel.InvBOMDetail = _dbContext.InvBOMDetail.Include(x=>x.Item).Include(x=>x.UOM).Include(x=>x.Nature).Where(x => x.BOMId == id).ToList();
                ViewBag.SecondLevelCategoryLOV = new SelectList(from ac in _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.Id == invBOMViewModel.InvBOM.SecondItemCategoryId && x.CategoryLevel == 2 && x.CompanyId == companyId && x.Code.StartsWith(items)).OrderBy(x => x.Code).ToList()
                                                                select new
                                                                {
                                                                    Id = ac.Id,
                                                                    Name = ac.Code + " - " + ac.Name
                                                                }, "Id", "Name");
                ViewBag.FourthLevelCategoryLOV = new SelectList(from c in _dbContext.InvItemCategories

                                                                where c.Id == invBOMViewModel.InvBOM.FourthItemCategoryId && c.CategoryLevel == 4
                                                                select new
                                                                {
                                                                    Id = c.Id,
                                                                    Name = c.Code + " - " + c.Name
                                                                }, "Id", "Name");
                ViewBag.EntityState = "Update";
                ViewBag.NavbarHeading = "Update BOM";
                ViewBag.TitleStatus = "Created";
                return View(invBOMViewModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(InvBOMViewModel model, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var BOMRepo = new BOMRepository(_dbContext);
            int TransactionNo = 1;
            var list = _dbContext.InvBOM.ToList();
            if (list.Count != 0)
            {
                TransactionNo = list.Select(x => x.TransactionNo).Max() + 1;
            }
            if (model.InvBOM.Id == 0)
            {

                model.InvBOM.TransactionNo = TransactionNo;
                model.InvBOM.CreatedBy = userId;
                model.InvBOM.CompanyId = companyId;


                bool isSuccess = await BOMRepo.Create(model, collection);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = string.Format("BOM. {0} has been created successfully.", model.InvBOM.TransactionNo);
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Something went wrong.";
                }
                return RedirectToAction("Create", "BOM");
            }
            else
            {
                model.InvBOM.UpdatedBy = userId;
                model.InvBOM.CompanyId = companyId;
                


                bool isSuccess = await BOMRepo.Update(model, collection);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = string.Format("BOM. {0} has been updated successfully.", model.InvBOM.TransactionNo);
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
            List<InvBOM> list = new List<InvBOM>();
            ViewBag.NavbarHeading = "List of BOM";
            return View(list);
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
                var searchSecondCat = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchFourthCat = Request.Form["columns[3][search][value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var Data = (from records in _dbContext.InvBOM.Include(x=>x.SecondItemCategory).Include(x=>x.FourthItemCategory).Where(x => x.IsDeleted != true) select records);

                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    Data = Data.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                Data = !string.IsNullOrEmpty(searchTransNo) ? Data.Where(m => m.TransactionNo.ToString().ToUpper().Contains(searchTransNo.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchTransDate) ? Data.Where(m => m.TransactionDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchTransDate.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchSecondCat) ? Data.Where(m => m.SecondItemCategory.Name.ToString().ToUpper().Contains(searchSecondCat.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchFourthCat) ? Data.Where(m => m.FourthItemCategory.Name.ToString().ToUpper().Contains(searchFourthCat.ToUpper())) : Data;

                recordsTotal = Data.Count();
                var data = Data.Skip(skip).Take(pageSize).ToList();

                List<InvBOMViewModel> viewModel = new List<InvBOMViewModel>();
                foreach (var item in data)
                {
                    InvBOMViewModel model = new InvBOMViewModel();
                    model.InvBOM = item;
                    model.InvBOM.Approve = approve;
                    model.InvBOM.Unapprove = unApprove;
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
            InvBOM model = _dbContext.InvBOM.Find(id);
            model.ApprovedBy = _userId;
            model.ApprovedDate = DateTime.UtcNow;
            model.Status = "Approved";
            _dbContext.InvBOM.Update(model);
            _dbContext.SaveChanges();
            TempData["error"] = "false";
            TempData["message"] = "BOM has been approved successfully.";
            return RedirectToAction("Index", "BOM");
        }
        public async Task<IActionResult> Delete(int id)
        {
            var Repo = new BOMRepository(_dbContext);
            bool isSuccess = await Repo.Delete(id);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "BOM has been deleted.";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong.";
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult UnApprove(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            InvBOM model = _dbContext.InvBOM.Find(id);

            if (model != null)
            {
                model.ApprovedBy = _userId;
                model.ApprovedDate = DateTime.UtcNow;
                model.Status = "Created";
                _dbContext.InvBOM.Update(model);
                _dbContext.SaveChanges();
                TempData["error"] = "false";
                TempData["message"] = "BOM has been UnApproved.";

            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong!";
            }
            return RedirectToAction("Index", "BOM");

        }
    }
}
