using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Repository.AppModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Areas.Security.Controllers
{
    [Area("Security")]
    [Authorize]
    public class SecurityProfileController : Controller
    {
        private readonly Sys_ORG_ProfileRepository _ORG_ProfileRepository;
        private readonly Sys_ORG_Profile_DetailsRepository _ORG_Profile_DetailsRepository;
        private readonly NumbersDbContext _dbContext;

        public SecurityProfileController(NumbersDbContext context, Sys_ORG_ProfileRepository ORG_ProfileRepository, Sys_ORG_Profile_DetailsRepository ORG_Profile_DetailsRepository)
        {
            _dbContext = context;
            _ORG_ProfileRepository = ORG_ProfileRepository;
            _ORG_Profile_DetailsRepository = ORG_Profile_DetailsRepository;
        }

        public async Task<IActionResult> Index(int? Id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var securityProfileVM = new SecurityProfileVM();
            if (Id.HasValue)
            {
                int id = Convert.ToInt32(Id);
                
                securityProfileVM.sys_ORG_Profile = await _ORG_ProfileRepository.GetByIdAsync(id);
                securityProfileVM.Sys_ORG_Profile_Details = _ORG_Profile_DetailsRepository.Get(x => x.SysORGProfile_ID == id).ToList();
                //ViewBag.ItemId = securityProfileVM.sys_ORG_Profile.CalssificationId;
                securityProfileVM.ClassificationList = new SelectList(_dbContext.AppCompanyConfigs.Where((a => a.Id == securityProfileVM.sys_ORG_Profile.CalssificationId && a.BaseId == 1066))
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   code = a.Id,
                                                   text = a.ConfigDescription,
                                               })
                                               .ToList(), "id", "text");
                ViewBag.Organization = (from c in _dbContext.SysOrganization select c).ToList();
                return View(securityProfileVM);
            }
            securityProfileVM.ClassificationList = new SelectList(_dbContext.AppCompanyConfigs.Where((a => a.BaseId == 1066))
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   code = a.Id,
                                                   text = a.ConfigDescription,
                                               })
                                               .ToList(), "id", "text");
            return View(securityProfileVM);
        }

        public IActionResult List()
        {
            var securityProfileVMList = new List<SecurityProfileVM>();
            var model =  _ORG_ProfileRepository.Get(x=>x.IsActive).ToList();
            foreach(var modl in model)
            {
                var securityProfileVM = new SecurityProfileVM();
                securityProfileVM.sys_ORG_Profile = modl;
                securityProfileVM.Classification = (from c in _dbContext.AppCompanyConfigs where c.Id == modl.CalssificationId select c.ConfigValue).FirstOrDefault();
                securityProfileVMList.Add(securityProfileVM);

            }
            return View(securityProfileVMList);
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(SecurityProfileVM securityProfileVM, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            if (securityProfileVM.sys_ORG_Profile.ID == 0)
            {
                Sys_ORG_Profile sys_ORG_Profile = new Sys_ORG_Profile();
                sys_ORG_Profile.Creation_Date = DateTime.Now;
                sys_ORG_Profile.CreatedBy = userId;
                sys_ORG_Profile.CompanyId = companyId;
                sys_ORG_Profile.IsActive = true;
                sys_ORG_Profile.CalssificationId= Convert.ToInt32(collection["CalssificationId"]);
                sys_ORG_Profile.ProfileName = Convert.ToString(collection["ProfileName"]);
                await _ORG_ProfileRepository.CreateAsync(sys_ORG_Profile);
                var rownber = collection["OrganizationName"].Count;
                for (int i = 0; i < rownber; i++)
                {
                    var org = new Sys_ORG_Profile_Details();
                    org.Organization_ID = Convert.ToInt32(collection["Organization_ID"][i]);
                    org.IsEnable = Convert.ToBoolean(collection["IsEnable"][i]);
                    org.SysORGProfile_ID = sys_ORG_Profile.ID;
                    await _ORG_Profile_DetailsRepository.CreateAsync(org);
                }
            }
            else
            {
                var sys_ORG_Profile = await _ORG_ProfileRepository.GetByIdAsync(securityProfileVM.sys_ORG_Profile.ID);
                sys_ORG_Profile.Updation_Date = DateTime.Now;
                sys_ORG_Profile.UpdatedBy = userId;
                sys_ORG_Profile.CompanyId = companyId;
                sys_ORG_Profile.CalssificationId = Convert.ToInt32(collection["CalssificationId"]);
                sys_ORG_Profile.ProfileName = Convert.ToString(collection["ProfileName"]);
                await _ORG_ProfileRepository.UpdateAsync(sys_ORG_Profile);
                var rownber = collection["OrganizationName"].Count;
                for (int i = 0; i<rownber; i++)
                {
                    int id= Convert.ToInt32(collection["ID"][i]);
                    var org = _ORG_Profile_DetailsRepository.Get(x=>x.ID== id).FirstOrDefault();
                    org.Organization_ID = Convert.ToInt32(collection["Organization_ID"][i]);
                    org.IsEnable = Convert.ToBoolean(collection["IsEnable"][i]);
                    org.SysORGProfile_ID = sys_ORG_Profile.ID;
                    await _ORG_Profile_DetailsRepository.UpdateAsync(org);
                }
        }
    return RedirectToAction("Index");
 }
       
        [HttpGet]
        public IActionResult GetOrganizationName(int id)
        {
            var organizaions = (from m in _dbContext.SysOrgClassification where m.ClassificationId == id select m.OrganizationId).ToList();
            var oragization = (from m in _dbContext.SysOrganization
                       from o in organizaions
                       where m.Organization_Id == o
                       select m).ToList();
            return Ok(oragization);
        }
    }
}
