using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Repository.Helpers;
using Numbers.Repository.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Numbers.Areas.Greige.Controllers
{
    [Area("Greige")]
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        private readonly int _level1, _level2, _level3, _level4;
        private readonly string _splitter;

        public CategoryController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
            _level1 = 1;
            _level2 = 2;
            _level3 = 2;
            _level4 = 4;
            _splitter = ".";
           
        }
        public  IActionResult Index(int Id)
        {
            GRCategory bale = new GRCategory();
            var configValues = new ConfigValues(_dbContext);
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
 
            if (Id !=0)
            {
                  bale = _dbContext.GRCategory.Where(x => x.Id==Id ).FirstOrDefault();
               
            }
            
            return View(bale);
        }

        public IActionResult Details(int Id)
        {
            GRCategory bale = new GRCategory();
            var configValues = new ConfigValues(_dbContext);
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;

            if (Id != 0)
            {
                bale = _dbContext.GRCategory.Where(x => x.Id == Id ).FirstOrDefault();

            }

            return View(bale);
        }


        public IActionResult GetCategoryList()
        {
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            string userId = HttpContext.Session.GetString("UserId");
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;

            var approve = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == resp_Id).FirstOrDefault().Approve;
            var unApprove = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == resp_Id).FirstOrDefault().UnApprove;

            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            //var searchValue = Request.Form["search[value]"].FirstOrDefault();
            var searchCode = Request.Form["columns[1][search][value]"].FirstOrDefault();
            var searchDate = Request.Form["columns[2][search][value]"].FirstOrDefault();
            var searchMinThreads = Request.Form["columns[3][search][value]"].FirstOrDefault();
            var searchMaxThreads = Request.Form["columns[4][search][value]"].FirstOrDefault();
            var searchMinWidth = Request.Form["columns[5][search][value]"].FirstOrDefault();
            var searchMaxWidth = Request.Form["columns[6][search][value]"].FirstOrDefault();
            var searchDescription = Request.Form["columns[7][search][value]"].FirstOrDefault();






            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;
            var CostCenterData = _dbContext.GRCategory.Where(x => x.IsDeleted == false);
            if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
            {
                CostCenterData = CostCenterData.OrderBy(sortColumn + " " + sortColumnDirection);
            }

            CostCenterData = !string.IsNullOrEmpty(searchCode) ? CostCenterData.Where(m => m.TransactionNo.ToString().Contains(searchCode)) : CostCenterData;
            CostCenterData = !string.IsNullOrEmpty(searchDate) ? CostCenterData.Where(m => m.TransactionDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchDate.ToUpper())) : CostCenterData;
            CostCenterData = !string.IsNullOrEmpty(searchMinThreads) ? CostCenterData.Where(m => m.MinThreads.ToString().Contains(searchMinThreads.ToUpper())) : CostCenterData;
            CostCenterData = !string.IsNullOrEmpty(searchMaxThreads) ? CostCenterData.Where(m => m.MaxThreads.ToString().Contains(searchMaxThreads.ToUpper())) : CostCenterData;
            CostCenterData = !string.IsNullOrEmpty(searchMinWidth) ? CostCenterData.Where(m => m.MinWidth.ToString().Contains(searchMinWidth.ToUpper())) : CostCenterData;
            CostCenterData = !string.IsNullOrEmpty(searchMaxWidth) ? CostCenterData.Where(m => m.MaxWidth.ToString().Contains(searchMaxWidth.ToUpper())) : CostCenterData;
            CostCenterData = !string.IsNullOrEmpty(searchDescription) ? CostCenterData.Where(m => m.Description.ToString().Contains(searchDescription.ToUpper())) : CostCenterData;
            
            //recordsTotal = CostCenterData.Count();

            recordsTotal = CostCenterData.Count();
            var data = CostCenterData.ToList();
            if (pageSize == -1)
            {
                data = CostCenterData.Skip(skip).Take(recordsTotal == 0 ? 1 : pageSize == -1 ? recordsTotal : 1).ToList();
            }
            else
            {
                data = CostCenterData.Skip(skip).Take(pageSize).ToList();
            }


            //var data = CostCenterData.Skip(skip).Take(pageSize).ToList();

            List<GRCategory> details = new List<GRCategory>();
            foreach (var item in data)
            {
                var category = new GRCategory();
                category.UpdatedBy = item.TransactionDate.ToString(Helpers.CommonHelper.DateFormat);
                category.category = item;
                category.category.Approve = approve;
                category.category.Unapprove = unApprove;
                details.Add(category);

            }
            var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = details.OrderByDescending(x=>x.TransactionNo), };
            return Ok(jsonData);
        }
        [HttpPost]
        public async  Task<IActionResult> Create(GRCategory model)
        {
            
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            string userId = HttpContext.Session.GetString("UserId");
           
            if (model.Id == 0)
            {
                int TransactionNo = 1;
                var list = _dbContext.GRCategory.ToList();
                if(list.Count!=0)
                {
                    TransactionNo =list.Select(x => x.TransactionNo).Max() + 1;
                }
                model.TransactionNo = TransactionNo;
                model.CreatedBy = userId;
                model.CreatedDate = DateTime.Now;
                model.Description = string.Concat("Threads (",model.MinThreads, "-", model.MaxThreads,") Width (",model.MinWidth, "-", model.MaxWidth,")");
                bool istrue =  CheckCategory(model.Description);
                if (!istrue)
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Greige Category already exist!";
                    return RedirectToAction("Index"); 
                }
               
                _dbContext.GRCategory.Add(model);
                await _dbContext.SaveChangesAsync();
                TempData["error"] = "false";
                TempData["message"] = "Greige Category has been saved successfully.";              
            }
            else
            {
                var Category = new GRCategory();
                Category.Id = model.Id;
                Category.TransactionNo = model.TransactionNo;
                Category.TransactionDate = model.TransactionDate;
                Category.MinThreads = model.MinThreads;
                Category.MaxThreads = model.MaxThreads;
                Category.MinWidth = model.MinWidth;
                Category.MaxWidth = model.MaxWidth;
                Category.ItemCategoryId = model.ItemCategoryId;
                Category.Description = string.Concat("Threads (", model.MinThreads, "-", model.MaxThreads, ") Width (", model.MinWidth, "-", model.MaxWidth, ")");
                Category.IsDeleted = false;
                Category.CompanyId = companyId;
                Category.UpdatedBy = userId;
                Category.UpdatedDate = DateTime.Now;
                _dbContext.GRCategory.Update(Category);
              await  _dbContext.SaveChangesAsync();
                TempData["error"] = "false";
                TempData["message"] = "Greige Category has been update successfully.";
            }
            return RedirectToAction("Index");
        }

        public async Task<InvItemCategories> CreateCategory(string desc)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            var ParentCode = _dbContext.AppCompanySetups.Where(x => x.Name == "Greige Category Parent Code").FirstOrDefault().Value;
            var categoryRepo = new CategoryRepo(_dbContext);
            InvItemCategories model = new InvItemCategories();
            var parentId = _dbContext.InvItemCategories.Where(x=>x.Code ==ParentCode && x.CategoryLevel ==2).FirstOrDefault().Id;
            model = CreateGreigeChild(parentId);
           
            if (model != null)
            {
                model.Code = model.ParentCode + "." + model.Code;
                model.Status = "Child";
                model.IsActive = true;
                model.Name = desc;
                model.CompanyId = _companyId;
                model.CreatedBy = _userId;
                model.CreatedDate = DateTime.Now;
                _dbContext.InvItemCategories.Add(model);
                await _dbContext.SaveChangesAsync();
                return model;
            }else
            {
                return model;
            }
            
        }

        public InvItemCategories CreateGreigeChild(int id)
        {
            ViewBag.EntityState = "Create";
            ViewBag.Heading = "Create Child";
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            InvItemCategories itemCategories = new InvItemCategories();
            var parentAccount = _dbContext.InvItemCategories.Where(a => a.Id == id && a.IsDeleted == false).FirstOrDefault(); //get parentId
            if (parentAccount == null)
                return itemCategories;

            //get last account code
            var account = _dbContext.InvItemCategories
                        .Where(a => a.ParentId == id && a.CompanyId == companyId && a.IsDeleted == false)
                        .OrderByDescending(a => a.Code)
                        .FirstOrDefault();
            string newCode;
            int newCodeLength;
            short newAccountLevel = (short)(parentAccount.CategoryLevel + 1);
            if (newAccountLevel == 1)
                newCodeLength = _level1;
            else if (newAccountLevel == 2)
                newCodeLength = _level2;
            else if (newAccountLevel == 3)
                newCodeLength = _level3;
            else if (newAccountLevel == 4)
                newCodeLength = _level4;
            else
            {
                newCodeLength = _level4;
                return itemCategories;
            }
            if (account == null) //means no child row, start with 1
            {
                newCode = "01";
                if (newCodeLength != 4)
                    newCode = newCode.PadLeft(newCodeLength, '0');
                // newCode = newCode.Substring(newCode.Length, newCodeLength);
            }
            else  //increment 1 into last code
            {
                if (newCodeLength == 4)
                {
                    newCodeLength = 10;
                    newCode = account.Code.Substring(10);
                    int c = Convert.ToInt16(newCode) + 1;
                    newCode = c.ToString();
                    newCode = newCode.PadLeft(2, '0');
                }
                else
                {
                    newCode = account.Code.Substring(account.Code.Length - newCodeLength, newCodeLength);
                    int c = Convert.ToInt16(newCode) + 1;
                    newCode = c.ToString();
                    newCode = newCode.PadLeft(newCodeLength, '0');
                }

            }
            //var account = _dbContext.GLAccounts.Where(a => a.Id == account.ParentId && a.IsDeleted == false).FirstOrDefault();
            ViewBag.Title = "Create account";
            ViewBag.Action = "Create";
            ViewBag.Level1 = _level1;
            ViewBag.Level2 = _level2;
            ViewBag.Level3 = _level3;
            ViewBag.Level4 = _level4;
            ViewBag.Splitter = _splitter;

            itemCategories.ParentId = id;
            itemCategories.CategoryLevel = newAccountLevel;
            itemCategories.ParentCode = parentAccount.Code;
            itemCategories.Code = newCode;
            itemCategories.Name = "";
            itemCategories.Id = 0;
            return itemCategories;
        }


        public bool CheckCategory(string desc)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            GRCategory model = _dbContext.GRCategory.Where(x => x.Description == desc ).FirstOrDefault();

            if (model != null)
            {
                return false;

            }
            return true;
        }

        public async Task<InvItemCategories> UpdateCategory(GRCategory model)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            var invItemCategory = _dbContext.InvItemCategories.Where(x => x.Id == model.ItemCategoryId).FirstOrDefault();
            if (invItemCategory != null)
            {
                invItemCategory.Name = model.Description;
                _dbContext.InvItemCategories.Update(invItemCategory);
            }

            return invItemCategory;
        }

        public async Task<IActionResult> Approve(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            GRCategory model = _dbContext.GRCategory.Where(x=>x.Id==id ).FirstOrDefault();
            model.ApprovedBy = _userId;
            model.ApprovedDate = DateTime.UtcNow;
            model.IsApproved = true;
            model.Status = "Approved";
            if (model.ItemCategoryId == null || model.ItemCategoryId == 0)
            {
                var istrueCategory = await CreateCategory(model.Description);
                if (istrueCategory == null)
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Main Category Not Created!";
                    return RedirectToAction("Index");
                }
                model.ItemCategoryId = istrueCategory.Id;
            }
            else
            {
               await UpdateCategory(model);
            }
             _dbContext.GRCategory.Update(model);
            _dbContext.SaveChanges();
            TempData["error"] = "false";
            TempData["message"] = "Greige Category has been approved successfully.";
            return RedirectToAction("Index", "Category");
        }
        public IActionResult UnApprove(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            GRCategory model = _dbContext.GRCategory.Where(x => x.Id == id ).FirstOrDefault();

            var checkCatgryRfrnc = (from a in _dbContext.GRQuality where a.GRCategoryId == id select a.GRCategoryId).ToList();

            if (checkCatgryRfrnc.Count==0)
            {

               
                model.ApprovedBy = _userId;
                model.ApprovedDate = DateTime.UtcNow;
                model.IsApproved = false;
                model.Status = "Created";
                _dbContext.GRCategory.Update(model);
                _dbContext.SaveChanges();
                TempData["error"] = "false";
                TempData["message"] = "Category has been UnApproved successfully";

            }

            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Transaction No is Used in Greige Quality..!";


            }


            //model.ApprovedBy = _userId;
            //model.ApprovedDate = DateTime.UtcNow;
            //model.IsApproved = false;
            //model.Status = "Created";
            //_dbContext.GRCategory.Update(model);
            //_dbContext.SaveChanges();
            //TempData["error"] = "false";
            //TempData["message"] = "Category has been UnApproved successfully.";
            return RedirectToAction("Index", "Category");
        }






        public IActionResult Delete(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;

            // var Category = new GRCategory { Id = id }; 
            var Category = new GRCategory { Id = id }; 


            if (Category != null)
            {
                _dbContext.GRCategory.Attach(Category);
                _dbContext.GRCategory.Remove(Category);
                _dbContext.SaveChanges();

                TempData["error"] = "false";
                TempData["message"] = "Greige Category has been deleted successfully.";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong.";
            }
            return RedirectToAction("Index");
        }
        
    }
}
