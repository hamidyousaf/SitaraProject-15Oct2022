using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Numbers.Entity.Models;
using Numbers.Repository.HR;

namespace Numbers.Areas.HR.Controllers
{
    [Area("HR")]
    [Authorize]
    public class LeaveController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            IEnumerable<HRLeave> list = LeaveRepo.GetAll();
            return View(list);
        }

        [HttpGet]
        public IActionResult Create(int id)
        {
            if (id == 0)
            {
                return View(new HRLeave());
            }
            else
            {
                HRLeave leaveCreate = LeaveRepo.GetById(id);
                return View(leaveCreate);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(HRLeave model)
        { 
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            else
            {
                if (model.Id == 0)
                {
                    bool isSuccess =await LeaveRepo.Create(model);
                    if (isSuccess == true)
                    {
                        TempData["error"] = "false";
                        TempData["message"] = "Leave has been created successfully.";
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
                    bool isSuccess = await LeaveRepo.Update(model);
                    if (isSuccess == true)
                    {
                        TempData["error"] = "false";
                        TempData["message"] = "Leave has been updated successfully.";
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
            bool isSuccess = await LeaveRepo.Delete(id);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Leave has been deleted successfully.";
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