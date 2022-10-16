using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Interface;
using Numbers.Repository.AppModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Numbers.Areas.Approval.Controllers
{
    [Area("Approval")]
    [Authorize]
    public class ApprovalGroupController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        private readonly SysApprovalGroupDetailsRepository _SysApprovalGroupDetailsRepository;
        private readonly SysApprovalGroupRepository _SysApprovalGroupRepository;

        public ApprovalGroupController(NumbersDbContext context, SysApprovalGroupDetailsRepository sysApprovalGroupDetailsRepository, SysApprovalGroupRepository sysApprovalGroupRepository)
        {
            _dbContext = context;
            _SysApprovalGroupDetailsRepository = sysApprovalGroupDetailsRepository;
            _SysApprovalGroupRepository = sysApprovalGroupRepository;
        }
        public IActionResult List()
        {
            var approvalGroupVMList = new List<ApprovalGroupVM>();
            //var model = unitofwork.SysApprovalGroupRepository.Get(x => x.IsActive).ToList(); 
            var model = _SysApprovalGroupRepository.Get(x => x.IsActive).ToList();
            foreach(var grp in model)
            {
                ApprovalGroupVM approvalGroupVM = new ApprovalGroupVM();
                approvalGroupVM.SysApprovalGroup = grp;
                approvalGroupVMList.Add(approvalGroupVM);
            }
            return View(approvalGroupVMList);
        }

        public IActionResult Index(int id)
        {
            if (id == 0)
            {
                ViewBag.UserList = new SelectList((from b in _dbContext.Users
                                                   where b.CompanyId == HttpContext.Session.GetInt32("CompanyId")
                                                   select b
                                  ).ToList(), "Id", "UserName");

                ApprovalGroupVM ApprovalGroupVM = new ApprovalGroupVM();
                ApprovalGroupVM.SysApprovalGroup = new SysApprovalGroup();
                ApprovalGroupVM.SysApprovalGroupDetails = new List<SysApprovalGroupDetails>();
                ApprovalGroupVM.Users= new List<ApplicationUser>();

                return View(ApprovalGroupVM);

            }

            else
            {
                ViewBag.UserList = new SelectList((from b in _dbContext.Users
                                                   where b.CompanyId == HttpContext.Session.GetInt32("CompanyId")
                                                   select b
                                  ).ToList(), "Id", "UserName");

                ApprovalGroupVM ApprovalGroupVM = new ApprovalGroupVM();
                ApprovalGroupVM.SysApprovalGroup = _dbContext.SysApprovalGroup.Find(id);
                var model = _dbContext.SysApprovalGroupDetails.Where(a => a.Approval_Group_Id == id).ToList();
                ApprovalGroupVM.Users = new List<ApplicationUser>();
                foreach (var usr in model)
                {
                    var Username = (from u in _dbContext.Users where u.Id == usr.User_ID select u).FirstOrDefault();
                    ApprovalGroupVM.Users.Add(Username);
                }

                return View(ApprovalGroupVM);
            }
            //var model = new ApprovalGroupVM();
            //model.SysApprovalGroup = _dbContext.SysApprovalGroup.FirstOrDefault();
            //model.SysApprovalGroupList = _dbContext.SysApprovalGroup.ToList();
            //model.SysApprovalGroupDetailsList = _dbContext.SysApprovalGroupDetails.ToList();
            //return View(model);
        }

        public async Task<IActionResult> Create(ApprovalGroupVM approvalGroupVM, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            int resp_Id= HttpContext.Session.GetInt32("Resp_ID").Value;
            if (approvalGroupVM.SysApprovalGroup.Id == 0)
            {
                approvalGroupVM.SysApprovalGroup.CompanyId = companyId;
                approvalGroupVM.SysApprovalGroup.CreatedBy = userId;
                approvalGroupVM.SysApprovalGroup.CreatedDate = DateTime.Now;
                approvalGroupVM.SysApprovalGroup.Resp_ID = resp_Id;
                approvalGroupVM.SysApprovalGroup.IsActive = true;
                await _SysApprovalGroupRepository.CreateAsync(approvalGroupVM.SysApprovalGroup);
                var rownber = collection["id"].Count;
                for (int i = 0; i < rownber; i++)
                {
                    SysApprovalGroupDetails detail = new SysApprovalGroupDetails();
                    detail.Approval_Group_Id = approvalGroupVM.SysApprovalGroup.Id;
                    string userName = Convert.ToString(collection["id"][i]);
                    var userid = (from u in _dbContext.Users where u.UserName == userName select u.Id).FirstOrDefault();
                    detail.User_ID = userid;

                    await _SysApprovalGroupDetailsRepository.CreateAsync(detail);
                }
                
            }
            else
            {
                approvalGroupVM.SysApprovalGroup.CompanyId = companyId;
                approvalGroupVM.SysApprovalGroup.UpdatedBy = userId;
                approvalGroupVM.SysApprovalGroup.UpdatedDate = DateTime.Now;
                approvalGroupVM.SysApprovalGroup.Resp_ID = resp_Id;
                await _SysApprovalGroupRepository.UpdateAsync(approvalGroupVM.SysApprovalGroup);
                //Removing Users from db which deleted in edit mode. 
                var rownber = collection["User_ID"].Count;
                var existing_User = _SysApprovalGroupDetailsRepository.Get(x => x.Approval_Group_Id == approvalGroupVM.SysApprovalGroup.Id).ToList();
                List<string> myList = new List<string>();
                for (int i = 0; i < rownber; i++)
                {
                    string userName = Convert.ToString(collection["User_ID"][i]);
                    var userid = (from u in _dbContext.Users where u.UserName == userName select u.Id).FirstOrDefault();
                    myList.Add(userid);
                }
                if (myList.Count > 0)
                {
                    foreach (var usr in existing_User)
                    {
                        bool result = myList.Exists(s => s == usr.User_ID);
                        if (!result)
                        {
                            await _SysApprovalGroupDetailsRepository.DeleteAsync(usr);
                        }
                    }
                }
                if (rownber == 0)
                {
                    foreach (var usr in existing_User)
                    {
                        await _SysApprovalGroupDetailsRepository.DeleteAsync(usr);
                    }
                }
                //Inserting new User in db which added in edit mode.
                var rownbernew = collection["id"].Count;
                for (int i = 0; i < rownbernew; i++)
                {
                    string userName = Convert.ToString(collection["id"][i]);
                    var userid = (from u in _dbContext.Users where u.UserName == userName select u.Id).FirstOrDefault();
                    SysApprovalGroupDetails detail = _SysApprovalGroupDetailsRepository.Get(x => x.User_ID == userid && x.Approval_Group_Id == approvalGroupVM.SysApprovalGroup.Id).FirstOrDefault();
                    if (detail == null)
                    {
                        SysApprovalGroupDetails sysApprovalGroupDetails = new SysApprovalGroupDetails();
                        sysApprovalGroupDetails.User_ID = userid;
                        sysApprovalGroupDetails.Approval_Group_Id = approvalGroupVM.SysApprovalGroup.Id;
                        sysApprovalGroupDetails.Description = approvalGroupVM.SysApprovalGroup.Description;
                        await _SysApprovalGroupDetailsRepository.CreateAsync(sysApprovalGroupDetails);
                    }
                }

            }
            return RedirectToAction("Index");
        }
    }
}
