using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;

namespace Numbers.Controllers
{
    public class RoleController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        //NumbersDbContext numbersDbContext;
        private readonly NumbersDbContext _dbContext;
        public RoleController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, NumbersDbContext numbersDbContext)
        {
            _dbContext = numbersDbContext;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            //  var allusers = _dbContext.Roles.ToList();
            var allusers = _dbContext.Roles.FromSql("select * from AspNetRoles WHERE CompanyId={0}", companyId).ToList();
            var userVM = allusers.Select(role => new RoleViewModel
            { RoleId = role.Id, RoleName = role.Name }).ToList();
            var model = userVM;
            ViewBag.NavbarHeading = "List of Roles";
            return View(model);
        }
        public IActionResult CreateRole()
        {
            ViewBag.NavbarHeading = "Create New Role";
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(RoleViewModel roleViewModel)
        {    
            //var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            IdentityResult roleResult;
            //Adding Admin Role
            var roleCheck = await _roleManager.RoleExistsAsync(roleViewModel.RoleName);
            if (!roleCheck)
            {
                //create the roles and seed them to the database
                roleResult = await _roleManager.CreateAsync(new IdentityRole(roleViewModel.RoleName));
                return RedirectToAction(nameof(Index));
            }
            else
                ViewBag.Message = "Role already exists.";
            return View();
        }
        [HttpGet]
        public IActionResult AssignRole(string id)
        { 
            RoleViewModel role = new RoleViewModel();
            role.RoleId = id;
            ViewBag.NavbarHeading = "Setup Permission for User Role";
            //Show role name at front end assign role window
            ViewBag.RoleName = _dbContext.Roles
                .Where(r => r.Id==id)            
                .Select(r => r.Name).FirstOrDefault();

            return View(role);
        }
        [HttpPost]
        public IActionResult AssignRole(RoleViewModel roleView)
        {
            //delete old roles and insert new
            var roles = _dbContext.AppRoleMenus.Where(r => r.RoleId.Equals(roleView.RoleId));
            _dbContext.AppRoleMenus.RemoveRange(roles);
            //insert new roles
            string[] menuId = roleView.Menus.Split(',');
            foreach (string id in menuId)
            {
                AppRoleMenu menu = new AppRoleMenu();
                menu.MenuId = Convert.ToInt32(id);
                menu.RoleId = roleView.RoleId;
                _dbContext.AppRoleMenus.Add(menu);

            }

            
            _dbContext.SaveChanges();
            return RedirectToAction(nameof(Index));

        }
    }
}