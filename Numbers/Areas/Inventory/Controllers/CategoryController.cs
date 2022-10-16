using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using Numbers.Repository.Inventory;

namespace Numbers.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        private readonly int _level1, _level2, _level3, _level4;
        private readonly string _splitter;
        private readonly IList<SelectListItem> _options = new List<SelectListItem>();
        Dictionary<string, object> returnResponse = new Dictionary<string, object>();
        public CategoryController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
            _level1 = 1;
            _level2 = 2;
            _level3 = 2;
            _level4 = 4;
            _splitter = ".";
        }

        [HttpGet]
        public IActionResult Index()
        {
            //int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            //ViewBag.DbContext = _dbContext;
            //ViewBag.Company = companyId;
            //var categoryRepo = new CategoryRepo(_dbContext);
            //var list = categoryRepo.GetAll(companyId);
            //return View(list);
            ViewBag.NavbarHeading = "List of Categories";
            return View();
        }
        [HttpGet]
        public IActionResult Create(int id)
        {
            var list = new SelectList(_dbContext.InvItemCategories.Where(l => l.IsActive == true).ToList(), "Id", "Name");
            if (list != null)
            {
                var helper = new InventoryHelper(_dbContext,HttpContext.Session.GetInt32("CompanyId").Value);
                ViewBag.listOfcategory = helper.GetCategoriesSelectLists();
            }
            else
            {
                ViewBag.listOfcategory = new SelectList("0", "Select..");
            }
            if (id == 0)
            {
                ViewBag.EntityState = "Create";
                return View(new InvItemCategories());
            }
            else
            {
                ViewBag.EntityState = "Update";
                var categoryRepo = new CategoryRepo(_dbContext);
                var category = categoryRepo.GetById(id);
                return View(category);
            }          
        }

        [HttpPost]
        public async Task<IActionResult> Create(InvItemCategories model)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            if (ModelState.IsValid)
            {
                if (model.Id == 0)
                {
                    model.CompanyId = companyId;
                    model.CreatedBy = userId;
                    var categoryRepo = new CategoryRepo(_dbContext);
                    bool isSuccess = await categoryRepo.Create(model);
                    if (isSuccess == true)
                    {
                        returnResponse.Add("success", true);
                        returnResponse.Add("message", string.Concat("Category has been created <br>"));
                        return Ok(returnResponse);
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
                    model.CompanyId = companyId;
                    model.UpdatedBy = userId;
                    var categoryRepo = new CategoryRepo(_dbContext);
                    bool isSuccess = await categoryRepo.Update(model);
                    if (isSuccess == true)
                    {
                        returnResponse.Add("success", true);
                        returnResponse.Add("message", string.Concat("Category has been updated <br>"));
                        return Ok(returnResponse);
                    }
                    else
                    {
                        TempData["error"] = "true";
                        TempData["message"] = "Something went wrong.";
                    }
                    return RedirectToAction(nameof(Index));
                }    
            }
            TempData["error"] = "true";
            var allErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).FirstOrDefault();
            TempData["message"] = allErrors;
            return View(model);
        }  
        
        public async Task<IActionResult> Delete(int id)
        {
            var categoryRepo = new CategoryRepo(_dbContext);
            bool isSuccess = await categoryRepo.Delete(id);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Category has been deleted successfully.";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong.";
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            ViewBag.EntityState = "Update";
            ViewBag.Heading = "Update";
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            InvItemCategories itemCategories = new InvItemCategories();
            var category = _dbContext.InvItemCategories.Where(c => c.Id == id && c.IsDeleted == false && c.CompanyId == companyId).FirstOrDefault();
            if (category.Code.Contains("."))
            {
                int a = category.Code.LastIndexOf('.');
                string b = category.Code.Substring(0, a);
                string c = category.Code.Substring(a + 1);
                category.ParentCode = b + ".";
                category.Code = c;
            }
    
            else if (category == null) { return NotFound(); }       
            return View("PopupCreate", category);
        }

        public IActionResult CreateChild(int id)
        {
            ViewBag.EntityState = "Create";
            ViewBag.Heading = "Create Child";
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            InvItemCategories itemCategories = new InvItemCategories();
            var parentAccount = _dbContext.InvItemCategories.Where(a => a.Id == id && a.IsDeleted == false && a.CompanyId == companyId).FirstOrDefault(); //get parentId
            if (parentAccount == null)
                return NotFound("Parent account not found, please refresh page");

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
                return NotFound("Parent account not found, please refresh page");
            }
            if (account == null) //means no child row, start with 1
            {
                newCode = "01";
                if(newCodeLength !=4)
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
                    newCode = newCode.PadLeft(2,'0');
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
            itemCategories.Id=0;
            return View("PopupCreate", itemCategories);
        }

        public InvItemCategories  CreateGreigeChild(int id)
        {
            ViewBag.EntityState = "Create";
            ViewBag.Heading = "Create Child";
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            InvItemCategories itemCategories = new InvItemCategories();
            var parentAccount = _dbContext.InvItemCategories.Where(a => a.Id == id && a.IsDeleted == false && a.CompanyId == companyId).FirstOrDefault(); //get parentId
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

        public IActionResult CreateSibling(int id)
        {
            ViewBag.EntityState = "Create";
            ViewBag.Heading = "Create Sibling";
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string newCode;
            int newCodeLength;
            InvItemCategories itemCategories = new InvItemCategories();
            InvItemCategories categories = new InvItemCategories();
            if (id == 0)
            {
                categories = _dbContext.InvItemCategories.Where(a => a.IsDeleted == false && a.CompanyId == companyId && a.ParentId == null && a.Status == "Parent").LastOrDefault();
                newCodeLength =0;
            }//get parentId
            else
            {
                categories = _dbContext.InvItemCategories.Where(a => a.IsDeleted == false && a.CompanyId == companyId && a.ParentId == id && a.Status == "Child").LastOrDefault();
                short newAccountLevel = (short)(categories.CategoryLevel);
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
                    return NotFound("Parent account not found, please refresh page");
                }
            }  
            if (categories.Status=="Parent") //increment 1 into last code
            {
                int c = Convert.ToInt16(categories.Code) + 1;
                newCode = c.ToString();
                newCode = newCode.PadLeft(2, '0');
            }
            else if(categories.Status=="Child")
            {
                if (newCodeLength == 4)
                    newCodeLength = 4 - 2;
                newCode = categories.Code.Substring(categories.Code.Length - newCodeLength, newCodeLength);
                int c = Convert.ToInt16(newCode) + 1;
                newCode = c.ToString();
                newCode = newCode.PadLeft(newCodeLength, '0');
            }
            else //means no child row, start with 1
            {
                newCode = "01";
            }
            
            //var account = _dbContext.GLAccounts.Where(a => a.Id == account.ParentId && a.IsDeleted == false).FirstOrDefault();
            ViewBag.Title = "Create account";
            ViewBag.Action = "Create";
            ViewBag.Level1 = _level1;
            ViewBag.Level2 = _level2;
            ViewBag.Level3 = _level3;
            ViewBag.Level4 = _level4;

            
            if (categories.ParentId == null)
            {
                itemCategories.ParentCode = newCode;
                itemCategories.CategoryLevel = 1;
            }
            else
            {
                int a = categories.Code.LastIndexOf('.');
                string b = categories.Code.Substring(0, a);
                itemCategories.ParentCode = b+".";
                itemCategories.ParentId = categories.ParentId;
                itemCategories.Code = newCode;
                itemCategories.CategoryLevel = categories.CategoryLevel;
            }
            itemCategories.Name = "";
            itemCategories.Id = 0;
            return View("PopupCreate", itemCategories);
        }
    }
}
