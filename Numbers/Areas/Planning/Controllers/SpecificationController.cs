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
using Microsoft.AspNetCore.Authorization;
using Numbers.Repository.Helpers;

namespace Numbers.Areas.Planning.Controllers
{
    [Authorize]
    [Area("Planning")]
    public class SpecificationController : Controller
    {
        private readonly NumbersDbContext _dbContext;


        public SpecificationController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            List<PlanSpecification> cost = _dbContext.PlanSpecifications.Where(a => a.IsDeleted == false && a.CompanyId==companyId).ToList();
            return View(cost);
        }

        public IActionResult GetSI()
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
                var searchCode = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchDate = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchDescription = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchDepartment = Request.Form["columns[4][search][value]"].FirstOrDefault();
                var searchSubDepartment = Request.Form["columns[5][search][value]"].FirstOrDefault();
                var searchStatus = Request.Form["columns[6][search][value]"].FirstOrDefault();


                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var PlanSpecificationsData = (from PlanSpecifications in _dbContext.PlanSpecifications
                                      .Where(x => x.IsDeleted == false && x.CompanyId==companyId).OrderByDescending(x => x.Id)
                                      .Include(x => x.ItemCategoryLevel2)
                                      .Include(x => x.ItemCategoryLevel4)
                                      .Include(x => x.GRQuality) select PlanSpecifications);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    PlanSpecificationsData = PlanSpecificationsData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                //if (!string.IsNullOrEmpty(searchValue))
                //{
                //    PlanSpecificationsData = PlanSpecificationsData.Where(m => m.Code.ToString().Contains(searchValue)
                //                                    || m.Date.ToString(CommonHelper.DateFormat).ToUpper().Contains(searchValue.ToUpper())
                //                                    || m.Description.ToString().ToUpper().Contains(searchValue.ToUpper())
                //                                    || m.Status.ToString().ToUpper().Contains(searchValue.ToUpper())
                //                                    || _dbContext.GLDivision.FirstOrDefault(x => x.Id == Convert.ToInt32(m.DivisionId)).Name.ToUpper().Contains(searchValue.ToUpper())
                //                                    || _dbContext.GLSubDivision.FirstOrDefault(x => x.Id == Convert.ToInt32(m.SubDivisionId)).Name.ToUpper().Contains(searchValue.ToUpper())
                //                                  );

                //}
                PlanSpecificationsData = !string.IsNullOrEmpty(searchCode) ? PlanSpecificationsData.Where(m => m.Code.ToString().Contains(searchCode)) : PlanSpecificationsData;
                PlanSpecificationsData = !string.IsNullOrEmpty(searchDate) ? PlanSpecificationsData.Where(m => m.Date.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchDate.ToUpper())) : PlanSpecificationsData;
                PlanSpecificationsData = !string.IsNullOrEmpty(searchDescription) ? PlanSpecificationsData.Where(m => m.ItemCategoryLevel2.Name.ToString().ToUpper().Contains(searchDescription.ToUpper())) : PlanSpecificationsData;
                PlanSpecificationsData = !string.IsNullOrEmpty(searchDepartment) ? PlanSpecificationsData.Where(m => m.ItemCategoryLevel4.Name.ToString().ToUpper().Contains(searchDepartment.ToUpper())) : PlanSpecificationsData;
                PlanSpecificationsData = !string.IsNullOrEmpty(searchSubDepartment) ? PlanSpecificationsData.Where(m => m.GRQuality.Description.ToString().ToUpper().Contains(searchSubDepartment.ToUpper())) : PlanSpecificationsData;
                PlanSpecificationsData = !string.IsNullOrEmpty(searchStatus) ? PlanSpecificationsData.Where(m => (m.Status != null ? m.Status.ToString().ToUpper().Contains(searchStatus.ToUpper()) : false)) : PlanSpecificationsData;

                recordsTotal = PlanSpecificationsData.Count();
                var data = PlanSpecificationsData.ToList();
                if (pageSize == -1)
                {
                    data = PlanSpecificationsData.Skip(skip).Take(recordsTotal == 0 ? 1 : pageSize == -1 ? recordsTotal : 1).ToList();
                }
                else
                {
                    data = PlanSpecificationsData.Skip(skip).Take(pageSize).ToList();
                }
                List<PlanSpecificationVM> Details = new List<PlanSpecificationVM>();
                foreach (var grp in data)
                {
                    PlanSpecificationVM PlanSpecificationVM = new PlanSpecificationVM();
                    PlanSpecificationVM.ItemCategoryLevel2 = _dbContext.InvItemCategories.FirstOrDefault(x => x.Id == grp.ItemCategoryLevel2Id).Name;
                    PlanSpecificationVM.ItemCategoryLevel4 = _dbContext.InvItemCategories.FirstOrDefault(x => x.Id == grp.ItemCategoryLevel4Id).Name;
                    PlanSpecificationVM.GreigeQuality = _dbContext.GRQuality.FirstOrDefault(x => x.Id == grp.GRQualityId).Description;
                    PlanSpecificationVM.Date = grp.Date.ToString(Helpers.CommonHelper.DateFormat);
                    PlanSpecificationVM.PlanSpecifications = grp;
                    Details.Add(PlanSpecificationVM);
                }
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = Details.OrderByDescending(x => x.PlanSpecifications.Id) };
                return Ok(jsonData);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [Authorize]
        [HttpPost]
        public IActionResult GetSpecificationForPopUp()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            List<PlanSpecification> specification = _dbContext.PlanSpecifications
                .Include(x => x.ItemCategoryLevel4)
                .Include(x => x.GRQuality)
                .Where(x => x.IsDeleted != true && x.IsApproved == true && x.CompanyId==companyId).OrderByDescending(x => x.Id).ToList();
            return PartialView("_SpecificationPopUp", specification);
        }

        public IActionResult Create(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            var configValues = new ConfigValues(_dbContext);

            ViewBag.ItemCategory2 = configValues.GetSecondCategoryByResp(resp_Id);

            // ViewBag.ItemCategory4 = new SelectList(_dbContext.InvItemCategories.Where(a => a.IsDeleted == false ).ToList(), "Id", "Name");
            ViewBag.GreigeQuality = new SelectList(_dbContext.GRQuality.Where(a =>a.Status=="Approved" && a.IsDeleted == false ).ToList(), "Id", "Description");
            
            if (id == 0)
            {
                TempData["Code"] = GetPlanSpecificationsNo(companyId);
                PlanSpecificationVM PlanSpecificationVM = new PlanSpecificationVM();
                PlanSpecificationVM.PlanSpecifications = new PlanSpecification();
                PlanSpecificationVM.PlanSpecificationList = _dbContext.PlanSpecifications.Where(a => a.IsDeleted == false && a.CompanyId==companyId).ToList();
                ViewBag.NavbarHeading = "Create Specification";
                return View(PlanSpecificationVM);

            }
               
            else
            {
                 

                PlanSpecificationVM PlanSpecificationVM = new PlanSpecificationVM();
                PlanSpecificationVM.PlanSpecifications = _dbContext.PlanSpecifications.Find(id);
                TempData["Code"] = PlanSpecificationVM.PlanSpecifications.Code;
                PlanSpecificationVM.PlanSpecificationList = _dbContext.PlanSpecifications.Where(a => a.IsDeleted == false).ToList();
                ViewBag.ItemCategory4 = new SelectList(_dbContext.InvItemCategories.Where(a => a.IsDeleted == false && a.Id == PlanSpecificationVM.PlanSpecifications.ItemCategoryLevel4Id).ToList(), "Id", "Name");
                return View(PlanSpecificationVM);
            }




        }

        public int GetPlanSpecificationsNo(int companyId)
        {
            int maxReceiptNo = 1;
            var receipts = _dbContext.PlanSpecifications.Where(r => r.CompanyId == companyId).ToList();
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
        public async Task<IActionResult> Create(PlanSpecificationVM PlanSpecificationVM)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            try
            {
                if (PlanSpecificationVM.PlanSpecifications.Id == 0)
                {
                    PlanSpecification model = new PlanSpecification();
                    model.Date = PlanSpecificationVM.PlanSpecifications.Date;
                    model.Code = this.GetPlanSpecificationsNo(companyId);
                    model.CreatedBy = userId;
                    model.CreatedDate = DateTime.Now;
                    model.CompanyId = companyId;
                    model.IsDeleted = false;
                    model.IsActive = PlanSpecificationVM.PlanSpecifications.IsActive;
                    model.ItemCategoryLevel2Id = PlanSpecificationVM.PlanSpecifications.ItemCategoryLevel2Id;
                    model.ItemCategoryLevel4Id = PlanSpecificationVM.PlanSpecifications.ItemCategoryLevel4Id;
                    model.GRQualityId = PlanSpecificationVM.PlanSpecifications.GRQualityId;
                    _dbContext.PlanSpecifications.Add(model);
                    await _dbContext.SaveChangesAsync();


             


                }
                else
                {
                    PlanSpecification model = _dbContext.PlanSpecifications.Find(PlanSpecificationVM.PlanSpecifications.Id);
                    model.Date = PlanSpecificationVM.PlanSpecifications.Date;
                    model.Code = PlanSpecificationVM.PlanSpecifications.Code;
                    model.UpdatedBy = userId;
                    model.UpdatedDate = DateTime.Now;
                    model.CompanyId = companyId;
                    model.IsDeleted = false;
                    model.IsActive = PlanSpecificationVM.PlanSpecifications.IsActive;
                    model.ItemCategoryLevel2Id = PlanSpecificationVM.PlanSpecifications.ItemCategoryLevel2Id;
                    model.ItemCategoryLevel4Id = PlanSpecificationVM.PlanSpecifications.ItemCategoryLevel4Id;
                    model.GRQualityId = PlanSpecificationVM.PlanSpecifications.GRQualityId;
                    _dbContext.PlanSpecifications.Update(model);
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
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var checkName = _dbContext.PlanSpecifications.Where(a => a.Code == code && a.IsDeleted == false && a.CompanyId==companyId).Count();
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
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var division = _dbContext.GLDivision.Where(a => a.Id == id && a.IsDeleted == false && a.CompanyId==companyId).Select(a=>a.Name).FirstOrDefault();
            if (division != null)
            {
                return division;

            }
            else
            {
                return "";

            }

        }
        public IActionResult GetItemCategory4(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;

            var subdivision = (from a in _dbContext.InvItemCategories.Where(x => x.Id == id && x.IsDeleted == false).ToList()
                              join d in _dbContext.InvItemCategories.Where(x=>x.IsDeleted==false) on a.Id equals d.ParentId 
                              join dd in _dbContext.InvItemCategories.Where(x => x.IsDeleted == false) on d.Id equals dd.ParentId

                              select dd).ToList()
                              ;

            return Ok(subdivision);
        }
        
        public IActionResult Approve(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            PlanSpecification model = _dbContext.PlanSpecifications.Find(id);
            model.ApprovedBy = _userId;
            model.ApprovedDate = DateTime.UtcNow;
            model.IsApproved = true;
            model.Status = "Approved";
            _dbContext.PlanSpecifications.Update(model);
            _dbContext.SaveChanges();
            TempData["error"] = "false";
            TempData["message"] = "Specification has been approved successfully.";
            return RedirectToAction("Create", "Specification");
        }
        public IActionResult UnApprove(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            PlanSpecification model = _dbContext.PlanSpecifications.Find(id);
            model.ApprovedBy = _userId;
            model.ApprovedDate = DateTime.UtcNow;
            model.IsApproved = false;
            model.Status = "Created";
            _dbContext.PlanSpecifications.Update(model);
            _dbContext.SaveChanges();
            TempData["error"] = "false";
            TempData["message"] = "Specification has been UnApproved successfully.";
            return RedirectToAction("Create", "Specification");
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            PlanSpecification PlanSpecifications = _dbContext.PlanSpecifications
                .Include(i => i.ItemCategoryLevel2)
                .Include(i => i.ItemCategoryLevel4)
                .Include(i => i.GRQuality)
                .Where(i => i.Id == id && i.IsDeleted == false && i.CompanyId==companyId).FirstOrDefault();
            ViewBag.NavbarHeading = "Specification";
            ViewBag.TitleStatus = "Approved";

            return View(PlanSpecifications);
        }
        [HttpGet]
        public IActionResult CheckSpecification(int secondCategoryId,int forthCategoryId, int greigeQualityId)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var data =  _dbContext.PlanSpecifications
                .FirstOrDefault(x => x.ItemCategoryLevel2Id == secondCategoryId && x.ItemCategoryLevel4Id == forthCategoryId && x.GRQualityId == greigeQualityId && x.CompanyId == companyId);
            if (data != null)
            {
                return Ok(new ErrorMessage{Status = false , Message = "This Specification is already existing!" });
            }
            return Ok(true);
        }

        public async Task<IActionResult> Delete(int id)
        {
            //var ContractRepo = new GRFolding(_dbContext);
            string userId = HttpContext.Session.GetString("UserId");
            bool isSuccess;

            var deleteItem = _dbContext.PlanSpecifications.Where(x => x.Id == id).FirstOrDefault();
            if (deleteItem == null)
            {
                isSuccess = false;
            }
            else
            {


                var findSeasonalPlan = _dbContext.SeasonalPlaningDetail.Where(x => x.SpecificationId == id).FirstOrDefault();
                if (findSeasonalPlan == null)
                {
                    deleteItem.IsDeleted = true;
                    var entry = _dbContext.PlanSpecifications.Update(deleteItem);
                    entry.OriginalValues.SetValues(await entry.GetDatabaseValuesAsync());
                    await _dbContext.SaveChangesAsync();
                    isSuccess = true;

                }
                else
                {

                    isSuccess = false;

                }



            }

            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Specification has been deleted successfully";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Specification Consumed in seasonal planning";
            }
            return RedirectToAction("Create", "Specification");
        }
    }
}
