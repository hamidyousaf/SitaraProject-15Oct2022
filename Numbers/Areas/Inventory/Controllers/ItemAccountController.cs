using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using Numbers.Repository.Inventory;
using System.Linq.Dynamic.Core;

namespace Numbers.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    [Authorize]
    public class ItemAccountController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public ItemAccountController(NumbersDbContext context)
        {
            _dbContext = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var itemAccountRepo = new ItemAccountRepo(_dbContext);
            IEnumerable<InvItemAccount> list = itemAccountRepo.GetAll(companyId);
            if (list != null)
            {
                ViewBag.NavbarHeading = "List of Item Accounts";
                return View(list);
            }
            TempData["error"] = "true";
            TempData["message"] = "There are no Item Accounts in stock, You need to add Item Account in order to view them.";
            ViewBag.NavbarHeading = "List of Item Accounts";
            return RedirectToAction(nameof(Create));
        }

        [HttpGet]
        public IActionResult Create(int id)
        {
            ViewBag.ListOfAccount = new SelectList(from a in _dbContext.GLAccounts.Where(
                                                a => a.IsDeleted == false && a.Company.Id == HttpContext.Session.GetInt32("CompanyId").Value && a.AccountLevel == 4).OrderBy(a => a.Code)
                                               .ToList()
                                               select new
                                               {
                                                   Id = a.Id,
                                                   Name = string.Concat(a.Code, " - ", a.Name),

                                               }, "Id", "Name");


            var itemAccountRepo = new ItemAccountRepo(_dbContext);
            if (id == 0)
            {
                ViewBag.EntityState = "Create";
                ViewBag.NavbarHeading = "Create Item Account";
                return View(new InvItemAccount()); 
            }
            else
            {
                ViewBag.EntityState = "Update";
                ViewBag.NavbarHeading = "Update Item Account";
                InvItemAccount itemAccount = itemAccountRepo.GetById(id);
                return View(itemAccount);
            }
           
        }

        [HttpPost]
        public async Task<IActionResult> Create(InvItemAccount model)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var itemAccountRepo = new ItemAccountRepo(_dbContext);
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            else
            {
                if (model.Id == 0)
                {
                    model.CreatedBy = userId;
                    model.CompanyId = companyId;
                    bool isSuccess = await itemAccountRepo.Create(model);
                    if (isSuccess == true)
                    {
                        TempData["error"] = "false";
                        TempData["message"] = "Item Account has been Created successfully.";
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
                    model.UpdatedBy = userId;
                    model.CompanyId = companyId;
                    bool isSuccess = await itemAccountRepo.Update(model);
                    if (isSuccess == true)
                    {
                        TempData["error"] = "false";
                        TempData["message"] = "Item Account has been updated successfully.";
                    }
                    else
                    {
                        TempData["error"] = "true";
                        TempData["message"] = "Something went wrong.";
                    }
                    return RedirectToAction(nameof(Index));
                }
            }            
        }

        public async Task<IActionResult> Delete(int id)
        {
            var itemAccountRepo = new ItemAccountRepo(_dbContext);
            bool isSuccess = await itemAccountRepo.Delete(id);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Item Account has been deleted successfully.";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong.";
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult GetList()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                //var searchValue = Request.Form["search[value]"].FirstOrDefault();
                var searchId = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchAccount = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchAsset = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchSale = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchCost = Request.Form["columns[4][search][value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var ItemAccountsData = (from ItemAccounts in _dbContext.InvItemAccounts
                                .Include(v => v.GLAssetAccount)
                                .Include(v => v.GLSaleAccount)
                                .Include(v => v.GLCostofSaleAccount)
                                .Include(v => v.GLWIPAccount)
                                .Where(v => v.IsDeleted == false && v.CompanyId == companyId)
                                select ItemAccounts);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    ItemAccountsData = ItemAccountsData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                ItemAccountsData = !string.IsNullOrEmpty(searchId) ? ItemAccountsData.Where(m => m.Id.ToString().ToUpper().Contains(searchId.ToUpper())) : ItemAccountsData;
                ItemAccountsData = !string.IsNullOrEmpty(searchAccount) ? ItemAccountsData.Where(m => m.Name.ToString().ToUpper().Contains(searchAccount.ToUpper())) : ItemAccountsData;
                ItemAccountsData = !string.IsNullOrEmpty(searchAsset) ? ItemAccountsData.Where(m => m.GLAssetAccount.Name.ToString().ToUpper().Contains(searchAsset.ToUpper())) : ItemAccountsData;
                ItemAccountsData = !string.IsNullOrEmpty(searchSale) ? ItemAccountsData.Where(m => m.GLSaleAccount.Name.ToString().ToUpper().Contains(searchSale.ToUpper())) : ItemAccountsData;
                ItemAccountsData = !string.IsNullOrEmpty(searchCost) ? ItemAccountsData.Where(m => m.GLCostofSaleAccount.Name.ToString().ToUpper().Contains(searchCost.ToUpper())) : ItemAccountsData;

                recordsTotal = ItemAccountsData.Count();
                var data = ItemAccountsData.ToList();
                if (pageSize == -1)
                {
                    data = ItemAccountsData.Skip(skip).Take(recordsTotal == 0 ? 1 : pageSize == -1 ? recordsTotal : 1).ToList();
                }
                else
                {
                    data = ItemAccountsData.Skip(skip).Take(pageSize).ToList();
                }
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data };
                return Ok(jsonData);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}