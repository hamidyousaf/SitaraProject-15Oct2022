using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
namespace Numbers.Areas.GL.Controllers
{
    [Area("GL")]
    public class SubAccountsController : Controller
    {
        private readonly NumbersDbContext _dbContext;


        public SubAccountsController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            List<GLSubAccount> account = _dbContext.GLSubAccounts.Where(a => a.IsDeleted == false).ToList();
            ViewBag.NavbarHeading = "List of Sub Accounts";
            return View(account);
        }

        [HttpGet]
        public IActionResult Create(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            if (id == 0)
            {
                SubAccountVM subAccountVM = new SubAccountVM();
                subAccountVM.GLSubAccount = new GLSubAccount();
                subAccountVM.GLSubAccountDetails = new List<GLSubAccountDetail>();
                //var result = (from c in _dbContext.GLSubAccounts select c).Count();
                //if (result > 0)
                //{
                //    var Code = _dbContext.GLSubAccounts.Select(x => x.Code).Max();
                //    int code = Convert.ToInt32(Code) + 1;
                //    ViewBag.Id = code;
                //}
                //else
                //{
                //    ViewBag.Id = 1;
                //}
                return View(subAccountVM);
            }

            else
            {
                SubAccountVM subAccountVM = new SubAccountVM();
                subAccountVM.GLSubAccount = _dbContext.GLSubAccounts.Find(id);
                //ViewBag.ModuleId = (from a in _dbContext.GLAccounts.Where(x => x.SubAccountId == Convert.ToString(id))
                //                    join b in _dbContext.AppCompanySetups on a.Code equals b.Value
                //                    where b.CompanyId == companyId
                //                    select b.ModuleId).ToList();
                //var LC = _dbContext.APLC.Where(x => x.IsDeleted == false && x.IsActive && x.SubAccountId == id).ToList();
                subAccountVM.GLSubAccountDetails = _dbContext.GLSubAccountDetails.Where(a => a.SubAccountId.Id == id && a.IsDelete == false).ToList();
                //foreach (var lc in LC)
                //{
                //    GLSubAccountDetail gLSubAccountDetail = new GLSubAccountDetail();
                //    bool LCId = _dbContext.GLSubAccountDetails.Where(x => x.LCID == lc.Id).Any();
                //    if (!LCId)
                //    {
                //        gLSubAccountDetail.LCNo = lc.LCNo;
                //        gLSubAccountDetail.LCID = lc.Id;
                //        gLSubAccountDetail.Code = "0";
                //        subAccountVM.GLSubAccountDetails.Add(gLSubAccountDetail);
                //    }
                //}
                //foreach(var decription in subAccountVM.GLSubAccountDetails)
                //{
                //    decription.LCNo = _dbContext.APLC.Where(x => x.Id == decription.LCID).Select(x => x.LCNo).FirstOrDefault();
                //}
                return View(subAccountVM);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(SubAccountVM accountVM, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            try
            {
                if (accountVM.GLSubAccount.Id == 0)
                {
                    GLSubAccount model = new GLSubAccount();
                    model.Date = accountVM.GLSubAccount.Date;
                    model.Description = accountVM.GLSubAccount.Description;
                    model.CreatedBy = userId;
                    model.CreatedDate = DateTime.Now;
                    model.CompanyId = companyId;
                    model.IsActive = accountVM.GLSubAccount.IsActive;
                    model.IsDeleted = false;
                    model.Status = "Created";
                    var result = (from c in _dbContext.GLSubAccounts select c).Count();
                    if (result > 0)
                    {
                        var Code = _dbContext.GLSubAccounts.Select(x => x.Code).Max();
                        int code = Convert.ToInt32(Code) + 1;
                        model.Code = code;
                    }
                    else
                    {
                        model.Code = 1;
                    }
                    _dbContext.GLSubAccounts.Add(model);
                    await _dbContext.SaveChangesAsync();


                    for (int i = 0; i < collection["tblDetailCode"].Count; i++)
                    {
                        GLSubAccountDetail detail = new GLSubAccountDetail();
                        detail.GLSubAccountId = model.Id;
                        detail.Description = Convert.ToString(collection["Description"][i]);
                        detail.Code = Convert.ToString(collection["tblDetailCode"][i]);
                        detail.CreatedBy = userId;
                        detail.CreatedDate = DateTime.Now;
                        detail.IsDelete = false;
                        detail.IsActive = accountVM.GLSubAccount.IsActive;
                        _dbContext.GLSubAccountDetails.Add(detail);
                        await _dbContext.SaveChangesAsync();
                    }



                }
                else
                {
                    GLSubAccount model = _dbContext.GLSubAccounts.Find(accountVM.GLSubAccount.Id);
                    model.Date = accountVM.GLSubAccount.Date;
                    model.Code = accountVM.GLSubAccount.Code;
                    model.Description = accountVM.GLSubAccount.Description;
                    model.CreatedBy = userId;
                    model.CreatedDate = DateTime.Now;
                    model.CompanyId = companyId;
                    model.IsActive = accountVM.GLSubAccount.IsActive;
                    model.IsDeleted = false;
                    _dbContext.GLSubAccounts.Update(model);
                    await _dbContext.SaveChangesAsync();
                    var detail = _dbContext.GLSubAccountDetails.Where(a => a.GLSubAccountId == model.Id).ToList();
                    var updateddetail = new List<GLSubAccountDetail>();
                    for (int i = 0; i < collection["tblDetailCode"].Count; i++)
                    {
                        var checkOldRows = detail.Find(e => e.Id == Convert.ToInt32(collection["DetailId"][i]));
                        if (checkOldRows != null)
                        {
                            GLSubAccountDetail accountDetail = await _dbContext.GLSubAccountDetails.FindAsync(Convert.ToInt32(collection["DetailId"][i]));
                            accountDetail.GLSubAccountId = model.Id;
                            accountDetail.Description = Convert.ToString(collection["Description"][i]);
                            try { 
                                accountDetail.LCID = Convert.ToInt32(collection["LCId"][i]);
                            }
                            catch (Exception ex)
                            {
                                string val = ex.ToString();
                            }
                            accountDetail.Code = Convert.ToString(collection["tblDetailCode"][i]);
                            accountDetail.UpdatedBy = userId;
                            accountDetail.UpdatedDate = DateTime.Now;
                            accountDetail.IsDelete = false;
                            accountDetail.IsActive = accountVM.GLSubAccount.IsActive;
                            updateddetail.Add(accountDetail);
                            _dbContext.GLSubAccountDetails.Update(accountDetail);
                            await _dbContext.SaveChangesAsync();
                        }
                        else
                        {
                            GLSubAccountDetail accountDetail = new GLSubAccountDetail();
                            accountDetail.GLSubAccountId = model.Id;
                            accountDetail.Description = null;
                            try
                            {
                                accountDetail.LCID = Convert.ToInt32(collection["LCId"][i]);
                            }
                            catch(Exception ex)
                            {
                                string val = ex.ToString();
                            }
                            accountDetail.Description = Convert.ToString(collection["Description"][i]);
                            accountDetail.Code = Convert.ToString(collection["tblDetailCode"][i]);
                            accountDetail.CreatedBy = userId;
                            accountDetail.CreatedDate = DateTime.Now;
                            accountDetail.IsDelete = false;
                            accountDetail.IsActive = accountVM.GLSubAccount.IsActive;
                            accountDetail.Id = 0;
                            updateddetail.Add(accountDetail);
                            _dbContext.GLSubAccountDetails.Add(accountDetail);
                            await _dbContext.SaveChangesAsync();
                        }
                    }
                    /*var delete = _dbContext.GLSubAccountDetails.Where(c => !updateddetail.Contains(c)).ToList();
                    foreach (var item in delete)
                    {
                        _dbContext.GLSubAccountDetails.Remove(item);
                    }
                    await _dbContext.SaveChangesAsync();*/
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                string message = ex.Message.ToString();
                TempData["ErrorMessage"] = ex.Message.ToString();
            }
            return RedirectToAction(nameof(Index));
        }
        
        public IActionResult GetSubAccounts()
        {
            try
            {
                int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
                string userId = HttpContext.Session.GetString("UserId");
                var approve = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == resp_Id).FirstOrDefault().Approve;
                var unApprove = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == userId && x.IsDeleted == false && x.ResponsibilityId == resp_Id).FirstOrDefault().UnApprove;

                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                //var searchValue = Request.Form["search[value]"].FirstOrDefault();
                var searchId = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchDate = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchDescription = Request.Form["columns[2][search][value]"].FirstOrDefault();
                var searchSubAccount = Request.Form["columns[3][search][value]"].FirstOrDefault();
                var searchStatus = Request.Form["columns[4][search][value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var Data = (from GLSubAccountDetails in _dbContext.GLSubAccountDetails.Where(x =>  x.IsDelete == false)
                            join GLSubAccount in _dbContext.GLSubAccounts.Where(x => x.IsDeleted == false)
                            on GLSubAccountDetails.GLSubAccountId equals GLSubAccount.Id
                            select new SubAccountVM
                            {
                                GLSubAccount = GLSubAccount,
                                GLSubAccountDetail = GLSubAccountDetails
                            } );
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    Data = Data.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                Data = !string.IsNullOrEmpty(searchId) ? Data.Where(m => m.GLSubAccount.Id.ToString().Contains(searchId)) : Data;
                Data = !string.IsNullOrEmpty(searchDate) ? Data.Where(m => m.GLSubAccount.Date.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchDate.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchDescription) ? Data.Where(m => m.GLSubAccount.Description.ToString().ToUpper().Contains(searchDescription.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchSubAccount) ? Data.Where(m => m.GLSubAccountDetail.Description.ToString().ToUpper().Contains(searchSubAccount.ToUpper())) : Data;
                Data = !string.IsNullOrEmpty(searchStatus) ? Data.Where(m => m.GLSubAccount.Status.ToString().ToUpper().Contains(searchStatus.ToUpper())) : Data;
                recordsTotal = Data.Count();
                var data = Data.Skip(skip).Take(pageSize).ToList();
                List<SubAccountVM> Details = new List<SubAccountVM>();
                foreach (var grp in data)
                {
                    SubAccountVM detail = new SubAccountVM();
                    detail.ShortDate = grp.GLSubAccount.Date.ToString(Helpers.CommonHelper.DateFormat);
                    detail.GLSubAccount = grp.GLSubAccount;
                    detail.GLSubAccount.Approve = approve;
                    detail.GLSubAccount.Unapprove = unApprove;
                    detail.GLSubAccountDetail = grp.GLSubAccountDetail;
                    Details.Add(detail);
                }
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = Details };
                return Ok(jsonData);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public bool CheckCode(int code)
        {
            var checkName = _dbContext.GLSubAccounts.Where(a => a.Code == code && a.IsDeleted == false).Count();
            if (checkName > 0)
            {
                return true;
            }
            else
            {
                return false;
            }


        }

        public IActionResult Approve(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            GLSubAccount model = _dbContext.GLSubAccounts.Find(id);
            model.ApprovedBy = _userId;
            model.ApprovedDate = DateTime.UtcNow;
            model.IsApproved = true;
            model.Status = "Approved";
            _dbContext.GLSubAccounts.Update(model);
            _dbContext.SaveChanges();
            TempData["error"] = "false";
            TempData["message"] = "Sub Account has been approved successfully.";
            return RedirectToAction("Index", "SubAccounts");
        }
        public IActionResult UnApprove(int id)
        {
            int _companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string _userId = HttpContext.Session.GetString("UserId");
            GLSubAccount model = _dbContext.GLSubAccounts.Find(id);
            model.ApprovedBy = _userId;
            model.ApprovedDate = DateTime.UtcNow;
            model.IsApproved = false;
            model.Status = "Created";
            _dbContext.GLSubAccounts.Update(model);
            _dbContext.SaveChanges();
            TempData["error"] = "false";
            TempData["message"] = "Sub Accounts has been UnApproved successfully.";
            return RedirectToAction("Index", "SubAccounts");
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var gLSubAccounts = _dbContext.GLSubAccounts
                .Where(i => i.Id == id && i.CompanyId == companyId && i.IsActive == true && i.IsDeleted == false)
                .FirstOrDefault();
            var gLSubAccountDetails = _dbContext.GLSubAccountDetails
                .Where(i => i.GLSubAccountId == id && i.IsDelete == false)
                .ToList();
            ViewBag.NavbarHeading = "Sub Accounts Detail";
            ViewBag.TitleStatus = "Approved";

            TempData["Detail"] = gLSubAccountDetails;
            return View(gLSubAccounts);
        }
    }
}
