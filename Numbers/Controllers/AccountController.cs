using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
//using Numbers.ViewModels;
//using Numbers.Models;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Security.Claims;
using Numbers.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Collections;
using Numbers.Repository;
using Numbers.Repository.Helpers;
using System.Linq.Dynamic.Core;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Numbers.Controllers
{

    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly NumbersDbContext _dbContext;
        private IPasswordHasher<ApplicationUser> passwordHasher;
        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, NumbersDbContext numbersDbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = numbersDbContext;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Login()
        {
            TempData["CurrentCompany"] = _dbContext.AppCompanies.FirstOrDefault().Name;
            // Prevents application to load default query string on login page(?ReturnUrl=2F)
            if (!string.IsNullOrEmpty(Request.QueryString.Value))
                return RedirectToAction("Login");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            TempData["CurrentCompany"] = _dbContext.AppCompanies.FirstOrDefault().Name;

            if (!ModelState.IsValid)
                return View(loginViewModel);
            var user = await _userManager.FindByNameAsync(loginViewModel.UserName);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                   
                    string host = Convert.ToString(HttpContext.Request.Host);
                    string ReportPathUpdate = String.Concat("http://" + host + "/ReportEngine/ReportViewer/");
                    var model = _dbContext.AppCompanyConfigs.Where(x => x.ConfigName == "Report Path").FirstOrDefault();
                    model.ConfigValue = ReportPathUpdate;
                    _dbContext.AppCompanyConfigs.Update(model);
                    _dbContext.SaveChanges();
                    // string yourPath = HttpContext.Request.Body;

                    //HttpRequest request = url.ActionContext.HttpContext.Request;
                    //var roles = await _userManager.GetRolesAsync(user);
                    //to add claim for existing user
                    //await _userManager.AddClaimAsync(user, new Claim("FullName", user.FullName));
                    //await _userManager.AddClaimAsync(user, new Claim("Photo", user.Photo));

                    SetSessionValues(user);
                    return RedirectToAction("Responsibility", "Dashboard", new { area = "" });
                    // return RedirectToAction("Index", "Dashboard",new { area = "" });
                }
                if (result.IsLockedOut)
                {
                    //throw new InvalidOperationException ("User account locked out.");
                    TempData["error"] = "true";
                    TempData["message"] = "User Is Locked";
                    return View();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(loginViewModel);
                }
            }
            ModelState.AddModelError("", "User name/password not found");
            return View(loginViewModel);
        }
        //[Authorize(Roles = "Manager")]
        public IActionResult Register(string id)
        {


            ViewBag.RoleName = new SelectList(_dbContext.Roles//.Where(u => !u.Name.Contains("Admin"))
                                 .ToList(), "Name", "Name");
            ViewBag.Responsibility = new SelectList((from r in _dbContext.Sys_Responsibilities.Include(x=>x.Company).ToList() select new {
                Resp_Id=r.Resp_Id,
                Resp_Name = r.Resp_Id + "-" + r.Resp_Name + " - " + r.Description
            }), "Resp_Id", "Resp_Name");
            ViewBag.Departments = new SelectList((from r in _dbContext.GLDivision.ToList()
                                                     select new
                                                     {
                                                         Id = r.Id,
                                                         Name = r.Id + "-" + r.Name 
                                                     }), "Id", "Name");
            ViewBag.DepartmentID = new SelectList(_dbContext.AppCompanies, "Id", "Name");

            RegisterViewModel registerVM = new RegisterViewModel();
            registerVM.Sys_ResponsibilitiesDetailList = new List<Sys_ResponsibilitiesDetail>();
            registerVM.SysUserDepartment = new List<SysUserDepartment>();

            ViewBag.NavbarHeading = "Register to Numbers";

            return View(registerVM);
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel, IFormFile img, IFormCollection collection)
        {
            //This Portion Deals with IFormFile / Upload            
            if (img != null)
            {
                if (img.Length > 0)
                {
                    var fileName = Path.GetFileName(img.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\userphotos", fileName);
                    using (var Fstream = new FileStream(filePath, FileMode.Create))
                    {
                        await img.CopyToAsync(Fstream);
                        var fullPath = "/img/userphotos/" + fileName;
                        //registerViewModel.Photo = fullPath;
                        registerViewModel.Photo = fullPath;
                    }
                }
            }
            // Registering App Users
            if (ModelState.IsValid)
            {
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                var user = new ApplicationUser { UserName = registerViewModel.UserName, Email = registerViewModel.Email, FullName = registerViewModel.FullName, Photo = registerViewModel.Photo, Id = registerViewModel.Id, CompanyId = companyId, AllDepartment=true };
                var result = await _userManager.CreateAsync(user, registerViewModel.Password);
                if (result.Succeeded)
                {
                    AppUserCompany userCompany = new AppUserCompany();
                    userCompany.CompanyId = companyId;
                    userCompany.UserId = user.Id;
                    userCompany.IsDefault = false;
                    _dbContext.AppUserCompanies.Add(userCompany);
                    await _dbContext.SaveChangesAsync();

                    await this._userManager.AddToRoleAsync(user, registerViewModel.UserRoles);
                    // Add User Claims for full name. You can check for the success of addition 
                    await _userManager.AddClaimAsync(user, new Claim("FullName", user.FullName));
                    if (user.Photo != null)
                        await _userManager.AddClaimAsync(user, new Claim("Photo", user.Photo));

                    var rownber = collection["ResponsibilityId"].Count;
                    for (int i = 0; i < rownber; i++)
                    {
                        var SysResponsibilityDet = new Sys_ResponsibilitiesDetail();
                        SysResponsibilityDet.UserId = user.Id;
                       SysResponsibilityDet.ResponsibilityId = Convert.ToInt32(collection["ResponsibilityId"][i]);
                        SysResponsibilityDet.Responsibility = Convert.ToString(collection["Responsibility"][i]);
                        //SysResponsibilityDet.ResponsibilityKey = Convert.ToString(collection["ResponsibilityKey"][i]);
                        SysResponsibilityDet.Date_from = Convert.ToDateTime(collection["DateFrom"][i]);
                        SysResponsibilityDet.Date_To = Convert.ToDateTime(collection["DateTo"][i]);
                        SysResponsibilityDet.Remarks = collection["Remarks"][i];
                        if (collection["Approve"][i] == "on")
                        {
                            SysResponsibilityDet.Approve = true;
                        }
                        else
                        {
                            SysResponsibilityDet.Approve = false;
                        }
                        if (collection["UnApprove"][i] == "on")
                        {
                            SysResponsibilityDet.UnApprove = true;
                        }
                        else
                        {
                            SysResponsibilityDet.UnApprove = false;
                        }
                        _dbContext.Sys_ResponsibilitiesDetail.Add(SysResponsibilityDet);
                        await _dbContext.SaveChangesAsync();

                    }

                    // Department working

                    var rownberDepartment = collection["DepartmentId"].Count;

                    for (int i = 0; i < rownberDepartment; i++)
                    {
                        var respDetailId = Convert.ToInt32(collection["DepId"][i]);

                        var SysResponsibilityDet = _dbContext.SysUserDepartments.Where(x => x.Id == respDetailId).FirstOrDefault();
                        if (SysResponsibilityDet == null)
                        {
                            //SysResponsibilityDet.UserId = user.Id;
                            SysResponsibilityDet = new SysUserDepartment();
                            SysResponsibilityDet.DepartmentId = Convert.ToInt32(collection["DepartmentId"][i]);
                            SysResponsibilityDet.Department = Convert.ToString(collection["Department"][i]);
                            SysResponsibilityDet.SubDepartmentId = Convert.ToInt32(collection["SubDepartmentId"][i]);
                            SysResponsibilityDet.SubDepartment = Convert.ToString(collection["subDepartment"][i]);
                            SysResponsibilityDet.UserId = user.Id;
                            _dbContext.SysUserDepartments.Add(SysResponsibilityDet);
                            await _dbContext.SaveChangesAsync();
                        }
                        else
                        {
                            SysResponsibilityDet.UserId = user.Id;
                            SysResponsibilityDet.DepartmentId = Convert.ToInt32(collection["DepartmentId"][i]);
                            SysResponsibilityDet.Department = Convert.ToString(collection["Department"][i]);
                            SysResponsibilityDet.SubDepartmentId = Convert.ToInt32(collection["SubDepartmentId"][i]);
                            SysResponsibilityDet.SubDepartment = Convert.ToString(collection["subDepartment"][i]);
                            _dbContext.SysUserDepartments.Update(SysResponsibilityDet);
                            await _dbContext.SaveChangesAsync();
                        }
                    }


                    return RedirectToAction("Index", "Account");
                }
                ViewBag.Name = new SelectList(_dbContext.Roles.Where(u => !u.Name.Contains("Admin")).ToList(), "Name", "Name");

                //var user = new IdentityUser() { UserName = registerViewModel.UserName };
                //var result = await _userManager.CreateAsync(user, registerViewModel.Password);

                //if (result.Succeeded)
                //{
                //    return RedirectToAction("Index", "Dashboard");
                //}
                ModelState.AddModelError("", result.Errors.First().Description);
            }
            return View(new RegisterViewModel()) ;
        }
        public IActionResult EditRegisteredUser(string id, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            ViewBag.RoleName = new SelectList(_dbContext.Roles.FromSql("select * from AspNetRoles WHERE CompanyId={0}", companyId)
                      .ToList(), "Name", "Name");
            // ViewBag.Responsibility = new SelectList(_dbContext.Sys_Responsibilities.ToList(), "Resp_Id", "Resp_Name");

            ViewBag.Responsibility = new SelectList((from r in _dbContext.Sys_Responsibilities.Include(x => x.Company).ToList()
                                                     select new
                                                     {
                                                         Resp_Id = r.Resp_Id,
                                                         Resp_Name = r.Resp_Id +"-"+ r.Resp_Name + " - " + r.Company.Name
                                                     }), "Resp_Id", "Resp_Name");
            ViewBag.Departments = new SelectList((from r in _dbContext.GLDivision.ToList()
                                                  select new
                                                  {
                                                      Id = r.Id,
                                                      Name = r.Id + "-" + r.Name
                                                  }), "Id", "Name");
            ViewBag.NavbarHeading = "Edit records in Numbers";
            ApplicationUser user = _userManager.FindByIdAsync(id).Result;
            var viewModel = new RegisterViewModel();

            viewModel.Sys_ResponsibilitiesDetailList = _dbContext.Sys_ResponsibilitiesDetail.Where(x=>x.UserId==id && x.IsDeleted==false).ToList();
            viewModel.SysUserDepartment = _dbContext.SysUserDepartments.Where(x => x.UserId == id && x.IsDeleted == false).ToList();
            #region Get User Role Name
            var userRole = _dbContext.UserRoles.Where(x => x.UserId == user.Id).FirstOrDefault();
            var roleId = userRole.RoleId;
            var roles = _dbContext.Roles.Where(r => r.Id == roleId).FirstOrDefault();
            #endregion Get User Role Name

            viewModel.Id = user.Id;
            viewModel.FullName = user.FullName;
            viewModel.UserName = user.UserName;
            viewModel.Email = user.Email;
            viewModel.Photo = user.Photo;
            viewModel.UserRoles = roles.Name;
            viewModel.AllDepartment = user.AllDepartment; 

            return View(viewModel);
        }

        //public async Task<IActionResult> EditRegisteredUser(RegisterViewModel registerViewModel, IFormFile img)
        //{
        //    if (img != null)
        //    {
        //        if (img.Length > 0)
        //        {
        //            var fileName = Path.GetFileName(img.FileName);
        //            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\userphotos", fileName);
        //            using (var Fstream = new FileStream(filePath, FileMode.Create))
        //            {
        //                await img.CopyToAsync(Fstream);
        //                var fullPath = "/img/userphotos/" + fileName;
        //                //registerViewModel.Photo = fullPath;
        //                registerViewModel.Photo = fullPath;
        //            }
        //        }
        //    }
        //    //var user = new ApplicationUser { UserName = registerViewModel.UserName, Email = registerViewModel.Email, FullName = registerViewModel.FullName, Photo = registerViewModel.Photo, Id = registerViewModel.Id };
        //    var user = await _userManager.FindByIdAsync(HttpContext.Session.GetString("UserId"));
        //    user.FullName = registerViewModel.FullName;
        //    user.UserName = registerViewModel.UserName;
        //    user.Email = registerViewModel.Email;
        //    user.Photo = registerViewModel.Photo;
        //    if (registerViewModel.Password != null || registerViewModel.ConfirmPassword!=null)
        //    {
        //        if(registerViewModel.Password== registerViewModel.ConfirmPassword)
        //        {
        //            var newPassword = _userManager.PasswordHasher.HashPassword(user, registerViewModel.Password);
        //            user.PasswordHash = newPassword;
        //            //await this._userManager.AddToRoleAsync(user, registerViewModel.UserRoles);
        //            var result = await _userManager.UpdateAsync(user);
        //            if (result.Succeeded)
        //            {
        //                TempData["error"] = "false";
        //                TempData["message"] = "Current Logged-In User has been Updated Succesfully...!";
        //            }
        //            else
        //            {
        //                TempData["error"] = "true";
        //                TempData["message"] = "Update has been Failed ...!";
        //            }
        //        }
        //        else {
        //            TempData["error"] = "true";
        //            TempData["message"] = "Password doesn't match ...!";
        //        }

        //    }
        //    else
        //    {
        //        var result = await _userManager.UpdateAsync(user);
        //        if (result.Succeeded)
        //        {
        //            TempData["error"] = "false";
        //            TempData["message"] = "Current Logged-In User has been Updated Succesfully...!";
        //        }
        //        else
        //        {
        //            TempData["error"] = "true";
        //            TempData["message"] = "Update has been Failed ...!";
        //        }
        //    }
        //    return RedirectToAction("Index", "Dashboard");
        //    //return View(registerViewModel);
        //}

        [HttpPost]
        public async Task<IActionResult> EditRegisteredUser(RegisterViewModel registerViewModel, IFormFile img, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string[] idsDeleted = Convert.ToString(collection["IdsDeleted"]).Split(",");
            string[] idsDepDeleted = Convert.ToString(collection["IdsDepDeleted"]).Split(",");
            foreach (var item in idsDeleted)
            {
                if (item != "" && item != "0")
                {
                    var itemToRemove = _dbContext.Sys_ResponsibilitiesDetail.Find(Convert.ToInt32(item));
                    itemToRemove.IsDeleted = true;
                    var tracker = _dbContext.Sys_ResponsibilitiesDetail.Update(itemToRemove);
                    tracker.OriginalValues.SetValues(await tracker.GetDatabaseValuesAsync());
                    await _dbContext.SaveChangesAsync();

                }
            }
            foreach (var item in idsDepDeleted)
            {
                if (item != "" && item != "0")
                {
                    var itemToRemove = _dbContext.SysUserDepartments.Find(Convert.ToInt32(item));
                    itemToRemove.IsDeleted = true;
                    var tracker = _dbContext.SysUserDepartments.Update(itemToRemove);
                    tracker.OriginalValues.SetValues(await tracker.GetDatabaseValuesAsync());
                    await _dbContext.SaveChangesAsync();

                }
            }
            registerViewModel.Sys_ResponsibilitiesDetailList = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.UserId == registerViewModel.Id ).ToList();
            registerViewModel.SysUserDepartment = _dbContext.SysUserDepartments.Where(x => x.UserId == registerViewModel.Id ).ToList();
            //if (ModelState.IsValid)
            //{

                ApplicationUser user = await _userManager.FindByIdAsync(registerViewModel.Id);
            if (user != null)
            {
                user.FullName = registerViewModel.FullName;
                user.UserName = registerViewModel.UserName;
                user.Email = registerViewModel.Email;
                //user.IsEdit = registerViewModel.IsEdit;
                //user.IsCancel = registerViewModel.IsCancel;
                user.AllDepartment = registerViewModel.AllDepartment;
                user.CompanyId = companyId;
                if (registerViewModel.Password != null && registerViewModel.ConfirmPassword != null)
                {
                    user.PasswordHash = registerViewModel.Password;
                    //user.PasswordHash = passwordHasher.HashPassword(user, registerViewModel.Password);
                    user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, registerViewModel.Password);
                }

                user.SecurityStamp = Guid.NewGuid().ToString();

                if (img != null)
                {
                    if (img.Length > 0)
                    {
                        var fileName = Path.GetFileName(img.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\userphotos", fileName);
                        using (var Fstream = new FileStream(filePath, FileMode.Create))
                        {
                            await img.CopyToAsync(Fstream);
                            var fullPath = "/img/userphotos/" + fileName;
                            //registerViewModel.Photo = fullPath;
                            user.Photo = fullPath;
                        }
                    }
                }

                if (registerViewModel.UserRoles != null)
                {
                    var role = _dbContext.Roles.Where(x => x.Id == (from a in _dbContext.UserRoles.Where(y => y.UserId == user.Id) select a.RoleId).FirstOrDefault()).FirstOrDefault();
                    await this._userManager.RemoveFromRoleAsync(user, role.Name);
                    await this._userManager.AddToRoleAsync(user, registerViewModel.UserRoles);
                }

                var result = await this._userManager.UpdateAsync(user);
                if (result != null)
                {
                    TempData["error"] = "false";
                    TempData["message"] = "User Updated Succesfully.";
                }
                else
                {
                    TempData["error"] = "true";
                    TempData["message"] = "Something Wet Wrong.";
                }

                var rownber = collection["ResponsibilityId"].Count;

                for (int i = 0; i < rownber; i++)
                {
                    var respDetailId = Convert.ToInt32(collection["RespId"][i]);

                    var SysResponsibilityDet = _dbContext.Sys_ResponsibilitiesDetail.Where(x => x.Id == respDetailId).FirstOrDefault();
                    if (SysResponsibilityDet == null)
                    {
                        //SysResponsibilityDet.UserId = user.Id;
                        SysResponsibilityDet = new Sys_ResponsibilitiesDetail();
                        SysResponsibilityDet.ResponsibilityId = Convert.ToInt32(collection["ResponsibilityId"][i]);
                        SysResponsibilityDet.Responsibility = Convert.ToString(collection["Responsibility"][i]);
                        //SysResponsibilityDet.ResponsibilityKey = Convert.ToString(collection["ResponsibilityKey"][i]);
                        SysResponsibilityDet.Date_from = Convert.ToDateTime(collection["DateFrom"][i]);
                        SysResponsibilityDet.Date_To = Convert.ToDateTime(collection["DateTo"][i]);
                        SysResponsibilityDet.Remarks = collection["Remarks"][i];
                        if (collection["Approve"][i] == "on")
                        {
                            SysResponsibilityDet.Approve = true;
                        }
                        else if (collection["Approve"][i] == "off")
                        {
                            SysResponsibilityDet.Approve = false;
                        }

                        if (collection["UnApprove"][i] == "on")
                        {
                            SysResponsibilityDet.UnApprove = true;
                        }
                        else if (collection["UnApprove"][i] == "off")
                        {
                            SysResponsibilityDet.UnApprove = false;
                        }
                        SysResponsibilityDet.UserId = user.Id;
                        _dbContext.Sys_ResponsibilitiesDetail.Add(SysResponsibilityDet);
                        await _dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        SysResponsibilityDet.UserId = user.Id;
                        SysResponsibilityDet.ResponsibilityId = Convert.ToInt32(collection["ResponsibilityId"][i]);
                        SysResponsibilityDet.Responsibility = Convert.ToString(collection["Responsibility"][i]);
                        //SysResponsibilityDet.ResponsibilityKey = Convert.ToString(collection["ResponsibilityKey"][i]);
                        SysResponsibilityDet.Date_from = Convert.ToDateTime(collection["DateFrom"][i]);
                        SysResponsibilityDet.Date_To = Convert.ToDateTime(collection["DateTo"][i]);
                        SysResponsibilityDet.Remarks = collection["Remarks"][i];
                        if (collection["Approve"][i] == "on")
                        {
                            SysResponsibilityDet.Approve = true;
                        }
                        else if (collection["Approve"][i] == "off")
                        {
                            SysResponsibilityDet.Approve = false;
                        }
                        if (collection["UnApprove"][i] == "on")
                        {
                            SysResponsibilityDet.UnApprove = true;
                        }
                        else if (collection["UnApprove"][i] == "off")
                        {
                            SysResponsibilityDet.UnApprove = false;
                        }

                        _dbContext.Sys_ResponsibilitiesDetail.Update(SysResponsibilityDet);
                        await _dbContext.SaveChangesAsync();
                    }
                }

                // Department working

                var rownberDepartment = collection["DepartmentId"].Count;

                for (int i = 0; i < rownberDepartment; i++)
                {
                    var respDetailId = Convert.ToInt32(collection["DepId"][i]);

                    var SysResponsibilityDet = _dbContext.SysUserDepartments.Where(x => x.Id == respDetailId).FirstOrDefault();
                    if (SysResponsibilityDet == null)
                    {
                        //SysResponsibilityDet.UserId = user.Id;
                        SysResponsibilityDet = new SysUserDepartment();
                        SysResponsibilityDet.DepartmentId = Convert.ToInt32(collection["DepartmentId"][i]);
                        SysResponsibilityDet.Department = Convert.ToString(collection["Department"][i]);
                        SysResponsibilityDet.SubDepartmentId = Convert.ToInt32(collection["SubDepartmentId"][i]);
                        SysResponsibilityDet.SubDepartment = Convert.ToString(collection["subDepartment"][i]);
                        SysResponsibilityDet.UserId = user.Id;
                        _dbContext.SysUserDepartments.Add(SysResponsibilityDet);
                        await _dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        SysResponsibilityDet.UserId = user.Id;
                        SysResponsibilityDet.DepartmentId = Convert.ToInt32(collection["DepartmentId"][i]);
                        SysResponsibilityDet.Department = Convert.ToString(collection["Department"][i]);
                        SysResponsibilityDet.SubDepartmentId = Convert.ToInt32(collection["SubDepartmentId"][i]);
                        SysResponsibilityDet.SubDepartment = Convert.ToString(collection["subDepartment"][i]);
                        _dbContext.SysUserDepartments.Update(SysResponsibilityDet);
                        await _dbContext.SaveChangesAsync();
                    }
                }


                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "User Not Found.";
            }
                return View();

            //}
            //return View(registerViewModel);
        }
        public IActionResult ProfileSetting(string id)
        {
            ViewBag.NavbarHeading = "Profile Setting";
            ApplicationUser user = _userManager.FindByIdAsync(HttpContext.Session.GetString("UserId")).Result;
            var viewModel = new RegisterViewModel();
            viewModel.FullName = user.FullName;
            viewModel.UserName = user.UserName;
            viewModel.Email = user.Email;
            viewModel.Photo = user.Photo;
            viewModel.Id = user.Id;
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> ProfileSetting(RegisterViewModel registerViewModel, IFormFile img)
        {
            if (img != null)
            {
                if (img.Length > 0)
                {
                    var fileName = Path.GetFileName(img.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\userphotos", fileName);
                    using (var Fstream = new FileStream(filePath, FileMode.Create))
                    {
                        await img.CopyToAsync(Fstream);
                        var fullPath = "/img/userphotos/" + fileName;
                        //registerViewModel.Photo = fullPath;
                        registerViewModel.Photo = fullPath;
                    }
                }
            }
            //var user = new ApplicationUser { UserName = registerViewModel.UserName, Email = registerViewModel.Email, FullName = registerViewModel.FullName, Photo = registerViewModel.Photo, Id = registerViewModel.Id };
            var user = await _userManager.FindByIdAsync(HttpContext.Session.GetString("UserId"));
            user.FullName = registerViewModel.FullName;
            user.UserName = registerViewModel.UserName;
            user.Email = registerViewModel.Email;
            user.Photo = registerViewModel.Photo;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["error"] = "false";
                TempData["message"] = "Profile has been Updated Succesfully...!";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Update has been Failed ...!";
            }
            return RedirectToAction("Index", "Dashboard");
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            HttpContext.Session.Clear();
            foreach (var cookie in HttpContext.Request.Cookies)
            {
                Response.Cookies.Delete(cookie.Key);
            }
            return RedirectToAction("Login", "Account", new { area = "" });
        }
        [Authorize(Roles = "Manager")]
        public IActionResult CreateRoleMenu()
        {
            return View();
        }

        //[Authorize(Roles = "Manager")]
       // [Authorize(Roles = "Manager, Admin, Developer, Administrator")]
        public IActionResult Index()
        {
            /*
            var allusers = _numbersDbContext.Users.ToList();
            var users = allusers.Where(x => x.Role.Select(role => role.Name).Contains("User")).ToList();
            var userVM = users.Select(user => new UserViewModel { Username = user.FullName, Roles = string.Join(",", user.Roles.Select(role => role.Name)) }).ToList();

            var admins = allusers.Where(x => x.Roles.Select(role => role.Name).Contains("Admin")).ToList();
            var adminsVM = admins.Select(user => new UserViewModel { Username = user.FullName, Roles = string.Join(",", user.Roles.Select(role => role.Name)) }).ToList();
            var model = new GroupedUserViewModel { Users = userVM, Admins = adminsVM };
            */
            var allusers = _dbContext.Users.Where(c => c.CompanyId == HttpContext.Session.GetInt32("CompanyId").Value).OrderBy(x=>x.FullName).ToList();
            var userVM = allusers.Select(user => new UserViewModel
            { Id = user.Id, FullName = user.FullName, Username = user.UserName, Email = user.Email, Photo = user.Photo }).ToList();
            var model = userVM;
            ViewBag.NavbarHeading = "List of Registered Users";
            return View(model);

        }
        public IActionResult AccessDenied()
        {

            return View();
        }
        public IActionResult MenuList()
        {
            IList<AppMenu> menusList = _dbContext.AppMenus.ToList();
            return View("Menu/MenuList", menusList);
        }
        public IActionResult MenuCreate(int? id)
        {
            AppMenu appMenu = new AppMenu();
            if (id != null)
            {
                ViewBag.EntityState = "Update";
                appMenu = _dbContext.AppMenus.Find(id);
                return View("Menu/MenuCreate", appMenu);

            }
            else
            {
                ViewBag.EntityState = "Create";
                return View("Menu/MenuCreate", appMenu);
            }
        }
        [HttpPost]
        public IActionResult MenuCreate(AppMenu appMenu)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (appMenu.Id == 0)
                    {
                        appMenu.CompanyId = HttpContext.Session.GetInt32("CompanyId").Value;
                        appMenu.CreatedBy = HttpContext.Session.GetString("UserId");
                        appMenu.CreatedDate = DateTime.Now;
                        _dbContext.AppMenus.Add(appMenu);
                    }
                    else
                    {
                        _dbContext.Entry(appMenu).State = EntityState.Modified;
                        appMenu.CompanyId = HttpContext.Session.GetInt32("CompanyId").Value;
                        appMenu.UpdatedBy = HttpContext.Session.GetString("UserId");
                        appMenu.UpdatedDate = DateTime.Now;
                    }
                    _dbContext.SaveChanges();

                    return RedirectToAction("MenuList");
                }
                return View("Menu/MenuList", appMenu);
            }
            catch (Exception Exp)
            {
                var x = Exp;
                return View(appMenu);
            }
        }
        [HttpGet]
        public IActionResult GetMenus(string roleId)
        {
            //var menus = _dbContext.AppMenus
            //    .Where(m=> m.CompanyId==HttpContext.Session.GetInt32("CompanyId"))
            //    .Select(m =>
            //    new
            //    {
            //        Id = m.Id,
            //        Parent = (m.ParentId ==0 ? "#" : m.ParentId.ToString()),
            //        Text = m.Name,
            //        Icon = m.IconClass
            //    });
            //return Ok(menus);

            //var menus1 = _dbContext.AppMenus
            //                       .Join(T)
            //                       .Where(r => r.RoleId == roleId);
            /*
            var menus = (from m in _dbContext.AppMenus
                         join r in _dbContext.AppRoleMenus on m.Id equals r.MenuId into relation
                         from sub in _dbContext.AppRoleMenus.DefaultIfEmpty()
                         where sub.RoleId == roleId
                         select new { m.Id, m.Name, m.ParentId, m.IconClass, Selected = sub.MenuId });
                         */
            //var menus = from m in _dbContext.AppMenus
            //            where (m.CompanyId == HttpContext.Session.GetInt32("CompanyId") && m.IsActive == true)
            //            select new
            //            {
            //                m.Id,
            //                m.Name,
            //                m.ParentId,
            //                m.IconClass,
            //                Check = (_dbContext.AppRoleMenus
            //                            .Where(r => r.MenuId.Equals(m.Id) && r.RoleId.Equals(roleId))
            //                            .Select(r => r.Id)
            //                            ).FirstOrDefault()
            //            };
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var menus = from m in _dbContext.AppMenus
                        from cm in _dbContext.AppCompanyModules
                        where cm.CompanyId == companyId && cm.Id == m.ModuleId && m.ModuleId != 0 && m.IsActive == true
                        select new
                        {
                            m.Id,
                            m.Name,
                            m.ParentId,
                            m.IconClass,
                            Check = (_dbContext.AppRoleMenus
                                        .Where(r => r.MenuId.Equals(m.Id) && r.RoleId.Equals(roleId))
                                        .Select(r => r.Id)
                                        ).FirstOrDefault()
                        };

            List<TreeMenu> treeMenu = new List<TreeMenu>();
            foreach (var item in menus)
            {
                TreeMenu menu = new TreeMenu();
                menu.Id = Convert.ToString(item.Id);
                menu.Text = item.Name;
                menu.Icon = item.IconClass;
                menu.Parent = (item.ParentId == 0 ? "#" : item.ParentId.ToString());
                // menu.Parent = (item.ParentId == 0 ? "#" : Convert.ToString(item.ParentId));
                //menu.State.Selected = false;
                TreeMenuState state = new TreeMenuState();
                if (item.Check == 0)
                    state.Selected = false;
                else
                    state.Selected = true;
                menu.State = state;
                treeMenu.Add(menu);
            }
            return Ok(treeMenu);
        }
        private void SetSessionValues(ApplicationUser applicationUser)
        {
            var userId = applicationUser.Id;
            var company = _dbContext.AppUserCompanies
                            .Include(x => x.Company)
                            .Where(x => x.User.Id == userId).FirstOrDefault();

            var companyID = company.CompanyId;
            var companyName = company.Company.Name;
            var companyLogo = company.Company.Logo;
            var companyCode = company.Company.Code;


            //load default company name for user
            //var companyName = _dbContext.AppUserCompanies
            //                .Where(x => x.User.Id == userId)
            //                .Select(x => x.Company.Name).FirstOrDefault();
            //load role base menus for user. 
            var roles = _userManager.GetRolesAsync(applicationUser);
            //var userIdentity = (ClaimsIdentity)User.Identity;
            //var claims1 = userIdentity.Claims;
            //var roleClaimType = userIdentity.RoleClaimType;
            //var roles1 = claims1.Where(c => c.Type == ClaimTypes.Role).ToList();
            // var roleId = _dbContext.Roles.Where(r => r.Name == roles.Result[0]).Select(r => r.Id);
            //var roleId = from r in _dbContext.Roles
            //             where r.Name == roles.Result[0].ToString()
            //             select new
            //             {
            //                 r.Id
            //             }.ToString();
            //var menu = JsonConvert.SerializeObject(_dbContext.AppRoleMenus
            //    .Where(x => x.Role.Name == roles.Result[0] && x.Menu.IsActive == true && x.Menu.CompanyId==companyID)
            //    .Select(x => x.Menu)
            //    .OrderBy(x => x.Sequence));


            var menu = JsonConvert.SerializeObject(
                from rm in _dbContext.AppRoleMenus
                from cm in _dbContext.AppCompanyModules
                where rm.Role.Name == roles.Result[0] && rm.Menu.IsActive == true && cm.CompanyId == companyID
                && cm.Id == rm.Menu.ModuleId
                orderby rm.Menu.Sequence
                select rm.Menu
                );
            var allmenu = JsonConvert.SerializeObject(
              from rm in _dbContext.AppMenus.Where(x=>x.IsActive==true && x.CompanyId==1)
              select rm
              );

            var reportPath = _dbContext.AppCompanyConfigs
                .Where(n => n.ConfigName == "Report Path")
                .Select(v => v.ConfigValue).FirstOrDefault();
            string hostEnv = _httpContextAccessor.HttpContext.Request.Host.Value;
            //Convert.ToInt32(HttpContext.Session.GetString("ActiveUserId")) = applicationUser.AppUserID;
            //Convert.ToInt32(HttpContext.Session.GetString("ActiveCompanyId")) = companyID;
            //CommonHelper.DateFormat = "dd-MMM-yyyy";

            //HttpContext.Session.SetString("ActiveUserId", "" + applicationUser.AppUserID);
            //HttpContext.Session.SetString("ActiveCompanyId", "" + companyID);

            // HttpContext.Session.SetInt32("AppUserId", applicationUser.AppUserID);
            HttpContext.Session.SetInt32("CompanyId", companyID);
            HttpContext.Session.SetInt32("CompanyCode", companyCode);
            HttpContext.Session.SetString("UserId", applicationUser.Id);
            HttpContext.Session.SetString("UserName", applicationUser.FullName);
            HttpContext.Session.SetString("CompanyName", companyName);
            HttpContext.Session.SetString("CompanyLogo", companyLogo);
            HttpContext.Session.SetString("menus", menu);
            HttpContext.Session.SetString("allmenus", allmenu);
            HttpContext.Session.SetString("ReportPath", reportPath);
            HttpContext.Session.SetString("DateFormat", "dd-MMM-yyyy");
            HttpContext.Session.SetString("hostingEnv", "http://" + hostEnv + "/ReportViewer/");
            AppSession.Configure(_httpContextAccessor);


            //Use Cookies
            const string issuer = "numbers.visionplusapps.com";
            var claims = new List<Claim>
            {
                new Claim("CompanyId",companyID.ToString(),ClaimValueTypes.String,issuer),
                //new Claim("AppUserId",companyID.ToString(),ClaimValueTypes.String,issuer),
                new Claim("DateFormat",companyID.ToString(),ClaimValueTypes.String,issuer),
                new Claim("UserId",companyID.ToString(),ClaimValueTypes.String,issuer)
            };
        }
        [HttpGet]
        public IActionResult ChangePassword()
        {
            ViewBag.NavbarHeading = "Change Password";
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["error"] = "true";
                TempData["message"] = "Please Enter Correct Password.";
                return RedirectToAction(nameof(ChangePassword));
            }
            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.ConfirmNewPassword);
            if (result.Succeeded)
            {
                TempData["error"] = "false";
                TempData["message"] = "You have Successfully Changed Your Password...";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong while changing Password.";
            }
            return RedirectToAction(nameof(ChangePassword));
        }


        public IActionResult GetResponsibilities(int id)
        {
            var resp = _dbContext.Sys_Responsibilities.Where(x => x.Resp_Id == id).FirstOrDefault();

            return Ok(resp);
        }

        public IActionResult GetList()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchFullName = Request.Form["columns[0][search][value]"].FirstOrDefault();
                var searchUsername = Request.Form["columns[1][search][value]"].FirstOrDefault();
                var searchEmail = Request.Form["columns[2][search][value]"].FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                var Data = (from DC in _dbContext.Users.Where(c => c.CompanyId == companyId).OrderBy(x => x.FullName) select DC);
                if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
                {
                    Data = Data.OrderBy(sortColumn + " " + sortColumnDirection);
                    
                }

                Data = !string.IsNullOrEmpty(searchFullName) ? Data.Where(m => m.FullName.ToString().ToLower().Contains(searchFullName.ToLower())) : Data;
                Data = !string.IsNullOrEmpty(searchUsername) ? Data.Where(m => m.UserName.ToString().ToLower().Contains(searchUsername.ToLower())) : Data;
                Data = !string.IsNullOrEmpty(searchEmail) ? Data.Where(m => m.Email.ToString().ToLower().Contains(searchEmail.ToLower())) : Data;

                recordsTotal = Data.Count();
                var data = Data.Skip(skip).Take(pageSize).ToList();
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data };
                return Ok(jsonData);
            }
            catch (Exception)
            {
                throw;
            }
        }










        public class TreeMenu
        {
            public string Id { get; set; }
            public string Text { get; set; }
            public string Parent { get; set; }
            public string Icon { get; set; }
            public string[] Parents { get; set; }
            public TreeMenuState State { get; set; }
        }
        public class TreeMenuState
        {
            public bool Selected { get; set; }
        }
    }
}
