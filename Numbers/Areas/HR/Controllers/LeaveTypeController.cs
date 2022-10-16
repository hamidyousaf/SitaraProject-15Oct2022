using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Numbers.Entity.Models;
using Numbers.Repository.HR;

namespace Numbers.Areas.HR.Controllers
{
    [Area("HR")]
    [Authorize]
    public class LeaveTypeController : Controller
    {

        [HttpGet]
        public IActionResult Index()
        {
            var list =  LeaveTypeRepo.GetAll();
            return View(list);
        }

        [HttpGet]
        public IActionResult Create(int id)
        {
            if (id == 0)
            {
                return View();
            }
            else
            {
                var objEdit =  LeaveTypeRepo.GetById(id);
                return View(objEdit);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(HRLeaveType model)
        {

                if (model.Id == 0)
                {
                    
                    bool isSuccess = await LeaveTypeRepo.Create(model);
                    if (isSuccess == true)
                    {
                        TempData["error"] = "false";
                        TempData["message"] = "Leave Type has been created successfully.";
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
                    bool isSuccess = await LeaveTypeRepo.Update(model);
                    if (isSuccess == true)
                    {
                        TempData["error"] = "false";
                        TempData["message"] = "Leave Type has been updated successfully.";
                    }
                    else
                    {
                        TempData["error"] = "true";
                        TempData["message"] = "Something went wrong.";
                    }
                    return RedirectToAction(nameof(Index));
                }
            
        }

        public async Task<IActionResult> Delete(int id)
        {
            bool isSuccess =await  LeaveTypeRepo.Delete(id);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Leave Type has been deleted successfully.";
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