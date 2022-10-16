using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Repository.AR;
using Numbers.Repository.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;

namespace Numbers.Areas.AR.Controllers
{
    [Area("AR")]
    [Authorize]
    public class RecoveryPercentageController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        private readonly ARRecoveryPercentageRepository _ARRecoveryPercentageRepository;
        private readonly ARRecoveryPercentageItemRepository _ARRecoveryPercentageItemRepository;
        public RecoveryPercentageController(NumbersDbContext context, ARRecoveryPercentageRepository ARRecoveryPercentageRepository, ARRecoveryPercentageItemRepository ARRecoveryPercentageItemRepository)
        {
            _dbContext = context;
            _ARRecoveryPercentageRepository = ARRecoveryPercentageRepository;
            _ARRecoveryPercentageItemRepository = ARRecoveryPercentageItemRepository;
        }
        public IActionResult Index()
        {
            ViewBag.NavbarHeading = "List of Recovery Percentage";
            return View();
        }
        [HttpGet]
        public IActionResult Create(int? id)
        {
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string UserId = HttpContext.Session.GetString("UserId");
            var configValues = new ConfigValues(_dbContext);
            ViewBag.CategoryType = configValues.GetConfigValues("AR", "Category Type", companyId);
            //ViewBag.listOfItemCategories = new SelectList(from ac in _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.CategoryLevel == 2 && x.CompanyId == companyId && x.Code.StartsWith("01")).OrderBy(x => x.Code).ToList()
            //                                              where !_dbContext.ARSuplierItemsGroup.Where(x => x.ARCustomerId == id).Any(s => ac.Id.ToString().Contains(s.CategoryId.ToString()))
            //                                              select new
            //                                              {
            //                                                  Id = ac.Id,
            //                                                  Name = ac.Code + " - " + ac.Name
            //                                              }, "Id", "Name");

           
            ARRecoveryPercentageViewModel aRRecoveryPercentageViewModel = new ARRecoveryPercentageViewModel();
            if (id == null|| id==0)

            {
                ViewBag.EntityState = "Create";
                ViewBag.NavbarHeading = "Create Recovery Percentage";
                var items = _dbContext.AppCompanySetups.Where(x => x.Name == "Local Finish ItemCode").FirstOrDefault().Value;

                ViewBag.listOfItemCategories = new SelectList(from ac in _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.CategoryLevel == 2 /*&& x.CompanyId == companyId*/ && x.Code.StartsWith(items)).OrderBy(x => x.Code).ToList()
                                                              where !_dbContext.ARRecoveryPercentage.Where(x => x.IsDeleted == false && x.CompanyId == companyId).Any(s => ac.Id.ToString().Contains(s.ItemCategory_Id.ToString()))
                                                              select new
                                                              {
                                                                  Id = ac.Id,
                                                                  Name = ac.Code + " - " + ac.Name
                                                              }, "Id", "Name");
                var result = _ARRecoveryPercentageRepository.Get(x=>x.IsActive == true).ToList();
                if (result.Count > 0)
                {
                    ViewBag.Id = _ARRecoveryPercentageRepository.Get().Select(x => x.TransactionNo).Max() + 1;
                }
                else
                {
                    ViewBag.Id = 1;
                }
            }
            else
            {
                ViewBag.EntityState = "Update";
                ViewBag.NavbarHeading = "Update Recovery Percentage";
                
                List<ARRecoveryPercentageItem>  ARRecoveryPercentageItem = new List<ARRecoveryPercentageItem>();
                aRRecoveryPercentageViewModel.ARRecoveryPercentageItem = new List<ARRecoveryPercentageItem>();

                aRRecoveryPercentageViewModel.ARRecoveryPercentage = _ARRecoveryPercentageRepository.Get(x => x.Id == id).FirstOrDefault();
                ARRecoveryPercentageItem = _ARRecoveryPercentageItemRepository.Get(x => x.RecoveryPercentage_Id == id).ToList();

                var items = _dbContext.AppCompanySetups.Where(x => x.Name == "Local Finish ItemCode").FirstOrDefault().Value;

                ViewBag.listOfItemCategories = new SelectList(from ac in _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.CategoryLevel == 2 /*&& x.CompanyId == companyId*/ && x.Code.StartsWith(items)).OrderBy(x => x.Code).ToList()
                                                              where !_dbContext.ARRecoveryPercentage.Where(x => x.IsDeleted == false && x.Id!=id).Any(s => ac.Id.ToString().Contains(s.ItemCategory_Id.ToString()))
                                                              select new
                                                              {
                                                                  Id = ac.Id,
                                                                  Name = ac.Code + " - " + ac.Name
                                                              }, "Id", "Name");
                foreach (var item in ARRecoveryPercentageItem)
                {
                    item.CategoryType = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == item.CategoryType_Id).ConfigValue;
                    aRRecoveryPercentageViewModel.ARRecoveryPercentageItem.Add(item);
                }
            }
            
            return View(aRRecoveryPercentageViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ARRecoveryPercentageViewModel viewModel, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            var aRRecoveryPercentage = new ARRecoveryPercentage();
            if (viewModel.ARRecoveryPercentage.Id == 0)
                {
                    ARRecoveryPercentageItem[] aRRecoveryPercentageItem = JsonConvert.DeserializeObject<ARRecoveryPercentageItem[]>(collection["Details"]);
                    if (aRRecoveryPercentageItem.Count() > 0)
                    {
                        aRRecoveryPercentage.ItemCategory_Id = viewModel.ARRecoveryPercentage.ItemCategory_Id;
                        aRRecoveryPercentage.TransactionNo = viewModel.ARRecoveryPercentage.TransactionNo;
                        aRRecoveryPercentage.CreatedBy = userId;
                        aRRecoveryPercentage.CreatedDate = DateTime.Now;
                        aRRecoveryPercentage.CompanyId = companyId;
                        aRRecoveryPercentage.ResponsibilityId = resp_Id;
                        aRRecoveryPercentage.IsActive = true;
                        aRRecoveryPercentage.IsDeleted = false;
                        aRRecoveryPercentage.Status = "Created";

                    _dbContext.ARRecoveryPercentage.Add(aRRecoveryPercentage);
                        await _dbContext.SaveChangesAsync();


                        foreach (var items in aRRecoveryPercentageItem)
                        {
                            if (items.Id != 0)
                            {
                                ARRecoveryPercentageItem data = _dbContext.ARRecoveryPercentageItem.Where(i => i.RecoveryPercentage_Id == aRRecoveryPercentage.Id && i.Id == items.Id).FirstOrDefault();

                                    ARRecoveryPercentageItem obj = new ARRecoveryPercentageItem();
                                    obj = data;
                                    obj.FromPerc = items.FromPerc;
                                    obj.ToPerc = items.ToPerc;
                                    obj.CategoryType_Id = items.CategoryType_Id;

                                    _dbContext.ARRecoveryPercentageItem.Update(obj);
                                    _dbContext.SaveChanges();
                            }
                            else
                            {
                                ARRecoveryPercentageItem data = new ARRecoveryPercentageItem();
                                data = items;
                                data.CategoryType_Id = items.CategoryType_Id;
                                data.RecoveryPercentage_Id = aRRecoveryPercentage.Id;


                                _dbContext.ARRecoveryPercentageItem.Add(data);
                                _dbContext.SaveChanges();
                            }
                        }
                        TempData["error"] = "false";
                        TempData["message"] = "Recovery Percentage has been created successfully";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        TempData["error"] = "true";
                        TempData["message"] = "No any Recovery Percentage has been Created. It must contain atleast one item";
                        return RedirectToAction(nameof(Index));
                    }

                }
            else
                {
                    ARRecoveryPercentageItem[] aRRecoveryPercentageItem = JsonConvert.DeserializeObject<ARRecoveryPercentageItem[]>(collection["Details"]);
                    if (aRRecoveryPercentageItem.Count() > 0)
                    {
                        viewModel.ARRecoveryPercentage.UpdatedBy = userId;
                        viewModel.ARRecoveryPercentage.CompanyId = companyId;
                        viewModel.ARRecoveryPercentage.ResponsibilityId = resp_Id;
                        //updating existing data
                        var obj = _dbContext.ARRecoveryPercentage.Find(viewModel.ARRecoveryPercentage.Id);
                        obj.ItemCategory_Id = viewModel.ARRecoveryPercentage.ItemCategory_Id;
                        obj.TransactionNo = viewModel.ARRecoveryPercentage.TransactionNo;
                        //obj.Status = "Created";
                        obj.UpdatedBy = userId;
                        obj.CompanyId = viewModel.ARRecoveryPercentage.CompanyId;
                        obj.UpdatedDate = DateTime.Now;
                        obj.IsDeleted = false;
                        obj.IsActive = true;
                        var entry = _dbContext.ARRecoveryPercentage.Update(obj);
                        entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                        await _dbContext.SaveChangesAsync();


                        var removeItems = _dbContext.ARRecoveryPercentageItem.Where(u => u.RecoveryPercentage_Id == viewModel.ARRecoveryPercentage.Id).ToList();
                        if (removeItems != null)
                        {
                            foreach (var item in removeItems)
                            {
                                _dbContext.ARRecoveryPercentageItem.Remove(item);
                                _dbContext.SaveChanges();
                            }
                        }

                        foreach (var items in aRRecoveryPercentageItem)
                        {
                            if (items.Id != 0)
                            {
                                ARRecoveryPercentageItem data = _dbContext.ARRecoveryPercentageItem.Where(i => i.RecoveryPercentage_Id == aRRecoveryPercentage.Id && i.Id == items.Id).FirstOrDefault();

                                ARRecoveryPercentageItem objDetail = new ARRecoveryPercentageItem();
                                objDetail = data;
                                objDetail.FromPerc = items.FromPerc;
                                objDetail.ToPerc = items.ToPerc;
                                objDetail.CategoryType_Id = items.CategoryType_Id;

                                _dbContext.ARRecoveryPercentageItem.Update(objDetail);
                                _dbContext.SaveChanges();
                            }
                            else
                            {
                                ARRecoveryPercentageItem data = new ARRecoveryPercentageItem();
                                data = items;
                                data.CategoryType_Id = items.CategoryType_Id;
                                data.RecoveryPercentage_Id = obj.Id;


                                _dbContext.ARRecoveryPercentageItem.Add(data);
                                _dbContext.SaveChanges();
                            }
                        }
                    
                    }
                    else
                    {
                        TempData["error"] = "true";
                        TempData["message"] = "No any Recovery Percentage has been Created. It must contain atleast one item";
                        return RedirectToAction(nameof(Index));
                    }
                    TempData["error"] = "false";
                    TempData["message"] = "Recovery Percentage has been updated successfully";
                    return RedirectToAction(nameof(Index));
            }
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
                //var searchValue = Request.Form["search[value]"].FirstOrDefault();
                var searchTransNo = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchItemCat = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchStatus = Request.Form["columns[2][search][value]"].FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var RecoveryPercentageData = (from RecoveryPercentage in _dbContext.ARRecoveryPercentage.Where(x => x.IsDeleted == false && x.CompanyId == companyId) select RecoveryPercentage);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    RecoveryPercentageData = RecoveryPercentageData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                RecoveryPercentageData = !string.IsNullOrEmpty(searchTransNo) ? RecoveryPercentageData.Where(m => m.TransactionNo.ToString().Contains(searchTransNo)) : RecoveryPercentageData;
                RecoveryPercentageData = !string.IsNullOrEmpty(searchItemCat) ? RecoveryPercentageData.Where(m => _dbContext.InvItemCategories.FirstOrDefault(x => x.Id == Convert.ToInt32(m.ItemCategory_Id)).Name.ToUpper().Contains(searchItemCat.ToUpper())  || _dbContext.InvItemCategories.FirstOrDefault(x => x.Id == Convert.ToInt32(m.ItemCategory_Id)).Code.ToUpper().Contains(searchItemCat.ToUpper())) : RecoveryPercentageData;
                RecoveryPercentageData = !string.IsNullOrEmpty(searchStatus) ? RecoveryPercentageData.Where(m => m.Status.ToString().ToUpper().Contains(searchStatus.ToUpper())) : RecoveryPercentageData;

                recordsTotal = RecoveryPercentageData.Count();
                var data = RecoveryPercentageData.ToList();
                if (pageSize == -1)
                {
                    data = RecoveryPercentageData.Skip(skip).Take(recordsTotal == 0 ? 1 : pageSize == -1 ? recordsTotal : 1).ToList();
                }
                else
                {
                    data = RecoveryPercentageData.Skip(skip).Take(pageSize).ToList();
                }
                List<ARRecoveryPercentageViewModel> Details = new List<ARRecoveryPercentageViewModel>();
                foreach (var grp in data)
                {
                    ARRecoveryPercentageViewModel aRRecoveryPercentageViewModel = new ARRecoveryPercentageViewModel();
                    aRRecoveryPercentageViewModel.CategoryName = _dbContext.InvItemCategories.FirstOrDefault(x => x.Id == grp.ItemCategory_Id).Name;
                    aRRecoveryPercentageViewModel.CategoryCode = _dbContext.InvItemCategories.FirstOrDefault(x => x.Id == grp.ItemCategory_Id).Code;
                    aRRecoveryPercentageViewModel.ItemCategory = aRRecoveryPercentageViewModel.CategoryCode + " - " + aRRecoveryPercentageViewModel.CategoryName;
                    aRRecoveryPercentageViewModel.ARRecoveryPercentage = grp;
                    aRRecoveryPercentageViewModel.ARRecoveryPercentage.Approve = approve;
                    aRRecoveryPercentageViewModel.ARRecoveryPercentage.Unapprove = unApprove;
                    Details.Add(aRRecoveryPercentageViewModel);

                }
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = Details };
                return Ok(jsonData);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var deleteRecoveryPercentage = _dbContext.ARRecoveryPercentage.Find(id);
            if (deleteRecoveryPercentage != null)
            {
                deleteRecoveryPercentage.IsDeleted = true;
                deleteRecoveryPercentage.IsActive = false;
                var entry = _dbContext.ARRecoveryPercentage.Update(deleteRecoveryPercentage);
                entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                await _dbContext.SaveChangesAsync();
                TempData["error"] = "false";
                TempData["message"] = "Recovery Percentage has been deleted successfully.";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong.";
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Approve(int id)
        {
            string userId = HttpContext.Session.GetString("UserId");
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var model = _dbContext.ARRecoveryPercentage.Where(i => i.Id== id && i.Status != "Approved" && !i.IsDeleted && i.CompanyId == companyId).FirstOrDefault();
            model.ApprovedDate = DateTime.Now;         
            model.ApprovedBy = userId;         
            model.IsApproved = true;
            model.Status = "Approved";
            _dbContext.ARRecoveryPercentage.Update(model);
            await _dbContext.SaveChangesAsync();
            ViewBag.NavbarHeading = "Recovery Percentage Approved successfully ";
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> UnApprove(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var model = _dbContext.ARRecoveryPercentage.Where(i => i.Id == id  && !i.IsDeleted && i.CompanyId == companyId).FirstOrDefault();
            model.UnApprovedDate = DateTime.Now;
            model.UnApprovedBy = userId;
            model.IsApproved = false;
            model.Status = "Created";
            _dbContext.ARRecoveryPercentage.Update(model);
            await _dbContext.SaveChangesAsync();
            ViewBag.NavbarHeading = "Recovery Percentage Un-Approved successfully ";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            ARRecoveryPercentageViewModel aRRecoveryPercentageViewModel = new ARRecoveryPercentageViewModel();

            if (id == 0)
            {

                ViewBag.EntityState = "Create";
                ViewBag.NavbarHeading = "Create Recovery Percentage";
                var configValues = new ConfigValues(_dbContext);
                ViewBag.CategoryType = configValues.GetConfigValues("AR", "Category Type", companyId);
                var items = _dbContext.AppCompanySetups.Where(x => x.Name == "Local Finish ItemCode").FirstOrDefault().Value;
                ViewBag.listOfItemCategories = new SelectList(from ac in _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.CategoryLevel == 2 && x.CompanyId == companyId && x.Code.StartsWith(items)).OrderBy(x => x.Code).ToList()
                                                              where !_dbContext.ARRecoveryPercentage.Where(x => x.IsDeleted == false).Any(s => ac.Id.ToString().Contains(s.ItemCategory_Id.ToString()))
                                                              select new
                                                              {
                                                                  Id = ac.Id,
                                                                  Name = ac.Code + " - " + ac.Name
                                                              }, "Id", "Name");


                return View(new ARRecoveryPercentageViewModel());
            }
            else
            {
                ViewBag.EntityState = "Update";
                ViewBag.NavbarHeading = "Update Recovery Percentage";

                List<ARRecoveryPercentageItem> ARRecoveryPercentageItem = new List<ARRecoveryPercentageItem>();
                aRRecoveryPercentageViewModel.ARRecoveryPercentageItem = new List<ARRecoveryPercentageItem>();

                aRRecoveryPercentageViewModel.ARRecoveryPercentage = _ARRecoveryPercentageRepository.Get(x => x.Id == id).FirstOrDefault();
                ARRecoveryPercentageItem = _ARRecoveryPercentageItemRepository.Get(x => x.RecoveryPercentage_Id == id).ToList();

                var items = _dbContext.AppCompanySetups.Where(x => x.Name == "Local Finish ItemCode").FirstOrDefault().Value;

                ViewBag.listOfItemCategories = new SelectList(from ac in _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.CategoryLevel == 2 && x.CompanyId == companyId && x.Code.StartsWith(items)).OrderBy(x => x.Code).ToList()
                                                              where !_dbContext.ARRecoveryPercentage.Where(x => x.IsDeleted == false && x.Id != id).Any(s => ac.Id.ToString().Contains(s.ItemCategory_Id.ToString()))
                                                              select new
                                                              {
                                                                  Id = ac.Id,
                                                                  Name = ac.Code + " - " + ac.Name
                                                              }, "Id", "Name");
                foreach (var item in ARRecoveryPercentageItem)
                {
                    item.CategoryType = _dbContext.AppCompanyConfigs.FirstOrDefault(x => x.Id == item.CategoryType_Id).ConfigValue;
                    aRRecoveryPercentageViewModel.ARRecoveryPercentageItem.Add(item);
                }
            }
            return View(aRRecoveryPercentageViewModel);
        }




    }
}
