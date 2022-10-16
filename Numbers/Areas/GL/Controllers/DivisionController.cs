using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Numbers.Helpers;
using Newtonsoft.Json;

namespace Numbers.Areas.GL.Controllers
{
    [Area("GL")]
    public class DivisionController : Controller
    {
        private readonly NumbersDbContext _dbContext;


        public DivisionController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index(int id)
        {
            if (id == 0)
            {
                DivisionVM divisionVM = new DivisionVM();
                divisionVM.GLDivision = new GLDivision();
                divisionVM.GLSubDivision = new GLSubDivision();
                divisionVM.GLSubDivisionList = new List<GLSubDivision>();

                divisionVM.GLDivisionList = _dbContext.GLDivision.Where(a => a.IsDeleted == false).ToList();
                return View(divisionVM);

            }

            else
            {

                DivisionVM divisionVM = new DivisionVM();
                divisionVM.GLDivision = _dbContext.GLDivision.Find(id);
                divisionVM.GLSubDivisionList = _dbContext.GLSubDivision.Where(a => a.IsDeleted == false && a.GLDivisionId == id).ToList();
                return View(divisionVM);
            }
        }

        public IActionResult List(int id)
        {

            DivisionVM divisionVM = new DivisionVM();
            divisionVM.GLDivision = new GLDivision();
            divisionVM.GLDivisionList = _dbContext.GLDivision.Where(a => a.IsDeleted == false).ToList();
            divisionVM.GLSubDivisionList = _dbContext.GLSubDivision.Where(a=> a.IsDeleted == false).ToList();
            ViewBag.NavbarHeading = "List of Departments";

            return View(divisionVM);
        }


        [HttpPost]
        public async Task<IActionResult> Create(DivisionVM division, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            try
            {
                GLSubDivision[] List = JsonConvert.DeserializeObject<GLSubDivision[]>(collection["detail"]);
                if (division.GLDivision.Id == 0)
                {
                    GLDivision model = new GLDivision();
                    model.Date = division.GLDivision.Date;
                    model.Name = division.GLDivision.Name;
                    model.IsActive = division.GLDivision.IsActive;
                    model.Description = division.GLDivision.Description;
                    model.CreatedBy = userId;
                    model.CreatedDate = DateTime.Now;
                    model.CompanyId = companyId;
                    model.IsDeleted = false;
                    _dbContext.GLDivision.Add(model);
                    await _dbContext.SaveChangesAsync();
                    foreach(var detail in List)
                    {
                        var glsubDivisions = new GLSubDivision();
                        glsubDivisions = detail;
                        glsubDivisions.CompanyId = companyId;
                        glsubDivisions.GLDivisionId = model.Id;
                        glsubDivisions.CreatedBy = userId;
                        glsubDivisions.CreatedDate = DateTime.Now;
                        glsubDivisions.IsDeleted = false;
                        glsubDivisions.IsActive = division.GLDivision.IsActive;
                        _dbContext.GLSubDivision.Add(glsubDivisions);
                        await _dbContext.SaveChangesAsync();
                    }
                    return RedirectToAction(nameof(List));
                }
                else
                {
                    GLDivision model = _dbContext.GLDivision.Find(division.GLDivision.Id);
                    model.Date = division.GLDivision.Date;
                    model.Name = division.GLDivision.Name;
                    model.Description = division.GLDivision.Description;
                    model.IsActive = division.GLDivision.IsActive;
                    model.UpdatedBy = userId;
                    model.UpdatedDate = DateTime.Now;
                    model.CompanyId = companyId;
                    model.IsDeleted = false;
                    _dbContext.GLDivision.Update(model);
                    await _dbContext.SaveChangesAsync();
                    var existingList = _dbContext.GLSubDivision.Where(x => x.GLDivisionId == division.GLDivision.Id).ToList();
                    //Deleting monthly target limit
                    foreach (var item in existingList)
                    {
                        bool isExist = List.Any(x => x.Id == item.Id);
                        if (!isExist)
                        {
                            _dbContext.GLSubDivision.Remove(item);
                            _dbContext.SaveChanges();
                        }
                    }
                    //Inserting/Updating Subdivison
                    foreach (var detail in List)
                    {
                        if (detail.Id == 0) //Inserting New Records
                        {
                            var glsubDivisions = new GLSubDivision();
                            glsubDivisions = detail;
                            glsubDivisions.CompanyId = companyId;
                            glsubDivisions.GLDivisionId = model.Id;
                            glsubDivisions.CreatedBy = userId;
                            glsubDivisions.CreatedDate = DateTime.Now;
                            glsubDivisions.IsDeleted = false;
                            glsubDivisions.IsActive = division.GLDivision.IsActive;
                            _dbContext.GLSubDivision.Add(glsubDivisions);
                            _dbContext.SaveChanges();
                        }
                        else   //Updating Records
                        {
                            var itemdetail = _dbContext.GLSubDivision.Where(x => x.Id == detail.Id).FirstOrDefault();
                            itemdetail.Name = detail.Name;
                            itemdetail.Description = detail.Description;
                            itemdetail.GLDivisionId = model.Id;
                            itemdetail.CreatedBy = userId;
                            itemdetail.CreatedDate = DateTime.Now;
                            itemdetail.IsDeleted = false;
                            itemdetail.IsActive = division.GLDivision.IsActive;
                            _dbContext.GLSubDivision.Update(itemdetail);
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message.ToString());
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult GetList()
        {
            try
            {
                int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                string userId = HttpContext.Session.GetString("UserId");
                var approve = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == resp_Id).FirstOrDefault().Approve;
                var unApprove = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == resp_Id).FirstOrDefault().UnApprove;

                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                //var searchValue = Request.Form["search[value]"].FirstOrDefault();
                var searchId = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchDate = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchDepartment = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchSubDepartment = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchStatus = Request.Form["columns[4][search][value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var GLSubDivisionData = (from GLSubDivision in _dbContext.GLSubDivision.Where(x => x.IsDeleted == false ) 
                                         join GLDivision in _dbContext.GLDivision.Where(x => x.IsDeleted == false && x.CompanyId == companyId)
                                         on GLSubDivision.GLDivisionId equals GLDivision.Id 
                                         select new GLSubDivisionVM
                                         {
                                            GLDivision = GLDivision,
                                            GLSubDivision = GLSubDivision
                                         });

                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    GLSubDivisionData = GLSubDivisionData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                //if (!string.IsNullOrEmpty(searchValue))
                //{
                //    GLDivisionData = GLDivisionData.Where(m => m.Id.ToString().Contains(searchValue)
                //                                   || m.Date.ToString(CommonHelper.DateFormat).ToUpper().Contains(searchValue.ToUpper())
                //                                   || m.Name.ToString().ToUpper().Contains(searchValue.ToUpper())
                //                                   || _dbContext.GLSubDivision.FirstOrDefault(x => x.GLDivisionId == Convert.ToInt32(m.Id)).Name.ToUpper().Contains(searchValue.ToUpper())
                //                                   || m.Status.ToString().ToUpper().Contains(searchValue.ToUpper())
                //                                   );
                //}

                GLSubDivisionData = !string.IsNullOrEmpty(searchId) ? GLSubDivisionData.Where(m => m.GLDivision.Id.ToString().Contains(searchId.ToString())) : GLSubDivisionData;
                GLSubDivisionData = !string.IsNullOrEmpty(searchDate) ? GLSubDivisionData.Where(m => m.GLDivision.Date.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchDate.ToUpper())) : GLSubDivisionData;
                GLSubDivisionData = !string.IsNullOrEmpty(searchDepartment) ? GLSubDivisionData.Where(m => m.GLDivision.Name.ToString().ToUpper().Contains(searchDepartment.ToUpper())) : GLSubDivisionData;
                GLSubDivisionData = !string.IsNullOrEmpty(searchSubDepartment) ? GLSubDivisionData.Where(m => m.GLSubDivision.Name.ToString().ToUpper().Contains(searchSubDepartment.ToUpper())) : GLSubDivisionData;
                GLSubDivisionData = !string.IsNullOrEmpty(searchStatus) ? GLSubDivisionData.Where(m => m.GLDivision.Status != null ? m.GLDivision.Status.ToString().ToUpper().Contains(searchStatus.ToUpper()) : false) : GLSubDivisionData;

                recordsTotal = GLSubDivisionData.Count();
                var data = GLSubDivisionData.Skip(skip).Take(pageSize).ToList();
                List<GLSubDivisionVM> Details = new List<GLSubDivisionVM>();
                foreach (var grp in data)
                {
                    GLSubDivisionVM gLSubDivisionVM = new GLSubDivisionVM();
                    gLSubDivisionVM.Date = grp.GLDivision.Date.ToString(Helpers.CommonHelper.DateFormat);
                    gLSubDivisionVM.GLDivision = grp.GLDivision;
                    gLSubDivisionVM.GLSubDivision = grp.GLSubDivision;
                    gLSubDivisionVM.GLSubDivision.Approve = approve;
                    gLSubDivisionVM.GLSubDivision.Unapprove = unApprove;
                    Details.Add(gLSubDivisionVM);

                }

                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = Details };
                return Ok(jsonData);

            }
            catch (Exception)
            {
                throw;
            }
        }


        public bool CheckName(string name)
        {
            var checkName = _dbContext.GLDivision.Where(a => a.Name == name && a.IsDeleted == false).Count();
            if (checkName > 0)
            {
                return true;

            }
            else
            {
                return false;

            }


        }
        public bool Checkdesc(string name)
        {
            var checkName = _dbContext.GLDivision.Where(a => a.Name == name && a.IsDeleted == false).Count();
            if (checkName > 0)
            {
                return true;

            }
            else
            {
                return false;

            }


        }

        public   IActionResult Approve(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            GLDivision model = _dbContext.GLDivision.Find(id);
            model.ApprovedBy = _userId;
            model.ApprovedDate = DateTime.UtcNow;
            model.IsApproved = true;
            model.Status = "Approved";
              _dbContext.GLDivision.Update(model);
            _dbContext.SaveChanges();
            TempData["error"] = "false";
            TempData["message"] = "Department has been approved successfully.";
            return RedirectToAction("List", "Division");
        }
        public IActionResult UnApprove(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            GLDivision model = _dbContext.GLDivision.Find(id);
            model.ApprovedBy = _userId;
            model.ApprovedDate = DateTime.UtcNow;
            model.IsApproved = false;
            model.Status = "Created";
            _dbContext.GLDivision.Update(model);
            _dbContext.SaveChanges();
            TempData["error"] = "false";
            TempData["message"] = "Department has been UnApproved successfully.";
            return RedirectToAction("Index", "Division");
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var gLDivision = _dbContext.GLDivision
                .Where(i => i.Id == id && i.CompanyId == companyId && i.IsDeleted == false).FirstOrDefault();
            var gLSubDivision = _dbContext.GLSubDivision
                .Where(i => i.GLDivisionId == id && i.IsDeleted == false)
                                .ToList();
            ViewBag.NavbarHeading = "Department Detail";
            ViewBag.TitleStatus = "Approved";

            TempData["Detail"] = gLSubDivision;
            return View(gLDivision);
        }
    }
}
