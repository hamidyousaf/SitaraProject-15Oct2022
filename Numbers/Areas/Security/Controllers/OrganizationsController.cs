using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Interface;
using Numbers.Repository.AppModule;
using Numbers.Repository.Helpers;
using Numbers.Repository.Inventory;

namespace Numbers.Areas.Security.Controllers
{
    [Area("Security")]
    [Authorize]
    public class OrganizationsController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        private readonly SysOrganizationRepository _sysOrganizationRepository;
        private readonly SysOrgClassificationRepository _sysOrgClassificationRepository;

        public OrganizationsController(NumbersDbContext context, SysOrganizationRepository sysOrganizationRepository, SysOrgClassificationRepository sysOrgClassificationRepository)
        {
            _dbContext = context;
            _sysOrganizationRepository = sysOrganizationRepository;
            _sysOrgClassificationRepository = sysOrgClassificationRepository;
        }

        public IActionResult Index()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userid = HttpContext.Session.GetString("UserId");
            var storeIssueRepo = new OrganizationRepo(_dbContext);
            IEnumerable<SysOrganization> list = storeIssueRepo.GetAll(companyId, userid);
            ViewBag.NavbarHeading = "List of Organization";
            return View(list);
        }

        public IActionResult Create(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var storeIssueRepo = new OrganizationRepo(_dbContext);
            var configValues = new ConfigValues(_dbContext);
            ViewBag.Counter = 0;
            ViewBag.WareHouse = configValues.GetConfigValues("Inventory", "Ware House", companyId);
            ViewBag.CostCenter = configValues.GetConfigValues("Inventory", "Cost Center", companyId);
            ViewBag.Company = new SelectList(_dbContext.AppCompanies.Where(x => x.IsActive == true).ToList(), "Id", "Name");
            if (id == 0)
            {
                TempData["Mode"] = false;
                ViewBag.EntityState = "Create";
                ViewBag.NavbarHeading = "Create Organization";

                var model = new OrganizationViewModel();
                return View(model);
            }
            else
            {
                TempData["Mode"] = true;
                ViewBag.Id = id;
                OrganizationViewModel modelEdit = storeIssueRepo.GetById(id);
                // SYS_MENU_D[] storeIssueItems = storeIssueRepo.GetStoreIssueItems(id);
                //modelEdit.InvStoreIssueItems = _dbContext.SYS_MENU_D
                //                        .Include(i => i.SYS_FORMS)
                //                        .Include(i => i.SYS_MENU_M)
                //                          .Where(i => i.MENU_M_ID == id)
                //                          .ToList();

                modelEdit.InvStoreIssueItems = _dbContext.SysOrgClassification.Where(i => i.OrganizationId == id).ToList();
                ViewBag.EntityState = "Update";
                ViewBag.NavbarHeading = "Organization";
                ViewBag.TitleStatus = "Created";
                return View(modelEdit);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Create(OrganizationViewModel model, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var menuRepo = new OrganizationRepo(_dbContext);
            if (model.Organization_Id == 0)
            {
                var newModel = new SysOrganization();
                //  newModel.Organization_Id = model.Organization_Id;
                newModel.OrgName = model.OrgName;
                newModel.OrgType = model.OrgType;
                newModel.EffectiveFromDate = model.EffectiveFromDate;
                newModel.EffectiveToDate = model.EffectiveToDate;
                newModel.OrgLocation = model.OrgLocation;
                newModel.OrgInternalExternal = model.OrgInternalExternal;
                newModel.OrgAddress = model.OrgAddress;
                newModel.OrgIntAddress = model.OrgIntAddress;
                newModel.ShortName = model.ShortName;
                newModel.CompanyId = model.CompanyId;
                //unitofwork.SysOrganizationRepository.Insert(newModel);
                await _sysOrganizationRepository.CreateAsync(newModel);
               // unitofwork.SaveChanges();
                SysOrgClassification[] InvStoreIssueItems = JsonConvert.DeserializeObject<SysOrgClassification[]>(collection["details"]);
                if (InvStoreIssueItems.Count() > 0)
                {
                    foreach (var items in InvStoreIssueItems)
                    {
                        if (items.ClassificationDetailId != 0)
                        {
                            //SysOrgClassification data = _dbContext.SysOrgClassification.Where(i => i.OrganizationId == newModel.Organization_Id && i.ClassificationDetailId == items.ClassificationDetailId).FirstOrDefault();
                            //SysOrgClassification data = unitofwork.SysOrgClassificationRepository.Get(i => i.OrganizationId == newModel.Organization_Id && i.ClassificationDetailId == items.ClassificationDetailId).FirstOrDefault();
                            SysOrgClassification data =  _sysOrgClassificationRepository.Get(i => i.OrganizationId == newModel.Organization_Id && i.ClassificationDetailId == items.ClassificationDetailId).FirstOrDefault();
                            
                            //foreach (var i in data)
                            //{

                            SysOrgClassification obj = new SysOrgClassification();
                            obj = data;
                            obj.OrganizationId = newModel.Organization_Id;
                            obj.ClassificationId = items.ClassificationId;
                            obj.Ledger_Id = items.Ledger_Id;
                            obj.OperationUnitId = items.OperationUnitId;
                            obj.BusinessUnitId = items.BusinessUnitId;
                            await _sysOrgClassificationRepository.UpdateAsync(obj);
                            

                            // _dbContext.SysOrgClassification.Update(obj);
                            //_dbContext.SaveChanges();
                            //}
                        }
                        else
                        {
                            SysOrgClassification data = new SysOrgClassification();
                            ///data = items;
                            data.OrganizationId = newModel.Organization_Id;
                            data.ClassificationId = items.ClassificationId;
                            data.Ledger_Id = items.Ledger_Id;
                            data.OperationUnitId = items.OperationUnitId;
                            data.BusinessUnitId = items.BusinessUnitId;
                            await _sysOrgClassificationRepository.CreateAsync(data);
                           // _dbContext.SysOrgClassification.Add(data);
                           // _dbContext.SaveChanges();
                        }
                    }
                }
                /*bool isSuccess = await menuRepo.Create(model, collection);
                if (isSuccess == true)
                {
                    TempData["error"] = "false";
                    TempData["message"] = "Organization  has been created successfully.";
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Something went wrong.";
                }*/
                return RedirectToAction(nameof(Index));
            }
            else
            {
                model.LastUpdatedBy = userId;
                //bool isSuccess = await menuRepo.Update(model, collection);
                //var obj = unitofwork.SysOrganizationRepository.GetByID(model.Organization_Id);
                var obj = _sysOrganizationRepository.Get(x=>x.Organization_Id==model.Organization_Id).FirstOrDefault();
                obj.OrgName = model.OrgName;
                obj.OrgType = model.OrgType;
                obj.EffectiveFromDate = model.EffectiveFromDate;
                obj.EffectiveToDate = model.EffectiveToDate;
                obj.OrgLocation = model.OrgLocation;
                obj.OrgInternalExternal = model.OrgInternalExternal;
                obj.OrgAddress = model.OrgAddress;
                obj.OrgIntAddress = model.OrgIntAddress;
                obj.ShortName = model.ShortName;
                obj.LastUpdatedDate = DateTime.Now;
                obj.CompanyId = model.CompanyId;
                await _sysOrganizationRepository.UpdateAsync(obj);
               // unitofwork.SysOrganizationRepository.Update(obj);
                //unitofwork.SaveChanges();
                SysOrgClassification[] InvStoreIssueItems = JsonConvert.DeserializeObject<SysOrgClassification[]>(collection["details"]);
                if (InvStoreIssueItems.Count() > 0)
                {
                    foreach (var items in InvStoreIssueItems)
                    {
                        if (items.ClassificationDetailId != 0)
                        {
                            SysOrgClassification data = _sysOrgClassificationRepository.Get(i => i.OrganizationId == obj.Organization_Id && i.ClassificationDetailId == items.ClassificationDetailId).FirstOrDefault();
                            SysOrgClassification obj2 = new SysOrgClassification();
                            obj2 = data;
                            obj2.OrganizationId = obj.Organization_Id;
                            obj2.ClassificationId = items.ClassificationId;
                            obj2.Ledger_Id = items.Ledger_Id;
                            obj2.OperationUnitId = items.OperationUnitId;
                            obj2.BusinessUnitId = items.BusinessUnitId;
                            await _sysOrgClassificationRepository.UpdateAsync(obj2);
                        }
                        else
                        {
                            SysOrgClassification data = new SysOrgClassification();
                            ///data = items;
                            data.OrganizationId = obj.Organization_Id;
                            data.ClassificationId = items.ClassificationId;
                            data.Ledger_Id = items.Ledger_Id;
                            data.OperationUnitId = items.OperationUnitId;
                            data.BusinessUnitId = items.BusinessUnitId;
                            await _sysOrgClassificationRepository.CreateAsync(data);
                        }
                    }
                }
                /* if (isSuccess == true)
                 {
                     TempData["error"] = "false";
                     TempData["message"] = "Organization has been updated successfully.";
                 }
                 else
                 {
                     TempData["error"] = "true";
                     TempData["message"] = "Something went wrong.";
                 }*/
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Delete(int Id)
        {
            var obj = await _sysOrganizationRepository.GetByIdAsync(Id);
            await _sysOrganizationRepository.DeleteAsync(obj);
            return RedirectToAction(nameof(Index));
        }

   }
}