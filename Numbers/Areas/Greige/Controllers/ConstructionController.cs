using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Repository.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Numbers.Areas.Greige.Controllers
{
    [Area("Greige")]
    [Authorize]
    public class ConstructionController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        private readonly int _level1, _level2, _level3, _level4;
        private readonly string _splitter;
    
        public ConstructionController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbContext = dbContext;
            _level1 = 1;
            _level2 = 2;
            _level3 = 2;
            _level4 = 4;
            _splitter = ".";
          
        }
        public  IActionResult Index(int Id)
        {
            GRConstruction bale = new GRConstruction();
           int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            ViewBag.YarnItems = new SelectList(_dbContext.InvItems.Where(x => x.IsDeleted != true).ToList(), "Id", "Name");
            bale.GRCategoryLOV = new SelectList(_dbContext.GRCategory.Where(x => x.IsDeleted != true && x.Status == "Approved").ToList(), "Id", "Description");


            if (Id !=0)
            {
                  bale = _dbContext.GRConstruction.Where(x => x.Id==Id ).FirstOrDefault();
                bale.GRCategoryLOV = new SelectList(_dbContext.GRCategory.Where(x => x.IsDeleted != true && x.Status == "Approved").ToList(), "Id", "Description");

            }
            
            return View(bale);
        }


        public IActionResult Details(int Id)
        {
            GRConstruction bale = new GRConstruction();
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            ViewBag.YarnItems = new SelectList(_dbContext.InvItems.Where(x => x.IsDeleted != true).ToList(), "Id", "Name");


            if (Id != 0)
            {
                bale = _dbContext.GRConstruction.Where(x => x.Id == Id ).FirstOrDefault();

            }

            return View(bale);
        }


        public IActionResult GetConstructionList()
       {
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            string userId = HttpContext.Session.GetString("UserId");
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
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
            var searchReed = Request.Form["columns[3][search][value]"].FirstOrDefault();
            var searchPick = Request.Form["columns[4][search][value]"].FirstOrDefault();
            var searchWarp = Request.Form["columns[5][search][value]"].FirstOrDefault();
            var searchWeft = Request.Form["columns[6][search][value]"].FirstOrDefault();
            var searchThreads = Request.Form["columns[7][search][value]"].FirstOrDefault();
            var searchDescription = Request.Form["columns[8][search][value]"].FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;
            var CostCenterData = _dbContext.GRConstruction.Include(x => x.Warp).Include(x => x.Weft).Where(x => x.IsDeleted == false );
            if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
            {
                CostCenterData = CostCenterData.OrderBy(sortColumn + " " + sortColumnDirection);
            }
            
            CostCenterData = !string.IsNullOrEmpty(searchCode) ? CostCenterData.Where(m => m.Id.ToString().Contains(searchCode)) : CostCenterData;
            CostCenterData = !string.IsNullOrEmpty(searchDate) ? CostCenterData.Where(m => m.TransactionDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchDate.ToUpper())) : CostCenterData;
            CostCenterData = !string.IsNullOrEmpty(searchReed) ? CostCenterData.Where(m => m.Reed.ToString().ToUpper().Contains(searchReed.ToUpper())) : CostCenterData;
            CostCenterData = !string.IsNullOrEmpty(searchPick) ? CostCenterData.Where(m => m.Pick.ToString().ToUpper().Contains(searchPick.ToUpper())) : CostCenterData;
            CostCenterData = !string.IsNullOrEmpty(searchWarp) ? CostCenterData.Where(m => m.Warp.Name.ToString().ToUpper().Contains(searchWarp.ToUpper())) : CostCenterData;
            CostCenterData = !string.IsNullOrEmpty(searchWeft) ? CostCenterData.Where(m => m.Weft.Name.ToString().ToUpper().Contains(searchWeft.ToUpper())) : CostCenterData;
            CostCenterData = !string.IsNullOrEmpty(searchThreads) ? CostCenterData.Where(m => m.Threads.ToString().ToUpper().Contains(searchThreads.ToUpper())) : CostCenterData;
            CostCenterData = !string.IsNullOrEmpty(searchDescription) ? CostCenterData.Where(m => m.Description.ToString().ToUpper().Contains(searchDescription.ToUpper())) : CostCenterData;

           // recordsTotal = CostCenterData.Count();

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


           // var data = CostCenterData.Skip(skip).Take(pageSize).ToList();
            List<GRConstruction> details = new List<GRConstruction>();
           foreach(var item in data)
            {
                var construction = new GRConstruction();
                construction.UpdatedBy = item.TransactionDate.ToString(Helpers.CommonHelper.DateFormat);
                construction.construction = item;
                construction.construction.Approve = approve;
                construction.construction.Unapprove = unApprove;
                details.Add(construction);
            }
            var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = details.OrderByDescending(x=>x.TransactionNo), };
            return Ok(jsonData);
        }
        [HttpPost]
        public async  Task<IActionResult> Create(GRConstruction model)
        {
            
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var items = _dbContext.InvItems.Where(x => x.IsDeleted == false).ToList();
            if (model.Id == 0)
            {
                int TransactionNo = 1;
                var list = _dbContext.GRConstruction.ToList();
                if(list.Count!=0)
                {
                    TransactionNo =list.Select(x => x.TransactionNo).Max() + 1;
                }
                model.TransactionNo = TransactionNo;
                model.CreatedBy = userId;
                model.CreatedDate = DateTime.Now;
                model.Threads = model.Reed + model.Pick;
                model.Description = string.Concat(model.Reed, "X", model.Pick,"/", items.Where(x=>x.Id==model.WarpId).Select(x=>x.Name).FirstOrDefault(), "X", items.Where(x => x.Id == model.WeftId).Select(x => x.Name).FirstOrDefault());
                bool istrue = CheckConstrunction(model.Description, model.GRCategoryId);
                if (!istrue)
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Construction already exist!";
                    return RedirectToAction("Index");
                }
                _dbContext.GRConstruction.Add(model);
                await _dbContext.SaveChangesAsync();
                TempData["error"] = "false";
                TempData["message"] = "Greige Construction has been saved successfully.";              
            }
            else
            {
                var construction = new GRConstruction();
                construction.Id = model.Id;
                construction.TransactionNo = model.TransactionNo;
                construction.TransactionDate = model.TransactionDate;
                construction.Reed = model.Reed;
                construction.Pick = model.Pick;
                construction.WarpId = model.WarpId;
                construction.WeftId = model.WeftId;
                construction.GRCategoryId = model.GRCategoryId;
                construction.ItemCategoryId = model.ItemCategoryId;
                construction.Threads = model.Reed + model.Pick;
                construction.Description = string.Concat(model.Reed, "X", model.Pick, "/", items.Where(x => x.Id == model.WarpId).Select(x => x.Name).FirstOrDefault(), "X", items.Where(x => x.Id == model.WeftId).Select(x => x.Name).FirstOrDefault());
                construction.IsDeleted = false;
                construction.CompanyId = companyId;
                construction.UpdatedBy = userId;
                construction.UpdatedDate = DateTime.Now;
                _dbContext.GRConstruction.Update(construction);
              await  _dbContext.SaveChangesAsync();
                TempData["error"] = "false";
                TempData["message"] = "Greige Greige Construction has been update successfully.";
            }
            return RedirectToAction("Index");
        }

        public bool CheckConstrunction(string desc, int GRCategoryId)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            GRConstruction model = _dbContext.GRConstruction.Where(x=>x.Description==desc && x.GRCategoryId == GRCategoryId ).FirstOrDefault();

            if (model != null)
            {
                return false;

            }
            return true;
        }

        public async Task<IActionResult> Approve(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            GRConstruction model = _dbContext.GRConstruction.Where(x=>x.Id==id ).FirstOrDefault();
           
            model.ApprovedBy = _userId;
            model.ApprovedDate = DateTime.UtcNow;
            model.IsApproved = true;
            model.Status = "Approved";
            if (model.ItemCategoryId == null || model.ItemCategoryId == 0)
            {
                var istrueCategory = await CreateCategory(model);
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
            _dbContext.GRConstruction.Update(model);
            _dbContext.SaveChanges();
            TempData["error"] = "false";
            TempData["message"] = "Constrcution has been approved successfully.";
            return RedirectToAction("Index", "Construction");
        }
        public IActionResult UnApprove(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            GRConstruction model = _dbContext.GRConstruction.Where(x => x.Id == id ).FirstOrDefault();
            var checkCatgryRfrnc = (from a in _dbContext.GRQuality where a.GRConstructionId == id  select a.GRConstructionId).ToList();
            if (checkCatgryRfrnc.Count==0)
            {
                //TempData["error"] = "false";
                //TempData["message"] = "Construction has been UnApproved successfully.";
                model.ApprovedBy = _userId;
                model.ApprovedDate = DateTime.UtcNow;
                model.IsApproved = false;
                model.Status = "Created";
                _dbContext.GRConstruction.Update(model);
                _dbContext.SaveChanges();
                TempData["error"] = "false";
                TempData["message"] = "Construction has been UnApproved successfully.";
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
            //_dbContext.GRConstruction.Update(model);
            //_dbContext.SaveChanges();
           
            return RedirectToAction("Index", "Construction");
        }


        public  IActionResult Delete(int id)
        {
            // var construction = new GRConstruction { Id = id }; 
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var construction = new GRConstruction();
            construction = _dbContext.GRConstruction.Where(x => x.Id == id ).FirstOrDefault();

            if (construction != null)
            {
                _dbContext.GRConstruction.Attach(construction);
                _dbContext.GRConstruction.Remove(construction);
                _dbContext.SaveChanges();

                TempData["error"] = "false";
                TempData["message"] = "Greige Construction has been deleted successfully.";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong.";
            }
            return RedirectToAction("Index");
        }

        public async Task<InvItemCategories> CreateCategory(GRConstruction modelConstruction)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            InvItemCategories model = new InvItemCategories();
            var parentId = 0;
            if (modelConstruction.GRCategoryId!=0 && modelConstruction.GRCategoryId !=null)
            {
                var GRCategory = _dbContext.GRCategory.Where(x => x.Id == modelConstruction.GRCategoryId ).ToList();
                if(GRCategory!=null && GRCategory.Count!=0)
                {
                    parentId = GRCategory.FirstOrDefault().ItemCategoryId;
                    model = CreateGreigeChild(parentId);
                }
            }
                

            if (model != null)
            {
                model.Code = model.ParentCode + "." + model.Code;
                model.Status = "Child";
                model.IsActive = true;
                model.Name = modelConstruction.Description;
                model.CompanyId = _companyId;
                model.CreatedBy = _userId;
                model.CreatedDate = DateTime.Now;
                _dbContext.InvItemCategories.Add(model);
                await _dbContext.SaveChangesAsync();
                return model;
            }
            else
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
            var parentAccount = _dbContext.InvItemCategories.Where(a => a.Id == id && a.IsDeleted == false ).FirstOrDefault(); //get parentId
            if (parentAccount == null)
                return itemCategories;

            //get last account code
            var account = _dbContext.InvItemCategories
                        .Where(a => a.ParentId == id && a.IsDeleted == false)
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



        public async Task<InvItemCategories> UpdateCategory(GRConstruction model)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            var invItemCategory = _dbContext.InvItemCategories.Where(x => x.Id == model.ItemCategoryId ).FirstOrDefault();
            if (invItemCategory != null)
            {
                invItemCategory.Name = model.Description;
                _dbContext.InvItemCategories.Update(invItemCategory);
            }

            return invItemCategory;
        }


    }
}
