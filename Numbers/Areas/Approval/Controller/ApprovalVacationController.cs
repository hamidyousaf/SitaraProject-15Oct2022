using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Numbers.Areas.Approval.Controllers
{
    [Area("Approval")]
    [Authorize]
    public class ApprovalVacationController : Controller
    {
        private readonly NumbersDbContext _dbContext;

        public ApprovalVacationController(NumbersDbContext context)
        {
            _dbContext = context;
        }
        public IActionResult Index()
        {
            ViewBag.User = _dbContext.Users.Where(c => c.CompanyId == HttpContext.Session.GetInt32("CompanyId").Value).ToList();
            ViewBag.Vacation= _dbContext.Sys_Vacation_Rule.Where(c => c.CompanyId == HttpContext.Session.GetInt32("CompanyId").Value&& c.IsDeleted==false).ToList();
            return View();
        }
        public IActionResult Submit( IFormCollection collection)
        {
            //int id = Convert.ToInt32(collection["baseId"].ToString());
            string deletedIds = collection["deletedIds"].ToString();
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            Sys_Vacation_Rule[] data = JsonConvert.DeserializeObject<Sys_Vacation_Rule[]>(collection["data"]);
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            foreach (Sys_Vacation_Rule item in data)
            {
                if (item.Id == 0)
                {
                    item.CompanyId = companyId;
                    item.CreatedBy = userId;
                    item.CreatedDate = DateTime.Now;
                    item.Resp_ID = resp_Id;
                    item.ToUserId = item.ToUserId.ToString();
                    item.FromUserId = item.FromUserId.ToString();
                    _dbContext.Sys_Vacation_Rule.Add(item);
                    _dbContext.SaveChanges();
                }
                else
                {
                    item.CompanyId = companyId;
                    item.UpdatedBy = userId;
                    item.UpdatedDate = DateTime.Now;
                    item.Resp_ID = resp_Id;
                    _dbContext.Sys_Vacation_Rule.Update(item);
                    _dbContext.SaveChanges();
                }
            }
            foreach (var item in deletedIds.Split(","))
            {
                if (item != "")
                {
                    Sys_Vacation_Rule rule = _dbContext.Sys_Vacation_Rule.Find(Convert.ToInt32(item));
                    rule.IsDeleted = true;
                    rule.UpdatedBy = userId;
                    rule.UpdatedDate = DateTime.Now;
                    _dbContext.Sys_Vacation_Rule.Update(rule);
                    _dbContext.SaveChanges();
                }
            }
            TempData["error"] = false;
            return RedirectToAction("Index","ApprovalVacationRule");
        }
    }
}
