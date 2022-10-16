using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Helpers;

using System.Linq.Dynamic.Core;
using Numbers.Repository.AR;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Numbers.Areas.AR.Controllers
{
    [Area("AR")]
    [Authorize]
    public class SalePersonController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        private readonly ARSalePersonRepository _ARSalePersonRepository;
        private readonly ARAnualySaleTargetRepository _AnualySaleTargetRepository;
        private readonly ARMonthlySaleTargetRepository _ARMonthlySaleTargetRepository;
        private readonly ARSalePersonCityRepository _ARSalePersonCityRepository;
        private readonly ARSalePersonItemCategoryRepository _ARSPItemCategoryRepository;
        public SalePersonController(NumbersDbContext context, ARSalePersonRepository arSalePersonRepository, ARAnualySaleTargetRepository aRAnualySaleTarget, ARMonthlySaleTargetRepository aRMonthlySaleTargetRepository, ARSalePersonCityRepository aRSalePersonCityRepository, ARSalePersonItemCategoryRepository aRSalePersonItemCategoryRepository)
        {
            _dbContext = context;
            _ARSalePersonRepository = arSalePersonRepository;
            _AnualySaleTargetRepository = aRAnualySaleTarget;
            _ARMonthlySaleTargetRepository = aRMonthlySaleTargetRepository;
            _ARSalePersonCityRepository = aRSalePersonCityRepository;
            _ARSPItemCategoryRepository = aRSalePersonItemCategoryRepository;
        }
        public IActionResult Index()
        {
            ViewBag.NavbarHeading = "List of Sale Persons";
            return View();
        }
        public IActionResult Create(int? id)
        {
            ARSalePersonViewModel aRSalePersonViewModel = new ARSalePersonViewModel();
            var companyId = HttpContext.Session.GetInt32("CompanyId");
            ViewBag.EntityState = "Create";
            ViewBag.NavbarHeading = "Create Sale Person";
            aRSalePersonViewModel.Cities = new SelectList(_dbContext.AppCities.Where(c => c.CountryId == 1).OrderBy(c => c.Id).ToList(), "Id", "Name");
            //aRSalePersonViewModel.Customer = new SelectList(_dbContext.ARCustomers.OrderBy(c => c.Id).ToList(), "Id", "Name");
            var items = _dbContext.AppCompanySetups.Where(x => x.Name == "Local Finish ItemCode").FirstOrDefault().Value;

            aRSalePersonViewModel.ListOfItemCategories = new SelectList(from ac in _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.CategoryLevel == 2 && x.Code.StartsWith(items)).OrderBy(x => x.Code).ToList()
                                                                        select new
                                                                        {
                                                                            Id = ac.Id,
                                                                            Name = ac.Code + " - " + ac.Name
                                                                        }, "Id", "Name");
            aRSalePersonViewModel.ItemCategories = _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.CategoryLevel == 2 /*&& x.CompanyId == companyId*/).ToList();
            aRSalePersonViewModel.City = _dbContext.AppCities.Where(c => c.CountryId == 1).OrderBy(c => c.Id).ToList();
            if (id != 0)
            {
                if (id > 0)
                {
                    ViewBag.EntityState = "Update";
                    ViewBag.NavbarHeading = "Update Sale Person";
                }

                aRSalePersonViewModel.ARSalePerson = _ARSalePersonRepository.Get(x => x.ID == id).FirstOrDefault();
                aRSalePersonViewModel.StartDate = aRSalePersonViewModel?.ARSalePerson?.StartDate.ToString("dd-MMM-yyyy") ?? DateTime.Now.ToString("dd-MMM-yyyy"); ;
                aRSalePersonViewModel.EndDate = aRSalePersonViewModel?.ARSalePerson?.EndDate.ToString("dd-MMM-yyyy") ?? DateTime.Now.ToString("dd-MMM-yyyy");
                aRSalePersonViewModel.ARAnnualSaleTargets = _AnualySaleTargetRepository.Get(x => x.SalePerson == id).ToList();
                aRSalePersonViewModel.ARSalePersonCities = _ARSalePersonCityRepository.Get(x => x.SalePerson_ID == id).ToList();
                aRSalePersonViewModel.ARSalePersonItemCategories = _ARSPItemCategoryRepository.Get(x => x.SalePerson_ID == id).ToList();


                var date = _dbContext.ARMonthlySaleTargets.Where(x => x.SalePerson == id).ToList();
                List<ARMonthlySaleTargets> list = new List<ARMonthlySaleTargets>();
                foreach (var item in date)
                {
                    ARMonthlySaleTargets saleTargets = new ARMonthlySaleTargets();
                    saleTargets = item;
                    saleTargets.tMonth = Convert.ToDateTime(saleTargets.Month);
                    list.Add(saleTargets);
                }
                var data = list.OrderBy(x => x.tMonth);
                aRSalePersonViewModel.ARMonthlySaleTargets = data.ToList();
            }
            // No selected items
            var AnualSaleTargetCaogories = aRSalePersonViewModel.ARSalePersonItemCategories.Where(p => !aRSalePersonViewModel.ARAnnualSaleTargets.Any(x => x.ItemCategory == p.ItemCategory_ID)).ToList();
            //aRSalePersonViewModel.AnualSaleTargetCaogoriesOnEdit = new SelectList(from ac in _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.CategoryLevel == 2 && x.CompanyId == companyId).OrderBy(x => x.Code).ToList()
            //                                                        where AnualSaleTargetCaogories.Exists(p=>p.ItemCategory_ID == ac.Id)
            //                                                        select new
            //                                                        {
            //                                                            Id = ac.Id,
            //                                                            Name = ac.Code + " - " + ac.Name
            //                                                        }, "Id", "Name");   
            //aRSalePersonViewModel.AnualSaleTargetCaogoriesOnEdit = new SelectList(from ac in _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.CategoryLevel == 2 && x.CompanyId == companyId).OrderBy(x => x.Code).ToList()
            //                                                                      where aRSalePersonViewModel.ARSalePersonItemCategories.Exists(p => p.ItemCategory_ID == ac.Id)
            //                                                                      select new
            //                                                                      {
            //                                                                          Id = ac.Id,
            //                                                                          Name = ac.Code + " - " + ac.Name
            //                                                                      }, "Id", "Name");
            if (id != null)
            {
                aRSalePersonViewModel.AnualSaleTargetCaogoriesOnEdit = new SelectList(_dbContext.ARSalePersonItemCategory
                .Include(x => x.InvItemCategories)
                .Include(x => x.City)
                .Where(x => x.SalePerson_ID == aRSalePersonViewModel.ARSalePerson.ID)
                .Select(x => new ListOfValue
                {
                    Id = x.ItemCategory_ID,
                    Name = x.City.Name + " - " + x.InvItemCategories.Code + " - " + x.InvItemCategories.Name
                }).ToList()
                , "Id", "Name");
                aRSalePersonViewModel.MonthlySaleTargetCaogoriesOnEdit = new SelectList(_dbContext.ARAnnualSaleTargets
                    .Include(x => x.InvItemCategories)
                    .Include(x => x.City)
                    .Where(x=>x.SalePerson == aRSalePersonViewModel.ARSalePerson.ID)
                    .Select(x => new ListOfValue
                    {
                        Id = x.ItemCategory,
                        Name = x.City.Name + " - " + x.InvItemCategories.Code + " - " + x.InvItemCategories.Name
                    }).ToList(), "Id", "Name");
            }
            
            // No selected items
            var MonthlySaleTargetCaogories = aRSalePersonViewModel.ARSalePersonItemCategories.Where(p => !aRSalePersonViewModel.ARMonthlySaleTargets.Any(x => x.ItemCategory == p.ItemCategory_ID)).ToList();
            //aRSalePersonViewModel.MonthlySaleTargetCaogoriesOnEdit = new SelectList(from ac in _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.CategoryLevel == 2 && x.CompanyId == companyId).OrderBy(x => x.Code).ToList()
            //                                                                      where MonthlySaleTargetCaogories.Exists(p => p.ItemCategory_ID == ac.Id)
            //                                                                      select new
            //                                                                      {
            //                                                                          Id = ac.Id,
            //                                                                          Name = ac.Code + " - " + ac.Name
            //                                                                      }, "Id", "Name");

            //aRSalePersonViewModel.MonthlySaleTargetCaogoriesOnEdit = new SelectList(from ac in _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.CategoryLevel == 2 && x.CompanyId == companyId).OrderBy(x => x.Code).ToList()
            //                                                                        where aRSalePersonViewModel.ARSalePersonItemCategories.Exists(p => p.ItemCategory_ID == ac.Id)
            //                                                                        select new
            //                                                                        {
            //                                                                            Id = ac.Id,
            //                                                                            Name = ac.Code + " - " + ac.Name
            //                                                                        }, "Id", "Name");

            return View(aRSalePersonViewModel);
        }
        public JsonResult checkProductCodeAlreadyExists(string code)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            System.Threading.Thread.Sleep(200);
            if (code == "0")
                return Json(0);
            var chkCode = _dbContext.ARSalePerson.Where(a => a.IsActive == true && a.IsDelete == false && a.EmployeeCode == code && a.CompanyId == companyId).FirstOrDefault();
            return Json(chkCode == null ? 0 : 1);
        }
        [HttpPost]
        public async Task<IActionResult> Create(ARSalePersonViewModel aRSalePersonViewModel, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            ARMonthlySaleTargets[] monthTarget = JsonConvert.DeserializeObject<ARMonthlySaleTargets[]>(collection["monthTarget"]);
            ARAnnualSaleTargets[] annualTarget = JsonConvert.DeserializeObject<ARAnnualSaleTargets[]>(collection["annualTarget"]);
            ARSalePersonCity[] cities = JsonConvert.DeserializeObject<ARSalePersonCity[]>(collection["SPCity"]);
            ARSalePersonItemCategory[] itemCategories = JsonConvert.DeserializeObject<ARSalePersonItemCategory[]>(collection["SPItemCat"]);
            if (aRSalePersonViewModel.ARSalePerson.ID == 0)
            {
                aRSalePersonViewModel.ARSalePerson.CompanyId = companyId;
                aRSalePersonViewModel.ARSalePerson.CreatedBy = userId;
                aRSalePersonViewModel.ARSalePerson.CreatedDate = DateTime.Now;
                aRSalePersonViewModel.ARSalePerson.Resp_ID = resp_Id;
                aRSalePersonViewModel.ARSalePerson.IsActive = true;
                aRSalePersonViewModel.ARSalePerson.IsDelete = false;
                await _ARSalePersonRepository.CreateAsync(aRSalePersonViewModel.ARSalePerson);
                //Monthly Target
                foreach (var month in monthTarget)
                {
                    ARMonthlySaleTargets aRMonthlySaleTarget = new ARMonthlySaleTargets();
                    aRMonthlySaleTarget.ItemCategory = month.ItemCategory;
                    aRMonthlySaleTarget.CityId = month.CityId;
                    aRMonthlySaleTarget.MonthTarget = month.MonthTarget;
                    aRMonthlySaleTarget.Month = month.Month;
                    aRMonthlySaleTarget.Year = month.Year;
                    aRMonthlySaleTarget.SalePerson = aRSalePersonViewModel.ARSalePerson.ID;
                    aRMonthlySaleTarget.CreatedBy = userId;
                    aRMonthlySaleTarget.CreatedDate = DateTime.Now;
                    aRMonthlySaleTarget.IsDelete = false;
                    aRMonthlySaleTarget.IsActive = true;
                    await _ARMonthlySaleTargetRepository.CreateAsync(aRMonthlySaleTarget);
                }
                //Annual Target
                foreach (var anual in annualTarget)
                {
                    ARAnnualSaleTargets aRAnnualSaleTarget = new ARAnnualSaleTargets();
                    aRAnnualSaleTarget.ItemCategory = anual.ItemCategory;
                    aRAnnualSaleTarget.AnnualTarget = anual.AnnualTarget;
                    aRAnnualSaleTarget.CityId = anual.CityId;
                    aRAnnualSaleTarget.StartDate = anual.StartDate;
                    aRAnnualSaleTarget.EndDate = anual.EndDate;
                    aRAnnualSaleTarget.SalePerson = aRSalePersonViewModel.ARSalePerson.ID;
                    aRAnnualSaleTarget.CreatedBy = userId;
                    aRAnnualSaleTarget.CreatedDate = DateTime.Now;
                    aRAnnualSaleTarget.IsActive = true;
                    aRAnnualSaleTarget.IsDelete = false;
                    await _AnualySaleTargetRepository.CreateAsync(aRAnnualSaleTarget);
                }
                //Sale Person Cities
                foreach (var city in cities)
                {
                    ARSalePersonCity aRSalePersonCity = new ARSalePersonCity();
                    aRSalePersonCity.City_ID = city.City_ID;
                    aRSalePersonCity.SalePerson_ID = aRSalePersonViewModel.ARSalePerson.ID;
                    aRSalePersonCity.CreatedBy = userId;
                    aRSalePersonCity.CreatedDate = DateTime.Now;
                    aRSalePersonCity.IsActive = true;
                    aRSalePersonCity.IsDelete = false;
                    await _ARSalePersonCityRepository.CreateAsync(aRSalePersonCity);
                }
                //Sale Person ItemCategories
                foreach (var itemCate in itemCategories)
                {
                    ARSalePersonItemCategory aRSalePersonItemCategory = new ARSalePersonItemCategory();
                    aRSalePersonItemCategory.ItemCategory_ID = itemCate.ItemCategory_ID;
                    aRSalePersonItemCategory.CityId = itemCate.CityId;
                    aRSalePersonItemCategory.SalePerson_ID = aRSalePersonViewModel.ARSalePerson.ID;
                    aRSalePersonItemCategory.CreatedBy = userId;
                    aRSalePersonItemCategory.CreatedDate = DateTime.Now;
                    aRSalePersonItemCategory.IsActive = true;
                    aRSalePersonItemCategory.IsDelete = false;
                    await _ARSPItemCategoryRepository.CreateAsync(aRSalePersonItemCategory);
                }
                await _dbContext.SaveChangesAsync();
                TempData["error"] = "false";
                TempData["message"] = "SalePerson has been saved successfully.";
            }
            else
            {
                ARSalePerson salePerson = _ARSalePersonRepository.Get(x => x.ID == aRSalePersonViewModel.ARSalePerson.ID).FirstOrDefault();
                salePerson.EmployeeCode = aRSalePersonViewModel.ARSalePerson.EmployeeCode;
                salePerson.Department = aRSalePersonViewModel.ARSalePerson.Department;
                salePerson.Designation = aRSalePersonViewModel.ARSalePerson.Designation;
                salePerson.Name = aRSalePersonViewModel.ARSalePerson.Name;
                salePerson.Comission = aRSalePersonViewModel.ARSalePerson.Comission;
                salePerson.EndDate = aRSalePersonViewModel.ARSalePerson.EndDate;
                salePerson.StartDate = aRSalePersonViewModel.ARSalePerson.StartDate;
                salePerson.IsActive = true;
                salePerson.IsDelete = false;
                salePerson.UpdatedBy = userId;
                salePerson.UpdatedDate = DateTime.Now;
                _dbContext.ARSalePerson.Update(salePerson);
                var existingMonthTarget = _ARMonthlySaleTargetRepository.Get(x => x.SalePerson == salePerson.ID).ToList();
                var existingAnualTarget = _AnualySaleTargetRepository.Get(x => x.SalePerson == salePerson.ID).ToList();
                var existingSPCities = _ARSalePersonCityRepository.Get(x => x.SalePerson_ID == salePerson.ID).ToList();
                var existingItemCate = _ARSPItemCategoryRepository.Get(x => x.SalePerson_ID == salePerson.ID).ToList();
                //Deleting monthly target limit
                foreach (var month in existingMonthTarget)
                {
                    bool isExist = monthTarget.Any(x => x.ID == month.ID);
                    if (!isExist)
                    {
                        await _ARMonthlySaleTargetRepository.DeleteAsync(month);
                        _dbContext.SaveChanges();
                    }
                }
                //Deleting annual target limit
                foreach (var annual in existingAnualTarget)
                {
                    bool isExist = annualTarget.Any(x => x.ID == annual.ID);
                    if (!isExist)
                    {
                        await _AnualySaleTargetRepository.DeleteAsync(annual);
                        _dbContext.SaveChanges();
                    }
                }
                //Deleting Cities
                foreach (var city in existingSPCities)
                {
                    bool isExist = cities.Any(x => x.ID == city.ID);
                    if (!isExist)
                    {
                        await _ARSalePersonCityRepository.DeleteAsync(city);
                        _dbContext.SaveChanges();
                    }
                }
                //Deleting ItemCategories
                foreach (var itemCat in existingItemCate)
                {
                    bool isExist = itemCategories.Any(x => x.ID == itemCat.ID);
                    if (!isExist)
                    {
                        await _ARSPItemCategoryRepository.DeleteAsync(itemCat);
                        _dbContext.SaveChanges();
                    }
                }
                //Inserting/Updating annual limit
                foreach (var anual in annualTarget)
                {
                    if (anual.ID == 0) //Inserting New Records
                    {
                        ARAnnualSaleTargets aRAnnualSaleTarget = new ARAnnualSaleTargets();
                        aRAnnualSaleTarget.ItemCategory = anual.ItemCategory;
                        aRAnnualSaleTarget.CityId = anual.CityId;
                        aRAnnualSaleTarget.AnnualTarget = anual.AnnualTarget;
                        aRAnnualSaleTarget.StartDate = anual.StartDate;
                        aRAnnualSaleTarget.EndDate = anual.EndDate;
                        aRAnnualSaleTarget.SalePerson = aRSalePersonViewModel.ARSalePerson.ID;
                        aRAnnualSaleTarget.CreatedBy = userId;
                        aRAnnualSaleTarget.CreatedDate = DateTime.Now;
                        aRAnnualSaleTarget.IsActive = true;
                        aRAnnualSaleTarget.IsDelete = false;
                        await _AnualySaleTargetRepository.CreateAsync(aRAnnualSaleTarget);
                    }
                    else   //Updating Records
                    {
                        var annualLimit = _AnualySaleTargetRepository.Get(x => x.ID == anual.ID).FirstOrDefault();
                        annualLimit.ItemCategory = anual.ItemCategory;
                        annualLimit.CityId = anual.CityId;
                        annualLimit.StartDate = anual.StartDate;
                        annualLimit.EndDate = anual.EndDate;
                        annualLimit.AnnualTarget = anual.AnnualTarget;
                        annualLimit.IsActive = true;
                        annualLimit.IsDelete = false;
                        annualLimit.UpdatedBy = userId;
                        annualLimit.UpdatedDate = DateTime.Now;
                        _dbContext.ARAnnualSaleTargets.Update(annualLimit);
                    }
                }

                //Inserting/Updating monthly limit
                foreach (var month in monthTarget)
                {
                    if (month.ID == 0) //Inserting New Records
                    {
                        ARMonthlySaleTargets aRMonthlySaleTarget = new ARMonthlySaleTargets();
                        aRMonthlySaleTarget.ItemCategory = month.ItemCategory;
                        aRMonthlySaleTarget.CityId = month.CityId;
                        aRMonthlySaleTarget.MonthTarget = month.MonthTarget;
                        aRMonthlySaleTarget.Month = month.Month;
                        aRMonthlySaleTarget.Year = month.Year;
                        aRMonthlySaleTarget.SalePerson = aRSalePersonViewModel.ARSalePerson.ID;
                        aRMonthlySaleTarget.CreatedBy = userId;
                        aRMonthlySaleTarget.CreatedDate = DateTime.Now;
                        aRMonthlySaleTarget.IsDelete = false;
                        aRMonthlySaleTarget.IsActive = true;
                        await _ARMonthlySaleTargetRepository.CreateAsync(aRMonthlySaleTarget);
                    }
                    else   //Updating Records
                    {
                        var monthlyLimit = _ARMonthlySaleTargetRepository.Get(x => x.ID == month.ID).FirstOrDefault();
                        monthlyLimit.Month = month.Month;
                        monthlyLimit.Year = month.Year;
                        monthlyLimit.MonthTarget = month.MonthTarget;
                        

                        monthlyLimit.ItemCategory = month.ItemCategory;
                        monthlyLimit.CityId = month.CityId;
                        monthlyLimit.IsActive = true;
                        monthlyLimit.IsDelete = false;
                        monthlyLimit.UpdatedBy = userId;
                        monthlyLimit.UpdatedDate = DateTime.Now;
                        _dbContext.ARMonthlySaleTargets.Update(monthlyLimit);
                    }
                }

                //Inserting/Updating SalePerson Cities
                foreach (var city in cities)
                {
                    if (city.ID == 0) //Inserting New Records
                    {
                        ARSalePersonCity aRSalePersonCity = new ARSalePersonCity();
                        aRSalePersonCity.City_ID = city.City_ID;
                        aRSalePersonCity.SalePerson_ID = aRSalePersonViewModel.ARSalePerson.ID;
                        aRSalePersonCity.CreatedBy = userId;
                        aRSalePersonCity.CreatedDate = DateTime.Now;
                        aRSalePersonCity.IsActive = true;
                        aRSalePersonCity.IsDelete = false;
                        await _ARSalePersonCityRepository.CreateAsync(aRSalePersonCity);
                    }
                    else   //Updating Records
                    {
                        var _cities = _ARSalePersonCityRepository.Get(x => x.ID == city.ID).FirstOrDefault();
                        _cities.City_ID = city.City_ID;
                        _cities.IsActive = true;
                        _cities.IsDelete = false;
                        _cities.UpdatedBy = userId;
                        _cities.UpdatedDate = DateTime.Now;
                        _dbContext.ARSalePersonCity.Update(_cities);
                    }
                }

                //Inserting/Updating SalePerson ItemCategories
                foreach (var itemCate in itemCategories)
                {
                    if (itemCate.ID == 0) //Inserting New Records
                    {
                        ARSalePersonItemCategory aRSalePersonItemCategory = new ARSalePersonItemCategory();
                        aRSalePersonItemCategory.ItemCategory_ID = itemCate.ItemCategory_ID;
                        aRSalePersonItemCategory.CityId = itemCate.CityId;
                        aRSalePersonItemCategory.SalePerson_ID = aRSalePersonViewModel.ARSalePerson.ID;
                        aRSalePersonItemCategory.CreatedBy = userId;
                        aRSalePersonItemCategory.CreatedDate = DateTime.Now;
                        aRSalePersonItemCategory.IsActive = true;
                        aRSalePersonItemCategory.IsDelete = false;
                        await _ARSPItemCategoryRepository.CreateAsync(aRSalePersonItemCategory);
                    }
                    else   //Updating Records
                    {
                        var _categories = _ARSPItemCategoryRepository.Get(x => x.ID == itemCate.ID).FirstOrDefault();
                        _categories.ItemCategory_ID = itemCate.ItemCategory_ID;
                        _categories.CityId = itemCate.CityId;
                        _categories.IsActive = true;
                        _categories.IsDelete = false;
                        _categories.UpdatedBy = userId;
                        _categories.UpdatedDate = DateTime.Now;
                        _dbContext.ARSalePersonItemCategory.Update(_categories);
                    }
                }

                TempData["error"] = "false";
                TempData["message"] = "SalePerson has been Updated successfully.";
            }
            _dbContext.SaveChanges();

            return RedirectToAction("Create");
        }

        [HttpPost]
        public IActionResult GetSalePersons()
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
                var searchEmpCode = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchName = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchDepartment = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchDesignation = Request.Form["columns[4][search][value]"].FirstOrDefault();
                var searchCommision = Request.Form["columns[5][search][value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var SalePersons = (from tempcustomer in _dbContext.ARSalePerson.Where(x => x.IsDelete == false && x.CompanyId == companyId) select tempcustomer);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    SalePersons = SalePersons.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                SalePersons = !string.IsNullOrEmpty(searchEmpCode) ? SalePersons.Where(m => m.EmployeeCode.ToString().ToUpper().Contains(searchEmpCode.ToUpper())) : SalePersons;
                SalePersons = !string.IsNullOrEmpty(searchName) ? SalePersons.Where(m => m.Name.ToString().ToUpper().Contains(searchName.ToUpper())) : SalePersons;
                SalePersons = !string.IsNullOrEmpty(searchDepartment) ? SalePersons.Where(m => m.Department.ToString().ToUpper().Contains(searchDepartment.ToUpper())) : SalePersons;
                SalePersons = !string.IsNullOrEmpty(searchDesignation) ? SalePersons.Where(m => m.Designation != null ? m.Designation.ToString().ToUpper().Contains(searchDesignation.ToUpper()) : false) : SalePersons;
                SalePersons = !string.IsNullOrEmpty(searchCommision) ? SalePersons.Where(m => m.Comission.ToString().ToUpper().Contains(searchCommision.ToUpper())) : SalePersons;

                recordsTotal = SalePersons.Count();
                var data = SalePersons.ToList();
                if (pageSize == -1)
                {
                    data = SalePersons.Skip(skip).Take(recordsTotal == 0 ? 1 : pageSize == -1 ? recordsTotal : 1).ToList();
                }
                else
                {
                    data = SalePersons.Skip(skip).Take(pageSize).ToList();
                }
                List<ARSalePerson> Details = new List<ARSalePerson>();
                foreach (var grp in data)
                {
                    ARSalePerson saleperson = new ARSalePerson();
                    saleperson = grp;
                    Details.Add(saleperson);

                }
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = Details.OrderByDescending(p=>p.ID) };
                return Ok(jsonData);
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<IActionResult> Delete(int? id)
        {
            var ARSalePerson = _ARSalePersonRepository.Get(x => x.ID == id).FirstOrDefault();
            ARSalePerson.IsDelete = true;
            ARSalePerson.IsActive = false;
            await _ARSalePersonRepository.UpdateAsync(ARSalePerson);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public string GetFormatedDate(DateTime date)
        {
            date = date.AddYears(1).AddSeconds(-1);
            return date.ToString("dd-MMM-yyyy");
        }

        public IActionResult Detail(int id)
        {
            ARSalePersonViewModel aRSalePersonViewModel = new ARSalePersonViewModel();
            var companyId = HttpContext.Session.GetInt32("CompanyId");
            aRSalePersonViewModel.Cities = new SelectList(_dbContext.AppCities.Where(c => c.CountryId == 1).OrderBy(c => c.Id).ToList(), "Id", "Name");
            //aRSalePersonViewModel.Customer = new SelectList(_dbContext.ARCustomers.OrderBy(c => c.Id).ToList(), "Id", "Name");
            var items = _dbContext.AppCompanySetups.Where(x => x.Name == "Local Finish ItemCode").FirstOrDefault().Value;

            aRSalePersonViewModel.ListOfItemCategories = new SelectList(from ac in _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.CategoryLevel == 2 && x.Code.StartsWith(items)).OrderBy(x => x.Code).ToList()
                                                                        select new
                                                                        {
                                                                            Id = ac.Id,
                                                                            Name = ac.Code + " - " + ac.Name
                                                                        }, "Id", "Name");
            aRSalePersonViewModel.ItemCategories = _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.CategoryLevel == 2 && x.CompanyId == companyId).ToList();
            aRSalePersonViewModel.City = _dbContext.AppCities.Where(c => c.CountryId == 1).OrderBy(c => c.Id).ToList();
            if (id != 0)
            {
                if (id > 0)
                {
                    ViewBag.EntityState = "Detail";
                    ViewBag.NavbarHeading = "Detail Sale Person";
                }

                aRSalePersonViewModel.ARSalePerson = _ARSalePersonRepository.Get(x => x.ID == id).FirstOrDefault();
                aRSalePersonViewModel.StartDate = aRSalePersonViewModel?.ARSalePerson?.StartDate.ToString("dd-MMM-yyyy") ?? DateTime.Now.ToString("dd-MMM-yyyy"); ;
                aRSalePersonViewModel.EndDate = aRSalePersonViewModel?.ARSalePerson?.EndDate.ToString("dd-MMM-yyyy") ?? DateTime.Now.ToString("dd-MMM-yyyy");
                aRSalePersonViewModel.ARAnnualSaleTargets = _AnualySaleTargetRepository.Get(x => x.SalePerson == id).ToList();
                aRSalePersonViewModel.ARSalePersonCities = _ARSalePersonCityRepository.Get(x => x.SalePerson_ID == id).ToList();
                aRSalePersonViewModel.ARSalePersonItemCategories = _ARSPItemCategoryRepository.Get(x => x.SalePerson_ID == id).ToList();


                var date = _dbContext.ARMonthlySaleTargets.Where(x => x.SalePerson == id).ToList();
                List<ARMonthlySaleTargets> list = new List<ARMonthlySaleTargets>();
                foreach (var item in date)
                {
                    ARMonthlySaleTargets saleTargets = new ARMonthlySaleTargets();
                    saleTargets = item;
                    saleTargets.tMonth = Convert.ToDateTime(saleTargets.Month);
                    list.Add(saleTargets);
                }
                var data = list.OrderBy(x => x.tMonth);
                aRSalePersonViewModel.ARMonthlySaleTargets = data.ToList();
            }
            // No selected items
           

            return View(aRSalePersonViewModel);
        }
    }
}
