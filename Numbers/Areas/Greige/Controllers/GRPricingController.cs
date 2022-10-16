using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Numbers.Entity.Models;
using Numbers.Entity.ViewModels;
using Numbers.Repository.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Numbers.Areas.Greige.Controllers
{
    [Area("Greige")]
    [Authorize]
    public class GRPricingController : Controller
    {
        private readonly NumbersDbContext _dbContext;

        public GRPricingController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
            
        }
        public  IActionResult Index(int Id)
        {
            GRPricing bale = new GRPricing();
            var configValues = new ConfigValues(_dbContext);
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
           

            ViewBag.Item = new SelectList(from ac in _dbContext.GRQuality.Where(x => x.IsDeleted == false ).OrderBy(x => x.Id).ToList()
                                                            select new
                                                            {
                                                                Id = ac.Id,
                                                                Description = ac.Id + " - " + ac.Description
                                                       }, "Id", "Description");
        

            if (Id !=0)
            {
                List<GRPricing> baledata = _dbContext.GRPricing.Where(x => x.Id==Id).ToList();
                foreach (var value in baledata)
                {
                    bale.TransactionNo = value.TransactionNo;
                    bale.TransactionDate = value.TransactionDate;
                    bale.Description = value.Description;
                    bale.StartDate = value.StartDate;
                    bale.EndDate = value.EndDate;
                    bale.GreigeQualityId = value.GreigeQualityId;
                    bale.Remarks = value.Remarks;
                    bale.Id = value.Id;
                    bale.SaleRate = value.SaleRate;
  
                }

                ViewBag.Item = new SelectList(from ac in _dbContext.GRQuality.Where(x => x.IsDeleted == false).OrderBy(x => x.Id).ToList()
                                              select new
                                              {
                                                  Id = ac.Id,
                                                  Name = ac.Id + " - " + ac.Description
                                              }, "Id", "Description");

            }

            return View(bale);
        }


        public IActionResult GetBaleList()
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            //var searchValue = Request.Form["search[value]"].FirstOrDefault();
            var searchCode = Request.Form["columns[1][search][value]"].FirstOrDefault();
            var searchDate = Request.Form["columns[2][search][value]"].FirstOrDefault();
            var searchLevel2 = Request.Form["columns[3][search][value]"].FirstOrDefault();
            var searchLevel3 = Request.Form["columns[4][search][value]"].FirstOrDefault();
            var searchLevel4 = Request.Form["columns[5][search][value]"].FirstOrDefault();
            var searchItem = Request.Form["columns[6][search][value]"].FirstOrDefault();
            var searchType = Request.Form["columns[7][search][value]"].FirstOrDefault();
            var searchBaleNo = Request.Form["columns[8][search][value]"].FirstOrDefault();
            var searchMtr = Request.Form["columns[9][search][value]"].FirstOrDefault();
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;
            var CostCenterData = (from p in _dbContext.GRPricing.Where(x => x.CompanyId == companyId && x.IsClosed == false)
                                  join category in _dbContext.GRQuality on p.GreigeQualityId equals category.Id
                                  join itemcode in _dbContext.InvItems on category.ItemId equals itemcode.Id

                                  select new
                                  {
                                      Id = p.Id,
                                      p,
                                      
                                      category.Description,
                                      itemcode.Code

                                  });
            // var CostCenterData = _dbContext.BaleInformation.Where(x => x.TransactionNo > 0);
            if (!string.IsNullOrEmpty(sortColumn) && (!string.IsNullOrEmpty(sortColumnDirection)))
            {
                CostCenterData = CostCenterData.OrderBy(sortColumn + " " + sortColumnDirection);
            }
            //if (!string.IsNullOrEmpty(searchValue))
            //{
            //    CostCenterData = CostCenterData.Where(m => m.Code.ToString().Contains(searchValue)
            //                                    || m.Date.ToString(CommonHelper.DateFormat).ToUpper().Contains(searchValue.ToUpper())
            //                                    || m.Description.ToString().ToUpper().Contains(searchValue.ToUpper())
            //                                    || m.Status.ToString().ToUpper().Contains(searchValue.ToUpper())
            //                                    || _dbContext.GLDivision.FirstOrDefault(x => x.Id == Convert.ToInt32(m.DivisionId)).Name.ToUpper().Contains(searchValue.ToUpper())
            //                                    || _dbContext.GLSubDivision.FirstOrDefault(x => x.Id == Convert.ToInt32(m.SubDivisionId)).Name.ToUpper().Contains(searchValue.ToUpper())
            //                                  );

            //}
            CostCenterData = !string.IsNullOrEmpty(searchCode) ? CostCenterData.Where(m => m.p.TransactionNo.ToString().Contains(searchCode)) : CostCenterData;
            CostCenterData = !string.IsNullOrEmpty(searchDate) ? CostCenterData.Where(m => m.p.StartDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchDate.ToUpper())) : CostCenterData;
            CostCenterData = !string.IsNullOrEmpty(searchDate) ? CostCenterData.Where(m => m.p.EndDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchDate.ToUpper())) : CostCenterData;
            CostCenterData = !string.IsNullOrEmpty(searchLevel2) ? CostCenterData.Where(m => m.p.SaleRate.ToString().ToUpper().Contains(searchLevel2.ToUpper())) : CostCenterData;
            CostCenterData = !string.IsNullOrEmpty(searchLevel3) ? CostCenterData.Where(m => m.Description.ToString().ToUpper().Contains(searchLevel3.ToUpper())) : CostCenterData;
            CostCenterData = !string.IsNullOrEmpty(searchLevel3) ? CostCenterData.Where(m => m.Code.ToString().ToUpper().Contains(searchLevel3.ToUpper())) : CostCenterData;

            recordsTotal = CostCenterData.Count();
            var data = CostCenterData.ToList();
            if (pageSize == -1)
            {
                data = CostCenterData.Skip(skip).Take(recordsTotal == 0 ? 1 : pageSize == -1 ? recordsTotal : 1).ToList();
            }
            else
            {
                data = CostCenterData.Skip(skip).Take(pageSize).ToList();
            }
            List<GRPricingVM> Details = new List<GRPricingVM>();
            foreach (var grp in data)
            {
                GRPricingVM a = new GRPricingVM();
                a.GriegeRequisition = grp.p;
                a.Id = grp.p.Id;
                a.Code = grp.Code;
                a.Description = grp.Description;
                a.SaleRate = grp.p.SaleRate;
                a.UpdatedBy = grp.p.StartDate.ToString("dd-MMM-yyyy");
                a.CreatedBy = grp.p.EndDate.ToString("dd-MMM-yyyy");
                a.TransactionNo = grp.p.TransactionNo;
                a.TransactionDate = grp.p.TransactionDate;
                Details.Add(a);

            }

            var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = Details.OrderByDescending(x => x.TransactionNo) };
            return Ok(jsonData);
        }
        [HttpPost]
        public async  Task<IActionResult> Create(GRPricing model)
        {
            
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            string userId = HttpContext.Session.GetString("UserId");
            if (model.Id == 0)
            {
                
               int TransactionNo = _dbContext.GRPricing.Select(x => x.TransactionNo).Max();
                    var bale = new GRPricing();
                   // bale.TransactionNo = 1 ;
                   bale.TransactionNo = TransactionNo +1 ;
                    bale.TransactionDate = model.TransactionDate;
                bale.IsClosed = false;
                bale.Remarks = model.Remarks ?? "";
                bale.SaleRate = model.SaleRate;
                bale.StartDate = model.StartDate;
                bale.EndDate = model.EndDate;
                bale.GreigeQualityId = model.GreigeQualityId;
                bale.Description = "";
                bale.CreatedBy = userId;
                bale.CreatedDate = DateTime.Now;
                bale.UpdatedBy = model.UpdatedBy ?? "";
                bale.UpdatedDate = model.UpdatedDate;
                bale.CompanyId = companyId;
                    _dbContext.GRPricing.Add(bale);
                await _dbContext.SaveChangesAsync();


                TempData["error"] = "false";
                TempData["message"] = "Pricing for "+ bale.TransactionNo +" has been saved successfully.";              
            }
            else
            {
                var bale = new GRPricing();
                bale.Id = model.Id;
                bale.TransactionNo = model.TransactionNo;
                bale.TransactionDate = model.TransactionDate;
                bale.IsClosed = false;
                bale.Remarks = model.Remarks;
                bale.SaleRate = model.SaleRate;
                bale.StartDate = model.StartDate;
                bale.EndDate = model.EndDate;
                bale.GreigeQualityId = model.GreigeQualityId;
                bale.Description = "";
                bale.CreatedBy = model.CreatedBy;
                bale.CreatedDate = model.CreatedDate;
                bale.UpdatedDate = DateTime.Now;
                bale.UpdatedBy = userId;
                bale.CompanyId = companyId;
                _dbContext.GRPricing.Update(bale);
              await  _dbContext.SaveChangesAsync();
                TempData["error"] = "false";
                TempData["message"] = "Pricing for " + bale.TransactionNo + " has been update successfully.";
            }
            return RedirectToAction("Index");
        }

        public string ItemName(int? Itemid)
        {
            string Item="";
           // var ItemName = _dbContext.InvItems.Where(x => x.Id == Itemid).ToList();

            var ItemName = (from ac in _dbContext.InvItems.Where(x => x.IsDeleted == false && x.Id == Itemid).ToList()
                           select new
                           {     
                             ac.Name
                           });
            foreach (var grp in ItemName)
            {
                Item = (grp.Name).ToString();
            }
            return Item;
        }

        public string ItemCode(int? Itemid)
        {
            string Item="";
            // var ItemName = _dbContext.InvItems.Where(x => x.Id == Itemid).ToList();

            var ItemName = (from ac in _dbContext.InvItems.Where(x => x.IsDeleted == false && x.Id == Itemid).ToList()
                            select new
                            {
                              ac.Code
                            });

            foreach (var grp in ItemName)
            {
                Item = (grp.Code).ToString();
            }
            return Item;
        }
        public  IActionResult Close(int id)
        {
            var bale = new GRPricing { Id = id };// _dbContext.BaleInformation.Where(x => x.Id==id);


            if (bale !=null)
            {
                bale.IsClosed = true;
                _dbContext.GRPricing.Update(bale);
                _dbContext.SaveChanges();

                TempData["error"] = "false";
                TempData["message"] = "Pricing has been Closed successfully.";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong.";
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult CheckPricing(int Id, string StartDate , string EndDate)
        {
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            var ItemPricingDetails = _dbContext.GRPricing.Where(p => p.GreigeQualityId == Id && p.CompanyId == companyId);
            var StDate = Convert.ToDateTime(StartDate).Date;
            var EdDate = Convert.ToDateTime(EndDate).Date;
            if (ItemPricingDetails.Any(p => p.IsClosed != true && StDate <= p.StartDate.Date || EdDate<=p.EndDate.Date || StDate <=p.EndDate.Date ))
            {
                
                return Json(true);
               
            }
            else
            {
                return Json(false);
            }
            
        }
    }
}
