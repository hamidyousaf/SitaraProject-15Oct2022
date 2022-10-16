using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Repository.AppModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Areas.Approval.Controllers
{
    [Area("Approval")]
    [Authorize]
    public class RulesApprovalController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        private readonly Sys_Rules_ApprovalRepository _sys_Rules_ApprovalRepository;
        private readonly Sys_Rules_Approval_DetailsRepository _sys_Rules_Approval_DetailsRepository;
        private readonly SysApprovalGroupRepository _SysApprovalGroupRepository;


        public RulesApprovalController(NumbersDbContext context, Sys_Rules_ApprovalRepository sys_Rules_ApprovalRepository, Sys_Rules_Approval_DetailsRepository sys_Rules_Approval_DetailsRepository, 
            SysApprovalGroupRepository SysApprovalGroupRepository)
        {
            _dbContext = context;
            _sys_Rules_ApprovalRepository = sys_Rules_ApprovalRepository;
            _sys_Rules_Approval_DetailsRepository = sys_Rules_Approval_DetailsRepository;
            _SysApprovalGroupRepository = SysApprovalGroupRepository;
        }
        public IActionResult List()
        {
            var RulesGroupVmLsit = new List<RulesGroupVm>();
            var model = _sys_Rules_ApprovalRepository.Get(x => x.IsActive).ToList();
            foreach (var modl in model)
            {
                RulesGroupVm rulesGroupVm = new RulesGroupVm();
                rulesGroupVm.Sys_Rules_Approval = modl;
                RulesGroupVmLsit.Add(rulesGroupVm);
            }
            return View(RulesGroupVmLsit);
        }
        public IActionResult Index(int? id)
        {
            var model = new RulesGroupVm();
            if (id.HasValue)
            {
                model.Sys_Rules_Approval = _sys_Rules_ApprovalRepository.Get(x => x.IsActive && x.Id == id).FirstOrDefault();
                model.Sys_Rules_Approval_Details = _sys_Rules_Approval_DetailsRepository.Get(x => x.Rule_Id == id).ToList();
            }
            ViewBag.GroupList = _SysApprovalGroupRepository.Get(a => a.IsActive).Select(a => new { id = a.Id, name = a.Group_Name, }).ToList();
            ViewBag.Operator = _dbContext.AppCompanyConfigs.Where(a => a.BaseId == 1069).Select(a => new { id = a.Id, name = a.ConfigValue }).ToList();
            ViewBag.AppCompanyConfigs = (from a in _dbContext.AppCompanyConfigs where a.BaseId == 1066 select a).ToList();
            ViewBag.GLDivision = (from g in _dbContext.GLDivision select g).ToList();

            var tableList = _dbContext.Model.GetEntityTypes().Select(a => new { name = a.Name, }).Distinct().ToList();
            List<string> Types = new List<string>();
            foreach (var table in tableList)
            {
                if (table.name.Contains("Numbers.Entity.Models"))
                {
                    var tbl = table.name.Remove(0, 22);
                    Types.Add(tbl);
                }
            }
            ViewBag.TypeList = Types;
            return View(model);
        }

        public async Task<IActionResult> Create(RulesGroupVm rulesGroupVm, IFormCollection collection)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            string userId = HttpContext.Session.GetString("UserId");
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            if (rulesGroupVm.Sys_Rules_Approval.Id == 0)
            {
                Sys_Rules_Approval sys_Rules_Approval = new Sys_Rules_Approval();
                sys_Rules_Approval.CreatedBy = userId;
                sys_Rules_Approval.CreatedDate = DateTime.Now;
                sys_Rules_Approval.IsActive = true;
                sys_Rules_Approval.IsDeleted = false;
                sys_Rules_Approval.Group = Convert.ToString(collection["Group"]);
                sys_Rules_Approval.Type = Convert.ToString(collection["Type"]);
                sys_Rules_Approval.Rule_Name = rulesGroupVm.Sys_Rules_Approval.Rule_Name;
                sys_Rules_Approval.Description = rulesGroupVm.Sys_Rules_Approval.Description;
                await _sys_Rules_ApprovalRepository.CreateAsync(sys_Rules_Approval);
                var rowNumber = collection["Attribute_Name"].Count;
                for (int i = 0; i < rowNumber; i++)
                {
                    Sys_Rules_Approval_Details sys_Rules_Approval_Details = new Sys_Rules_Approval_Details();
                    sys_Rules_Approval_Details.Attribute_Name = Convert.ToString(collection["Attribute_Name"][i]);
                    sys_Rules_Approval_Details.Operator = Convert.ToString(collection["Operator_Name"][i]);
                    sys_Rules_Approval_Details.Value = Convert.ToString(collection["Valueid"][i]);
                    sys_Rules_Approval_Details.Table_Name = Convert.ToString(collection["Table_Name"][i]);
                    sys_Rules_Approval_Details.Rule_Id = sys_Rules_Approval.Id;
                    await _sys_Rules_Approval_DetailsRepository.CreateAsync(sys_Rules_Approval_Details);
                }
            }
            else
            {
                Sys_Rules_Approval sys_Rules_Approval = _sys_Rules_ApprovalRepository.Get(x => x.Id == rulesGroupVm.Sys_Rules_Approval.Id).FirstOrDefault();
                sys_Rules_Approval.Rule_Name = rulesGroupVm.Sys_Rules_Approval.Rule_Name;
                sys_Rules_Approval.Description = rulesGroupVm.Sys_Rules_Approval.Description;
                sys_Rules_Approval.UpdatedBy = userId;
                sys_Rules_Approval.UpdatedDate = DateTime.Now;
                sys_Rules_Approval.IsActive = true;
                await _sys_Rules_ApprovalRepository.UpdateAsync(sys_Rules_Approval);
                var rowNumber = collection["Attribute"].Count;
                var existing_Rules = _sys_Rules_Approval_DetailsRepository.Get(x => x.Rule_Id == sys_Rules_Approval.Id).ToList();
                var myList = new List<Sys_Rules_Approval_Details>();
                if (rowNumber != existing_Rules.Count)
                {
                    for (int i = 0; i < rowNumber; i++)
                    {
                        string atribute = Convert.ToString(collection["Attribute"][i]);
                        string table = Convert.ToString(collection["Table"][i]);
                        var ruledetail =_sys_Rules_Approval_DetailsRepository.Get(x => x.Rule_Id == sys_Rules_Approval.Id && x.Attribute_Name == atribute && x.Table_Name == table).FirstOrDefault();
                        myList.Add(ruledetail);
                    }
                }
                if (myList.Count > 0)
                {
                    foreach (var usr in existing_Rules)
                    {
                        bool result = myList.Exists(s => s.Table_Name == usr.Table_Name && s.Attribute_Name == usr.Attribute_Name && s.Value == usr.Value);
                        var resultCount = myList.Count(s => s.Table_Name == usr.Table_Name && s.Attribute_Name == usr.Attribute_Name && s.Value == usr.Value);
                        if (!result || resultCount > 1)
                        {
                            await _sys_Rules_Approval_DetailsRepository.DeleteAsync(usr);
                        }

                    }
                }
                if (rowNumber == 0)
                {
                    foreach (var usr in existing_Rules)
                    {
                        await _sys_Rules_Approval_DetailsRepository.DeleteAsync(usr);
                    }
                }
                var rowNmber = collection["Attribute_Name"].Count;
                for (int i = 0; i < rowNmber; i++)
                {
                    Sys_Rules_Approval_Details sys_Rules_Approval_Details = new Sys_Rules_Approval_Details();
                    sys_Rules_Approval_Details.Attribute_Name = Convert.ToString(collection["Attribute_Name"][i]);
                    sys_Rules_Approval_Details.Operator = Convert.ToString(collection["Operator_Name"][i]);
                    sys_Rules_Approval_Details.Value = Convert.ToString(collection["Valueid"][i]);
                    sys_Rules_Approval_Details.Table_Name = Convert.ToString(collection["Table_Name"][i]);
                    sys_Rules_Approval_Details.Rule_Id = sys_Rules_Approval.Id;
                    await _sys_Rules_Approval_DetailsRepository.CreateAsync(sys_Rules_Approval_Details);
                }

            }
            return RedirectToAction("Index");
        }

        public IActionResult GetAttribute(string tableName)
        {
            var type = "Numbers.Entity.Models." + tableName;
            var entityType = _dbContext.Model.FindEntityType(type);
            List<string> Types = new List<string>();

            foreach (var property in entityType.GetProperties())
            {
                var columnName = property.Relational().ColumnName;
                Types.Add(columnName);
            };
            return Ok(Types);
        }
    }
}