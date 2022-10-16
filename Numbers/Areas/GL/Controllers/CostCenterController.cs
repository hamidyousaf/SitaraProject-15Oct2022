using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using System.Linq.Dynamic.Core;
using Numbers.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Numbers.Areas.GL.Controllers
{   [Area("GL")]
    public class CostCenterController : Controller
    {
        private readonly NumbersDbContext _dbContext;


        public CostCenterController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            List<CostCenter> cost = _dbContext.CostCenter.Where(a => a.IsDeleted == false).ToList();
            return View(cost);
        }

        public IActionResult GetSI()
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
                var searchCode = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchDate = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchDescription = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchDepartment = Request.Form["columns[4][search][value]"].FirstOrDefault();
                var searchSubDepartment = Request.Form["columns[5][search][value]"].FirstOrDefault();
                var searchStatus = Request.Form["columns[6][search][value]"].FirstOrDefault();






                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var CostCenterData = (from CostCenter in _dbContext.CostCenter.Where(x=>x.CompanyId== companyId)
                                      .Where(x => x.IsDeleted == false).OrderByDescending(x=>x.Id)
                                      .Include(x => x.Division)
                                      .Include(x => x.SubDivision) select CostCenter);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    CostCenterData = CostCenterData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                //if (!string.IsNullOrEmpty(searchValue))
                //{
                //    CostCenterData = CostCenterData.Where(m => m.Code.ToString().Contains(searchValue)
                //                                    || m.Date.ToString(CommonHelper.DateFormat).ToUpper().Contains(searchValue.ToUpper())
                //                                    || m.Description.ToString().ToUpper().Contains(searchValue.ToUpper())
                //                                    || m.Status.ToString().ToUpper().Contains(searchValue.ToUpper())
                //                                    || _dbContext.GLDivision.FirstOrDefault(x => x.Id == Convert.ToInt32(m.DivisionId)).Name.ToUpper().Contains(searchValue.ToUpper())
                //                                    || _dbContext.GLSubDivision.FirstOrDefault(x => x.Id == Convert.ToInt32(m.SubDivisionId)).Name.ToUpper().Contains(searchValue.ToUpper())
                //                                  );

                //}
                CostCenterData = !string.IsNullOrEmpty(searchCode) ? CostCenterData.Where(m => m.Code.ToString().Contains(searchCode)) : CostCenterData;
                CostCenterData = !string.IsNullOrEmpty(searchDate) ? CostCenterData.Where(m => m.Date.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchDate.ToUpper())) : CostCenterData;
                CostCenterData = !string.IsNullOrEmpty(searchDescription) ? CostCenterData.Where(m => m.Description != null ? m.Description.ToString().ToLower().Contains(searchDescription.ToLower()) : false) : CostCenterData;
                CostCenterData = !string.IsNullOrEmpty(searchDepartment) ? CostCenterData.Where(m => m.Division.Name.ToString().ToUpper().Contains(searchDepartment.ToUpper())) : CostCenterData;
                CostCenterData = !string.IsNullOrEmpty(searchSubDepartment) ? CostCenterData.Where(m => m.SubDivision.Name.ToString().ToUpper().Contains(searchSubDepartment.ToUpper())) : CostCenterData;
                CostCenterData = !string.IsNullOrEmpty(searchStatus) ? CostCenterData.Where(m => (m.Status != null ? m.Status.ToString().ToUpper().Contains(searchStatus.ToUpper()) : false)) : CostCenterData;

                recordsTotal = CostCenterData.Count();
                var data = CostCenterData.Skip(skip).Take(pageSize).ToList();
                List<CostCenterVM> Details = new List<CostCenterVM>();
                foreach (var grp in data)
                {
                    CostCenterVM costCenterVM = new CostCenterVM();
                    //aRInvoiceViewModel.CostCenter.DivisionId = grp.;
                    //aRInvoiceViewModel.CostCenter.SubDivisionId =  grp.SubDivisionId;
                    if (grp.DivisionId != 0)
                    {
                        costCenterVM.Division = _dbContext.GLDivision.FirstOrDefault(x => x.Id == grp.DivisionId).Name;
                    }else
                    {
                        costCenterVM.Division = "N/A";
                    }
                    if (grp.SubDivisionId != 0)
                    {
                        costCenterVM.SubDivision = _dbContext.GLSubDivision.FirstOrDefault(x => x.Id == grp.SubDivisionId).Name;
                    }
                    else
                    {
                        costCenterVM.SubDivision = "N/A";
                    }
                    costCenterVM.Date = grp.Date.ToString(Helpers.CommonHelper.DateFormat);
                    costCenterVM.CostCenter = grp;
                    costCenterVM.CostCenter.Approve = approve;
                    costCenterVM.CostCenter.Unapprove = unApprove;
                    Details.Add(costCenterVM);
                }
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = Details.OrderByDescending(x=>x.CostCenter.Id) };
                return Ok(jsonData);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public JsonResult checkDescriptionAlreadyExists(string description)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            System.Threading.Thread.Sleep(200);
            if (description == null)
                return Json(0);
            var chkCode = _dbContext.CostCenter.Where(a => a.IsDeleted == false && a.Description == description && a.CompanyId == companyId).FirstOrDefault();
            return Json(chkCode == null ? 0 : 1);
        }


        public IActionResult Create(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            ViewBag.Divsions = new SelectList(_dbContext.GLDivision.Where(a => a.IsDeleted == false && a.IsActive==true && a.IsApproved==true && a.CompanyId==companyId).ToList(), "Id", "Name");
            ViewBag.SubDivision = new SelectList(_dbContext.GLSubDivision.Where(a => a.IsDeleted == false && a.CompanyId==companyId ).ToList(), "Id", "Name");
            
            if (id == 0)
            {
                TempData["Code"] = GetCostCenterNo(companyId);
                CostCenterVM costCenterVM = new CostCenterVM();
                costCenterVM.CostCenter = new CostCenter();
                costCenterVM.CostCenterList = _dbContext.CostCenter.Where(a => a.IsDeleted == false).ToList();

                return View(costCenterVM);

            }
               
            else
            {

                CostCenterVM costCenterVM = new CostCenterVM();
                costCenterVM.CostCenter = _dbContext.CostCenter.Find(id);
                TempData["Code"] = costCenterVM.CostCenter.Code;
                costCenterVM.CostCenterList = _dbContext.CostCenter.Where(a => a.IsDeleted == false).ToList();
                return View(costCenterVM);
            }




        }

        public int GetCostCenterNo(int companyId)
        {
            int maxReceiptNo = 1;
            var receipts = _dbContext.CostCenter.Where(r => r.CompanyId == companyId).ToList();
            if (receipts.Count > 0)
            {
                maxReceiptNo = receipts.Max(r => r.Code);
                return maxReceiptNo + 1;
            }
            else
            {
                return maxReceiptNo;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CostCenterVM costCenterVM)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            try
            {
                if (costCenterVM.CostCenter.Id == 0)
                {
                    CostCenter model = new CostCenter();
                    model.Date = costCenterVM.CostCenter.Date;
                    model.Code = this.GetCostCenterNo(companyId);
                    model.Description = costCenterVM.CostCenter.Description;
                    model.CreatedBy = userId;
                    model.CreatedDate = DateTime.Now;
                    model.CompanyId = companyId;
                    model.IsDeleted = false;
                    model.IsActive = costCenterVM.CostCenter.IsActive;
                    model.DivisionId = costCenterVM.CostCenter.DivisionId;
                    model.SubDivisionId = costCenterVM.CostCenter.SubDivisionId;
                    _dbContext.CostCenter.Add(model);
                    await _dbContext.SaveChangesAsync();


             


                }
                else
                {
                    CostCenter model = _dbContext.CostCenter.Find(costCenterVM.CostCenter.Id);
                    model.Date = costCenterVM.CostCenter.Date;
                    model.Code = costCenterVM.CostCenter.Code;
                    model.Description = costCenterVM.CostCenter.Description;
                    model.UpdatedBy = userId;
                    model.UpdatedDate = DateTime.Now;
                    model.CompanyId = companyId;
                    model.IsDeleted = false;
                    model.IsActive = costCenterVM.CostCenter.IsActive;
                    model.DivisionId = costCenterVM.CostCenter.DivisionId;
                    model.SubDivisionId = costCenterVM.CostCenter.SubDivisionId;
                    _dbContext.CostCenter.Update(model);
                    await _dbContext.SaveChangesAsync();

                }

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.InnerException.Message);
                string message = ex.Message.ToString();

                TempData["ErrorMessage"] = ex.Message.ToString();

            }




            return RedirectToAction(nameof(Create),new { id = 0 });
        }


        public bool CheckCode(int code)
        {
            var checkName = _dbContext.CostCenter.Where(a => a.Code == code && a.IsDeleted == false).Count();
            if (checkName > 0)
            {
                return true;

            }
            else
            {
                return false;

            }


        }

        public string GetDivision(int id)
        {
            var division = _dbContext.GLDivision.Where(a => a.Id == id && a.IsDeleted == false).Select(a=>a.Name).FirstOrDefault();
            if (division != null)
            {
                return division;

            }
            else
            {
                return "";

            }

        }
        public IActionResult GetSubDivisions(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var subdivision = _dbContext.GLSubDivision.Where(x => x.GLDivisionId == id && x.IsDeleted == false && x.CompanyId == companyId).ToList();

            return Ok(subdivision);
        }
        public IActionResult GetCostCenter(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var costCenters = _dbContext.CostCenter.Where(x => x.SubDivisionId == id && x.IsDeleted == false).ToList();

            return Ok(costCenters);
        }
        public IActionResult Approve(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            CostCenter model = _dbContext.CostCenter.Find(id);
            model.ApprovedBy = _userId;
            model.ApprovedDate = DateTime.UtcNow;
            model.IsApproved = true;
            model.Status = "Approved";
            _dbContext.CostCenter.Update(model);
            _dbContext.SaveChanges();
            TempData["error"] = "false";
            TempData["message"] = "CostCenter has been approved successfully.";
            return RedirectToAction("Create", "CostCenter");
        }
        public IActionResult UnApprove(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            CostCenter model = _dbContext.CostCenter.Find(id);
            model.ApprovedBy = _userId;
            model.ApprovedDate = DateTime.UtcNow;
            model.IsApproved = false;
            model.Status = "Created";
            _dbContext.CostCenter.Update(model);
            _dbContext.SaveChanges();
            TempData["error"] = "false";
            TempData["message"] = "CostCenter has been UnApproved successfully.";
            return RedirectToAction("Create", "CostCenter");
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            CostCenter costCenter = _dbContext.CostCenter
                .Include(i => i.Division)
                .Include(i => i.SubDivision)
                .Where(i => i.Id == id && i.IsDeleted == false).FirstOrDefault();
            ViewBag.NavbarHeading = "Cost Center Detail";
            ViewBag.TitleStatus = "Approved";

            return View(costCenter);
        }

    }
}
