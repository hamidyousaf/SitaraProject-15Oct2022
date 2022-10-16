using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Numbers.Entity.Models;
using Numbers.Repository.HR;

namespace Numbers.Areas.HR.Controllers
{
    [Area("HR")]
    [Authorize]
    public class LeaveGroupController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            var list = LeaveGroupRepo.GetAll();
            return View(list);
        }

        [HttpGet]
        public IActionResult Create(int id)
        {
            if (id == 0)
            {
                ViewBag.EntityState = "Create";
                return View(new HRLeaveTypeGroup());
            }
            else
            {
                ViewBag.EntityState = "Update";
                var groupEdit = LeaveGroupRepo.GetById(id);
                return View(groupEdit);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(HRLeaveTypeGroup model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            else
            {
                if (model.Id == 0)
                {
                    bool isSuccess = await LeaveGroupRepo.Create(model);
                    if (isSuccess == true)
                    {
                        TempData["error"] = "false";
                        TempData["message"] = "Leave Group Type has been created successfully.";
                    }
                    else
                    {
                        TempData["error"] = "true";
                        TempData["message"] = "Something went wrong.";
                    }
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    bool isSuccess = await LeaveGroupRepo.Update(model);
                    if (isSuccess == true)
                    {
                        TempData["error"] = "false";
                        TempData["message"] = "Leave Group Type has been updated successfully.";
                    }
                    else
                    {
                        TempData["error"] = "true";
                        TempData["message"] = "Something went wrong.";
                    }
                    return RedirectToAction(nameof(Index));
                }
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            bool isSuccess = await LeaveGroupRepo.Delete(id);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Leave Group Type has been deleted successfully.";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}