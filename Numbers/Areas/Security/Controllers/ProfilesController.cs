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
using Numbers.Repository.Helpers;

namespace Numbers.Areas.Security.Controllers
{
    [Area("Security")]
    [Authorize]
    public class ProfilesController : Controller
    {
        private readonly NumbersDbContext _dbContext;


        public ProfilesController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public IActionResult Index()
        {
            IEnumerable<SysProfileValues> Module = _dbContext.SysProfileValues.Where(x=>x.IsActive==true).ToList();
            return View(Module);
        }

        public IActionResult Create(int? id)
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId");

            ViewBag.Responsibility = new SelectList(_dbContext.Sys_Responsibilities.ToList(), "Resp_Id", "Resp_Name");

            ViewBag.SecurityProfile =new SelectList (( from a in _dbContext.SysOrganization
                                       from d in _dbContext.SysOrgClassification.Where(x => x.OrganizationId == a.Organization_Id && x.ClassificationId == 179)
                                       select new
                                       {
                                           Id = a.Organization_Id,
                                           Name = a.OrgName
                                       }).ToList(), "Id", "Name");

            if (id == null)
            {
                ViewBag.EntityState = "Create";
                ViewBag.NavbarHeading = "Create Profile";
                ViewBag.Id = null;
                SysProfileValues sysProfileValues = new SysProfileValues();
                return View(sysProfileValues);
            }
            else
            {

                ViewBag.EntityState = "Update";
                ViewBag.NavbarHeading = "Update Profile";
                SysProfileValues model = _dbContext.SysProfileValues.Find(id);
                model.SysProfileValuesDetailList = _dbContext.SysProfileValues.Where(a => a.Doc_Id == id && a.IsDeleted == false).ToList();

                return View(model);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Create(SysProfileValues sysProfileValues, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");


            if (sysProfileValues.Doc_Id == 0)
            {
                var respId = Convert.ToInt32(collection["Responsibilites"]);
                if (respId != 0)
                {
                    var respItems = _dbContext.SysProfileValues.Where(x => x.RespId == respId).ToList();
                    foreach (var item in respItems)
                    {
                        item.IsActive = false;
                        item.IsDeleted = true;
                        _dbContext.Update(item);
                        _dbContext.SaveChanges();
                    }
                }
                sysProfileValues.CompanyId = companyId;
                sysProfileValues.CreatedBy = userId;
                sysProfileValues.CreatedDate = DateTime.Now;
                sysProfileValues.IsDeleted = false;
                sysProfileValues.IsActive = true;

                _dbContext.SysProfileValues.Add(sysProfileValues);
                _dbContext.SaveChanges();

                var rownber =collection["ProfileOptionName"].Count;
                for (int i = 0; i < rownber; i++)
                {
                    var profiles = new SysProfileValues();
                    profiles.RespId = Convert.ToInt32(collection["Responsibilites"]);
                    profiles.ProfileOption = Convert.ToString(collection["ProfileOptionName"][i]);
                    profiles.RespValues = Convert.ToString(collection["ResponField"][i]);

                    _dbContext.SysProfileValues.Add(profiles);
                    await _dbContext.SaveChangesAsync();

                }

            }
            else
            {

                var data = _dbContext.SysProfileValues.Find(sysProfileValues.Doc_Id);

                sysProfileValues.CompanyId = companyId;
                sysProfileValues.LastUpdatedBy = sysProfileValues.LastUpdatedBy;
                sysProfileValues.LastUpdatedDate = DateTime.Now;

                _dbContext.SysProfileValues.Update(sysProfileValues);
                _dbContext.SaveChanges();

                var rownber = collection["ProfileOptionName"].Count;
                for (int i = 0; i < rownber; i++)
                {
                    var profiles = new SysProfileValues();
                    profiles.RespId = Convert.ToInt32(collection["Responsibilites"]);
                    profiles.ProfileOption = Convert.ToString(collection["ProfileOptionName"][i]);
                    profiles.RespValues = Convert.ToString(collection["ResponField"][i]);

                    _dbContext.SysProfileValues.Add(profiles);
                    await _dbContext.SaveChangesAsync();

                }

            }

            return RedirectToAction(nameof(Index));
        }


        public IActionResult GetProfiles(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;

            var Profiles = _dbContext.AppCompanyConfigs.Where(x=>x.BaseId == 1068 && x.IsDeleted==false);
            //var organization = from a in _dbContext.SysOrganization
            //                   from d in _dbContext.SysOrgClassification.Where(x=>x.OrganizationId==a.Organization_Id && x.ClassificationId== 179)
            //                   select new {
            //                       id = a.Organization_Id,
            //                       Name =a.OrgName
            //                   };

            return Ok(Profiles);
        }
        public IActionResult GetOrganizations(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            //var Profiles = (from a in _dbContext.SysOrganization
            //                from d in _dbContext.SysOrgClassification.Where(x => x.OrganizationId == a.Organization_Id && x.ClassificationId == id)
            //                select new
            //                {
            //                    Id = a.Organization_Id,
            //                    Name = a.OrgName
            //                }).ToList();

            var Profiles = (
                            from a in _dbContext.Sys_ORG_Profile.Where(x => x.CalssificationId == id)
                            select new
                            {
                                Id = a.ID,
                                Name = a.ProfileName
                            }).ToList();
            return Ok(Profiles);
        }


        public async Task<IActionResult> DeleteProfile(int Id)
        {
            var Profile = await _dbContext.SysProfileValues.Where(n => n.Doc_Id == Id).FirstAsync();
            if (Profile != null)
            {
                Profile.IsActive = false;
                Profile.IsDeleted = true;
                _dbContext.SysProfileValues.Update(Profile);
                await _dbContext.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult GetProfileDetails(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var items = _dbContext.SysProfileValues.Where(x => x.RespId == id && x.IsDeleted == false);
            return Ok(items);
        }

    }
}