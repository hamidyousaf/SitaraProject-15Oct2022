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
using Numbers.Repository.AppModule;
using Numbers.Repository.Helpers;

namespace Numbers.Controllers
{
    [Area("ApplicationModule")]
    [Authorize]
    public class Sys_ResponsibilitiesController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        private readonly ResponsibilityRepository _responsibilityRepository;
        public Sys_ResponsibilitiesController(NumbersDbContext context, ResponsibilityRepository responsibilityRepository)
        {
            _dbContext = context;
            _responsibilityRepository = responsibilityRepository;
        }
        public IActionResult Index(int? id)
        {
            var configValues = new ConfigValues(_dbContext);
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            ViewBag.Companies = new SelectList( _dbContext.AppCompanies.ToList(),"Id","Name");
            ViewBag.ItemCategory = new SelectList(( from a in  _dbContext.InvItemCategories.Where(x=>x.IsDeleted==false && x.IsActive==true && x.AccountLevel!=4).ToList() select new { 
            Id =a.Id, Name =string.Concat(a.Code, '-',a.Name )}),"Id","Name");
            ViewBag.POType = configValues.GetConfigValues("AP", "Purchase Order Type", companyId);
            ViewBag.SubAccount = _dbContext.GLSubAccounts.ToList();
            ViewBag.SubAccounts = _dbContext.GLSubAccountDetails.ToList();
            var Costcenters = _dbContext.CostCenter.Where(x => x.IsDeleted == false).ToList();
           // ViewBag.Costcenters = _dbContext.CostCenter.Where(x => x.IsDeleted == false).ToList();
            var Departments = _dbContext.GLDivision.Where(x => x.IsDeleted == false).ToList();
            //ViewBag.Departments = _dbContext.GLDivision.Where(x => x.IsDeleted == false).ToList();
            var SubDepartments = _dbContext.GLSubDivision.Where(x => x.IsDeleted == false).ToList();
            //ViewBag.SubDepartments = _dbContext.GLSubDivision.Where(x => x.IsDeleted == false).ToList();
           // string[] arr = new string[] { "Hi" };
            //List<GLDivision> ls = Departments.ToList();
            var dep = new GLDivision();
            dep.Id = 0;
            dep.Name = "";
            Departments.Add(dep);
            //List<GLSubDivision> subD = SubDepartments.ToList();
            var subdep = new GLSubDivision();
            subdep.Id = 0;
            subdep.Name = "";
            SubDepartments.Add(subdep);
            //List<CostCenter> costcenter = Costcenters.ToList();
            var cost = new CostCenter();
            cost.Id = 0;
            cost.Description = "";
            Costcenters.Add(cost);
            ViewBag.Departments = Departments.OrderBy(x=>x.Id);
            ViewBag.SubDepartments = SubDepartments.OrderBy(x=>x.Id);
            ViewBag.Costcenters = Costcenters.OrderBy(x=>x.Id);
            ViewBag.Menus = new SelectList(_dbContext.Sys_Responsibilities.Where(
                                               (a => a.CompanyId == companyId
                                              ))
                                              .Select(a => new
                                              {
                                                  id = a.Resp_Id,
                                                  code = a.Resp_Id,
                                                  text = a.Resp_Id + " : " + a.Resp_Name,
                                              }).OrderBy(a => a.text)
                                              .ToList(), "id", "text");

            if (id == null)
            {
                ViewBag.VoucherDetail = "[]";
                ViewBag.EntityState = "Create";
                ViewBag.NavbarHeading = "Create Responsibility";
                ViewBag.Id = null;

                Responsibilities Responsibilities = new Responsibilities();
                Responsibilities.Effective_To_Date = DateTime.Now.AddYears(1);
                return View(Responsibilities);
            }
            else
            {
              
                ViewBag.EntityState = "Update";
                ViewBag.NavbarHeading = "Update Responsibility";
                Responsibilities Responsibilities = new Responsibilities();
                Responsibilities Res = _dbContext.Sys_Responsibilities.Find(id);
                ViewBag.ItemId = Res.Menu_Id;
                var voucherDetail =  _dbContext.SysResponsibilityDetails.Where(x=>x.RespId==id).ToArray();
                ViewBag.VoucherDetail = JsonConvert.SerializeObject(voucherDetail);
                Res.ResponsibilityItemCategory = _dbContext.Sys_ResponsibilityItemCategory.Include(x=>x.ItemCategory).Where(x => x.ResponsibilityId == id).ToList();
                return View(Res);
            }
        }
        public async Task<IActionResult> List()
        {
            //IEnumerable<Responsibilities> Responsibilities = await _dbContext.Sys_Responsibilities.ToListAsync();
            IEnumerable<Responsibilities> Responsibilities = await _responsibilityRepository.GetAllAsync();
            return View(Responsibilities);
        }
        [HttpPost]
        public async Task<IActionResult> Create(Responsibilities responsibilities, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            if (responsibilities.Resp_Id == 0)
            {
               /* responsibilities.Resp_Name = responsibilities.Resp_Name;
                responsibilities.Description = responsibilities.Description;
                responsibilities.Effective_From_Date = responsibilities.Effective_From_Date;
                responsibilities.Effective_To_Date = responsibilities.Effective_To_Date;*/
                responsibilities.Created_By = userId;
                //responsibilities.CompanyId = companyId;
                responsibilities.Created_Date = DateTime.Now;
                /*responsibilities.Module_Id = responsibilities.Module_Id;
                responsibilities.Menu_Id = responsibilities.Menu_Id;*/
                await _responsibilityRepository.CreateAsync(responsibilities);
               // _dbContext.Sys_Responsibilities.Add(responsibilities);
                TempData["error"] = "false";
                TempData["message"] = "Responsibility has been added successfully.";

                var rownber = collection["ItemCategoryId"].Count;
                for (int i = 0; i < rownber; i++)
                {
                    var SysResponsibilityDet = new Sys_ResponsibilityItemCategory();
                    SysResponsibilityDet.ResponsibilityId = responsibilities.Resp_Id;
                    SysResponsibilityDet.ItemCategoryId = Convert.ToInt32(collection["ItemCategoryId"][i]);
                    _dbContext.Sys_ResponsibilityItemCategory.Add(SysResponsibilityDet);
                    await _dbContext.SaveChangesAsync();

                }


            }
            else
            {
               var config = _dbContext.Sys_Responsibilities.Find(responsibilities.Resp_Id);
               //var config =unitofwork.ResponsibilitiesRepository.GetByID(responsibilities.Resp_Id);

                config.Resp_Name = responsibilities.Resp_Name;
                config.Description = responsibilities.Description;
                config.Effective_From_Date = responsibilities.Effective_From_Date;
                config.Effective_To_Date = responsibilities.Effective_To_Date;
                config.Updated_By = userId;
                config.Updated_Date = DateTime.Now;
                config.Module_Id = responsibilities.Module_Id;
                config.Menu_Id = responsibilities.Menu_Id;
                config.TypeId = responsibilities.TypeId;
                config.CompanyId = responsibilities.CompanyId;
                await _responsibilityRepository.UpdateAsync(config);
              //  await _dbContext.SaveChangesAsync();
                TempData["error"] = "false";
                TempData["message"] = "Module has been Updated successfully.";

                var RespDetail = _dbContext.Sys_ResponsibilityItemCategory.Where(x => x.ResponsibilityId == config.Resp_Id).ToList();
               foreach(Sys_ResponsibilityItemCategory item in RespDetail)
                {
                    _dbContext.Sys_ResponsibilityItemCategory.Remove(item);
                }
                var rownber = collection["ItemCategoryId"].Count;
                for (int i = 0; i < rownber; i++)
                {
                   // var respDetailId = Convert.ToInt32(collection["RespDetailId"][i]);

                    //var SysResponsibilityDet = _dbContext.Sys_ResponsibilityItemCategory.Where(x => x.Id == respDetailId).FirstOrDefault();
                    //if (SysResponsibilityDet == null)
                    //{
                       var  SysResponsibilityDet = new Sys_ResponsibilityItemCategory();
                        SysResponsibilityDet.ResponsibilityId = responsibilities.Resp_Id;
                        SysResponsibilityDet.ItemCategoryId = Convert.ToInt32(collection["ItemCategoryId"][i]);

                        _dbContext.Sys_ResponsibilityItemCategory.Add(SysResponsibilityDet);
                        await _dbContext.SaveChangesAsync();
                    //}
                    //else
                    //{
                    //    SysResponsibilityDet.ResponsibilityId = responsibilities.Resp_Id;
                    //    SysResponsibilityDet.ItemCategoryId = Convert.ToInt32(collection["ItemCategoryId"][i]);
                    //    _dbContext.Sys_ResponsibilityItemCategory.Update(SysResponsibilityDet);
                    //    await _dbContext.SaveChangesAsync();
                    //}
                }

            }

            SysResponsibilityDetails[] voucherDetail = JsonConvert.DeserializeObject<SysResponsibilityDetails[]>(collection["VoucherDetail"]);
            foreach (var item in voucherDetail)
            {
                if (item.Id != 0) // New Voucher Detail Line
                {
                    SysResponsibilityDetails detail = new SysResponsibilityDetails();
                    detail = _dbContext.SysResponsibilityDetails.Find(item.Id);
                    detail.RespId = responsibilities.Resp_Id;
                    detail.Description = item.Description;
                    detail.DepartmentId = item.DepartmentId;
                    detail.SubDepartmentId = item.SubDepartmentId;
                    detail.CostCenterId = item.CostCenterId;
                    detail.IsDeleted = false;
                    var entry = _dbContext.SysResponsibilityDetails.Update(detail);
                    entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                }
                else
                {
                    SysResponsibilityDetails detail = new SysResponsibilityDetails();
                    detail.RespId = responsibilities.Resp_Id;
                    detail.Description = item.Description;
                    detail.DepartmentId = item.DepartmentId;
                    detail.SubDepartmentId = item.SubDepartmentId;
                    detail.CostCenterId = item.CostCenterId;
                    detail.IsDeleted = false;
                    _dbContext.SysResponsibilityDetails.Add(detail);
                }
                await _dbContext.SaveChangesAsync();

            }
            //await _dbContext.SaveChangesAsync();
            return Redirect("/ApplicationModule/Sys_Responsibilities/Index");
             
        }
    }
}
