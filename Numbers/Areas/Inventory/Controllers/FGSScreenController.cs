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

namespace Numbers.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    [Authorize]
    public class FGSScreenController : Controller
    {
        private readonly NumbersDbContext _dbContext;

        public FGSScreenController(NumbersDbContext dbContext)
        {
            _dbContext = dbContext;
            
        }
        public  IActionResult Index(int Id)
        {
            BaleInformation bale = new BaleInformation();
            var configValues = new ConfigValues(_dbContext);
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            ViewBag.ProductTypeLOV = configValues.GetConfigValues("AR", "Product Type", Convert.ToInt32(companyId)).OrderByDescending(x => x.Text.Contains("Fresh Category"));
            var items = _dbContext.AppCompanySetups.Where(x => x.Name == "Local Finish ItemCode").FirstOrDefault().Value;
            ViewBag.ProductionOrders = new SelectList((from prd in _dbContext.ProductionOrders.Where(x => x.CompanyId == companyId && x.IsDeleted !=true)
                                                      select new { 
                                                      Id=prd.Id,
                                                      Name=prd.TransactionNo}).OrderByDescending(x => x.Id),"Id","Name");
            ViewBag.ItemCategory2 = new SelectList(from ac in _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.CategoryLevel == 2 && x.CompanyId == companyId && x.Code.StartsWith(items)).OrderBy(x => x.Code).ToList()
                                                            select new
                                                            {
                                                                Id = ac.Id,
                                                                Name = ac.Code + " - " + ac.Name
                                                       }, "Id", "Name");
            //ViewBag.BaleType = configValues.GetConfigValues("Inventory", "Bale Type", companyId);
            ViewBag.BaleType=new SelectList((from b in _dbContext.AppCompanyConfigBases
                            join c in _dbContext.AppCompanyConfigs on b.Id equals c.BaseId
                            where c.CompanyId == companyId && b.CompanyId == companyId && c.IsActive && c.IsDeleted == false && b.Name == "Bale Type" && b.Module == "Inventory"
                            select c
                                  ).ToList(), "ConfigValue", "ConfigValue");
            bale.WareHouseLOV = new SelectList(_dbContext.AppCompanyConfigs.Where(x=>x.ConfigValue == "GD Sale Local"), "Id", "ConfigValue");

            //var Baletype = _dbContext.BaleInformation.ToList();
            //ViewBag.BaleType = new SelectList(Baletype, "BaleType", "BaleType").ToList();
            if (Id !=0)
            {
                List<BaleInformation> baledata = _dbContext.BaleInformation.Where(x => x.Id==Id).ToList();
                foreach (var value in baledata)
                {
                    bale.TransactionNo = value.TransactionNo;
                    bale.TransactionDate = value.TransactionDate;
                    bale.ItemCategory2 = value.ItemCategory2;
                    bale.ItemCategory3 = value.ItemCategory3;
                    bale.ItemCategory4 = value.ItemCategory4;
                    bale.ItemId = value.ItemId;
                    bale.BaleNumber = value.BaleNumber;
                    bale.Meters = value.Meters;
                    bale.BaleType = value.BaleType;
                    bale.ProductTypeId = value.ProductTypeId;
                    bale.WarehouseId = value.WarehouseId;
                    bale.ProductionOrderId = value.ProductionOrderId;
                    bale.ProductionOrderNo = value.ProductionOrderNo;
                    bale.ProductionQty = value.ProductionQty;
                    bale.TotalFGSQty = value.TotalFGSQty;
                    bale.LotNo = value.LotNo;
                    
                }
                ViewBag.ItemCategory3 = new SelectList(from ac in _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.CategoryLevel == 3 && x.CompanyId == companyId ).OrderBy(x => x.Code).ToList()
                                                       select new
                                                       {
                                                           Id = ac.Id,
                                                           Name = ac.Code + " - " + ac.Name
                                                       }, "Id", "Name");
                ViewBag.ItemCategory4 = new SelectList(from ac in _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.CategoryLevel == 4 && x.CompanyId == companyId ).OrderBy(x => x.Code).ToList()
                                                       select new
                                                       {
                                                           Id = ac.Id,
                                                           Name = ac.Code + " - " + ac.Name
                                                       }, "Id", "Name");
                ViewBag.Items = new SelectList(from ac in _dbContext.InvItems.Where(x => x.IsDeleted == false  && x.CompanyId == companyId).OrderBy(x => x.Code).ToList()
                                                       select new
                                                       {
                                                           Id = ac.Id,
                                                           Name = ac.Code + " - " + ac.Name
                                                       }, "Id", "Name");
                ViewBag.ProductionOrders = new SelectList(from prd in _dbContext.ProductionOrders.Where(x => x.CompanyId == companyId && x.IsDeleted != true)
                                                          select new
                                                          {
                                                              Id = prd.Id,
                                                              Name = prd.TransactionNo
                                                          }, "Id", "Name");

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
         

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;
            var CostCenterData = (from CostCenter in _dbContext.BaleInformation
                                      join category2 in _dbContext.InvItemCategories on CostCenter.ItemCategory2 equals category2.Id
                                      join category3 in _dbContext.InvItemCategories on CostCenter.ItemCategory3 equals category3.Id
                                      join category4 in _dbContext.InvItemCategories on CostCenter.ItemCategory4 equals category4.Id
                                      join item in _dbContext.InvItems on CostCenter.ItemId equals item.Id
                                      //join b in _dbContext.AppCompanyConfigs on Convert.ToInt32(CostCenter.BaleType) equals b.Id
                                              
                                  select new
                                  {
                                      CostCenter.Id,
                                      CostCenter.Meters,
                                      CostCenter.BaleNumber,
                                      BaleType= CostCenter.BaleType,
                                      CostCenter.TransactionNo,
                                      CostCenter.TransactionDate,
                                      ItemCategory2 = category2.Name,
                                      ItemCategory3 = category3.Name,
                                      ItemCategory4= category4.Name,
                                      ItemId=item.Name,
                                      CostCenter.ProductionOrderNo
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
            CostCenterData = !string.IsNullOrEmpty(searchCode) ? CostCenterData.Where(m => m.ProductionOrderNo.ToString().Contains(searchCode)) : CostCenterData;
            CostCenterData = !string.IsNullOrEmpty(searchCode) ? CostCenterData.Where(m => m.TransactionNo.ToString().Contains(searchCode)) : CostCenterData;
            CostCenterData = !string.IsNullOrEmpty(searchDate) ? CostCenterData.Where(m => m.TransactionDate.ToString(Helpers.CommonHelper.DateFormat).ToUpper().Contains(searchDate.ToUpper())) : CostCenterData;
            CostCenterData = !string.IsNullOrEmpty(searchLevel2) ? CostCenterData.Where(m => m.ItemCategory2.ToString().ToUpper().Contains(searchLevel2.ToUpper())) : CostCenterData;
            CostCenterData = !string.IsNullOrEmpty(searchLevel3) ? CostCenterData.Where(m => m.ItemCategory3.ToString().ToUpper().Contains(searchLevel3.ToUpper())) : CostCenterData;
            CostCenterData = !string.IsNullOrEmpty(searchLevel4) ? CostCenterData.Where(m => m.ItemCategory4.ToString().ToUpper().Contains(searchLevel4.ToUpper())) : CostCenterData;
            
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
            List<VMBaleInformation> Details = new List<VMBaleInformation>();
            foreach (var grp in data)
            {
                VMBaleInformation a = new VMBaleInformation();
                a.Id = grp.Id;
                a.TransactionDate = grp.TransactionDate.ToString(Helpers.CommonHelper.DateFormat);
                a.TransactionNo = grp.TransactionNo;
                a.Level2 = grp.ItemCategory2;
                a.level3 = grp.ItemCategory3;
                a.level4 = grp.ItemCategory4;
                a.ItemId = grp.ItemId;
                a.BaleMeter = Convert.ToString(grp.Meters);
                a.BaleType = grp.BaleType;
                a.BaleNo = grp.BaleNumber;
                a.ProductionOrderNo = grp.ProductionOrderNo;
                Details.Add(a);

            }

            var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = Details.OrderByDescending(x=>x.TransactionNo) };
            return Ok(jsonData);
        }
        [HttpPost]
        public async  Task<IActionResult> Create(BaleInformation model)
        {
            
            int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
            int resp_Id = HttpContext.Session.GetInt32("Resp_ID").Value;
            string userId = HttpContext.Session.GetString("UserId");
            var ProductType = _dbContext.AppCompanyConfigs.Where(x => x.Id == model.ProductTypeId).FirstOrDefault();
            var notation = "";
            if(!ProductType.ConfigValue.Contains("Fresh"))
            {
                notation = "C";
            }
            else
            {
                notation = _dbContext.AppCompanyConfigs.Where(x => x.ConfigValue == model.BaleType).FirstOrDefault().UserValue1;
            }

            
            if (model.Id == 0)
            {
                int? BaleCode = _dbContext.BaleInformation.Select(x => x.BaleCode).Max();
                int TransactionNo = _dbContext.BaleInformation.Select(x => x.TransactionNo).Max();
                var CurentYear = DateTime.Now.Year.ToString().Substring(2);
                 
                for (int i=1;i<=model.NoOfBale;i++)
                {
                    //string FNumber = string.Concat(notation, CurentYear, BaleCode + i );
                    string FNumber = string.Concat(notation, CurentYear, TransactionNo + i );
                    var bale = new BaleInformation();

                    bale.TransactionNo = TransactionNo + i ;
                    bale.TransactionDate = model.TransactionDate;
                    bale.ItemCategory2 = model.ItemCategory2;
                    bale.ItemCategory3 = model.ItemCategory3;
                    bale.ItemCategory4 = model.ItemCategory4;
                    bale.ItemId = model.ItemId;
                    bale.Design = ItemName(model.ItemId);
                    bale.DesignCode = ItemCode(model.ItemId);
                    bale.UsedFNumber = false;
                    bale.BaleNumber = FNumber;
                    bale.NoOfBale = model.NoOfBale;
                    bale.BaleCode = model.BaleCode;
                    bale.ProductTypeId = model.ProductTypeId;
                    bale.BaleType = model.BaleType;
                    bale.Meters = model.Meters;
                    bale.Bales = 1;
                    bale.TempBales = 1;
                    bale.WarehouseId = model.WarehouseId;
                    bale.ProductionOrderId = model.ProductionOrderId;
                    bale.ProductionQty = model.ProductionQty;
                    bale.BalProductionQty = model.BalProductionQty;
                    bale.ProductionOrderNo = model.ProductionOrderNo;
                    bale.LotNo = model.LotNo;
                    bale.TotalFGSQty = model.TotalFGSQty;
         
                    bale.Brand = "0";
                    _dbContext.BaleInformation.Add(bale);
                    
                }
                await _dbContext.SaveChangesAsync();


                TempData["error"] = "false";
                TempData["message"] = "Bale Information has been saved successfully.";              
            }
            else
            {
                var bale = new BaleInformation();
                bale.Id = model.Id;
                bale.TransactionNo = model.TransactionNo;
                bale.TransactionDate = model.TransactionDate;
                bale.ItemCategory2 = model.ItemCategory2;
                bale.ItemCategory3 = model.ItemCategory3;
                bale.ItemCategory4 = model.ItemCategory4;
                bale.ItemId = model.ItemId;
                bale.Design = ItemName(model.ItemId);
                bale.DesignCode = ItemCode(model.ItemId);
                bale.UsedFNumber = false;
                bale.BaleNumber = model.BaleNumber;
                bale.NoOfBale = model.NoOfBale;
                bale.BaleType = model.BaleType;
                bale.ProductTypeId = model.ProductTypeId;
                bale.Meters = model.Meters;
                bale.Bales = 1;
                bale.TempBales = 1;
                bale.WarehouseId = model.WarehouseId;
                bale.ProductionOrderId = model.ProductionOrderId;
                bale.ProductionQty = model.ProductionQty;
                bale.BalProductionQty = model.BalProductionQty;
                bale.LotNo = model.LotNo;
                bale.ProductionOrderNo = model.ProductionOrderNo;
                bale.TotalFGSQty = model.TotalFGSQty;
                bale.Brand = "0"; 

                _dbContext.BaleInformation.Update(bale);
              await  _dbContext.SaveChangesAsync();
                TempData["error"] = "false";
                TempData["message"] = "Bale Information has been update successfully.";
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
        public  IActionResult Delete(int id)
        {
            var bale = new BaleInformation { Id = id };// _dbContext.BaleInformation.Where(x => x.Id==id);


            if (bale !=null)
            {
                _dbContext.BaleInformation.Attach(bale);
                _dbContext.BaleInformation.Remove(bale);
                _dbContext.SaveChanges();

                TempData["error"] = "false";
                TempData["message"] = "Bale Information has been deleted successfully.";
            }
            else
            {
                TempData["error"] = "true";
                TempData["message"] = "Something went wrong.";
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult CheckBaleNo(string baleNo)
        {
            var bale = baleNo.ToUpper();
            if (!string.IsNullOrEmpty(baleNo))
            {
                var model = _dbContext.BaleInformation.FirstOrDefault(x => x.BaleNumber == bale);
                if (model != null)
                {
                    return Ok(new ErrorMessage { Status = false, Message = "This Bale No is already exist!" });
                }
                return Ok(null);
            }
            return Ok(null);
        }

        [HttpGet]
        public IActionResult GetProdutionData(int Id)
        {
           
           
                var model = (from a in _dbContext.ProductionOrders.Where(x=>x.Id == Id)
                             join b in _dbContext.ProductionOrderItems on a.Id equals b.ProductionOrderId
                             select new { 
                             ProductionQty = b.VersionQuantity,
                             BalProductionQty=0,
                                 ProductionOrderNo=a.TransactionNo
                             }).FirstOrDefault();
                //if (model != null)
                //{
                //    return Ok(new ErrorMessage { Status = false, Message = "This Bale No is already exist!" });
                //}
      
            return Ok(model);
        }

        //[HttpGet]
        //public IActionResult GetFourthCategoryFromPO(int productionOrderId, int ItemcategorySecondL)
        //{
        //    int companyId = HttpContext.Session.GetInt32("CompanyId").Value;
        //    var item = _dbContext.InvItemCategories.Where(x => x.IsDeleted == false && x.CategoryLevel == 3 && x.ParentId == Id /*&& x.CompanyId == companyId*/).OrderBy(x => x.Code).ToList().
        //                  Select(a => new
        //                  {
        //                      id = a.Id,
        //                      text = string.Concat(a.Code, " - ", a.Name)
        //                  });
        //    return Ok(item);

        //}
    }
}
