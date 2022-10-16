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
    public class JobDescriptionController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public JobDescriptionController(NumbersDbContext context)
        {
            _dbContext = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            JobDescriptionRepo objList = new JobDescriptionRepo(_dbContext);
            var list = objList.GetAll();
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
                JobDescriptionRepo objCreate = new JobDescriptionRepo(_dbContext);
                var description = objCreate.GetById(id);
                return View(description);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(HRJobDescription model)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (!ModelState.IsValid)
            {
                return View();
            }
            else
            {
                if (model.Id == 0)
                {
                    model.CreatedBy = userId;
                    model.CreatedDate = DateTime.Now;
                    model.IsDeleted = false;
                    JobDescriptionRepo objCreate = new JobDescriptionRepo(_dbContext);
                    bool isSuccess = await objCreate.Create(model);
                    if (isSuccess == true)
                    {
                        TempData["error"] = "false";
                        TempData["message"] = "Job Description has been created successfully.";
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
                    model.UpdatedBy = userId;
                    JobDescriptionRepo objUpdate = new JobDescriptionRepo(_dbContext);
                    bool isSuccess = await objUpdate.Update(model);
                    if (isSuccess == true)
                    {
                        TempData["error"] = "false";
                        TempData["message"] = "Job Description has been updated successfully.";
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
            JobDescriptionRepo objDelete = new JobDescriptionRepo(_dbContext);
            bool isSuccess = await objDelete.Delete(id);
            if (isSuccess == true)
            {
                TempData["error"] = "false";
                TempData["message"] = "Job Description has been deleted successfully.";
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