using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Numbers.Entity.Models;
using Numbers.Repository.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Areas.Report.Controllers
{
    [Area("Report")]
    [Authorize]
    public class ReportController : Controller
    {
        private readonly NumbersDbContext _dbContext;
        public ReportController(NumbersDbContext context)
        {
            _dbContext = context;
        }
        [Authorize]
        [HttpGet]
        public IActionResult Index(string module)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            ViewBag.Resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            var Items = _dbContext.InvItems.Where(a => a.IsDeleted == false && a.CompanyId == companyId && a.IsActive == true)
                                            .Select(a => new
                                            {
                                                id = a.Id,
                                                text = string.Concat(a.Code, " - ", a.Name),
                                                code = a.Code
                                            })
                                           .OrderBy(a => a.text)
                                           .ToList();
            var Categories = _dbContext.InvItemCategories.Where(a => /*a.CompanyId == companyId &&*/ a.CategoryLevel == 2).Select(a => new
                                                         {
                                                             id = a.Id,
                                                             code = a.Code,
                                                             text = string.Concat(a.Code, " - ", a.Name)
                                                         });
            var Categories4 = _dbContext.InvItemCategories.Where(a => /*a.CompanyId == companyId &&*/ a.CategoryLevel == 4).Select(a => new
            {
                id = a.Id,
                code = a.Code,
                text = string.Concat(a.Code, " - ", a.Name)
            });

            var suppliers = _dbContext.APSuppliers.Where((c => c.CompanyId == companyId))
                                               .Select(c => new
                                               {
                                                   id = c.Id,
                                                   text = c.Name
                                               })
                                               .OrderBy(a => a.text)
                                               .ToList();
            var CostCenters = _dbContext.CostCenter.Where((c => c.CompanyId == companyId))
                                              .Select(c => new
                                              {
                                                  id = c.Id,
                                                  text = c.Description
                                              })
                                              .OrderBy(a => a.text)
                                              .ToList();
            var Contract = _dbContext.GRWeavingContracts.Where((c => c.CompanyId == companyId))
                                              .Select(c => new
                                              {
                                                  id = c.Id,
                                                  text = c.TransactionNo
                                              })
                                              .OrderBy(a => a.text)
                                              .ToList();
            var PoNo = _dbContext.APPurchaseOrders.Where((c => c.CompanyId == companyId))
                                               .Select(c => new
                                               {
                                                   id = c.Id,
                                                   text = c.PONo
                                               })
                                               .OrderBy(a => a.text)
                                               .ToList();
            var IGP = _dbContext.APIGP.Where((c => c.CompanyId == companyId))
                                               .Select(c => new
                                               {
                                                   id = c.Id,
                                                   text = c.IGP
                                               })
                                               .OrderBy(a => a.text)
                                               .ToList();
            var IssueNo = _dbContext.YarnIssuances.Where((c => c.CompanyId == companyId))
                                             .Select(c => new
                                             {
                                                 id = c.Id,
                                                 text = c.IssueNo
                                             })
                                             .OrderBy(a => a.text)
                                             .ToList();
            var Weaver = _dbContext.APSuppliers.Where((c => c.CompanyId == companyId))
                                            .Select(c => new
                                            {
                                                id = c.Id,
                                                text = c.Name
                                            })
                                            .OrderBy(a => a.text)
                                            .ToList();

            var Departments = _dbContext.GLDivision.Where((c => c.CompanyId == companyId))
                                              .Select(c => new
                                              {
                                                  id = c.Id,
                                                  text = c.Name
                                              })
                                              .OrderBy(a => a.text)
                                              .ToList();
            
            var manufactures = _dbContext.AppCompanyConfigs.Where(m => m.Module == "Inventory" && m.ConfigName == "Manufacture Company" && m.CompanyId == companyId && m.IsDeleted == false)
                                             .Select(m => new
                                             {
                                                 id = m.Id,
                                                 text = m.ConfigValue
                                             })
                                             .OrderBy(m => m.text)
                                             .ToList();
            var Counteries = _dbContext.AppCountries
                                              .Select(c => new
                                              {
                                                  id = c.Id,
                                                  text = c.Name
                                              })
                                              .OrderBy(a => a.text)
                                              .ToList();
            var MpNo = _dbContext.PlanMonthlyPlanning
                                            .Select(c => new
                                            {
                                                id = c.Id,
                                                text = string.Concat(" MP#: ", c.TransactionNo)
                                            })
                                            .OrderBy(a => a.text)
                                            .ToList();
              var Transdate = _dbContext.PlanMonthlyPlanning
                                              .Select(c => new
                                              {
                                                  id = c.Id,
                                                  text = c.TransactionDate
                                              })
                                              .OrderBy(a => a.text)
                                              .ToList();
            var Planof = _dbContext.PlanMonthlyPlanning
                                            .Select(c => new
                                            {
                                                id = c.Id,
                                                text = c.Planofmonth
                                            })
                                            .OrderBy(a => a.text)
                                            .ToList();
            
            var Seasonwise = (from a in _dbContext.SeasonalPlaning
                              join b in _dbContext.PlanMonthlyPlanning on a.SeasonId equals b.SPId
                                            select new
                                            {
                                                id = a.SeasonId,
                                                text = b.SP
                                            })
                                            .OrderBy(a => a.text)
                                            .ToList();

            var SpNo = _dbContext.SeasonalPlaning.Where(c=>c.CompanyId==companyId)
                                         .Select(c => new
                                         {
                                         id = c.Id,
                                         text = string.Concat(" Sp#: ", c.TransactionNo)
                                         })
                                         .Distinct().OrderBy(c => c.text)
                                         .ToList();
         
            var GreigeQuality = _dbContext.GRQuality
                                        .Select(c => new
                                        {
                                        id = c.Id,
                                        text = c.Description
                                        })
                                       .OrderBy(c => c.text)
                                       .ToList();
            var SeasonalNo = _dbContext.SeasonalPlaning
                                     .Select(c => new
                                     {
                                         id = c.Id,
                                         text = string.Concat(" Sp#: ", c.TransactionNo)
                                     })
                                    .Distinct().OrderBy(c => c.text)
                                     .ToList();
            var SeasonalGreigeQuality = _dbContext.GRQuality
                                        .Select(c => new
                                        {
                                            id = c.Id,
                                            text = c.Description
                                        })
                                       .Distinct().OrderBy(c => c.text)
                                       .ToList();
            var Season =(from a in _dbContext.SeasonalPlaning
                        join b in _dbContext.AppCompanyConfigs on a.SeasonId equals b.Id
                                        select new
                                        {
                                        id=a.SeasonId,
                                        text = b.ConfigValue
                          
                                        })
                                       .Distinct().OrderBy(c => c.text)
                                       .ToList();
            var cities = _dbContext.AppCities
                                              .Select(c => new
                                              {
                                                  id = c.Id,
                                                  text = c.Name
                                              })
                                              .OrderBy(a => a.text)
                                              .ToList();
            ViewBag.Items = new SelectList(Items.OrderBy(x => x.code), "id", "text").ToList();
            ViewBag.Reports = new SelectList(_dbContext.AppReports.Where(s => s.IsListed == true && s.Module == module  && s.IsActive == true).ToList(), "Name", "Description");
            ViewBag.Categories = new SelectList(Categories, "id", "text").ToList();
            ViewBag.Categories4 = new SelectList(Categories4, "id", "text").ToList();
            ViewBag.CostCenters= new SelectList(CostCenters, "id", "text").ToList();
            ViewBag.Suppliers = new SelectList(suppliers, "id", "text").ToList();
            ViewBag.Departments = new SelectList(Departments, "id", "text").ToList();
            ViewBag.Manufactures = new SelectList(manufactures, "id", "text").ToList();
            ViewBag.Countries = new SelectList(Counteries, "id", "text").ToList();
            ViewBag.Cities = new SelectList(cities, "id", "text").ToList();
            ViewBag.Contract = new SelectList(Contract, "id", "text").ToList();
            ViewBag.PoNo = new SelectList(PoNo, "id", "text").ToList();
            ViewBag.IGP = new SelectList(IGP, "id", "text").ToList();
            ViewBag.IssueNo = new SelectList(IssueNo, "id", "text").ToList();
            ViewBag.Weaver = new SelectList(Weaver, "id", "text").ToList();
            ViewBag.SpNo = new SelectList(SpNo, "id", "text").ToList();
            ViewBag.GreigeQuality = new SelectList(GreigeQuality, "id", "text").ToList();
            ViewBag.Season = new SelectList(Season, "id", "text").ToList();
            ViewBag.MpNo = new SelectList(MpNo, "id", "text").ToList();
            ViewBag.Transdate = new SelectList(Transdate, "id", "text").ToList();
            ViewBag.Planof = new SelectList(Planof, "id", "text").ToList(); 
            ViewBag.SeasonalNo = new SelectList(SeasonalNo, "id", "text").ToList(); 
            ViewBag.SeasonalGreigeQuality = new SelectList(SeasonalGreigeQuality, "id", "text").ToList(); 
          //  ViewBag.SpNo = new SelectList(SpNo, "id", "text").ToList();
            ViewBag.Seasonwise = new SelectList(Seasonwise, "id", "text").ToList();


            ViewBag.NavbarHeading = "Preview Reports";
            return View();
        }
        [HttpGet]
        public IActionResult Template1(string module)
        {
            try
            {
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                var Items = _dbContext.InvItems.Where(a => a.IsDeleted == false &&  a.IsActive == true)
                                            .Select(a => new
                                            {
                                                id = a.Id,
                                                text = string.Concat(a.Code, " - ", a.Name),
                                                code = a.Code
                                            })
                                           .OrderBy(a => a.text)
                                           .ToList();
                var itemcode = _dbContext.AppCompanySetups.Where(x => x.Name == "Local Finish ItemCode").FirstOrDefault().Value;
                 
                var Categories = _dbContext.InvItemCategories.Where(a => a.CompanyId == companyId && a.CategoryLevel == 2 && a.Code.StartsWith(itemcode)).Select(a => new
                {
                    id = a.Id,
                    code = a.Code,
                    text = string.Concat(a.Code, " - ", a.Name)
                    
                });
                var Brand = _dbContext.InvItemCategories.Where(a => a.CompanyId == companyId && a.CategoryLevel == 4 && a.Code.StartsWith("07")).Select(a => new
                {
                    id = a.Id,
                    code = a.Code,
                    text = string.Concat(a.Code, " - ", a.Name)
                });
                var manufactures = _dbContext.AppCompanyConfigs.Where(m => m.Module == "Inventory" && m.ConfigName == "Manufacture Company" && m.CompanyId == companyId && m.IsDeleted == false)
                                            .Select(m => new
                                            {
                                                id = m.Id,
                                                text = m.ConfigValue
                                            })
                                            .OrderBy(m => m.text)
                                            .ToList();
                var customers = _dbContext.ARCustomers.Where((c => c.CompanyId == companyId && c.IsActive == true))
                                              .Select(c => new
                                              {
                                                  id = c.Id,
                                                  text = c.Name
                                              })
                                              .OrderBy(a => a.text)
                                              .ToList();
                //var RecoveryCategory = (from b in _dbContext.AppCompanyConfigBases
                //                        join c in _dbContext.AppCompanyConfigs on b.Id equals c.BaseId
                //                        wherec.IsActive && c.IsDeleted == false && b.Name == configName && b.Module == module
                //                        select new
                //                        {
                //                            id = a.CategoryType_Id,
                //                            text = b.ConfigValue
                //                        })
                //                       .Distinct().OrderBy(c => c.text)
                //                       .ToList();

                //var RecoveryCategory = _dbContext.ARRecoveryPercentageItem
                //                       .Select(c => new
                //                       {
                //                           id = c.Id,
                //                           text = c.CategoryType_Id
                //                       })
                //                       .OrderBy(a => a.text)
                //                       .ToList();
                //var SecondItemCategory = _dbContext.InvItemCategories.Where(a => a.CompanyId == companyId && a.CategoryLevel == 2 && a.Code.StartsWith(itemcode)).Select(a => new
                //{
                //    id = a.Id,

                //    text = a.Name
                //});

                var SecondItemCategory = _dbContext.InvItemCategories.Where(a => a.CompanyId == companyId && a.CategoryLevel == 2 && a.Code.StartsWith(itemcode))
                    .Select(a => new
                {
                    id = a.Id,
                    //code = a.Code,
                    //text = string.Concat(a.Code, " - ", a.Name)
                    text = a.Name
                });
                var SalesPerson =  _dbContext.ARSalePerson.Where(a => a.CompanyId == companyId && a.IsActive == true).Select(c => new
                {
                    id = c.ID,
                    text = c.Name
                })
                .OrderBy(a => a.text).ToList();
                var customerCategory =  _dbContext.AppCompanyConfigs.Where(a => a.BaseId == 40 && a.CompanyId == companyId).Select(c => new
                {
                    id = c.Id,
                    text = c.ConfigValue
                })
                .OrderBy(a => a.text).ToList();
                var cities = _dbContext.AppCities.Select(c => new
                {
                    id = c.Id,
                    text = c.Name
                })
                .OrderBy(c => c.text)
                .ToList();
                var DCNo = _dbContext.ARDeliveryChallans.Select(c => new
                {
                    id = c.Id,
                    text = c.DCNo
                })
                .OrderBy(c => c.text)
                .ToList();
                var Vendors = _dbContext.APSuppliers.Where((c => c.CompanyId == companyId))
                                               .Select(c => new
                                               {
                                                   id = c.Id,
                                                   text = c.Name
                                               })
                                               .OrderBy(a => a.text)
                                               .ToList();

                var IrnNo = _dbContext.APIRN.Where((c => c.CompanyId == companyId))
                                               .Select(c => new
                                               {
                                                   id = c.IRNNo,
                                                   text = c.IRNNo
                                               })
                                               .OrderBy(a => a.text)
                                               .ToList();
                var PoNo = _dbContext.APPurchaseOrders.Where((c =>c.CompanyId == companyId))
                                               .Select(c => new
                                               {
                                                   id = c.PONo,
                                                   text = c.PONo
                                               })
                                               .OrderBy(a => a.text)
                                               .ToList();
                var InvoiceNo = _dbContext.ARInvoices.Select(c => new
                {
                    id = c.Id,
                    text = c.InvoiceNo
                })
                .OrderBy(c => c.text)
                .ToList();
                var Agent = _dbContext.ARCommissionAgents.Where((a => a.CompanyId == companyId))
                                                .Select(a => new
                                                {
                                                    id = a.Id,
                                                    text = a.Name
                                                })
                                                  .OrderBy(a => a.text)
                                                 .ToList();
                var Customers = _dbContext.ARCustomers.Select(c => new
                {
                    id = c.Id,
                    text = String.Concat(c.Id,"-",c.Name)
                })
                .OrderBy(c => c.text)
                .ToList();
                var configValues = new ConfigValues(_dbContext);
                ViewBag.Reports = new SelectList(_dbContext.AppReports.Where(s => s.IsListed == true && s.Module == module  && s.IsActive == true).ToList(), "Name", "Description");
                ViewBag.Items = new SelectList(Items.OrderBy(x => x.code), "id", "text").ToList();
                ViewBag.Categories = new SelectList(Categories, "id", "text").ToList();
                ViewBag.Brand= new SelectList(Brand, "id", "text").ToList();
                ViewBag.Manufactures = new SelectList(manufactures, "id", "text").ToList();
                ViewBag.customers = new SelectList(customers, "id", "text").ToList();
                ViewBag.SalesPerson = new SelectList(SalesPerson, "id", "text").ToList();
                ViewBag.CustomerCategory = new SelectList(customerCategory, "id", "text").ToList();
                ViewBag.Cities = new SelectList(cities, "id", "text").ToList();
                ViewBag.SONo=new SelectList(_dbContext.ARSaleOrders.Where(s => s.IsDeleted != true && s.CompanyId==companyId ).ToList(), "Id", "SaleOrderNo");
                ViewBag.DCNo = new SelectList(DCNo, "id", "text").ToList();
                ViewBag.InvoiceNo = new SelectList(InvoiceNo, "id", "text").ToList();
                ViewBag.IrnNo = new SelectList(IrnNo, "id", "text").ToList();
                ViewBag.PoNo = new SelectList(PoNo, "id", "text").ToList();
                ViewBag.Vendors = new SelectList(Vendors, "id", "text").ToList();
                ViewBag.Customers = new SelectList(Customers, "id", "text").ToList();
                ViewBag.CategoryType = configValues.GetConfigValues("AR", "Category Type", companyId);
                ViewBag.SecondItemCategory = new SelectList(SecondItemCategory, "id", "text").ToList();
                ViewBag.ProductTypeLOV = configValues.GetConfigValues("AR", "Product Type", Convert.ToInt32(companyId)).OrderByDescending(x => x.Text.Contains("Fresh Category")).ToList();
                ViewBag.Agent = new SelectList(Agent, "id", "text").ToList();
                ViewBag.Season= configValues.GetConfigValues("Inventory", "Season", Convert.ToInt32(companyId)).ToList();


                ViewBag.NavbarHeading = "Preview Reports";
                return View("~/Areas/Report/Views/AR/Template1.cshtml");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                throw;
            }
        }

        [HttpGet]
        public IActionResult Greige(string module)
        {
            try
            {
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                var suppliers = _dbContext.APSuppliers.Where((c => c.CompanyId == companyId))
                                               .Select(c => new
                                               {
                                                   id = c.Id,
                                                   text = c.Name
                                               })
                                               .OrderBy(a => a.text)
                                               .ToList();
                var ConstructionQuality = _dbContext.GRQuality.Where((c => c.CompanyId == companyId && c.IsDeleted != true))
                                              .Select(c => new
                                              {
                                                  id = c.Id,
                                                  text = c.Description
                                              })
                                              .OrderBy(a => a.text)
                                              .ToList();
                var ContractNo = _dbContext.GRWeavingContracts.Where((c => c.CompanyId == companyId && c.IsDeleted !=true))
                                            .Select(c => new
                                            {
                                                id = c.Id,
                                                text = c.TransactionNo
                                            })
                                            .OrderBy(a => a.text)
                                            .ToList();
                var IssueNo = _dbContext.GreigeIssuance.Where((c => c.CompanyId == companyId && c.IsDeleted != true))
                                            .Select(c => new
                                            {
                                                id = c.Id,
                                                text = c.TransactionNo
                                            })
                                            .OrderBy(a => a.text)
                                            .ToList();
                var ProductionOrderNo = _dbContext.ProductionOrders.Where((c => c.CompanyId == companyId && c.IsDeleted != true))
                                            .Select(c => new
                                            {
                                                id = c.Id,
                                                text = c.TransactionNo
                                            })
                                            .OrderBy(a => a.text)
                                            .ToList();
                var Brand = _dbContext.InvItemCategories.Where((c => c.CompanyId == companyId && c.IsDeleted != true && c.CategoryLevel==4))
                                            .Select(c => new
                                            {
                                                id = c.Id,
                                                text = c.Name
                                            })
                                            .OrderBy(a => a.text)
                                            .ToList();


                var configValues = new ConfigValues(_dbContext);

                ViewBag.Reports = new SelectList(_dbContext.AppReports.Where(s => s.IsListed == true && s.Module == module  && s.IsActive == true).ToList(), "Name", "Description");
                ViewBag.Suppliers = new SelectList(suppliers.OrderBy(x => x.id), "id", "text").ToList();
                ViewBag.ConstructionQuality = new SelectList(ConstructionQuality.OrderBy(x => x.id), "id", "text").ToList();
                ViewBag.ContractNo = new SelectList(ContractNo.OrderBy(x => x.id), "id", "text").ToList();
                ViewBag.IssueNo = new SelectList(IssueNo.OrderBy(x => x.id), "id", "text").ToList();
                ViewBag.ProductionOrderNo = new SelectList(ProductionOrderNo.OrderBy(x => x.id), "id", "text").ToList();
                ViewBag.Brand = new SelectList(Brand.OrderBy(x => x.id), "id", "text").ToList();
              


                ViewBag.NavbarHeading = "Preview Reports";
                return View("~/Areas/Report/Views/Greige/Greige.cshtml");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                throw;
            }
        }





        [HttpGet]
        public IActionResult AutoInvoice(string module)
        {
            try
            {
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                var customers = _dbContext.ARCustomers.Where((c => /*c.CompanyId == companyId &&*/ c.IsActive == true))
                                              .Select(c => new
                                              {
                                                  id = c.Id,
                                                  text = c.Name
                                              })
                                              .OrderBy(a => a.text)
                                              .ToList();

                var items = _dbContext.AppCompanySetups.Where(x => x.Name == "Local Finish ItemCode").FirstOrDefault().Value;

                ViewBag.SecondLevelCategoryLOV = new SelectList(from ac in _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.CategoryLevel == 2/* && x.CompanyId == companyId*/ && x.Code.StartsWith(items)).OrderBy(x => x.Code).ToList()
                                                                select new
                                                                {
                                                                    Id = ac.Id,
                                                                    Name = ac.Code + " - " + ac.Name
                                                                }, "Id", "Name");

                var customerCategory =  _dbContext.AppCompanyConfigs.Where(a => a.BaseId == 40 /*&& a.CompanyId == companyId*/).Select(c => new
                {
                    id = c.Id,
                    text = c.ConfigValue
                })
                .OrderBy(a => a.text).ToList();
                var cities = _dbContext.AppCities.Where(x=>x.CountryId==1).Select(c => new
                {
                    id = c.Id,
                    text = c.Name
                })
                .OrderBy(c => c.text)
                .ToList();
                var configValues = new ConfigValues(_dbContext);
                ViewBag.Reports = new SelectList(_dbContext.AppReports.Where(s => s.IsListed == true && s.Module == module  && s.IsActive == true).ToList(), "Name", "Description"); 
                ViewBag.customers = new SelectList(customers, "id", "text").ToList();
                ViewBag.CustomerCategory = new SelectList(customerCategory, "id", "text").ToList();
                ViewBag.Cities = new SelectList(cities, "id", "text").ToList();
                ViewBag.NavbarHeading = "Preview Reports";
                return View("~/Areas/Report/Views/AR/AutoInvoice.cshtml");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                throw;
            }
        }

        [HttpGet]
        public IActionResult Inventory(string module)
        {
            try
            {
                int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
                var configValues = new ConfigValues(_dbContext);

                var Items = _dbContext.InvItems.Where(a => a.CompanyId == companyId && a.IsDeleted == false)
                                               .Select(a => new
                                               {
                                                   id = a.Id,
                                                   code=a.Code,
                                                   text = string.Concat(a.Code, " - ", a.Name)
                                               });
                var Categories = _dbContext.InvItemCategories.Where(a => a.CompanyId == companyId && a.CategoryLevel==4).Select(a => new
                                     {
                                         id = a.Id,
                                         code = a.Code,
                                         text = string.Concat(a.Code, " - ", a.Name)
                                     });
                var CostCenters = _dbContext.CostCenter.Where((c => c.CompanyId == companyId))
                                              .Select(c => new
                                              {
                                                  id = c.Id,
                                                  text = c.Description
                                              })
                                              .OrderBy(a => a.text)
                                              .ToList();


                ViewBag.Reports = new SelectList(_dbContext.AppReports.Where(s => s.IsListed == true && s.Module == module  && s.IsActive == true).ToList(), "Name", "Description");
                // ViewBag.Items = new SelectList( Items.ToList(),"Id" , "text");
                ViewBag.ItemsFrom = new SelectList(Items.OrderBy(x =>x.code), "id","text").ToList(); //new SelectList(_dbContext.InvItems.Where(a => a.CompanyId == companyId && a.IsDeleted == false).ToList(), "Id", "Name");
                ViewBag.ItemsTo = new SelectList(Items.OrderByDescending(x => x.code), "id", "text").ToList();
                ViewBag.Categories = new SelectList(Categories, "id", "text").ToList();
                ViewBag.CostCenters = new SelectList(CostCenters, "id", "text").ToList();
                ViewBag.Department = new SelectList(_dbContext.GLDivision.Where(x => x.CompanyId == companyId && x.IsDeleted != true).ToList(), "Id", "Name");
                ViewBag.WareHouseTo = configValues.GetConfigValues("Inventory", "Ware House", companyId);
                ViewBag.WareHouseFrom = configValues.GetConfigValues("Inventory", "Ware House", companyId);
                ViewBag.NavbarHeading = "Preview Reports";
                //
                return View("~/Areas/Report/Views/INV/Inventory.cshtml");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                throw;
            }
        }

        [HttpGet]
        public IActionResult GetIRNNumber(DateTime startDate, DateTime endDate, string q = "")
        {
            var numberList = _dbContext.APIRN.Where(p => p.IRNNo.ToString().Contains(q)).Select(o => new
            {
                id = o.IRNNo,
                text = o.IRNNo
            })
                .Take(25)
                .ToList();
            return Ok(numberList);
        }
        [HttpGet]
        public IActionResult GetIGPNumber(DateTime startDate, DateTime endDate, string q = "")
        {
            var numberList = _dbContext.APIGP.Where(p => p.IGP.ToString().Contains(q)).Select(o => new
            {
                id = o.IGP,
                text = o.IGP
            })
                .Take(25)
                .ToList();
            return Ok(numberList);
        }
        [HttpGet]
        public IActionResult GetPONumber(DateTime startDate, DateTime endDate, string q = "")
        {
            var numberList = _dbContext.APPurchaseOrders.Where(p => p.PONo.ToString().Contains(q)).Select(o => new
            {
                id = o.PONo,
                text = o.PONo
            })
                .Take(25)
                .ToList();
            return Ok(numberList);
        }
        [HttpGet]
        public IActionResult GetGRNNumber(DateTime startDate, DateTime endDate, string q = "")
        {
            var numberList = _dbContext.APGRN.Where(p => p.GRNNO.ToString().Contains(q)).Select(o => new
            {
                id = o.GRNNO,
                text = o.GRNNO
            })
                .Take(25)
                .ToList();
            return Ok(numberList);
        }
        
        public IActionResult GetOrderRef()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var orderRef = _dbContext.APPurchaseRequisition.Where((c => c.CompanyId == companyId))
                                               .Select(c => new
                                               {
                                                   value = c.RefrenceNo,
                                                   text = c.RefrenceNo
                                               })
                                               .Take(25)
                                               .ToList();
            return Ok(orderRef);
        }
        public IActionResult GetStore()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var orderRef = _dbContext.GLDivision.Where((c => c.CompanyId == companyId))
                                               .Select(c => new
                                               {
                                                   value = c.Id,
                                                   text = c.Name
                                               })
                                               .Take(25)
                                               .ToList();
            return Ok(orderRef);
        } 
        public IActionResult GetIssueReturnNumber()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var orderRef = _dbContext.InvStoreIssues.Where((c => c.CompanyId == companyId))
                                               .Select(c => new
                                               {
                                                   value = c.IssueNo,
                                                   text = c.IssueNo
                                               })
                                               .Take(25)
                                               .ToList();
            return Ok(orderRef);
        }

        public IActionResult InsertToUserList()
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var orderRef = _dbContext.InvStoreIssues.Where((c => c.CompanyId == companyId))
                                               .Select(c => new
                                               {
                                                   value = c.IssueNo,
                                                   text = c.IssueNo
                                               })
                                               .Take(25)
                                               .ToList();
            return Ok(orderRef);
        }
    }
}
